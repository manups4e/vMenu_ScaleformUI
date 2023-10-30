using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ScaleformUI.Menu;

using vMenuClient.data;

using static vMenuClient.MpPedDataManager;

namespace vMenuClient.menus
{
    public class MpPedCustomization
    {
        // Variables
        private UIMenu menu;
        public UIMenu createCharacterMenu = new("Create Character", "Create A New Character");
        public UIMenu savedCharactersMenu = new("vMenu", "Manage Saved Characters");
        public UIMenu inheritanceMenu = new("vMenu", "Character Inheritance Options");
        public UIMenu appearanceMenu = new("vMenu", "Character Appearance Options");
        public UIMenu faceShapeMenu = new("vMenu", "Character Face Shape Options");
        public UIMenu tattoosMenu = new("vMenu", "Character Tattoo Options");
        public UIMenu clothesMenu = new("vMenu", "Character Clothing Options");
        public UIMenu propsMenu = new("vMenu", "Character Props Options");
        private readonly UIMenu manageSavedCharacterMenu = new("vMenu", "Manage MP Character");

        // Need to be able to disable/enable these buttons from another class.
        internal UIMenuItem createMaleBtn = new("Create Male Character", "Create a new male character.");
        internal UIMenuItem createFemaleBtn = new("Create Female Character", "Create a new female character.");
        internal UIMenuItem editPedBtn = new("Edit Saved Character", "This allows you to edit everything about your saved character. The changes will be saved to this character's save file entry once you hit the save button.");

        public static bool DontCloseMenus { get; set; }
        public static bool DisableBackButton { get; set; }
        string selectedSavedCharacterManageName = "";
        private bool isEdidtingPed = false;
        private readonly List<dynamic> facial_expressions = new() { "mood_Normal_1", "mood_Happy_1", "mood_Angry_1", "mood_Aiming_1", "mood_Injured_1", "mood_stressed_1", "mood_smug_1", "mood_sulk_1", };

        private MultiplayerPedData currentCharacter = new();



        /// <summary>
        /// Makes or updates the character creator menu. Also has an option to load data from the <see cref="currentCharacter"/> data, to allow for editing an existing ped.
        /// </summary>
        /// <param name="male"></param>
        /// <param name="editPed"></param>
        private void MakeCreateCharacterMenu(bool male, bool editPed = false)
        {
            createMaleBtn.SetRightLabel("→→→");
            createFemaleBtn.SetRightLabel("→→→");
            isEdidtingPed = editPed;
            if (!editPed)
            {
                currentCharacter = new MultiplayerPedData();
                currentCharacter.DrawableVariations.clothes = new Dictionary<int, KeyValuePair<int, int>>();
                currentCharacter.PropVariations.props = new Dictionary<int, KeyValuePair<int, int>>();
                currentCharacter.PedHeadBlendData = Game.PlayerPed.GetHeadBlendData();
                currentCharacter.Version = 1;
                currentCharacter.ModelHash = male ? (uint)GetHashKey("mp_m_freemode_01") : (uint)GetHashKey("mp_f_freemode_01");
                currentCharacter.IsMale = male;

                SetPedComponentVariation(Game.PlayerPed.Handle, 3, 15, 0, 0);
                SetPedComponentVariation(Game.PlayerPed.Handle, 8, 15, 0, 0);
                SetPedComponentVariation(Game.PlayerPed.Handle, 11, 15, 0, 0);
            }
            currentCharacter.DrawableVariations.clothes ??= new Dictionary<int, KeyValuePair<int, int>>();
            currentCharacter.PropVariations.props ??= new Dictionary<int, KeyValuePair<int, int>>();

            // Set the facial expression to default in case it doesn't exist yet, or keep the current one if it does.
            currentCharacter.FacialExpression ??= facial_expressions[0];

            // Set the facial expression on the ped itself.
            SetFacialIdleAnimOverride(Game.PlayerPed.Handle, currentCharacter.FacialExpression ?? facial_expressions[0], null);

            // Set the facial expression item list to the correct saved index.
            if (createCharacterMenu.MenuItems.ElementAt(6) is UIMenuListItem li)
            {
                dynamic index = facial_expressions.IndexOf(currentCharacter.FacialExpression ?? facial_expressions[0]);
                if (index < 0)
                {
                    index = 0;
                }
                li.Index = index;
            }

            appearanceMenu.Clear();
            tattoosMenu.Clear();
            clothesMenu.Clear();
            propsMenu.Clear();

            #region appearance menu.
            List<dynamic> opacity = new List<dynamic>() { "0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%" };

            List<dynamic> overlayColorsList = new List<dynamic>();
            for (int i = 0; i < GetNumHairColors(); i++)
            {
                overlayColorsList.Add($"Color #{i + 1}");
            }

            int maxHairStyles = GetNumberOfPedDrawableVariations(Game.PlayerPed.Handle, 2);
            //if (currentCharacter.ModelHash == (uint)PedHash.FreemodeFemale01)
            //{
            //    maxHairStyles /= 2;
            //}
            List<dynamic> hairStylesList = new List<dynamic>();
            for (int i = 0; i < maxHairStyles; i++)
            {
                hairStylesList.Add($"Style #{i + 1}");
            }
            hairStylesList.Add($"Style #{maxHairStyles + 1}");

            List<dynamic> blemishesStyleList = new List<dynamic>();
            for (int i = 0; i < GetNumHeadOverlayValues(0); i++)
            {
                blemishesStyleList.Add($"Style #{i + 1}");
            }

            List<dynamic> beardStylesList = new List<dynamic>();
            for (int i = 0; i < GetNumHeadOverlayValues(1); i++)
            {
                beardStylesList.Add($"Style #{i + 1}");
            }

            List<dynamic> eyebrowsStyleList = new List<dynamic>();
            for (int i = 0; i < GetNumHeadOverlayValues(2); i++)
            {
                eyebrowsStyleList.Add($"Style #{i + 1}");
            }

            List<dynamic> ageingStyleList = new List<dynamic>();
            for (int i = 0; i < GetNumHeadOverlayValues(3); i++)
            {
                ageingStyleList.Add($"Style #{i + 1}");
            }

            List<dynamic> makeupStyleList = new List<dynamic>();
            for (int i = 0; i < GetNumHeadOverlayValues(4); i++)
            {
                makeupStyleList.Add($"Style #{i + 1}");
            }

            List<dynamic> blushStyleList = new List<dynamic>();
            for (int i = 0; i < GetNumHeadOverlayValues(5); i++)
            {
                blushStyleList.Add($"Style #{i + 1}");
            }

            List<dynamic> complexionStyleList = new List<dynamic>();
            for (int i = 0; i < GetNumHeadOverlayValues(6); i++)
            {
                complexionStyleList.Add($"Style #{i + 1}");
            }

            List<dynamic> sunDamageStyleList = new List<dynamic>();
            for (int i = 0; i < GetNumHeadOverlayValues(7); i++)
            {
                sunDamageStyleList.Add($"Style #{i + 1}");
            }

            List<dynamic> lipstickStyleList = new List<dynamic>();
            for (int i = 0; i < GetNumHeadOverlayValues(8); i++)
            {
                lipstickStyleList.Add($"Style #{i + 1}");
            }

            List<dynamic> molesFrecklesStyleList = new List<dynamic>();
            for (int i = 0; i < GetNumHeadOverlayValues(9); i++)
            {
                molesFrecklesStyleList.Add($"Style #{i + 1}");
            }

            List<dynamic> chestHairStyleList = new List<dynamic>();
            for (int i = 0; i < GetNumHeadOverlayValues(10); i++)
            {
                chestHairStyleList.Add($"Style #{i + 1}");
            }

            List<dynamic> bodyBlemishesList = new List<dynamic>();
            for (int i = 0; i < GetNumHeadOverlayValues(11); i++)
            {
                bodyBlemishesList.Add($"Style #{i + 1}");
            }

            List<dynamic> eyeColorList = new List<dynamic>();
            for (int i = 0; i < 32; i++)
            {
                eyeColorList.Add($"Eye Color #{i + 1}");
            }

            /*

            0               Blemishes             0 - 23,   255  
            1               Facial Hair           0 - 28,   255  
            2               Eyebrows              0 - 33,   255  
            3               Ageing                0 - 14,   255  
            4               Makeup                0 - 74,   255  
            5               Blush                 0 - 6,    255  
            6               Complexion            0 - 11,   255  
            7               Sun Damage            0 - 10,   255  
            8               Lipstick              0 - 9,    255  
            9               Moles/Freckles        0 - 17,   255  
            10              Chest Hair            0 - 16,   255  
            11              Body Blemishes        0 - 11,   255  
            12              Add Body Blemishes    0 - 1,    255  
            
            */


            // hair
            int currentHairStyle = editPed ? currentCharacter.PedAppearance.hairStyle : GetPedDrawableVariation(Game.PlayerPed.Handle, 2);
            int currentHairColor = editPed ? currentCharacter.PedAppearance.hairColor : 0;
            int currentHairHighlightColor = editPed ? currentCharacter.PedAppearance.hairHighlightColor : 0;

            // 0 blemishes
            int currentBlemishesStyle = editPed ? currentCharacter.PedAppearance.blemishesStyle : GetPedHeadOverlayValue(Game.PlayerPed.Handle, 0) != 255 ? GetPedHeadOverlayValue(Game.PlayerPed.Handle, 0) : 0;
            float currentBlemishesOpacity = editPed ? currentCharacter.PedAppearance.blemishesOpacity : 0f;
            SetPedHeadOverlay(Game.PlayerPed.Handle, 0, currentBlemishesStyle, currentBlemishesOpacity);

            // 1 beard
            int currentBeardStyle = editPed ? currentCharacter.PedAppearance.beardStyle : GetPedHeadOverlayValue(Game.PlayerPed.Handle, 1) != 255 ? GetPedHeadOverlayValue(Game.PlayerPed.Handle, 1) : 0;
            float currentBeardOpacity = editPed ? currentCharacter.PedAppearance.beardOpacity : 0f;
            int currentBeardColor = editPed ? currentCharacter.PedAppearance.beardColor : 0;
            SetPedHeadOverlay(Game.PlayerPed.Handle, 1, currentBeardStyle, currentBeardOpacity);
            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, currentBeardColor, currentBeardColor);

            // 2 eyebrows
            int currentEyebrowStyle = editPed ? currentCharacter.PedAppearance.eyebrowsStyle : GetPedHeadOverlayValue(Game.PlayerPed.Handle, 2) != 255 ? GetPedHeadOverlayValue(Game.PlayerPed.Handle, 2) : 0;
            float currentEyebrowOpacity = editPed ? currentCharacter.PedAppearance.eyebrowsOpacity : 0f;
            int currentEyebrowColor = editPed ? currentCharacter.PedAppearance.eyebrowsColor : 0;
            SetPedHeadOverlay(Game.PlayerPed.Handle, 2, currentEyebrowStyle, currentEyebrowOpacity);
            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 2, 1, currentEyebrowColor, currentEyebrowColor);

            // 3 ageing
            int currentAgeingStyle = editPed ? currentCharacter.PedAppearance.ageingStyle : GetPedHeadOverlayValue(Game.PlayerPed.Handle, 3) != 255 ? GetPedHeadOverlayValue(Game.PlayerPed.Handle, 3) : 0;
            float currentAgeingOpacity = editPed ? currentCharacter.PedAppearance.ageingOpacity : 0f;
            SetPedHeadOverlay(Game.PlayerPed.Handle, 3, currentAgeingStyle, currentAgeingOpacity);

            // 4 makeup
            int currentMakeupStyle = editPed ? currentCharacter.PedAppearance.makeupStyle : GetPedHeadOverlayValue(Game.PlayerPed.Handle, 4) != 255 ? GetPedHeadOverlayValue(Game.PlayerPed.Handle, 4) : 0;
            float currentMakeupOpacity = editPed ? currentCharacter.PedAppearance.makeupOpacity : 0f;
            int currentMakeupColor = editPed ? currentCharacter.PedAppearance.makeupColor : 0;
            SetPedHeadOverlay(Game.PlayerPed.Handle, 4, currentMakeupStyle, currentMakeupOpacity);
            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 4, 2, currentMakeupColor, currentMakeupColor);

            // 5 blush
            int currentBlushStyle = editPed ? currentCharacter.PedAppearance.blushStyle : GetPedHeadOverlayValue(Game.PlayerPed.Handle, 5) != 255 ? GetPedHeadOverlayValue(Game.PlayerPed.Handle, 5) : 0;
            float currentBlushOpacity = editPed ? currentCharacter.PedAppearance.blushOpacity : 0f;
            int currentBlushColor = editPed ? currentCharacter.PedAppearance.blushColor : 0;
            SetPedHeadOverlay(Game.PlayerPed.Handle, 5, currentBlushStyle, currentBlushOpacity);
            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 5, 2, currentBlushColor, currentBlushColor);

            // 6 complexion
            int currentComplexionStyle = editPed ? currentCharacter.PedAppearance.complexionStyle : GetPedHeadOverlayValue(Game.PlayerPed.Handle, 6) != 255 ? GetPedHeadOverlayValue(Game.PlayerPed.Handle, 6) : 0;
            float currentComplexionOpacity = editPed ? currentCharacter.PedAppearance.complexionOpacity : 0f;
            SetPedHeadOverlay(Game.PlayerPed.Handle, 6, currentComplexionStyle, currentComplexionOpacity);

            // 7 sun damage
            int currentSunDamageStyle = editPed ? currentCharacter.PedAppearance.sunDamageStyle : GetPedHeadOverlayValue(Game.PlayerPed.Handle, 7) != 255 ? GetPedHeadOverlayValue(Game.PlayerPed.Handle, 7) : 0;
            float currentSunDamageOpacity = editPed ? currentCharacter.PedAppearance.sunDamageOpacity : 0f;
            SetPedHeadOverlay(Game.PlayerPed.Handle, 7, currentSunDamageStyle, currentSunDamageOpacity);

            // 8 lipstick
            int currentLipstickStyle = editPed ? currentCharacter.PedAppearance.lipstickStyle : GetPedHeadOverlayValue(Game.PlayerPed.Handle, 8) != 255 ? GetPedHeadOverlayValue(Game.PlayerPed.Handle, 8) : 0;
            float currentLipstickOpacity = editPed ? currentCharacter.PedAppearance.lipstickOpacity : 0f;
            int currentLipstickColor = editPed ? currentCharacter.PedAppearance.lipstickColor : 0;
            SetPedHeadOverlay(Game.PlayerPed.Handle, 8, currentLipstickStyle, currentLipstickOpacity);
            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 8, 2, currentLipstickColor, currentLipstickColor);

            // 9 moles/freckles
            int currentMolesFrecklesStyle = editPed ? currentCharacter.PedAppearance.molesFrecklesStyle : GetPedHeadOverlayValue(Game.PlayerPed.Handle, 9) != 255 ? GetPedHeadOverlayValue(Game.PlayerPed.Handle, 9) : 0;
            float currentMolesFrecklesOpacity = editPed ? currentCharacter.PedAppearance.molesFrecklesOpacity : 0f;
            SetPedHeadOverlay(Game.PlayerPed.Handle, 9, currentMolesFrecklesStyle, currentMolesFrecklesOpacity);

            // 10 chest hair
            int currentChesthairStyle = editPed ? currentCharacter.PedAppearance.chestHairStyle : GetPedHeadOverlayValue(Game.PlayerPed.Handle, 10) != 255 ? GetPedHeadOverlayValue(Game.PlayerPed.Handle, 10) : 0;
            float currentChesthairOpacity = editPed ? currentCharacter.PedAppearance.chestHairOpacity : 0f;
            int currentChesthairColor = editPed ? currentCharacter.PedAppearance.chestHairColor : 0;
            SetPedHeadOverlay(Game.PlayerPed.Handle, 10, currentChesthairStyle, currentChesthairOpacity);
            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 10, 1, currentChesthairColor, currentChesthairColor);

            // 11 body blemishes
            int currentBodyBlemishesStyle = editPed ? currentCharacter.PedAppearance.bodyBlemishesStyle : GetPedHeadOverlayValue(Game.PlayerPed.Handle, 11) != 255 ? GetPedHeadOverlayValue(Game.PlayerPed.Handle, 11) : 0;
            float currentBodyBlemishesOpacity = editPed ? currentCharacter.PedAppearance.bodyBlemishesOpacity : 0f;
            SetPedHeadOverlay(Game.PlayerPed.Handle, 11, currentBodyBlemishesStyle, currentBodyBlemishesOpacity);

            int currentEyeColor = editPed ? currentCharacter.PedAppearance.eyeColor : 0;
            SetPedEyeColor(Game.PlayerPed.Handle, currentEyeColor);

            UIMenuListItem hairStyles = new UIMenuListItem("Hair Style", hairStylesList, currentHairStyle, "Select a hair style.");
            //UIMenuListItem hairColors = new UIMenuListItem("Hair Color", overlayColorsList, currentHairColor, "Select a hair color.");
            UIMenuListItem hairColors = new UIMenuListItem("Hair Color", overlayColorsList, currentHairColor, "Select a hair color.");
            UIMenuColorPanel hairStylePanel = new UIMenuColorPanel("Hair Color", ColorPanelType.Hair);
            hairColors.AddPanel(hairStylePanel);
            //UIMenuListItem hairHighlightColors = new UIMenuListItem("Hair Highlight Color", overlayColorsList, currentHairHighlightColor, "Select a hair highlight color.");
            UIMenuListItem hairHighlightColors = new UIMenuListItem("Hair Highlight Color", overlayColorsList, currentHairHighlightColor, "Select a hair highlight color.");
            UIMenuColorPanel hairHighlightPanel = new UIMenuColorPanel("Hair Highlight Color", ColorPanelType.Hair);
            hairHighlightColors.AddPanel(hairHighlightPanel);

            UIMenuListItem blemishesStyle = new UIMenuListItem("Blemishes Style", blemishesStyleList, currentBlemishesStyle, "Select a blemishes style.");
            //MenuSliderItem blemishesOpacity = new UIMenuSliderItem("Blemishes Opacity", "Select a blemishes opacity.", 0, 10, (int)(currentBlemishesOpacity * 10f), false);
            UIMenuListItem blemishesOpacity = new UIMenuListItem("Blemishes Opacity", opacity, (int)(currentBlemishesOpacity * 10f), "Select a blemishes opacity.");
            UIMenuPercentagePanel blemishesOpacityPanel = new UIMenuPercentagePanel("Blemishes Opacity");
            blemishesOpacity.AddPanel(blemishesOpacityPanel);


            UIMenuListItem beardStyles = new UIMenuListItem("Beard Style", beardStylesList, currentBeardStyle, "Select a beard/facial hair style.");
            UIMenuListItem beardOpacity = new UIMenuListItem("Beard Opacity", opacity, (int)(currentBeardOpacity * 10f), "Select the opacity for your beard/facial hair.");
            UIMenuPercentagePanel beardOpacityPanel = new UIMenuPercentagePanel("Beard Opacity");
            beardOpacity.AddPanel(beardOpacityPanel);

            UIMenuListItem beardColor = new UIMenuListItem("Beard Color", overlayColorsList, currentBeardColor, "Select a beard color.");
            UIMenuColorPanel beardColorPanel = new UIMenuColorPanel("Beard Color", ColorPanelType.Hair);
            beardColor.AddPanel(beardColorPanel);

            //MenuSliderItem beardOpacity = new UIMenuSliderItem("Beard Opacity", "Select the opacity for your beard/facial hair.", 0, 10, (int)(currentBeardOpacity * 10f), false);
            //UIMenuListItem beardColor = new UIMenuListItem("Beard Color", overlayColorsList, currentBeardColor, "Select a beard color");

            UIMenuListItem eyebrowStyle = new UIMenuListItem("Eyebrows Style", eyebrowsStyleList, currentEyebrowStyle, "Select an eyebrows style.");
            UIMenuListItem eyebrowOpacity = new UIMenuListItem("Eyebrows Opacity", opacity, (int)(currentEyebrowOpacity * 10f), "Select the opacity for your eyebrows.");
            UIMenuPercentagePanel eyebrowOpacityPanel = new UIMenuPercentagePanel("Eyebrows Opacity");
            eyebrowOpacity.AddPanel(eyebrowOpacityPanel);

            UIMenuListItem eyebrowColor = new UIMenuListItem("Eyebrows Color", overlayColorsList, currentEyebrowColor, "Select an eyebrows color.");
            UIMenuColorPanel eyebrowColorPanel = new UIMenuColorPanel("Eyebrows Color", ColorPanelType.Hair);
            eyebrowColor.AddPanel(eyebrowColorPanel);

            //MenuSliderItem eyebrowOpacity = new UIMenuSliderItem("Eyebrows Opacity", "Select the opacity for your eyebrows.", 0, 10, (int)(currentEyebrowOpacity * 10f), false);

            UIMenuListItem ageingStyle = new UIMenuListItem("Ageing Style", ageingStyleList, currentAgeingStyle, "Select an ageing style.");
            UIMenuListItem ageingOpacity = new UIMenuListItem("Ageing Opacity", opacity, (int)(currentAgeingOpacity * 10f), "Select an ageing opacity.");
            UIMenuPercentagePanel ageingOpacityPanel = new UIMenuPercentagePanel("Ageing Opacity");
            ageingOpacity.AddPanel(ageingOpacityPanel);

            //MenuSliderItem ageingOpacity = new UIMenuSliderItem("Ageing Opacity", "Select an ageing opacity.", 0, 10, (int)(currentAgeingOpacity * 10f), false);

            UIMenuListItem makeupStyle = new UIMenuListItem("Makeup Style", makeupStyleList, currentMakeupStyle, "Select a makeup style.");
            UIMenuListItem makeupOpacity = new UIMenuListItem("Makeup Opacity", opacity, (int)(currentMakeupOpacity * 10f), "Select a makeup opacity");
            UIMenuPercentagePanel makeupOpacityPanel = new UIMenuPercentagePanel("Makeup Opacity");
            makeupOpacity.AddPanel(makeupOpacityPanel);

            //MenuSliderItem makeupOpacity = new UIMenuSliderItem("Makeup Opacity", 0, 10, (int)(currentMakeupOpacity * 10f), "Select a makeup opacity.");
            UIMenuListItem makeupColor = new UIMenuListItem("Makeup Color", overlayColorsList, currentMakeupColor, "Select a makeup color.");
            UIMenuColorPanel makeupColorPanel = new UIMenuColorPanel("Makeup Color", ColorPanelType.Makeup);
            makeupColor.AddPanel(makeupColorPanel);


            UIMenuListItem blushStyle = new UIMenuListItem("Blush Style", blushStyleList, currentBlushStyle, "Select a blush style.");
            UIMenuListItem blushOpacity = new UIMenuListItem("Blush Opacity", opacity, (int)(currentBlushOpacity * 10f), "Select a blush opacity.");
            UIMenuPercentagePanel blushOpacityPanel = new UIMenuPercentagePanel("Blush Opacity");
            blushOpacity.AddPanel(blushOpacityPanel);

            //MenuSliderItem blushOpacity = new UIMenuSliderItem("Blush Opacity", 0, 10, (int)(currentBlushOpacity * 10f), "Select a blush opacity.");
            UIMenuListItem blushColor = new UIMenuListItem("Blush Color", overlayColorsList, currentBlushColor, "Select a blush color.");
            UIMenuColorPanel blushColorPanel = new UIMenuColorPanel("Blush Color", ColorPanelType.Makeup);
            blushColor.AddPanel(blushColorPanel);


            UIMenuListItem complexionStyle = new UIMenuListItem("Complexion Style", complexionStyleList, currentComplexionStyle, "Select a complexion style.");
            //MenuSliderItem complexionOpacity = new UIMenuSliderItem("Complexion Opacity", 0, 10, (int)(currentComplexionOpacity * 10f), "Select a complexion opacity.");
            UIMenuListItem complexionOpacity = new UIMenuListItem("Complexion Opacity", opacity, (int)(currentComplexionOpacity * 10f), "Select a complexion opacity.");
            UIMenuPercentagePanel complexionOpacityPanel = new UIMenuPercentagePanel("Complexion Opacity");
            complexionOpacity.AddPanel(complexionOpacityPanel);


            UIMenuListItem sunDamageStyle = new UIMenuListItem("Sun Damage Style", sunDamageStyleList, currentSunDamageStyle, "Select a sun damage style.");
            //MenuSliderItem sunDamageOpacity = new UIMenuSliderItem("Sun Damage Opacity", 0, 10, (int)(currentSunDamageOpacity * 10f), "Select a sun damage opacity.");
            UIMenuListItem sunDamageOpacity = new UIMenuListItem("Sun Damage Opacity", opacity, (int)(currentSunDamageOpacity * 10f), "Select a sun damage opacity.");
            UIMenuPercentagePanel sunDamageOpacityPanel = new UIMenuPercentagePanel("Sun Damage ");
            sunDamageOpacity.AddPanel(sunDamageOpacityPanel);


            UIMenuListItem lipstickStyle = new UIMenuListItem("Lipstick Style", lipstickStyleList, currentLipstickStyle, "Select a lipstick style.");
            //MenuSliderItem lipstickOpacity = new UIMenuSliderItem("Lipstick Opacity", 0, 10, (int)(currentLipstickOpacity * 10f), "Select a lipstick opacity.");
            UIMenuListItem lipstickOpacity = new UIMenuListItem("Lipstick Opacity", opacity, (int)(currentLipstickOpacity * 10f), "Select a lipstick opacity.");
            UIMenuPercentagePanel lipstickOpacityPanel = new UIMenuPercentagePanel("Lipstick Opacity");
            lipstickOpacity.AddPanel(lipstickOpacityPanel);

            UIMenuListItem lipstickColor = new UIMenuListItem("Lipstick Color", overlayColorsList, currentLipstickColor, "Select a lipstick color.");
            UIMenuColorPanel lipstickColorPanel = new UIMenuColorPanel("Lipstick Color", ColorPanelType.Makeup);
            lipstickColor.AddPanel(lipstickColorPanel);


            UIMenuListItem molesFrecklesStyle = new UIMenuListItem("Moles and Freckles Style", molesFrecklesStyleList, currentMolesFrecklesStyle, "Select a moles and freckles style.");
            //MenuSliderItem molesFrecklesOpacity = new UIMenuSliderItem("Moles and Freckles Opacity", 0, 10, (int)(currentMolesFrecklesOpacity * 10f), "Select a moles and freckles opacity.");
            UIMenuListItem molesFrecklesOpacity = new UIMenuListItem("Moles and Freckles Opacity", opacity, (int)(currentMolesFrecklesOpacity * 10f), "Select a moles and freckles opacity.");
            UIMenuPercentagePanel molesFrecklesOpacityPanel = new UIMenuPercentagePanel("Moles and ");
            molesFrecklesOpacity.AddPanel(molesFrecklesOpacityPanel);


            UIMenuListItem chestHairStyle = new UIMenuListItem("Chest Hair Style", chestHairStyleList, currentChesthairStyle, "Select a chest hair style.");
            //MenuSliderItem chestHairOpacity = new UIMenuSliderItem("Chest Hair Opacity", 0, 10, (int)(currentChesthairOpacity * 10f), "Select a chest hair opacity.");
            UIMenuListItem chestHairOpacity = new UIMenuListItem("Chest Hair Opacity", opacity, (int)(currentChesthairOpacity * 10f), "Select a chest hair opacity.");
            UIMenuPercentagePanel chestHairOpacityPanel = new UIMenuPercentagePanel("Chest Hair ");
            chestHairOpacity.AddPanel(chestHairOpacityPanel);

            UIMenuListItem chestHairColor = new UIMenuListItem("Chest Hair Color", overlayColorsList, currentChesthairColor, "Select a chest hair color.");
            UIMenuColorPanel chestHairColorPanel = new UIMenuColorPanel("Chest Hair ", ColorPanelType.Hair);
            chestHairColor.AddPanel(chestHairColorPanel);


            // Body blemishes
            UIMenuListItem bodyBlemishesStyle = new UIMenuListItem("Body Blemishes Style", bodyBlemishesList, currentBodyBlemishesStyle, "Select body blemishes style.");
            UIMenuListItem bodyBlemishesOpacity = new UIMenuListItem("Body Blemishes Opacity", opacity, (int)(currentBodyBlemishesOpacity * 10f), "Select body blemishes opacity.");
            UIMenuPercentagePanel bodyBlemishesOpacityPanel = new UIMenuPercentagePanel("Body Blemishes ");
            bodyBlemishesOpacity.AddPanel(bodyBlemishesOpacityPanel);


            UIMenuListItem eyeColor = new UIMenuListItem("Eye Colors", eyeColorList, currentEyeColor, "Select an eye/contact lens color.");

            appearanceMenu.AddItem(hairStyles);
            appearanceMenu.AddItem(hairColors);
            appearanceMenu.AddItem(hairHighlightColors);

            appearanceMenu.AddItem(blemishesStyle);
            appearanceMenu.AddItem(blemishesOpacity);

            appearanceMenu.AddItem(beardStyles);
            appearanceMenu.AddItem(beardOpacity);
            appearanceMenu.AddItem(beardColor);

            appearanceMenu.AddItem(eyebrowStyle);
            appearanceMenu.AddItem(eyebrowOpacity);
            appearanceMenu.AddItem(eyebrowColor);

            appearanceMenu.AddItem(ageingStyle);
            appearanceMenu.AddItem(ageingOpacity);

            appearanceMenu.AddItem(makeupStyle);
            appearanceMenu.AddItem(makeupOpacity);
            appearanceMenu.AddItem(makeupColor);

            appearanceMenu.AddItem(blushStyle);
            appearanceMenu.AddItem(blushOpacity);
            appearanceMenu.AddItem(blushColor);

            appearanceMenu.AddItem(complexionStyle);
            appearanceMenu.AddItem(complexionOpacity);

            appearanceMenu.AddItem(sunDamageStyle);
            appearanceMenu.AddItem(sunDamageOpacity);

            appearanceMenu.AddItem(lipstickStyle);
            appearanceMenu.AddItem(lipstickOpacity);
            appearanceMenu.AddItem(lipstickColor);

            appearanceMenu.AddItem(molesFrecklesStyle);
            appearanceMenu.AddItem(molesFrecklesOpacity);

            appearanceMenu.AddItem(chestHairStyle);
            appearanceMenu.AddItem(chestHairOpacity);
            appearanceMenu.AddItem(chestHairColor);

            appearanceMenu.AddItem(bodyBlemishesStyle);
            appearanceMenu.AddItem(bodyBlemishesOpacity);

            appearanceMenu.AddItem(eyeColor);

            if (male)
            {
                // There are weird people out there that wanted makeup for male characters
                // so yeah.... here you go I suppose... strange...

                /*
                makeupStyle.Enabled = false;
                makeupStyle.SetLeftBadge(BadgeIcon.LOCK);
                makeupStyle.Description = "This is not available for male characters.";

                makeupOpacity.Enabled = false;
                makeupOpacity.SetLeftBadge(BadgeIcon.LOCK);
                makeupOpacity.Description = "This is not available for male characters.";

                makeupColor.Enabled = false;
                makeupColor.SetLeftBadge(BadgeIcon.LOCK);
                makeupColor.Description = "This is not available for male characters.";


                blushStyle.Enabled = false;
                blushStyle.SetLeftBadge(BadgeIcon.LOCK);
                blushStyle.Description = "This is not available for male characters.";

                blushOpacity.Enabled = false;
                blushOpacity.SetLeftBadge(BadgeIcon.LOCK);
                blushOpacity.Description = "This is not available for male characters.";

                blushColor.Enabled = false;
                blushColor.SetLeftBadge(BadgeIcon.LOCK);
                blushColor.Description = "This is not available for male characters.";


                lipstickStyle.Enabled = false;
                lipstickStyle.SetLeftBadge(BadgeIcon.LOCK);
                lipstickStyle.Description = "This is not available for male characters.";

                lipstickOpacity.Enabled = false;
                lipstickOpacity.SetLeftBadge(BadgeIcon.LOCK);
                lipstickOpacity.Description = "This is not available for male characters.";

                lipstickColor.Enabled = false;
                lipstickColor.SetLeftBadge(BadgeIcon.LOCK);
                lipstickColor.Description = "This is not available for male characters.";
                */
            }
            else
            {
                beardStyles.Enabled = false;
                beardStyles.SetLeftBadge(BadgeIcon.LOCK);
                beardStyles.Description = "This is not available for female characters.";

                beardOpacity.Enabled = false;
                beardOpacity.SetLeftBadge(BadgeIcon.LOCK);
                beardOpacity.Description = "This is not available for female characters.";

                beardColor.Enabled = false;
                beardColor.SetLeftBadge(BadgeIcon.LOCK);
                beardColor.Description = "This is not available for female characters.";


                chestHairStyle.Enabled = false;
                chestHairStyle.SetLeftBadge(BadgeIcon.LOCK);
                chestHairStyle.Description = "This is not available for female characters.";

                chestHairOpacity.Enabled = false;
                chestHairOpacity.SetLeftBadge(BadgeIcon.LOCK);
                chestHairOpacity.Description = "This is not available for female characters.";

                chestHairColor.Enabled = false;
                chestHairColor.SetLeftBadge(BadgeIcon.LOCK);
                chestHairColor.Description = "This is not available for female characters.";
            }

            #endregion

            #region clothing options menu
            string[] clothingCategoryNames = new string[12] { "Unused (head)", "Masks", "Unused (hair)", "Upper Body", "Lower Body", "Bags & Parachutes", "Shoes", "Scarfs & Chains", "Shirt & Accessory", "Body Armor & Accessory 2", "Badges & Logos", "Shirt Overlay & Jackets" };
            for (int i = 0; i < 12; i++)
            {
                if (i is not 0 and not 2)
                {
                    int currentVariationIndex = editPed && currentCharacter.DrawableVariations.clothes.ContainsKey(i) ? currentCharacter.DrawableVariations.clothes[i].Key : GetPedDrawableVariation(Game.PlayerPed.Handle, i);
                    int currentVariationTextureIndex = editPed && currentCharacter.DrawableVariations.clothes.ContainsKey(i) ? currentCharacter.DrawableVariations.clothes[i].Value : GetPedTextureVariation(Game.PlayerPed.Handle, i);

                    int maxDrawables = GetNumberOfPedDrawableVariations(Game.PlayerPed.Handle, i);

                    List<dynamic> items = new List<dynamic>();
                    for (int x = 0; x < maxDrawables; x++)
                    {
                        items.Add($"Drawable #{x} (of {maxDrawables})");
                    }

                    int maxTextures = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, i, currentVariationIndex);

                    UIMenuListItem listItem = new UIMenuListItem(clothingCategoryNames[i], items, currentVariationIndex, $"Select a drawable using the arrow keys and press ~o~enter~s~ to cycle through all available textures. Currently selected texture: #{currentVariationTextureIndex + 1} (of {maxTextures}).");
                    clothesMenu.AddItem(listItem);
                }
            }
            #endregion

            #region props options menu
            string[] propNames = new string[5] { "Hats & Helmets", "Glasses", "Misc Props", "Watches", "Bracelets" };
            for (int x = 0; x < 5; x++)
            {
                int propId = x;
                if (x > 2)
                {
                    propId += 3;
                }

                int currentProp = editPed && currentCharacter.PropVariations.props.ContainsKey(propId) ? currentCharacter.PropVariations.props[propId].Key : GetPedPropIndex(Game.PlayerPed.Handle, propId);
                int currentPropTexture = editPed && currentCharacter.PropVariations.props.ContainsKey(propId) ? currentCharacter.PropVariations.props[propId].Value : GetPedPropTextureIndex(Game.PlayerPed.Handle, propId);

                List<dynamic> propsList = new List<dynamic>();
                for (int i = 0; i < GetNumberOfPedPropDrawableVariations(Game.PlayerPed.Handle, propId); i++)
                {
                    propsList.Add($"Prop #{i} (of {GetNumberOfPedPropDrawableVariations(Game.PlayerPed.Handle, propId)})");
                }
                propsList.Add("No Prop");


                if (GetPedPropIndex(Game.PlayerPed.Handle, propId) != -1)
                {
                    int maxPropTextures = GetNumberOfPedPropTextureVariations(Game.PlayerPed.Handle, propId, currentProp);
                    UIMenuListItem propListItem = new UIMenuListItem($"{propNames[x]}", propsList, currentProp, $"Select a prop using the arrow keys and press ~o~enter~s~ to cycle through all available textures. Currently selected texture: #{currentPropTexture + 1} (of {maxPropTextures}).");
                    propsMenu.AddItem(propListItem);
                }
                else
                {
                    UIMenuListItem propListItem = new UIMenuListItem($"{propNames[x]}", propsList, currentProp, "Select a prop using the arrow keys and press ~o~enter~s~ to cycle through all available textures.");
                    propsMenu.AddItem(propListItem);
                }


            }
            #endregion

            #region face features menu
            foreach (UIMenuSliderItem item in faceShapeMenu.MenuItems)
            {
                if (editPed)
                {
                    if (currentCharacter.FaceShapeFeatures.features == null)
                    {
                        currentCharacter.FaceShapeFeatures.features = new Dictionary<int, float>();
                    }
                    else
                    {
                        if (currentCharacter.FaceShapeFeatures.features.ContainsKey(faceShapeMenu.MenuItems.IndexOf(item)))
                        {
                            item.Value = (int)(currentCharacter.FaceShapeFeatures.features[faceShapeMenu.MenuItems.IndexOf(item)] * 10f) + 10;
                            SetPedFaceFeature(Game.PlayerPed.Handle, faceShapeMenu.MenuItems.IndexOf(item), currentCharacter.FaceShapeFeatures.features[faceShapeMenu.MenuItems.IndexOf(item)]);
                        }
                        else
                        {
                            item.Value = 10;
                            SetPedFaceFeature(Game.PlayerPed.Handle, faceShapeMenu.MenuItems.IndexOf(item), 0f);
                        }
                    }
                }
                else
                {
                    item.Value = 10;
                    SetPedFaceFeature(Game.PlayerPed.Handle, faceShapeMenu.MenuItems.IndexOf(item), 0f);
                }
            }
            #endregion

            #region Tattoos menu
            List<dynamic> headTattoosList = new List<dynamic>();
            List<dynamic> torsoTattoosList = new List<dynamic>();
            List<dynamic> leftArmTattoosList = new List<dynamic>();
            List<dynamic> rightArmTattoosList = new List<dynamic>();
            List<dynamic> leftLegTattoosList = new List<dynamic>();
            List<dynamic> rightLegTattoosList = new List<dynamic>();
            List<dynamic> badgeTattoosList = new List<dynamic>();

            TattoosData.GenerateTattoosData();
            if (male)
            {
                int counter = 1;
                foreach (Tattoo tattoo in MaleTattoosCollection.HEAD)
                {
                    headTattoosList.Add($"Tattoo #{counter} (of {MaleTattoosCollection.HEAD.Count})");
                    counter++;
                }
                counter = 1;
                foreach (Tattoo tattoo in MaleTattoosCollection.TORSO)
                {
                    torsoTattoosList.Add($"Tattoo #{counter} (of {MaleTattoosCollection.TORSO.Count})");
                    counter++;
                }
                counter = 1;
                foreach (Tattoo tattoo in MaleTattoosCollection.LEFT_ARM)
                {
                    leftArmTattoosList.Add($"Tattoo #{counter} (of {MaleTattoosCollection.LEFT_ARM.Count})");
                    counter++;
                }
                counter = 1;
                foreach (Tattoo tattoo in MaleTattoosCollection.RIGHT_ARM)
                {
                    rightArmTattoosList.Add($"Tattoo #{counter} (of {MaleTattoosCollection.RIGHT_ARM.Count})");
                    counter++;
                }
                counter = 1;
                foreach (Tattoo tattoo in MaleTattoosCollection.LEFT_LEG)
                {
                    leftLegTattoosList.Add($"Tattoo #{counter} (of {MaleTattoosCollection.LEFT_LEG.Count})");
                    counter++;
                }
                counter = 1;
                foreach (Tattoo tattoo in MaleTattoosCollection.RIGHT_LEG)
                {
                    rightLegTattoosList.Add($"Tattoo #{counter} (of {MaleTattoosCollection.RIGHT_LEG.Count})");
                    counter++;
                }
                counter = 1;
                foreach (Tattoo tattoo in MaleTattoosCollection.BADGES)
                {
                    badgeTattoosList.Add($"Badge #{counter} (of {MaleTattoosCollection.BADGES.Count})");
                    counter++;
                }
            }
            else
            {
                int counter = 1;
                foreach (Tattoo tattoo in FemaleTattoosCollection.HEAD)
                {
                    headTattoosList.Add($"Tattoo #{counter} (of {FemaleTattoosCollection.HEAD.Count})");
                    counter++;
                }
                counter = 1;
                foreach (Tattoo tattoo in FemaleTattoosCollection.TORSO)
                {
                    torsoTattoosList.Add($"Tattoo #{counter} (of {FemaleTattoosCollection.TORSO.Count})");
                    counter++;
                }
                counter = 1;
                foreach (Tattoo tattoo in FemaleTattoosCollection.LEFT_ARM)
                {
                    leftArmTattoosList.Add($"Tattoo #{counter} (of {FemaleTattoosCollection.LEFT_ARM.Count})");
                    counter++;
                }
                counter = 1;
                foreach (Tattoo tattoo in FemaleTattoosCollection.RIGHT_ARM)
                {
                    rightArmTattoosList.Add($"Tattoo #{counter} (of {FemaleTattoosCollection.RIGHT_ARM.Count})");
                    counter++;
                }
                counter = 1;
                foreach (Tattoo tattoo in FemaleTattoosCollection.LEFT_LEG)
                {
                    leftLegTattoosList.Add($"Tattoo #{counter} (of {FemaleTattoosCollection.LEFT_LEG.Count})");
                    counter++;
                }
                counter = 1;
                foreach (Tattoo tattoo in FemaleTattoosCollection.RIGHT_LEG)
                {
                    rightLegTattoosList.Add($"Tattoo #{counter} (of {FemaleTattoosCollection.RIGHT_LEG.Count})");
                    counter++;
                }
                counter = 1;
                foreach (Tattoo tattoo in FemaleTattoosCollection.BADGES)
                {
                    badgeTattoosList.Add($"Badge #{counter} (of {FemaleTattoosCollection.BADGES.Count})");
                    counter++;
                }
            }

            const string tatDesc = "Cycle through the list to preview tattoos. If you like one, press enter to select it, selecting it will add the tattoo if you don't already have it. If you already have that tattoo then the tattoo will be removed.";
            UIMenuListItem headTatts = new UIMenuListItem("Head Tattoos", headTattoosList, 0, tatDesc);
            UIMenuListItem torsoTatts = new UIMenuListItem("Torso Tattoos", torsoTattoosList, 0, tatDesc);
            UIMenuListItem leftArmTatts = new UIMenuListItem("Left Arm Tattoos", leftArmTattoosList, 0, tatDesc);
            UIMenuListItem rightArmTatts = new UIMenuListItem("Right Arm Tattoos", rightArmTattoosList, 0, tatDesc);
            UIMenuListItem leftLegTatts = new UIMenuListItem("Left Leg Tattoos", leftLegTattoosList, 0, tatDesc);
            UIMenuListItem rightLegTatts = new UIMenuListItem("Right Leg Tattoos", rightLegTattoosList, 0, tatDesc);
            UIMenuListItem badgeTatts = new UIMenuListItem("Badge Overlays", badgeTattoosList, 0, tatDesc);

            tattoosMenu.AddItem(headTatts);
            tattoosMenu.AddItem(torsoTatts);
            tattoosMenu.AddItem(leftArmTatts);
            tattoosMenu.AddItem(rightArmTatts);
            tattoosMenu.AddItem(leftLegTatts);
            tattoosMenu.AddItem(rightLegTatts);
            tattoosMenu.AddItem(badgeTatts);
            tattoosMenu.AddItem(new UIMenuItem("Remove All Tattoos", "Click this if you want to remove all tattoos and start over."));
            #endregion
        }

        /// <summary>
        /// Saves the mp character and quits the editor if successful.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SavePed()
        {
            currentCharacter.PedHeadBlendData = Game.PlayerPed.GetHeadBlendData();
            if (isEdidtingPed)
            {
                string json = JsonConvert.SerializeObject(currentCharacter);
                if (StorageManager.SaveJsonData(currentCharacter.SaveName, json, true))
                {
                    Notify.Success("Your character was saved successfully.");
                    return true;
                }
                else
                {
                    Notify.Error("Your character could not be saved. Reason unknown. :(");
                    return false;
                }
            }
            else
            {
                string name = await GetUserInput(windowTitle: "Enter a save name.", maxInputLength: 30);
                if (string.IsNullOrEmpty(name))
                {
                    Notify.Error(CommonErrors.InvalidInput);
                    return false;
                }
                else
                {
                    currentCharacter.SaveName = "mp_ped_" + name;
                    string json = JsonConvert.SerializeObject(currentCharacter);

                    if (StorageManager.SaveJsonData("mp_ped_" + name, json, false))
                    {
                        Notify.Success($"Your character (~g~<C>{name}</C>~s~) has been saved.");
                        Log($"Saved Character {name}. Data: {json}");
                        return true;
                    }
                    else
                    {
                        Notify.Error($"Saving failed, most likely because this name (~y~<C>{name}</C>~s~) is already in use.");
                        return false;
                    }
                }
            }

        }

        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            // Create the menu.
            menu = new UIMenu(Game.Player.Name, "MP Ped Customization");

            UIMenuItem savedCharacters = new UIMenuItem("Saved Characters", "Spawn, edit or delete your existing saved multiplayer characters.");
            savedCharacters.SetRightLabel("→→→");

            CreateSavedPedsMenu();

            menu.AddItem(createMaleBtn);
            createMaleBtn.Activated += async (sender, args) => await sender.SwitchTo(createCharacterMenu, 0, true);
            menu.AddItem(createFemaleBtn);
            createFemaleBtn.Activated += async (sender, args) => await sender.SwitchTo(createCharacterMenu, 0, true);
            menu.AddItem(savedCharacters);
            savedCharacters.Activated += async (sender, args) => await sender.SwitchTo(savedCharactersMenu, 0, true);

            createCharacterMenu.InstructionalButtons.Add(new(Control.MoveLeftRight, "Turn Head"));
            inheritanceMenu.InstructionalButtons.Add(new(Control.MoveLeftRight, "Turn Head"));
            appearanceMenu.InstructionalButtons.Add(new(Control.MoveLeftRight, "Turn Head"));
            faceShapeMenu.InstructionalButtons.Add(new(Control.MoveLeftRight, "Turn Head"));
            tattoosMenu.InstructionalButtons.Add(new(Control.MoveLeftRight, "Turn Head"));
            clothesMenu.InstructionalButtons.Add(new(Control.MoveLeftRight, "Turn Head"));
            propsMenu.InstructionalButtons.Add(new(Control.MoveLeftRight, "Turn Head"));

            createCharacterMenu.InstructionalButtons.Add(new(Control.PhoneExtraOption, "Turn Character"));
            inheritanceMenu.InstructionalButtons.Add(new(Control.PhoneExtraOption, "Turn Character"));
            appearanceMenu.InstructionalButtons.Add(new(Control.PhoneExtraOption, "Turn Character"));
            faceShapeMenu.InstructionalButtons.Add(new(Control.PhoneExtraOption, "Turn Character"));
            tattoosMenu.InstructionalButtons.Add(new(Control.PhoneExtraOption, "Turn Character"));
            clothesMenu.InstructionalButtons.Add(new(Control.PhoneExtraOption, "Turn Character"));
            propsMenu.InstructionalButtons.Add(new(Control.PhoneExtraOption, "Turn Character"));

            createCharacterMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeRight, "Turn Camera Right"));
            inheritanceMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeRight, "Turn Camera Right"));
            appearanceMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeRight, "Turn Camera Right"));
            faceShapeMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeRight, "Turn Camera Right"));
            tattoosMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeRight, "Turn Camera Right"));
            clothesMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeRight, "Turn Camera Right"));
            propsMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeRight, "Turn Camera Right"));

            createCharacterMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeLeft, "Turn Camera Left"));
            inheritanceMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeLeft, "Turn Camera Left"));
            appearanceMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeLeft, "Turn Camera Left"));
            faceShapeMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeLeft, "Turn Camera Left"));
            tattoosMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeLeft, "Turn Camera Left"));
            clothesMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeLeft, "Turn Camera Left"));
            propsMenu.InstructionalButtons.Add(new(Control.ParachuteBrakeLeft, "Turn Camera Left"));


            UIMenuItem inheritanceButton = new UIMenuItem("Character Inheritance", "Character inheritance options.");
            UIMenuItem appearanceButton = new UIMenuItem("Character Appearance", "Character appearance options.");
            UIMenuItem faceButton = new UIMenuItem("Character Face Shape Options", "Character face shape options.");
            UIMenuItem tattoosButton = new UIMenuItem("Character Tattoo Options", "Character tattoo options.");
            UIMenuItem clothesButton = new UIMenuItem("Character Clothes", "Character clothes.");
            UIMenuItem propsButton = new UIMenuItem("Character Props", "Character props.");
            UIMenuItem saveButton = new UIMenuItem("Save Character", "Save your character.");
            UIMenuItem exitNoSave = new UIMenuItem("Exit Without Saving", "Are you sure? All unsaved work will be lost.");
            UIMenuListItem faceExpressionList = new UIMenuListItem("Facial Expression", new List<dynamic> { "Normal", "Happy", "Angry", "Aiming", "Injured", "Stressed", "Smug", "Sulk" }, 0, "Set a facial expression that will be used whenever your ped is idling.");

            inheritanceButton.SetRightLabel("→→→");
            appearanceButton.SetRightLabel("→→→");
            faceButton.SetRightLabel("→→→");
            tattoosButton.SetRightLabel("→→→");
            clothesButton.SetRightLabel("→→→");
            propsButton.SetRightLabel("→→→");

            createCharacterMenu.AddItem(inheritanceButton);
            createCharacterMenu.AddItem(appearanceButton);
            createCharacterMenu.AddItem(faceButton);
            createCharacterMenu.AddItem(tattoosButton);
            createCharacterMenu.AddItem(clothesButton);
            createCharacterMenu.AddItem(propsButton);
            createCharacterMenu.AddItem(faceExpressionList);
            createCharacterMenu.AddItem(saveButton);
            createCharacterMenu.AddItem(exitNoSave);

            inheritanceButton.Activated += async (sender, args) => await sender.SwitchTo(inheritanceMenu, 0, true);
            appearanceButton.Activated += async (sender, args) => await sender.SwitchTo(appearanceMenu, 0, true);
            faceButton.Activated += async (sender, args) => await sender.SwitchTo(faceShapeMenu, 0, true);
            tattoosButton.Activated += async (sender, args) => await sender.SwitchTo(tattoosMenu, 0, true);
            clothesButton.Activated += async (sender, args) => await sender.SwitchTo(clothesMenu, 0, true);
            propsButton.Activated += async (sender, args) => await sender.SwitchTo(propsMenu, 0, true);

            #region inheritance
            Dictionary<string, int> dads = new Dictionary<string, int>();
            Dictionary<string, int> moms = new Dictionary<string, int>();

            void AddInheritance(Dictionary<string, int> dict, int listId, string textPrefix)
            {
                int baseIdx = dict.Count;
                int basePed = GetPedHeadBlendFirstIndex(listId);

                // list 0/2 are male, list 1/3 are female
                string suffix = $" ({(listId % 2 == 0 ? "Male" : "Female")})";

                for (int i = 0; i < GetNumParentPedsOfType(listId); i++)
                {
                    // get the actual parent name, or the index if none
                    string label = GetLabelText($"{textPrefix}{i}");
                    if (string.IsNullOrWhiteSpace(label) || label == "NULL")
                    {
                        label = $"{baseIdx + i}";
                    }

                    // append the gender of the list
                    label += suffix;
                    dict[label] = basePed + i;
                }
            }

            int GetInheritance(Dictionary<string, int> list, UIMenuListItem listItem)
            {
                if (listItem.Index < listItem.Items.Count)
                {
                    if (list.TryGetValue((string)listItem.Items[listItem.Index], out int idx))
                    {
                        return idx;
                    }
                }

                return 0;
            }

            int listIdx = 0;
            foreach (Dictionary<string, int> list in new[] { dads, moms })
            {
                void AddDads()
                {
                    AddInheritance(list, 0, "Male_");
                    AddInheritance(list, 2, "Special_Male_");
                }

                void AddMoms()
                {
                    AddInheritance(list, 1, "Female_");
                    AddInheritance(list, 3, "Special_Female_");
                }

                if (listIdx == 0)
                {
                    AddDads();
                    AddMoms();
                }
                else
                {
                    AddMoms();
                    AddDads();
                }

                listIdx++;
            }

            UIMenuListItem inheritanceDads = new UIMenuListItem("Father", dads.Keys.Cast<dynamic>().ToList(), 0, "Select a father.");
            UIMenuListItem inheritanceMoms = new UIMenuListItem("Mother", moms.Keys.Cast<dynamic>().ToList(), 0, "Select a mother.");
            List<float> mixValues = new List<float>() { 0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f };
            UIMenuSliderItem inheritanceShapeMix = new UIMenuSliderItem("Head Shape Mix", "Select how much of your head shape should be inherited from your father or mother. All the way on the left is your dad, all the way on the right is your mom.", 0, 10, 5, true);
            UIMenuSliderItem inheritanceSkinMix = new UIMenuSliderItem("Body Skin Mix", "Select how much of your body skin tone should be inherited from your father or mother. All the way on the left is your dad, all the way on the right is your mom.", 0, 10, 5, true);

            inheritanceMenu.AddItem(inheritanceDads);
            inheritanceMenu.AddItem(inheritanceMoms);
            inheritanceMenu.AddItem(inheritanceShapeMix);
            inheritanceMenu.AddItem(inheritanceSkinMix);

            // formula from maintransition.#sc
            float GetMinimum()
            {
                return currentCharacter.IsMale ? 0.05f : 0.3f;
            }

            float GetMaximum()
            {
                return currentCharacter.IsMale ? 0.7f : 0.95f;
            }

            float ClampMix(int value)
            {
                float sliderFraction = mixValues[value];
                float min = GetMinimum();
                float max = GetMaximum();

                return min + (sliderFraction * (max - min));
            }

            int UnclampMix(float value)
            {
                float min = GetMinimum();
                float max = GetMaximum();

                float origFraction = (value - min) / (max - min);
                return Math.Max(Math.Min((int)(origFraction * 10), 10), 0);
            }

            void SetHeadBlend()
            {
                SetPedHeadBlendData(Game.PlayerPed.Handle, GetInheritance(dads, inheritanceDads), GetInheritance(moms, inheritanceMoms), 0, GetInheritance(dads, inheritanceDads), GetInheritance(moms, inheritanceMoms), 0, ClampMix(inheritanceShapeMix.Value), ClampMix(inheritanceSkinMix.Value), 0f, true);
            }

            inheritanceMenu.OnListChange += (_menu, listItem, itemIndex) =>
            {
                SetHeadBlend();
            };

            inheritanceMenu.OnSliderChange += (sender, item, itemIndex) =>
            {
                SetHeadBlend();
            };
            #endregion

            #region appearance
            Dictionary<int, KeyValuePair<string, string>> hairOverlays = new Dictionary<int, KeyValuePair<string, string>>()
            {
                { 0, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_a") },
                { 1, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_z") },
                { 2, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_z") },
                { 3, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_003_a") },
                { 4, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_z") },
                { 5, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_z") },
                { 6, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_z") },
                { 7, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_z") },
                { 8, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_008_a") },
                { 9, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_z") },
                { 10, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_z") },
                { 11, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_z") },
                { 12, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_z") },
                { 13, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_z") },
                { 14, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_long_a") },
                { 15, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_long_a") },
                { 16, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_z") },
                { 17, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_a") },
                { 18, new KeyValuePair<string, string>("mpbusiness_overlays", "FM_Bus_M_Hair_000_a") },
                { 19, new KeyValuePair<string, string>("mpbusiness_overlays", "FM_Bus_M_Hair_001_a") },
                { 20, new KeyValuePair<string, string>("mphipster_overlays", "FM_Hip_M_Hair_000_a") },
                { 21, new KeyValuePair<string, string>("mphipster_overlays", "FM_Hip_M_Hair_001_a") },
                { 22, new KeyValuePair<string, string>("multiplayer_overlays", "FM_M_Hair_001_a") },
            };

            // manage the list changes for appearance items.
            appearanceMenu.OnListChange += (_menu, listItem, itemIndex) =>
            {
                if (_menu.MenuItems.IndexOf(listItem) == 0) // hair style
                {
                    ClearPedFacialDecorations(Game.PlayerPed.Handle);
                    currentCharacter.PedAppearance.HairOverlay = new KeyValuePair<string, string>("", "");

                    if (itemIndex >= GetNumberOfPedDrawableVariations(Game.PlayerPed.Handle, 2))
                    {
                        SetPedComponentVariation(Game.PlayerPed.Handle, 2, 0, 0, 0);
                        currentCharacter.PedAppearance.hairStyle = 0;
                    }
                    else
                    {
                        SetPedComponentVariation(Game.PlayerPed.Handle, 2, itemIndex, 0, 0);
                        currentCharacter.PedAppearance.hairStyle = itemIndex;
                        if (hairOverlays.ContainsKey(itemIndex))
                        {
                            SetPedFacialDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(hairOverlays[itemIndex].Key), (uint)GetHashKey(hairOverlays[itemIndex].Value));
                            currentCharacter.PedAppearance.HairOverlay = new KeyValuePair<string, string>(hairOverlays[itemIndex].Key, hairOverlays[itemIndex].Value);
                        }
                    }
                }
                else if (_menu.MenuItems.IndexOf(listItem) == 33) // eye color
                {
                    int selection = itemIndex;
                    SetPedEyeColor(Game.PlayerPed.Handle, selection);
                    currentCharacter.PedAppearance.eyeColor = selection;
                }
            };
            appearanceMenu.OnColorPanelChange += (_item, panel, index) =>
            {
                UIMenu _menu = _item.Parent;
                int itemIndex = _item.Parent.MenuItems.IndexOf(_item);
                if (itemIndex is 1 or 2) // hair colors
                {
                    int hairColor = ((UIMenuColorPanel)_menu.MenuItems[1].Panels[0]).CurrentSelection;
                    int hairHighlightColor = ((UIMenuColorPanel)_menu.MenuItems[1].Panels[1]).CurrentSelection;

                    SetPedHairColor(Game.PlayerPed.Handle, hairColor, hairHighlightColor);

                    currentCharacter.PedAppearance.hairColor = hairColor;
                    currentCharacter.PedAppearance.hairHighlightColor = hairHighlightColor;
                }
                else
                {
                    int selection = ((UIMenuListItem)_item).Index;
                    float opacity = 0f;
                    if (_menu.MenuItems[itemIndex + 1] is UIMenuListItem item2)
                    {
                        opacity = (((float)item2.Index + 1) / 10f) - 0.1f;
                    }
                    else if (_menu.MenuItems[itemIndex - 1] is UIMenuListItem item1)
                    {
                        opacity = (((float)item1.Index + 1) / 10f) - 0.1f;
                    }
                    else if (_menu.MenuItems[itemIndex] is UIMenuListItem item)
                    {
                        opacity = (((float)item.Index + 1) / 10f) - 0.1f;
                    }
                    else
                    {
                        opacity = 1f;
                    }

                    switch (itemIndex)
                    {
                        case 3: // blemishes
                            SetPedHeadOverlay(Game.PlayerPed.Handle, 0, selection, opacity);
                            currentCharacter.PedAppearance.blemishesStyle = selection;
                            currentCharacter.PedAppearance.blemishesOpacity = opacity;
                            break;
                        case 5: // beards
                            SetPedHeadOverlay(Game.PlayerPed.Handle, 1, selection, opacity);
                            currentCharacter.PedAppearance.beardStyle = selection;
                            currentCharacter.PedAppearance.beardOpacity = opacity;
                            break;
                        case 7: // beards color
                            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, selection, selection);
                            currentCharacter.PedAppearance.beardColor = selection;
                            break;
                        case 8: // eyebrows
                            SetPedHeadOverlay(Game.PlayerPed.Handle, 2, selection, opacity);
                            currentCharacter.PedAppearance.eyebrowsStyle = selection;
                            currentCharacter.PedAppearance.eyebrowsOpacity = opacity;
                            break;
                        case 10: // eyebrows color
                            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 2, 1, selection, selection);
                            currentCharacter.PedAppearance.eyebrowsColor = selection;
                            break;
                        case 11: // ageing
                            SetPedHeadOverlay(Game.PlayerPed.Handle, 3, selection, opacity);
                            currentCharacter.PedAppearance.ageingStyle = selection;
                            currentCharacter.PedAppearance.ageingOpacity = opacity;
                            break;
                        case 13: // makeup
                            SetPedHeadOverlay(Game.PlayerPed.Handle, 4, selection, opacity);
                            currentCharacter.PedAppearance.makeupStyle = selection;
                            currentCharacter.PedAppearance.makeupOpacity = opacity;
                            break;
                        case 15: // makeup color
                            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 4, 2, selection, selection);
                            currentCharacter.PedAppearance.makeupColor = selection;
                            break;
                        case 16: // blush style
                            SetPedHeadOverlay(Game.PlayerPed.Handle, 5, selection, opacity);
                            currentCharacter.PedAppearance.blushStyle = selection;
                            currentCharacter.PedAppearance.blushOpacity = opacity;
                            break;
                        case 18: // blush color
                            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 5, 2, selection, selection);
                            currentCharacter.PedAppearance.blushColor = selection;
                            break;
                        case 19: // complexion
                            SetPedHeadOverlay(Game.PlayerPed.Handle, 6, selection, opacity);
                            currentCharacter.PedAppearance.complexionStyle = selection;
                            currentCharacter.PedAppearance.complexionOpacity = opacity;
                            break;
                        case 21: // sun damage
                            SetPedHeadOverlay(Game.PlayerPed.Handle, 7, selection, opacity);
                            currentCharacter.PedAppearance.sunDamageStyle = selection;
                            currentCharacter.PedAppearance.sunDamageOpacity = opacity;
                            break;
                        case 23: // lipstick
                            SetPedHeadOverlay(Game.PlayerPed.Handle, 8, selection, opacity);
                            currentCharacter.PedAppearance.lipstickStyle = selection;
                            currentCharacter.PedAppearance.lipstickOpacity = opacity;
                            break;
                        case 25: // lipstick color
                            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 8, 2, selection, selection);
                            currentCharacter.PedAppearance.lipstickColor = selection;
                            break;
                        case 26: // moles and freckles
                            SetPedHeadOverlay(Game.PlayerPed.Handle, 9, selection, opacity);
                            currentCharacter.PedAppearance.molesFrecklesStyle = selection;
                            currentCharacter.PedAppearance.molesFrecklesOpacity = opacity;
                            break;
                        case 28: // chest hair
                            SetPedHeadOverlay(Game.PlayerPed.Handle, 10, selection, opacity);
                            currentCharacter.PedAppearance.chestHairStyle = selection;
                            currentCharacter.PedAppearance.chestHairOpacity = opacity;
                            break;
                        case 30: // chest hair color
                            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 10, 1, selection, selection);
                            currentCharacter.PedAppearance.chestHairColor = selection;
                            break;
                        case 31: // body blemishes
                            SetPedHeadOverlay(Game.PlayerPed.Handle, 11, selection, opacity);
                            currentCharacter.PedAppearance.bodyBlemishesStyle = selection;
                            currentCharacter.PedAppearance.bodyBlemishesOpacity = opacity;
                            break;
                    }
                }
            };

            // manage the slider changes for opacity on the appearance items.
            appearanceMenu.OnListChange += (_menu, listItem, itemIndex) =>
                {
                    if (itemIndex is > 2 and < 33)
                    {

                        int selection = listItem.Index;
                        float opacity = 0f;
                        if (_menu.MenuItems[itemIndex] is UIMenuListItem item2)
                        {
                            opacity = (((float)item2.Index + 1) / 10f) - 0.1f;
                        }
                        else if (_menu.MenuItems[itemIndex + 1] is UIMenuListItem item1)
                        {
                            opacity = (((float)item1.Index + 1) / 10f) - 0.1f;
                        }
                        else if (_menu.MenuItems[itemIndex - 1] is UIMenuListItem item)
                        {
                            opacity = (((float)item.Index + 1) / 10f) - 0.1f;
                        }
                        else
                        {
                            opacity = 1f;
                        }

                        switch (itemIndex)
                        {
                            case 4: // blemishes
                                SetPedHeadOverlay(Game.PlayerPed.Handle, 0, selection, opacity);
                                currentCharacter.PedAppearance.blemishesStyle = selection;
                                currentCharacter.PedAppearance.blemishesOpacity = opacity;
                                break;
                            case 6: // beards
                                SetPedHeadOverlay(Game.PlayerPed.Handle, 1, selection, opacity);
                                currentCharacter.PedAppearance.beardStyle = selection;
                                currentCharacter.PedAppearance.beardOpacity = opacity;
                                break;
                            case 9: // eyebrows
                                SetPedHeadOverlay(Game.PlayerPed.Handle, 2, selection, opacity);
                                currentCharacter.PedAppearance.eyebrowsStyle = selection;
                                currentCharacter.PedAppearance.eyebrowsOpacity = opacity;
                                break;
                            case 12: // ageing
                                SetPedHeadOverlay(Game.PlayerPed.Handle, 3, selection, opacity);
                                currentCharacter.PedAppearance.ageingStyle = selection;
                                currentCharacter.PedAppearance.ageingOpacity = opacity;
                                break;
                            case 14: // makeup
                                SetPedHeadOverlay(Game.PlayerPed.Handle, 4, selection, opacity);
                                currentCharacter.PedAppearance.makeupStyle = selection;
                                currentCharacter.PedAppearance.makeupOpacity = opacity;
                                break;
                            case 17: // blush style
                                SetPedHeadOverlay(Game.PlayerPed.Handle, 5, selection, opacity);
                                currentCharacter.PedAppearance.blushStyle = selection;
                                currentCharacter.PedAppearance.blushOpacity = opacity;
                                break;
                            case 20: // complexion
                                SetPedHeadOverlay(Game.PlayerPed.Handle, 6, selection, opacity);
                                currentCharacter.PedAppearance.complexionStyle = selection;
                                currentCharacter.PedAppearance.complexionOpacity = opacity;
                                break;
                            case 22: // sun damage
                                SetPedHeadOverlay(Game.PlayerPed.Handle, 7, selection, opacity);
                                currentCharacter.PedAppearance.sunDamageStyle = selection;
                                currentCharacter.PedAppearance.sunDamageOpacity = opacity;
                                break;
                            case 24: // lipstick
                                SetPedHeadOverlay(Game.PlayerPed.Handle, 8, selection, opacity);
                                currentCharacter.PedAppearance.lipstickStyle = selection;
                                currentCharacter.PedAppearance.lipstickOpacity = opacity;
                                break;
                            case 27: // moles and freckles
                                SetPedHeadOverlay(Game.PlayerPed.Handle, 9, selection, opacity);
                                currentCharacter.PedAppearance.molesFrecklesStyle = selection;
                                currentCharacter.PedAppearance.molesFrecklesOpacity = opacity;
                                break;
                            case 29: // chest hair
                                SetPedHeadOverlay(Game.PlayerPed.Handle, 10, selection, opacity);
                                currentCharacter.PedAppearance.chestHairStyle = selection;
                                currentCharacter.PedAppearance.chestHairOpacity = opacity;
                                break;
                            case 32: // body blemishes
                                SetPedHeadOverlay(Game.PlayerPed.Handle, 11, selection, opacity);
                                currentCharacter.PedAppearance.bodyBlemishesStyle = selection;
                                currentCharacter.PedAppearance.bodyBlemishesOpacity = opacity;
                                break;
                        }
                    }
                };
            #endregion

            #region clothes
            clothesMenu.OnListChange += (_menu, listItem, realIndex) =>
            {
                int componentIndex = realIndex + 1;
                if (realIndex > 0)
                {
                    componentIndex += 1;
                }

                int textureIndex = GetPedTextureVariation(Game.PlayerPed.Handle, componentIndex);
                int newTextureIndex = 0;
                SetPedComponentVariation(Game.PlayerPed.Handle, componentIndex, realIndex, newTextureIndex, 0);
                currentCharacter.DrawableVariations.clothes ??= new Dictionary<int, KeyValuePair<int, int>>();

                int maxTextures = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, componentIndex, realIndex);

                currentCharacter.DrawableVariations.clothes[componentIndex] = new KeyValuePair<int, int>(realIndex, newTextureIndex);
                listItem.Description = $"Select a drawable using the arrow keys and press ~o~enter~s~ to cycle through all available textures. Currently selected texture: #{newTextureIndex + 1} (of {maxTextures}).";
            };

            clothesMenu.OnListSelect += (sender, listItem, realIndex) =>
            {
                int componentIndex = realIndex + 1; // skip face options as that fucks up with inheritance faces
                if (realIndex > 0) // skip hair features as that is done in the appeareance menu
                {
                    componentIndex += 1;
                }

                int textureIndex = GetPedTextureVariation(Game.PlayerPed.Handle, componentIndex);
                int newTextureIndex = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, componentIndex, listItem.Index) - 1 < textureIndex + 1 ? 0 : textureIndex + 1;
                SetPedComponentVariation(Game.PlayerPed.Handle, componentIndex, listItem.Index, newTextureIndex, 0);
                currentCharacter.DrawableVariations.clothes ??= new Dictionary<int, KeyValuePair<int, int>>();

                int maxTextures = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, componentIndex, listItem.Index);

                currentCharacter.DrawableVariations.clothes[componentIndex] = new KeyValuePair<int, int>(listItem.Index, newTextureIndex);
                listItem.Description = $"Select a drawable using the arrow keys and press ~o~enter~s~ to cycle through all available textures. Currently selected texture: #{newTextureIndex + 1} (of {maxTextures}).";
            };
            #endregion

            #region props
            propsMenu.OnListChange += (_menu, listItem, realIndex) =>
            {
                int propIndex = realIndex;
                if (realIndex == 3)
                {
                    propIndex = 6;
                }
                if (realIndex == 4)
                {
                    propIndex = 7;
                }

                int textureIndex = 0;
                if (realIndex >= GetNumberOfPedPropDrawableVariations(Game.PlayerPed.Handle, propIndex))
                {
                    SetPedPropIndex(Game.PlayerPed.Handle, propIndex, -1, -1, false);
                    ClearPedProp(Game.PlayerPed.Handle, propIndex);
                    currentCharacter.PropVariations.props ??= new Dictionary<int, KeyValuePair<int, int>>();
                    currentCharacter.PropVariations.props[propIndex] = new KeyValuePair<int, int>(-1, -1);
                    listItem.Description = $"Select a prop using the arrow keys and press ~o~enter~s~ to cycle through all available textures.";
                }
                else
                {
                    SetPedPropIndex(Game.PlayerPed.Handle, propIndex, realIndex, textureIndex, true);
                    currentCharacter.PropVariations.props ??= new Dictionary<int, KeyValuePair<int, int>>();
                    currentCharacter.PropVariations.props[propIndex] = new KeyValuePair<int, int>(realIndex, textureIndex);
                    if (GetPedPropIndex(Game.PlayerPed.Handle, propIndex) == -1)
                    {
                        listItem.Description = $"Select a prop using the arrow keys and press ~o~enter~s~ to cycle through all available textures.";
                    }
                    else
                    {
                        int maxPropTextures = GetNumberOfPedPropTextureVariations(Game.PlayerPed.Handle, propIndex, realIndex);
                        listItem.Description = $"Select a prop using the arrow keys and press ~o~enter~s~ to cycle through all available textures. Currently selected texture: #{textureIndex + 1} (of {maxPropTextures}).";
                    }
                }
            };

            propsMenu.OnListSelect += (sender, listItem, index) =>
            {
                int propIndex = sender.MenuItems.IndexOf(listItem);
                if (sender.MenuItems.IndexOf(listItem) == 3)
                {
                    propIndex = 6;
                }
                if (sender.MenuItems.IndexOf(listItem) == 4)
                {
                    propIndex = 7;
                }

                int textureIndex = GetPedPropTextureIndex(Game.PlayerPed.Handle, propIndex);
                int newTextureIndex = GetNumberOfPedPropTextureVariations(Game.PlayerPed.Handle, propIndex, index) - 1 < textureIndex + 1 ? 0 : textureIndex + 1;
                if (textureIndex >= GetNumberOfPedPropDrawableVariations(Game.PlayerPed.Handle, propIndex))
                {
                    SetPedPropIndex(Game.PlayerPed.Handle, propIndex, -1, -1, false);
                    ClearPedProp(Game.PlayerPed.Handle, propIndex);
                    currentCharacter.PropVariations.props ??= new Dictionary<int, KeyValuePair<int, int>>();
                    currentCharacter.PropVariations.props[propIndex] = new KeyValuePair<int, int>(-1, -1);
                    listItem.Description = $"Select a prop using the arrow keys and press ~o~enter~s~ to cycle through all available textures.";
                }
                else
                {
                    SetPedPropIndex(Game.PlayerPed.Handle, propIndex, index, newTextureIndex, true);
                    currentCharacter.PropVariations.props ??= new Dictionary<int, KeyValuePair<int, int>>();
                    currentCharacter.PropVariations.props[propIndex] = new KeyValuePair<int, int>(index, newTextureIndex);
                    if (GetPedPropIndex(Game.PlayerPed.Handle, propIndex) == -1)
                    {
                        listItem.Description = $"Select a prop using the arrow keys and press ~o~enter~s~ to cycle through all available textures.";
                    }
                    else
                    {
                        int maxPropTextures = GetNumberOfPedPropTextureVariations(Game.PlayerPed.Handle, propIndex, index);
                        listItem.Description = $"Select a prop using the arrow keys and press ~o~enter~s~ to cycle through all available textures. Currently selected texture: #{newTextureIndex + 1} (of {maxPropTextures}).";
                    }
                }
                //propsMenu.UpdateScaleform();
            };
            #endregion

            #region face shape data
            /*
            Nose_Width  
            Nose_Peak_Hight  
            Nose_Peak_Lenght  
            Nose_Bone_High  
            Nose_Peak_Lowering  
            Nose_Bone_Twist  
            EyeBrown_High  
            EyeBrown_Forward  
            Cheeks_Bone_High  
            Cheeks_Bone_Width  
            Cheeks_Width  
            Eyes_Openning  
            Lips_Thickness  
            Jaw_Bone_Width 'Bone size to sides  
            Jaw_Bone_Back_Lenght 'Bone size to back  
            Chimp_Bone_Lowering 'Go Down  
            Chimp_Bone_Lenght 'Go forward  
            Chimp_Bone_Width  
            Chimp_Hole  
            Neck_Thikness  
            */

            List<float> faceFeaturesValuesList = new List<float>()
            {
               -1.0f,    // 0
               -0.9f,    // 1
               -0.8f,    // 2
               -0.7f,    // 3
               -0.6f,    // 4
               -0.5f,    // 5
               -0.4f,    // 6
               -0.3f,    // 7
               -0.2f,    // 8
               -0.1f,    // 9
                0.0f,    // 10
                0.1f,    // 11
                0.2f,    // 12
                0.3f,    // 13
                0.4f,    // 14
                0.5f,    // 15
                0.6f,    // 16
                0.7f,    // 17
                0.8f,    // 18
                0.9f,    // 19
                1.0f     // 20
            };

            string[] faceFeaturesNamesList = new string[20]
            {
                "Nose Width",               // 0
                "Noes Peak Height",         // 1
                "Nose Peak Length",         // 2
                "Nose Bone Height",         // 3
                "Nose Peak Lowering",       // 4
                "Nose Bone Twist",          // 5
                "Eyebrows Height",          // 6
                "Eyebrows Depth",           // 7
                "Cheekbones Height",        // 8
                "Cheekbones Width",         // 9
                "Cheeks Width",             // 10
                "Eyes Opening",             // 11
                "Lips Thickness",           // 12
                "Jaw Bone Width",           // 13
                "Jaw Bone Depth/Length",    // 14
                "Chin Height",              // 15
                "Chin Depth/Length",        // 16
                "Chin Width",               // 17
                "Chin Hole Size",           // 18
                "Neck Thickness"            // 19
            };

            for (int i = 0; i < 20; i++)
            {
                UIMenuSliderItem faceFeature = new UIMenuSliderItem(faceFeaturesNamesList[i], $"Set the {faceFeaturesNamesList[i]} face feature value.", 0, 20, 10, true);
                faceShapeMenu.AddItem(faceFeature);
            }

            faceShapeMenu.OnSliderChange += (sender, sliderItem, newPosition) =>
            {
                currentCharacter.FaceShapeFeatures.features ??= new Dictionary<int, float>();
                float value = faceFeaturesValuesList[newPosition];
                currentCharacter.FaceShapeFeatures.features[sender.MenuItems.IndexOf(sliderItem)] = value;
                SetPedFaceFeature(Game.PlayerPed.Handle, sender.MenuItems.IndexOf(sliderItem), value);
            };

            #endregion

            #region tattoos
            void CreateListsIfNull()
            {
                currentCharacter.PedTatttoos.HeadTattoos ??= new List<KeyValuePair<string, string>>();
                currentCharacter.PedTatttoos.TorsoTattoos ??= new List<KeyValuePair<string, string>>();
                currentCharacter.PedTatttoos.LeftArmTattoos ??= new List<KeyValuePair<string, string>>();
                currentCharacter.PedTatttoos.RightArmTattoos ??= new List<KeyValuePair<string, string>>();
                currentCharacter.PedTatttoos.LeftLegTattoos ??= new List<KeyValuePair<string, string>>();
                currentCharacter.PedTatttoos.RightLegTattoos ??= new List<KeyValuePair<string, string>>();
                currentCharacter.PedTatttoos.BadgeTattoos ??= new List<KeyValuePair<string, string>>();
            }

            void ApplySavedTattoos()
            {
                // remove all decorations, and then manually re-add them all. what a retarded way of doing this R*....
                ClearPedDecorations(Game.PlayerPed.Handle);

                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.HeadTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.TorsoTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.LeftArmTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.RightArmTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.LeftLegTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.RightLegTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.BadgeTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }

                if (!string.IsNullOrEmpty(currentCharacter.PedAppearance.HairOverlay.Key) && !string.IsNullOrEmpty(currentCharacter.PedAppearance.HairOverlay.Value))
                {
                    // reset hair value
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(currentCharacter.PedAppearance.HairOverlay.Key), (uint)GetHashKey(currentCharacter.PedAppearance.HairOverlay.Value));
                }
            }

            tattoosMenu.OnIndexChange += (sender, newIndex) =>
            {
                CreateListsIfNull();
                ApplySavedTattoos();
            };

            #region tattoos menu list select events
            tattoosMenu.OnListChange += (sender, item, menuIndex) =>
            {
                CreateListsIfNull();
                ApplySavedTattoos();
                if (sender.MenuItems.IndexOf(item) == 0) // head
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.HEAD.ElementAt(menuIndex) : FemaleTattoosCollection.HEAD.ElementAt(menuIndex);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (!currentCharacter.PedTatttoos.HeadTattoos.Contains(tat))
                    {
                        SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tat.Key), (uint)GetHashKey(tat.Value));
                    }
                }
                else if (sender.MenuItems.IndexOf(item) == 1) // torso
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.TORSO.ElementAt(menuIndex) : FemaleTattoosCollection.TORSO.ElementAt(menuIndex);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (!currentCharacter.PedTatttoos.TorsoTattoos.Contains(tat))
                    {
                        SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tat.Key), (uint)GetHashKey(tat.Value));
                    }
                }
                else if (sender.MenuItems.IndexOf(item) == 2) // left arm
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.LEFT_ARM.ElementAt(menuIndex) : FemaleTattoosCollection.LEFT_ARM.ElementAt(menuIndex);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (!currentCharacter.PedTatttoos.LeftArmTattoos.Contains(tat))
                    {
                        SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tat.Key), (uint)GetHashKey(tat.Value));
                    }
                }
                else if (sender.MenuItems.IndexOf(item) == 3) // right arm
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.RIGHT_ARM.ElementAt(menuIndex) : FemaleTattoosCollection.RIGHT_ARM.ElementAt(menuIndex);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (!currentCharacter.PedTatttoos.RightArmTattoos.Contains(tat))
                    {
                        SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tat.Key), (uint)GetHashKey(tat.Value));
                    }
                }
                else if (sender.MenuItems.IndexOf(item) == 4) // left leg
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.LEFT_LEG.ElementAt(menuIndex) : FemaleTattoosCollection.LEFT_LEG.ElementAt(menuIndex);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (!currentCharacter.PedTatttoos.LeftLegTattoos.Contains(tat))
                    {
                        SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tat.Key), (uint)GetHashKey(tat.Value));
                    }
                }
                else if (sender.MenuItems.IndexOf(item) == 5) // right leg
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.RIGHT_LEG.ElementAt(menuIndex) : FemaleTattoosCollection.RIGHT_LEG.ElementAt(menuIndex);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (!currentCharacter.PedTatttoos.RightLegTattoos.Contains(tat))
                    {
                        SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tat.Key), (uint)GetHashKey(tat.Value));
                    }
                }
                else if (sender.MenuItems.IndexOf(item) == 6) // badges
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.BADGES.ElementAt(menuIndex) : FemaleTattoosCollection.BADGES.ElementAt(menuIndex);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (!currentCharacter.PedTatttoos.BadgeTattoos.Contains(tat))
                    {
                        SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tat.Key), (uint)GetHashKey(tat.Value));
                    }
                }
            };

            tattoosMenu.OnListSelect += (sender, item, menuIndex) =>
            {
                CreateListsIfNull();

                if (sender.MenuItems.IndexOf(item) == 0) // head
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.HEAD.ElementAt(item.Index) : FemaleTattoosCollection.HEAD.ElementAt(item.Index);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (currentCharacter.PedTatttoos.HeadTattoos.Contains(tat))
                    {
                        Subtitle.Custom($"Tattoo #{item.Index + 1} has been ~r~removed~s~.");
                        currentCharacter.PedTatttoos.HeadTattoos.Remove(tat);
                    }
                    else
                    {
                        Subtitle.Custom($"Tattoo #{item.Index + 1} has been ~g~added~s~.");
                        currentCharacter.PedTatttoos.HeadTattoos.Add(tat);
                    }
                }
                else if (sender.MenuItems.IndexOf(item) == 1) // torso
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.TORSO.ElementAt(item.Index) : FemaleTattoosCollection.TORSO.ElementAt(item.Index);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (currentCharacter.PedTatttoos.TorsoTattoos.Contains(tat))
                    {
                        Subtitle.Custom($"Tattoo #{item.Index + 1} has been ~r~removed~s~.");
                        currentCharacter.PedTatttoos.TorsoTattoos.Remove(tat);
                    }
                    else
                    {
                        Subtitle.Custom($"Tattoo #{item.Index + 1} has been ~g~added~s~.");
                        currentCharacter.PedTatttoos.TorsoTattoos.Add(tat);
                    }
                }
                else if (sender.MenuItems.IndexOf(item) == 2) // left arm
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.LEFT_ARM.ElementAt(item.Index) : FemaleTattoosCollection.LEFT_ARM.ElementAt(item.Index);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (currentCharacter.PedTatttoos.LeftArmTattoos.Contains(tat))
                    {
                        Subtitle.Custom($"Tattoo #{item.Index + 1} has been ~r~removed~s~.");
                        currentCharacter.PedTatttoos.LeftArmTattoos.Remove(tat);
                    }
                    else
                    {
                        Subtitle.Custom($"Tattoo #{item.Index + 1} has been ~g~added~s~.");
                        currentCharacter.PedTatttoos.LeftArmTattoos.Add(tat);
                    }
                }
                else if (sender.MenuItems.IndexOf(item) == 3) // right arm
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.RIGHT_ARM.ElementAt(item.Index) : FemaleTattoosCollection.RIGHT_ARM.ElementAt(item.Index);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (currentCharacter.PedTatttoos.RightArmTattoos.Contains(tat))
                    {
                        Subtitle.Custom($"Tattoo #{item.Index + 1} has been ~r~removed~s~.");
                        currentCharacter.PedTatttoos.RightArmTattoos.Remove(tat);
                    }
                    else
                    {
                        Subtitle.Custom($"Tattoo #{item.Index + 1} has been ~g~added~s~.");
                        currentCharacter.PedTatttoos.RightArmTattoos.Add(tat);
                    }
                }
                else if (sender.MenuItems.IndexOf(item) == 4) // left leg
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.LEFT_LEG.ElementAt(item.Index) : FemaleTattoosCollection.LEFT_LEG.ElementAt(item.Index);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (currentCharacter.PedTatttoos.LeftLegTattoos.Contains(tat))
                    {
                        Subtitle.Custom($"Tattoo #{item.Index + 1} has been ~r~removed~s~.");
                        currentCharacter.PedTatttoos.LeftLegTattoos.Remove(tat);
                    }
                    else
                    {
                        Subtitle.Custom($"Tattoo #{item.Index + 1} has been ~g~added~s~.");
                        currentCharacter.PedTatttoos.LeftLegTattoos.Add(tat);
                    }
                }
                else if (sender.MenuItems.IndexOf(item) == 5) // right leg
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.RIGHT_LEG.ElementAt(item.Index) : FemaleTattoosCollection.RIGHT_LEG.ElementAt(item.Index);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (currentCharacter.PedTatttoos.RightLegTattoos.Contains(tat))
                    {
                        Subtitle.Custom($"Tattoo #{item.Index + 1} has been ~r~removed~s~.");
                        currentCharacter.PedTatttoos.RightLegTattoos.Remove(tat);
                    }
                    else
                    {
                        Subtitle.Custom($"Tattoo #{item.Index + 1} has been ~g~added~s~.");
                        currentCharacter.PedTatttoos.RightLegTattoos.Add(tat);
                    }
                }
                else if (sender.MenuItems.IndexOf(item) == 6) // badges
                {
                    Tattoo Tattoo = currentCharacter.IsMale ? MaleTattoosCollection.BADGES.ElementAt(item.Index) : FemaleTattoosCollection.BADGES.ElementAt(item.Index);
                    KeyValuePair<string, string> tat = new KeyValuePair<string, string>(Tattoo.collectionName, Tattoo.name);
                    if (currentCharacter.PedTatttoos.BadgeTattoos.Contains(tat))
                    {
                        Subtitle.Custom($"Badge #{item.Index + 1} has been ~r~removed~s~.");
                        currentCharacter.PedTatttoos.BadgeTattoos.Remove(tat);
                    }
                    else
                    {
                        Subtitle.Custom($"Badge #{item.Index + 1} has been ~g~added~s~.");
                        currentCharacter.PedTatttoos.BadgeTattoos.Add(tat);
                    }
                }

                ApplySavedTattoos();

            };

            // eventhandler for when a tattoo is selected.
            tattoosMenu.OnItemSelect += (sender, item, index) =>
            {
                Notify.Success("All tattoos have been removed.");
                currentCharacter.PedTatttoos.HeadTattoos.Clear();
                currentCharacter.PedTatttoos.TorsoTattoos.Clear();
                currentCharacter.PedTatttoos.LeftArmTattoos.Clear();
                currentCharacter.PedTatttoos.RightArmTattoos.Clear();
                currentCharacter.PedTatttoos.LeftLegTattoos.Clear();
                currentCharacter.PedTatttoos.RightLegTattoos.Clear();
                currentCharacter.PedTatttoos.BadgeTattoos.Clear();
                ClearPedDecorations(Game.PlayerPed.Handle);
            };

            #endregion
            #endregion


            // handle list changes in the character creator menu.
            createCharacterMenu.OnListChange += (sender, item, itemIndex) =>
            {
                if (item == faceExpressionList)
                {
                    currentCharacter.FacialExpression = facial_expressions[itemIndex];
                    SetFacialIdleAnimOverride(Game.PlayerPed.Handle, currentCharacter.FacialExpression ?? facial_expressions[0], null);
                }
            };

            // handle button presses for the createCharacter menu.
            createCharacterMenu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == saveButton) // save ped
                {
                    if (await SavePed())
                    {
                        while (!MenuHandler.IsAnyMenuOpen)
                        {
                            await BaseScript.Delay(0);
                        }

                        while (IsControlPressed(2, 201) || IsControlPressed(2, 217) || IsDisabledControlPressed(2, 201) || IsDisabledControlPressed(2, 217))
                        {
                            await BaseScript.Delay(0);
                        }

                        await BaseScript.Delay(100);

                        createCharacterMenu.GoBack();
                    }
                }
                else if (item == exitNoSave) // exit without saving
                {
                    bool confirm = false;
                    AddTextEntry("vmenu_warning_message_first_line", "Are you sure you want to exit the character creator?");
                    AddTextEntry("vmenu_warning_message_second_line", "You will lose all (unsaved) customization!");
                    createCharacterMenu.Visible = false;

                    // wait for confirmation or cancel input.
                    while (true)
                    {
                        await BaseScript.Delay(0);
                        int unk = 1;
                        int unk2 = 1;
                        SetWarningMessage("vmenu_warning_message_first_line", 20, "vmenu_warning_message_second_line", true, 0, ref unk, ref unk2, true, 0);
                        if (IsControlJustPressed(2, 201) || IsControlJustPressed(2, 217)) // continue/accept
                        {
                            confirm = true;
                            break;
                        }
                        else if (IsControlJustPressed(2, 202)) // cancel
                        {
                            break;
                        }
                    }

                    // if confirmed to discard changes quit the editor.
                    if (confirm)
                    {
                        while (IsControlPressed(2, 201) || IsControlPressed(2, 217) || IsDisabledControlPressed(2, 201) || IsDisabledControlPressed(2, 217))
                        {
                            await BaseScript.Delay(0);
                        }

                        await BaseScript.Delay(100);
                        menu.Visible = true;
                    }
                    else // otherwise cancel and go back to the editor.
                    {
                        createCharacterMenu.Visible = true;
                    }
                }
                else if (item == inheritanceButton) // update the inheritance menu anytime it's opened to prevent some weird glitch where old data is used.
                {
                    PedHeadBlendData data = Game.PlayerPed.GetHeadBlendData();
                    inheritanceDads.Index = inheritanceDads.Items.IndexOf(dads.FirstOrDefault(entry => entry.Value == data.FirstFaceShape).Key);
                    inheritanceMoms.Index = inheritanceMoms.Items.IndexOf(moms.FirstOrDefault(entry => entry.Value == data.SecondFaceShape).Key);
                    inheritanceShapeMix.Value = UnclampMix(data.ParentFaceShapePercent);
                    inheritanceSkinMix.Value = UnclampMix(data.ParentSkinTonePercent);
                }
            };

            // eventhandler for whenever a menu item is selected in the main mp characters menu.
            menu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == createMaleBtn)
                {
                    uint model = (uint)GetHashKey("mp_m_freemode_01");

                    if (!HasModelLoaded(model))
                    {
                        RequestModel(model);
                        while (!HasModelLoaded(model))
                        {
                            await BaseScript.Delay(0);
                        }
                    }

                    int maxHealth = Game.PlayerPed.MaxHealth;
                    int maxArmour = Game.Player.MaxArmor;
                    int health = Game.PlayerPed.Health;
                    int armour = Game.PlayerPed.Armor;

                    SaveWeaponLoadout("vmenu_temp_weapons_loadout_before_respawn");
                    SetPlayerModel(Game.Player.Handle, model);
                    await SpawnWeaponLoadoutAsync("vmenu_temp_weapons_loadout_before_respawn", false, true, true);

                    Game.Player.MaxArmor = maxArmour;
                    Game.PlayerPed.MaxHealth = maxHealth;
                    Game.PlayerPed.Health = health;
                    Game.PlayerPed.Armor = armour;

                    ClearPedDecorations(Game.PlayerPed.Handle);
                    ClearPedFacialDecorations(Game.PlayerPed.Handle);
                    SetPedDefaultComponentVariation(Game.PlayerPed.Handle);
                    SetPedHairColor(Game.PlayerPed.Handle, 0, 0);
                    SetPedEyeColor(Game.PlayerPed.Handle, 0);
                    ClearAllPedProps(Game.PlayerPed.Handle);

                    MakeCreateCharacterMenu(male: true);
                }
                else if (item == createFemaleBtn)
                {
                    uint model = (uint)GetHashKey("mp_f_freemode_01");

                    if (!HasModelLoaded(model))
                    {
                        RequestModel(model);
                        while (!HasModelLoaded(model))
                        {
                            await BaseScript.Delay(0);
                        }
                    }

                    int maxHealth = Game.PlayerPed.MaxHealth;
                    int maxArmour = Game.Player.MaxArmor;
                    int health = Game.PlayerPed.Health;
                    int armour = Game.PlayerPed.Armor;

                    SaveWeaponLoadout("vmenu_temp_weapons_loadout_before_respawn");
                    SetPlayerModel(Game.Player.Handle, model);
                    await SpawnWeaponLoadoutAsync("vmenu_temp_weapons_loadout_before_respawn", false, true, true);

                    Game.Player.MaxArmor = maxArmour;
                    Game.PlayerPed.MaxHealth = maxHealth;
                    Game.PlayerPed.Health = health;
                    Game.PlayerPed.Armor = armour;

                    ClearPedDecorations(Game.PlayerPed.Handle);
                    ClearPedFacialDecorations(Game.PlayerPed.Handle);
                    SetPedDefaultComponentVariation(Game.PlayerPed.Handle);
                    SetPedHairColor(Game.PlayerPed.Handle, 0, 0);
                    SetPedEyeColor(Game.PlayerPed.Handle, 0);
                    ClearAllPedProps(Game.PlayerPed.Handle);

                    MakeCreateCharacterMenu(male: false);
                }
                else if (item == savedCharacters)
                {
                    UpdateSavedPedsMenu();
                }
            };
        }

        /// <summary>
        /// Spawns this saved ped.
        /// </summary>
        /// <param name="name"></param>
        internal async Task SpawnThisCharacter(string name, bool restoreWeapons)
        {
            currentCharacter = StorageManager.GetSavedMpCharacterData(name);
            await SpawnSavedPed(restoreWeapons);
        }

        /// <summary>
        /// Spawns the ped from the data inside <see cref="currentCharacter"/>.
        /// Character data MUST be set BEFORE calling this function.
        /// </summary>
        /// <returns></returns>
        private async Task SpawnSavedPed(bool restoreWeapons)
        {
            if (currentCharacter.Version < 1)
            {
                return;
            }
            if (IsModelInCdimage(currentCharacter.ModelHash))
            {
                if (!HasModelLoaded(currentCharacter.ModelHash))
                {
                    RequestModel(currentCharacter.ModelHash);
                    while (!HasModelLoaded(currentCharacter.ModelHash))
                    {
                        await BaseScript.Delay(0);
                    }
                }
                int maxHealth = Game.PlayerPed.MaxHealth;
                int maxArmour = Game.Player.MaxArmor;
                int health = Game.PlayerPed.Health;
                int armour = Game.PlayerPed.Armor;

                SaveWeaponLoadout("vmenu_temp_weapons_loadout_before_respawn");
                SetPlayerModel(Game.Player.Handle, currentCharacter.ModelHash);
                await SpawnWeaponLoadoutAsync("vmenu_temp_weapons_loadout_before_respawn", false, true, true);

                Game.Player.MaxArmor = maxArmour;
                Game.PlayerPed.MaxHealth = maxHealth;
                Game.PlayerPed.Health = health;
                Game.PlayerPed.Armor = armour;

                ClearPedDecorations(Game.PlayerPed.Handle);
                ClearPedFacialDecorations(Game.PlayerPed.Handle);
                SetPedDefaultComponentVariation(Game.PlayerPed.Handle);
                SetPedHairColor(Game.PlayerPed.Handle, 0, 0);
                SetPedEyeColor(Game.PlayerPed.Handle, 0);
                ClearAllPedProps(Game.PlayerPed.Handle);

                #region headblend
                PedHeadBlendData data = currentCharacter.PedHeadBlendData;
                SetPedHeadBlendData(Game.PlayerPed.Handle, data.FirstFaceShape, data.SecondFaceShape, data.ThirdFaceShape, data.FirstSkinTone, data.SecondSkinTone, data.ThirdSkinTone, data.ParentFaceShapePercent, data.ParentSkinTonePercent, 0f, data.IsParentInheritance);

                while (!HasPedHeadBlendFinished(Game.PlayerPed.Handle))
                {
                    await BaseScript.Delay(0);
                }
                #endregion

                #region appearance
                PedAppearance appData = currentCharacter.PedAppearance;
                // hair
                SetPedComponentVariation(Game.PlayerPed.Handle, 2, appData.hairStyle, 0, 0);
                SetPedHairColor(Game.PlayerPed.Handle, appData.hairColor, appData.hairHighlightColor);
                if (!string.IsNullOrEmpty(appData.HairOverlay.Key) && !string.IsNullOrEmpty(appData.HairOverlay.Value))
                {
                    SetPedFacialDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(appData.HairOverlay.Key), (uint)GetHashKey(appData.HairOverlay.Value));
                }
                // blemishes
                SetPedHeadOverlay(Game.PlayerPed.Handle, 0, appData.blemishesStyle, appData.blemishesOpacity);
                // bread
                SetPedHeadOverlay(Game.PlayerPed.Handle, 1, appData.beardStyle, appData.beardOpacity);
                SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, appData.beardColor, appData.beardColor);
                // eyebrows
                SetPedHeadOverlay(Game.PlayerPed.Handle, 2, appData.eyebrowsStyle, appData.eyebrowsOpacity);
                SetPedHeadOverlayColor(Game.PlayerPed.Handle, 2, 1, appData.eyebrowsColor, appData.eyebrowsColor);
                // ageing
                SetPedHeadOverlay(Game.PlayerPed.Handle, 3, appData.ageingStyle, appData.ageingOpacity);
                // makeup
                SetPedHeadOverlay(Game.PlayerPed.Handle, 4, appData.makeupStyle, appData.makeupOpacity);
                SetPedHeadOverlayColor(Game.PlayerPed.Handle, 4, 2, appData.makeupColor, appData.makeupColor);
                // blush
                SetPedHeadOverlay(Game.PlayerPed.Handle, 5, appData.blushStyle, appData.blushOpacity);
                SetPedHeadOverlayColor(Game.PlayerPed.Handle, 5, 2, appData.blushColor, appData.blushColor);
                // complexion
                SetPedHeadOverlay(Game.PlayerPed.Handle, 6, appData.complexionStyle, appData.complexionOpacity);
                // sundamage
                SetPedHeadOverlay(Game.PlayerPed.Handle, 7, appData.sunDamageStyle, appData.sunDamageOpacity);
                // lipstick
                SetPedHeadOverlay(Game.PlayerPed.Handle, 8, appData.lipstickStyle, appData.lipstickOpacity);
                SetPedHeadOverlayColor(Game.PlayerPed.Handle, 8, 2, appData.lipstickColor, appData.lipstickColor);
                // moles and freckles
                SetPedHeadOverlay(Game.PlayerPed.Handle, 9, appData.molesFrecklesStyle, appData.molesFrecklesOpacity);
                // chest hair 
                SetPedHeadOverlay(Game.PlayerPed.Handle, 10, appData.chestHairStyle, appData.chestHairOpacity);
                SetPedHeadOverlayColor(Game.PlayerPed.Handle, 10, 1, appData.chestHairColor, appData.chestHairColor);
                // body blemishes 
                SetPedHeadOverlay(Game.PlayerPed.Handle, 11, appData.bodyBlemishesStyle, appData.bodyBlemishesOpacity);
                // eyecolor
                SetPedEyeColor(Game.PlayerPed.Handle, appData.eyeColor);
                #endregion

                #region Face Shape Data
                for (int i = 0; i < 19; i++)
                {
                    SetPedFaceFeature(Game.PlayerPed.Handle, i, 0f);
                }

                if (currentCharacter.FaceShapeFeatures.features != null)
                {
                    foreach (KeyValuePair<int, float> t in currentCharacter.FaceShapeFeatures.features)
                    {
                        SetPedFaceFeature(Game.PlayerPed.Handle, t.Key, t.Value);
                    }
                }
                else
                {
                    currentCharacter.FaceShapeFeatures.features = new Dictionary<int, float>();
                }

                #endregion

                #region Clothing Data
                if (currentCharacter.DrawableVariations.clothes != null && currentCharacter.DrawableVariations.clothes.Count > 0)
                {
                    foreach (KeyValuePair<int, KeyValuePair<int, int>> cd in currentCharacter.DrawableVariations.clothes)
                    {
                        SetPedComponentVariation(Game.PlayerPed.Handle, cd.Key, cd.Value.Key, cd.Value.Value, 0);
                    }
                }
                #endregion

                #region Props Data
                if (currentCharacter.PropVariations.props != null && currentCharacter.PropVariations.props.Count > 0)
                {
                    foreach (KeyValuePair<int, KeyValuePair<int, int>> cd in currentCharacter.PropVariations.props)
                    {
                        if (cd.Value.Key > -1)
                        {
                            SetPedPropIndex(Game.PlayerPed.Handle, cd.Key, cd.Value.Key, cd.Value.Value > -1 ? cd.Value.Value : 0, true);
                        }
                    }
                }
                #endregion

                #region Tattoos

                currentCharacter.PedTatttoos.HeadTattoos ??= new List<KeyValuePair<string, string>>();
                currentCharacter.PedTatttoos.TorsoTattoos ??= new List<KeyValuePair<string, string>>();
                currentCharacter.PedTatttoos.LeftArmTattoos ??= new List<KeyValuePair<string, string>>();
                currentCharacter.PedTatttoos.RightArmTattoos ??= new List<KeyValuePair<string, string>>();
                currentCharacter.PedTatttoos.LeftLegTattoos ??= new List<KeyValuePair<string, string>>();
                currentCharacter.PedTatttoos.RightLegTattoos ??= new List<KeyValuePair<string, string>>();
                currentCharacter.PedTatttoos.BadgeTattoos ??= new List<KeyValuePair<string, string>>();

                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.HeadTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.TorsoTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.LeftArmTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.RightArmTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.LeftLegTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.RightLegTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                foreach (KeyValuePair<string, string> tattoo in currentCharacter.PedTatttoos.BadgeTattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Key), (uint)GetHashKey(tattoo.Value));
                }
                #endregion
            }

            // Set the facial expression, or set it to 'normal' if it wasn't saved/set before.
            SetFacialIdleAnimOverride(Game.PlayerPed.Handle, currentCharacter.FacialExpression ?? facial_expressions[0], null);
        }

        /// <summary>
        /// Creates the saved mp characters menu.
        /// </summary>
        private void CreateSavedPedsMenu()
        {
            UpdateSavedPedsMenu();

            UIMenuItem spawnPed = new UIMenuItem("Spawn Saved Character", "Spawns the selected saved character.");
            editPedBtn = new UIMenuItem("Edit Saved Character", "This allows you to edit everything about your saved character. The changes will be saved to this character's save file entry once you hit the save button.");
            UIMenuItem clonePed = new UIMenuItem("Clone Saved Character", "This will make a clone of your saved character. It will ask you to provide a name for that character. If that name is already taken the action will be canceled.");
            UIMenuItem setAsDefaultPed = new UIMenuItem("Set As Default Character", "If you set this character as your default character, and you enable the 'Respawn As Default MP Character' option in the Misc Settings menu, then you will be set as this character whenever you (re)spawn.");
            UIMenuItem renameCharacter = new UIMenuItem("Rename Saved Character", "You can rename this saved character. If the name is already taken then the action will be canceled.");
            UIMenuItem delPed = new UIMenuItem("Delete Saved Character", "Deletes the selected saved character. This can not be undone!");
            delPed.SetLeftBadge(BadgeIcon.WARNING);
            manageSavedCharacterMenu.AddItem(spawnPed);
            manageSavedCharacterMenu.AddItem(editPedBtn);
            manageSavedCharacterMenu.AddItem(clonePed);
            manageSavedCharacterMenu.AddItem(setAsDefaultPed);
            manageSavedCharacterMenu.AddItem(renameCharacter);
            manageSavedCharacterMenu.AddItem(delPed);
            editPedBtn.Activated += async (a, b) => await a.SwitchTo(createCharacterMenu, 0, true);

            manageSavedCharacterMenu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == editPedBtn)
                {
                    currentCharacter = StorageManager.GetSavedMpCharacterData(selectedSavedCharacterManageName);

                    await SpawnSavedPed(true);

                    MakeCreateCharacterMenu(male: currentCharacter.IsMale, editPed: true);
                }
                else if (item == spawnPed)
                {
                    currentCharacter = StorageManager.GetSavedMpCharacterData(selectedSavedCharacterManageName);

                    await SpawnSavedPed(true);
                }
                else if (item == clonePed)
                {
                    MultiplayerPedData tmpCharacter = StorageManager.GetSavedMpCharacterData("mp_ped_" + selectedSavedCharacterManageName);
                    string name = await GetUserInput(windowTitle: "Enter a name for the cloned character", defaultText: tmpCharacter.SaveName.Substring(7), maxInputLength: 30);
                    if (string.IsNullOrEmpty(name))
                    {
                        Notify.Error(CommonErrors.InvalidSaveName);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(GetResourceKvpString("mp_ped_" + name)))
                        {
                            Notify.Error(CommonErrors.SaveNameAlreadyExists);
                        }
                        else
                        {
                            tmpCharacter.SaveName = "mp_ped_" + name;
                            if (StorageManager.SaveJsonData("mp_ped_" + name, JsonConvert.SerializeObject(tmpCharacter), false))
                            {
                                Notify.Success($"Your character has been cloned. The name of the cloned character is: ~g~<C>{name}</C>~s~.");
                                UpdateSavedPedsMenu();
                            }
                            else
                            {
                                Notify.Error("The clone could not be created, reason unknown. Does a character already exist with that name? :(");
                            }
                        }
                    }
                }
                else if (item == renameCharacter)
                {
                    MultiplayerPedData tmpCharacter = StorageManager.GetSavedMpCharacterData("mp_ped_" + selectedSavedCharacterManageName);
                    string name = await GetUserInput(windowTitle: "Enter a new character name", defaultText: tmpCharacter.SaveName.Substring(7), maxInputLength: 30);
                    if (string.IsNullOrEmpty(name))
                    {
                        Notify.Error(CommonErrors.InvalidInput);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(GetResourceKvpString("mp_ped_" + name)))
                        {
                            Notify.Error(CommonErrors.SaveNameAlreadyExists);
                        }
                        else
                        {
                            tmpCharacter.SaveName = "mp_ped_" + name;
                            if (StorageManager.SaveJsonData("mp_ped_" + name, JsonConvert.SerializeObject(tmpCharacter), false))
                            {
                                StorageManager.DeleteSavedStorageItem("mp_ped_" + selectedSavedCharacterManageName);
                                Notify.Success($"Your character has been renamed to ~g~<C>{name}</C>~s~.");
                                UpdateSavedPedsMenu();
                                while (!MenuHandler.IsAnyMenuOpen)
                                {
                                    await BaseScript.Delay(0);
                                }
                                manageSavedCharacterMenu.GoBack();
                            }
                            else
                            {
                                Notify.Error("Something went wrong while renaming your character, your old character will NOT be deleted because of this.");
                            }
                        }
                    }
                }
                else if (item == delPed)
                {
                    if (delPed.RightLabel == "Are you sure?")
                    {
                        delPed.SetRightLabel("");
                        DeleteResourceKvp("mp_ped_" + selectedSavedCharacterManageName);
                        Notify.Success("Your saved character has been deleted.");
                        manageSavedCharacterMenu.GoBack();
                        UpdateSavedPedsMenu();
                    }
                    else
                    {
                        delPed.SetRightLabel("Are you sure?");
                    }
                }
                else if (item == setAsDefaultPed)
                {
                    Notify.Success($"Your character <C>{selectedSavedCharacterManageName}</C> will now be used as your default character whenever you (re)spawn.");
                    SetResourceKvp("vmenu_default_character", "mp_ped_" + selectedSavedCharacterManageName);
                }

                if (item != delPed)
                {
                    if (delPed.RightLabel == "Are you sure?")
                    {
                        delPed.SetRightLabel("");
                    }
                }
            };

            // reset the "are you sure" state.
            manageSavedCharacterMenu.OnMenuClose += (sender) =>
            {
                manageSavedCharacterMenu.MenuItems[2].SetRightLabel("");
            };

            savedCharactersMenu.OnItemSelect += (sender, item, index) =>
            {
                selectedSavedCharacterManageName = item.Label;
                manageSavedCharacterMenu.Subtitle = item.Label;
                //manageSavedCharacterMenu.CounterPreText = $"{(item.Label.Substring(0, 3) == "(M)" ? "(Male) " : "(Female) ")}";
            };
        }

        /// <summary>
        /// Updates the saved peds menu.
        /// </summary>
        private void UpdateSavedPedsMenu()
        {
            string defaultChar = GetResourceKvpString("vmenu_default_character") ?? "";

            List<dynamic> names = new List<dynamic>();
            int handle = StartFindKvp("mp_ped_");
            while (true)
            {
                string foundName = FindKvp(handle);
                if (string.IsNullOrEmpty(foundName))
                {
                    break;
                }
                else
                {
                    names.Add(foundName.Substring(7));
                }
            }
            EndFindKvp(handle);
            savedCharactersMenu.Clear();
            if (names.Count > 0)
            {
                names.Sort((a, b) => { return a.ToLower().CompareTo(b.ToLower()); });
                foreach (dynamic item in names)
                {
                    dynamic tmpData = StorageManager.GetSavedMpCharacterData("mp_ped_" + item);
                    UIMenuItem btn = new UIMenuItem(item, "Click to spawn, edit, clone, rename or delete this saved character.");
                    btn.SetRightLabel($"({(tmpData.IsMale ? "M" : "F")}) →→→");
                    if (defaultChar == "mp_ped_" + item)
                    {
                        btn.SetLeftBadge(BadgeIcon.TICK);
                        btn.Description += " ~g~This character is currently set as your default character and will be used whenever you (re)spawn.";
                    }
                    savedCharactersMenu.AddItem(btn);
                    btn.Activated += async (sender, args) => await sender.SwitchTo(manageSavedCharacterMenu, 0, true);
                }
            }
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
