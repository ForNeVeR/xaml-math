using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath.Utils
{
    internal static class TexFontUtilities
    {
        public const int NoFontId = -1;

        public const double PixelsPerPoint = 1.0;

        public const int ExtensionTop = 0;
        public const int ExtensionMiddle = 1;
        public const int ExtensionRepeat = 2;
        public const int ExtensionBottom = 3;

        public const int MetricsWidth = 0;
        public const int MetricsHeight = 1;
        public const int MetricsDepth = 2;
        public const int MetricsItalic = 3;
        
        /// <summary>
        /// Contains recognized Greek capital letters.
        /// </summary>
        public static readonly List<string> GreekCapitalLetters = new List<string>()
        {
            "Alpha","Beta","Gamma","Delta","Epsilon","Zeta","Eta","Theta","Iota","Kappa",
            "Lambda","Mu","Nu","Xi","Omicron","Pi","Rho","Sigma","Tau","Upsilon","Phi","Chi","Psi","Omega","varTheta","Digamma",
        };

        //NOTE: "nabla"and "partial"are not greek alphabets,"Digamma" and "digamma" are also ancient, they are just included here for simplicity
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
        
    }
}
