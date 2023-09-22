using ScaleformUI.Menu;

namespace vMenuClient.menus
{
    public class About
    {
        // Variables
        private UIMenu menu;

        private void CreateMenu()
        {
            // Create the menu.
            menu = new UIMenu("vMenu", "About vMenu");

            // Create menu items.
            var version = new UIMenuItem("vMenu Version", $"This server is using vMenu ~b~~h~{MainMenu.Version}~h~~s~ and ScaleformUI ~b~~h~4.3.5 The Gary - Avocato~h~~s~.");
            version.SetRightLabel($"~h~{MainMenu.Version}~h~ - ~h4.3.5 The Gary - Avocato~");
            var credits = new UIMenuItem("About vMenu / Credits", "vMenu is made by ~b~Vespura~s~. For more info, checkout ~b~www.vespura.com/vmenu~s~. Thank you to: Deltanic, Brigliar, IllusiveTea, Shayan Doust, zr0iq and Golden for your contributions!~n~ ScaleformUI by Manups4e, More info at ~b~github.com/manups4e/ScaleformUI~s~");

            var serverInfoMessage = vMenuShared.ConfigManager.GetSettingsString(vMenuShared.ConfigManager.Setting.vmenu_server_info_message);
            if (!string.IsNullOrEmpty(serverInfoMessage))
            {
                var serverInfo = new UIMenuItem("Server Info", serverInfoMessage);
                var siteUrl = vMenuShared.ConfigManager.GetSettingsString(vMenuShared.ConfigManager.Setting.vmenu_server_info_website_url);
                if (!string.IsNullOrEmpty(siteUrl))
                {
                    serverInfo.SetRightLabel($"{siteUrl}");
                }
                menu.AddItem(serverInfo);
            }
            menu.AddItem(version);
            menu.AddItem(credits);
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
