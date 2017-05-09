using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace DnD_Next_5e
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        public bool _isModified = false;



        private void winMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }  

        #region File Menu



        private void btnNewChar_Click(object sender, RoutedEventArgs e)
        {
            if(_isModified)
            {
                MessageBoxResult save = MessageBox.Show("Changes have been made. Would you like to save them?", "Save Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (save == MessageBoxResult.Yes)
                {
                    SaveFile();
                    SetToDefault();
                    _isModified = false; //this needs to go after SetToDefault() because _isModified gets set to 'true' during UpdateAllAbilities()
                }
                if (save == MessageBoxResult.No)
                {
                    SetToDefault();
                    _isModified = false; //this needs to go after SetToDefault() because _isModified gets set to 'true' during UpdateAllAbilities()
                }
            }
            else
            {
                SetToDefault();
                _isModified = false;
            }
        }

        private void btnHelpCreateChar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This Feature Coming Soon. or not.");
        }

        private void btnOpenChar_Click(object sender, RoutedEventArgs e)
        {
            if (_isModified)
            {
                MessageBoxResult save = MessageBox.Show("Changes have been made. Would you like to save them?", "Save Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (save == MessageBoxResult.Yes)
                {
                    SaveFile();
                    OpenFile();
                    _isModified = false; //this needs to go after SetToDefault() because _isModified gets set to 'true' during UpdateAllAbilities()
                }
                if (save == MessageBoxResult.No)
                {
                    OpenFile();
                    _isModified = false; //this needs to go after SetToDefault() because _isModified gets set to 'true' during UpdateAllAbilities()
                }
            }
            else
            {
                OpenFile();
                _isModified = false;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (!_isModified)
            {
                this.Close();
            }
            else
            {
                MessageBoxResult save = MessageBox.Show("Changes have been made. Would you like to save them?", "Save Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (save == MessageBoxResult.Yes)
                {
                    SaveFile();                    
                    this.Close();
                }
                if (save == MessageBoxResult.No)
                {
                    this.Close();
                }
            }
        }

        private void btnStats_Click(object sender, RoutedEventArgs e)
        {
            GridVisiblity(grdStats);
        }

        private void btnEquipment_Click(object sender, RoutedEventArgs e)
        {
            GridVisiblity(grdEquipment);
        }

        private void btnSpells_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Convert.ToString(elpStrSave.Fill));
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region When to update fields
        private void field_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateAllAbilities();
        }

        private void enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateAllAbilities();
            }
        }

        private void numbersOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = OnlyNumbersAllowed(e.Text);
        }

        private void numbersAndPlus_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = NumbersAndPlusAllowed(e.Text);
        }

        private void ellipse_click(object sender, MouseButtonEventArgs e)
        {
            var elip = sender as Ellipse;
            var fill = elip.Fill as SolidColorBrush;

            if (fill != null && fill.Color == Colors.White)
            {
                elip.Fill = Brushes.Black;
            }
            else
            {
                elip.Fill = Brushes.White;
            }
            UpdateAllAbilities();        
        }

        private void Mod_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateAllAbilities();
        }

        private void Mod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int tempInt = 0;
                string name = sender.GetPropertyVar("Name");
                if (!int.TryParse(sender.GetPropertyVar("Text"), out tempInt)) return;
                UpdateAllAbilities();

                switch (name)
                {
                    case "txtStrMod":
                        AddPlusIfPositive(tempInt, txtStrMod);
                        break;
                }
            }
        }

        //private void listClass_DropDownClosed(object sender, EventArgs e)
        //{
        //    if (selectedClass != listClass.Text)
        //    {
        //        UpdateClass(listClass.Text);
        //        selectedClass = listClass.Text;
        //    }
        //}

        //private void listClass_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (selectedClass != listClass.Text)
        //    {
        //        UpdateClass(listClass.Text);
        //        selectedClass = listClass.Text;
        //    }
        //}

        //private void listBackground_DropDownClosed(object sender, EventArgs e)
        //{
        //    if (selectedBackground != listBackground.Text)
        //    {
        //        UpdateBackground(listBackground.Text);
        //        selectedBackground = listBackground.Text;
        //    }
        //}

        //private void listBackground_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (selectedBackground != listBackground.Text)
        //    {
        //        UpdateBackground(listBackground.Text);
        //        selectedBackground = listBackground.Text;
        //    }
        //}

        //private void listRace_DropDownClosed(object sender, EventArgs e)
        //{
        //    selectedRace = UpdateRace(selectedRace, listRace.Text);
        //    UpdateAllAbilities();
        //    if (msgCount < 1 && listRace.Text == "Half-Elf")
        //    {
        //        MessageBox.Show(string.Format("Please manually increase 2 Stats except Charisma by 1.{0}NOTE: You can't choose the same stat twice.{0}DOUBLE NOTE: If you change the Race you need to manually decrease the Stats you chose by 1.", Environment.NewLine));
        //        msgCount++;
        //    }
        //    else
        //    {
        //        if (listRace.Text != "Half-Elf" && msgCount == 1) msgCount--;
        //    }
        //}

        //private void listRace_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    selectedRace = UpdateRace(selectedRace, listRace.Text);
        //    UpdateAllAbilities();
        //    if (msgCount < 1 && listRace.Text == "Half-Elf")
        //    {
        //        MessageBox.Show(string.Format("Please manually increase 2 Stats except Charisma by 1.{0}NOTE: You can't choose the same stat twice.{0}DOUBLE NOTE: If you change the Race you need to manually decrease the Stats you chose by 1.", Environment.NewLine));
        //        msgCount++;
        //    }
        //    else
        //    {
        //        if (listRace.Text != "Half-Elf" && msgCount == 1) msgCount--;
        //    }
        //}

        #endregion

        #region Methods

        /// <summary>
        /// Update All Abilities and Modifiers
        /// </summary>
        public void UpdateAllAbilities() //Sets _isModified to true
        {
            SetAbilityModifier(txtStr, txtStrMod);
            SetAbilityModifier(txtDex, txtDexMod);
            SetAbilityModifier(txtCon, txtConMod);
            SetAbilityModifier(txtInt, txtIntMod);
            SetAbilityModifier(txtWis, txtWisMod);
            SetAbilityModifier(txtCha, txtChaMod);
            CalcAbilities(elpStrSave, txtStrMod, txtStrSaveThrw);
            CalcAbilities(elpDexSave, txtDexMod, txtDexSaveThrw);
            CalcAbilities(elpConSave, txtConMod, txtConSaveThrw);
            CalcAbilities(elpIntSave, txtIntMod, txtIntSaveThrw);
            CalcAbilities(elpWisSave, txtWisMod, txtWisSaveThrw);
            CalcAbilities(elpChaSave, txtChaMod, txtChaSaveThrw);
            CalcAbilities(elpAcrobatics, txtDexMod, txtAcrobatics);
            CalcAbilities(elpAnimalHandling, txtWisMod, txtAnimalHandling);
            CalcAbilities(elpArcana, txtIntMod, txtArcana);
            CalcAbilities(elpAthletics, txtStrMod, txtAthletics);
            CalcAbilities(elpDeception, txtChaMod, txtDeception);
            CalcAbilities(elpHistory, txtIntMod, txtHistory);
            CalcAbilities(elpInsight, txtWisMod, txtInsight);
            CalcAbilities(elpIntimidation, txtChaMod, txtIntimidation);
            CalcAbilities(elpInvestigation, txtIntMod, txtInvestigation);
            CalcAbilities(elpMedicine, txtWisMod, txtMedicine);
            CalcAbilities(elpNature, txtIntMod, txtNature);
            CalcAbilities(elpPerception, txtWisMod, txtPerception);
            CalcAbilities(elpPerformance, txtChaMod, txtPerformance);
            CalcAbilities(elpPersuasion, txtChaMod, txtPersuasion);
            CalcAbilities(elpReligion, txtIntMod, txtReligion);
            CalcAbilities(elpSleightOfHand, txtDexMod, txtSleightOfHand);
            CalcAbilities(elpStealth, txtDexMod, txtStealth);
            CalcAbilities(elpSurvival, txtWisMod, txtSurvival);
            SetModifiedBool();
        }

        /// <summary>
        /// Calculates the Modifier for an Attribute
        /// </summary>
        /// <param name="txtIniVal"> Ability to find modifier for </param>
        /// <param name="txtCalcMod"> Ability Modifier that will be calculated</param>
        public void SetAbilityModifier(TextBox txtIniVal, TextBox txtCalcMod)
        {
            int intStr = 0;

            if (int.TryParse(txtIniVal.Text, out intStr))
            {
                double tempInt = Math.Floor(((double)intStr - 10.0) / 2.0);
                intStr = Convert.ToInt32(tempInt);
                AddPlusIfPositive(intStr, txtCalcMod);
            }
        }

        /// <summary>
        /// Adds Prof and Stat Mod 
        /// </summary>
        /// <param name="tempElp">Elipse to check</param>
        /// <param name="txtMod">Stat Modifier to check</param>
        /// <param name="txtOutMod">Stat Modifier to set</param>
        public void CalcAbilities(Ellipse tempElp, TextBox txtMod, TextBox txtOutMod)
        {

            int tempInt = 0;

            var fill = tempElp.Fill as SolidColorBrush;

            if (fill != null && fill.Color == Colors.Black)
            {
                tempInt = (Convert.ToInt32(txtMod.Text) + Convert.ToInt32(txtProfBonus.Text));
                AddPlusIfPositive(tempInt, txtOutMod);
            }
            else
            {
                tempInt = Convert.ToInt32(txtMod.Text);
                AddPlusIfPositive(tempInt, txtOutMod);
            }
        }

        /// <summary>
        /// Adds a + if number is positive
        /// </summary>
        /// <param name="tempInt">Int to check</param>
        /// <param name="CalcVal">Textbox to return string to</param>
        public void AddPlusIfPositive(int tempInt, TextBox CalcVal)
        {
            string tempString = "";

            if (tempInt >= 0)
            {
                tempString = string.Format("+{0}", tempInt);
            }
            else
            {
                tempString = tempInt.ToString();
            }

            CalcVal.Text = tempString;

        }

        /// <summary>
        /// Only allows Numbers
        /// </summary>
        /// <param name="text">Character to check</param>
        /// <returns></returns>
        public static bool OnlyNumbersAllowed(string text)
        {
            Regex numbers = new Regex("[^0-9]+");
            return numbers.IsMatch(text);
        }

        /// <summary>
        /// Only allows numbers and plus sign
        /// </summary>
        /// <param name="text">Character to check</param>
        /// <returns></returns>
        public static bool NumbersAndPlusAllowed(string text)
        {
            Regex numbers = new Regex("[^0-9+-]+");
            return numbers.IsMatch(text);
        }

        /// <summary>
        /// Selects all text in Textbox on focus
        /// </summary>
        private void SelectAll(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            
            if(tb != null)
            {
                tb.SelectAll();
            }
        }

        /// <summary>
        /// Does not select all text if it has keyboard focus
        /// </summary>
        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);

            if(tb != null)
            {
                if(!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }

        /// <summary>
        /// Sets _isModified to true
        /// </summary>
        public void SetModifiedBool()
        {
            if (!_isModified)
                _isModified = true;            
        }

        /// <summary>
        /// Sets all fields back to Default Values
        /// </summary>
        public void SetToDefault()
        {
            txtCharName.Text = "Your Character Name Goes Here!";
            txtLevel.Text = "1";
            listClass.Text = "";
            listBackground.Text = "";
            txtPlayerName.Text = "Your Real Name!";
            listRace.Text = "";
            //"Mat Fuked uP" = "";
            //"Mat Fuked uP" = "";
            txtProfBonus.Text = "1";
            txtStr.Text = "10";
            txtDex.Text = "10";
            txtCon.Text = "10";
            txtInt.Text = "10";
            txtWis.Text = "10";
            txtCha.Text = "10";
            elpStrSave.Fill = Brushes.White;
            elpDexSave.Fill = Brushes.White;
            elpConSave.Fill = Brushes.White;
            elpIntSave.Fill = Brushes.White;
            elpWisSave.Fill = Brushes.White;
            elpChaSave.Fill = Brushes.White;
            elpAcrobatics.Fill = Brushes.White;
            elpAnimalHandling.Fill = Brushes.White;
            elpArcana.Fill = Brushes.White;
            elpAthletics.Fill = Brushes.White;
            elpDeception.Fill = Brushes.White;
            elpHistory.Fill = Brushes.White;
            elpInsight.Fill = Brushes.White;
            elpIntimidation.Fill = Brushes.White;
            elpInvestigation.Fill = Brushes.White;
            elpMedicine.Fill = Brushes.White;
            elpNature.Fill = Brushes.White;
            elpPerception.Fill = Brushes.White;
            elpPerformance.Fill = Brushes.White;
            elpPersuasion.Fill = Brushes.White;
            elpReligion.Fill = Brushes.White;
            elpSleightOfHand.Fill = Brushes.White;
            elpStealth.Fill = Brushes.White;
            elpSurvival.Fill = Brushes.White;
            UpdateAllAbilities();            
        }

        /// <summary>
        /// Takes Previous Race and removes that Race's Stat Mods and Applys newly selected Race's Stat Mods
        /// </summary>
        /// <param name="OriginalRace">Previous Race</param>
        /// <param name="NewRace">Newly Selected Race</param>
        public string UpdateRace(String OriginalRace, String NewRace)
        {
            var currentModifier = GetModifierValues(OriginalRace);
            var newModifier = GetModifierValues(NewRace);

            RemoveCurrentModifier(currentModifier);
            ClearProficiencies(OriginalRace);
            ApplyModifier(newModifier);

            return NewRace;
        }

        /// <summary>
        /// When 'UpdateRace' is called, set the Ellipes of the Original Race back to white
        /// </summary>
        /// <param name="OrignalRace"></param>
        public void ClearProficiencies(string OrignalRace)
        {
            switch (OrignalRace)
            {
                case "High Elf":
                    elpPerception.Fill = Brushes.White;
                    break;

                case "Wood Elf":
                    elpPerception.Fill = Brushes.White;
                    break;

                case "Dark Elf":
                    elpPerception.Fill = Brushes.White;
                    break;

                case "Forest Gnome":
                    elpIntSave.Fill = Brushes.White;
                    elpWisSave.Fill = Brushes.White;
                    elpChaSave.Fill = Brushes.White;
                    break;

                case "Rock Gnome":
                    elpIntSave.Fill = Brushes.White;
                    elpWisSave.Fill = Brushes.White;
                    elpChaSave.Fill = Brushes.White;
                    break;

                case "Half-Orc":
                    elpIntimidation.Fill = Brushes.White;
                    break;
            }

        }

        /// <summary>
        /// Find the Stat Modifiers for a Race
        /// </summary>
        /// <param name="race"></param>
        /// <returns></returns>
        public Modifiers GetModifierValues(string race)
        {
            var modifier = new Modifiers();
            switch (race)
            {
                case "":
                    modifier.speed = "0";
                    break;

                case "Hill Dwarf":
                    modifier.con = 2;
                    modifier.wis = 1;
                    modifier.speed = "25";
                    break;

                case "Mountain Dwarf":
                    modifier.con = 2;
                    modifier.str = 2;
                    modifier.speed = "25";
                    break;

                case "High Elf":
                    modifier.dex = 2;
                    modifier.speed = "30";
                    elpPerception.Fill = Brushes.Black;
                    modifier.intl = 1;
                    break;

                case "Wood Elf":
                    modifier.dex = 2;
                    modifier.speed = "35";
                    elpPerception.Fill = Brushes.Black;
                    modifier.wis = 1;
                    break;

                case "Dark Elf":
                    modifier.dex = 2;
                    modifier.speed = "30";
                    elpPerception.Fill = Brushes.Black;
                    modifier.cha = 1;
                    break;

                case "Lightfoot Halfing":
                    modifier.dex = 2;
                    modifier.speed = "25";
                    modifier.cha = 1;
                    break;

                case "Stout Halfling":
                    modifier.dex = 2;
                    modifier.speed = "25";
                    modifier.con = 1;
                    break;

                case "Human":
                    modifier.str = 1;
                    modifier.dex = 1;
                    modifier.con = 1;
                    modifier.intl = 1;
                    modifier.wis = 1;
                    modifier.cha = 1;
                    modifier.speed = "30";                    
                    break;

                case "Dragonborn":
                    modifier.str = 2;
                    modifier.cha = 1;
                    modifier.speed = "30";
                    break;

                case "Forest Gnome":
                    modifier.intl = 2;
                    modifier.speed = "25";
                    elpIntSave.Fill = Brushes.Black;
                    elpWisSave.Fill = Brushes.Black;
                    elpChaSave.Fill = Brushes.Black;
                    modifier.dex = 1;
                    break;

                case "Rock Gnome":
                    modifier.intl = 2;
                    modifier.speed = "25";
                    elpIntSave.Fill = Brushes.Black;
                    elpWisSave.Fill = Brushes.Black;
                    elpChaSave.Fill = Brushes.Black;
                    modifier.con = 1;
                    break;

                case "Half-Elf":
                    modifier.cha = 2;
                    modifier.speed = "30";
                    break;

                case "Half-Orc":
                    modifier.str = 2;
                    modifier.con = 1;
                    elpIntimidation.Fill = Brushes.Black;
                    modifier.speed = "30";
                    break;

                case "Tiefling":
                    modifier.intl = 1;
                    modifier.cha = 2;
                    modifier.speed = "30";
                    break;
            }

            return modifier;
        }

        /// <summary>
        /// Remove modifiers from Race
        /// </summary>
        /// <param name="modifiers"></param>
        public void RemoveCurrentModifier(Modifiers modifiers)
        {
            txtStr.Text = Convert.ToString(Convert.ToInt32(txtStr.Text) - modifiers.str);
            txtDex.Text = Convert.ToString(Convert.ToInt32(txtDex.Text) - modifiers.dex);
            txtCon.Text = Convert.ToString(Convert.ToInt32(txtCon.Text) - modifiers.con);
            txtInt.Text = Convert.ToString(Convert.ToInt32(txtInt.Text) - modifiers.intl);
            txtWis.Text = Convert.ToString(Convert.ToInt32(txtWis.Text) - modifiers.wis);
            txtCha.Text = Convert.ToString(Convert.ToInt32(txtCha.Text) - modifiers.cha);
        }

        /// <summary>
        /// Add modifiers of newly selected race
        /// </summary>
        /// <param name="modifiers"></param>
        public void ApplyModifier(Modifiers modifiers)
        {
            txtStr.Text = Convert.ToString(Convert.ToInt32(txtStr.Text) + modifiers.str);
            txtDex.Text = Convert.ToString(Convert.ToInt32(txtDex.Text) + modifiers.dex);
            txtCon.Text = Convert.ToString(Convert.ToInt32(txtCon.Text) + modifiers.con);
            txtInt.Text = Convert.ToString(Convert.ToInt32(txtInt.Text) + modifiers.intl);
            txtWis.Text = Convert.ToString(Convert.ToInt32(txtWis.Text) + modifiers.wis);
            txtCha.Text = Convert.ToString(Convert.ToInt32(txtCha.Text) + modifiers.cha);
            txtSpeed.Text = modifiers.speed;
        }

        public void UpdateBackground(String Background)
        {

            switch (Background)
            {
                case "Soldier":
                    
                    break;
            }
        }

        public void UpdateClass(String Class)
        {

            switch (Class)
            {
                case "Barbarian":
                    
                    break;
            }
        }

        /// <summary>
        /// Loads deserialized character information
        /// </summary>
        /// <param name="Char">Character File to be loaded into application</param>
        public void LoadCharacter(Character Char)
        {
            txtCharName.Text = Char.CharacterName;
            txtLevel.Text = Char.CharacterLevel;
            listClass.Text = Char.CharacterClass;
            listBackground.Text = Char.CharacterBackground;
            txtPlayerName.Text = Char.PlayerName;
            listRace.Text = Char.CharacterRace;
            //"Mat Fuked uP" = Char.CharacterAlignment;
            //"Mat Fuked uP" = Char.CharacterExperiencePoints;
            txtSpeed.Text = Char.CharacterSpeed;
            txtProfBonus.Text = Char.ProficiencyBonus;
            txtStr.Text = Char.Strength;
            txtDex.Text = Char.Dexterity;
            txtCon.Text = Char.Constitution;
            txtInt.Text = Char.Intelligence;
            txtWis.Text = Char.Wisdom;
            txtCha.Text = Char.Charisma;
            elpStrSave.Fill = Char.ElpStrengthSavingThrow;
            elpDexSave.Fill = Char.ElpDexteritySavingThrow;
            elpConSave.Fill = Char.ElpConstitutionSavingThrow;
            elpIntSave.Fill = Char.ElpIntelligenceSavingThrow;
            elpWisSave.Fill = Char.ElpWisdomSavingThrow;
            elpChaSave.Fill = Char.ElpCharismaSavingThrow;
            elpAcrobatics.Fill = Char.ElpAcrobatics;
            elpAnimalHandling.Fill = Char.ElpAnimalHandling;
            elpArcana.Fill = Char.ElpArcana;
            elpAthletics.Fill = Char.ElpAthletics;
            elpDeception.Fill = Char.ElpDeception;
            elpHistory.Fill = Char.ElpHistory;
            elpInsight.Fill = Char.ElpInsight;
            elpIntimidation.Fill = Char.ElpIntimidation;
            elpInvestigation.Fill = Char.ElpInvestigation;
            elpMedicine.Fill = Char.ElpMedicine;
            elpNature.Fill = Char.ElpNature;
            elpPerception.Fill = Char.ElpPerception;
            elpPerformance.Fill = Char.ElpPerformance;
            elpPersuasion.Fill = Char.ElpPersuasion;
            elpReligion.Fill = Char.ElpReligion;
            elpSleightOfHand.Fill = Char.ElpSleightOfHand;
            elpStealth.Fill = Char.ElpStealth;
            elpSurvival.Fill = Char.ElpSurvival;
        }

        /// <summary>
        /// Enumerates through all grids to show only the selected grid while hiding all others
        /// </summary>
        /// <param name="grid"></param>
        public void GridVisiblity(Grid grid)
        {
            //string log = string.Empty;
            //int counter = 0;
            //foreach (Grid child in FindVisualChildren<Grid>(Main))
            //{
            //    counter++;
            //    log = string.Format("{0}{1}[{3}] {2}", log, Environment.NewLine, child.Name, counter);
            //}
            //MessageBox.Show(log);
            foreach (Grid child in FindVisualChildren<Grid>(Main))
            {
                if (child != grdTitle && child != FileMenuGreyBar) // || child == Main || string.IsNullOrWhiteSpace(child.Name) || child.Name == "MainGrid")
                {
                    if (child != grid)
                        child.Visibility = Visibility.Hidden;
                    else
                        child.Visibility = Visibility.Visible;
                }
            }
        }
        
        /// <summary>
        /// Enumerate through each child and child of child by object type
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="depObj">Name of object</param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        if (childOfChild != null && child is T)
                           yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Sets and Saves the Class 'Character' properties to the default "My Documents\DnD" folder & creates the "DnD" folder if it doesn't exist
        /// </summary>
        public void SaveFile()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\DnD\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path); //if Drive:\Users\<username>\Documents\DnD doesn't exist, create it
            
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.InitialDirectory = path;
            saveFile.DefaultExt = ".dnd";
            saveFile.Filter = "DnD Files (*.dnd)|*.dnd";
            if (saveFile.ShowDialog() == true)
            {
                saveFile.OverwritePrompt = true;
                Character character = new Character
                {
                    CharacterName = txtCharName.Text,
                    CharacterLevel = txtLevel.Text,
                    CharacterClass = listClass.Text,
                    CharacterBackground = listBackground.Text,
                    PlayerName = txtPlayerName.Text,
                    CharacterRace = listRace.Text,
                    CharacterAlignment = "Mat Fuked uP",
                    CharacterExperiencePoints = "Mat Fuked uP",
                    CharacterSpeed = txtSpeed.Text,
                    ProficiencyBonus = txtProfBonus.Text,
                    Strength = txtStr.Text,
                    Dexterity = txtDex.Text,
                    Constitution = txtCon.Text,
                    Intelligence = txtInt.Text,
                    Wisdom = txtWis.Text,
                    Charisma = txtCha.Text,
                    ElpStrengthSavingThrow = elpStrSave.Fill,
                    ElpDexteritySavingThrow = elpDexSave.Fill,
                    ElpConstitutionSavingThrow = elpConSave.Fill,
                    ElpIntelligenceSavingThrow = elpIntSave.Fill,
                    ElpWisdomSavingThrow = elpWisSave.Fill,
                    ElpCharismaSavingThrow = elpChaSave.Fill,
                    ElpAcrobatics = elpAcrobatics.Fill,
                    ElpAnimalHandling = elpAnimalHandling.Fill,
                    ElpArcana = elpArcana.Fill,
                    ElpAthletics = elpAthletics.Fill,
                    ElpDeception = elpDeception.Fill,
                    ElpHistory = elpHistory.Fill,
                    ElpInsight = elpInsight.Fill,
                    ElpIntimidation = elpIntimidation.Fill,
                    ElpInvestigation = elpInvestigation.Fill,
                    ElpMedicine = elpMedicine.Fill,
                    ElpNature = elpNature.Fill,
                    ElpPerception = elpPerception.Fill,
                    ElpPerformance = elpPerformance.Fill,
                    ElpPersuasion = elpPersuasion.Fill,
                    ElpReligion = elpReligion.Fill,
                    ElpSleightOfHand = elpSleightOfHand.Fill,
                    ElpStealth = elpStealth.Fill,
                    ElpSurvival = elpSurvival.Fill
                };
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                using (StreamWriter sw = new StreamWriter(saveFile.FileName))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, character);
                }
                _isModified = false;
            }
        }

        /// <summary>
        /// Calls 'LoadCharacter' to opens a saved 'Character' Class properties from a '.dnd' file
        /// </summary>
        public void OpenFile()
        {
            OpenFileDialog savedCharacters = new OpenFileDialog();
            savedCharacters.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\DnD\\";
            savedCharacters.DefaultExt = ".dnd";
            savedCharacters.Filter = "DnD Files (*.dnd)|*.dnd";
            if (savedCharacters.ShowDialog() == true)
            {
                using (StreamReader savedChar = new StreamReader(savedCharacters.FileName))
                {
                    string savedInfo = savedChar.ReadToEnd();
                    Character deserializedCharacter = JsonConvert.DeserializeObject<Character>(savedInfo);
                    LoadCharacter(deserializedCharacter);
                }
                UpdateAllAbilities();
                _isModified = false;
            }
        }
        #endregion

        #region GridFocus
        private void grdTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            grdTitle.Focus();
        }

        private void grdStats_MouseDown(object sender, MouseButtonEventArgs e)                         
        {
            grdStats.Focus();
        }

        private void grdEquipment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            grdEquipment.Focus();
        }


        #endregion


    }

    public static class Tools
    {
        /// <summary>
        /// Returns a String Value of the object with the specified propertyName
        /// </summary>
        /// <param name="obj">The specified object you want the string value of a property from</param>
        /// <param name="propertyName">The objects property name</param>
        /// <returns></returns>
        public static string GetPropertyVar(this object obj, string property)
        {
            return obj.GetType().GetProperties().Single(prop => prop.Name == property).GetValue(obj, null).ToString();
        }

        
    }
        
    public class OldValueStorage
    {
        public static string PreviousValue { get; set; }
    }

    /// <summary>
    /// Defines all properties that get saved 
    /// </summary>
    public class Character
    {
        public string CharacterName { get; set; }
        public string CharacterLevel { get; set; }
        public string CharacterClass { get; set; }
        public string CharacterBackground { get; set; }
        public string PlayerName { get; set; }
        public string CharacterRace { get; set; }
        public string CharacterAlignment { get; set; }
        public string CharacterExperiencePoints { get; set; }
        public string CharacterSpeed { get; set; }
        public string ProficiencyBonus { get; set; }
        public string Strength { get; set; }
        public string Dexterity { get; set; }
        public string Constitution { get; set; }
        public string Intelligence { get; set; }
        public string Wisdom { get; set; }
        public string Charisma { get; set; }
        public Brush ElpStrengthSavingThrow { get; set; }
        public Brush ElpDexteritySavingThrow { get; set; }
        public Brush ElpConstitutionSavingThrow { get; set; }
        public Brush ElpIntelligenceSavingThrow { get; set; }
        public Brush ElpWisdomSavingThrow { get; set; }
        public Brush ElpCharismaSavingThrow { get; set; }
        public Brush ElpAcrobatics { get; set; }
        public Brush ElpAnimalHandling { get; set; }
        public Brush ElpArcana { get; set; }
        public Brush ElpAthletics { get; set; }
        public Brush ElpDeception { get; set; }
        public Brush ElpHistory { get; set; }
        public Brush ElpInsight { get; set; }
        public Brush ElpIntimidation { get; set; }
        public Brush ElpInvestigation { get; set; }
        public Brush ElpMedicine { get; set; }
        public Brush ElpNature { get; set; }
        public Brush ElpPerception { get; set; }
        public Brush ElpPerformance { get; set; }
        public Brush ElpPersuasion { get; set; }
        public Brush ElpReligion { get; set; }
        public Brush ElpSleightOfHand { get; set; }
        public Brush ElpStealth { get; set; }
        public Brush ElpSurvival { get; set; }
    }

    /// <summary>
    /// Stat Modifiers for Races
    /// </summary>
    public class Modifiers
    {
        public int str { get; set; }
        public int dex { get; set; }
        public int con { get; set; }
        public int intl { get; set; }
        public int wis { get; set; }
        public int cha { get; set; }
        public string speed { get; set; }
    }
}


