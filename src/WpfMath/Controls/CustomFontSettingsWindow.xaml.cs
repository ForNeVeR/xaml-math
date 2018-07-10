using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media;
using System.Xml;
using static WpfMath.Utilities.CustomFontUtilities;
using System.Windows.Input;

namespace WpfMath.Controls
{
    /// <summary>
    /// Provides a way for creating a math font setting.
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/List_of_Unicode_characters"/>.
    /// <remarks>
    /// This control helps you to create a custom font settings for what you want WpfMath to render.
    /// </remarks>
    public partial class CustomFontSettingsWindow : Window
    {
        #region Variables for input and output
        /// <summary>
        /// Use this for loading a font setting to the <see cref="CustomFontSettingsControl"/>.
        /// </summary>
        private XmlDocument CustomTexFont = new XmlDocument();

        /// <summary>
        /// Use this for creating the custom settings' xml file.
        /// </summary>
        private XmlTextWriter xmlTxtWriter;

        /// <summary>
        /// Stores a groupname and a collection of its Unicode Characters.
        /// </summary>
        private Dictionary<string, ObservableCollection<UnicodeListItem>> symbolsgroups = new Dictionary<string, ObservableCollection<UnicodeListItem>>();

        private Dictionary<string, FontFamily> fontFamilies = new Dictionary<string, FontFamily>();

        /// <summary>
        /// Stores the font name and its local path.
        /// </summary>
        private Dictionary<string, string> FontDescrInfoDict = new Dictionary<string, string>();

        /// <summary>
        /// Stores the TextStyleMapping and a list of its Mapranges (Maprange->code, fontId, start).
        /// </summary>
        private Dictionary<string, List<Tuple<string, uint, uint>>> TextStyleMappingsRangesDict = new Dictionary<string, List<Tuple<string, uint, uint>>>();

        /// <summary>
        /// Stores the group name and ( the symbol name, its unicode codepoint and the font index).
        /// </summary>
        //private Dictionary<string, List<Tuple<string, int, int>>> groups_SymbolsDict = new Dictionary<string, List<Tuple<string, int, int>>>();

        private Dictionary<string, SymbolMapPreview> groups_SymViewDict = new Dictionary<string, SymbolMapPreview>();
        private Dictionary<string, TextStyleMapPreview> groups_TxtStyViewDict = new Dictionary<string, TextStyleMapPreview>();
        private Dictionary<string, FontDescriptionPreview> groups_FontDescrDict = new Dictionary<string, FontDescriptionPreview>();

        #endregion

        /// <summary>
        /// Initializes a new <see cref="CustomFontSettingsWindow"/>.
        /// </summary>
        public CustomFontSettingsWindow()
        {
            InitializeComponent();

            InitializeControlProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeControlProperties()
        {
            CustomTexFont = new XmlDocument();
            xmlTxtWriter = null;
            symbolsgroups = new Dictionary<string, ObservableCollection<UnicodeListItem>>();
            fontFamilies = new Dictionary<string, FontFamily>();
            FontDescrInfoDict = new Dictionary<string, string>();
            groups_SymViewDict = new Dictionary<string, SymbolMapPreview>();
            groups_TxtStyViewDict = new Dictionary<string, TextStyleMapPreview>();
            groups_FontDescrDict = new Dictionary<string, FontDescriptionPreview>();
            //fontFamilies.Add("STIX General", new FontFamily(new Uri("pack://application:,,,/Fonts/"), "./#STIXGeneral"));
            //foreach (var item in Enum.GetValues(typeof(FontType)))
            //{
            //    fontFamilies.Add(item, GetFontFamily(item));
            //}//STIX General
            InitializeFontChars("Asana Math");
        }

        #region Custom Font Settings Exporting Methods
        /// <summary>
        /// Exports the values of the input fields to an xml file.
        /// </summary>
        /// <param name="filepath">The output path of the custom font settings.</param>
        private void ExportFontInfo(string filepath)
        {
            //using (FileStream fs=new FileStream(filepath, FileMode.Create,FileAccess.Write))
            //{
            xmlTxtWriter = new XmlTextWriter(filepath, System.Text.Encoding.UTF8);
            xmlTxtWriter.WriteStartDocument(true);
            xmlTxtWriter.Formatting = Formatting.Indented;
            xmlTxtWriter.Indentation = 3;
            xmlTxtWriter.WriteStartElement("TeXFont");
            xmlTxtWriter.WriteWhitespace("\n\n");
            PutParameters();
            PutGeneralSettings();
            PutTextStyleMappings();

            PutDefaultTextStyleMapping();

            PutSymbolMappings();

            PutFontDescriptions();

            xmlTxtWriter.WriteEndElement();
            xmlTxtWriter.WriteWhitespace("\n\n");
            xmlTxtWriter.WriteEndDocument();
            xmlTxtWriter.WriteWhitespace("\n\n");
            xmlTxtWriter.Close();

        }

        /// <summary>
        /// Adds the "Parameters" Node to the output XmlTextWriter.
        /// </summary>
        private void PutParameters()
        {
            xmlTxtWriter.WriteComment("general parameters used in the TeX algorithms");
            xmlTxtWriter.WriteWhitespace("\n\n");
            xmlTxtWriter.WriteStartElement("Parameters");

            #region Input Params
            double para = Num1Txb.Value ?? 0;
            double para1 = Num2Txb.Value ?? 0;
            double para2 = Num3Txb.Value ?? 0;
            double para3 = Denom1Txb.Value ?? 0;
            double para4 = Denom2Txb.Value ?? 0;
            double para5 = Denom3Txb.Value ?? 0;
            double para6 = Sup1Txb.Value ?? 0;
            double para7 = Sup2Txb.Value ?? 0;
            double para8 = Sup3Txb.Value ?? 0;
            double para9 = Sub1Txb.Value ?? 0;
            double para10 = Sub2Txb.Value ?? 0;
            double para11 = Sub3Txb.Value ?? 0;
            double para12 = SupdropTxb.Value ?? 0;
            double para13 = SubdropTxb.Value ?? 0;
            double para14 = AxisHeightTxb.Value ?? 0;
            double para15 = DefaultRuleThicknessTxb.Value ?? 0;
            double para16 = BigOpSpacing1Txb.Value ?? 0;
            double para17 = BigOpSpacing2Txb.Value ?? 0;
            double para18 = BigOpSpacing3Txb.Value ?? 0;
            double para19 = BigOpSpacing4Txb.Value ?? 0;
            double para20 = BigOpSpacing5Txb.Value ?? 0;
            #endregion

            xmlTxtWriter.WriteStartAttribute("num1");
            xmlTxtWriter.WriteString(para.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("num2");
            xmlTxtWriter.WriteString(para1.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("num3");
            xmlTxtWriter.WriteString(para2.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("denom1");
            xmlTxtWriter.WriteString(para3.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("denom2");
            xmlTxtWriter.WriteString(para4.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("denom3");
            xmlTxtWriter.WriteString(para5.ToString());
            xmlTxtWriter.WriteEndAttribute();

            xmlTxtWriter.WriteStartAttribute("sup1");
            xmlTxtWriter.WriteString(para6.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("sup2");
            xmlTxtWriter.WriteString(para7.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("sup3");
            xmlTxtWriter.WriteString(para8.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("sub1");
            xmlTxtWriter.WriteString(para9.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("sub2");
            xmlTxtWriter.WriteString(para10.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("sub3");
            xmlTxtWriter.WriteString(para11.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("supdrop");
            xmlTxtWriter.WriteString(para12.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("subdrop");
            xmlTxtWriter.WriteString(para13.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("axisheight");
            xmlTxtWriter.WriteString(para14.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("defaultrulethickness");
            xmlTxtWriter.WriteString(para15.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("bigopspacing1");
            xmlTxtWriter.WriteString(para16.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("bigopspacing2");
            xmlTxtWriter.WriteString(para17.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("bigopspacing3");
            xmlTxtWriter.WriteString(para18.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("bigopspacing4");
            xmlTxtWriter.WriteString(para19.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteStartAttribute("bigopspacing5");
            xmlTxtWriter.WriteString(para20.ToString());
            xmlTxtWriter.WriteEndAttribute();
            xmlTxtWriter.WriteEndElement();

            xmlTxtWriter.WriteWhitespace("\n\n");
        }

        /// <summary>
        /// Adds the "GeneralSettings" Node to the output XmlTextWriter.
        /// </summary>
        private void PutGeneralSettings()
        {
            xmlTxtWriter.WriteComment("general settings");
            xmlTxtWriter.WriteWhitespace("\n\n");
            xmlTxtWriter.WriteStartElement("GeneralSettings");

            xmlTxtWriter.WriteStartAttribute("mufontid");
            xmlTxtWriter.WriteString(GSMufontid.Value.ToString());
            xmlTxtWriter.WriteEndAttribute();

            xmlTxtWriter.WriteStartAttribute("spacefontid");
            xmlTxtWriter.WriteString(GSSpacefontid.Value.ToString());
            xmlTxtWriter.WriteEndAttribute();
            //xmlTxtWriter.WriteWhitespace("\t");

            xmlTxtWriter.WriteStartAttribute("scriptfactor");
            xmlTxtWriter.WriteString(GSScriptfactor.Value.ToString());
            xmlTxtWriter.WriteEndAttribute();

            xmlTxtWriter.WriteStartAttribute("scriptscriptfactor");
            xmlTxtWriter.WriteString(GSScriptscriptfactor.Value.ToString());
            xmlTxtWriter.WriteEndAttribute();

            xmlTxtWriter.WriteEndElement();
            xmlTxtWriter.WriteWhitespace("\n\n");
        }

        /// <summary>
        /// Adds the "TextStyleMappings" Node to the output XmlTextWriter.
        /// </summary>
        private void PutTextStyleMappings()
        {
            xmlTxtWriter.WriteStartElement("TextStyleMappings");
            xmlTxtWriter.WriteWhitespace("\n");
            foreach (var item in TextStyleMappingsRangesDict)
            {
                xmlTxtWriter.WriteStartElement("TextStyleMapping");
                xmlTxtWriter.WriteStartAttribute("name");
                xmlTxtWriter.WriteString(item.Key);
                xmlTxtWriter.WriteEndAttribute();
                foreach (var item1 in item.Value)
                {
                    xmlTxtWriter.WriteStartElement("MapRange");
                    xmlTxtWriter.WriteStartAttribute("code");
                    xmlTxtWriter.WriteString(item1.Item1);
                    xmlTxtWriter.WriteEndAttribute();
                    xmlTxtWriter.WriteStartAttribute("fontId");
                    xmlTxtWriter.WriteString(item1.Item2.ToString());
                    xmlTxtWriter.WriteEndAttribute();
                    xmlTxtWriter.WriteStartAttribute("start");
                    xmlTxtWriter.WriteString(item1.Item3.ToString());
                    xmlTxtWriter.WriteEndAttribute();
                    xmlTxtWriter.WriteEndElement(); xmlTxtWriter.WriteWhitespace("\n");
                }
                xmlTxtWriter.WriteEndElement(); xmlTxtWriter.WriteWhitespace("\n");
            }
            xmlTxtWriter.WriteEndElement(); xmlTxtWriter.WriteWhitespace("\n");
        }

        private void PutDefaultTextStyleMapping()
        {
            string str = @"the default text style mappings, used when no text style is specified or when a specific mapping \n 
                         for a text style is not defined (e.g. \'numbers\' and \'small\' in \'mathcal\') ";
            xmlTxtWriter.WriteComment(str);
            xmlTxtWriter.WriteWhitespace("\n\n");
            xmlTxtWriter.WriteStartElement("DefaultTextStyleMapping");
            xmlTxtWriter.WriteWhitespace("\n\n");
            xmlTxtWriter.WriteStartElement("MapStyle");

            xmlTxtWriter.WriteEndElement();

            xmlTxtWriter.WriteEndElement();
            xmlTxtWriter.WriteWhitespace("\n\n");
        }

        /// <summary>
        /// Adds the "SymbolMappings" Node to the output XmlTextWriter.
        /// </summary>
        private void PutSymbolMappings()
        {
            xmlTxtWriter.WriteStartElement("SymbolMappings");
            xmlTxtWriter.WriteWhitespace("\n\n");

            foreach (var groupviewitem in groups_SymViewDict)
            {
                xmlTxtWriter.WriteComment(groupviewitem.Key);
                xmlTxtWriter.WriteWhitespace("\n\n");
                foreach (var symbolviewitem in groupviewitem.Value.SymbolValues)
                {
                    AddSymbolMappingNode(symbolviewitem.Item1, symbolviewitem.Item2, symbolviewitem.Item3);
                }

                xmlTxtWriter.WriteWhitespace("\n\n");
            }

            xmlTxtWriter.WriteEndElement();
            xmlTxtWriter.WriteWhitespace("\n\n");
        }

        /// <summary>
        /// Creates a "SymbolMapping" node with the specified <paramref name="nameStr"/>, <paramref name="unicodeDecimal"/> and <paramref name="fontIdNum"/>.
        /// </summary>
        /// <param name="nameStr">The MathFont Name.</param>
        /// <param name="unicodeDecimal">The Unicode Copepoint.</param>
        /// <param name="fontIdNum">The Font Identification index.</param>
        private void AddSymbolMappingNode(string nameStr, int unicodeDecimal, int fontIdNum)
        {
            xmlTxtWriter.WriteStartElement("SymbolMapping");
            xmlTxtWriter.WriteStartAttribute("name");
            xmlTxtWriter.WriteString(nameStr);
            xmlTxtWriter.WriteEndAttribute();

            xmlTxtWriter.WriteStartAttribute("ch");
            xmlTxtWriter.WriteString(unicodeDecimal.ToString());
            xmlTxtWriter.WriteEndAttribute();

            xmlTxtWriter.WriteStartAttribute("fontId");
            xmlTxtWriter.WriteString(fontIdNum.ToString());
            xmlTxtWriter.WriteEndAttribute();

            xmlTxtWriter.WriteEndElement();
        }

        private void PutFontDescriptions()
        {
            xmlTxtWriter.WriteStartElement("FontDescriptions");
            xmlTxtWriter.WriteWhitespace("\n\n");
            foreach (var fontUIItem in FontDescrGrid.Children.OfType<Expander>())
            {
                string fontUIItemName = ((string)fontUIItem.Header).Split('>')[1];
                string gontUiItemId = ((string)fontUIItem.Header).Split('-')[0];

            }
            xmlTxtWriter.WriteEndElement();
            xmlTxtWriter.WriteWhitespace("\n\n");
        }

        #endregion

        #region Character Previewing Region
        /* Kern---->Used to enhance the spacing between characters
         * Ligature--->A character consisting of two or more joined letters, e.g. æ. 
         */
        
        /// <summary>
        /// Adds an <see cref="ObservableCollection&lt;UnicodeListItem&gt;"/> to a specified <paramref name="groupname"/> based on the range.
        /// </summary>
        /// <param name="groupname">The name of the group.</param>
        /// <param name="low">The lower bound.</param>
        /// <param name="high">The upper bound.</param>
        /// <param name="fontype">The type of font.</param>
        private void SetupGroup(string groupname, int low, int high, string fontype = "Cambria Math Regular")
        {
            ObservableCollection<UnicodeListItem> grouplist = GetFontChars(fontype,low ,high);
            
            if (symbolsgroups.ContainsKey(groupname) == false)
            {
                symbolsgroups.Add(groupname, grouplist);
                if (groups_SymViewDict.ContainsKey(groupname) == false)
                {
                    groups_SymViewDict.Add(groupname, new SymbolMapPreview());
                    ComboBoxItem cboxItem = new ComboBoxItem
                    {
                        Content = groupname,
                        Height = 20
                    };
                    cboxItem.Selected += GroupComboBoxItem_Selected;
                    GroupsComboBox.Items.Add(cboxItem);
                }
            }

        }

        
        private void InitializeFontChars(string fontName)
        {
            if (fontFamilies.ContainsKey(fontName))
            {

            }
            //fontFamilies.Add(fontName, GetFontFamily(fontName));

            #region Setups
            //SetupGroup("Mathematical Operators & Number Forms", 0x2150, 0x218F);
            //SetupGroup("Mathematical Operators & Number Forms 1", 0x2200, 0x22FF); //Mathematical Operators    
            //SetupGroup("Miscellaneous Mathematical", 0x27C0, 0x27EF); // Symbols-A
            //SetupGroup("Basic Latin", 0x021, 0x7E);
            //SetupGroup("Latin-1 Supplement", 0x0A1, 0xAC);
            //SetupGroup("Latin-1 Supplement 1", 0x0AE, 0xFF);
            //SetupGroup("Latin Extended", 0x100, 0x17F); //A
            //SetupGroup("Latin Extended 1", 0x180, 0x237); //B
            //SetupGroup("Punctuation & Diacritical Marks", 0x2000, 0x206F); //General Punctuation
            //SetupGroup("Punctuation & Diacritical Marks 1", 0x2B0, 0x2FF); //Spacing Modifier Letters
            //SetupGroup("Punctuation & Diacritical Marks 2", 0x300, 0x36F); //Combining Diacritical Marks
            //SetupGroup("Greek and Coptic", 0x370, 0x3FF);
            //SetupGroup("Cyrillic", 0x400, 0x4FF);
            //SetupGroup("Currency Symbols & Phonetic Extensions", 0x20A0, 0x20CF); //Currency Symbols
            //SetupGroup("Currency Symbols & Phonetic Extensions 1", 0x1D00, 0x1D7F); //Phonetic Extensions
            //SetupGroup("Currency Symbols & Phonetic Extensions 2", 0x1D80, 0x1DBF); //Phonetic Extensions Supplement
            //SetupGroup("Latin Extended", 0x1E00, 0x1EFF); //Latin Extended Additional
            //SetupGroup("Punctuation & Diacritical Marks", 0x20D0, 0x20FF); //Combining Diacritical Marks for Symbols
            //SetupGroup("Letterlike Symbols", 0x2100, 0x214F);
            //SetupGroup("Arrows", 0x2190, 0x21FF);
            //SetupGroup("Miscellaneous", 0x2300, 0x23FF); //Miscellaneous Technical
            //SetupGroup("Miscellaneous 1", 0x2400, 0x243F); //Control Pictures
            //SetupGroup("Enclosed Alphanumerics", 0x2460, 0x24FF);
            //SetupGroup("Shapes", 0x2700, 0x27BF); //Dingbats 
            //SetupGroup("Shapes 1", 0x2500, 0x257F); //Box Drawing
            //SetupGroup("Shapes 2", 0x25A0, 0x25FF); //Geometric Shapes
            //SetupGroup("Miscellaneous", 0x2600, 0x26FF); //Miscellaneous Symbols
            //SetupGroup("Arrows", 0x27F0, 0x27FF);
            //SetupGroup("Arrows 1", 0x2900, 0x297F);
            //SetupGroup("Miscellaneous Mathematical", 0x2980, 0x29FF); // Symbols-B
            //SetupGroup("Supplemental Mathematical Operators", 0x2A00, 0x2AFF);
            //SetupGroup("Miscellaneous 2", 0x2B12, 0x2B54); //Miscellaneous Symbols and Arrows
            //SetupGroup("Miscellaneous 3", 0xFB00, 0xFB4F); //Alphabetic Presentation Forms

            #endregion

            //SetupGroup(fontName, 35, 980, ValidateFontName(fontName));
            //RowDefinition rowdef = new RowDefinition();
            //int a = FontDescrGrid.RowDefinitions.Count;
        }

        #endregion

        #region Font Settings Importing Methods
        private void ImportFontInfo(XmlDocument xmlDoc)
        {
            if (xmlDoc.DocumentElement.Name == "TeXFont" && xmlDoc.DocumentElement.HasChildNodes == true)
            {
                foreach (XmlNode item in xmlDoc.DocumentElement.ChildNodes)
                {
                    if (item.Name == "Parameters")
                    {
                        TakeParametersData(item);
                    }
                    if (item.Name == "GeneralSettings")
                    {
                        TakeGeneralSettingsData(item);
                    }
                    if (item.Name == "TextStyleMappings")
                    {
                        TakeTextStyleMappingsData(item);
                    }
                    if (item.Name == "DefaultTextStyleMapping")
                    {

                    }
                    if (item.Name == "SymbolMappings")
                    {
                        TakeSymbolMappingsData(item);
                    }
                    if (item.Name == "FontDescriptions")
                    {

                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the attributes of the "Parameters" node from the <paramref name="inputNode"/>.
        /// </summary>
        /// <param name="inputNode">The node containing the parameters.</param>
        private void TakeParametersData(XmlNode inputNode)
        {
            if (inputNode.Name == "Parameters" && inputNode.Attributes.Count > 0)
            {
                foreach (XmlAttribute item in inputNode.Attributes)
                {
                    switch (item.Name)
                    {
                        case "num1":
                            {
                                Num1Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "num2":
                            {
                                Num2Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "num3":
                            {
                                Num3Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "denom1":
                            {
                                Denom1Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "denom2":
                            {
                                Denom2Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "denom3":
                            {
                                Denom3Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "sup1":
                            {
                                Sup1Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "sup2":
                            {
                                Sup2Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "sup3":
                            {
                                Sup3Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "sub1":
                            {
                                Sub1Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "sub2":
                            {
                                Sub2Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "sub3":
                            {
                                Sub3Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "supdrop":
                            {
                                SupdropTxb.Text = item.Value ?? "0";
                                break;
                            }
                        case "subdrop":
                            {
                                SubdropTxb.Text = item.Value ?? "0";
                                break;
                            }
                        case "bigopspacing1":
                            {
                                BigOpSpacing1Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "bigopspacing2":
                            {
                                BigOpSpacing2Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "bigopspacing3":
                            {
                                BigOpSpacing3Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "bigopspacing4":
                            {
                                BigOpSpacing4Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "bigopspacing5":
                            {
                                BigOpSpacing5Txb.Text = item.Value ?? "0";
                                break;
                            }
                        case "axisheight":
                            {
                                AxisHeightTxb.Text = item.Value ?? "0";
                                break;
                            }
                        case "defaultrulethickness":
                            {
                                DefaultRuleThicknessTxb.Text = item.Value ?? "0";
                                break;
                            }
                        default:
                            break;
                    }

                }
            }
        }

        /// <summary>
        /// Retrieves the attributes of the "GeneralSettings" node from the <paramref name="inputNode"/>.
        /// </summary>
        /// <param name="inputNode">The node containing the general settings.</param>
        private void TakeGeneralSettingsData(XmlNode inputNode)
        {
            if (inputNode.Name == "GeneralSettings" && inputNode.Attributes.Count > 0)
            {
                foreach (XmlAttribute item in inputNode.Attributes)
                {
                    switch (item.Name)
                    {
                        case "mufontid":
                            {
                                GSMufontid.Text = item.Value ?? "0";
                                break;
                            }
                        case "spacefontid":
                            {
                                GSSpacefontid.Text = item.Value ?? "0";
                                break;
                            }
                        case "scriptfactor":
                            {
                                GSScriptfactor.Text = item.Value ?? "0";
                                break;
                            }
                        case "scriptscriptfactor":
                            {
                                GSScriptscriptfactor.Text = item.Value ?? "0";
                                break;
                            }
                        default:
                            break;
                    }

                }
            }
        }

        /// <summary>
        /// Retrieves the contents of the "TextStyleMappings" node from the <paramref name="inputNode"/>.
        /// </summary>
        /// <param name="inputNode">The node containing the text style mappings.</param>
        private void TakeTextStyleMappingsData(XmlNode inputNode)
        {
            if (inputNode.Name == "TextStyleMappings" && inputNode.ChildNodes.Count > 0)
            {
                foreach (XmlNode item in inputNode.ChildNodes)
                {
                    if (item.Name == "TextStyleMapping")
                    {
                        TakeTextStyleMappingData( item);
                    }
                }

            }
        }

        /// <summary>
        /// Retrieves the contents of the "TextStyleMapping" node from the <paramref name="inputNode"/>.
        /// </summary>
        /// <param name="inputNode">The node containing the text style mapping.</param>
        private void TakeTextStyleMappingData(XmlNode inputNode)
        {
            if (inputNode.Name == "TextStyleMapping")
            {
                string strTN = inputNode.Attributes["name"].Value;
                TextStyleMappingsRangesDict.Add(strTN, new List<Tuple<string, uint, uint>>());
                groups_TxtStyViewDict.Add(strTN, new TextStyleMapPreview());
                foreach (XmlNode item in inputNode.ChildNodes)
                {
                    if (item.Name== "MapRange")
                    {
                        string cStr = item.Attributes["code"].Value ?? "null";
                        string fiStr= item.Attributes["fontId"].Value ?? "0";
                        string sStr= item.Attributes["start"].Value ?? "0";
                        groups_TxtStyViewDict[strTN].TextStyleMapRanges.Add(new Tuple<string, uint, uint>(cStr, uint.Parse(fiStr), uint.Parse(sStr)));
                    }
                }

            }
        }

        private void TakeDefaultTextStyleMappingsData(XmlNode inputNode)
        {
            if (inputNode.Name == "DefaultTextStyleMappings" && inputNode.Attributes.Count > 0)
            {

            }
        }

        /// <summary>
        /// Retrieves the contents of the "SymbolMappings" node from the <paramref name="inputNode"/>.
        /// </summary>
        /// <param name="inputNode">The node containing the symbol mappings.</param>
        private void TakeSymbolMappingsData(XmlNode inputNode)
        {
            if (inputNode.Name == "SymbolMappings" && inputNode.ChildNodes.Count > 0)
            {
                string curGroup = "";
                foreach (XmlNode item in inputNode.ChildNodes)
                {
                    if (item.NodeType == XmlNodeType.Comment)
                    {
                        curGroup = item.Value;
                        symbolsgroups.Add(curGroup, new ObservableCollection<UnicodeListItem>());
                        groups_SymViewDict.Add(curGroup, new SymbolMapPreview());
                    }
                    if (item.NodeType == XmlNodeType.Element && item.Name == "SymbolMapping" && item.Attributes.Count > 0 && symbolsgroups.ContainsKey(curGroup) == true)
                    {
                        groups_SymViewDict[curGroup].SymbolValues.Add(new Tuple<string, int, int>(item.Attributes["name"].Value, int.Parse(item.Attributes["ch"].Value), int.Parse(item.Attributes["fontId"].Value)));
                    }
                    if (item.NodeType == XmlNodeType.Element && item.Name == "SymbolMapping" && item.Attributes.Count > 0 && symbolsgroups.ContainsKey(curGroup) == false)
                    {
                        curGroup = $"ungrouped{symbolsgroups.Count}";
                        symbolsgroups.Add(curGroup, new ObservableCollection<UnicodeListItem>());
                        groups_SymViewDict.Add(curGroup, new SymbolMapPreview());
                        groups_SymViewDict[curGroup].SymbolValues.Add(new Tuple<string, int, int>(item.Attributes["name"].Value, int.Parse(item.Attributes["ch"].Value), int.Parse(item.Attributes["fontId"].Value)));
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }


        private void TakeFontDescriptionsData(XmlNode inputNode)
        {
            if (inputNode.Name == "FontDescriptions" && inputNode.Attributes.Count > 0)
            {

            }
        }

        private void TakeFontData(XmlNode inputNode)
        {
            if (inputNode.Name == "Font" && inputNode.Attributes.Count > 0)
            {

            }
        }

        #endregion

        #region UI Event Handlers
        private ComboBoxItem symbolCBxItem = null;
        private ComboBoxItem fontDescCBxItem = null;
        private void GenerateSettings_Click(object o, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog()
            {
                DefaultExt = ".xml",
                Filter = "Xml File(*.xml)|*.xml"
            };
            if (dlg.ShowDialog() == true)
            {
                ExportFontInfo(dlg.FileName);
            }
        }

        private void LoadSettings_Click(object o, RoutedEventArgs e)
        {
            InitializeControlProperties();
            OpenFileDialog dlg = new OpenFileDialog
            {
                Title = "Select The Font Settings File",
                Filter = "Xml File(*.xml)|*.xml",
                Multiselect = false
            };
            if (dlg.ShowDialog() == true)
            {
                CustomTexFont = new XmlDocument();
                CustomTexFont.Load(dlg.FileName);
                ImportFontInfo(CustomTexFont);
            }
        }


        private void AddTextStyleMappings_Click(object o, RoutedEventArgs e)
        {
            RowDefinition rowdef = new RowDefinition();
            TextStyleMapDialog dlg = new TextStyleMapDialog()
            {
                Owner=this
            };
            if (dlg.ShowDialog()==true)
            {
                
                TextStyleMapPreview mapPreview = new TextStyleMapPreview
                {
                    TextStyleMapName = dlg.TextStyleMapName,
                    TextStyleMapRanges=dlg.TextStyleMapRanges
                };
                mapPreview.SetValue(Grid.RowProperty, TxtStMapsGrid.RowDefinitions.Count);
                TxtStMapsGrid.RowDefinitions.Add(rowdef);
                TxtStMapsGrid.Children.Add(mapPreview);
            }
            
        }

        //for adding a new group to the symbols
        private void AddGroupSymbols_Click(object o, RoutedEventArgs e)
        {
            SymbolGroupDialog dlg = new SymbolGroupDialog();
            if (dlg.ShowDialog()==true)
            {
                string symstr = dlg.SymbolGroupName;
                if (groups_SymViewDict.ContainsKey(symstr)==false)
                {
                    ComboBoxItem cbitem = new ComboBoxItem
                    {
                        Content = $"{symstr}",
                        Height = 20
                    };
                    cbitem.Selected += GroupComboBoxItem_Selected;
                    //if (groups_SymViewDict.ContainsKey(symstr) == false)
                    GroupsComboBox.Items.Add(cbitem);
                    groups_SymViewDict.Add(symstr, new SymbolMapPreview());

                }
                
            }
            
        }

        private void AddFontDescriptions_Click(object o, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Title = "Select Font File",
                Filter = "FontFile(*.otf;*.ttc;*.ttf;)|*.otf;*.ttc;*.ttf"
            };
            if (dlg.ShowDialog() == true)
            {
                ComboBoxItem cbitem = new ComboBoxItem
                {
                    Content=$"{FontsComboBox.Items.Count}--->{dlg.SafeFileName}",
                    Height=20
                };
                cbitem.Selected += FontDescriptionComboBoxItem_Selected;
                FontsComboBox.Items.Add(cbitem);
                FontDescrInfoDict.Add(dlg.SafeFileName, dlg.FileName);
                Init(ValidateFontName(dlg.SafeFileName), dlg.FileName);
                var fontDescPrevCtrl = new FontDescriptionPreview(GetFontChars(ValidateFontName(dlg.SafeFileName), 0x27F0, 0x27FF))
                {
                    FontName = ValidateFontName(dlg.SafeFileName),
                    FontPath = dlg.FileName
                };
                groups_FontDescrDict.Add(dlg.SafeFileName,fontDescPrevCtrl);
                fontFamilies.Add(dlg.SafeFileName, GetFontFamily());
                
            }
        }    

        private void GroupComboBoxItem_Selected(object o, RoutedEventArgs e)
        {
            ComboBoxItem cbox = o as ComboBoxItem;
            symbolCBxItem = cbox;
            string str = cbox.Content.ToString();
            if (symbolsgroups.ContainsKey(str))
            {
                symbolsContainer.Content = groups_SymViewDict[str];
                CurrentGroupCountTxb.Text = $"{groups_SymViewDict[str].SymbolsAdded} / {groups_SymViewDict[str].SymbolItems.Count.ToString()}";
            }
        }

        //Use this event for selections of listbox items in the Font Description Expander Region.
        private void FontDescriptionComboBoxItem_Selected(object o, RoutedEventArgs e)
        {
            ComboBoxItem cboxItm = o as ComboBoxItem;
            fontDescCBxItem = cboxItm;
            string str = cboxItm.Content.ToString().Split('>')[1];
            if (groups_FontDescrDict.ContainsKey(str))
            {
                fontsContainer.Content = groups_FontDescrDict[str];
            }
        }

        private void SettingsWindow_KeyDown(object o, KeyEventArgs e)
        {
            string str= symbolCBxItem?.Content.ToString();
            string str1 = fontDescCBxItem?.Content.ToString();
            if (Keyboard.Modifiers == ModifierKeys.Control&&e.Key==Key.G)
            {
                //This is used to send a font description list item to the selected symbol group.

                if (str!=null&&str1!=null&& groups_SymViewDict.ContainsKey(str)&& groups_FontDescrDict.ContainsKey(str1))
                {
                    groups_SymViewDict[str].SymbolItems.Add(groups_FontDescrDict[str1].CurrentSelectedItem);
                }
            }

            if (Keyboard.Modifiers==ModifierKeys.Control&&e.Key==Key.S)
            {
                SaveFileDialog dlg = new SaveFileDialog()
                {
                    DefaultExt = ".xml",
                    Filter = "Xml File(*.xml)|*.xml"
                };
                if (dlg.ShowDialog() == true)
                {
                    ExportFontInfo(dlg.FileName);
                }
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
            {
                InitializeControlProperties();
                OpenFileDialog dlg = new OpenFileDialog
                {
                    Title = "Select The Font Settings File",
                    Filter = "Xml File(*.xml)|*.xml",
                    Multiselect = false
                };
                if (dlg.ShowDialog() == true)
                {
                    CustomTexFont = new XmlDocument();
                    CustomTexFont.Load(dlg.FileName);
                    ImportFontInfo(CustomTexFont);
                }
            }



        }
        #endregion

    }
}
