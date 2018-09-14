using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfMath.Exceptions;

namespace WpfMath.Utils
{
    /// <summary>
    /// Represents a class containing extra information for working with fonts.
    /// </summary>
    internal static class TexFontUtilities
    {
        public const int NoFontId = -1;

        public const double PixelsPerPoint = 1.0;

        public const int ExtensionTop = 0;
        public const int ExtensionMiddle = 1;
        public const int ExtensionRepeat = 2;
        public const int ExtensionBottom = 3;
        public const int ExtensionLeft = 4;
        public const int ExtensionRight = 5;

        public const int MetricsWidth = 0;
        public const int MetricsHeight = 1;
        public const int MetricsDepth = 2;
        public const int MetricsItalic = 3;

        /// <summary>
        /// Contains recognized Hindu-Arabic digits.
        /// </summary>
        public static readonly List<char> Digits = new List<char>()
        {
            '0','1','2','3','4','5','6','7','8','9'
        };

        /// <summary>
        /// Contains recognized English capital letters.
        /// </summary>
        public static readonly List<char> EnglishCapitalLetters = new List<char>()
        {
            'A','B','C','D','E','F','G','H','I','J','K','L','M',
            'N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };

        /// <summary>
        /// Contains recognized English small letters.
        /// </summary>
        public static readonly List<char> EnglishSmallLetters = new List<char>()
        {
            'a','b','c','d','e','f','g','h','i','j','k','l','m',
            'n','o','p','q','r','s','t','u','v','w','x','y','z'
        };
        
        /// <summary>
        /// Contains recognized Greek capital letters.
        /// </summary>
        public static readonly List<string> GreekCapitalLetters = new List<string>()
        {
            "Alpha","Beta","Gamma","Delta","Epsilon","Zeta","Eta","Theta","Iota","Kappa",
            "Lambda","Mu","Nu","Xi","Omicron","Pi","Rho","Sigma","Tau","Upsilon","Phi","Chi","Psi","Omega","varTheta","Digamma",
        };

        //NOTE: "nabla"and "partial"are not greek alphabets,"Digamma", "digamma" and "var[A-Za-z]+" are also ancient, they are just included here for simplicity
        /// <summary>
        /// Contains recognized Greek small letters.
        /// </summary>
        public static readonly List<string> GreekSmallLetters = new List<string>()
        {
            "alpha","beta","gamma","delta","varepsilon","zeta","eta","theta","iota","kappa",
            "lambda","mu","nu","xi","omicron","pi","rho","varsigma","sigma","tau","upsilon","varphi","chi","psi","omega",
            "vartheta","phi","varpi","digamma","varkappa","varrho","epsilon","nabla","partial"
        };

        /// <summary>
        /// Contains text styles and their alphabet prefix.
        /// </summary>
        public static readonly Dictionary<string, string> TextStylesPrefixDict = new Dictionary<string, string>()
        {
            {"mathbb","Bbb" },
            {"mathbf","mbf" },
            {"mathbffrak","mbffrak" },
            {"mathbfit","mbfit" },
            {"mathbfscr","mbfscr" },
            {"mathbfsf","mbfsans" },
            {"mathbfsfit","mbfitsans" },
            {"mathcal","mcal" },
            {"mathfrak","mfrak" },
            {"mathit","mit" },
            {"mathrm","mup" },
            {"mathscr","mscr" },
            {"mathsf","msans" },
            {"mathsfit","mitsans" },
            {"mathtt","mtt" },
        };

        /// <summary>
        /// Returns the true representation of an alphanumeric character as a string based on the English Latin alphabets
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static string GetCharacterasString(char character)
        {
            if (char.IsNumber(character))
            {
                switch (character)
                {
                    case '0':
                        return "zero";
                    case '1':
                        return "one";
                    case '2':
                        return "two";
                    case '3':
                        return "three";
                    case '4':
                        return "four";
                    case '5':
                        return "five";
                    case '6':
                        return "six";
                    case '7':
                        return "seven";
                    case '8':
                        return "eight";
                    case '9':
                        return "nine";
                    default:
                        throw new TexParseException($"{character} is an unknown number.");
                }
            }
            else
            {
                return character.ToString();
            }
        }

    }
}
