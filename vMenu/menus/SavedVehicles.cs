﻿using System.Collections.Generic;
using System.Linq;

using ScaleformUI.Menu;

namespace vMenuClient.menus
{
    public class SavedVehicles
    {
        // Variables
        private UIMenu menu;
        private readonly UIMenu selectedVehicleMenu = new("Manage Vehicle", "Manage this saved vehicle.");
        private readonly UIMenu unavailableVehiclesMenu = new("Missing Vehicles", "Unavailable Saved Vehicles");
        private Dictionary<string, VehicleInfo> savedVehicles = new();
        private readonly List<UIMenu> subMenus = new();
        private Dictionary<UIMenuItem, KeyValuePair<string, VehicleInfo>> svMenuItems = new();
        private KeyValuePair<string, VehicleInfo> currentlySelectedVehicle = new();
        private int deleteButtonPressedCount = 0;
        private int replaceButtonPressedCount = 0;

        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            string menuTitle = "Saved Vehicles";
            #region Create menus and submenus
            // Create the menu.
            menu = new UIMenu(menuTitle, "Manage Saved Vehicles");

            UIMenuItem saveVehicle = new UIMenuItem("Save Current Vehicle", "Save the vehicle you are currently sitting in.");
            menu.AddItem(saveVehicle);
            saveVehicle.SetLeftBadge(BadgeIcon.CAR);

            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == saveVehicle)
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        SaveVehicle();
                    }
                    else
                    {
                        Notify.Error("You are currently not in any vehicle. Please enter a vehicle before trying to save it.");
                    }
                }
            };

            for (int i = 0; i < 23; i++)
            {
                UIMenu categoryMenu = new UIMenu("Saved Vehicles", GetLabelText($"VEH_CLASS_{i}"));

                UIMenuItem categoryButton = new UIMenuItem(GetLabelText($"VEH_CLASS_{i}"), $"All saved vehicles from the {GetLabelText($"VEH_CLASS_{i}")} category.");
                subMenus.Add(categoryMenu);
                menu.AddItem(categoryButton);
                categoryButton.SetRightLabel("→→→");
                categoryButton.Activated += async (a, b) => await a.SwitchTo(categoryMenu, 0, true);

                categoryMenu.OnMenuClose += (sender) =>
                {
                    UpdateMenuAvailableCategories();
                };

                categoryMenu.OnItemSelect += (sender, item, index) =>
                {
                    UpdateSelectedVehicleMenu(item, sender);
                };
            }

            UIMenuItem unavailableModels = new UIMenuItem("Unavailable Saved Vehicles", "These vehicles are currently unavailable because the models are not present in the game. These vehicles are most likely not being streamed from the server.");
            unavailableModels.SetRightLabel("→→→");

            menu.AddItem(unavailableModels);
            unavailableModels.Activated += async (a, b) => await a.SwitchTo(unavailableVehiclesMenu, 0, true);

            UIMenuItem spawnVehicle = new UIMenuItem("Spawn Vehicle", "Spawn this saved vehicle.");
            UIMenuItem renameVehicle = new UIMenuItem("Rename Vehicle", "Rename your saved vehicle.");
            UIMenuItem replaceVehicle = new UIMenuItem("~r~Replace Vehicle", "Your saved vehicle will be replaced with the vehicle you are currently sitting in. ~r~Warning: this can NOT be undone!");
            UIMenuItem deleteVehicle = new UIMenuItem("~r~Delete Vehicle", "~r~This will delete your saved vehicle. Warning: this can NOT be undone!");
            selectedVehicleMenu.AddItem(spawnVehicle);
            selectedVehicleMenu.AddItem(renameVehicle);
            selectedVehicleMenu.AddItem(replaceVehicle);
            selectedVehicleMenu.AddItem(deleteVehicle);

            selectedVehicleMenu.OnMenuOpen += (sender, data) =>
            {
                spawnVehicle.SetRightLabel("(" + GetDisplayNameFromVehicleModel(currentlySelectedVehicle.Value.model).ToLower() + ")");
            };

            selectedVehicleMenu.OnMenuClose += (sender) =>
            {
                deleteButtonPressedCount = 0;
                deleteVehicle.SetRightLabel("");
                replaceButtonPressedCount = 0;
                replaceVehicle.SetRightLabel("");
            };

            selectedVehicleMenu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == spawnVehicle)
                {
                    if (MainMenu.VehicleSpawnerMenu != null)
                    {
                        await SpawnVehicle(currentlySelectedVehicle.Value.model, MainMenu.VehicleSpawnerMenu.SpawnInVehicle, MainMenu.VehicleSpawnerMenu.ReplaceVehicle, false, vehicleInfo: currentlySelectedVehicle.Value, saveName: currentlySelectedVehicle.Key.Substring(4));
                    }
                    else
                    {
                        await SpawnVehicle(currentlySelectedVehicle.Value.model, true, true, false, vehicleInfo: currentlySelectedVehicle.Value, saveName: currentlySelectedVehicle.Key.Substring(4));
                    }
                }
                else if (item == renameVehicle)
                {
                    string newName = await GetUserInput(windowTitle: "Enter a new name for this vehicle.", maxInputLength: 30);
                    if (string.IsNullOrEmpty(newName))
                    {
                        Notify.Error(CommonErrors.InvalidInput);
                    }
                    else
                    {
                        if (StorageManager.SaveVehicleInfo("veh_" + newName, currentlySelectedVehicle.Value, false))
                        {
                            DeleteResourceKvp(currentlySelectedVehicle.Key);
                            while (!selectedVehicleMenu.Visible)
                            {
                                await BaseScript.Delay(0);
                            }
                            Notify.Success("Your vehicle has successfully been renamed.");
                            UpdateMenuAvailableCategories();
                            selectedVehicleMenu.GoBack();
                            currentlySelectedVehicle = new KeyValuePair<string, VehicleInfo>(); // clear the old info
                        }
                        else
                        {
                            Notify.Error("This name is already in use or something unknown failed. Contact the server owner if you believe something is wrong.");
                        }
                    }
                }
                else if (item == replaceVehicle)
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        if (replaceButtonPressedCount == 0)
                        {
                            replaceButtonPressedCount = 1;
                            item.SetRightLabel("Press again to confirm.");
                            Notify.Alert("Are you sure you want to replace this vehicle? Press the button again to confirm.");
                        }
                        else
                        {
                            replaceButtonPressedCount = 0;
                            item.SetRightLabel("");
                            SaveVehicle(currentlySelectedVehicle.Key.Substring(4));
                            selectedVehicleMenu.GoBack();
                            Notify.Success("Your saved vehicle has been replaced with your current vehicle.");
                        }
                    }
                    else
                    {
                        Notify.Error("You need to be in a vehicle before you can replace your old vehicle.");
                    }
                }
                else if (item == deleteVehicle)
                {
                    if (deleteButtonPressedCount == 0)
                    {
                        deleteButtonPressedCount = 1;
                        item.SetRightLabel("Press again to confirm.");
                        Notify.Alert("Are you sure you want to delete this vehicle? Press the button again to confirm.");
                    }
                    else
                    {
                        deleteButtonPressedCount = 0;
                        item.SetRightLabel("");
                        DeleteResourceKvp(currentlySelectedVehicle.Key);
                        UpdateMenuAvailableCategories();
                        selectedVehicleMenu.GoBack();
                        Notify.Success("Your saved vehicle has been deleted.");
                    }
                }
                if (item != deleteVehicle) // if any other button is pressed, restore the delete vehicle button pressed count.
                {
                    deleteButtonPressedCount = 0;
                    deleteVehicle.SetRightLabel("");
                }
                if (item != replaceVehicle)
                {
                    replaceButtonPressedCount = 0;
                    replaceVehicle.SetRightLabel("");
                }
            };
            //unavailableVehiclesMenu.InstructionalButtons.Add(Control.FrontendDelete, "Delete Vehicle!");

            /*
            unavailableVehiclesMenu.ButtonPressHandlers.Add(new UIMenu.ButtonPressHandler(Control.FrontendDelete, Menu.ControlPressCheckType.JUST_RELEASED, new Action<Menu, Control>((m, c) =>
            {
                if (m.Size > 0)
                {
                    var index = m.CurrentIndex;
                    if (index < m.Size)
                    {
                        var item = m.MenuItems.Find(i => i.Index == index);
                        if (item != null && item.ItemData is KeyValuePair<string, VehicleInfo> sd)
                        {
                            if (item.RightLabel == "~r~Are you sure?")
                            {
                                Log("Unavailable saved vehicle deleted, data: " + JsonConvert.SerializeObject(sd));
                                DeleteResourceKvp(sd.Key);
                                unavailableVehiclesMenu.GoBack();
                                UpdateMenuAvailableCategories();
                            }
                            else
                            {
                                item.SetRightLabel("~r~Are you sure?");
                            }
                        }
                        else
                        {
                            Notify.Error("Somehow this vehicle could not be found.");
                        }
                    }
                    else
                    {
                        Notify.Error("You somehow managed to trigger deletion of a menu item that doesn't exist, how...?");
                    }
                }
                else
                {
                    Notify.Error("There are currrently no unavailable vehicles to delete!");
                }
            }), true));
            */

            void ResetAreYouSure()
            {
                foreach (UIMenuItem i in unavailableVehiclesMenu.MenuItems)
                {
                    if (i.ItemData is KeyValuePair<string, VehicleInfo> vd)
                    {
                        i.SetRightLabel($"({vd.Value.name})");
                    }
                }
            }
            unavailableVehiclesMenu.OnMenuClose += (sender) => ResetAreYouSure();
            unavailableVehiclesMenu.OnIndexChange += (sender, newIndex) => ResetAreYouSure();

            #endregion
        }


        /// <summary>
        /// Updates the selected vehicle.
        /// </summary>
        /// <param name="selectedItem"></param>
        /// <param name="parentMenu"></param>
        /// <returns>A bool, true if successfull, false if unsuccessfull</returns>
        private bool UpdateSelectedVehicleMenu(UIMenuItem selectedItem, UIMenu parentMenu)
        {
            if (!svMenuItems.ContainsKey(selectedItem))
            {
                Notify.Error("In some very strange way, you've managed to select a button, that does not exist according to this list. So your vehicle could not be loaded. :( Maybe your save files are broken?");
                return false;
            }
            KeyValuePair<string, VehicleInfo> vehInfo = svMenuItems[selectedItem];
            selectedVehicleMenu.Subtitle = $"{vehInfo.Key.Substring(4)} ({vehInfo.Value.name})";
            currentlySelectedVehicle = vehInfo;
            parentMenu.SwitchTo(selectedVehicleMenu);
            return true;
        }


        /// <summary>
        /// Updates the available vehicle category list.
        /// </summary>
        public void UpdateMenuAvailableCategories()
        {
            savedVehicles = GetSavedVehicles();
            svMenuItems = new Dictionary<UIMenuItem, KeyValuePair<string, VehicleInfo>>();

            for (int i = 1; i < GetMenu().Size - 1; i++)
            {
                if (savedVehicles.Any(a => GetVehicleClassFromName(a.Value.model) == i - 1 && IsModelInCdimage(a.Value.model)))
                {
                    GetMenu().MenuItems[i].SetRightBadge(BadgeIcon.NONE);
                    GetMenu().MenuItems[i].SetRightLabel("→→→");
                    GetMenu().MenuItems[i].Enabled = true;
                    GetMenu().MenuItems[i].Description = $"All saved vehicles from the {GetMenu().MenuItems[i].Label} category.";
                }
                else
                {
                    GetMenu().MenuItems[i].SetRightLabel("");
                    GetMenu().MenuItems[i].SetRightBadge(BadgeIcon.LOCK);
                    GetMenu().MenuItems[i].Enabled = false;
                    GetMenu().MenuItems[i].Description = $"You do not have any saved vehicles that belong to the {GetMenu().MenuItems[i].Label} category.";
                }
            }

            // Check if the items count will be changed. If there are less cars than there were before, one probably got deleted
            // so in that case we need to refresh the index of that menu just to be safe. If not, keep the index where it is for improved
            // usability of the menu.
            foreach (UIMenu m in subMenus)
            {
                int size = m.Size;
                int vclass = subMenus.IndexOf(m);

                int count = savedVehicles.Count(a => GetVehicleClassFromName(a.Value.model) == vclass);
            }

            foreach (UIMenu m in subMenus)
            {
                // Clear items but don't reset the index because we can guarantee that the index won't be out of bounds.
                // this is the case because of the loop above where we reset the index if the items count changes.
                m.Clear();
            }

            // Always clear this index because it's useless anyway and it's safer.
            unavailableVehiclesMenu.Clear();

            foreach (KeyValuePair<string, VehicleInfo> sv in savedVehicles)
            {
                if (IsModelInCdimage(sv.Value.model))
                {
                    int vclass = GetVehicleClassFromName(sv.Value.model);
                    UIMenu menu = subMenus[vclass];

                    UIMenuItem savedVehicleBtn = new UIMenuItem(sv.Key.Substring(4), $"Manage this saved vehicle.")
                    {
                        Label = $"{sv.Key.Substring(4)} ({sv.Value.name}) →→→"
                    };
                    menu.AddItem(savedVehicleBtn);

                    svMenuItems.Add(savedVehicleBtn, sv);
                }
                else
                {
                    UIMenuItem missingVehItem = new UIMenuItem(sv.Key.Substring(4), "This model could not be found in the game files. Most likely because this is an addon vehicle and it's currently not streamed by the server.")
                    {
                        Enabled = false,
                        ItemData = sv
                    };
                    missingVehItem.SetRightLabel("(" + sv.Value.name + ")");
                    missingVehItem.SetLeftBadge(BadgeIcon.LOCK);

                    //SetResourceKvp(sv.Key + "_tmp_dupe", JsonConvert.SerializeObject(sv.Value));
                    unavailableVehiclesMenu.AddItem(missingVehItem);
                }
            }

            foreach (UIMenu m in subMenus)
            {
                m.MenuItems.Sort((A, B) =>
                {
                    return A.Label.ToLower().CompareTo(B.Label.ToLower());
                });
            }
        }

        /// <summary>
        /// Create the menu if it doesn't exist, and then returns it.
        /// </summary>
        /// <returns>The Menu</returns>
        public UIMenu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }
    }
}
