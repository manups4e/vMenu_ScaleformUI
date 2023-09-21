using System.Collections.Generic;

using ScaleformUI.Menu;

using vMenuClient.data;

namespace vMenuClient.menus
{
    public class WeaponLoadouts
    {
        // Variables
        private UIMenu menu = null;
        private readonly UIMenu SavedLoadoutsMenu = new("Saved Loadouts", "saved weapon loadouts list");
        private readonly UIMenu ManageLoadoutMenu = new("Mange Loadout", "Manage saved weapon loadout");
        public bool WeaponLoadoutsSetLoadoutOnRespawn { get; private set; } = UserDefaults.WeaponLoadoutsSetLoadoutOnRespawn;

        private readonly Dictionary<string, List<ValidWeapon>> SavedWeapons = new();

        public static Dictionary<string, List<ValidWeapon>> GetSavedWeapons()
        {
            var handle = StartFindKvp("vmenu_string_saved_weapon_loadout_");
            var saves = new Dictionary<string, List<ValidWeapon>>();
            while (true)
            {
                var kvp = FindKvp(handle);
                if (string.IsNullOrEmpty(kvp))
                {
                    break;
                }
                saves.Add(kvp, JsonConvert.DeserializeObject<List<ValidWeapon>>(GetResourceKvpString(kvp)));
            }
            EndFindKvp(handle);
            return saves;
        }

        private string SelectedSavedLoadoutName { get; set; } = "";
        // vmenu_temp_weapons_loadout_before_respawn
        // vmenu_string_saved_weapon_loadout_

        /// <summary>
        /// Returns the saved weapons list, as well as sets the <see cref="SavedWeapons"/> variable.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<ValidWeapon>> RefreshSavedWeaponsList()
        {
            if (SavedWeapons.Count > 0)
            {
                SavedWeapons.Clear();
            }

            var handle = StartFindKvp("vmenu_string_saved_weapon_loadout_");
            var saves = new List<string>();
            while (true)
            {
                var kvp = FindKvp(handle);
                if (string.IsNullOrEmpty(kvp))
                {
                    break;
                }
                saves.Add(kvp);
            }
            EndFindKvp(handle);

            foreach (var save in saves)
            {
                SavedWeapons.Add(save, JsonConvert.DeserializeObject<List<ValidWeapon>>(GetResourceKvpString(save)));
            }

            return SavedWeapons;
        }

        /// <summary>
        /// Creates the menu if it doesn't exist yet and sets the event handlers.
        /// </summary>
        public void CreateMenu()
        {
            menu = new UIMenu(Game.Player.Name, "weapon loadouts management");


            var saveLoadout = new UIMenuItem("Save Loadout", "Save your current weapons into a new loadout slot.");
            var savedLoadoutsMenuBtn = new UIMenuItem("Manage Loadouts", "Manage saved weapon loadouts.");
            savedLoadoutsMenuBtn.SetRightLabel("→→→");
            var enableDefaultLoadouts = new UIMenuCheckboxItem("Restore Default Loadout On Respawn", WeaponLoadoutsSetLoadoutOnRespawn, "If you've set a loadout as default loadout, then your loadout will be equipped automatically whenever you (re)spawn.");

            menu.AddItem(saveLoadout);
            menu.AddItem(savedLoadoutsMenuBtn);
            savedLoadoutsMenuBtn.Activated += async (a, b) => await a.SwitchTo(SavedLoadoutsMenu, 0, true);
            if (IsAllowed(Permission.WLEquipOnRespawn))
            {
                menu.AddItem(enableDefaultLoadouts);

                menu.OnCheckboxChange += (sender, checkbox, _checked) =>
                {
                    WeaponLoadoutsSetLoadoutOnRespawn = _checked;
                };
            }


            void RefreshSavedWeaponsMenu()
            {
                var oldCount = SavedLoadoutsMenu.Size;
                SavedLoadoutsMenu.Clear();

                RefreshSavedWeaponsList();

                foreach (var sw in SavedWeapons)
                {
                    var btn = new UIMenuItem(sw.Key.Replace("vmenu_string_saved_weapon_loadout_", ""), "Click to manage this loadout.");
                    btn.SetRightLabel("→→→");
                    SavedLoadoutsMenu.AddItem(btn);
                    btn.Activated += async (a, b) => await a.SwitchTo(ManageLoadoutMenu, 0, true);
                }
            }


            var spawnLoadout = new UIMenuItem("Equip Loadout", "Spawn this saved weapons loadout. This will remove all your current weapons and replace them with this saved slot.");
            var renameLoadout = new UIMenuItem("Rename Loadout", "Rename this saved loadout.");
            var cloneLoadout = new UIMenuItem("Clone Loadout", "Clones this saved loadout to a new slot.");
            var setDefaultLoadout = new UIMenuItem("Set As Default Loadout", "Set this loadout to be your default loadout for whenever you (re)spawn. This will override the 'Restore Weapons' option inside the Misc Settings menu. You can toggle this option in the main Weapon Loadouts menu.");
            var replaceLoadout = new UIMenuItem("~r~Replace Loadout", "~r~This replaces this saved slot with the weapons that you currently have in your inventory. This action can not be undone!");
            var deleteLoadout = new UIMenuItem("~r~Delete Loadout", "~r~This will delete this saved loadout. This action can not be undone!");

            if (IsAllowed(Permission.WLEquip))
            {
                ManageLoadoutMenu.AddItem(spawnLoadout);
            }

            ManageLoadoutMenu.AddItem(renameLoadout);
            ManageLoadoutMenu.AddItem(cloneLoadout);
            ManageLoadoutMenu.AddItem(setDefaultLoadout);
            ManageLoadoutMenu.AddItem(replaceLoadout);
            ManageLoadoutMenu.AddItem(deleteLoadout);

            // Save the weapons loadout.
            menu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == saveLoadout)
                {
                    var name = await GetUserInput("Enter a save name", 30);
                    if (string.IsNullOrEmpty(name))
                    {
                        Notify.Error(CommonErrors.InvalidInput);
                    }
                    else
                    {
                        if (SavedWeapons.ContainsKey("vmenu_string_saved_weapon_loadout_" + name))
                        {
                            Notify.Error(CommonErrors.SaveNameAlreadyExists);
                        }
                        else
                        {
                            if (SaveWeaponLoadout("vmenu_string_saved_weapon_loadout_" + name))
                            {
                                Log("saveweapons called from menu select (save loadout button)");
                                Notify.Success($"Your weapons have been saved as ~g~<C>{name}</C>~s~.");
                            }
                            else
                            {
                                Notify.Error(CommonErrors.UnknownError);
                            }
                        }
                    }
                }
            };

            // manage spawning, renaming, deleting etc.
            ManageLoadoutMenu.OnItemSelect += async (sender, item, index) =>
            {
                if (SavedWeapons.ContainsKey(SelectedSavedLoadoutName))
                {
                    var weapons = SavedWeapons[SelectedSavedLoadoutName];

                    if (item == spawnLoadout) // spawn
                    {
                        await SpawnWeaponLoadoutAsync(SelectedSavedLoadoutName, false, true, false);
                    }
                    else if (item == renameLoadout || item == cloneLoadout) // rename or clone
                    {
                        var newName = await GetUserInput("Enter a save name", SelectedSavedLoadoutName.Replace("vmenu_string_saved_weapon_loadout_", ""), 30);
                        if (string.IsNullOrEmpty(newName))
                        {
                            Notify.Error(CommonErrors.InvalidInput);
                        }
                        else
                        {
                            if (SavedWeapons.ContainsKey("vmenu_string_saved_weapon_loadout_" + newName))
                            {
                                Notify.Error(CommonErrors.SaveNameAlreadyExists);
                            }
                            else
                            {
                                SetResourceKvp("vmenu_string_saved_weapon_loadout_" + newName, JsonConvert.SerializeObject(weapons));
                                Notify.Success($"Your weapons loadout has been {(item == renameLoadout ? "renamed" : "cloned")} to ~g~<C>{newName}</C>~s~.");

                                if (item == renameLoadout)
                                {
                                    DeleteResourceKvp(SelectedSavedLoadoutName);
                                }

                                ManageLoadoutMenu.GoBack();
                            }
                        }
                    }
                    else if (item == setDefaultLoadout) // set as default
                    {
                        SetResourceKvp("vmenu_string_default_loadout", SelectedSavedLoadoutName);
                        Notify.Success("This is now your default loadout.");
                        item.SetLeftBadge(BadgeIcon.TICK);
                    }
                    else if (item == replaceLoadout) // replace
                    {
                        if (replaceLoadout.RightLabel == "Are you sure?")
                        {
                            replaceLoadout.SetRightLabel("");
                            SaveWeaponLoadout(SelectedSavedLoadoutName);
                            Log("save weapons called from replace loadout");
                            Notify.Success("Your saved loadout has been replaced with your current weapons.");
                        }
                        else
                        {
                            replaceLoadout.SetRightLabel("Are you sure?");
                        }
                    }
                    else if (item == deleteLoadout) // delete
                    {
                        if (deleteLoadout.RightLabel == "Are you sure?")
                        {
                            deleteLoadout.SetRightLabel("");
                            DeleteResourceKvp(SelectedSavedLoadoutName);
                            ManageLoadoutMenu.GoBack();
                            Notify.Success("Your saved loadout has been deleted.");
                        }
                        else
                        {
                            deleteLoadout.SetRightLabel("Are you sure?");
                        }
                    }
                }
            };

            // Reset the 'are you sure' states.
            ManageLoadoutMenu.OnMenuClose += (sender) =>
            {
                deleteLoadout.SetRightLabel("");
                renameLoadout.SetRightLabel("");
            };
            // Reset the 'are you sure' states.
            ManageLoadoutMenu.OnIndexChange += (sender, newIndex) =>
            {
                deleteLoadout.SetRightLabel("");
                renameLoadout.SetRightLabel("");
            };

            // Refresh the spawned weapons menu whenever this menu is opened.
            SavedLoadoutsMenu.OnMenuOpen += (sender, data) =>
            {
                RefreshSavedWeaponsMenu();
            };

            // Set the current saved loadout whenever a loadout is selected.
            SavedLoadoutsMenu.OnItemSelect += (sender, item, index) =>
            {
                if (SavedWeapons.ContainsKey("vmenu_string_saved_weapon_loadout_" + item.Label))
                {
                    SelectedSavedLoadoutName = "vmenu_string_saved_weapon_loadout_" + item.Label;
                }
                else // shouldn't ever happen, but just in case
                {
                    ManageLoadoutMenu.GoBack();
                }
            };

            // Reset the index whenever the ManageLoadout menu is opened. Just to prevent auto selecting the delete option for example.
            ManageLoadoutMenu.OnMenuOpen += (sender, data) =>
            {
                var kvp = GetResourceKvpString("vmenu_string_default_loadout");
                if (string.IsNullOrEmpty(kvp) || kvp != SelectedSavedLoadoutName)
                {
                    setDefaultLoadout.SetLeftBadge(BadgeIcon.NONE);
                }
                else
                {
                    setDefaultLoadout.SetLeftBadge(BadgeIcon.TICK);
                }

            };

            // Refresh the saved weapons menu.
            RefreshSavedWeaponsMenu();
        }

        /// <summary>
        /// Gets the menu.
        /// </summary>
        /// <returns></returns>
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
