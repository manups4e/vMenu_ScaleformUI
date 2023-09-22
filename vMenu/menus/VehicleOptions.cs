using System;
using System.Collections.Generic;
using System.Linq;

using ScaleformUI.Menu;

using vMenuClient.data;

namespace vMenuClient.menus
{
    public class VehicleOptions
    {
        #region Variables
        // Menu variable, will be defined in CreateMenu()
        private UIMenu menu;

        // Submenus
        public UIMenu VehicleModMenu { get; private set; }
        public UIMenu VehicleDoorsMenu { get; private set; }
        public UIMenu VehicleWindowsMenu { get; private set; }
        public UIMenu VehicleComponentsMenu { get; private set; }
        public UIMenu VehicleLiveriesMenu { get; private set; }
        public UIMenu VehicleColorsMenu { get; private set; }
        public UIMenu DeleteConfirmMenu { get; private set; }
        public UIMenu VehicleUnderglowMenu { get; private set; }

        // Public variables (getters only), return the private variables.
        public bool VehicleGodMode { get; private set; } = UserDefaults.VehicleGodMode;
        public bool VehicleGodInvincible { get; private set; } = UserDefaults.VehicleGodInvincible;
        public bool VehicleGodEngine { get; private set; } = UserDefaults.VehicleGodEngine;
        public bool VehicleGodVisual { get; private set; } = UserDefaults.VehicleGodVisual;
        public bool VehicleGodStrongWheels { get; private set; } = UserDefaults.VehicleGodStrongWheels;
        public bool VehicleGodRamp { get; private set; } = UserDefaults.VehicleGodRamp;
        public bool VehicleGodAutoRepair { get; private set; } = UserDefaults.VehicleGodAutoRepair;

        public bool VehicleNeverDirty { get; private set; } = UserDefaults.VehicleNeverDirty;
        public bool VehicleEngineAlwaysOn { get; private set; } = UserDefaults.VehicleEngineAlwaysOn;
        public bool VehicleNoSiren { get; private set; } = UserDefaults.VehicleNoSiren;
        public bool VehicleNoBikeHelemet { get; private set; } = UserDefaults.VehicleNoBikeHelmet;
        public bool FlashHighbeamsOnHonk { get; private set; } = UserDefaults.VehicleHighbeamsOnHonk;
        public bool DisablePlaneTurbulence { get; private set; } = UserDefaults.VehicleDisablePlaneTurbulence;
        public bool DisableHelicopterTurbulence { get; private set; } = UserDefaults.VehicleDisableHelicopterTurbulence;
        public bool VehicleBikeSeatbelt { get; private set; } = UserDefaults.VehicleBikeSeatbelt;
        public bool VehicleInfiniteFuel { get; private set; } = false;
        public bool VehicleShowHealth { get; private set; } = false;
        public bool VehicleFrozen { get; private set; } = false;
        public bool VehicleTorqueMultiplier { get; private set; } = false;
        public bool VehiclePowerMultiplier { get; private set; } = false;
        public float VehicleTorqueMultiplierAmount { get; private set; } = 2f;
        public float VehiclePowerMultiplierAmount { get; private set; } = 2f;

        private readonly Dictionary<UIMenuItem, int> vehicleExtras = new();
        #endregion

        #region CreateMenu()
        /// <summary>
        /// Create menu creates the vehicle options menu.
        /// </summary>
        private void CreateMenu()
        {
            // Create the menu.
            menu = new UIMenu(Game.Player.Name, "Vehicle Options");

            #region menu items variables
            // vehicle god mode menu
            UIMenu vehGodMenu = new UIMenu("Vehicle Godmode", "Vehicle Godmode Options");
            UIMenuItem vehGodMenuBtn = new UIMenuItem("God Mode Options", "Enable or disable specific damage types.");
            vehGodMenuBtn.SetRightLabel("→→→");

            // Create Checkboxes.
            UIMenuCheckboxItem vehicleGod = new UIMenuCheckboxItem("Vehicle God Mode", VehicleGodMode, "Makes your vehicle not take any damage. Note, you need to go into the god menu options below to select what kind of damage you want to disable.");
            UIMenuCheckboxItem vehicleNeverDirty = new UIMenuCheckboxItem("Keep Vehicle Clean", VehicleNeverDirty, "This will constantly clean your car if the vehicle dirt level goes above 0. Note that this only cleans ~o~dust~s~ or ~o~dirt~s~. This does not clean mud, snow or other ~r~damage decals~s~. Repair your vehicle to remove them.");
            UIMenuCheckboxItem vehicleBikeSeatbelt = new UIMenuCheckboxItem("Bike Seatbelt", VehicleBikeSeatbelt, "Prevents you from being knocked off your bike, bicyle, ATV or similar.");
            UIMenuCheckboxItem vehicleEngineAO = new UIMenuCheckboxItem("Engine Always On", VehicleEngineAlwaysOn, "Keeps your vehicle engine on when you exit your vehicle.");
            UIMenuCheckboxItem vehicleNoTurbulence = new UIMenuCheckboxItem("Disable Plane Turbulence", DisablePlaneTurbulence, "Disables the turbulence for all planes.");
            UIMenuCheckboxItem vehicleNoTurbulenceHeli = new UIMenuCheckboxItem("Disable Helicopter Turbulence", DisableHelicopterTurbulence, "Disables the turbulence for all helicopters.");
            UIMenuCheckboxItem vehicleNoSiren = new UIMenuCheckboxItem("Disable Siren", VehicleNoSiren, "Disables your vehicle's siren. Only works if your vehicle actually has a siren.");
            UIMenuCheckboxItem vehicleNoBikeHelmet = new UIMenuCheckboxItem("No Bike Helmet", VehicleNoBikeHelemet, "No longer auto-equip a helmet when getting on a bike or quad.");
            UIMenuCheckboxItem vehicleFreeze = new UIMenuCheckboxItem("Freeze Vehicle", VehicleFrozen, "Freeze your vehicle's position.");
            UIMenuCheckboxItem torqueEnabled = new UIMenuCheckboxItem("Enable Torque Multiplier", VehicleTorqueMultiplier, "Enables the torque multiplier selected from the list below.");
            UIMenuCheckboxItem powerEnabled = new UIMenuCheckboxItem("Enable Power Multiplier", VehiclePowerMultiplier, "Enables the power multiplier selected from the list below.");
            UIMenuCheckboxItem highbeamsOnHonk = new UIMenuCheckboxItem("Flash Highbeams On Honk", FlashHighbeamsOnHonk, "Turn on your highbeams on your vehicle when honking your horn. Does not work during the day when you have your lights turned off.");
            UIMenuCheckboxItem showHealth = new UIMenuCheckboxItem("Show Vehicle Health", VehicleShowHealth, "Shows the vehicle health on the screen.");
            UIMenuCheckboxItem infiniteFuel = new UIMenuCheckboxItem("Infinite Fuel", VehicleInfiniteFuel, "Enables or disables infinite fuel for this vehicle, only works if FRFuel is installed.");

            // Create buttons.
            UIMenuItem fixVehicle = new UIMenuItem("Repair Vehicle", "Repair any visual and physical damage present on your vehicle.");
            UIMenuItem cleanVehicle = new UIMenuItem("Wash Vehicle", "Clean your vehicle.");
            UIMenuItem toggleEngine = new UIMenuItem("Toggle Engine On/Off", "Turn your engine on/off.");
            UIMenuItem setLicensePlateText = new UIMenuItem("Set License Plate Text", "Enter a custom license plate for your vehicle.");
            UIMenuItem modMenuBtn = new UIMenuItem("Mod Menu", "Tune and customize your vehicle here.");
            modMenuBtn.SetRightLabel("→→→");
            UIMenuItem doorsMenuBtn = new UIMenuItem("Vehicle Doors", "Open, close, remove and restore vehicle doors here.");
            doorsMenuBtn.SetRightLabel("→→→");
            UIMenuItem windowsMenuBtn = new UIMenuItem("Vehicle Windows", "Roll your windows up/down or remove/restore your vehicle windows here.");
            windowsMenuBtn.SetRightLabel("→→→");
            UIMenuItem componentsMenuBtn = new UIMenuItem("Vehicle Extras", "Add/remove vehicle components/extras.");
            componentsMenuBtn.SetRightLabel("→→→");
            UIMenuItem liveriesMenuBtn = new UIMenuItem("Vehicle Liveries", "Style your vehicle with fancy liveries!");
            liveriesMenuBtn.SetRightLabel("→→→");
            UIMenuItem colorsMenuBtn = new UIMenuItem("Vehicle Colors", "Style your vehicle even further by giving it some ~g~Snailsome ~s~colors!");
            colorsMenuBtn.SetRightLabel("→→→");
            UIMenuItem underglowMenuBtn = new UIMenuItem("Vehicle Neon Kits", "Make your vehicle shine with some fancy neon underglow!");
            underglowMenuBtn.SetRightLabel("→→→");
            UIMenuItem vehicleInvisible = new UIMenuItem("Toggle Vehicle Visibility", "Makes your vehicle visible/invisible. ~r~Your vehicle will be made visible again as soon as you leave the vehicle. Otherwise you would not be able to get back in.");
            UIMenuItem flipVehicle = new UIMenuItem("Flip Vehicle", "Sets your current vehicle on all 4 wheels.");
            UIMenuItem vehicleAlarm = new UIMenuItem("Toggle Vehicle Alarm", "Starts/stops your vehicle's alarm.");
            UIMenuItem cycleSeats = new UIMenuItem("Cycle Through Vehicle Seats", "Cycle through the available vehicle seats.");
            List<dynamic> lights = new List<dynamic>()
            {
                "Hazard Lights",
                "Left Indicator",
                "Right Indicator",
                "Interior Lights",
                //"Taxi Light", // this doesn't seem to work no matter what.
                "Helicopter Spotlight",
            };
            UIMenuListItem vehicleLights = new UIMenuListItem("Vehicle Lights", lights, 0, "Turn vehicle lights on/off.");

            List<dynamic> stationNames = new List<dynamic>();

            foreach (string radioStationName in Enum.GetNames(typeof(RadioStation)))
            {
                stationNames.Add(radioStationName);
            }

            int radioIndex = UserDefaults.VehicleDefaultRadio;

            if (radioIndex == (int)RadioStation.RadioOff)
            {
                RadioStation[] stations = (RadioStation[])Enum.GetValues(typeof(RadioStation));
                int index = Array.IndexOf(stations, RadioStation.RadioOff);
                radioIndex = index;
            }

            UIMenuListItem radioStations = new UIMenuListItem("Default radio station", stationNames, radioIndex, "Select a defalut radio station to be set when spawning new car");

            List<dynamic> tiresList = new List<dynamic>() { "All Tires", "Tire #1", "Tire #2", "Tire #3", "Tire #4", "Tire #5", "Tire #6", "Tire #7", "Tire #8" };
            UIMenuListItem vehicleTiresList = new UIMenuListItem("Fix / Destroy Tires", tiresList, 0, "Fix or destroy a specific vehicle tire, or all of them at once. Note, not all indexes are valid for all vehicles, some might not do anything on certain vehicles.");

            UIMenuItem destroyEngine = new UIMenuItem("Destroy Engine", "Destroys your vehicle's engine.");

            UIMenuItem deleteBtn = new UIMenuItem("~r~Delete Vehicle", "Delete your vehicle, this ~r~can NOT be undone~s~!");
            deleteBtn.SetLeftBadge(BadgeIcon.WARNING);
            deleteBtn.SetRightLabel("→→→");
            UIMenuItem deleteNoBtn = new UIMenuItem("NO, CANCEL", "NO, do NOT delete my vehicle and go back!");
            UIMenuItem deleteYesBtn = new UIMenuItem("~r~YES, DELETE", "Yes I'm sure, delete my vehicle please, I understand that this cannot be undone.");
            deleteYesBtn.SetLeftBadge(BadgeIcon.WARNING);
            // Create lists.
            List<dynamic> dirtlevel = new List<dynamic> { "No Dirt", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
            UIMenuListItem setDirtLevel = new UIMenuListItem("Set Dirt Level", dirtlevel, 0, "Select how much dirt should be visible on your vehicle, press ~r~enter~s~ " +
                "to apply the selected level.");
            List<dynamic> licensePlates = new List<dynamic> { GetLabelText("CMOD_PLA_0"), GetLabelText("CMOD_PLA_1"), GetLabelText("CMOD_PLA_2"), GetLabelText("CMOD_PLA_3"),
                GetLabelText("CMOD_PLA_4"), "North Yankton" };
            UIMenuListItem setLicensePlateType = new UIMenuListItem("License Plate Type", licensePlates, 0, "Choose a license plate type and press ~r~enter ~s~to apply " +
                "it to your vehicle.");
            List<dynamic> torqueMultiplierList = new List<dynamic> { "x2", "x4", "x8", "x16", "x32", "x64", "x128", "x256", "x512", "x1024" };
            UIMenuListItem torqueMultiplier = new UIMenuListItem("Set Engine Torque Multiplier", torqueMultiplierList, 0, "Set the engine torque multiplier.");
            List<dynamic> powerMultiplierList = new List<dynamic> { "x2", "x4", "x8", "x16", "x32", "x64", "x128", "x256", "x512", "x1024" };
            UIMenuListItem powerMultiplier = new UIMenuListItem("Set Engine Power Multiplier", powerMultiplierList, 0, "Set the engine power multiplier.");
            List<dynamic> speedLimiterOptions = new List<dynamic>() { "Set", "Reset", "Custom Speed Limit" };
            UIMenuListItem speedLimiter = new UIMenuListItem("Speed Limiter", speedLimiterOptions, 0, "Set your vehicles max speed to your ~y~current speed~s~. Resetting your vehicles max speed will set the max speed of your current vehicle back to default. Only your current vehicle is affected by this option.");
            #endregion

            #region Submenus
            // Submenu's
            VehicleModMenu = new UIMenu("Mod Menu", "Vehicle Mods");
            /*
            VehicleModMenu.InstructionalButtons.Add(Control.Jump, "Toggle Vehicle Doors");
            VehicleModMenu.ButtonPressHandlers.Add(new UIMenu.ButtonPressHandler(Control.Jump, Menu.ControlPressCheckType.JUST_PRESSED, new Action<Menu, Control>((m, c) =>
            {
                var veh = GetVehicle();
                if (veh != null && veh.Exists() && !veh.IsDead && veh.Driver == Game.PlayerPed)
                {
                    var open = GetVehicleDoorAngleRatio(veh.Handle, 0) < 0.1f;
                    if (open)
                    {
                        for (var i = 0; i < 8; i++)
                        {
                            SetVehicleDoorOpen(veh.Handle, i, false, false);
                        }
                    }
                    else
                    {
                        SetVehicleDoorsShut(veh.Handle, false);
                    }
                }
            }), false));
            */
            VehicleDoorsMenu = new UIMenu("Vehicle Doors", "Vehicle Doors Management");
            VehicleWindowsMenu = new UIMenu("Vehicle Windows", "Vehicle Windows Management");
            VehicleComponentsMenu = new UIMenu("Vehicle Extras", "Vehicle Extras/Components");
            VehicleLiveriesMenu = new UIMenu("Vehicle Liveries", "Vehicle Liveries");
            VehicleColorsMenu = new UIMenu("Vehicle Colors", "Vehicle Colors");
            DeleteConfirmMenu = new UIMenu("Confirm Action", "Delete Vehicle, Are You Sure?");
            VehicleUnderglowMenu = new UIMenu("Vehicle Neon Kits", "Vehicle Neon Underglow Options");

            #endregion

            #region Add items to the menu.
            // Add everything to the menu. (based on permissions)
            if (IsAllowed(Permission.VOGod)) // GOD MODE
            {
                menu.AddItem(vehicleGod);
                menu.AddItem(vehGodMenuBtn);
                vehGodMenuBtn.Activated += async (a, b) => await a.SwitchTo(vehGodMenu, 0, true);

                UIMenuCheckboxItem godInvincible = new UIMenuCheckboxItem("Invincible", VehicleGodInvincible, "Makes the car invincible. Includes fire damage, explosion damage, collision damage and more.");
                UIMenuCheckboxItem godEngine = new UIMenuCheckboxItem("Engine Damage", VehicleGodEngine, "Disables your engine from taking any damage.");
                UIMenuCheckboxItem godVisual = new UIMenuCheckboxItem("Visual Damage", VehicleGodVisual, "This prevents scratches and other damage decals from being applied to your vehicle. It does not prevent (body) deformation damage.");
                UIMenuCheckboxItem godStrongWheels = new UIMenuCheckboxItem("Strong Wheels", VehicleGodStrongWheels, "Disables your wheels from being deformed and causing reduced handling. This does not make tires bulletproof.");
                UIMenuCheckboxItem godRamp = new UIMenuCheckboxItem("Ramp Damage", VehicleGodRamp, "Disables vehicles such as the Ramp Buggy from taking damage when using the ramp.");
                UIMenuCheckboxItem godAutoRepair = new UIMenuCheckboxItem("~r~Auto Repair", VehicleGodAutoRepair, "Automatically repairs your vehicle when it has ANY type of damage. It's recommended to keep this turned off to prevent glitchyness.");

                vehGodMenu.AddItem(godInvincible);
                vehGodMenu.AddItem(godEngine);
                vehGodMenu.AddItem(godVisual);
                vehGodMenu.AddItem(godStrongWheels);
                vehGodMenu.AddItem(godRamp);
                vehGodMenu.AddItem(godAutoRepair);

                vehGodMenu.OnCheckboxChange += (sender, item, _checked) =>
                {
                    if (item == godInvincible)
                    {
                        VehicleGodInvincible = _checked;
                    }
                    else if (item == godEngine)
                    {
                        VehicleGodEngine = _checked;
                    }
                    else if (item == godVisual)
                    {
                        VehicleGodVisual = _checked;
                    }
                    else if (item == godStrongWheels)
                    {
                        VehicleGodStrongWheels = _checked;
                    }
                    else if (item == godRamp)
                    {
                        VehicleGodRamp = _checked;
                    }
                    else if (item == godAutoRepair)
                    {
                        VehicleGodAutoRepair = _checked;
                    }
                };

            }
            if (IsAllowed(Permission.VORepair)) // REPAIR VEHICLE
            {
                menu.AddItem(fixVehicle);
            }
            if (IsAllowed(Permission.VOKeepClean))
            {
                menu.AddItem(vehicleNeverDirty);
            }
            if (IsAllowed(Permission.VOWash))
            {
                menu.AddItem(cleanVehicle); // CLEAN VEHICLE
                menu.AddItem(setDirtLevel); // SET DIRT LEVEL
            }
            if (IsAllowed(Permission.VOMod)) // MOD MENU
            {
                menu.AddItem(modMenuBtn);
            }
            if (IsAllowed(Permission.VOColors)) // COLORS MENU
            {
                menu.AddItem(colorsMenuBtn);
            }
            if (IsAllowed(Permission.VOUnderglow)) // UNDERGLOW EFFECTS
            {
                menu.AddItem(underglowMenuBtn);
                underglowMenuBtn.Activated += async (a, b) => await a.SwitchTo(VehicleUnderglowMenu, 0, true);
            }
            if (IsAllowed(Permission.VOLiveries)) // LIVERIES MENU
            {
                menu.AddItem(liveriesMenuBtn);
            }
            if (IsAllowed(Permission.VOComponents)) // COMPONENTS MENU
            {
                menu.AddItem(componentsMenuBtn);
            }
            if (IsAllowed(Permission.VOEngine)) // TOGGLE ENGINE ON/OFF
            {
                menu.AddItem(toggleEngine);
            }
            if (IsAllowed(Permission.VOChangePlate))
            {
                menu.AddItem(setLicensePlateText); // SET LICENSE PLATE TEXT
                menu.AddItem(setLicensePlateType); // SET LICENSE PLATE TYPE
            }
            if (IsAllowed(Permission.VODoors)) // DOORS MENU
            {
                menu.AddItem(doorsMenuBtn);
            }
            if (IsAllowed(Permission.VOWindows)) // WINDOWS MENU
            {
                menu.AddItem(windowsMenuBtn);
            }
            if (IsAllowed(Permission.VOBikeSeatbelt))
            {
                menu.AddItem(vehicleBikeSeatbelt);
            }
            if (IsAllowed(Permission.VOSpeedLimiter)) // SPEED LIMITER
            {
                menu.AddItem(speedLimiter);
            }
            if (IsAllowed(Permission.VOTorqueMultiplier))
            {
                menu.AddItem(torqueEnabled); // TORQUE ENABLED
                menu.AddItem(torqueMultiplier); // TORQUE LIST
            }
            if (IsAllowed(Permission.VOPowerMultiplier))
            {
                menu.AddItem(powerEnabled); // POWER ENABLED
                menu.AddItem(powerMultiplier); // POWER LIST
            }
            if (IsAllowed(Permission.VODisableTurbulence))
            {
                menu.AddItem(vehicleNoTurbulence);
                menu.AddItem(vehicleNoTurbulenceHeli);
            }
            if (IsAllowed(Permission.VOFlip)) // FLIP VEHICLE
            {
                menu.AddItem(flipVehicle);
            }
            if (IsAllowed(Permission.VOAlarm)) // TOGGLE VEHICLE ALARM
            {
                menu.AddItem(vehicleAlarm);
            }
            if (IsAllowed(Permission.VOCycleSeats)) // CYCLE THROUGH VEHICLE SEATS
            {
                menu.AddItem(cycleSeats);
            }
            if (IsAllowed(Permission.VOLights)) // VEHICLE LIGHTS LIST
            {
                menu.AddItem(vehicleLights);
            }
            if (IsAllowed(Permission.VOFixOrDestroyTires))
            {
                menu.AddItem(vehicleTiresList);
            }
            if (IsAllowed(Permission.VODestroyEngine))
            {
                menu.AddItem(destroyEngine);
            }
            if (IsAllowed(Permission.VOFreeze)) // FREEZE VEHICLE
            {
                menu.AddItem(vehicleFreeze);
            }
            if (IsAllowed(Permission.VOInvisible)) // MAKE VEHICLE INVISIBLE
            {
                menu.AddItem(vehicleInvisible);
            }
            if (IsAllowed(Permission.VOEngineAlwaysOn)) // LEAVE ENGINE RUNNING
            {
                menu.AddItem(vehicleEngineAO);
            }
            if (IsAllowed(Permission.VOInfiniteFuel)) // INFINITE FUEL
            {
                menu.AddItem(infiniteFuel);
            }
            // always allowed
            menu.AddItem(showHealth); // SHOW VEHICLE HEALTH

            // I don't really see why would you want to disable this so I will not add useless permissions
            menu.AddItem(radioStations);

            if (IsAllowed(Permission.VONoSiren) && !GetSettingsBool(Setting.vmenu_use_els_compatibility_mode)) // DISABLE SIREN
            {
                menu.AddItem(vehicleNoSiren);
            }
            if (IsAllowed(Permission.VONoHelmet)) // DISABLE BIKE HELMET
            {
                menu.AddItem(vehicleNoBikeHelmet);
            }
            if (IsAllowed(Permission.VOFlashHighbeamsOnHonk)) // FLASH HIGHBEAMS ON HONK
            {
                menu.AddItem(highbeamsOnHonk);
            }

            if (IsAllowed(Permission.VODelete)) // DELETE VEHICLE
            {
                menu.AddItem(deleteBtn);
            }
            #endregion

            #region delete vehicle handle stuff
            DeleteConfirmMenu.AddItem(deleteNoBtn);
            DeleteConfirmMenu.AddItem(deleteYesBtn);
            DeleteConfirmMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == deleteNoBtn)
                {
                    DeleteConfirmMenu.GoBack();
                }
                else
                {
                    Vehicle veh = GetVehicle();
                    if (veh != null && veh.Exists() && GetVehicle().Driver == Game.PlayerPed)
                    {
                        SetVehicleHasBeenOwnedByPlayer(veh.Handle, false);
                        SetEntityAsMissionEntity(veh.Handle, false, false);
                        veh.Delete();
                    }
                    else
                    {
                        if (!Game.PlayerPed.IsInVehicle())
                        {
                            Notify.Alert(CommonErrors.NoVehicle);
                        }
                        else
                        {
                            Notify.Alert("You need to be in the driver's seat if you want to delete a vehicle.");
                        }

                    }
                    DeleteConfirmMenu.GoBack();
                    menu.GoBack();
                }
            };
            #endregion

            #region Bind Submenus to their buttons.
            modMenuBtn.Activated += async (a, b) => await a.SwitchTo(VehicleModMenu, 0, true);
            doorsMenuBtn.Activated += async (a, b) => await a.SwitchTo(VehicleDoorsMenu, 0, true);
            windowsMenuBtn.Activated += async (a, b) => await a.SwitchTo(VehicleWindowsMenu, 0, true);
            componentsMenuBtn.Activated += async (a, b) => await a.SwitchTo(VehicleComponentsMenu, 0, true);
            liveriesMenuBtn.Activated += async (a, b) => await a.SwitchTo(VehicleLiveriesMenu, 0, true);
            colorsMenuBtn.Activated += async (a, b) => await a.SwitchTo(VehicleColorsMenu, 0, true);
            deleteBtn.Activated += async (a, b) => await a.SwitchTo(DeleteConfirmMenu, 0, true);
            #endregion

            #region Handle button presses
            // Manage button presses.
            menu.OnItemSelect += (sender, item, index) =>
            {
                // If the player is actually in a vehicle, continue.
                if (GetVehicle() != null && GetVehicle().Exists())
                {
                    // Create a vehicle object.
                    Vehicle vehicle = GetVehicle();

                    // Check if the player is the driver of the vehicle, if so, continue.
                    if (vehicle.GetPedOnSeat(VehicleSeat.Driver) == new Ped(Game.PlayerPed.Handle))
                    {
                        // Repair vehicle.
                        if (item == fixVehicle)
                        {
                            vehicle.Repair();
                        }
                        // Clean vehicle.
                        else if (item == cleanVehicle)
                        {
                            vehicle.Wash();
                        }
                        // Flip vehicle.
                        else if (item == flipVehicle)
                        {
                            SetVehicleOnGroundProperly(vehicle.Handle);
                        }
                        // Toggle alarm.
                        else if (item == vehicleAlarm)
                        {
                            ToggleVehicleAlarm(vehicle);
                        }
                        // Toggle engine
                        else if (item == toggleEngine)
                        {
                            SetVehicleEngineOn(vehicle.Handle, !vehicle.IsEngineRunning, false, true);
                        }
                        // Set license plate text
                        else if (item == setLicensePlateText)
                        {
                            SetLicensePlateCustomText();
                        }
                        // Make vehicle invisible.
                        else if (item == vehicleInvisible)
                        {
                            if (vehicle.IsVisible)
                            {
                                // Check the visibility of all peds inside before setting the vehicle as invisible.
                                Dictionary<Ped, bool> visiblePeds = new Dictionary<Ped, bool>();
                                foreach (Ped p in vehicle.Occupants)
                                {
                                    visiblePeds.Add(p, p.IsVisible);
                                }

                                // Set the vehicle invisible or invincivble.
                                vehicle.IsVisible = !vehicle.IsVisible;

                                // Restore visibility for each ped.
                                foreach (KeyValuePair<Ped, bool> pe in visiblePeds)
                                {
                                    pe.Key.IsVisible = pe.Value;
                                }
                            }
                            else
                            {
                                // Set the vehicle invisible or invincivble.
                                vehicle.IsVisible = !vehicle.IsVisible;
                            }
                        }
                        // Destroy vehicle engine
                        else if (item == destroyEngine)
                        {
                            SetVehicleEngineHealth(vehicle.Handle, -4000);
                        }
                    }

                    // If the player is not the driver seat and a button other than the option below (cycle seats) was pressed, notify them.
                    else if (item != cycleSeats)
                    {
                        Notify.Error("You have to be the driver of a vehicle to access this menu!", true, false);
                    }

                    // Cycle vehicle seats
                    if (item == cycleSeats)
                    {
                        CycleThroughSeats();
                    }
                }
            };
            #endregion

            #region Handle checkbox changes.
            menu.OnCheckboxChange += (sender, item, _checked) =>
            {
                // Create a vehicle object.
                Vehicle vehicle = GetVehicle();

                if (item == vehicleGod) // God Mode Toggled
                {
                    VehicleGodMode = _checked;
                }
                else if (item == vehicleFreeze) // Freeze Vehicle Toggled
                {
                    VehicleFrozen = _checked;
                    if (!_checked)
                    {
                        if (vehicle != null && vehicle.Exists())
                        {
                            FreezeEntityPosition(vehicle.Handle, false);
                        }
                    }
                }
                else if (item == torqueEnabled) // Enable Torque Multiplier Toggled
                {
                    VehicleTorqueMultiplier = _checked;
                }
                else if (item == powerEnabled) // Enable Power Multiplier Toggled
                {
                    VehiclePowerMultiplier = _checked;
                    if (_checked)
                    {
                        if (vehicle != null && vehicle.Exists())
                        {
                            SetVehicleEnginePowerMultiplier(vehicle.Handle, VehiclePowerMultiplierAmount);
                        }
                    }
                    else
                    {
                        if (vehicle != null && vehicle.Exists())
                        {
                            SetVehicleEnginePowerMultiplier(vehicle.Handle, 1f);
                        }
                    }
                }
                else if (item == vehicleEngineAO) // Leave Engine Running (vehicle always on) Toggled
                {
                    VehicleEngineAlwaysOn = _checked;
                }
                else if (item == showHealth) // show vehicle health on screen.
                {
                    VehicleShowHealth = _checked;
                }
                else if (item == vehicleNoSiren) // Disable Siren Toggled
                {
                    VehicleNoSiren = _checked;
                    if (vehicle != null && vehicle.Exists())
                    {
                        vehicle.IsSirenSilent = _checked;
                    }
                }
                else if (item == vehicleNoBikeHelmet) // No Helemet Toggled
                {
                    VehicleNoBikeHelemet = _checked;
                }
                else if (item == highbeamsOnHonk)
                {
                    FlashHighbeamsOnHonk = _checked;
                }
                else if (item == vehicleNoTurbulence)
                {
                    DisablePlaneTurbulence = _checked;
                    if (vehicle != null && vehicle.Exists() && vehicle.Model.IsPlane)
                    {
                        if (MainMenu.VehicleOptionsMenu.DisablePlaneTurbulence)
                        {
                            SetPlaneTurbulenceMultiplier(vehicle.Handle, 0f);
                        }
                        else
                        {
                            SetPlaneTurbulenceMultiplier(vehicle.Handle, 1.0f);
                        }
                    }
                }
                else if (item == vehicleNoTurbulenceHeli)
                {
                    DisableHelicopterTurbulence = _checked;
                    if (vehicle != null && vehicle.Exists() && vehicle.Model.IsHelicopter)
                    {
                        if (MainMenu.VehicleOptionsMenu.DisableHelicopterTurbulence)
                        {
                            SetHeliTurbulenceScalar(vehicle.Handle, 0f);
                        }
                        else
                        {
                            SetHeliTurbulenceScalar(vehicle.Handle, 1f);
                        }
                    }
                }
                else if (item == vehicleNeverDirty)
                {
                    VehicleNeverDirty = _checked;
                }
                else if (item == vehicleBikeSeatbelt)
                {
                    VehicleBikeSeatbelt = _checked;
                }
                else if (item == infiniteFuel)
                {
                    VehicleInfiniteFuel = _checked;
                }
            };
            #endregion

            #region Handle List Changes.
            // Handle list changes.
            menu.OnListChange += (sender, item, itemIndex) =>
            {
                if (GetVehicle() != null && GetVehicle().Exists())
                {
                    Vehicle veh = GetVehicle();
                    // If the torque multiplier changed. Change the torque multiplier to the new value.
                    if (item == torqueMultiplier)
                    {
                        // Get the selected value and remove the "x" in the string with nothing.
                        dynamic value = torqueMultiplierList[itemIndex].ToString().Replace("x", "");
                        // Convert the value to a float and set it as a public variable.
                        VehicleTorqueMultiplierAmount = float.Parse(value);
                    }
                    // If the power multiplier is changed. Change the power multiplier to the new value.
                    else if (item == powerMultiplier)
                    {
                        // Get the selected value. Remove the "x" from the string.
                        dynamic value = powerMultiplierList[itemIndex].ToString().Replace("x", "");
                        // Conver the string into a float and set it to be the value of the public variable.
                        VehiclePowerMultiplierAmount = float.Parse(value);
                        if (VehiclePowerMultiplier)
                        {
                            SetVehicleEnginePowerMultiplier(veh.Handle, VehiclePowerMultiplierAmount);
                        }
                    }
                    else if (item == setLicensePlateType)
                    {
                        // Set the license plate style.
                        switch (itemIndex)
                        {
                            case 0:
                                veh.Mods.LicensePlateStyle = LicensePlateStyle.BlueOnWhite1;
                                break;
                            case 1:
                                veh.Mods.LicensePlateStyle = LicensePlateStyle.BlueOnWhite2;
                                break;
                            case 2:
                                veh.Mods.LicensePlateStyle = LicensePlateStyle.BlueOnWhite3;
                                break;
                            case 3:
                                veh.Mods.LicensePlateStyle = LicensePlateStyle.YellowOnBlue;
                                break;
                            case 4:
                                veh.Mods.LicensePlateStyle = LicensePlateStyle.YellowOnBlack;
                                break;
                            case 5:
                                veh.Mods.LicensePlateStyle = LicensePlateStyle.NorthYankton;
                                break;
                            default:
                                break;
                        }
                    }
                }
            };
            #endregion

            #region Handle List Items Selected
            menu.OnListSelect += async (sender, item, itemIndex) =>
            {
                // Set dirt level
                if (item == setDirtLevel)
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        GetVehicle().DirtLevel = float.Parse(item.Index.ToString());
                    }
                    else
                    {
                        Notify.Error(CommonErrors.NoVehicle);
                    }
                }
                // Toggle vehicle lights
                else if (item == vehicleLights)
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        Vehicle veh = GetVehicle();
                        // We need to do % 4 because this seems to be some sort of flags system. For a taxi, this function returns 65, 66, etc.
                        // So % 4 takes care of that.
                        int state = GetVehicleIndicatorLights(veh.Handle) % 4; // 0 = none, 1 = left, 2 = right, 3 = both

                        if (item.Index == 0) // Hazard lights
                        {
                            if (state != 3) // either all lights are off, or one of the two (left/right) is off.
                            {
                                SetVehicleIndicatorLights(veh.Handle, 1, true); // left on
                                SetVehicleIndicatorLights(veh.Handle, 0, true); // right on
                            }
                            else // both are on.
                            {
                                SetVehicleIndicatorLights(veh.Handle, 1, false); // left off
                                SetVehicleIndicatorLights(veh.Handle, 0, false); // right off
                            }
                        }
                        else if (item.Index == 1) // left indicator
                        {
                            if (state != 1) // Left indicator is (only) off
                            {
                                SetVehicleIndicatorLights(veh.Handle, 1, true); // left on
                                SetVehicleIndicatorLights(veh.Handle, 0, false); // right off
                            }
                            else
                            {
                                SetVehicleIndicatorLights(veh.Handle, 1, false); // left off
                                SetVehicleIndicatorLights(veh.Handle, 0, false); // right off
                            }
                        }
                        else if (item.Index == 2) // right indicator
                        {
                            if (state != 2) // Right indicator (only) is off
                            {
                                SetVehicleIndicatorLights(veh.Handle, 1, false); // left off
                                SetVehicleIndicatorLights(veh.Handle, 0, true); // right on
                            }
                            else
                            {
                                SetVehicleIndicatorLights(veh.Handle, 1, false); // left off
                                SetVehicleIndicatorLights(veh.Handle, 0, false); // right off
                            }
                        }
                        else if (item.Index == 3) // Interior lights
                        {
                            SetVehicleInteriorlight(veh.Handle, !IsVehicleInteriorLightOn(veh.Handle));
                            //CommonFunctions.Log("Something cool here.");
                        }
                        //else if (item.Index == 4) // taxi light
                        //{
                        //    veh.IsTaxiLightOn = !veh.IsTaxiLightOn;
                        //    //    SetTaxiLights(veh, true);
                        //    //    SetTaxiLights(veh, false);
                        //    //    //CommonFunctions.Log(IsTaxiLightOn(veh).ToString());
                        //    //    //SetTaxiLights(veh, true);
                        //    //    //CommonFunctions.Log(IsTaxiLightOn(veh).ToString());
                        //    //    //SetTaxiLights(veh, false);
                        //    //    //SetTaxiLights(veh, !IsTaxiLightOn(veh));
                        //    //    CommonFunctions.Log
                        //}
                        else if (item.Index == 4) // helicopter spotlight
                        {
                            SetVehicleSearchlight(veh.Handle, !IsVehicleSearchlightOn(veh.Handle), true);
                        }
                    }
                    else
                    {
                        Notify.Error(CommonErrors.NoVehicle);
                    }
                }
                // Speed Limiter
                else if (item == speedLimiter)
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        Vehicle vehicle = GetVehicle();

                        if (vehicle != null && vehicle.Exists())
                        {
                            if (item.Index == 0) // Set
                            {
                                SetEntityMaxSpeed(vehicle.Handle, 500.01f);
                                SetEntityMaxSpeed(vehicle.Handle, vehicle.Speed);

                                if (ShouldUseMetricMeasurements()) // kph
                                {
                                    Notify.Info($"Vehicle speed is now limited to ~b~{Math.Round(vehicle.Speed * 3.6f, 1)} KPH~s~.");
                                }
                                else // mph
                                {
                                    Notify.Info($"Vehicle speed is now limited to ~b~{Math.Round(vehicle.Speed * 2.237f, 1)} MPH~s~.");
                                }

                            }
                            else if (item.Index == 1) // Reset
                            {
                                SetEntityMaxSpeed(vehicle.Handle, 500.01f); // Default max speed seemingly for all vehicles.
                                Notify.Info("Vehicle speed is now no longer limited.");
                            }
                            else if (item.Index == 2) // custom speed
                            {
                                string inputSpeed = await GetUserInput("Enter a speed (in meters/sec)", "20.0", 5);
                                if (!string.IsNullOrEmpty(inputSpeed))
                                {
                                    if (float.TryParse(inputSpeed, out float outFloat))
                                    {
                                        //vehicle.MaxSpeed = outFloat;
                                        SetEntityMaxSpeed(vehicle.Handle, 500.01f);
                                        await BaseScript.Delay(0);
                                        SetEntityMaxSpeed(vehicle.Handle, outFloat + 0.01f);
                                        if (ShouldUseMetricMeasurements()) // kph
                                        {
                                            Notify.Info($"Vehicle speed is now limited to ~b~{Math.Round(outFloat * 3.6f, 1)} KPH~s~.");
                                        }
                                        else // mph
                                        {
                                            Notify.Info($"Vehicle speed is now limited to ~b~{Math.Round(outFloat * 2.237f, 1)} MPH~s~.");
                                        }
                                    }
                                    else if (int.TryParse(inputSpeed, out int outInt))
                                    {
                                        SetEntityMaxSpeed(vehicle.Handle, 500.01f);
                                        await BaseScript.Delay(0);
                                        SetEntityMaxSpeed(vehicle.Handle, outInt + 0.01f);
                                        if (ShouldUseMetricMeasurements()) // kph
                                        {
                                            Notify.Info($"Vehicle speed is now limited to ~b~{Math.Round(outInt * 3.6f, 1)} KPH~s~.");
                                        }
                                        else // mph
                                        {
                                            Notify.Info($"Vehicle speed is now limited to ~b~{Math.Round(outInt * 2.237f, 1)} MPH~s~.");
                                        }
                                    }
                                    else
                                    {
                                        Notify.Error("This is not a valid number. Please enter a valid speed in meters per second.");
                                    }
                                }
                                else
                                {
                                    Notify.Error(CommonErrors.InvalidInput);
                                }
                            }
                        }
                    }
                }
                else if (item == vehicleTiresList)
                {
                    //bool fix = item == vehicleTiresList;

                    Vehicle veh = GetVehicle();
                    if (veh != null && veh.Exists())
                    {
                        if (Game.PlayerPed == veh.Driver)
                        {
                            if (item.Index == 0)
                            {
                                if (IsVehicleTyreBurst(veh.Handle, 0, false))
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        SetVehicleTyreFixed(veh.Handle, i);
                                    }
                                    Notify.Success("All vehicle tyres have been fixed.");
                                }
                                else
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        SetVehicleTyreBurst(veh.Handle, i, false, 1f);
                                    }
                                    Notify.Success("All vehicle tyres have been destroyed.");
                                }
                            }
                            else
                            {
                                int index = item.Index - 1;
                                if (IsVehicleTyreBurst(veh.Handle, index, false))
                                {
                                    SetVehicleTyreFixed(veh.Handle, index);
                                    Notify.Success($"Vehicle tyre #{item.Index} has been fixed.");
                                }
                                else
                                {
                                    SetVehicleTyreBurst(veh.Handle, index, false, 1f);
                                    Notify.Success($"Vehicle tyre #{item.Index} has been destroyed.");
                                }
                            }
                        }
                        else
                        {
                            Notify.Error(CommonErrors.NeedToBeTheDriver);
                        }
                    }
                    else
                    {
                        Notify.Error(CommonErrors.NoVehicle);
                    }
                }
                else if (item == radioStations)
                {
                    RadioStation newStation = (RadioStation)Enum.GetValues(typeof(RadioStation)).GetValue(item.Index);

                    Vehicle veh = GetVehicle();
                    if (veh != null && veh.Exists())
                    {
                        veh.RadioStation = newStation;
                    }

                    UserDefaults.VehicleDefaultRadio = (int)newStation;
                }
            };
            #endregion

            #region Vehicle Colors Submenu Stuff
            // primary menu
            UIMenu primaryColorsMenu = new UIMenu("Vehicle Colors", "Primary Colors");

            UIMenuItem primaryColorsBtn = new UIMenuItem("Primary Color");
            primaryColorsBtn.SetRightLabel("→→→");
            VehicleColorsMenu.AddItem(primaryColorsBtn);
            primaryColorsBtn.Activated += async (a, b) => await a.SwitchTo(primaryColorsMenu, 0, true);

            // secondary menu
            UIMenu secondaryColorsMenu = new UIMenu("Vehicle Colors", "Secondary Colors");

            UIMenuItem secondaryColorsBtn = new UIMenuItem("Secondary Color");
            secondaryColorsBtn.SetRightLabel("→→→");
            VehicleColorsMenu.AddItem(secondaryColorsBtn);
            secondaryColorsBtn.Activated += async (a, b) => await a.SwitchTo(secondaryColorsMenu, 0, true);

            // color lists
            List<dynamic> classic = new List<dynamic>();
            List<dynamic> matte = new List<dynamic>();
            List<dynamic> metals = new List<dynamic>();
            List<dynamic> util = new List<dynamic>();
            List<dynamic> worn = new List<dynamic>();
            List<dynamic> chameleon = new List<dynamic>();
            List<dynamic> wheelColors = new List<dynamic>() { "Default Alloy" };

            // Just quick and dirty solution to put this in a new enclosed section so that we can still use 'i' as a counter in the other code parts.
            {
                int i = 0;
                foreach (VehicleData.VehicleColor vc in VehicleData.ClassicColors)
                {
                    classic.Add($"{GetLabelText(vc.label)} ({i + 1}/{VehicleData.ClassicColors.Count})");
                    i++;
                }

                i = 0;
                foreach (VehicleData.VehicleColor vc in VehicleData.MatteColors)
                {
                    matte.Add($"{GetLabelText(vc.label)} ({i + 1}/{VehicleData.MatteColors.Count})");
                    i++;
                }

                i = 0;
                foreach (VehicleData.VehicleColor vc in VehicleData.MetalColors)
                {
                    metals.Add($"{GetLabelText(vc.label)} ({i + 1}/{VehicleData.MetalColors.Count})");
                    i++;
                }

                i = 0;
                foreach (VehicleData.VehicleColor vc in VehicleData.UtilColors)
                {
                    util.Add($"{GetLabelText(vc.label)} ({i + 1}/{VehicleData.UtilColors.Count})");
                    i++;
                }

                i = 0;
                foreach (VehicleData.VehicleColor vc in VehicleData.WornColors)
                {
                    worn.Add($"{GetLabelText(vc.label)} ({i + 1}/{VehicleData.WornColors.Count})");
                    i++;
                }

                if (GetSettingsBool(Setting.vmenu_using_chameleon_colours))
                {
                    i = 0;
                    foreach (VehicleData.VehicleColor vc in VehicleData.ChameleonColors)
                    {
                        chameleon.Add($"{GetLabelText(vc.label)} ({i + 1}/{VehicleData.ChameleonColors.Count})");
                        i++;
                    }
                }

                wheelColors.AddRange(classic);
            }

            UIMenuListItem wheelColorsList = new UIMenuListItem("Wheel Color", wheelColors, 0);
            UIMenuListItem dashColorList = new UIMenuListItem("Dashboard Color", classic, 0);
            UIMenuListItem intColorList = new UIMenuListItem("Interior / Trim Color", classic, 0);
            UIMenuSliderItem vehicleEnveffScale = new UIMenuSliderItem("Vehicle Enveff Scale", "This works on certain vehicles only, like the besra for example. It 'fades' certain paint layers.", 0, 20, 10, true);

            UIMenuItem chrome = new UIMenuItem("Chrome");
            VehicleColorsMenu.AddItem(chrome);
            VehicleColorsMenu.AddItem(vehicleEnveffScale);

            VehicleColorsMenu.OnItemSelect += (sender, item, index) =>
            {
                Vehicle veh = GetVehicle();
                if (veh != null && veh.Exists() && !veh.IsDead && veh.Driver == Game.PlayerPed)
                {
                    if (item == chrome)
                    {
                        SetVehicleColours(veh.Handle, 120, 120); // chrome is index 120
                    }
                }
                else
                {
                    Notify.Error("You need to be the driver of a driveable vehicle to change this.");
                }
            };
            VehicleColorsMenu.OnSliderChange += (m, sliderItem, value) =>
            {
                Vehicle veh = GetVehicle();
                if (veh != null && veh.Driver == Game.PlayerPed && !veh.IsDead)
                {
                    if (sliderItem == vehicleEnveffScale)
                    {
                        SetVehicleEnveffScale(veh.Handle, value / 20f);
                    }
                }
                else
                {
                    Notify.Error("You need to be the driver of a driveable vehicle to change this slider.");
                }
            };

            VehicleColorsMenu.AddItem(dashColorList);
            VehicleColorsMenu.AddItem(intColorList);
            VehicleColorsMenu.AddItem(wheelColorsList);

            VehicleColorsMenu.OnListChange += HandleListIndexChanges;

            void HandleListIndexChanges(UIMenu sender, UIMenuListItem listItem, int newIndex)
            {
                int itemIndex = sender.MenuItems.IndexOf(listItem);
                Vehicle veh = GetVehicle();
                if (veh != null && veh.Exists() && !veh.IsDead && veh.Driver == Game.PlayerPed)
                {
                    int primaryColor = 0;
                    int secondaryColor = 0;
                    int pearlColor = 0;
                    int wheelColor = 0;
                    int dashColor = 0;
                    int intColor = 0;

                    GetVehicleColours(veh.Handle, ref primaryColor, ref secondaryColor);
                    GetVehicleExtraColours(veh.Handle, ref pearlColor, ref wheelColor);
                    GetVehicleDashboardColour(veh.Handle, ref dashColor);
                    GetVehicleInteriorColour(veh.Handle, ref intColor);

                    if (sender == primaryColorsMenu)
                    {
                        if (itemIndex == 1)
                        {
                            pearlColor = VehicleData.ClassicColors[newIndex].id;
                        }
                        else
                        {
                            pearlColor = 0;
                        }

                        switch (itemIndex)
                        {
                            case 0:
                            case 1:
                                primaryColor = VehicleData.ClassicColors[newIndex].id;
                                break;
                            case 2:
                                primaryColor = VehicleData.MatteColors[newIndex].id;
                                break;
                            case 3:
                                primaryColor = VehicleData.MetalColors[newIndex].id;
                                break;
                            case 4:
                                primaryColor = VehicleData.UtilColors[newIndex].id;
                                break;
                            case 5:
                                primaryColor = VehicleData.WornColors[newIndex].id;
                                break;
                        }

                        if (GetSettingsBool(Setting.vmenu_using_chameleon_colours))
                        {
                            if (itemIndex == 6)
                            {
                                primaryColor = VehicleData.ChameleonColors[newIndex].id;
                                secondaryColor = VehicleData.ChameleonColors[newIndex].id;

                                SetVehicleModKit(veh.Handle, 0);
                            }
                        }

                        SetVehicleColours(veh.Handle, primaryColor, secondaryColor);
                    }
                    else if (sender == secondaryColorsMenu)
                    {
                        switch (itemIndex)
                        {
                            case 0:
                                pearlColor = VehicleData.ClassicColors[newIndex].id;
                                break;
                            case 1:
                            case 2:
                                secondaryColor = VehicleData.ClassicColors[newIndex].id;
                                break;
                            case 3:
                                secondaryColor = VehicleData.MatteColors[newIndex].id;
                                break;
                            case 4:
                                secondaryColor = VehicleData.MetalColors[newIndex].id;
                                break;
                            case 5:
                                secondaryColor = VehicleData.UtilColors[newIndex].id;
                                break;
                            case 6:
                                secondaryColor = VehicleData.WornColors[newIndex].id;
                                break;
                        }
                        SetVehicleColours(veh.Handle, primaryColor, secondaryColor);
                    }
                    else if (sender == VehicleColorsMenu)
                    {
                        if (listItem == wheelColorsList)
                        {
                            if (newIndex == 0)
                            {
                                wheelColor = 156; // default alloy color.
                            }
                            else
                            {
                                wheelColor = VehicleData.ClassicColors[newIndex - 1].id;
                            }
                        }
                        else if (listItem == dashColorList)
                        {
                            dashColor = VehicleData.ClassicColors[newIndex].id;
                            // sadly these native names are mixed up :/ but ofc it's impossible to fix due to backwards compatibility.
                            // this should actually be called SetVehicleDashboardColour
                            SetVehicleInteriorColour(veh.Handle, dashColor);
                        }
                        else if (listItem == intColorList)
                        {
                            intColor = VehicleData.ClassicColors[newIndex].id;
                            // sadly these native names are mixed up :/ but ofc it's impossible to fix due to backwards compatibility.
                            // this should actually be called SetVehicleInteriorColour
                            SetVehicleDashboardColour(veh.Handle, intColor);
                        }
                    }

                    SetVehicleExtraColours(veh.Handle, pearlColor, wheelColor);
                }
                else
                {
                    Notify.Error("You need to be the driver of a vehicle in order to change the vehicle colors.");
                }
            }

            for (int i = 0; i < 2; i++)
            {
                UIMenuListItem pearlescentList = new UIMenuListItem("Pearlescent", classic, 0);
                UIMenuListItem classicList = new UIMenuListItem("Classic", classic, 0);
                UIMenuListItem metallicList = new UIMenuListItem("Metallic", classic, 0);
                UIMenuListItem matteList = new UIMenuListItem("Matte", matte, 0);
                UIMenuListItem metalList = new UIMenuListItem("Metals", metals, 0);
                UIMenuListItem utilList = new UIMenuListItem("Util", util, 0);
                UIMenuListItem wornList = new UIMenuListItem("Worn", worn, 0);

                if (i == 0)
                {
                    primaryColorsMenu.AddItem(classicList);
                    primaryColorsMenu.AddItem(metallicList);
                    primaryColorsMenu.AddItem(matteList);
                    primaryColorsMenu.AddItem(metalList);
                    primaryColorsMenu.AddItem(utilList);
                    primaryColorsMenu.AddItem(wornList);

                    if (GetSettingsBool(Setting.vmenu_using_chameleon_colours))
                    {
                        UIMenuListItem chameleonList = new UIMenuListItem("Chameleon", chameleon, 0);

                        primaryColorsMenu.AddItem(chameleonList);
                    }

                    primaryColorsMenu.OnListChange += HandleListIndexChanges;
                }
                else
                {
                    secondaryColorsMenu.AddItem(pearlescentList);
                    secondaryColorsMenu.AddItem(classicList);
                    secondaryColorsMenu.AddItem(metallicList);
                    secondaryColorsMenu.AddItem(matteList);
                    secondaryColorsMenu.AddItem(metalList);
                    secondaryColorsMenu.AddItem(utilList);
                    secondaryColorsMenu.AddItem(wornList);

                    secondaryColorsMenu.OnListChange += HandleListIndexChanges;
                }
            }
            #endregion

            #region Vehicle Doors Submenu Stuff
            UIMenuItem openAll = new UIMenuItem("Open All Doors", "Open all vehicle doors.");
            UIMenuItem closeAll = new UIMenuItem("Close All Doors", "Close all vehicle doors.");
            UIMenuItem LF = new UIMenuItem("Left Front Door", "Open/close the left front door.");
            UIMenuItem RF = new UIMenuItem("Right Front Door", "Open/close the right front door.");
            UIMenuItem LR = new UIMenuItem("Left Rear Door", "Open/close the left rear door.");
            UIMenuItem RR = new UIMenuItem("Right Rear Door", "Open/close the right rear door.");
            UIMenuItem HD = new UIMenuItem("Hood", "Open/close the hood.");
            UIMenuItem TR = new UIMenuItem("Trunk", "Open/close the trunk.");
            UIMenuItem E1 = new UIMenuItem("Extra 1", "Open/close the extra door (#1). Note this door is not present on most vehicles.");
            UIMenuItem E2 = new UIMenuItem("Extra 2", "Open/close the extra door (#2). Note this door is not present on most vehicles.");
            UIMenuItem BB = new UIMenuItem("Bomb Bay", "Open/close the bomb bay. Only available on some planes.");
            List<dynamic> doors = new List<dynamic>() { "Front Left", "Front Right", "Rear Left", "Rear Right", "Hood", "Trunk", "Extra 1", "Extra 2" };
            UIMenuListItem removeDoorList = new UIMenuListItem("Remove Door", doors, 0, "Remove a specific vehicle door completely.");
            UIMenuCheckboxItem deleteDoors = new UIMenuCheckboxItem("Delete Removed Doors", false, "When enabled, doors that you remove using the list above will be deleted from the world. If disabled, then the doors will just fall on the ground.");

            VehicleDoorsMenu.AddItem(LF);
            VehicleDoorsMenu.AddItem(RF);
            VehicleDoorsMenu.AddItem(LR);
            VehicleDoorsMenu.AddItem(RR);
            VehicleDoorsMenu.AddItem(HD);
            VehicleDoorsMenu.AddItem(TR);
            VehicleDoorsMenu.AddItem(E1);
            VehicleDoorsMenu.AddItem(E2);
            VehicleDoorsMenu.AddItem(BB);
            VehicleDoorsMenu.AddItem(openAll);
            VehicleDoorsMenu.AddItem(closeAll);
            VehicleDoorsMenu.AddItem(removeDoorList);
            VehicleDoorsMenu.AddItem(deleteDoors);

            VehicleDoorsMenu.OnListSelect += (sender, item, index) =>
            {
                Vehicle veh = GetVehicle();
                if (veh != null && veh.Exists())
                {
                    if (veh.Driver == Game.PlayerPed)
                    {
                        if (item == removeDoorList)
                        {
                            SetVehicleDoorBroken(veh.Handle, index, deleteDoors.Checked);
                        }
                    }
                    else
                    {
                        Notify.Error(CommonErrors.NeedToBeTheDriver);
                    }
                }
                else
                {
                    Notify.Error(CommonErrors.NoVehicle);
                }
            };

            // Handle button presses.
            VehicleDoorsMenu.OnItemSelect += (sender, item, index) =>
            {
                // Get the vehicle.
                Vehicle veh = GetVehicle();
                // If the player is in a vehicle, it's not dead and the player is the driver, continue.
                if (veh != null && veh.Exists() && !veh.IsDead && veh.Driver == Game.PlayerPed)
                {
                    // If button 0-5 are pressed, then open/close that specific index/door.
                    if (index < 8)
                    {
                        // If the door is open.
                        bool open = GetVehicleDoorAngleRatio(veh.Handle, index) > 0.1f;

                        if (open)
                        {
                            // Close the door.
                            SetVehicleDoorShut(veh.Handle, index, false);
                        }
                        else
                        {
                            // Open the door.
                            SetVehicleDoorOpen(veh.Handle, index, false, false);
                        }
                    }
                    // If the index >= 8, and the button is "openAll": open all doors.
                    else if (item == openAll)
                    {
                        // Loop through all doors and open them.
                        for (int door = 0; door < 8; door++)
                        {
                            SetVehicleDoorOpen(veh.Handle, door, false, false);
                        }
                        if (veh.HasBombBay)
                        {
                            veh.OpenBombBay();
                        }
                    }
                    // If the index >= 8, and the button is "closeAll": close all doors.
                    else if (item == closeAll)
                    {
                        // Close all doors.
                        SetVehicleDoorsShut(veh.Handle, false);
                        if (veh.HasBombBay)
                        {
                            veh.CloseBombBay();
                        }
                    }
                    // If bomb bay doors button is pressed and the vehicle has bomb bay doors.
                    else if (item == BB && veh.HasBombBay)
                    {
                        bool bombBayOpen = AreBombBayDoorsOpen(veh.Handle);
                        // If open, close them.
                        if (bombBayOpen)
                        {
                            veh.CloseBombBay();
                        }
                        // Otherwise, open them.
                        else
                        {
                            veh.OpenBombBay();
                        }
                    }
                }
                else
                {
                    Notify.Alert(CommonErrors.NoVehicle, placeholderValue: "to open/close a vehicle door");
                }
            };

            #endregion

            #region Vehicle Windows Submenu Stuff
            UIMenuItem fwu = new UIMenuItem("~y~↑~s~ Roll Front Windows Up", "Roll both front windows up.");
            UIMenuItem fwd = new UIMenuItem("~o~↓~s~ Roll Front Windows Down", "Roll both front windows down.");
            UIMenuItem rwu = new UIMenuItem("~y~↑~s~ Roll Rear Windows Up", "Roll both rear windows up.");
            UIMenuItem rwd = new UIMenuItem("~o~↓~s~ Roll Rear Windows Down", "Roll both rear windows down.");
            VehicleWindowsMenu.AddItem(fwu);
            VehicleWindowsMenu.AddItem(fwd);
            VehicleWindowsMenu.AddItem(rwu);
            VehicleWindowsMenu.AddItem(rwd);
            VehicleWindowsMenu.OnItemSelect += (sender, item, index) =>
            {
                Vehicle veh = GetVehicle();
                if (veh != null && veh.Exists() && !veh.IsDead)
                {
                    if (item == fwu)
                    {
                        RollUpWindow(veh.Handle, 0);
                        RollUpWindow(veh.Handle, 1);
                    }
                    else if (item == fwd)
                    {
                        RollDownWindow(veh.Handle, 0);
                        RollDownWindow(veh.Handle, 1);
                    }
                    else if (item == rwu)
                    {
                        RollUpWindow(veh.Handle, 2);
                        RollUpWindow(veh.Handle, 3);
                    }
                    else if (item == rwd)
                    {
                        RollDownWindow(veh.Handle, 2);
                        RollDownWindow(veh.Handle, 3);
                    }
                }
            };
            #endregion

            #region Vehicle Liveries Submenu Stuff
            menu.OnItemSelect += (sender, item, idex) =>
            {
                // If the liverys menu button is selected.
                if (item == liveriesMenuBtn)
                {
                    // Get the player's vehicle.
                    Vehicle veh = GetVehicle();
                    // If it exists, isn't dead and the player is in the drivers seat continue.
                    if (veh != null && veh.Exists() && !veh.IsDead)
                    {
                        if (veh.Driver == Game.PlayerPed)
                        {
                            VehicleLiveriesMenu.Clear();
                            SetVehicleModKit(veh.Handle, 0);
                            int liveryCount = GetVehicleLiveryCount(veh.Handle);

                            if (liveryCount > 0)
                            {
                                List<dynamic> liveryList = new List<dynamic>();
                                for (int i = 0; i < liveryCount; i++)
                                {
                                    string livery = GetLiveryName(veh.Handle, i);
                                    livery = GetLabelText(livery) != "NULL" ? GetLabelText(livery) : $"Livery #{i}";
                                    liveryList.Add(livery);
                                }
                                UIMenuListItem liveryListItem = new UIMenuListItem("Set Livery", liveryList, GetVehicleLivery(veh.Handle), "Choose a livery for this vehicle.");
                                VehicleLiveriesMenu.AddItem(liveryListItem);
                                VehicleLiveriesMenu.OnListChange += (_menu, listItem, newIndex) =>
                                {
                                    if (listItem == liveryListItem)
                                    {
                                        veh = GetVehicle();
                                        SetVehicleLivery(veh.Handle, newIndex);
                                    }
                                };
                                //VehicleLiveriesMenu.UpdateScaleform();
                            }
                            else
                            {
                                Notify.Error("This vehicle does not have any liveries.");
                                VehicleLiveriesMenu.Visible = false;
                                menu.Visible = true;
                                UIMenuItem backBtn = new UIMenuItem("No Liveries Available :(", "Click me to go back.");
                                backBtn.SetRightLabel("Go Back");
                                VehicleLiveriesMenu.AddItem(backBtn);
                                VehicleLiveriesMenu.OnItemSelect += (sender2, item2, index2) =>
                                {
                                    if (item2 == backBtn)
                                    {
                                        VehicleLiveriesMenu.GoBack();
                                    }
                                };

                                //VehicleLiveriesMenu.UpdateScaleform();
                            }
                        }
                        else
                        {
                            Notify.Error("You have to be the driver of a vehicle to access this menu.");
                        }
                    }
                    else
                    {
                        Notify.Error("You have to be the driver of a vehicle to access this menu.");
                    }
                }
            };
            #endregion

            #region Vehicle Mod Submenu Stuff
            menu.OnItemSelect += (sender, item, index) =>
            {
                // When the mod submenu is openend, reset all items in there.
                if (item == modMenuBtn)
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        UpdateMods();
                    }
                    else
                    {
                        VehicleModMenu.Visible = false;
                        menu.Visible = true;
                    }

                }
            };
            #endregion

            #region Vehicle Components Submenu
            // when the components menu is opened.
            menu.OnItemSelect += (sender, item, index) =>
            {
                // If the components menu is opened.
                if (item == componentsMenuBtn)
                {
                    // Empty the menu in case there were leftover buttons from another vehicle.
                    if (VehicleComponentsMenu.Size > 0)
                    {
                        VehicleComponentsMenu.Clear();
                        vehicleExtras.Clear();
                        //VehicleComponentsMenu.UpdateScaleform();
                    }

                    // Get the vehicle.
                    Vehicle veh = GetVehicle();

                    // Check if the vehicle exists, it's actually a vehicle, it's not dead/broken and the player is in the drivers seat.
                    if (veh != null && veh.Exists() && !veh.IsDead && veh.Driver == Game.PlayerPed)
                    {
                        //List<int> extraIds = new List<int>();
                        // Loop through all possible extra ID's (AFAIK: 0-14).
                        for (int extra = 0; extra < 14; extra++)
                        {
                            // If this extra exists...
                            if (veh.ExtraExists(extra))
                            {
                                // Add it's ID to the list.
                                //extraIds.Add(extra);

                                // Create a checkbox for it.
                                UIMenuCheckboxItem extraCheckbox = new UIMenuCheckboxItem($"Extra #{extra}", veh.IsExtraOn(extra), extra.ToString());
                                // Add the checkbox to the menu.
                                VehicleComponentsMenu.AddItem(extraCheckbox);

                                // Add it's ID to the dictionary.
                                vehicleExtras[extraCheckbox] = extra;
                            }
                        }



                        if (vehicleExtras.Count > 0)
                        {
                            UIMenuItem backBtn = new UIMenuItem("Go Back", "Go back to the Vehicle Options menu.");
                            VehicleComponentsMenu.AddItem(backBtn);
                            VehicleComponentsMenu.OnItemSelect += (sender3, item3, index3) =>
                            {
                                VehicleComponentsMenu.GoBack();
                            };
                        }
                        else
                        {
                            UIMenuItem backBtn = new UIMenuItem("No Extras Available :(", "Go back to the Vehicle Options menu.");
                            backBtn.SetRightLabel("Go Back");
                            VehicleComponentsMenu.AddItem(backBtn);
                            VehicleComponentsMenu.OnItemSelect += (sender3, item3, index3) =>
                            {
                                VehicleComponentsMenu.GoBack();
                            };
                        }
                        // And update the submenu to prevent weird glitches.
                        //VehicleComponentsMenu.UpdateScaleform();

                    }
                }
            };
            // when a checkbox in the components menu changes
            VehicleComponentsMenu.OnCheckboxChange += (sender, item, _checked) =>
            {
                // When a checkbox is checked/unchecked, get the selected checkbox item index and use that to get the component ID from the list.
                // Then toggle that extra.
                if (vehicleExtras.TryGetValue(item, out int extra))
                {
                    Vehicle veh = GetVehicle();
                    veh.ToggleExtra(extra, _checked);
                }
            };
            #endregion

            #region Underglow Submenu
            UIMenuCheckboxItem underglowFront = new UIMenuCheckboxItem("Enable Front Light", false, "Enable or disable the underglow on the front side of the vehicle. Note not all vehicles have lights.");
            UIMenuCheckboxItem underglowBack = new UIMenuCheckboxItem("Enable Rear Light", false, "Enable or disable the underglow on the left side of the vehicle. Note not all vehicles have lights.");
            UIMenuCheckboxItem underglowLeft = new UIMenuCheckboxItem("Enable Left Light", false, "Enable or disable the underglow on the right side of the vehicle. Note not all vehicles have lights.");
            UIMenuCheckboxItem underglowRight = new UIMenuCheckboxItem("Enable Right Light", false, "Enable or disable the underglow on the back side of the vehicle. Note not all vehicles have lights.");
            List<dynamic> underglowColorsList = new List<dynamic>();
            for (int i = 0; i < 13; i++)
            {
                underglowColorsList.Add(GetLabelText($"CMOD_NEONCOL_{i}"));
            }
            UIMenuListItem underglowColor = new UIMenuListItem(GetLabelText("CMOD_NEON_1"), underglowColorsList, 0, "Select the color of the neon underglow.");

            VehicleUnderglowMenu.AddItem(underglowFront);
            VehicleUnderglowMenu.AddItem(underglowBack);
            VehicleUnderglowMenu.AddItem(underglowLeft);
            VehicleUnderglowMenu.AddItem(underglowRight);

            VehicleUnderglowMenu.AddItem(underglowColor);

            menu.OnItemSelect += (sender, item, index) =>
            {
                #region reset checkboxes state when opening the menu.
                if (item == underglowMenuBtn)
                {
                    Vehicle veh = GetVehicle();
                    if (veh != null)
                    {
                        if (veh.Mods.HasNeonLights)
                        {
                            underglowFront.Checked = veh.Mods.HasNeonLight(VehicleNeonLight.Front) && veh.Mods.IsNeonLightsOn(VehicleNeonLight.Front);
                            underglowBack.Checked = veh.Mods.HasNeonLight(VehicleNeonLight.Back) && veh.Mods.IsNeonLightsOn(VehicleNeonLight.Back);
                            underglowLeft.Checked = veh.Mods.HasNeonLight(VehicleNeonLight.Left) && veh.Mods.IsNeonLightsOn(VehicleNeonLight.Left);
                            underglowRight.Checked = veh.Mods.HasNeonLight(VehicleNeonLight.Right) && veh.Mods.IsNeonLightsOn(VehicleNeonLight.Right);

                            underglowFront.Enabled = true;
                            underglowBack.Enabled = true;
                            underglowLeft.Enabled = true;
                            underglowRight.Enabled = true;

                            underglowFront.SetLeftBadge(BadgeIcon.NONE);
                            underglowBack.SetLeftBadge(BadgeIcon.NONE);
                            underglowLeft.SetLeftBadge(BadgeIcon.NONE);
                            underglowRight.SetLeftBadge(BadgeIcon.NONE);
                        }
                        else
                        {
                            underglowFront.Checked = false;
                            underglowBack.Checked = false;
                            underglowLeft.Checked = false;
                            underglowRight.Checked = false;

                            underglowFront.Enabled = false;
                            underglowBack.Enabled = false;
                            underglowLeft.Enabled = false;
                            underglowRight.Enabled = false;

                            underglowFront.SetLeftBadge(BadgeIcon.LOCK);
                            underglowBack.SetLeftBadge(BadgeIcon.LOCK);
                            underglowLeft.SetLeftBadge(BadgeIcon.LOCK);
                            underglowRight.SetLeftBadge(BadgeIcon.LOCK);
                        }
                    }
                    else
                    {
                        underglowFront.Checked = false;
                        underglowBack.Checked = false;
                        underglowLeft.Checked = false;
                        underglowRight.Checked = false;

                        underglowFront.Enabled = false;
                        underglowBack.Enabled = false;
                        underglowLeft.Enabled = false;
                        underglowRight.Enabled = false;

                        underglowFront.SetLeftBadge(BadgeIcon.LOCK);
                        underglowBack.SetLeftBadge(BadgeIcon.LOCK);
                        underglowLeft.SetLeftBadge(BadgeIcon.LOCK);
                        underglowRight.SetLeftBadge(BadgeIcon.LOCK);
                    }

                    underglowColor.Index = GetIndexFromColor();
                }
                #endregion
            };
            // handle item selections
            VehicleUnderglowMenu.OnCheckboxChange += (sender, item, _checked) =>
            {
                if (Game.PlayerPed.IsInVehicle())
                {
                    Vehicle veh = GetVehicle();
                    if (veh.Mods.HasNeonLights)
                    {
                        veh.Mods.NeonLightsColor = GetColorFromIndex(underglowColor.Index);
                        if (item == underglowLeft)
                        {
                            veh.Mods.SetNeonLightsOn(VehicleNeonLight.Left, veh.Mods.HasNeonLight(VehicleNeonLight.Left) && _checked);
                        }
                        else if (item == underglowRight)
                        {
                            veh.Mods.SetNeonLightsOn(VehicleNeonLight.Right, veh.Mods.HasNeonLight(VehicleNeonLight.Right) && _checked);
                        }
                        else if (item == underglowBack)
                        {
                            veh.Mods.SetNeonLightsOn(VehicleNeonLight.Back, veh.Mods.HasNeonLight(VehicleNeonLight.Back) && _checked);
                        }
                        else if (item == underglowFront)
                        {
                            veh.Mods.SetNeonLightsOn(VehicleNeonLight.Front, veh.Mods.HasNeonLight(VehicleNeonLight.Front) && _checked);
                        }
                    }
                }
            };

            VehicleUnderglowMenu.OnListChange += (sender, item, newIndex) =>
            {
                if (item == underglowColor)
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        Vehicle veh = GetVehicle();
                        if (veh.Mods.HasNeonLights)
                        {
                            veh.Mods.NeonLightsColor = GetColorFromIndex(newIndex);
                        }
                    }
                }
            };
            #endregion

            #region Handle menu-opening refreshing license plate
            menu.OnMenuOpen += (sender, data) =>
            {
                menu.MenuItems.ForEach((item) =>
                {
                    Vehicle veh = GetVehicle(true);

                    if (item == setLicensePlateType && item is UIMenuListItem listItem && veh != null && veh.Exists())
                    {
                        // Set the license plate style.
                        switch (veh.Mods.LicensePlateStyle)
                        {
                            case LicensePlateStyle.BlueOnWhite1:
                                listItem.Index = 0;
                                break;
                            case LicensePlateStyle.BlueOnWhite2:
                                listItem.Index = 1;
                                break;
                            case LicensePlateStyle.BlueOnWhite3:
                                listItem.Index = 2;
                                break;
                            case LicensePlateStyle.YellowOnBlue:
                                listItem.Index = 3;
                                break;
                            case LicensePlateStyle.YellowOnBlack:
                                listItem.Index = 4;
                                break;
                            case LicensePlateStyle.NorthYankton:
                                listItem.Index = 5;
                                break;
                            default:
                                break;
                        }
                    }
                });
            };
            #endregion

        }
        #endregion

        /// <summary>
        /// Public get method for the menu. Checks if the menu exists, if not create the menu first.
        /// </summary>
        /// <returns>Returns the Vehicle Options menu.</returns>
        public UIMenu GetMenu()
        {
            // If menu doesn't exist. Create one.
            if (menu == null)
            {
                CreateMenu();
            }
            // Return the menu.
            return menu;
        }

        #region Update Vehicle Mods Menu
        /// <summary>
        /// Refreshes the mods page. The selectedIndex allows you to go straight to a specific index after refreshing the menu.
        /// This is used because when the wheel type is changed, the menu is refreshed to update the available wheels list.
        /// </summary>
        /// <param name="selectedIndex">Pass this if you want to go straight to a specific mod/index.</param>
        public void UpdateMods(int selectedIndex = 0)
        {
            // If there are items, remove all of them.
            if (VehicleModMenu.Size > 0)
            {
                if (selectedIndex != 0)
                {
                    VehicleModMenu.Clear();
                }
                else
                {
                    VehicleModMenu.Clear();
                }

            }

            // Get the vehicle.
            Vehicle veh = GetVehicle();

            // Check if the vehicle exists, is still drivable/alive and it's actually a vehicle.
            if (veh != null && veh.Exists() && !veh.IsDead)
            {
                #region initial setup & dynamic vehicle mods setup
                // Set the modkit so we can modify the car.
                SetVehicleModKit(veh.Handle, 0);

                // Get all mods available on this vehicle.
                VehicleMod[] mods = veh.Mods.GetAllMods();

                // Loop through all the mods.
                foreach (VehicleMod mod in mods)
                {
                    veh = GetVehicle();

                    // Get the proper localized mod type (suspension, armor, etc) name.
                    string typeName = mod.LocalizedModTypeName;

                    // Create a list to all available upgrades for this modtype.
                    List<dynamic> modlist = new List<dynamic>();

                    // Get the current item index ({current}/{max upgrades})
                    string currentItem = $"[1/{mod.ModCount + 1}]";

                    // Add the stock value for this mod.
                    string name = $"Stock {typeName} {currentItem}";
                    modlist.Add(name);

                    // Loop through all available upgrades for this specific mod type.
                    for (int x = 0; x < mod.ModCount; x++)
                    {
                        // Create the item index.
                        currentItem = $"[{2 + x}/{mod.ModCount + 1}]";

                        // Create the name (again, converting to proper case), then add the name.
                        name = mod.GetLocalizedModName(x) != "" ? $"{ToProperString(mod.GetLocalizedModName(x))} {currentItem}" : $"{typeName} #{x} {currentItem}";
                        modlist.Add(name);
                    }

                    // Create the MenuListItem for this mod type.
                    int currIndex = GetVehicleMod(veh.Handle, (int)mod.ModType) + 1;
                    UIMenuListItem modTypeListItem = new UIMenuListItem(
                        typeName,
                        modlist,
                        currIndex,
                        $"Choose a ~y~{typeName}~s~ upgrade, it will be automatically applied to your vehicle."
                    )
                    {
                        ItemData = (int)mod.ModType
                    };

                    // Add the list item to the menu.
                    VehicleModMenu.AddItem(modTypeListItem);
                }
                #endregion

                #region more variables and setup
                veh = GetVehicle();
                // Create the wheel types list & listitem and add it to the menu.
                List<dynamic> wheelTypes = new List<dynamic>()
                {
                    "Sports",       // 0
                    "Muscle",       // 1
                    "Lowrider",     // 2
                    "SUV",          // 3
                    "Offroad",      // 4
                    "Tuner",        // 5
                    "Bike Wheels",  // 6
                    "High End",     // 7
                    "Benny's (1)",  // 8
                    "Benny's (2)",  // 9
                    "Open Wheel",   // 10
                    "Street"        // 11
                };
                UIMenuListItem vehicleWheelType = new UIMenuListItem("Wheel Type", wheelTypes, MathUtil.Clamp(GetVehicleWheelType(veh.Handle), 0, 11), $"Choose a ~y~wheel type~s~ for your vehicle.");
                if (!veh.Model.IsBoat && !veh.Model.IsHelicopter && !veh.Model.IsPlane && !veh.Model.IsBicycle && !veh.Model.IsTrain)
                {
                    VehicleModMenu.AddItem(vehicleWheelType);
                }

                // Create the checkboxes for some options.
                UIMenuCheckboxItem toggleCustomWheels = new UIMenuCheckboxItem("Toggle Custom Wheels", GetVehicleModVariation(veh.Handle, 23), "Press this to add or remove ~y~custom~s~ wheels.");
                UIMenuCheckboxItem xenonHeadlights = new UIMenuCheckboxItem("Xenon Headlights", IsToggleModOn(veh.Handle, 22), "Enable or disable ~b~xenon ~s~headlights.");
                UIMenuCheckboxItem turbo = new UIMenuCheckboxItem("Turbo", IsToggleModOn(veh.Handle, 18), "Enable or disable the ~y~turbo~s~ for this vehicle.");
                UIMenuCheckboxItem bulletProofTires = new UIMenuCheckboxItem("Bullet Proof Tires", !GetVehicleTyresCanBurst(veh.Handle), "Enable or disable ~y~bullet proof tires~s~ for this vehicle.");
                UIMenuCheckboxItem lowGripTires = new UIMenuCheckboxItem("Low Grip Tires", GetDriftTyresEnabled(veh.Handle), "Enable or disable ~y~low grip tires~s~ for this vehicle.");

                // Add the checkboxes to the menu.
                VehicleModMenu.AddItem(toggleCustomWheels);
                VehicleModMenu.AddItem(xenonHeadlights);
                int currentHeadlightColor = GetHeadlightsColorForVehicle(veh);
                if (currentHeadlightColor is < 0 or > 12)
                {
                    currentHeadlightColor = 13;
                }
                UIMenuListItem headlightColor = new UIMenuListItem("Headlight Color", new List<dynamic>() { "White", "Blue", "Electric Blue", "Mint Green", "Lime Green", "Yellow", "Golden Shower", "Orange", "Red", "Pony Pink", "Hot Pink", "Purple", "Blacklight", "Default Xenon" }, currentHeadlightColor, "New in the Arena Wars GTA V update: Colored headlights. Note you must enable Xenon Headlights first.");
                VehicleModMenu.AddItem(headlightColor);
                VehicleModMenu.AddItem(turbo);
                VehicleModMenu.AddItem(bulletProofTires);
                VehicleModMenu.AddItem(lowGripTires);
                // Create a list of tire smoke options.
                List<dynamic> tireSmokes = new List<dynamic>() { "Red", "Orange", "Yellow", "Gold", "Light Green", "Dark Green", "Light Blue", "Dark Blue", "Purple", "Pink", "Black" };
                Dictionary<string, int[]> tireSmokeColors = new Dictionary<string, int[]>()
                {
                    ["Red"] = new int[] { 244, 65, 65 },
                    ["Orange"] = new int[] { 244, 167, 66 },
                    ["Yellow"] = new int[] { 244, 217, 65 },
                    ["Gold"] = new int[] { 181, 120, 0 },
                    ["Light Green"] = new int[] { 158, 255, 84 },
                    ["Dark Green"] = new int[] { 44, 94, 5 },
                    ["Light Blue"] = new int[] { 65, 211, 244 },
                    ["Dark Blue"] = new int[] { 24, 54, 163 },
                    ["Purple"] = new int[] { 108, 24, 192 },
                    ["Pink"] = new int[] { 192, 24, 172 },
                    ["Black"] = new int[] { 1, 1, 1 }
                };
                int smoker = 0, smokeg = 0, smokeb = 0;
                GetVehicleTyreSmokeColor(veh.Handle, ref smoker, ref smokeg, ref smokeb);
                KeyValuePair<string, int[]> item = tireSmokeColors.ToList().Find((f) => { return f.Value[0] == smoker && f.Value[1] == smokeg && f.Value[2] == smokeb; });
                int index = tireSmokeColors.ToList().IndexOf(item);
                if (index < 0)
                {
                    index = 0;
                }

                UIMenuListItem tireSmoke = new UIMenuListItem("Tire Smoke Color", tireSmokes, index, $"Choose a ~y~tire smoke color~s~ for your vehicle.");
                VehicleModMenu.AddItem(tireSmoke);

                // Create the checkbox to enable/disable the tiresmoke.
                UIMenuCheckboxItem tireSmokeEnabled = new UIMenuCheckboxItem("Tire Smoke", IsToggleModOn(veh.Handle, 20), "Enable or disable ~y~tire smoke~s~ for your vehicle. ~h~~r~Important:~s~ When disabling tire smoke, you'll need to drive around before it takes affect.");
                VehicleModMenu.AddItem(tireSmokeEnabled);

                // Create list for window tint
                List<dynamic> windowTints = new List<dynamic>() { "Stock [1/7]", "None [2/7]", "Limo [3/7]", "Light Smoke [4/7]", "Dark Smoke [5/7]", "Pure Black [6/7]", "Green [7/7]" };
                int currentTint = GetVehicleWindowTint(veh.Handle);
                if (currentTint == -1)
                {
                    currentTint = 4; // stock
                }

                // Convert window tint to the correct index of the list above.
                switch (currentTint)
                {
                    case 0:
                        currentTint = 1; // None
                        break;
                    case 1:
                        currentTint = 5; // Pure Black
                        break;
                    case 2:
                        currentTint = 4; // Dark Smoke
                        break;
                    case 3:
                        currentTint = 3; // Light Smoke
                        break;
                    case 4:
                        currentTint = 0; // Stock
                        break;
                    case 5:
                        currentTint = 2; // Limo
                        break;
                    case 6:
                        currentTint = 6; // Green
                        break;
                    default:
                        break;
                }

                UIMenuListItem windowTint = new UIMenuListItem("Window Tint", windowTints, currentTint, "Apply tint to your windows.");
                VehicleModMenu.AddItem(windowTint);

                #endregion

                #region Checkbox Changes
                // Handle checkbox changes.
                VehicleModMenu.OnCheckboxChange += (sender2, item2, _checked) =>
                {
                    veh = GetVehicle();

                    // Xenon Headlights
                    if (item2 == xenonHeadlights)
                    {
                        ToggleVehicleMod(veh.Handle, 22, _checked);
                    }
                    // Turbo
                    else if (item2 == turbo)
                    {
                        ToggleVehicleMod(veh.Handle, 18, _checked);
                    }
                    // Bullet Proof Tires
                    else if (item2 == bulletProofTires)
                    {
                        SetVehicleTyresCanBurst(veh.Handle, !_checked);
                    }
                    // Low Grip Tyres
                    else if (item2 == lowGripTires)
                    {
                        SetDriftTyresEnabled(veh.Handle, _checked);
                    }
                    // Custom Wheels
                    else if (item2 == toggleCustomWheels)
                    {
                        SetVehicleMod(veh.Handle, 23, GetVehicleMod(veh.Handle, 23), !GetVehicleModVariation(veh.Handle, 23));

                        // If the player is on a motorcycle, also change the back wheels.
                        if (IsThisModelABike((uint)GetEntityModel(veh.Handle)))
                        {
                            SetVehicleMod(veh.Handle, 24, GetVehicleMod(veh.Handle, 24), GetVehicleModVariation(veh.Handle, 23));
                        }
                    }
                    // Toggle Tire Smoke
                    else if (item2 == tireSmokeEnabled)
                    {
                        // If it should be enabled:
                        if (_checked)
                        {
                            // Enable it.
                            ToggleVehicleMod(veh.Handle, 20, true);
                            // Get the selected color values.
                            dynamic r = tireSmokeColors[tireSmokes[tireSmoke.Index]][0];
                            dynamic g = tireSmokeColors[tireSmokes[tireSmoke.Index]][1];
                            dynamic b = tireSmokeColors[tireSmokes[tireSmoke.Index]][2];
                            // Set the color.
                            SetVehicleTyreSmokeColor(veh.Handle, r, g, b);
                        }
                        // If it should be disabled:
                        else
                        {
                            // Set the smoke to white.
                            SetVehicleTyreSmokeColor(veh.Handle, 255, 255, 255);
                            // Disable it.
                            ToggleVehicleMod(veh.Handle, 20, false);
                            // Remove the mod.
                            RemoveVehicleMod(veh.Handle, 20);
                        }
                    }
                };
                #endregion

                #region List Changes
                // Handle list selections
                VehicleModMenu.OnListChange += (sender2, item2, newIndex) =>
                {
                    int itemIndex = sender2.MenuItems.IndexOf(item2);
                    // Get the vehicle and set the mod kit.
                    veh = GetVehicle();
                    SetVehicleModKit(veh.Handle, 0);

                    #region handle the dynamic (vehicle-specific) mods
                    // If the affected list is actually a "dynamically" generated list, continue. If it was one of the manual options, go to else.
                    if (item2.ItemData is int modType)
                    {
                        int selectedUpgrade = item2.Index - 1;
                        bool customWheels = GetVehicleModVariation(veh.Handle, 23);

                        SetVehicleMod(veh.Handle, modType, selectedUpgrade, customWheels);
                    }
                    #endregion
                    // If it was not one of the lists above, then it was one of the manual lists/options selected, 
                    // either: vehicle Wheel Type, tire smoke color, or window tint:
                    #region Handle the items available on all vehicles.
                    // Wheel types
                    else if (item2 == vehicleWheelType)
                    {
                        int vehicleClass = GetVehicleClass(veh.Handle);
                        bool isBikeOrOpenWheel = (newIndex == 6 && veh.Model.IsBike) || (newIndex == 10 && vehicleClass == 22);
                        bool isNotBikeNorOpenWheel = newIndex != 6 && !veh.Model.IsBike && newIndex != 10 && vehicleClass != 22;
                        bool isCorrectVehicleType = isBikeOrOpenWheel || isNotBikeNorOpenWheel;
                        if (!isCorrectVehicleType)
                        {
                            // Go past the index if it's not a bike.
                            if (!veh.Model.IsBike && vehicleClass != 22)
                            {
                                item2.Index++;
                            }
                            // Reset the index to 6 if it is a bike
                            else
                            {
                                item2.Index = veh.Model.IsBike ? 6 : 10;
                            }
                        }
                        // Set the wheel type
                        SetVehicleWheelType(veh.Handle, item2.Index);

                        bool customWheels = GetVehicleModVariation(veh.Handle, 23);

                        // Reset the wheel mod index for front wheels
                        SetVehicleMod(veh.Handle, 23, -1, customWheels);

                        // If the model is a bike, do the same thing for the rear wheels.
                        if (veh.Model.IsBike)
                        {
                            SetVehicleMod(veh.Handle, 24, -1, customWheels);
                        }

                        // Refresh the menu with the item index so that the view doesn't change
                        UpdateMods(selectedIndex: itemIndex);
                    }
                    // Tire smoke
                    else if (item2 == tireSmoke)
                    {
                        // Get the selected color values.
                        dynamic r = tireSmokeColors[tireSmokes[newIndex]][0];
                        dynamic g = tireSmokeColors[tireSmokes[newIndex]][1];
                        dynamic b = tireSmokeColors[tireSmokes[newIndex]][2];

                        // Set the color.
                        SetVehicleTyreSmokeColor(veh.Handle, r, g, b);
                    }
                    // Window Tint
                    else if (item2 == windowTint)
                    {
                        // Stock = 4,
                        // None = 0,
                        // Limo = 5,
                        // LightSmoke = 3,
                        // DarkSmoke = 2,
                        // PureBlack = 1,
                        // Green = 6,

                        switch (newIndex)
                        {
                            case 1:
                                SetVehicleWindowTint(veh.Handle, 0); // None
                                break;
                            case 2:
                                SetVehicleWindowTint(veh.Handle, 5); // Limo
                                break;
                            case 3:
                                SetVehicleWindowTint(veh.Handle, 3); // Light Smoke
                                break;
                            case 4:
                                SetVehicleWindowTint(veh.Handle, 2); // Dark Smoke
                                break;
                            case 5:
                                SetVehicleWindowTint(veh.Handle, 1); // Pure Black
                                break;
                            case 6:
                                SetVehicleWindowTint(veh.Handle, 6); // Green
                                break;
                            case 0:
                            default:
                                SetVehicleWindowTint(veh.Handle, 4); // Stock
                                break;
                        }
                    }
                    else if (item2 == headlightColor)
                    {
                        if (newIndex == 13) // default
                        {
                            SetHeadlightsColorForVehicle(veh, 255);
                        }
                        else if (newIndex is > (-1) and < 13)
                        {
                            SetHeadlightsColorForVehicle(veh, newIndex);
                        }
                    }
                    #endregion
                };

                #endregion
            }
            // Refresh Index and update the scaleform to prevent weird broken menus.
            if (selectedIndex == 0)
            {
            }

            //VehicleModMenu.UpdateScaleform();

            // Set the selected index to the provided index (0 by default)
            // Used for example, when the wheelstype is changed, the menu is refreshed and we want to set the
            // selected item back to the "wheelsType" list so the user doesn't have to scroll down each time they
            // change the wheels type.
            //VehicleModMenu.CurrentIndex = selectedIndex;
        }

        internal static void SetHeadlightsColorForVehicle(Vehicle veh, int newIndex)
        {

            if (veh != null && veh.Exists() && veh.Driver == Game.PlayerPed)
            {
                if (newIndex is > (-1) and < 13)
                {
                    SetVehicleHeadlightsColour(veh.Handle, newIndex);
                }
                else
                {
                    SetVehicleHeadlightsColour(veh.Handle, -1);
                }
            }
        }

        internal static int GetHeadlightsColorForVehicle(Vehicle vehicle)
        {
            if (vehicle != null && vehicle.Exists())
            {
                if (IsToggleModOn(vehicle.Handle, 22))
                {
                    int val = GetVehicleHeadlightsColour(vehicle.Handle);
                    if (val is > (-1) and < 13)
                    {
                        return val;
                    }
                    return -1;
                }
            }
            return -1;
        }
        #endregion

        #region GetColorFromIndex function (underglow)

        private readonly List<int[]> _VehicleNeonLightColors = new()
        {
            { new int[3] { 255, 255, 255 } },   // White
            { new int[3] { 2, 21, 255 } },      // Blue
            { new int[3] { 3, 83, 255 } },      // Electric blue
            { new int[3] { 0, 255, 140 } },     // Mint Green
            { new int[3] { 94, 255, 1 } },      // Lime Green
            { new int[3] { 255, 255, 0 } },     // Yellow
            { new int[3] { 255, 150, 5 } },     // Golden Shower
            { new int[3] { 255, 62, 0 } },      // Orange
            { new int[3] { 255, 0, 0 } },       // Red
            { new int[3] { 255, 50, 100 } },    // Pony Pink
            { new int[3] { 255, 5, 190 } },     // Hot Pink
            { new int[3] { 35, 1, 255 } },      // Purple
            { new int[3] { 15, 3, 255 } },      // Blacklight
        };

        /// <summary>
        /// Converts a list index to a <see cref="System.Drawing.Color"/> struct.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private System.Drawing.Color GetColorFromIndex(int index)
        {
            if (index is >= 0 and < 13)
            {
                return System.Drawing.Color.FromArgb(_VehicleNeonLightColors[index][0], _VehicleNeonLightColors[index][1], _VehicleNeonLightColors[index][2]);
            }
            return System.Drawing.Color.FromArgb(255, 255, 255);
        }

        /// <summary>
        /// Returns the color index that is applied on the current vehicle. 
        /// If a color is active on the vehicle which is not in the list, it'll return the default index 0 (white).
        /// </summary>
        /// <returns></returns>
        private int GetIndexFromColor()
        {
            Vehicle veh = GetVehicle();

            if (veh == null || !veh.Exists() || !veh.Mods.HasNeonLights)
            {
                return 0;
            }

            int r = 255, g = 255, b = 255;

            GetVehicleNeonLightsColour(veh.Handle, ref r, ref g, ref b);

            if (r == 255 && g == 0 && b == 255) // default return value when the vehicle has no neon kit selected.
            {
                return 0;
            }

            if (_VehicleNeonLightColors.Any(a => { return a[0] == r && a[1] == g && a[2] == b; }))
            {
                return _VehicleNeonLightColors.FindIndex(a => { return a[0] == r && a[1] == g && a[2] == b; });
            }

            return 0;
        }
        #endregion
    }
}
