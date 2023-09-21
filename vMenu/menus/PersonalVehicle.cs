using System.Collections.Generic;
using System.Linq;

using ScaleformUI.Menu;

namespace vMenuClient.menus
{
    public class PersonalVehicle
    {
        // Variables
        private UIMenu menu;
        public bool EnableVehicleBlip { get; private set; } = UserDefaults.PVEnableVehicleBlip;

        // Empty constructor
        public PersonalVehicle() { }

        public Vehicle CurrentPersonalVehicle { get; internal set; } = null;

        public UIMenu VehicleDoorsMenu { get; internal set; } = null;


        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            // Menu
            menu = new UIMenu(GetSafePlayerName(Game.Player.Name), "Personal Vehicle Options");

            // menu items
            var setVehicle = new UIMenuItem("Set Vehicle", "Sets your current vehicle as your personal vehicle. If you already have a personal vehicle set then this will override your selection.");
            setVehicle.SetRightLabel("Current Vehicle: None");
            var toggleEngine = new UIMenuItem("Toggle Engine", "Toggles the engine on or off, even when you're not inside of the vehicle. This does not work if someone else is currently using your vehicle.");
            var toggleLights = new UIMenuListItem("Set Vehicle Lights", new List<dynamic>() { "Force On", "Force Off", "Reset" }, 0, "This will enable or disable your vehicle headlights, the engine of your vehicle needs to be running for this to work.");
            var toggleStance = new UIMenuListItem("Vehicle Stance", new List<dynamic>() { "Default", "Lowered" }, 0, "Select stance for your Personal Vehicle.");
            var kickAllPassengers = new UIMenuItem("Kick Passengers", "This will remove all passengers from your personal vehicle.");
            //MenuItem
            var lockDoors = new UIMenuItem("Lock Vehicle Doors", "This will lock all your vehicle doors for all players. Anyone already inside will always be able to leave the vehicle, even if the doors are locked.");
            var unlockDoors = new UIMenuItem("Unlock Vehicle Doors", "This will unlock all your vehicle doors for all players.");
            var doorsMenuBtn = new UIMenuItem("Vehicle Doors", "Open, close, remove and restore vehicle doors here.");
            doorsMenuBtn.SetRightLabel("→→→");
            var soundHorn = new UIMenuItem("Sound Horn", "Sounds the horn of the vehicle.");
            var toggleAlarm = new UIMenuItem("Toggle Alarm Sound", "Toggles the vehicle alarm sound on or off. This does not set an alarm. It only toggles the current sounding status of the alarm.");
            var enableBlip = new UIMenuCheckboxItem("Add Blip For Personal Vehicle", UIMenuCheckboxStyle.Cross, EnableVehicleBlip, "Enables or disables the blip that gets added when you mark a vehicle as your personal vehicle.");
            var exclusiveDriver = new UIMenuCheckboxItem("Exclusive Driver", UIMenuCheckboxStyle.Cross, false, "If enabled, then you will be the only one that can enter the drivers seat. Other players will not be able to drive the car. They can still be passengers.");
            //submenu
            VehicleDoorsMenu = new UIMenu("Vehicle Doors", "Vehicle Doors Management");
            doorsMenuBtn.Activated += async (a, b) => await a.SwitchTo(VehicleDoorsMenu, 0, true);

            // This is always allowed if this submenu is created/allowed.
            menu.AddItem(setVehicle);

            // Add conditional features.

            // Toggle engine.
            if (IsAllowed(Permission.PVToggleEngine))
            {
                menu.AddItem(toggleEngine);
            }

            // Toggle lights
            if (IsAllowed(Permission.PVToggleLights))
            {
                menu.AddItem(toggleLights);
            }

            // Toggle stance
            if (IsAllowed(Permission.PVToggleStance))
            {
                menu.AddItem(toggleStance);
            }

            // Kick vehicle passengers
            if (IsAllowed(Permission.PVKickPassengers))
            {
                menu.AddItem(kickAllPassengers);
            }

            // Lock and unlock vehicle doors
            if (IsAllowed(Permission.PVLockDoors))
            {
                menu.AddItem(lockDoors);
                menu.AddItem(unlockDoors);
            }

            if (IsAllowed(Permission.PVDoors))
            {
                menu.AddItem(doorsMenuBtn);
            }

            // Sound horn
            if (IsAllowed(Permission.PVSoundHorn))
            {
                menu.AddItem(soundHorn);
            }

            // Toggle alarm sound
            if (IsAllowed(Permission.PVToggleAlarm))
            {
                menu.AddItem(toggleAlarm);
            }

            // Enable blip for personal vehicle
            if (IsAllowed(Permission.PVAddBlip))
            {
                menu.AddItem(enableBlip);
            }

            if (IsAllowed(Permission.PVExclusiveDriver))
            {
                menu.AddItem(exclusiveDriver);
            }


            // Handle list presses
            menu.OnListSelect += (sender, item, index) =>
            {
                var itemIndex = sender.MenuItems.IndexOf(item);
                var veh = CurrentPersonalVehicle;
                if (veh != null && veh.Exists())
                {
                    if (!NetworkHasControlOfEntity(CurrentPersonalVehicle.Handle))
                    {
                        if (!NetworkRequestControlOfEntity(CurrentPersonalVehicle.Handle))
                        {
                            Notify.Error("You currently can't control this vehicle. Is someone else currently driving your car? Please try again after making sure other players are not controlling your vehicle.");
                            return;
                        }
                    }

                    if (item == toggleLights)
                    {
                        PressKeyFob(CurrentPersonalVehicle);
                        if (itemIndex == 0)
                        {
                            SetVehicleLights(CurrentPersonalVehicle.Handle, 3);
                        }
                        else if (itemIndex == 1)
                        {
                            SetVehicleLights(CurrentPersonalVehicle.Handle, 1);
                        }
                        else
                        {
                            SetVehicleLights(CurrentPersonalVehicle.Handle, 0);
                        }
                    }
                    else if (item == toggleStance)
                    {
                        PressKeyFob(CurrentPersonalVehicle);
                        if (itemIndex == 0)
                        {
                            SetReduceDriftVehicleSuspension(CurrentPersonalVehicle.Handle, false);
                        }
                        else if (itemIndex == 1)
                        {
                            SetReduceDriftVehicleSuspension(CurrentPersonalVehicle.Handle, true);
                        }
                    }

                }
                else
                {
                    Notify.Error("You have not yet selected a personal vehicle, or your vehicle has been deleted. Set a personal vehicle before you can use these options.");
                }
            };

            // Handle checkbox changes
            menu.OnCheckboxChange += (sender, item, _checked) =>
            {
                if (item == enableBlip)
                {
                    EnableVehicleBlip = _checked;
                    if (EnableVehicleBlip)
                    {
                        if (CurrentPersonalVehicle != null && CurrentPersonalVehicle.Exists())
                        {
                            if (CurrentPersonalVehicle.AttachedBlip == null || !CurrentPersonalVehicle.AttachedBlip.Exists())
                            {
                                CurrentPersonalVehicle.AttachBlip();
                            }
                            CurrentPersonalVehicle.AttachedBlip.Sprite = BlipSprite.PersonalVehicleCar;
                            CurrentPersonalVehicle.AttachedBlip.Name = "Personal Vehicle";
                        }
                        else
                        {
                            Notify.Error("You have not yet selected a personal vehicle, or your vehicle has been deleted. Set a personal vehicle before you can use these options.");
                        }

                    }
                    else
                    {
                        if (CurrentPersonalVehicle != null && CurrentPersonalVehicle.Exists() && CurrentPersonalVehicle.AttachedBlip != null && CurrentPersonalVehicle.AttachedBlip.Exists())
                        {
                            CurrentPersonalVehicle.AttachedBlip.Delete();
                        }
                    }
                }
                else if (item == exclusiveDriver)
                {
                    if (CurrentPersonalVehicle != null && CurrentPersonalVehicle.Exists())
                    {
                        if (NetworkRequestControlOfEntity(CurrentPersonalVehicle.Handle))
                        {
                            if (_checked)
                            {
                                // SetVehicleExclusiveDriver, but the current version is broken in C# so we manually execute it.
                                CitizenFX.Core.Native.Function.Call((CitizenFX.Core.Native.Hash)0x41062318F23ED854, CurrentPersonalVehicle, true);
                                SetVehicleExclusiveDriver_2(CurrentPersonalVehicle.Handle, Game.PlayerPed.Handle, 1);
                            }
                            else
                            {
                                // SetVehicleExclusiveDriver, but the current version is broken in C# so we manually execute it.
                                CitizenFX.Core.Native.Function.Call((CitizenFX.Core.Native.Hash)0x41062318F23ED854, CurrentPersonalVehicle, false);
                                SetVehicleExclusiveDriver_2(CurrentPersonalVehicle.Handle, 0, 1);
                            }
                        }
                        else
                        {
                            item.Checked = !_checked;
                            Notify.Error("You currently can't control this vehicle. Is someone else currently driving your car? Please try again after making sure other players are not controlling your vehicle.");
                        }
                    }
                }
            };

            // Handle button presses.
            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == setVehicle)
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        var veh = GetVehicle();
                        if (veh != null && veh.Exists())
                        {
                            if (Game.PlayerPed == veh.Driver)
                            {
                                CurrentPersonalVehicle = veh;
                                veh.PreviouslyOwnedByPlayer = true;
                                veh.IsPersistent = true;
                                if (EnableVehicleBlip && IsAllowed(Permission.PVAddBlip))
                                {
                                    if (veh.AttachedBlip == null || !veh.AttachedBlip.Exists())
                                    {
                                        veh.AttachBlip();
                                    }
                                    veh.AttachedBlip.Sprite = BlipSprite.PersonalVehicleCar;
                                    veh.AttachedBlip.Name = "Personal Vehicle";
                                }
                                var name = GetLabelText(veh.DisplayName);
                                if (string.IsNullOrEmpty(name) || name.ToLower() == "null")
                                {
                                    name = veh.DisplayName;
                                }
                                item.SetRightLabel($"Current Vehicle: {name}");
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
                    else
                    {
                        Notify.Error(CommonErrors.NoVehicle);
                    }
                }
                else if (CurrentPersonalVehicle != null && CurrentPersonalVehicle.Exists())
                {
                    if (item == kickAllPassengers)
                    {
                        if (CurrentPersonalVehicle.Occupants.Count() > 0 && CurrentPersonalVehicle.Occupants.Any(p => p != Game.PlayerPed))
                        {
                            var netId = VehToNet(CurrentPersonalVehicle.Handle);
                            TriggerServerEvent("vMenu:GetOutOfCar", netId, Game.Player.ServerId);
                        }
                        else
                        {
                            Notify.Info("There are no other players in your vehicle that need to be kicked out.");
                        }
                    }
                    else
                    {
                        if (!NetworkHasControlOfEntity(CurrentPersonalVehicle.Handle))
                        {
                            if (!NetworkRequestControlOfEntity(CurrentPersonalVehicle.Handle))
                            {
                                Notify.Error("You currently can't control this vehicle. Is someone else currently driving your car? Please try again after making sure other players are not controlling your vehicle.");
                                return;
                            }
                        }

                        if (item == toggleEngine)
                        {
                            PressKeyFob(CurrentPersonalVehicle);
                            SetVehicleEngineOn(CurrentPersonalVehicle.Handle, !CurrentPersonalVehicle.IsEngineRunning, true, true);
                        }

                        else if (item == lockDoors || item == unlockDoors)
                        {
                            PressKeyFob(CurrentPersonalVehicle);
                            var _lock = item == lockDoors;
                            LockOrUnlockDoors(CurrentPersonalVehicle, _lock);
                        }

                        else if (item == soundHorn)
                        {
                            PressKeyFob(CurrentPersonalVehicle);
                            SoundHorn(CurrentPersonalVehicle);
                        }

                        else if (item == toggleAlarm)
                        {
                            PressKeyFob(CurrentPersonalVehicle);
                            ToggleVehicleAlarm(CurrentPersonalVehicle);
                        }
                    }
                }
                else
                {
                    Notify.Error("You have not yet selected a personal vehicle, or your vehicle has been deleted. Set a personal vehicle before you can use these options.");
                }
            };

            #region Doors submenu 
            var openAll = new UIMenuItem("Open All Doors", "Open all vehicle doors.");
            var closeAll = new UIMenuItem("Close All Doors", "Close all vehicle doors.");
            var LF = new UIMenuItem("Left Front Door", "Open/close the left front door.");
            var RF = new UIMenuItem("Right Front Door", "Open/close the right front door.");
            var LR = new UIMenuItem("Left Rear Door", "Open/close the left rear door.");
            var RR = new UIMenuItem("Right Rear Door", "Open/close the right rear door.");
            var HD = new UIMenuItem("Hood", "Open/close the hood.");
            var TR = new UIMenuItem("Trunk", "Open/close the trunk.");
            var E1 = new UIMenuItem("Extra 1", "Open/close the extra door (#1). Note this door is not present on most vehicles.");
            var E2 = new UIMenuItem("Extra 2", "Open/close the extra door (#2). Note this door is not present on most vehicles.");
            var BB = new UIMenuItem("Bomb Bay", "Open/close the bomb bay. Only available on some planes.");
            var doors = new List<dynamic>() { "Front Left", "Front Right", "Rear Left", "Rear Right", "Hood", "Trunk", "Extra 1", "Extra 2", "Bomb Bay" };
            var removeDoorList = new UIMenuListItem("Remove Door", doors, 0, "Remove a specific vehicle door completely.");
            var deleteDoors = new UIMenuCheckboxItem("Delete Removed Doors", false, "When enabled, doors that you remove using the list above will be deleted from the world. If disabled, then the doors will just fall on the ground.");

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
                var itemIndex = sender.MenuItems.IndexOf(item);
                var veh = CurrentPersonalVehicle;
                if (veh != null && veh.Exists())
                {
                    if (!NetworkHasControlOfEntity(CurrentPersonalVehicle.Handle))
                    {
                        if (!NetworkRequestControlOfEntity(CurrentPersonalVehicle.Handle))
                        {
                            Notify.Error("You currently can't control this vehicle. Is someone else currently driving your car? Please try again after making sure other players are not controlling your vehicle.");
                            return;
                        }
                    }

                    if (item == removeDoorList)
                    {
                        PressKeyFob(veh);
                        SetVehicleDoorBroken(veh.Handle, index, deleteDoors.Checked);
                    }
                }
            };

            VehicleDoorsMenu.OnItemSelect += (sender, item, index) =>
            {
                var veh = CurrentPersonalVehicle;
                if (veh != null && veh.Exists() && !veh.IsDead)
                {
                    if (!NetworkHasControlOfEntity(CurrentPersonalVehicle.Handle))
                    {
                        if (!NetworkRequestControlOfEntity(CurrentPersonalVehicle.Handle))
                        {
                            Notify.Error("You currently can't control this vehicle. Is someone else currently driving your car? Please try again after making sure other players are not controlling your vehicle.");
                            return;
                        }
                    }

                    if (index < 8)
                    {
                        var open = GetVehicleDoorAngleRatio(veh.Handle, index) > 0.1f;
                        PressKeyFob(veh);
                        if (open)
                        {
                            SetVehicleDoorShut(veh.Handle, index, false);
                        }
                        else
                        {
                            SetVehicleDoorOpen(veh.Handle, index, false, false);
                        }
                    }
                    else if (item == openAll)
                    {
                        PressKeyFob(veh);
                        for (var door = 0; door < 8; door++)
                        {
                            SetVehicleDoorOpen(veh.Handle, door, false, false);
                        }
                    }
                    else if (item == closeAll)
                    {
                        PressKeyFob(veh);
                        for (var door = 0; door < 8; door++)
                        {
                            SetVehicleDoorShut(veh.Handle, door, false);
                        }
                    }
                    else if (item == BB && veh.HasBombBay)
                    {
                        PressKeyFob(veh);
                        var bombBayOpen = AreBombBayDoorsOpen(veh.Handle);
                        if (bombBayOpen)
                        {
                            veh.CloseBombBay();
                        }
                        else
                        {
                            veh.OpenBombBay();
                        }
                    }
                    else
                    {
                        Notify.Error("You have not yet selected a personal vehicle, or your vehicle has been deleted. Set a personal vehicle before you can use these options.");
                    }
                }
            };
            #endregion
        }



        private async void SoundHorn(Vehicle veh)
        {
            if (veh != null && veh.Exists())
            {
                var timer = GetGameTimer();
                while (GetGameTimer() - timer < 1000)
                {
                    SoundVehicleHornThisFrame(veh.Handle);
                    await Delay(0);
                }
            }
        }

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
