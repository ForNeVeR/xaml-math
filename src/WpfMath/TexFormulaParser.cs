using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using WpfMath.Atoms;
using WpfMath.Exceptions;
using Ionic.Zip;
using System.IO;
using System.Xml;
using System.Text;
using WpfMath.Utils;
using System.Text.RegularExpressions;
using WpfMath.Parsers;

namespace WpfMath
{
    // TODO: Put all error strings into resources.
    // TODO: Use TextReader for lexing.
    public class TexFormulaParser
    {
        // Special characters for parsing
        private const char escapeChar = '\\';

        private const char leftGroupChar = '{';
        private const char rightGroupChar = '}';
        private const char leftBracketChar = '[';
        private const char rightBracketChar = ']';

        private const char subScriptChar = '_';
        private const char superScriptChar = '^';
        private const char primeChar = '\'';

        // Information used for parsing
        private static HashSet<string> commands;
        private static IList<string> symbols;
        private static IList<string> delimeters;
        private static HashSet<string> TextStyles;
        private static readonly IDictionary<string, Func<SourceSpan, TexFormula>> predefinedFormulas =
            new Dictionary<string, Func<SourceSpan, TexFormula>>();
        private static IDictionary<string, Color> predefinedColors;

        private static readonly string[][] delimiterNames =
        {
            new[] { "lbrace", "rbrace" },
            new[] { "lsqbrack", "rsqbrack" },
            new[] { "lbrack", "rbrack" },
            new[] { "downarrow", "downarrow" },
            new[] { "uparrow", "uparrow" },
            new[] { "updownarrow", "updownarrow" },
            new[] { "Downarrow", "Downarrow" },
            new[] { "Uparrow", "Uparrow" },
            new[] { "Updownarrow", "Updownarrow" },
            new[] { "vert", "vert" },
            new[] { "Vert", "Vert" }
        };
        
        /// <summary>
        /// Gets or sets the number of declared fonts.
        /// </summary>
        public int DeclaredFonts { get; private set; }

        /// <summary>
        /// Gets or sets the default text style mapping for the formula parser.
        /// <para/>
        /// Item1->Digits                   <para/>
        /// Item2->EnglishCapitals          <para/>
        /// Item3->EnglishSmall             <para/>
        /// Item4->GreekCapitals            <para/>
        /// Item5->GreekSmall
        /// </summary>
        public Tuple<string, string, string, string, string> DefaultTextStyleMapping { get; private set; }
        /// <summary>
        /// Gets or sets the directory containing the font file(s).
        /// </summary>
        public string FormulaFontFilesDirectory { get; private set; }
        /// <summary>
        /// Gets or sets the path to the font information file.
        /// </summary>
        public string FormulaFontInfoFilePath { get; private set; }
        /// <summary>
        /// Gets or sets the path to the settings for the font.
        /// </summary>
        public string FormulaSettingsFilePath { get; private set; }
        /// <summary>
        /// Gets or sets the path to the font symbols name-type declaration file.
        /// </summary>
        public string FormulaSymbolsFilePath { get; private set; }
        
        /// <summary>
        /// Indicates whether the font(s) is(are) an internal or external font(s).
        /// </summary>
        public bool AreFontsInternal { get; private set; }
        
        /// <summary>
        /// Gets or sets the path to the font settings file in this library.
        /// </summary>
        public Dictionary<string, string> AvailableFonts { get; private set; }
 
        /// <summary>
        /// Initializes a new <see cref="TexFormulaParser"/>.
        /// </summary>
        public TexFormulaParser()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new <see cref="TexFormulaParser"/> with the specified properties.
        /// </summary>
        public TexFormulaParser(int declaredFonts, string fontFilesDir, string fontInfoFilePath, string formulaSettingsFilePath, string symbolsFilePath, bool isInternal)
        {
            Initialize(declaredFonts, fontFilesDir, fontInfoFilePath, formulaSettingsFilePath, symbolsFilePath, isInternal);
        }
        
        internal static string[][] DelimiterNames
        {
            get { return delimiterNames; }
        }

        private void Initialize(int declaredFonts=4,string fontFilesDir= "Fonts/Default/",string fontInfoFilePath= "WpfMath.Data.DefaultTexFont.xml",string formulaSettingsFilePath= "WpfMath.Data.TexFormulaSettings.xml",string symbolsFilePath= "WpfMath.Data.TexSymbols.xml",bool isInternal= true)
        {
            //
            // If start application isn't WPF, pack isn't registered by defaultTexFontParser
            //
            if (Application.ResourceAssembly == null)
            {
                Application.ResourceAssembly = Assembly.GetExecutingAssembly();
                if (!UriParser.IsKnownScheme("pack"))
                    UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);
            }

            AvailableFonts = new Dictionary<string,string>
            {
                {"Asana-Math","WpfMath.Data.AsanaMathFontSettings.wmpkg" },
                {"Computer-Modern","WpfMath.Data.AsanaMathFontSettings.wmpkg" },
                {"Default","WpfMath.Data.AsanaMathFontSettings.wmpkg" },
                
            };
            
            commands = new HashSet<string>
            {
                "color",
                "colorbox",
                "frac",
                "left",
                "overline",
                "right",
                "sqrt",
                "underline"
            };

            predefinedColors = new Dictionary<string, Color>();
            
            DeclaredFonts = declaredFonts;
            
            FormulaFontFilesDirectory = fontFilesDir;
            FormulaFontInfoFilePath = fontInfoFilePath;
            FormulaSettingsFilePath = formulaSettingsFilePath;
            FormulaSymbolsFilePath = symbolsFilePath;
            AreFontsInternal = isInternal;

            
            var formulaSettingsParser = new InternalTexFormulaSettingsParser(FormulaSettingsFilePath,AreFontsInternal);
            symbols = formulaSettingsParser.GetSymbolMappings();
            delimeters = formulaSettingsParser.GetDelimiterMappings();
            TextStyles = formulaSettingsParser.GetTextStyles();
            DefaultTextStyleMapping = formulaSettingsParser.GetDefaultTextStyleMappings();

            var colorParser = new PredefinedColorParser();
            colorParser.Parse(predefinedColors);

            var predefinedFormulasParser = new TexPredefinedFormulaParser();
            predefinedFormulasParser.Parse(predefinedFormulas);
        }

        /// <summary>
        /// Loads the specified <paramref name="settingsFile"/> for this <see cref="TexFormulaParser"/> to use for parsing.
        /// </summary>
        /// <param name="settingsFile"></param>
        /// <param name="fromthisAssembly"></param>
        public void LoadSettings(string settingsFile,bool fromthisAssembly)
        {
            if (settingsFile.EndsWith(".wmpkg"))
            {
                if (!fromthisAssembly && !File.Exists(settingsFile))
                {
                    throw new TexParseException("Invalid settings file path");
                }

                ZipFile zipfile = null;
                var dirs = Regex.Split(settingsFile.Substring(0, settingsFile.Length - 6), @"[./\\]");
                string folderName = dirs[dirs.Length - 1];

                var extractionDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WPFMATH\\" + folderName;
                var parentExtractionDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WPFMATH\\";

                if (Directory.Exists(extractionDir))
                {
                    //Clear the files in it if they exist
                    foreach (var item in Directory.EnumerateFiles(extractionDir))
                    {
                        File.Delete(item);
                    }
                }
                if (Directory.Exists(extractionDir) == false)
                {
                    //create it
                    Directory.CreateDirectory(extractionDir);
                }

                if (!fromthisAssembly && File.Exists(settingsFile))
                {
                    zipfile = ZipFile.Read(settingsFile);
                    zipfile.ExtractAll(extractionDir, ExtractExistingFileAction.DoNotOverwrite);
                }
                if (fromthisAssembly)
                {
                    using (var fs= (Assembly.GetExecutingAssembly().GetManifestResourceStream(settingsFile)))
                    {
                        zipfile= ZipFile.Read(fs);
                        zipfile.ExtractAll(extractionDir, ExtractExistingFileAction.DoNotOverwrite);
                    }  
                }
                
                var settingsguidefile = extractionDir + "\\SettingsInfo.xml";
                DirectoryInfo dirInfo = new DirectoryInfo(parentExtractionDir)
                {
                    Attributes = FileAttributes.Hidden
                };

                if (File.Exists(settingsguidefile))
                {
                    using (var fs = File.Open(settingsguidefile, FileMode.Open))
                    {
                        XmlDocument settingsDoc = new XmlDocument();
                        settingsDoc.Load(fs);
                        if (settingsDoc.DocumentElement.Name == "ParserSettings" && settingsDoc.DocumentElement.HasChildNodes)
                        {
                            var settingsDocNodes = settingsDoc.DocumentElement.GetXmlNodes();
                            foreach (var item in settingsDocNodes)
                            {
                                if (item.Name == "DeclaredFonts")
                                {
                                    if (int.TryParse(item.FirstChild.Value, out int result))
                                    {
                                        DeclaredFonts = result;
                                    }
                                    else
                                    {
                                    }
                                }
                                if (item.Name == "SymbolsPath")
                                {
                                    FormulaSymbolsFilePath = extractionDir + item.FirstChild.Value.Trim();
                                }
                                if (item.Name == "FontDescriptionPath")
                                {
                                    FormulaFontInfoFilePath = extractionDir + item.FirstChild.Value.Trim();
                                }
                                if (item.Name == "FormulaSettingsPath")
                                {
                                    FormulaSettingsFilePath = extractionDir + item.FirstChild.Value.Trim();
                                }
                                if (item.Name == "FontsDirectory")
                                {
                                    FormulaFontFilesDirectory = extractionDir + item.FirstChild.Value.Trim();
                                }
                            }
                            AreFontsInternal = false;
                            Initialize(DeclaredFonts, FormulaFontFilesDirectory, FormulaFontInfoFilePath, FormulaSettingsFilePath, FormulaSymbolsFilePath, AreFontsInternal);
                        }
                        fs.Close();
                    }
                }
                else
                {
                    throw new TexParseException("Invalid font package");
                }
                zipfile.Dispose();
            }
            
        }

        //TODO: Include Predefined tex formulas
        /// <summary>
        /// Generates a Formula setting file from the specified files
        /// </summary>
        /// <param name="settingsFileName"></param>
        /// <param name="ParserSettingsFile"></param>
        /// <param name="FontDescriptionFile"></param>
        /// <param name="FormulaSettingsPath"></param>
        /// <param name="FormulaSymbolsFile"></param>
        /// <param name="FontFiles"></param>
        public void SaveSettings(string settingsFileName,
            string ParserSettingsFile,
            string FontDescriptionFile,
            string FormulaSettingsPath,
            string FormulaSymbolsFile,
            string[] FontFiles)
        {
            if (File.Exists(settingsFileName))
            {
                string errstr = DateTime.Now.ToShortTimeString() ;
                Debug.WriteLine(errstr);
                File.Delete(settingsFileName);
            }
            using (ZipFile zipfile=new ZipFile(settingsFileName))
            {
                zipfile.SortEntriesBeforeSaving = true;
                if (ParserSettingsFile.EndsWith("SettingsInfo.xml")
                    &&FontDescriptionFile.EndsWith(".xml")
                    && FormulaSettingsPath.EndsWith(".xml")
                    && FormulaSymbolsFile.EndsWith(".xml"))
                {                    
                    zipfile.AddFile(ParserSettingsFile, "");
                    zipfile.AddFile(FontDescriptionFile, "");
                    zipfile.AddFile(FormulaSettingsPath, "");
                    zipfile.AddFile(FormulaSymbolsFile, "");
                }
                
                foreach (var item in FontFiles)
                {
                    zipfile.AddFile(item, "Fonts");
                }

                zipfile.Save();
            }  
        }
        
        internal static string GetDelimeterMapping(char character)
        {
            try
            {
                return delimeters[character];
            }
            catch (KeyNotFoundException)
            {
                throw new DelimiterMappingNotFoundException(character);
            }
        }

        internal static SymbolAtom GetDelimiterSymbol(string name, SourceSpan source,string symbolsFilepath= "WpfMath.Data.TexSymbols.xml", bool isInternal=true)
        {
            if (name == null)
                return null;

            var result = SymbolAtom.GetAtom(name, source,symbolsFilepath,isInternal );
            if (!result.IsDelimeter)
                return null;
            return result;
        }

        private static bool IsSymbol(char c)
        {
            return !char.IsLetterOrDigit(c);
        }

        private static bool IsWhiteSpace(char ch)
        {
            return ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
        }

        private static bool ShouldSkipWhiteSpace(string style) => style != TexUtilities.TextStyleName;

        public TexFormula Parse(string value, string textStyle = null)
        {
            Debug.WriteLine(value);
            var position = 0;
            var result = Parse(new SourceSpan(value, 0, value.Length), ref position, false, textStyle);
            result.DeclaredFonts = DeclaredFonts;
            result.FormulaFontFilesDirectory = FormulaFontFilesDirectory;
            result.FormulaFontInfoFilePath = FormulaFontInfoFilePath;
            result.FormulaSettingsFilePath = FormulaSettingsFilePath;
            result.FormulaSymbolsFilePath = FormulaSymbolsFilePath;
            result.AreFontsInternal = AreFontsInternal;
           
            return result;
        }

        private TexFormula Parse(SourceSpan value, string textStyle)
        {
            int localPostion = 0;
            return Parse(value, ref localPostion, false, textStyle);
        }

        private DelimiterInfo ParseUntilDelimiter(SourceSpan value, ref int position, string textStyle)
        {
            var embeddedFormula = Parse(value, ref position, true, textStyle);
            if (embeddedFormula.RootAtom == null)
                throw new TexParseException("Cannot find closing delimiter");

            var source = embeddedFormula.RootAtom.Source;
            var bodyRow = embeddedFormula.RootAtom as RowAtom;
            var lastAtom = bodyRow?.Elements.LastOrDefault() ?? embeddedFormula.RootAtom;
            var lastDelimiter = lastAtom as SymbolAtom;
            if (lastDelimiter == null || !lastDelimiter.IsDelimeter)
                throw new TexParseException($"Cannot find closing delimiter; got {lastDelimiter} instead");

            Atom bodyAtom;
            if (bodyRow == null)
            {
                bodyAtom = new RowAtom(source);
            }
            else if (bodyRow.Elements.Count > 2)
            {
                var row = bodyRow.Elements.Take(bodyRow.Elements.Count - 1)
                    .Aggregate(new RowAtom(source), (r, atom) => r.Add(atom));
                bodyAtom = row;
            }
            else if (bodyRow.Elements.Count == 2)
            {
                bodyAtom = bodyRow.Elements[0];
            }
            else
            {
                throw new NotSupportedException($"Cannot convert {bodyRow} to fenced atom body");
            }

            return new DelimiterInfo(bodyAtom, lastDelimiter);
        }

        private TexFormula Parse(SourceSpan value, ref int position, bool allowClosingDelimiter, string textStyle)
        {
            var formula = new TexFormula() { TextStyle = textStyle };
            var closedDelimiter = false;
            var skipWhiteSpace = ShouldSkipWhiteSpace(textStyle);
            var initialPosition = position;
            while (position < value.Length && !(allowClosingDelimiter && closedDelimiter))
            {
                char ch = value[position];
                var source = value.Segment(position, 1);
                if (IsWhiteSpace(ch))
                {
                    if (!skipWhiteSpace)
                    {
                        formula.Add(new SpaceAtom(source), source);
                    }

                    position++;
                }
                else if (ch == escapeChar)
                {
                    ProcessEscapeSequence(formula, value, ref position, allowClosingDelimiter, ref closedDelimiter);
                }
                else if (ch == leftGroupChar)
                {
                    var groupValue = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                    var parsedGroup = Parse(groupValue, textStyle);
                    var innerGroupAtom = parsedGroup.RootAtom ?? new RowAtom(groupValue);
                    var groupAtom = new TypedAtom(
                        innerGroupAtom.Source,
                        innerGroupAtom,
                        TexAtomType.Ordinary,
                        TexAtomType.Ordinary);
                    var scriptsAtom = this.AttachScripts(formula, value, ref position, groupAtom);
                    formula.Add(scriptsAtom, value.Segment(initialPosition, scriptsAtom.Source.Length));
                }
                else if (ch == rightGroupChar)
                {
                    throw new TexParseException("Found a closing '" + rightGroupChar
                        + "' without an opening '" + leftGroupChar + "'!");
                }
                else if (ch == superScriptChar || ch == subScriptChar || ch == primeChar)
                {
                    if (position == 0)
                        throw new TexParseException("Every script needs a base: \""
                            + superScriptChar + "\", \"" + subScriptChar + "\" and \""
                            + primeChar + "\" can't be the first character!");
                    else
                        throw new TexParseException("Double scripts found! Try using more braces.");
                }
                else
                {
                    var scriptsAtom = this.AttachScripts(
                        formula,
                        value,
                        ref position,
                        this.ConvertCharacter(formula, ref position, source),
                        skipWhiteSpace);
                    formula.Add(scriptsAtom, value.Segment(initialPosition, position));
                }
            }

            return formula;
        }

        private SourceSpan ReadGroup(TexFormula formula, SourceSpan value, ref int position, char openChar, char closeChar)
        {
            if (position == value.Length || value[position] != openChar)
                throw new TexParseException("missing '" + openChar + "'!");

            var group = 0;
            position++;
            var start = position;
            while (position < value.Length && !(value[position] == closeChar && group == 0))
            {
                if (value[position] == openChar)
                    group++;
                else if (value[position] == closeChar)
                    group--;
                position++;
            }

            if (position == value.Length)
            {
                // Reached end of formula but group has not been closed.
                throw new TexParseException("Illegal end,  missing '" + closeChar + "'!");
            }

            position++;
            return value.Segment(start, position - start - 1);
        }

        private TexFormula ReadScript(TexFormula formula, SourceSpan value, ref int position)
        {
            SkipWhiteSpace(value, ref position);
            if (position == value.Length)
                throw new TexParseException("illegal end, missing script!");

            var ch = value[position];
            if (ch == leftGroupChar)
            {
                return Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);
            }
            else
            {
                position++;
                return Parse(value.Segment(position - 1, 1), formula.TextStyle);
            }
        }

        private Atom ProcessCommand(
            TexFormula formula,
            SourceSpan value,
            ref int position,
            string command,
            bool allowClosingDelimiter,
            ref bool closedDelimiter)
        {
            int start = position - command.Length;

            SkipWhiteSpace(value, ref position);

            SourceSpan source;
            switch (command)
            {
                case "frac":
                    // Command is fraction.

                    var numeratorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar,
                        rightGroupChar), formula.TextStyle);
                    SkipWhiteSpace(value, ref position);
                    var denominatorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar,
                        rightGroupChar), formula.TextStyle);
                    if (numeratorFormula.RootAtom == null || denominatorFormula.RootAtom == null)
                        throw new TexParseException("Both numerator and denominator of a fraction can't be empty!");

                    source = value.Segment(start, position - start);
                    return new FractionAtom(source, numeratorFormula.RootAtom, denominatorFormula.RootAtom, true);

                case "left":
                    {
                        SkipWhiteSpace(value, ref position);
                        if (position == value.Length)
                            throw new TexParseException("`left` command should be passed a delimiter");

                        var delimiter = value[position];
                        ++position;
                        var left = position;

                        var internals = ParseUntilDelimiter(value, ref position, formula.TextStyle);

                        var opening = GetDelimiterSymbol(
                            GetDelimeterMapping(delimiter),
                            value.Segment(start, left - start),
                            FormulaSymbolsFilePath,AreFontsInternal);
                        if (opening == null)
                            throw new TexParseException($"Cannot find delimiter named {delimiter}");

                        var closing = internals.ClosingDelimiter;
                        source = value.Segment(start, position - start);
                        return new FencedAtom(source, internals.Body, opening, closing);
                    }
                case "overline":
                    {
                        var overlineFormula = this.Parse(this.ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);
                        this.SkipWhiteSpace(value, ref position);
                        source = value.Segment(start, position - start);
                        return new OverlinedAtom(source, overlineFormula.RootAtom);
                    }
                case "right":
                    {
                        if (!allowClosingDelimiter)
                            throw new TexParseException("`right` command is not allowed without `left`");

                        SkipWhiteSpace(value, ref position);
                        if (position == value.Length)
                            throw new TexParseException("`right` command should be passed a delimiter");

                        var delimiter = value[position];
                        ++position;

                        var closing = GetDelimiterSymbol(
                            GetDelimeterMapping(delimiter),
                            value.Segment(start, position - start),
                            FormulaSymbolsFilePath,AreFontsInternal);
                        if (closing == null)
                            throw new TexParseException($"Cannot find delimiter named {delimiter}");

                        closedDelimiter = true;
                        return closing;
                    }

                case "sqrt":
                    // Command is radical.

                    SkipWhiteSpace(value, ref position);
                    if (position == value.Length)
                        throw new TexParseException("illegal end!");

                    int sqrtEnd = position;

                    TexFormula degreeFormula = null;
                    if (value[position] == leftBracketChar)
                    {
                        // Degree of radical- is specified.
                        degreeFormula = Parse(ReadGroup(formula, value, ref position, leftBracketChar,
                            rightBracketChar), formula.TextStyle);
                        SkipWhiteSpace(value, ref position);
                    }

                    var sqrtFormula = this.Parse(
                        this.ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar),
                        formula.TextStyle);

                    if (sqrtFormula.RootAtom == null)
                    {
                        throw new TexParseException("The radicand of a square root can't be empty!");
                    }

                    source = value.Segment(start, sqrtEnd - start);
                    return new Radical(source, sqrtFormula.RootAtom, degreeFormula?.RootAtom);

                case "underline":
                    {
                        var underlineFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), formula.TextStyle);
                        SkipWhiteSpace(value, ref position);
                        source = value.Segment(start, position - start);
                        return new UnderlinedAtom(source, underlineFormula.RootAtom);
                    }
                case "color":
                    {
                        var colorName = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        var remainingString = value.Segment(position);
                        var remaining = Parse(remainingString, formula.TextStyle);
                        position = value.Length;
                        if (predefinedColors.TryGetValue(colorName.ToString(), out var color))
                        {
                            source = value.Segment(start, position - start);
                            return new StyledAtom(source, remaining.RootAtom, null, new SolidColorBrush(color));
                        }

                        throw new TexParseException($"Color {colorName} not found");
                    }
                case "colorbox":
                    {
                        var colorName = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        var remainingString = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
                        var remaining = Parse(remainingString, formula.TextStyle);
                        if (predefinedColors.TryGetValue(colorName.ToString(), out var color))
                        {
                            source = value.Segment(start, position - start);
                            return new StyledAtom(source, remaining.RootAtom, new SolidColorBrush(color), null);
                        }

                        throw new TexParseException($"Color {colorName} not found");
                    }
            }

            throw new TexParseException("Invalid command.");
        }

        private void ProcessEscapeSequence(
            TexFormula formula,
            SourceSpan value,
            ref int position,
            bool allowClosingDelimiter,
            ref bool closedDelimiter)
        {
            var initialSrcPosition = position;
            position++;
            var start = position;
            while (position < value.Length)
            {
                var ch = value[position];
                var isEnd = position == value.Length - 1;
                if (!char.IsLetter(ch) || isEnd)
                {
                    // Escape sequence has ended
                    // Or it's a symbol. Assuming in this case it will only be a single char.
                    if ((isEnd && char.IsLetter(ch)) || position - start == 0)
                    {
                        position++;
                    }
                    break;
                }

                position++;
            }

            var commandSpan = value.Segment(start, position - start);
            var command = commandSpan.ToString();
            var formulaSource = new SourceSpan(value.Source, initialSrcPosition, commandSpan.End);

            SymbolAtom symbolAtom =null;
            if (TexFontUtilities.GreekCapitalLetters.Contains(command) || TexFontUtilities.GreekSmallLetters.Contains(command))
            {
                string symbolName = TexFontUtilities.TextStylesPrefixDict[formula.TextStyle ?? "mathrm"] + command;//mtt6
                try
                {
                    var alphanumericchar = new AlphaNumericAtom(commandSpan, symbolName);

                    //current representation can't be found so use the default mapping
                    string greekmapping_default = null;
                    if (TexFontUtilities.GreekCapitalLetters.Contains(command))
                    {
                        greekmapping_default = DefaultTextStyleMapping.Item4;
                    }
                    if (TexFontUtilities.GreekSmallLetters.Contains(command))
                    {
                        greekmapping_default = DefaultTextStyleMapping.Item5;
                    }
                    string defaultSymbolName = TexFontUtilities.TextStylesPrefixDict[greekmapping_default] + command;
                    alphanumericchar = new AlphaNumericAtom(commandSpan, symbolName, defaultSymbolName);
                    //I need to make some slight changes for digamma,Digamma and var[A-Za-z]+

                    formula.Add(this.AttachScripts(formula, value, ref position, alphanumericchar), formulaSource);
                    
                }
                catch (SymbolNotFoundException e)
                {
                    throw new TexParseException("The macro \""
                            + command.ToString()
                            + "\" was mapped to an unknown symbol with the name \""
                            + symbolName + "\"!", e);
                }
                
            }
            else if (SymbolAtom.TryGetAtom(commandSpan, out symbolAtom, FormulaSymbolsFilePath, AreFontsInternal))
            {
                // Symbol was found.

                if (symbolAtom.Type == TexAtomType.Accent)
                {
                    var helper = new TexFormulaHelper(formula, formulaSource);
                    TexFormula accentFormula = ReadScript(formula, value, ref position);
                    helper.AddAccent(accentFormula, symbolAtom.Name);
                }
                else if (symbolAtom.Type == TexAtomType.BigOperator)
                {
                    var opAtom = new BigOperatorAtom(formulaSource, symbolAtom, null, null);
                    formula.Add(this.AttachScripts(formula, value, ref position, opAtom), formulaSource);
                }
                else
                {
                    formula.Add(this.AttachScripts(formula, value, ref position, symbolAtom), formulaSource);
                }
            }
            else if (predefinedFormulas.TryGetValue(command, out var factory))
            {
                // Predefined formula was found.
                var predefinedFormula = factory(formulaSource);
                var atom = this.AttachScripts(formula, value, ref position, predefinedFormula.RootAtom);
                formula.Add(atom, formulaSource);
            }
            else if (command.Equals("nbsp"))
            {
                // Space was found.
                var atom = this.AttachScripts(formula, value, ref position, new SpaceAtom(formulaSource));
                formula.Add(atom, formulaSource);
            }
            else if (TextStyles.Contains(command))
            {
                // Text style was found.
                this.SkipWhiteSpace(value, ref position);
                var styledFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar), command);
                if (styledFormula.RootAtom == null)
                    throw new TexParseException("Styled text can't be empty!");
                var atom = this.AttachScripts(formula, value, ref position, styledFormula.RootAtom);
                var source = new SourceSpan(formulaSource.Source, formulaSource.Start, position);
                formula.Add(atom, source);
            }
            else if (commands.Contains(command))
            {
                // Command was found.
                var commandAtom = this.ProcessCommand(
                    formula,
                    value,
                    ref position,
                    command,
                    allowClosingDelimiter,
                    ref closedDelimiter);

                commandAtom = allowClosingDelimiter
                    ? commandAtom
                    : AttachScripts(
                        formula,
                        value,
                        ref position,
                        commandAtom);

                var source = new SourceSpan(formulaSource.Source, formulaSource.Start, commandAtom.Source.End);
                formula.Add(commandAtom, source);
            }
            else
            {
                // Escape sequence is invalid.
                throw new TexParseException("Unknown symbol or command or predefined TeXFormula: '" + command + "'");
            }
        }

        private Atom AttachScripts(TexFormula formula, SourceSpan value, ref int position, Atom atom, bool skipWhiteSpace = true)
        {
            if (skipWhiteSpace)
            {
                SkipWhiteSpace(value, ref position);
            }

            var initialPosition = position;
            if (position == value.Length)
                return atom;

            // Check for prime marks.
            var primesRowAtom = new RowAtom(new SourceSpan(value.Source, position, 0));
            int i = position;
            while (i < value.Length)
            {
                if (value[i] == primeChar)
                {
                    var primesymbol = SymbolAtom.GetAtom("prime", value.Segment(i, 1), FormulaSymbolsFilePath, AreFontsInternal);
                    
                    primesRowAtom = primesRowAtom.Add(primesymbol);
                    position++;
                }
                else if (!IsWhiteSpace(value[i]))
                    break;
                i++;
            }

            var primesRowSource = new SourceSpan(
                value.Source,
                primesRowAtom.Source.Start,
                position - primesRowAtom.Source.Start);
            primesRowAtom = primesRowAtom.WithSource(primesRowSource);

            if (primesRowAtom.Elements.Count > 0)
                atom = new ScriptsAtom(primesRowAtom.Source, atom, null, primesRowAtom);

            if (position == value.Length)
                return atom;

            TexFormula superscriptFormula = null;
            TexFormula subscriptFormula = null;

            var ch = value[position];
            if (ch == superScriptChar)
            {
                // Attach superscript.
                position++;
                superscriptFormula = ReadScript(formula, value, ref position);

                SkipWhiteSpace(value, ref position);
                if (position < value.Length && value[position] == subScriptChar)
                {
                    // Attach subscript also.
                    position++;
                    subscriptFormula = ReadScript(formula, value, ref position);
                }
            }
            else if (ch == subScriptChar)
            {
                // Add subscript.
                position++;
                subscriptFormula = ReadScript(formula, value, ref position);

                SkipWhiteSpace(value, ref position);
                if (position < value.Length && value[position] == superScriptChar)
                {
                    // Attach superscript also.
                    position++;
                    superscriptFormula = ReadScript(formula, value, ref position);
                }
            }

            if (superscriptFormula == null && subscriptFormula == null)
                return atom;

            // Check whether to return Big Operator or Scripts.
            var subscriptAtom = subscriptFormula?.RootAtom;
            var superscriptAtom = superscriptFormula?.RootAtom;
            if (atom.GetRightType() == TexAtomType.BigOperator)
            {
                var source = value.Segment(atom.Source.Start, position - atom.Source.Start);
                if (atom is BigOperatorAtom typedAtom)
                {
                    return new BigOperatorAtom(
                        source,
                        typedAtom.BaseAtom,
                        subscriptAtom,
                        superscriptAtom,
                        typedAtom.UseVerticalLimits);
                }

                return new BigOperatorAtom(source, atom, subscriptAtom, superscriptAtom);
            }
            else
            {
                var source = new SourceSpan(value.Source, initialPosition, position - initialPosition);
                return new ScriptsAtom(source, atom, subscriptAtom, superscriptAtom);
            }
        }

        private Atom ConvertCharacter(TexFormula formula, ref int position, SourceSpan source)
        {
            var character = source[0];
            position++;
            if (IsSymbol(character) && formula.TextStyle != TexUtilities.TextStyleName)
            {
                // Character is symbol.
                var symbolName = symbols.ElementAtOrDefault(character);
                if (string.IsNullOrEmpty(symbolName))
                    throw new TexParseException($"Unknown character : '{character}'");

                try
                {
                    return SymbolAtom.GetAtom(symbolName, source,FormulaSymbolsFilePath,AreFontsInternal);
                }
                catch (SymbolNotFoundException e)
                {
                    throw new TexParseException("The character '"
                            + character.ToString()
                            + "' was mapped to an unknown symbol with the name '"
                            + (string)symbolName + "'!", e);
                }
            }
            else // Character is alpha-numeric or should be rendered as text.
            {
                if (formula.TextStyle=="text")
                {
                    return new CharAtom(source, character, formula.TextStyle);
                }
                else
                {
                    //convert the character to its internal macro representation
                    string charname = TexFontUtilities.TextStylesPrefixDict[formula.TextStyle ?? "mathrm"] + TexFontUtilities.GetCharacterasString(character);//mtt6
                    AlphaNumericAtom alphanumericchar = null;
                    try
                    {
                        string defaultcharmapping = null;
                        if (TexFontUtilities.Digits.Contains(character))
                        {
                            defaultcharmapping = DefaultTextStyleMapping.Item1;
                        }
                        if (TexFontUtilities.EnglishCapitalLetters.Contains(character))
                        {
                            defaultcharmapping = DefaultTextStyleMapping.Item2;
                        }
                        if (TexFontUtilities.EnglishSmallLetters.Contains(character))
                        {
                            defaultcharmapping = DefaultTextStyleMapping.Item3;
                        }
                        // create a default character name to be used if current representation can't be found 

                        string defaultcharname = TexFontUtilities.TextStylesPrefixDict[defaultcharmapping] + TexFontUtilities.GetCharacterasString(character);
                        alphanumericchar = new AlphaNumericAtom(source, charname,defaultcharname);

                        return alphanumericchar;
                    }
                    catch (Exception e)
                    {
                        throw new TexParseException("The character '"
                                + character.ToString()
                                + "' was mapped to an unknown symbol with the name \""
                                + charname + "\"!", e);
                    }
                }
            }
        }

        private void SkipWhiteSpace(SourceSpan value, ref int position)
        {
            while (position < value.Length && IsWhiteSpace(value[position]))
                position++;
        }
    }
}
