using System.Collections.Generic;
using System.Linq;

using ScaleformUI.Menu;

using vMenuClient.data;

namespace vMenuClient.menus
{
    public class VehicleSpawner
    {
        // Variables
        private UIMenu menu;
        public static Dictionary<string, uint> AddonVehicles;

        public bool SpawnInVehicle { get; private set; } = UserDefaults.VehicleSpawnerSpawnInside;
        public bool ReplaceVehicle { get; private set; } = UserDefaults.VehicleSpawnerReplacePrevious;
        public static List<bool> allowedCategories;

        private void CreateMenu()
        {
            #region initial setup.
            // Create the menu.
            menu = new UIMenu(Game.Player.Name, "Vehicle Spawner");

            // Create the buttons and checkboxes.
            UIMenuItem spawnByName = new UIMenuItem("Spawn Vehicle By Model Name", "Enter the name of a vehicle to spawn.");
            UIMenuCheckboxItem spawnInVeh = new UIMenuCheckboxItem("Spawn Inside Vehicle", SpawnInVehicle, "This will teleport you into the vehicle when you spawn it.");
            UIMenuCheckboxItem replacePrev = new UIMenuCheckboxItem("Replace Previous Vehicle", ReplaceVehicle, "This will automatically delete your previously spawned vehicle when you spawn a new vehicle.");

            // Add the items to the menu.
            if (IsAllowed(Permission.VSSpawnByName))
            {
                menu.AddItem(spawnByName);
            }
            menu.AddItem(spawnInVeh);
            menu.AddItem(replacePrev);
            #endregion

            #region addon cars menu
            // Vehicle Addons List
            UIMenu addonCarsMenu = new UIMenu("Addon Vehicles", "Spawn An Addon Vehicle");
            UIMenuItem addonCarsBtn = new UIMenuItem("Addon Vehicles", "A list of addon vehicles available on this server.");
            addonCarsBtn.SetRightLabel("→→→");

            menu.AddItem(addonCarsBtn);

            if (IsAllowed(Permission.VSAddon))
            {
                if (AddonVehicles != null)
                {
                    if (AddonVehicles.Count > 0)
                    {
                        addonCarsBtn.Activated += async (a, b) => await a.SwitchTo(addonCarsMenu, 0, true);
                        UIMenu unavailableCars = new UIMenu("Addon Spawner", "Unavailable Vehicles");
                        UIMenuItem unavailableCarsBtn = new UIMenuItem("Unavailable Vehicles", "These addon vehicles are not currently being streamed (correctly) and are not able to be spawned.");
                        unavailableCarsBtn.SetRightLabel("→→→");

                        for (int cat = 0; cat < 23; cat++)
                        {
                            UIMenu categoryMenu = new UIMenu("Addon Spawner", GetLabelText($"VEH_CLASS_{cat}"));
                            UIMenuItem categoryBtn = new UIMenuItem(GetLabelText($"VEH_CLASS_{cat}"), $"Spawn an addon vehicle from the {GetLabelText($"VEH_CLASS_{cat}")} class.");
                            categoryBtn.SetRightLabel("→→→");

                            addonCarsMenu.AddItem(categoryBtn);

                            if (!allowedCategories[cat])
                            {
                                categoryBtn.Description = "This vehicle class is disabled by the server.";
                                categoryBtn.Enabled = false;
                                categoryBtn.SetLeftBadge(BadgeIcon.LOCK);
                                categoryBtn.SetRightLabel("");
                                continue;
                            }

                            // Loop through all addon vehicles in this class.
                            foreach (KeyValuePair<string, uint> veh in AddonVehicles.Where(v => GetVehicleClassFromName(v.Value) == cat).ToList())
                            {
                                string localizedName = GetLabelText(GetDisplayNameFromVehicleModel(veh.Value));

                                string name = localizedName != "NULL" ? localizedName : GetDisplayNameFromVehicleModel(veh.Value);
                                name = name != "CARNOTFOUND" ? name : veh.Key;

                                UIMenuItem carBtn = new UIMenuItem(name, $"Click to spawn {name}.")
                                {
                                    Label = $"({veh.Key})",
                                    ItemData = veh.Key // store the model name in the button data.
                                };

                                // This should be impossible to be false, but we check it anyway.
                                if (IsModelInCdimage(veh.Value))
                                {
                                    categoryMenu.AddItem(carBtn);
                                }
                                else
                                {
                                    carBtn.Enabled = false;
                                    carBtn.Description = "This vehicle is not available. Please ask the server owner to check if the vehicle is being streamed correctly.";
                                    carBtn.SetLeftBadge(BadgeIcon.LOCK);
                                    unavailableCars.AddItem(carBtn);
                                }
                            }

                            //if (AddonVehicles.Count(av => GetVehicleClassFromName(av.Value) == cat && IsModelInCdimage(av.Value)) > 0)
                            if (categoryMenu.Size > 0)
                            {
                                categoryBtn.Activated += async (a, b) => await a.SwitchTo(categoryMenu, 0, true);

                                categoryMenu.OnItemSelect += (sender, item, index) =>
                                {
                                    SpawnVehicle(item.ItemData.ToString(), SpawnInVehicle, ReplaceVehicle);
                                };
                            }
                            else
                            {
                                categoryBtn.Description = "There are no addon cars available in this category.";
                                categoryBtn.Enabled = false;
                                categoryBtn.SetLeftBadge(BadgeIcon.LOCK);
                                categoryBtn.SetRightLabel("");
                            }
                        }

                        if (unavailableCars.Size > 0)
                        {
                            addonCarsMenu.AddItem(unavailableCarsBtn);
                            unavailableCarsBtn.Activated += async (a, b) => await a.SwitchTo(unavailableCars, 0, true);
                        }
                    }
                    else
                    {
                        addonCarsBtn.Enabled = false;
                        addonCarsBtn.SetLeftBadge(BadgeIcon.LOCK);
                        addonCarsBtn.Description = "There are no addon vehicles available on this server.";
                    }
                }
                else
                {
                    addonCarsBtn.Enabled = false;
                    addonCarsBtn.SetLeftBadge(BadgeIcon.LOCK);
                    addonCarsBtn.Description = "The list containing all addon cars could not be loaded, is it configured properly?";
                }
            }
            else
            {
                addonCarsBtn.Enabled = false;
                addonCarsBtn.SetLeftBadge(BadgeIcon.LOCK);
                addonCarsBtn.Description = "Access to this list has been restricted by the server owner.";
            }
            #endregion

            // These are the max speed, acceleration, braking and traction values per vehicle class.
            float[] speedValues = new float[23]
            {
                44.9374657f,
                50.0000038f,
                48.862133f,
                48.1321335f,
                50.7077942f,
                51.3333359f,
                52.3922348f,
                53.86687f,
                52.03867f,
                49.2241631f,
                39.6176529f,
                37.5559425f,
                42.72843f,
                21.0f,
                45.0f,
                65.1952744f,
                109.764259f,
                42.72843f,
                56.5962219f,
                57.5398865f,
                43.3140678f,
                26.66667f,
                53.0537224f
            };
            float[] accelerationValues = new float[23]
            {
                0.34f,
                0.29f,
                0.335f,
                0.28f,
                0.395f,
                0.39f,
                0.66f,
                0.42f,
                0.425f,
                0.475f,
                0.21f,
                0.3f,
                0.32f,
                0.17f,
                18.0f,
                5.88f,
                21.0700016f,
                0.33f,
                14.0f,
                6.86f,
                0.32f,
                0.2f,
                0.76f
            };
            float[] brakingValues = new float[23]
            {
                0.72f,
                0.95f,
                0.85f,
                0.9f,
                1.0f,
                1.0f,
                1.3f,
                1.25f,
                1.52f,
                1.1f,
                0.6f,
                0.7f,
                0.8f,
                3.0f,
                0.4f,
                3.5920403f,
                20.58f,
                0.9f,
                2.93960738f,
                3.9472363f,
                0.85f,
                5.0f,
                1.3f
            };
            float[] tractionValues = new float[23]
            {
                2.3f,
                2.55f,
                2.3f,
                2.6f,
                2.625f,
                2.65f,
                2.8f,
                2.782f,
                2.9f,
                2.95f,
                2.0f,
                3.3f,
                2.175f,
                2.05f,
                0.0f,
                1.6f,
                2.15f,
                2.55f,
                2.57f,
                3.7f,
                2.05f,
                2.5f,
                3.2925f
            };

            #region vehicle classes submenus
            // Loop through all the vehicle classes.
            for (int vehClass = 0; vehClass < 23; vehClass++)
            {
                // Get the class name.
                string className = GetLabelText($"VEH_CLASS_{vehClass}");

                // Create a button & a menu for it, add the menu to the menu pool and add & bind the button to the menu.
                UIMenuItem btn = new UIMenuItem(className, $"Spawn a vehicle from the ~o~{className} ~s~class.");
                btn.SetRightLabel("→→→");

                UIMenu vehicleClassMenu = new UIMenu("Vehicle Spawner", className);

                menu.AddItem(btn);

                if (allowedCategories[vehClass])
                {
                    btn.Activated += async (a, b) => await a.SwitchTo(vehicleClassMenu, 0, true);
                }
                else
                {
                    btn.SetLeftBadge(BadgeIcon.LOCK);
                    btn.Description = "This category has been disabled by the server owner.";
                    btn.Enabled = false;
                }

                // Create a dictionary for the duplicate vehicle names (in this vehicle class).
                Dictionary<string, int> duplicateVehNames = new Dictionary<string, int>();

                #region Add vehicles per class
                // Loop through all the vehicles in the vehicle class.
                foreach (string veh in VehicleData.Vehicles.VehicleClasses[className])
                {
                    // Convert the model name to start with a Capital letter, converting the other characters to lowercase. 
                    string properCasedModelName = veh[0].ToString().ToUpper() + veh.ToLower().Substring(1);

                    // Get the localized vehicle name, if it's "NULL" (no label found) then use the "properCasedModelName" created above.
                    string vehName = GetVehDisplayNameFromModel(veh) != "NULL" ? GetVehDisplayNameFromModel(veh) : properCasedModelName;
                    string vehModelName = veh;
                    uint model = (uint)GetHashKey(vehModelName);

                    float topSpeed = Map(GetVehicleModelEstimatedMaxSpeed(model), 0f, speedValues[vehClass], 0f, 1f);
                    float acceleration = Map(GetVehicleModelAcceleration(model), 0f, accelerationValues[vehClass], 0f, 1f);
                    float maxBraking = Map(GetVehicleModelMaxBraking(model), 0f, brakingValues[vehClass], 0f, 1f);
                    float maxTraction = Map(GetVehicleModelMaxTraction(model), 0f, tractionValues[vehClass], 0f, 1f);

                    // Loop through all the menu items and check each item's title/text and see if it matches the current vehicle (display) name.
                    bool duplicate = false;
                    for (int itemIndex = 0; itemIndex < vehicleClassMenu.Size; itemIndex++)
                    {
                        // If it matches...
                        if (vehicleClassMenu.MenuItems[itemIndex].Label.ToString() == vehName)
                        {

                            // Check if the model was marked as duplicate before.
                            if (duplicateVehNames.Keys.Contains(vehName))
                            {
                                // If so, add 1 to the duplicate counter for this model name.
                                duplicateVehNames[vehName]++;
                            }

                            // If this is the first duplicate, then set it to 2.
                            else
                            {
                                duplicateVehNames[vehName] = 2;
                            }

                            // The model name is a duplicate, so get the modelname and add the duplicate amount for this model name to the end of the vehicle name.
                            vehName += $" ({duplicateVehNames[vehName]})";

                            // Then create and add a new button for this vehicle.

                            if (DoesModelExist(veh))
                            {
                                UIMenuItem vehBtn = new UIMenuItem(vehName)
                                {
                                    Enabled = true,
                                    ItemData = new float[4] { topSpeed, acceleration, maxBraking, maxTraction }
                                };
                                vehBtn.SetRightLabel($"({vehModelName.ToLower()})");
                                vehicleClassMenu.AddItem(vehBtn);
                            }
                            else
                            {
                                UIMenuItem vehBtn = new UIMenuItem(vehName, "This vehicle is not available because the model could not be found in your game files. If this is a DLC vehicle, make sure the server is streaming it.")
                                {
                                    Enabled = false,
                                    ItemData = new float[4] { 0f, 0f, 0f, 0f }
                                };
                                vehBtn.SetRightLabel($"({vehModelName.ToLower()})");
                                vehicleClassMenu.AddItem(vehBtn);
                                vehBtn.SetRightBadge(BadgeIcon.LOCK);
                            }

                            // Mark duplicate as true and break from the loop because we already found the duplicate.
                            duplicate = true;
                            break;
                        }
                    }

                    // If it's not a duplicate, add the model name.
                    if (!duplicate)
                    {
                        if (DoesModelExist(veh))
                        {
                            UIMenuItem vehBtn = new UIMenuItem(vehName)
                            {
                                Enabled = true,
                                ItemData = new float[4] { topSpeed, acceleration, maxBraking, maxTraction, }
                            };
                            vehBtn.SetRightLabel($"({vehModelName.ToLower()})");
                            UIMenuStatisticsPanel pan = new UIMenuStatisticsPanel();
                            pan.AddStatistics("Speed", topSpeed);
                            pan.AddStatistics("Accel.", acceleration);
                            pan.AddStatistics("Breaking", maxBraking);
                            pan.AddStatistics("Traction", maxTraction);
                            vehBtn.AddPanel(pan);
                            vehicleClassMenu.AddItem(vehBtn);
                        }
                        else
                        {
                            UIMenuItem vehBtn = new UIMenuItem(vehName, "This vehicle is not available because the model could not be found in your game files. If this is a DLC vehicle, make sure the server is streaming it.")
                            {
                                Enabled = false,
                                ItemData = new float[4] { 0f, 0f, 0f, 0f }
                            };
                            vehBtn.SetRightLabel($"({vehModelName.ToLower()})");

                            vehicleClassMenu.AddItem(vehBtn);
                            vehBtn.SetRightBadge(BadgeIcon.LOCK);
                        }
                    }
                }
                #endregion

                // Handle button presses
                vehicleClassMenu.OnItemSelect += async (sender2, item2, index2) =>
                {
                    await SpawnVehicle(VehicleData.Vehicles.VehicleClasses[className][index2], SpawnInVehicle, ReplaceVehicle);
                };
            }
            #endregion

            #region handle events
            // Handle button presses.
            menu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == spawnByName)
                {
                    // Passing "custom" as the vehicle name, will ask the user for input.
                    await SpawnVehicle("custom", SpawnInVehicle, ReplaceVehicle);
                }
            };

            // Handle checkbox changes.
            menu.OnCheckboxChange += (sender, item, _checked) =>
            {
                if (item == spawnInVeh)
                {
                    SpawnInVehicle = _checked;
                }
                else if (item == replacePrev)
                {
                    ReplaceVehicle = _checked;
                }
            };
            #endregion
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
