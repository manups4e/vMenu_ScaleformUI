using ScaleformUI.Menu;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using vMenuClient.data;

namespace vMenuClient.menus
{
    public class MiscSettings
    {
        // Variables
        private UIMenu menu;
        private UIMenu teleportOptionsMenu;
        private UIMenu developerToolsMenu;
        private UIMenu entitySpawnerMenu;

        public bool ShowSpeedoKmh { get; private set; } = UserDefaults.MiscSpeedKmh;
        public bool ShowSpeedoMph { get; private set; } = UserDefaults.MiscSpeedMph;
        public bool ShowCoordinates { get; private set; } = false;
        public bool HideHud { get; private set; } = false;
        public bool HideRadar { get; private set; } = false;
        public bool ShowLocation { get; private set; } = UserDefaults.MiscShowLocation;
        public bool DeathNotifications { get; private set; } = UserDefaults.MiscDeathNotifications;
        public bool JoinQuitNotifications { get; private set; } = UserDefaults.MiscJoinQuitNotifications;
        public bool LockCameraX { get; private set; } = false;
        public bool LockCameraY { get; private set; } = false;
        public bool ShowLocationBlips { get; private set; } = UserDefaults.MiscLocationBlips;
        public bool ShowPlayerBlips { get; private set; } = UserDefaults.MiscShowPlayerBlips;
        public bool MiscShowOverheadNames { get; private set; } = UserDefaults.MiscShowOverheadNames;
        public bool ShowVehicleModelDimensions { get; private set; } = false;
        public bool ShowPedModelDimensions { get; private set; } = false;
        public bool ShowPropModelDimensions { get; private set; } = false;
        public bool ShowEntityHandles { get; private set; } = false;
        public bool ShowEntityModels { get; private set; } = false;
        public bool ShowEntityNetOwners { get; private set; } = false;
        public bool MiscRespawnDefaultCharacter { get; private set; } = UserDefaults.MiscRespawnDefaultCharacter;
        public bool RestorePlayerAppearance { get; private set; } = UserDefaults.MiscRestorePlayerAppearance;
        public bool RestorePlayerWeapons { get; private set; } = UserDefaults.MiscRestorePlayerWeapons;
        public bool DrawTimeOnScreen { get; internal set; } = UserDefaults.MiscShowTime;
        public PointF MiscMenuPosition { get; private set; } = UserDefaults.MiscMenuPosition;
        public bool MiscDisablePrivateMessages { get; private set; } = UserDefaults.MiscDisablePrivateMessages;
        public bool MiscDisableControllerSupport { get; private set; } = UserDefaults.MiscDisableControllerSupport;

        internal bool TimecycleEnabled { get; private set; } = false;
        internal int LastTimeCycleModifierIndex { get; private set; } = UserDefaults.MiscLastTimeCycleModifierIndex;
        internal int LastTimeCycleModifierStrength { get; private set; } = UserDefaults.MiscLastTimeCycleModifierStrength;


        // keybind states
        public bool KbTpToWaypoint { get; private set; } = UserDefaults.KbTpToWaypoint;
        public int KbTpToWaypointKey { get; } = vMenuShared.ConfigManager.GetSettingsInt(vMenuShared.ConfigManager.Setting.vmenu_teleport_to_wp_keybind_key) != -1
            ? vMenuShared.ConfigManager.GetSettingsInt(vMenuShared.ConfigManager.Setting.vmenu_teleport_to_wp_keybind_key)
            : 168; // 168 (F7 by default)
        public bool KbDriftMode { get; private set; } = UserDefaults.KbDriftMode;
        public bool KbRecordKeys { get; private set; } = UserDefaults.KbRecordKeys;
        public bool KbRadarKeys { get; private set; } = UserDefaults.KbRadarKeys;
        public bool KbPointKeys { get; private set; } = UserDefaults.KbPointKeys;
        internal static List<vMenuShared.ConfigManager.TeleportLocation> TpLocations = new();
        public PointF Position = new PointF(50, 50);

        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            if (UserDefaults.MiscMenuPosition.IsEmpty)
            {
                UserDefaults.MiscMenuPosition = new PointF(50, 50);
            }

            // Create the menu.
            menu = new UIMenu(Game.Player.Name, "Misc Settings");
            teleportOptionsMenu = new UIMenu(Game.Player.Name, "Teleport Options");
            developerToolsMenu = new UIMenu(Game.Player.Name, "Development Tools");
            entitySpawnerMenu = new UIMenu(Game.Player.Name, "Entity Spawner");

            // teleport menu
            UIMenu teleportMenu = new UIMenu(Game.Player.Name, "Teleport Locations");
            UIMenuItem teleportMenuBtn = new UIMenuItem("Teleport Locations", "Teleport to pre-configured locations, added by the server owner.");
            teleportMenuBtn.Activated += async (a, b) => await a.SwitchTo(teleportMenu, 0, true);

            // keybind settings menu
            UIMenu keybindMenu = new UIMenu(Game.Player.Name, "Keybind Settings");
            UIMenuItem keybindMenuBtn = new UIMenuItem("Keybind Settings", "Enable or disable keybinds for some options.");
            keybindMenuBtn.Activated += async (a, b) => await a.SwitchTo(keybindMenu, 0, true);

            // keybind settings menu items
            UIMenuCheckboxItem kbTpToWaypoint = new UIMenuCheckboxItem("Teleport To Waypoint", UIMenuCheckboxStyle.Tick, KbTpToWaypoint, "Teleport to your waypoint when pressing the keybind. By default, this keybind is set to ~r~F7~s~, server owners are able to change this however so ask them if you don't know what it is.");
            UIMenuCheckboxItem kbDriftMode = new UIMenuCheckboxItem("Drift Mode", UIMenuCheckboxStyle.Tick, KbDriftMode, "Makes your vehicle have almost no traction while holding left shift on keyboard, or X on controller.");
            UIMenuCheckboxItem kbRecordKeys = new UIMenuCheckboxItem("Recording Controls", UIMenuCheckboxStyle.Tick, KbRecordKeys, "Enables or disables the recording (gameplay recording for the Rockstar editor) hotkeys on both keyboard and controller.");
            UIMenuCheckboxItem kbRadarKeys = new UIMenuCheckboxItem("Minimap Controls", UIMenuCheckboxStyle.Tick, KbRadarKeys, "Press the Multiplayer Info (z on keyboard, down arrow on controller) key to switch between expanded radar and normal radar.");
            UIMenuCheckboxItem kbPointKeysCheckbox = new UIMenuCheckboxItem("Finger Point Controls", UIMenuCheckboxStyle.Tick, KbPointKeys, "Enables the finger point toggle key. The default QWERTY keyboard mapping for this is 'B', or for controller quickly double tap the right analog stick.");
            UIMenuItem backBtn = new UIMenuItem("Back");

            // Create the menu items.
            UIMenuDynamicListItem AlignMenuX = new UIMenuDynamicListItem("Menu X position", "This option will be saved immediately. You don't need to click save preferences.", MiscMenuPosition.X.ToString(), async (sender, direction) =>
            {
                float pos = MiscMenuPosition.X;
                if (direction == UIMenuDynamicListItem.ChangeDirection.Left)
                {
                    pos--;
                    if (pos < 0) pos = 0;
                }
                else
                {
                    pos++;
                    if (pos > 1920 - 288)
                        pos = 1920 - 288;
                }
                UserDefaults.MiscMenuPosition = MiscMenuPosition = new PointF(pos, MiscMenuPosition.Y);
                return pos.ToString();
            });
            UIMenuDynamicListItem AlignMenuY = new UIMenuDynamicListItem("Menu Y position", "This option will be saved immediately. You don't need to click save preferences.", MiscMenuPosition.Y.ToString(), async (sender, direction) =>
            {
                float pos = MiscMenuPosition.Y;
                if (direction == UIMenuDynamicListItem.ChangeDirection.Left)
                {
                    pos--;
                    if (pos < 0) pos = 0;
                }
                else
                {
                    pos++;
                    if (pos > 1080 - 400)
                        pos = 1080 - 400;
                }
                UserDefaults.MiscMenuPosition = MiscMenuPosition = new PointF(MiscMenuPosition.X, pos);
                return pos.ToString();
            });
            UIMenuCheckboxItem disablePms = new UIMenuCheckboxItem("Disable Private Messages", UIMenuCheckboxStyle.Tick, MiscDisablePrivateMessages, "Prevent others from sending you a private message via the Online Players menu. This also prevents you from sending messages to other players.");
            UIMenuCheckboxItem disableControllerKey = new UIMenuCheckboxItem("Disable Controller Support", UIMenuCheckboxStyle.Tick, MiscDisableControllerSupport, "This disables the controller menu toggle key. This does NOT disable the navigation buttons.");
            UIMenuCheckboxItem speedKmh = new UIMenuCheckboxItem("Show Speed KM/H", UIMenuCheckboxStyle.Tick, ShowSpeedoKmh, "Show a speedometer on your screen indicating your speed in KM/h.");
            UIMenuCheckboxItem speedMph = new UIMenuCheckboxItem("Show Speed MPH", UIMenuCheckboxStyle.Tick, ShowSpeedoMph, "Show a speedometer on your screen indicating your speed in MPH.");
            UIMenuCheckboxItem coords = new UIMenuCheckboxItem("Show Coordinates", UIMenuCheckboxStyle.Tick, ShowCoordinates, "Show your current coordinates at the top of your screen.");
            UIMenuCheckboxItem hideRadar = new UIMenuCheckboxItem("Hide Radar", UIMenuCheckboxStyle.Tick, HideRadar, "Hide the radar/minimap.");
            UIMenuCheckboxItem hideHud = new UIMenuCheckboxItem("Hide Hud", UIMenuCheckboxStyle.Tick, HideHud, "Hide all hud elements.");
            UIMenuCheckboxItem showLocation = new UIMenuCheckboxItem("Location Display", UIMenuCheckboxStyle.Tick, ShowLocation, "Shows your current location and heading, as well as the nearest cross road. Similar like PLD. ~r~Warning: This feature (can) take(s) up to -4.6 FPS when running at 60 Hz.");
            showLocation.SetLeftBadge(BadgeIcon.WARNING);
            UIMenuCheckboxItem drawTime = new UIMenuCheckboxItem("Show Time On Screen", UIMenuCheckboxStyle.Tick, DrawTimeOnScreen, "Shows you the current time on screen.");
            UIMenuItem saveSettings = new UIMenuItem("Save Personal Settings", "Save your current settings. All saving is done on the client side, if you re-install windows you will lose your settings. Settings are shared across all servers using vMenu.");
            saveSettings.SetRightBadge(BadgeIcon.TICK);

            UIMenuItem exportData = new UIMenuItem("Export/Import Data", "Coming soon (TM): the ability to import and export your saved data.");
            UIMenuCheckboxItem joinQuitNotifs = new UIMenuCheckboxItem("Join / Quit Notifications", UIMenuCheckboxStyle.Tick, JoinQuitNotifications, "Receive notifications when someone joins or leaves the server.");
            UIMenuCheckboxItem deathNotifs = new UIMenuCheckboxItem("Death Notifications", UIMenuCheckboxStyle.Tick, DeathNotifications, "Receive notifications when someone dies or gets killed.");
            UIMenuCheckboxItem nightVision = new UIMenuCheckboxItem("Toggle Night Vision", UIMenuCheckboxStyle.Tick, false, "Enable or disable night vision.");
            UIMenuCheckboxItem thermalVision = new UIMenuCheckboxItem("Toggle Thermal Vision", UIMenuCheckboxStyle.Tick, false, "Enable or disable thermal vision.");
            UIMenuCheckboxItem vehModelDimensions = new UIMenuCheckboxItem("Show Vehicle Dimensions", UIMenuCheckboxStyle.Tick, ShowVehicleModelDimensions, "Draws the model outlines for every vehicle that's currently close to you.");
            UIMenuCheckboxItem propModelDimensions = new UIMenuCheckboxItem("Show Prop Dimensions", UIMenuCheckboxStyle.Tick, ShowPropModelDimensions, "Draws the model outlines for every prop that's currently close to you.");
            UIMenuCheckboxItem pedModelDimensions = new UIMenuCheckboxItem("Show Ped Dimensions", UIMenuCheckboxStyle.Tick, ShowPedModelDimensions, "Draws the model outlines for every ped that's currently close to you.");
            UIMenuCheckboxItem showEntityHandles = new UIMenuCheckboxItem("Show Entity Handles", UIMenuCheckboxStyle.Tick, ShowEntityHandles, "Draws the the entity handles for all close entities (you must enable the outline functions above for this to work).");
            UIMenuCheckboxItem showEntityModels = new UIMenuCheckboxItem("Show Entity Models", UIMenuCheckboxStyle.Tick, ShowEntityModels, "Draws the the entity models for all close entities (you must enable the outline functions above for this to work).");
            UIMenuCheckboxItem showEntityNetOwners = new UIMenuCheckboxItem("Show Network Owners", UIMenuCheckboxStyle.Tick, ShowEntityNetOwners, "Draws the the entity net owner for all close entities (you must enable the outline functions above for this to work).");
            UIMenuSliderItem dimensionsDistanceSlider = new UIMenuSliderItem("Show Dimensions Radius", "Show entity model/handle/dimension draw range.", 0, 20, 20, false);

            UIMenuItem clearArea = new UIMenuItem("Clear Area", "Clears the area around your player (100 meters). Damage, dirt, peds, props, vehicles, etc. Everything gets cleaned up, fixed and reset to the default world state.");
            UIMenuCheckboxItem lockCamX = new UIMenuCheckboxItem("Lock Camera Horizontal Rotation", UIMenuCheckboxStyle.Tick, false, "Locks your camera horizontal rotation. Could be useful in helicopters I guess.");
            UIMenuCheckboxItem lockCamY = new UIMenuCheckboxItem("Lock Camera Vertical Rotation", UIMenuCheckboxStyle.Tick, false, "Locks your camera vertical rotation. Could be useful in helicopters I guess.");

            // Entity spawner
            UIMenuItem spawnNewEntity = new UIMenuItem("Spawn New Entity", "Spawns entity into the world and lets you set its position and rotation");
            UIMenuItem confirmEntityPosition = new UIMenuItem("Confirm Entity Position", "Stops placing entity and sets it at it current location.");
            UIMenuItem cancelEntity = new UIMenuItem("Cancel", "Deletes current entity and cancels its placement");
            UIMenuItem confirmAndDuplicate = new UIMenuItem("Confirm Entity Position And Duplicate", "Stops placing entity and sets it at it current location and creates new one to place.");

            UIMenu connectionSubmenu = new UIMenu(Game.Player.Name, "Connection Options");
            UIMenuItem connectionSubmenuBtn = new UIMenuItem("Connection Options", "Server connection/game quit options.");

            UIMenuItem quitSession = new UIMenuItem("Quit Session", "Leaves you connected to the server, but quits the network session. ~r~Can not be used when you are the host.");
            UIMenuItem rejoinSession = new UIMenuItem("Re-join Session", "This may not work in all cases, but you can try to use this if you want to re-join the previous session after clicking 'Quit Session'.");
            UIMenuItem quitGame = new UIMenuItem("Quit Game", "Exits the game after 5 seconds.");
            UIMenuItem disconnectFromServer = new UIMenuItem("Disconnect From Server", "Disconnects you from the server and returns you to the serverlist. ~r~This feature is not recommended, quit the game completely instead and restart it for a better experience.");
            connectionSubmenu.AddItem(quitSession);
            connectionSubmenu.AddItem(rejoinSession);
            connectionSubmenu.AddItem(quitGame);
            connectionSubmenu.AddItem(disconnectFromServer);

            UIMenuCheckboxItem enableTimeCycle = new UIMenuCheckboxItem("Enable Timecycle Modifier", TimecycleEnabled, "Enable or disable the timecycle modifier from the list below.");
            List<dynamic> timeCycleModifiersListData = TimeCycles.Timecycles.Cast<dynamic>().ToList();
            for (int i = 0; i < timeCycleModifiersListData.Count; i++)
            {
                timeCycleModifiersListData[i] += $" ({i + 1}/{timeCycleModifiersListData.Count})";
            }
            UIMenuListItem timeCycles = new UIMenuListItem("TM", timeCycleModifiersListData, MathUtil.Clamp(LastTimeCycleModifierIndex, 0, Math.Max(0, timeCycleModifiersListData.Count - 1)), "Select a timecycle modifier and enable the checkbox above.");
            UIMenuSliderItem timeCycleIntensity = new UIMenuSliderItem("Timecycle Modifier Intensity", "Set the timecycle modifier intensity.", 0, 20, LastTimeCycleModifierStrength, true);

            UIMenuCheckboxItem locationBlips = new UIMenuCheckboxItem("Location Blips", UIMenuCheckboxStyle.Tick, ShowLocationBlips, "Shows blips on the map for some common locations.");
            UIMenuCheckboxItem playerBlips = new UIMenuCheckboxItem("Show Player Blips", UIMenuCheckboxStyle.Tick, ShowPlayerBlips, "Shows blips on the map for all players. ~y~Note for when the server is using OneSync Infinity: this won't work for players that are too far away.");
            UIMenuCheckboxItem playerNames = new UIMenuCheckboxItem("Show Player Names", UIMenuCheckboxStyle.Tick, MiscShowOverheadNames, "Enables or disables player overhead names.");
            UIMenuCheckboxItem respawnDefaultCharacter = new UIMenuCheckboxItem("Respawn As Default MP", UIMenuCheckboxStyle.Tick, MiscRespawnDefaultCharacter, "If you enable this, then you will (re)spawn as your default saved MP character. Note the server owner can globally disable this option. To set your default character, go to one of your saved MP Characters and click the 'Set As Default Character' button.");
            UIMenuCheckboxItem restorePlayerAppearance = new UIMenuCheckboxItem("Restore Player Appearance", UIMenuCheckboxStyle.Tick, RestorePlayerAppearance, "Restore your player's skin whenever you respawn after being dead. Re-joining a server will not restore your previous skin.");
            UIMenuCheckboxItem restorePlayerWeapons = new UIMenuCheckboxItem("Restore Player Weapons", UIMenuCheckboxStyle.Tick, RestorePlayerWeapons, "Restore your weapons whenever you respawn after being dead. Re-joining a server will not restore your previous weapons.");

            connectionSubmenuBtn.Activated += async (a, b) => await a.SwitchTo(connectionSubmenu, 0, true);

            keybindMenu.OnCheckboxChange += (sender, item, _checked) =>
            {
                if (item == kbTpToWaypoint)
                {
                    KbTpToWaypoint = _checked;
                }
                else if (item == kbDriftMode)
                {
                    KbDriftMode = _checked;
                }
                else if (item == kbRecordKeys)
                {
                    KbRecordKeys = _checked;
                }
                else if (item == kbRadarKeys)
                {
                    KbRadarKeys = _checked;
                }
                else if (item == kbPointKeysCheckbox)
                {
                    KbPointKeys = _checked;
                }
            };
            keybindMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == backBtn)
                {
                    keybindMenu.GoBack();
                }
            };

            connectionSubmenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == quitGame)
                {
                    QuitGame();
                }
                else if (item == quitSession)
                {
                    if (NetworkIsSessionActive())
                    {
                        if (NetworkIsHost())
                        {
                            Notify.Error("Sorry, you cannot leave the session when you are the host. This would prevent other players from joining/staying on the server.");
                        }
                        else
                        {
                            QuitSession();
                        }
                    }
                    else
                    {
                        Notify.Error("You are currently not in any session.");
                    }
                }
                else if (item == rejoinSession)
                {
                    if (NetworkIsSessionActive())
                    {
                        Notify.Error("You are already connected to a session.");
                    }
                    else
                    {
                        Notify.Info("Attempting to re-join the session.");
                        NetworkSessionHost(-1, 32, false);
                    }
                }
                else if (item == disconnectFromServer)
                {

                    RegisterCommand("disconnect", new Action<dynamic, dynamic, dynamic>((a, b, c) => { }), false);
                    ExecuteCommand("disconnect");
                }
            };

            // Teleportation options
            if (IsAllowed(Permission.MSTeleportToWp) || IsAllowed(Permission.MSTeleportLocations) || IsAllowed(Permission.MSTeleportToCoord))
            {
                UIMenuItem teleportOptionsMenuBtn = new UIMenuItem("Teleport Options", "Various teleport options.");
                teleportOptionsMenuBtn.SetRightLabel("→→→");
                menu.AddItem(teleportOptionsMenuBtn);
                teleportOptionsMenuBtn.Activated += async (a, b) => await a.SwitchTo(teleportOptionsMenu, 0, true);

                UIMenuItem tptowp = new UIMenuItem("Teleport To Waypoint", "Teleport to the waypoint on your map.");
                UIMenuItem tpToCoord = new UIMenuItem("Teleport To Coords", "Enter x, y, z coordinates and you will be teleported to that location.");
                UIMenuItem saveLocationBtn = new UIMenuItem("Save Teleport Location", "Adds your current location to the teleport locations menu and saves it on the server.");
                teleportOptionsMenu.OnItemSelect += async (sender, item, index) =>
                {
                    // Teleport to waypoint.
                    if (item == tptowp)
                    {
                        TeleportToWp();
                    }
                    else if (item == tpToCoord)
                    {
                        string x = await GetUserInput("Enter X coordinate.");
                        if (string.IsNullOrEmpty(x))
                        {
                            Notify.Error(CommonErrors.InvalidInput);
                            return;
                        }
                        string y = await GetUserInput("Enter Y coordinate.");
                        if (string.IsNullOrEmpty(y))
                        {
                            Notify.Error(CommonErrors.InvalidInput);
                            return;
                        }
                        string z = await GetUserInput("Enter Z coordinate.");
                        if (string.IsNullOrEmpty(z))
                        {
                            Notify.Error(CommonErrors.InvalidInput);
                            return;
                        }


                        if (!float.TryParse(x, out float posX))
                        {
                            if (int.TryParse(x, out int intX))
                            {
                                posX = intX;
                            }
                            else
                            {
                                Notify.Error("You did not enter a valid X coordinate.");
                                return;
                            }
                        }
                        if (!float.TryParse(y, out float posY))
                        {
                            if (int.TryParse(y, out int intY))
                            {
                                posY = intY;
                            }
                            else
                            {
                                Notify.Error("You did not enter a valid Y coordinate.");
                                return;
                            }
                        }
                        if (!float.TryParse(z, out float posZ))
                        {
                            if (int.TryParse(z, out int intZ))
                            {
                                posZ = intZ;
                            }
                            else
                            {
                                Notify.Error("You did not enter a valid Z coordinate.");
                                return;
                            }
                        }

                        await TeleportToCoords(new Vector3(posX, posY, posZ), true);
                    }
                    else if (item == saveLocationBtn)
                    {
                        SavePlayerLocationToLocationsFile();
                    }
                };

                if (IsAllowed(Permission.MSTeleportToWp))
                {
                    teleportOptionsMenu.AddItem(tptowp);
                    keybindMenu.AddItem(kbTpToWaypoint);
                }
                if (IsAllowed(Permission.MSTeleportToCoord))
                {
                    teleportOptionsMenu.AddItem(tpToCoord);
                }
                if (IsAllowed(Permission.MSTeleportLocations))
                {
                    teleportOptionsMenu.AddItem(teleportMenuBtn);
                    teleportMenuBtn.Activated += async (a, b) => await a.SwitchTo(teleportMenu, 0, true);
                    teleportMenuBtn.SetRightLabel("→→→");

                    teleportMenu.OnMenuOpen += (sender, data) =>
                    {
                        if (teleportMenu.Size != TpLocations.Count())
                        {
                            teleportMenu.Clear();
                            foreach (TeleportLocation location in TpLocations)
                            {
                                double x = Math.Round(location.coordinates.X, 2);
                                double y = Math.Round(location.coordinates.Y, 2);
                                double z = Math.Round(location.coordinates.Z, 2);
                                double heading = Math.Round(location.heading, 2);
                                UIMenuItem tpBtn = new UIMenuItem(location.name, $"Teleport to ~y~{location.name}~n~~s~x: ~y~{x}~n~~s~y: ~y~{y}~n~~s~z: ~y~{z}~n~~s~heading: ~y~{heading}") { ItemData = location };
                                teleportMenu.AddItem(tpBtn);
                            }
                        }
                    };

                    teleportMenu.OnItemSelect += async (sender, item, index) =>
                    {
                        if (item.ItemData is vMenuShared.ConfigManager.TeleportLocation tl)
                        {
                            await TeleportToCoords(tl.coordinates, true);
                            SetEntityHeading(Game.PlayerPed.Handle, tl.heading);
                            SetGameplayCamRelativeHeading(0f);
                        }
                    };

                    if (IsAllowed(Permission.MSTeleportSaveLocation))
                    {
                        teleportOptionsMenu.AddItem(saveLocationBtn);
                    }
                }

            }

            #region dev tools menu

            UIMenuItem devToolsBtn = new UIMenuItem("Developer Tools", "Various development/debug tools.");
            devToolsBtn.SetRightLabel("→→→");
            menu.AddItem(devToolsBtn);
            devToolsBtn.Activated += async (a, b) => await a.SwitchTo(developerToolsMenu, 0, true);

            // clear area and coordinates
            if (IsAllowed(Permission.MSClearArea))
            {
                developerToolsMenu.AddItem(clearArea);
            }
            if (IsAllowed(Permission.MSShowCoordinates))
            {
                developerToolsMenu.AddItem(coords);
            }

            // model outlines
            if (!vMenuShared.ConfigManager.GetSettingsBool(vMenuShared.ConfigManager.Setting.vmenu_disable_entity_outlines_tool))
            {
                developerToolsMenu.AddItem(vehModelDimensions);
                developerToolsMenu.AddItem(propModelDimensions);
                developerToolsMenu.AddItem(pedModelDimensions);
                developerToolsMenu.AddItem(showEntityHandles);
                developerToolsMenu.AddItem(showEntityModels);
                developerToolsMenu.AddItem(showEntityNetOwners);
                developerToolsMenu.AddItem(dimensionsDistanceSlider);
            }


            // timecycle modifiers
            developerToolsMenu.AddItem(timeCycles);
            developerToolsMenu.AddItem(enableTimeCycle);
            developerToolsMenu.AddItem(timeCycleIntensity);

            developerToolsMenu.OnSliderChange += (sender, item, value) =>
            {
                if (item == timeCycleIntensity)
                {
                    ClearTimecycleModifier();
                    if (TimecycleEnabled)
                    {
                        SetTimecycleModifier(TimeCycles.Timecycles[sender.MenuItems.IndexOf(timeCycles)]);
                        float intensity = value / 20f;
                        SetTimecycleModifierStrength(intensity);
                    }
                    UserDefaults.MiscLastTimeCycleModifierIndex = sender.MenuItems.IndexOf(timeCycles);
                    UserDefaults.MiscLastTimeCycleModifierStrength = timeCycleIntensity.Value;
                }
                else if (item == dimensionsDistanceSlider)
                {
                    FunctionsController.entityRange = value / 20f * 2000f; // max radius = 2000f;
                }
            };

            developerToolsMenu.OnListChange += (sender, item, itemIndex) =>
            {
                if (item == timeCycles)
                {
                    ClearTimecycleModifier();
                    if (TimecycleEnabled)
                    {
                        SetTimecycleModifier(TimeCycles.Timecycles[sender.MenuItems.IndexOf(timeCycles)]);
                        float intensity = timeCycleIntensity.Value / 20f;
                        SetTimecycleModifierStrength(intensity);
                    }
                    UserDefaults.MiscLastTimeCycleModifierIndex = sender.MenuItems.IndexOf(timeCycles);
                    UserDefaults.MiscLastTimeCycleModifierStrength = timeCycleIntensity.Value;
                }
            };

            developerToolsMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == clearArea)
                {
                    Vector3 pos = Game.PlayerPed.Position;
                    BaseScript.TriggerServerEvent("vMenu:ClearArea", pos.X, pos.Y, pos.Z);
                }
            };

            developerToolsMenu.OnCheckboxChange += (sender, item, _checked) =>
            {
                if (item == vehModelDimensions)
                {
                    ShowVehicleModelDimensions = _checked;
                }
                else if (item == propModelDimensions)
                {
                    ShowPropModelDimensions = _checked;
                }
                else if (item == pedModelDimensions)
                {
                    ShowPedModelDimensions = _checked;
                }
                else if (item == showEntityHandles)
                {
                    ShowEntityHandles = _checked;
                }
                else if (item == showEntityModels)
                {
                    ShowEntityModels = _checked;
                }
                else if (item == showEntityNetOwners)
                {
                    ShowEntityNetOwners = _checked;
                }
                else if (item == enableTimeCycle)
                {
                    TimecycleEnabled = _checked;
                    ClearTimecycleModifier();
                    if (TimecycleEnabled)
                    {
                        SetTimecycleModifier(TimeCycles.Timecycles[sender.MenuItems.IndexOf(timeCycles)]);
                        float intensity = timeCycleIntensity.Value / 20f;
                        SetTimecycleModifierStrength(intensity);
                    }
                }
                else if (item == coords)
                {
                    ShowCoordinates = _checked;
                }
            };

            if (IsAllowed(Permission.MSEntitySpawner))
            {
                UIMenuItem entSpawnerMenuBtn = new UIMenuItem("Entity Spawner", "Spawn and move entities");
                entSpawnerMenuBtn.SetRightLabel("→→→");
                developerToolsMenu.AddItem(entSpawnerMenuBtn);
                entSpawnerMenuBtn.Activated += async (a, b) => await a.SwitchTo(entitySpawnerMenu, 0, true);

                entitySpawnerMenu.AddItem(spawnNewEntity);
                entitySpawnerMenu.AddItem(confirmEntityPosition);
                entitySpawnerMenu.AddItem(confirmAndDuplicate);
                entitySpawnerMenu.AddItem(cancelEntity);

                entitySpawnerMenu.OnItemSelect += async (sender, item, index) =>
                {
                    if (item == spawnNewEntity)
                    {
                        if (EntitySpawner.CurrentEntity != null || EntitySpawner.Active)
                        {
                            Notify.Error("You are already placing one entity, set its location or cancel and try again!");
                            return;
                        }

                        string result = await GetUserInput(windowTitle: "Enter model name");

                        if (string.IsNullOrEmpty(result))
                        {
                            Notify.Error(CommonErrors.InvalidInput);
                        }

                        EntitySpawner.SpawnEntity(result, Game.PlayerPed.Position);
                    }
                    else if (item == confirmEntityPosition || item == confirmAndDuplicate)
                    {
                        if (EntitySpawner.CurrentEntity != null)
                        {
                            EntitySpawner.FinishPlacement(item == confirmAndDuplicate);
                        }
                        else
                        {
                            Notify.Error("No entity to confirm position for!");
                        }
                    }
                    else if (item == cancelEntity)
                    {
                        if (EntitySpawner.CurrentEntity != null)
                        {
                            EntitySpawner.CurrentEntity.Delete();
                        }
                        else
                        {
                            Notify.Error("No entity to cancel!");
                        }
                    }
                };
            }

            #endregion


            // Keybind options
            if (IsAllowed(Permission.MSDriftMode))
            {
                keybindMenu.AddItem(kbDriftMode);
            }
            // always allowed keybind menu options
            keybindMenu.AddItem(kbRecordKeys);
            keybindMenu.AddItem(kbRadarKeys);
            keybindMenu.AddItem(kbPointKeysCheckbox);
            keybindMenu.AddItem(backBtn);

            // Always allowed
            menu.AddItem(AlignMenuX);
            menu.AddItem(AlignMenuY);
            menu.AddItem(disablePms);
            menu.AddItem(disableControllerKey);
            menu.AddItem(speedKmh);
            menu.AddItem(speedMph);
            menu.AddItem(keybindMenuBtn);
            keybindMenuBtn.SetRightLabel("→→→");
            if (IsAllowed(Permission.MSConnectionMenu))
            {
                menu.AddItem(connectionSubmenuBtn);
                connectionSubmenuBtn.SetRightLabel("→→→");
            }
            if (IsAllowed(Permission.MSShowLocation))
            {
                menu.AddItem(showLocation);
            }
            menu.AddItem(drawTime); // always allowed
            if (IsAllowed(Permission.MSJoinQuitNotifs))
            {
                menu.AddItem(joinQuitNotifs);
            }
            if (IsAllowed(Permission.MSDeathNotifs))
            {
                menu.AddItem(deathNotifs);
            }
            if (IsAllowed(Permission.MSNightVision))
            {
                menu.AddItem(nightVision);
            }
            if (IsAllowed(Permission.MSThermalVision))
            {
                menu.AddItem(thermalVision);
            }
            if (IsAllowed(Permission.MSLocationBlips))
            {
                menu.AddItem(locationBlips);
                ToggleBlips(ShowLocationBlips);
            }
            if (IsAllowed(Permission.MSPlayerBlips))
            {
                menu.AddItem(playerBlips);
            }
            if (IsAllowed(Permission.MSOverheadNames))
            {
                menu.AddItem(playerNames);
            }
            // always allowed, it just won't do anything if the server owner disabled the feature, but players can still toggle it.
            menu.AddItem(respawnDefaultCharacter);
            if (IsAllowed(Permission.MSRestoreAppearance))
            {
                menu.AddItem(restorePlayerAppearance);
            }
            if (IsAllowed(Permission.MSRestoreWeapons))
            {
                menu.AddItem(restorePlayerWeapons);
            }

            // Always allowed
            menu.AddItem(hideRadar);
            menu.AddItem(hideHud);
            menu.AddItem(lockCamX);
            menu.AddItem(lockCamY);
            if (MainMenu.EnableExperimentalFeatures)
            {
                menu.AddItem(exportData);
            }
            menu.AddItem(saveSettings);

            // Handle checkbox changes.
            menu.OnCheckboxChange += (sender, item, _checked) =>
            {
                if (item == disablePms)
                {
                    MiscDisablePrivateMessages = _checked;
                }
                else if (item == disableControllerKey)
                {
                    MiscDisableControllerSupport = _checked;
                    //MenuController.EnableMenuToggleKeyOnController = !_checked;
                }
                else if (item == speedKmh)
                {
                    ShowSpeedoKmh = _checked;
                }
                else if (item == speedMph)
                {
                    ShowSpeedoMph = _checked;
                }
                else if (item == hideHud)
                {
                    HideHud = _checked;
                    DisplayHud(!_checked);
                }
                else if (item == hideRadar)
                {
                    HideRadar = _checked;
                    if (!_checked)
                    {
                        DisplayRadar(true);
                    }
                }
                else if (item == showLocation)
                {
                    ShowLocation = _checked;
                }
                else if (item == drawTime)
                {
                    DrawTimeOnScreen = _checked;
                }
                else if (item == deathNotifs)
                {
                    DeathNotifications = _checked;
                }
                else if (item == joinQuitNotifs)
                {
                    JoinQuitNotifications = _checked;
                }
                else if (item == nightVision)
                {
                    SetNightvision(_checked);
                }
                else if (item == thermalVision)
                {
                    SetSeethrough(_checked);
                }
                else if (item == lockCamX)
                {
                    LockCameraX = _checked;
                }
                else if (item == lockCamY)
                {
                    LockCameraY = _checked;
                }
                else if (item == locationBlips)
                {
                    ToggleBlips(_checked);
                    ShowLocationBlips = _checked;
                }
                else if (item == playerBlips)
                {
                    ShowPlayerBlips = _checked;
                }
                else if (item == playerNames)
                {
                    MiscShowOverheadNames = _checked;
                }
                else if (item == respawnDefaultCharacter)
                {
                    MiscRespawnDefaultCharacter = _checked;
                }
                else if (item == restorePlayerAppearance)
                {
                    RestorePlayerAppearance = _checked;
                }
                else if (item == restorePlayerWeapons)
                {
                    RestorePlayerWeapons = _checked;
                }

            };

            // Handle button presses.
            menu.OnItemSelect += (sender, item, index) =>
            {
                // export data
                if (item == exportData)
                {
                    MenuHandler.CloseAndClearHistory();
                    Dictionary<string, VehicleInfo> vehicles = GetSavedVehicles();
                    Dictionary<string, PedInfo> normalPeds = StorageManager.GetSavedPeds();
                    List<MpPedDataManager.MultiplayerPedData> mpPeds = StorageManager.GetSavedMpPeds();
                    Dictionary<string, List<ValidWeapon>> weaponLoadouts = WeaponLoadouts.GetSavedWeapons();
                    string data = JsonConvert.SerializeObject(new
                    {
                        saved_vehicles = vehicles,
                        normal_peds = normalPeds,
                        mp_characters = mpPeds,
                        weapon_loadouts = weaponLoadouts
                    });
                    SendNuiMessage(data);
                    SetNuiFocus(true, true);
                }
                // save settings
                else if (item == saveSettings)
                {
                    UserDefaults.SaveSettings();
                }
            };
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

        private readonly struct Blip
        {
            public readonly Vector3 Location;
            public readonly int Sprite;
            public readonly string Name;
            public readonly int Color;
            public readonly int blipID;

            public Blip(Vector3 Location, int Sprite, string Name, int Color, int blipID)
            {
                this.Location = Location;
                this.Sprite = Sprite;
                this.Name = Name;
                this.Color = Color;
                this.blipID = blipID;
            }
        }

        private readonly List<Blip> blips = new();

        /// <summary>
        /// Toggles blips on/off.
        /// </summary>
        /// <param name="enable"></param>
        private void ToggleBlips(bool enable)
        {
            if (enable)
            {
                try
                {
                    foreach (LocationBlip bl in vMenuShared.ConfigManager.GetLocationBlipsData())
                    {
                        int blipID = AddBlipForCoord(bl.coordinates.X, bl.coordinates.Y, bl.coordinates.Z);
                        SetBlipSprite(blipID, bl.spriteID);
                        BeginTextCommandSetBlipName("STRING");
                        AddTextComponentSubstringPlayerName(bl.name);
                        EndTextCommandSetBlipName(blipID);
                        SetBlipColour(blipID, bl.color);
                        SetBlipAsShortRange(blipID, true);

                        Blip b = new Blip(bl.coordinates, bl.spriteID, bl.name, bl.color, blipID);
                        blips.Add(b);
                    }
                }
                catch (JsonReaderException ex)
                {
                    Debug.Write($"\n\n[vMenu] An error occurred while loading the locations.json file. Please contact the server owner to resolve this.\nWhen contacting the owner, provide the following error details:\n{ex.Message}.\n\n\n");
                }
            }
            else
            {
                if (blips.Count > 0)
                {
                    foreach (Blip blip in blips)
                    {
                        int id = blip.blipID;
                        if (DoesBlipExist(id))
                        {
                            RemoveBlip(ref id);
                        }
                    }
                }
                blips.Clear();
            }
        }

    }
}
