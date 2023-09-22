using ScaleformUI.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vMenuClient.menus
{
    public class OnlinePlayers
    {
        public List<int> PlayersWaypointList = new();
        public Dictionary<int, int> PlayerCoordWaypoints = new();

        // Menu variable, will be defined in CreateMenu()
        private UIMenu menu;

        readonly UIMenu playerMenu = new("Online Players", "Player:");
        IPlayer currentPlayer = new NativePlayer(Game.Player);


        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            // Create the menu.
            menu = new UIMenu(Game.Player.Name, "Online Players");

            UIMenuItem sendMessage = new UIMenuItem("Send Private Message", "Sends a private message to this player. ~r~Note: staff may be able to see all PM's.");
            UIMenuItem teleport = new UIMenuItem("Teleport To Player", "Teleport to this player.");
            UIMenuItem teleportVeh = new UIMenuItem("Teleport Into Player Vehicle", "Teleport into the vehicle of the player.");
            UIMenuItem summon = new UIMenuItem("Summon Player", "Teleport the player to you.");
            UIMenuItem toggleGPS = new UIMenuItem("Toggle GPS", "Enables or disables the GPS route on your radar to this player.");
            UIMenuItem spectate = new UIMenuItem("Spectate Player", "Spectate this player. Click this button again to stop spectating.");
            UIMenuItem printIdentifiers = new UIMenuItem("Print Identifiers", "This will print the player's identifiers to the client console (F8). And also save it to the CitizenFX.log file.");
            UIMenuItem kill = new UIMenuItem("~r~Kill Player", "Kill this player, note they will receive a notification saying that you killed them. It will also be logged in the Staff Actions log.");
            UIMenuItem kick = new UIMenuItem("~r~Kick Player", "Kick the player from the server.");
            UIMenuItem ban = new UIMenuItem("~r~Ban Player Permanently", "Ban this player permanently from the server. Are you sure you want to do this? You can specify the ban reason after clicking this button.");
            UIMenuItem tempban = new UIMenuItem("~r~Ban Player Temporarily", "Give this player a tempban of up to 30 days (max). You can specify duration and ban reason after clicking this button.");

            // always allowed
            playerMenu.AddItem(sendMessage);
            // permissions specific
            if (IsAllowed(Permission.OPTeleport))
            {
                playerMenu.AddItem(teleport);
                playerMenu.AddItem(teleportVeh);
            }
            if (IsAllowed(Permission.OPSummon))
            {
                playerMenu.AddItem(summon);
            }
            if (IsAllowed(Permission.OPSpectate))
            {
                playerMenu.AddItem(spectate);
            }
            if (IsAllowed(Permission.OPWaypoint))
            {
                playerMenu.AddItem(toggleGPS);
            }
            if (IsAllowed(Permission.OPIdentifiers))
            {
                playerMenu.AddItem(printIdentifiers);
            }
            if (IsAllowed(Permission.OPKill))
            {
                playerMenu.AddItem(kill);
            }
            if (IsAllowed(Permission.OPKick))
            {
                playerMenu.AddItem(kick);
            }
            if (IsAllowed(Permission.OPTempBan))
            {
                playerMenu.AddItem(tempban);
            }
            if (IsAllowed(Permission.OPPermBan))
            {
                playerMenu.AddItem(ban);
                ban.SetLeftBadge(BadgeIcon.WARNING);
            }

            playerMenu.OnMenuClose += (sender) =>
            {
                ban.SetRightLabel("");
            };

            playerMenu.OnIndexChange += (sender, newIndex) =>
            {
                ban.SetRightLabel("");
            };

            // handle button presses for the specific player's menu.
            playerMenu.OnItemSelect += async (sender, item, index) =>
            {
                // send message
                if (item == sendMessage)
                {
                    if (MainMenu.MiscSettingsMenu != null && !MainMenu.MiscSettingsMenu.MiscDisablePrivateMessages)
                    {
                        string message = await GetUserInput($"Private Message To {currentPlayer.Name}", 200);
                        if (string.IsNullOrEmpty(message))
                        {
                            Notify.Error(CommonErrors.InvalidInput);
                        }
                        else
                        {
                            TriggerServerEvent("vMenu:SendMessageToPlayer", currentPlayer.ServerId, message);
                            PrivateMessage(currentPlayer.ServerId.ToString(), message, true);
                        }
                    }
                    else
                    {
                        Notify.Error("You can't send a private message if you have private messages disabled yourself. Enable them in the Misc Settings menu and try again.");
                    }

                }
                // teleport (in vehicle) button
                else if (item == teleport || item == teleportVeh)
                {
                    if (!currentPlayer.IsLocal)
                    {
                        _ = TeleportToPlayer(currentPlayer, item == teleportVeh); // teleport to the player. optionally in the player's vehicle if that button was pressed.
                    }
                    else
                    {
                        Notify.Error("You can not teleport to yourself!");
                    }
                }
                // summon button
                else if (item == summon)
                {
                    if (Game.Player.Handle != currentPlayer.Handle)
                    {
                        SummonPlayer(currentPlayer);
                    }
                    else
                    {
                        Notify.Error("You can't summon yourself.");
                    }
                }
                // spectating
                else if (item == spectate)
                {
                    SpectatePlayer(currentPlayer);
                }
                // kill button
                else if (item == kill)
                {
                    KillPlayer(currentPlayer);
                }
                // manage the gps route being clicked.
                else if (item == toggleGPS)
                {
                    bool selectedPedRouteAlreadyActive = false;
                    if (PlayersWaypointList.Count > 0)
                    {
                        if (PlayersWaypointList.Contains(currentPlayer.ServerId))
                        {
                            selectedPedRouteAlreadyActive = true;
                        }
                        foreach (int serverId in PlayersWaypointList)
                        {
                            // remove any coord blip
                            if (PlayerCoordWaypoints.TryGetValue(serverId, out int wp))
                            {
                                SetBlipRoute(wp, false);
                                RemoveBlip(ref wp);

                                PlayerCoordWaypoints.Remove(serverId);
                            }

                            // remove any entity blip
                            int playerId = GetPlayerFromServerId(serverId);

                            if (playerId < 0)
                            {
                                continue;
                            }

                            int playerPed = GetPlayerPed(playerId);
                            if (DoesEntityExist(playerPed) && DoesBlipExist(GetBlipFromEntity(playerPed)))
                            {
                                int oldBlip = GetBlipFromEntity(playerPed);
                                SetBlipRoute(oldBlip, false);
                                RemoveBlip(ref oldBlip);
                                Notify.Custom($"~g~GPS route to ~s~<C>{GetSafePlayerName(currentPlayer.Name)}</C>~g~ is now disabled.");
                            }
                        }
                        PlayersWaypointList.Clear();
                    }

                    if (!selectedPedRouteAlreadyActive)
                    {
                        if (currentPlayer.ServerId != Game.Player.ServerId)
                        {
                            int blip;

                            if (currentPlayer.IsActive && currentPlayer.Character != null)
                            {
                                int ped = GetPlayerPed(currentPlayer.Handle);
                                blip = GetBlipFromEntity(ped);
                                if (!DoesBlipExist(blip))
                                {
                                    blip = AddBlipForEntity(ped);
                                }
                            }
                            else
                            {
                                if (!PlayerCoordWaypoints.TryGetValue(currentPlayer.ServerId, out blip))
                                {
                                    Vector3 coords = await MainMenu.RequestPlayerCoordinates(currentPlayer.ServerId);
                                    blip = AddBlipForCoord(coords.X, coords.Y, coords.Z);
                                    PlayerCoordWaypoints[currentPlayer.ServerId] = blip;
                                }
                            }

                            SetBlipColour(blip, 58);
                            SetBlipRouteColour(blip, 58);
                            SetBlipRoute(blip, true);

                            PlayersWaypointList.Add(currentPlayer.ServerId);
                            Notify.Custom($"~g~GPS route to ~s~<C>{GetSafePlayerName(currentPlayer.Name)}</C>~g~ is now active, press the ~s~Toggle GPS Route~g~ button again to disable the route.");
                        }
                        else
                        {
                            Notify.Error("You can not set a waypoint to yourself.");
                        }
                    }
                }
                else if (item == printIdentifiers)
                {
                    Func<string, string> CallbackFunction = (data) =>
                    {
                        Debug.WriteLine(data);
                        string ids = "~s~";
                        foreach (string s in JsonConvert.DeserializeObject<string[]>(data))
                        {
                            ids += "~n~" + s;
                        }
                        Notify.Custom($"~y~<C>{GetSafePlayerName(currentPlayer.Name)}</C>~g~'s Identifiers: {ids}", false);
                        return data;
                    };
                    BaseScript.TriggerServerEvent("vMenu:GetPlayerIdentifiers", currentPlayer.ServerId, CallbackFunction);
                }
                // kick button
                else if (item == kick)
                {
                    if (currentPlayer.Handle != Game.Player.Handle)
                    {
                        KickPlayer(currentPlayer, true);
                    }
                    else
                    {
                        Notify.Error("You cannot kick yourself!");
                    }
                }
                // temp ban
                else if (item == tempban)
                {
                    BanPlayer(currentPlayer, false);
                }
                // perm ban
                else if (item == ban)
                {
                    if (ban.RightLabel == "Are you sure?")
                    {
                        ban.SetRightLabel("");
                        _ = UpdatePlayerlist();
                        playerMenu.GoBack();
                        BanPlayer(currentPlayer, true);
                    }
                    else
                    {
                        ban.SetRightLabel("Are you sure?");
                    }
                }
            };

            // handle button presses in the player list.
            menu.OnItemSelect += (sender, item, index) =>
                {
                    int baseId = int.Parse(item.Label.Replace(" →→→", "").Replace("Server #", ""));
                    IPlayer player = MainMenu.PlayersList.FirstOrDefault(p => p.ServerId == baseId);

                    if (player != null)
                    {
                        currentPlayer = player;
                        playerMenu.Subtitle = $"~s~Player: ~y~{GetSafePlayerName(currentPlayer.Name)}";
                        //playerMenu.CounterPreText = $"[Server ID: ~y~{currentPlayer.ServerId}~s~] ";
                    }
                    else
                    {
                        playerMenu.GoBack();
                    }
                };
        }

        /// <summary>
        /// Updates the player items.
        /// </summary>
        public async Task UpdatePlayerlist()
        {
            void UpdateStuff()
            {
                menu.Clear();

                foreach (IPlayer p in MainMenu.PlayersList.OrderBy(a => a.Name))
                {
                    UIMenuItem pItem = new UIMenuItem($"{GetSafePlayerName(p.Name)}", $"Click to view the options for this player. Server ID: {p.ServerId}. Local ID: {p.Handle}.");
                    pItem.SetRightLabel($"Server #{p.ServerId} →→→");
                    menu.AddItem(pItem);
                    pItem.Activated += async (a, b) => await a.SwitchTo(playerMenu, 0, true);
                }
            }

            // First, update *before* waiting - so we get all local players.
            UpdateStuff();
            await MainMenu.PlayersList.WaitRequested();

            // Update after waiting too so we have all remote players.
            UpdateStuff();
        }

        /// <summary>
        /// Checks if the menu exists, if not then it creates it first.
        /// Then returns the menu.
        /// </summary>
        /// <returns>The Online Players Menu</returns>
        public UIMenu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
                return menu;
            }
            else
            {
                _ = UpdatePlayerlist();
                return menu;
            }
        }
    }
}
