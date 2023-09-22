using ScaleformUI.Menu;
using System;
using System.Collections.Generic;
using System.Linq;

namespace vMenuClient.menus
{
    public class BannedPlayers
    {
        // Variables
        private UIMenu menu;

        /// <summary>
        /// Struct used to store bans.
        /// </summary>
        public struct BanRecord
        {
            public string playerName;
            public List<string> identifiers;
            public DateTime bannedUntil;
            public string banReason;
            public string bannedBy;
            public string uuid;
        }

        BanRecord currentRecord = new();

        public List<BanRecord> banlist = new();

        readonly UIMenu bannedPlayer = new("Banned Player", "Ban Record: ");

        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            menu = new UIMenu(Game.Player.Name, "Banned Players Management");
            ScaleformUI.Scaleforms.InstructionalButton ib = new ScaleformUI.Scaleforms.InstructionalButton(Control.Jump, "Filter Options");
            menu.InstructionalButtons.Add(ib);
            ib.OnControlSelected += async (button) =>
            {
                string filterText = await GetUserInput("Filter username or ban id (leave this empty to reset the filter)");
                List<UIMenuItem> itList = menu.MenuItems;
                menu.Clear();
                if (banlist.Count > 1)
                {
                    if (string.IsNullOrEmpty(filterText))
                    {
                        Subtitle.Custom("Filters have been cleared.");
                        menu.MenuItems = itList;
                        UpdateBans();
                    }
                    else
                    {
                        menu.MenuItems = itList.Where(item => item.ItemData is BanRecord br && (br.playerName.ToLower().Contains(filterText.ToLower()) || br.uuid.ToLower().Contains(filterText.ToLower()))).ToList();
                        Subtitle.Custom("Filter has been applied.");
                    }
                }
                else
                {
                    Notify.Error("At least 2 players need to be banned in order to use the filter function.");
                }

                Log($"Button pressed: {menu.Title} {button.Text}");
            };

            bannedPlayer.AddItem(new UIMenuItem("Player Name"));
            bannedPlayer.AddItem(new UIMenuItem("Banned By"));
            bannedPlayer.AddItem(new UIMenuItem("Banned Until"));
            bannedPlayer.AddItem(new UIMenuItem("Player Identifiers"));
            bannedPlayer.AddItem(new UIMenuItem("Banned For"));
            bannedPlayer.AddItem(new UIMenuItem("~r~Unban", "~r~Warning, unbanning the player can NOT be undone. You will NOT be able to ban them again until they re-join the server. Are you absolutely sure you want to unban this player? ~s~Tip: Tempbanned players will automatically get unbanned if they log on to the server after their ban date has expired."));

            // should be enough for now to cover all possible identifiers.
            List<string> colors = new List<string>() { "~r~", "~g~", "~b~", "~o~", "~y~", "~p~", "~s~", "~t~", };

            bannedPlayer.OnMenuClose += (sender) =>
            {
                BaseScript.TriggerServerEvent("vMenu:RequestBanList", Game.Player.Handle);
                bannedPlayer.MenuItems[5].SetRightLabel("");
                UpdateBans();
            };

            bannedPlayer.OnIndexChange += (a, b) =>
            {
                bannedPlayer.MenuItems[5].SetRightLabel("");
            };

            bannedPlayer.OnItemSelect += (sender, item, index) =>
            {
                if (index == 5 && IsAllowed(Permission.OPUnban))
                {
                    if (item.RightLabel == "Are you sure?")
                    {
                        if (banlist.Contains(currentRecord))
                        {
                            UnbanPlayer(banlist.IndexOf(currentRecord));
                            bannedPlayer.MenuItems[5].SetRightLabel("");
                            bannedPlayer.GoBack();
                        }
                        else
                        {
                            Notify.Error("Somehow you managed to click the unban button but this ban record you're apparently viewing does not even exist. Weird...");
                        }
                    }
                    else
                    {
                        item.SetRightLabel("Are you sure?");
                    }
                }
                else
                {
                    bannedPlayer.MenuItems[5].SetRightLabel("");
                }

            };

            menu.OnItemSelect += (sender, item, index) =>
            {
                currentRecord = item.ItemData;

                bannedPlayer.Subtitle = "Ban Record: ~y~" + currentRecord.playerName;
                UIMenuItem nameItem = bannedPlayer.MenuItems[0];
                UIMenuItem bannedByItem = bannedPlayer.MenuItems[1];
                UIMenuItem bannedUntilItem = bannedPlayer.MenuItems[2];
                UIMenuItem playerIdentifiersItem = bannedPlayer.MenuItems[3];
                UIMenuItem banReasonItem = bannedPlayer.MenuItems[4];
                nameItem.SetRightLabel(currentRecord.playerName);
                nameItem.Description = "Player name: ~y~" + currentRecord.playerName;
                bannedByItem.SetRightLabel(currentRecord.bannedBy);
                bannedByItem.Description = "Player banned by: ~y~" + currentRecord.bannedBy;
                if (currentRecord.bannedUntil.Date.Year == 3000)
                {
                    bannedUntilItem.SetRightLabel("Forever");
                }
                else
                {
                    bannedUntilItem.SetRightLabel(currentRecord.bannedUntil.Date.ToString());
                }

                bannedUntilItem.Description = "This player is banned until: " + currentRecord.bannedUntil.Date.ToString();
                playerIdentifiersItem.Description = "";

                int i = 0;
                foreach (string id in currentRecord.identifiers)
                {
                    // only (admins) people that can unban players are allowed to view IP's.
                    // this is just a slight 'safety' feature in case someone who doesn't know what they're doing
                    // gave builtin.everyone access to view the banlist.
                    if (id.StartsWith("ip:") && !IsAllowed(Permission.OPUnban))
                    {
                        playerIdentifiersItem.Description += $"{colors[i]}ip: (hidden) ";
                    }
                    else
                    {
                        playerIdentifiersItem.Description += $"{colors[i]}{id.Replace(":", ": ")} ";
                    }
                    i++;
                }
                banReasonItem.Description = "Banned for: " + currentRecord.banReason;

                UIMenuItem unbanPlayerBtn = bannedPlayer.MenuItems[5];
                unbanPlayerBtn.SetRightLabel("");
                if (!IsAllowed(Permission.OPUnban))
                {
                    unbanPlayerBtn.Enabled = false;
                    unbanPlayerBtn.Description = "You are not allowed to unban players. You are only allowed to view their ban record.";
                    unbanPlayerBtn.SetLeftBadge(BadgeIcon.LOCK);
                }
            };
        }

        /// <summary>
        /// Updates the ban list menu.
        /// </summary>
        public void UpdateBans()
        {
            //menu.ResetFilter();
            menu.Clear();

            foreach (BanRecord ban in banlist)
            {
                UIMenuItem recordBtn = new UIMenuItem(ban.playerName, $"~y~{ban.playerName}~s~ was banned by ~y~{ban.bannedBy}~s~ until ~y~{ban.bannedUntil}~s~ for ~y~{ban.banReason}~s~.");
                recordBtn.SetRightLabel("→→→");
                recordBtn.ItemData = ban;
                menu.AddItem(recordBtn);
                recordBtn.Activated += async (a, b) => await a.SwitchTo(bannedPlayer, 0, true);
            }
        }

        /// <summary>
        /// Updates the list of ban records.
        /// </summary>
        /// <param name="banJsonString"></param>
        public void UpdateBanList(string banJsonString)
        {
            banlist.Clear();
            banlist = JsonConvert.DeserializeObject<List<BanRecord>>(banJsonString);
            UpdateBans();
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

        /// <summary>
        /// Sends an event to the server requesting the player to be unbanned.
        /// We'll just assume that worked fine, so remove the item from our local list, we'll re-sync once the menu is re-opened.
        /// </summary>
        /// <param name="index"></param>
        private void UnbanPlayer(int index)
        {
            BanRecord record = banlist[index];
            banlist.Remove(record);
            BaseScript.TriggerServerEvent("vMenu:RequestPlayerUnban", record.uuid);
        }

        /// <summary>
        /// Converts the ban record (json object) into a BanRecord struct.
        /// </summary>
        /// <param name="banRecordJsonObject"></param>
        /// <returns></returns>
        public static BanRecord JsonToBanRecord(dynamic banRecordJsonObject)
        {
            BanRecord newBr = new BanRecord();
            foreach (Newtonsoft.Json.Linq.JProperty brValue in banRecordJsonObject)
            {
                string key = brValue.Name.ToString();
                Newtonsoft.Json.Linq.JToken value = brValue.Value;
                if (key == "playerName")
                {
                    newBr.playerName = value.ToString();
                }
                else if (key == "identifiers")
                {
                    List<string> tmpList = new List<string>();
                    foreach (Newtonsoft.Json.Linq.JToken identifier in value)
                    {
                        tmpList.Add(identifier.ToString());
                    }
                    newBr.identifiers = tmpList;
                }
                else if (key == "bannedUntil")
                {
                    newBr.bannedUntil = DateTime.Parse(value.ToString());
                }
                else if (key == "banReason")
                {
                    newBr.banReason = value.ToString();
                }
                else if (key == "bannedBy")
                {
                    newBr.bannedBy = value.ToString();
                }
            }
            return newBr;
        }
    }
}
