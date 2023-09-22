using System.Collections.Generic;

using ScaleformUI.Menu;

using vMenuShared;

namespace vMenuClient.menus
{
    public class WeatherOptions
    {
        // Variables
        private UIMenu menu;
        public UIMenuCheckboxItem dynamicWeatherEnabled;
        public UIMenuCheckboxItem blackout;
        public UIMenuCheckboxItem snowEnabled;
        public static readonly List<string> weatherTypes = new()
        {
            "EXTRASUNNY",
            "CLEAR",
            "NEUTRAL",
            "SMOG",
            "FOGGY",
            "CLOUDS",
            "OVERCAST",
            "CLEARING",
            "RAIN",
            "THUNDER",
            "BLIZZARD",
            "SNOW",
            "SNOWLIGHT",
            "XMAS",
            "HALLOWEEN"
        };

        private void CreateMenu()
        {
            // Create the menu.
            menu = new UIMenu(Game.Player.Name, "Weather Options");

            dynamicWeatherEnabled = new UIMenuCheckboxItem("Toggle Dynamic Weather", EventManager.DynamicWeatherEnabled, "Enable or disable dynamic weather changes.");
            blackout = new UIMenuCheckboxItem("Toggle Blackout", EventManager.IsBlackoutEnabled, "This disables or enables all lights across the map.");
            snowEnabled = new UIMenuCheckboxItem("Enable Snow Effects", ConfigManager.GetSettingsBool(ConfigManager.Setting.vmenu_enable_snow), "This will force snow to appear on the ground and enable snow particle effects for peds and vehicles. Combine with X-MAS or Light Snow weather for best results.");
            UIMenuItem extrasunny = new UIMenuItem("Extra Sunny", "Set the weather to ~y~extra sunny~s~!") { ItemData = "EXTRASUNNY" };
            UIMenuItem clear = new UIMenuItem("Clear", "Set the weather to ~y~clear~s~!") { ItemData = "CLEAR" };
            UIMenuItem neutral = new UIMenuItem("Neutral", "Set the weather to ~y~neutral~s~!") { ItemData = "NEUTRAL" };
            UIMenuItem smog = new UIMenuItem("Smog", "Set the weather to ~y~smog~s~!") { ItemData = "SMOG" };
            UIMenuItem foggy = new UIMenuItem("Foggy", "Set the weather to ~y~foggy~s~!") { ItemData = "FOGGY" };
            UIMenuItem clouds = new UIMenuItem("Cloudy", "Set the weather to ~y~clouds~s~!") { ItemData = "CLOUDS" };
            UIMenuItem overcast = new UIMenuItem("Overcast", "Set the weather to ~y~overcast~s~!") { ItemData = "OVERCAST" };
            UIMenuItem clearing = new UIMenuItem("Clearing", "Set the weather to ~y~clearing~s~!") { ItemData = "CLEARING" };
            UIMenuItem rain = new UIMenuItem("Rainy", "Set the weather to ~y~rain~s~!") { ItemData = "RAIN" };
            UIMenuItem thunder = new UIMenuItem("Thunder", "Set the weather to ~y~thunder~s~!") { ItemData = "THUNDER" };
            UIMenuItem blizzard = new UIMenuItem("Blizzard", "Set the weather to ~y~blizzard~s~!") { ItemData = "BLIZZARD" };
            UIMenuItem snow = new UIMenuItem("Snow", "Set the weather to ~y~snow~s~!") { ItemData = "SNOW" };
            UIMenuItem snowlight = new UIMenuItem("Light Snow", "Set the weather to ~y~light snow~s~!") { ItemData = "SNOWLIGHT" };
            UIMenuItem xmas = new UIMenuItem("X-MAS Snow", "Set the weather to ~y~x-mas~s~!") { ItemData = "XMAS" };
            UIMenuItem halloween = new UIMenuItem("Halloween", "Set the weather to ~y~halloween~s~!") { ItemData = "HALLOWEEN" };
            UIMenuItem removeclouds = new UIMenuItem("Remove All Clouds", "Remove all clouds from the sky!");
            UIMenuItem randomizeclouds = new UIMenuItem("Randomize Clouds", "Add random clouds to the sky!");

            if (IsAllowed(Permission.WODynamic))
            {
                menu.AddItem(dynamicWeatherEnabled);
            }
            if (IsAllowed(Permission.WOBlackout))
            {
                menu.AddItem(blackout);
            }
            if (IsAllowed(Permission.WOSetWeather))
            {
                menu.AddItem(snowEnabled);
                menu.AddItem(extrasunny);
                menu.AddItem(clear);
                menu.AddItem(neutral);
                menu.AddItem(smog);
                menu.AddItem(foggy);
                menu.AddItem(clouds);
                menu.AddItem(overcast);
                menu.AddItem(clearing);
                menu.AddItem(rain);
                menu.AddItem(thunder);
                menu.AddItem(blizzard);
                menu.AddItem(snow);
                menu.AddItem(snowlight);
                menu.AddItem(xmas);
                menu.AddItem(halloween);
            }
            if (IsAllowed(Permission.WORandomizeClouds))
            {
                menu.AddItem(randomizeclouds);
            }

            if (IsAllowed(Permission.WORemoveClouds))
            {
                menu.AddItem(removeclouds);
            }

            menu.OnItemSelect += (sender, item, index2) =>
            {
                if (item == removeclouds)
                {
                    ModifyClouds(true);
                }
                else if (item == randomizeclouds)
                {
                    ModifyClouds(false);
                }
                else if (item.ItemData is string weatherType)
                {
                    Notify.Custom($"The weather will be changed to ~y~{item.Label}~s~. This will take {EventManager.WeatherChangeTime} seconds.");
                    UpdateServerWeather(weatherType, EventManager.IsBlackoutEnabled, EventManager.DynamicWeatherEnabled, EventManager.IsSnowEnabled);
                }
            };

            menu.OnCheckboxChange += (sender, item, _checked) =>
            {
                if (item == dynamicWeatherEnabled)
                {
                    Notify.Custom($"Dynamic weather changes are now {(_checked ? "~g~enabled" : "~r~disabled")}~s~.");
                    UpdateServerWeather(EventManager.GetServerWeather, EventManager.IsBlackoutEnabled, _checked, EventManager.IsSnowEnabled);
                }
                else if (item == blackout)
                {
                    Notify.Custom($"Blackout mode is now {(_checked ? "~g~enabled" : "~r~disabled")}~s~.");
                    UpdateServerWeather(EventManager.GetServerWeather, _checked, EventManager.DynamicWeatherEnabled, EventManager.IsSnowEnabled);
                }
                else if (item == snowEnabled)
                {
                    Notify.Custom($"Snow effects will now be forced {(_checked ? "~g~enabled" : "~r~disabled")}~s~.");
                    UpdateServerWeather(EventManager.GetServerWeather, EventManager.IsBlackoutEnabled, EventManager.DynamicWeatherEnabled, _checked);
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
    }
}
