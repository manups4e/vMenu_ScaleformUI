using System.Collections.Generic;
using System.Linq;

using ScaleformUI.Menu;

using vMenuClient.data;

namespace vMenuClient.menus
{
    public class WeaponOptions
    {
        // Variables
        private UIMenu menu;

        public bool UnlimitedAmmo { get; private set; } = UserDefaults.WeaponsUnlimitedAmmo;
        public bool NoReload { get; private set; } = UserDefaults.WeaponsNoReload;
        public bool AutoEquipChute { get; private set; } = UserDefaults.AutoEquipChute;
        public bool UnlimitedParachutes { get; private set; } = UserDefaults.WeaponsUnlimitedParachutes;

        public static Dictionary<string, uint> AddonWeapons = new();

        private Dictionary<UIMenu, ValidWeapon> weaponInfo;
        private Dictionary<UIMenuItem, string> weaponComponents;

        #region Create Menu
        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            // Setup weapon dictionaries.
            weaponInfo = new Dictionary<UIMenu, ValidWeapon>();
            weaponComponents = new Dictionary<UIMenuItem, string>();

            #region create main weapon options menu and add items
            // Create the menu.
            menu = new UIMenu(Game.Player.Name, "Weapon Options");

            var getAllWeapons = new UIMenuItem("Get All Weapons", "Get all weapons.");
            var removeAllWeapons = new UIMenuItem("Remove All Weapons", "Removes all weapons in your inventory.");
            var unlimitedAmmo = new UIMenuCheckboxItem("Unlimited Ammo", UnlimitedAmmo, "Unlimited ammunition supply.");
            var noReload = new UIMenuCheckboxItem("No Reload", NoReload, "Never reload.");
            var setAmmo = new UIMenuItem("Set All Ammo Count", "Set the amount of ammo in all your weapons.");
            var refillMaxAmmo = new UIMenuItem("Refill All Ammo", "Give all your weapons max ammo.");
            var spawnByName = new UIMenuItem("Spawn Weapon By Name", "Enter a weapon mode name to spawn.");

            // Add items based on permissions
            if (IsAllowed(Permission.WPGetAll))
            {
                menu.AddItem(getAllWeapons);
            }
            if (IsAllowed(Permission.WPRemoveAll))
            {
                menu.AddItem(removeAllWeapons);
            }
            if (IsAllowed(Permission.WPUnlimitedAmmo))
            {
                menu.AddItem(unlimitedAmmo);
            }
            if (IsAllowed(Permission.WPNoReload))
            {
                menu.AddItem(noReload);
            }
            if (IsAllowed(Permission.WPSetAllAmmo))
            {
                menu.AddItem(setAmmo);
                menu.AddItem(refillMaxAmmo);
            }
            if (IsAllowed(Permission.WPSpawnByName))
            {
                menu.AddItem(spawnByName);
            }
            #endregion

            #region addonweapons submenu
            var addonWeaponsBtn = new UIMenuItem("Addon Weapons", "Equip / remove addon weapons available on this server.");
            var addonWeaponsMenu = new UIMenu("Addon Weapons", "Equip/Remove Addon Weapons");
            menu.AddItem(addonWeaponsBtn);

            #region manage creating and accessing addon weapons menu
            if (IsAllowed(Permission.WPSpawn) && AddonWeapons != null && AddonWeapons.Count > 0)
            {
                foreach (var weapon in AddonWeapons)
                {
                    var name = weapon.Key.ToString();
                    var model = weapon.Value;
                    var item = new UIMenuItem(name, $"Click to add/remove this weapon ({name}) to/from your inventory.");
                    addonWeaponsMenu.AddItem(item);
                    if (!IsWeaponValid(model))
                    {
                        item.Enabled = false;
                        item.SetLeftBadge(BadgeIcon.LOCK);
                        item.Description = "This model is not available. Please ask the server owner to verify it's being streamed correctly.";
                    }
                }
                addonWeaponsMenu.OnItemSelect += (sender, item, index) =>
                {
                    var weapon = AddonWeapons.ElementAt(index);
                    if (HasPedGotWeapon(Game.PlayerPed.Handle, weapon.Value, false))
                    {
                        RemoveWeaponFromPed(Game.PlayerPed.Handle, weapon.Value);
                    }
                    else
                    {
                        var maxAmmo = 200;
                        GetMaxAmmo(Game.PlayerPed.Handle, weapon.Value, ref maxAmmo);
                        GiveWeaponToPed(Game.PlayerPed.Handle, weapon.Value, maxAmmo, false, true);
                    }
                };
                addonWeaponsBtn.SetRightLabel("→→→");
            }
            else
            {
                addonWeaponsBtn.SetLeftBadge(BadgeIcon.LOCK);
                addonWeaponsBtn.Enabled = false;
                addonWeaponsBtn.Description = "This option is not available on this server because you don't have permission to use it, or it is not setup correctly.";
            }
            #endregion
            #endregion

            #region parachute options menu

            if (IsAllowed(Permission.WPParachute))
            {
                // main parachute options menu setup
                var parachuteMenu = new UIMenu("Parachute Options", "Parachute Options");
                var parachuteBtn = new UIMenuItem("Parachute Options", "All parachute related options can be changed here.");
                parachuteBtn.SetRightLabel("→→→");

                menu.AddItem(parachuteBtn);

                var chutes = new List<dynamic>()
                {
                    GetLabelText("PM_TINT0"),
                    GetLabelText("PM_TINT1"),
                    GetLabelText("PM_TINT2"),
                    GetLabelText("PM_TINT3"),
                    GetLabelText("PM_TINT4"),
                    GetLabelText("PM_TINT5"),
                    GetLabelText("PM_TINT6"),
                    GetLabelText("PM_TINT7"),

                    // broken in FiveM for some weird reason:
                    GetLabelText("PS_CAN_0"),
                    GetLabelText("PS_CAN_1"),
                    GetLabelText("PS_CAN_2"),
                    GetLabelText("PS_CAN_3"),
                    GetLabelText("PS_CAN_4"),
                    GetLabelText("PS_CAN_5")
                };
                var chuteDescriptions = new List<dynamic>()
                {
                    GetLabelText("PD_TINT0"),
                    GetLabelText("PD_TINT1"),
                    GetLabelText("PD_TINT2"),
                    GetLabelText("PD_TINT3"),
                    GetLabelText("PD_TINT4"),
                    GetLabelText("PD_TINT5"),
                    GetLabelText("PD_TINT6"),
                    GetLabelText("PD_TINT7"),

                    // broken in FiveM for some weird reason:
                    GetLabelText("PSD_CAN_0") + " ~r~For some reason this one doesn't seem to work in FiveM.",
                    GetLabelText("PSD_CAN_1") + " ~r~For some reason this one doesn't seem to work in FiveM.",
                    GetLabelText("PSD_CAN_2") + " ~r~For some reason this one doesn't seem to work in FiveM.",
                    GetLabelText("PSD_CAN_3") + " ~r~For some reason this one doesn't seem to work in FiveM.",
                    GetLabelText("PSD_CAN_4") + " ~r~For some reason this one doesn't seem to work in FiveM.",
                    GetLabelText("PSD_CAN_5") + " ~r~For some reason this one doesn't seem to work in FiveM."
                };

                var togglePrimary = new UIMenuItem("Toggle Primary Parachute", "Equip or remove the primary parachute");
                var toggleReserve = new UIMenuItem("Enable Reserve Parachute", "Enables the reserve parachute. Only works if you enabled the primary parachute first. Reserve parachute can not be removed from the player once it's activated.");
                var primaryChutes = new UIMenuListItem("Primary Chute Style", chutes, 0, $"Primary chute: {chuteDescriptions[0]}");
                var secondaryChutes = new UIMenuListItem("Reserve Chute Style", chutes, 0, $"Reserve chute: {chuteDescriptions[0]}");
                var unlimitedParachutes = new UIMenuCheckboxItem("Unlimited Parachutes", UnlimitedParachutes, "Enable unlimited parachutes and reserve parachutes.");
                var autoEquipParachutes = new UIMenuCheckboxItem("Auto Equip Parachutes", AutoEquipChute, "Automatically equip a parachute and reserve parachute when entering planes/helicopters.");

                // smoke color list
                var smokeColorsList = new List<dynamic>()
                {
                    GetLabelText("PM_TINT8"), // no smoke
                    GetLabelText("PM_TINT9"), // red
                    GetLabelText("PM_TINT10"), // orange
                    GetLabelText("PM_TINT11"), // yellow
                    GetLabelText("PM_TINT12"), // blue
                    GetLabelText("PM_TINT13"), // black
                };
                var colors = new List<int[]>()
                {
                    new int[3] { 255, 255, 255 },
                    new int[3] { 255, 0, 0 },
                    new int[3] { 255, 165, 0 },
                    new int[3] { 255, 255, 0 },
                    new int[3] { 0, 0, 255 },
                    new int[3] { 20, 20, 20 },
                };

                var smokeColors = new UIMenuListItem("Smoke Trail Color", smokeColorsList, 0, "Choose a smoke trail color, then press select to change it. Changing colors takes 4 seconds, you can not use your smoke while the color is being changed.");

                parachuteMenu.AddItem(togglePrimary);
                parachuteMenu.AddItem(toggleReserve);
                parachuteMenu.AddItem(autoEquipParachutes);
                parachuteMenu.AddItem(unlimitedParachutes);
                parachuteMenu.AddItem(smokeColors);
                parachuteMenu.AddItem(primaryChutes);
                parachuteMenu.AddItem(secondaryChutes);

                parachuteMenu.OnItemSelect += (sender, item, index) =>
                {
                    if (item == togglePrimary)
                    {
                        if (HasPedGotWeapon(Game.PlayerPed.Handle, (uint)GetHashKey("gadget_parachute"), false))
                        {
                            Subtitle.Custom("Primary parachute removed.");
                            RemoveWeaponFromPed(Game.PlayerPed.Handle, (uint)GetHashKey("gadget_parachute"));
                        }
                        else
                        {
                            Subtitle.Custom("Primary parachute added.");
                            GiveWeaponToPed(Game.PlayerPed.Handle, (uint)GetHashKey("gadget_parachute"), 0, false, false);
                        }
                    }
                    else if (item == toggleReserve)
                    {
                        SetPlayerHasReserveParachute(Game.Player.Handle);
                        Subtitle.Custom("Reserve parachute has been added.");

                    }
                };

                parachuteMenu.OnCheckboxChange += (sender, item, _checked) =>
                {
                    if (item == unlimitedParachutes)
                    {
                        UnlimitedParachutes = _checked;
                    }
                    else if (item == autoEquipParachutes)
                    {
                        AutoEquipChute = _checked;
                    }
                };

                var switching = false;
                async void IndexChangedEventHandler(UIMenu sender, UIMenuListItem item, int newIndex)
                {
                    if (item == smokeColors)
                    {
                        if (!switching)
                        {
                            switching = true;
                            SetPlayerCanLeaveParachuteSmokeTrail(Game.Player.Handle, false);
                            await Delay(4000);
                            var color = colors[newIndex];
                            SetPlayerParachuteSmokeTrailColor(Game.Player.Handle, color[0], color[1], color[2]);
                            SetPlayerCanLeaveParachuteSmokeTrail(Game.Player.Handle, newIndex != 0);
                            switching = false;
                        }
                    }
                    else if (item == primaryChutes)
                    {
                        item.Description = $"Primary chute: {chuteDescriptions[newIndex]}";
                        SetPlayerParachuteTintIndex(Game.Player.Handle, newIndex);
                    }
                    else if (item == secondaryChutes)
                    {
                        item.Description = $"Reserve chute: {chuteDescriptions[newIndex]}";
                        SetPlayerReserveParachuteTintIndex(Game.Player.Handle, newIndex);
                    }
                }

                parachuteMenu.OnListSelect += (sender, item, index) => IndexChangedEventHandler(sender, item, item.Index);
                parachuteMenu.OnListChange += IndexChangedEventHandler;
            }
            #endregion

            #region Create Weapon Category Submenus
            var spacer = new UIMenuSeparatorItem("↓ Weapon Categories ↓", true);
            menu.AddItem(spacer);

            var handGuns = new UIMenu("Weapons", "Handguns");
            var handGunsBtn = new UIMenuItem("Handguns");

            var rifles = new UIMenu("Weapons", "Assault Rifles");
            var riflesBtn = new UIMenuItem("Assault Rifles");

            var shotguns = new UIMenu("Weapons", "Shotguns");
            var shotgunsBtn = new UIMenuItem("Shotguns");

            var smgs = new UIMenu("Weapons", "Sub-/Light Machine Guns");
            var smgsBtn = new UIMenuItem("Sub-/Light Machine Guns");

            var throwables = new UIMenu("Weapons", "Throwables");
            var throwablesBtn = new UIMenuItem("Throwables");

            var melee = new UIMenu("Weapons", "Melee");
            var meleeBtn = new UIMenuItem("Melee");

            var heavy = new UIMenu("Weapons", "Heavy Weapons");
            var heavyBtn = new UIMenuItem("Heavy Weapons");

            var snipers = new UIMenu("Weapons", "Sniper Rifles");
            var snipersBtn = new UIMenuItem("Sniper Rifles");
            #endregion

            #region Setup weapon category buttons and submenus.
            handGunsBtn.SetRightLabel("→→→");
            menu.AddItem(handGunsBtn);

            riflesBtn.SetRightLabel("→→→");
            menu.AddItem(riflesBtn);

            shotgunsBtn.SetRightLabel("→→→");
            menu.AddItem(shotgunsBtn);

            smgsBtn.SetRightLabel("→→→");
            menu.AddItem(smgsBtn);

            throwablesBtn.SetRightLabel("→→→");
            menu.AddItem(throwablesBtn);

            meleeBtn.SetRightLabel("→→→");
            menu.AddItem(meleeBtn);

            heavyBtn.SetRightLabel("→→→");
            menu.AddItem(heavyBtn);

            snipersBtn.SetRightLabel("→→→");
            menu.AddItem(snipersBtn);
            #endregion

            #region Loop through all weapons, create menus for them and add all menu items and handle events.
            foreach (var weapon in ValidWeapons.WeaponList)
            {
                var cat = (uint)GetWeapontypeGroup(weapon.Hash);
                if (!string.IsNullOrEmpty(weapon.Name) && IsAllowed(weapon.Perm))
                {
                    //Log($"[DEBUG LOG] [WEAPON-BUG] {weapon.Name} - {weapon.Perm} = {IsAllowed(weapon.Perm)} & All = {IsAllowed(Permission.WPGetAll)}");
                    #region Create menu for this weapon and add buttons
                    var weaponMenu = new UIMenu("Weapon Options", weapon.Name);
                    var stats = new Game.WeaponHudStats();
                    Game.GetWeaponHudStats(weapon.Hash, ref stats);
                    var weaponItem = new UIMenuItem(weapon.Name, $"Open the options for ~y~{weapon.Name}~s~.")
                    {
                        ItemData = stats
                    };
                    var pan = new UIMenuStatisticsPanel();
                    pan.AddStatistics("Damage", stats.hudDamage / 100f);
                    pan.AddStatistics("Speed", stats.hudSpeed / 100f);
                    pan.AddStatistics("Accuracy", stats.hudAccuracy / 100f);
                    pan.AddStatistics("Range", stats.hudRange / 100f);
                    weaponItem.SetRightLabel("→→→");
                    weaponItem.SetLeftBadge(BadgeIcon.GUN);
                    weaponItem.AddPanel(pan);

                    weaponInfo.Add(weaponMenu, weapon);

                    var getOrRemoveWeapon = new UIMenuItem("Equip/Remove Weapon", "Add or remove this weapon to/form your inventory.");
                    getOrRemoveWeapon.SetLeftBadge(BadgeIcon.GUN);
                    weaponMenu.AddItem(getOrRemoveWeapon);
                    if (!IsAllowed(Permission.WPSpawn))
                    {
                        getOrRemoveWeapon.Enabled = false;
                        getOrRemoveWeapon.Description = "You do not have permission to use this option.";
                        getOrRemoveWeapon.SetLeftBadge(BadgeIcon.LOCK);
                    }

                    var fillAmmo = new UIMenuItem("Re-fill Ammo", "Get max ammo for this weapon.");
                    fillAmmo.SetLeftBadge(BadgeIcon.AMMO);
                    weaponMenu.AddItem(fillAmmo);

                    var tints = new List<dynamic>();
                    if (weapon.Name.Contains(" Mk II"))
                    {
                        foreach (var tint in ValidWeapons.WeaponTintsMkII)
                        {
                            tints.Add(tint.Key);
                        }
                    }
                    else
                    {
                        foreach (var tint in ValidWeapons.WeaponTints)
                        {
                            tints.Add(tint.Key);
                        }
                    }

                    var weaponTints = new UIMenuListItem("Tints", tints, 0, "Select a tint for your weapon.");
                    weaponMenu.AddItem(weaponTints);
                    #endregion

                    #region Handle weapon specific list changes
                    weaponMenu.OnListChange += (sender, item, newIndex) =>
                    {
                        if (item == weaponTints)
                        {
                            if (HasPedGotWeapon(Game.PlayerPed.Handle, weaponInfo[sender].Hash, false))
                            {
                                SetPedWeaponTintIndex(Game.PlayerPed.Handle, weaponInfo[sender].Hash, newIndex);
                            }
                            else
                            {
                                Notify.Error("You need to get the weapon first!");
                            }
                        }
                    };
                    #endregion

                    #region Handle weapon specific button presses
                    weaponMenu.OnItemSelect += (sender, item, index) =>
                    {
                        var info = weaponInfo[sender];
                        var hash = info.Hash;

                        SetCurrentPedWeapon(Game.PlayerPed.Handle, hash, true);

                        if (item == getOrRemoveWeapon)
                        {
                            if (HasPedGotWeapon(Game.PlayerPed.Handle, hash, false))
                            {
                                RemoveWeaponFromPed(Game.PlayerPed.Handle, hash);
                                Subtitle.Custom("Weapon removed.");
                            }
                            else
                            {
                                var ammo = 255;
                                GetMaxAmmo(Game.PlayerPed.Handle, hash, ref ammo);
                                GiveWeaponToPed(Game.PlayerPed.Handle, hash, ammo, false, true);
                                Subtitle.Custom("Weapon added.");
                            }
                        }
                        else if (item == fillAmmo)
                        {
                            if (HasPedGotWeapon(Game.PlayerPed.Handle, hash, false))
                            {
                                var ammo = 900;
                                GetMaxAmmo(Game.PlayerPed.Handle, hash, ref ammo);
                                SetPedAmmo(Game.PlayerPed.Handle, hash, ammo);
                            }
                            else
                            {
                                Notify.Error("You need to get the weapon first before re-filling ammo!");
                            }
                        }
                    };
                    #endregion

                    #region load components
                    if (weapon.Components != null)
                    {
                        if (weapon.Components.Count > 0)
                        {
                            foreach (var comp in weapon.Components)
                            {
                                //Log($"{weapon.Name} : {comp.Key}");
                                var compItem = new UIMenuItem(comp.Key, "Click to equip or remove this component.");
                                weaponComponents.Add(compItem, comp.Key);
                                weaponMenu.AddItem(compItem);

                                #region Handle component button presses
                                weaponMenu.OnItemSelect += (sender, item, index) =>
                                {
                                    if (item == compItem)
                                    {
                                        var Weapon = weaponInfo[sender];
                                        var componentHash = Weapon.Components[weaponComponents[item]];
                                        if (HasPedGotWeapon(Game.PlayerPed.Handle, Weapon.Hash, false))
                                        {
                                            SetCurrentPedWeapon(Game.PlayerPed.Handle, Weapon.Hash, true);
                                            if (HasPedGotWeaponComponent(Game.PlayerPed.Handle, Weapon.Hash, componentHash))
                                            {
                                                RemoveWeaponComponentFromPed(Game.PlayerPed.Handle, Weapon.Hash, componentHash);

                                                Subtitle.Custom("Component removed.");
                                            }
                                            else
                                            {
                                                var ammo = GetAmmoInPedWeapon(Game.PlayerPed.Handle, Weapon.Hash);

                                                var clipAmmo = GetMaxAmmoInClip(Game.PlayerPed.Handle, Weapon.Hash, false);
                                                GetAmmoInClip(Game.PlayerPed.Handle, Weapon.Hash, ref clipAmmo);

                                                GiveWeaponComponentToPed(Game.PlayerPed.Handle, Weapon.Hash, componentHash);

                                                SetAmmoInClip(Game.PlayerPed.Handle, Weapon.Hash, clipAmmo);

                                                SetPedAmmo(Game.PlayerPed.Handle, Weapon.Hash, ammo);
                                                Subtitle.Custom("Component equiped.");
                                            }
                                        }
                                        else
                                        {
                                            Notify.Error("You need to get the weapon first before you can modify it.");
                                        }
                                    }
                                };
                                #endregion
                            }
                        }
                    }
                    #endregion

                    // refresh and add to menu.

                    if (cat == 970310034) // 970310034 rifles
                    {
                        rifles.AddItem(weaponItem);
                    }
                    else if (cat is 416676503 or 690389602) // 416676503 hand guns // 690389602 stun gun
                    {
                        handGuns.AddItem(weaponItem);
                    }
                    else if (cat == 860033945) // 860033945 shotguns
                    {
                        shotguns.AddItem(weaponItem);
                    }
                    else if (cat is 3337201093 or 1159398588) // 3337201093 sub machine guns // 1159398588 light machine guns
                    {
                        smgs.AddItem(weaponItem);
                    }
                    else if (cat is 1548507267 or 4257178988 or 1595662460) // 1548507267 throwables // 4257178988 fire extinghuiser // jerry can
                    {
                        throwables.AddItem(weaponItem);
                    }
                    else if (cat is 3566412244 or 2685387236) // 3566412244 melee weapons // 2685387236 knuckle duster
                    {
                        melee.AddItem(weaponItem);
                    }
                    else if (cat == 2725924767) // 2725924767 heavy weapons
                    {
                        heavy.AddItem(weaponItem);
                    }
                    else if (cat == 3082541095) // 3082541095 sniper rifles
                    {
                        snipers.AddItem(weaponItem);
                    }
                }
            }
            #endregion

            #region Disable submenus if no weapons in that category are allowed.
            if (handGuns.Size == 0)
            {
                handGunsBtn.SetLeftBadge(BadgeIcon.LOCK);
                handGunsBtn.Description = "The server owner removed the permissions for all weapons in this category.";
                handGunsBtn.Enabled = false;
            }
            if (rifles.Size == 0)
            {
                riflesBtn.SetLeftBadge(BadgeIcon.LOCK);
                riflesBtn.Description = "The server owner removed the permissions for all weapons in this category.";
                riflesBtn.Enabled = false;
            }
            if (shotguns.Size == 0)
            {
                shotgunsBtn.SetLeftBadge(BadgeIcon.LOCK);
                shotgunsBtn.Description = "The server owner removed the permissions for all weapons in this category.";
                shotgunsBtn.Enabled = false;
            }
            if (smgs.Size == 0)
            {
                smgsBtn.SetLeftBadge(BadgeIcon.LOCK);
                smgsBtn.Description = "The server owner removed the permissions for all weapons in this category.";
                smgsBtn.Enabled = false;
            }
            if (throwables.Size == 0)
            {
                throwablesBtn.SetLeftBadge(BadgeIcon.LOCK);
                throwablesBtn.Description = "The server owner removed the permissions for all weapons in this category.";
                throwablesBtn.Enabled = false;
            }
            if (melee.Size == 0)
            {
                meleeBtn.SetLeftBadge(BadgeIcon.LOCK);
                meleeBtn.Description = "The server owner removed the permissions for all weapons in this category.";
                meleeBtn.Enabled = false;
            }
            if (heavy.Size == 0)
            {
                heavyBtn.SetLeftBadge(BadgeIcon.LOCK);
                heavyBtn.Description = "The server owner removed the permissions for all weapons in this category.";
                heavyBtn.Enabled = false;
            }
            if (snipers.Size == 0)
            {
                snipersBtn.SetLeftBadge(BadgeIcon.LOCK);
                snipersBtn.Description = "The server owner removed the permissions for all weapons in this category.";
                snipersBtn.Enabled = false;
            }
            #endregion

            #region Handle button presses
            menu.OnItemSelect += (sender, item, index) =>
            {
                var ped = new Ped(Game.PlayerPed.Handle);
                if (item == getAllWeapons)
                {
                    foreach (var vw in ValidWeapons.WeaponList)
                    {
                        if (IsAllowed(vw.Perm))
                        {
                            GiveWeaponToPed(Game.PlayerPed.Handle, vw.Hash, vw.GetMaxAmmo, false, true);

                            var ammoInClip = GetMaxAmmoInClip(Game.PlayerPed.Handle, vw.Hash, false);
                            SetAmmoInClip(Game.PlayerPed.Handle, vw.Hash, ammoInClip);
                            var ammo = 0;
                            GetMaxAmmo(Game.PlayerPed.Handle, vw.Hash, ref ammo);
                            SetPedAmmo(Game.PlayerPed.Handle, vw.Hash, ammo);
                        }
                    }

                    SetCurrentPedWeapon(Game.PlayerPed.Handle, (uint)GetHashKey("weapon_unarmed"), true);
                }
                else if (item == removeAllWeapons)
                {
                    ped.Weapons.RemoveAll();
                }
                else if (item == setAmmo)
                {
                    SetAllWeaponsAmmo();
                }
                else if (item == refillMaxAmmo)
                {
                    foreach (var vw in ValidWeapons.WeaponList)
                    {
                        if (HasPedGotWeapon(Game.PlayerPed.Handle, vw.Hash, false))
                        {
                            var ammoInClip = GetMaxAmmoInClip(Game.PlayerPed.Handle, vw.Hash, false);
                            SetAmmoInClip(Game.PlayerPed.Handle, vw.Hash, ammoInClip);
                            var ammo = 0;
                            GetMaxAmmo(Game.PlayerPed.Handle, vw.Hash, ref ammo);
                            SetPedAmmo(Game.PlayerPed.Handle, vw.Hash, ammo);
                        }
                    }
                }
                else if (item == spawnByName)
                {
                    SpawnCustomWeapon();
                }
            };
            #endregion

            #region Handle checkbox changes
            menu.OnCheckboxChange += (sender, item, _checked) =>
            {
                if (item == noReload)
                {
                    NoReload = _checked;
                    Subtitle.Custom($"No reload is now {(_checked ? "enabled" : "disabled")}.");
                }
                else if (item == unlimitedAmmo)
                {
                    UnlimitedAmmo = _checked;
                    Subtitle.Custom($"Unlimited ammo is now {(_checked ? "enabled" : "disabled")}.");
                }
            };
            #endregion
        }


        #endregion

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