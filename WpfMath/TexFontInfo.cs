using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfMath
{
    // Specifies all information about single font.
    internal class TexFontInfo
    {
        public const int charCodesCount = 256;

        private readonly double[][] metrics;
        private readonly IDictionary<Tuple<char, char>, char> ligatures;
        private readonly IDictionary<Tuple<char, char>, double> kerns;
        private readonly WpfMath.CharFont[] nextLarger;
        private readonly int[][] extensions;

        public TexFontInfo(int fontId, GlyphTypeface font, double xHeight, double space, double quad)
        {
            this.metrics = new double[charCodesCount][];
            this.ligatures = new Dictionary<Tuple<char, char>, char>();
            this.kerns = new Dictionary<Tuple<char, char>, double>();
            this.nextLarger = new WpfMath.CharFont[charCodesCount];
            this.extensions = new int[charCodesCount][];

            this.FontId = fontId;
            this.Font = font;
            this.XHeight = xHeight;
            this.Space = space;
            this.Quad = quad;
            this.SkewCharacter = (char)1;
        }

        public int FontId
        {
            get;
            private set;
        }

        public GlyphTypeface Font
        {
            get;
            private set;
        }

        public double XHeight
        {
            get;
            private set;
        }

        public double Space
        {
            get;
            private set;
        }

        public double Quad
        {
            get;
            private set;
        }

        // Skew character (used for positioning accents).
        public char SkewCharacter
        {
            get;
            set;
        }

        public void AddKern(char leftChar, char rightChar, double kern)
        {
            this.kerns.Add(Tuple.Create(leftChar, rightChar), kern);
        }

        public void AddLigature(char leftChar, char rightChar, char ligatureChar)
        {
            this.ligatures.Add(Tuple.Create(leftChar, rightChar), ligatureChar);
        }

        public bool HasSpace()
        {
            return this.Space > WpfMath.TexUtilities.FloatPrecision;
        }

        public void SetExtensions(char character, int[] extensions)
        {
            this.extensions[character] = extensions;
        }

        public void SetMetrics(char character, double[] metrics)
        {
            this.metrics[character] = metrics;
        }

        public void SetNextLarger(char character, char largerCharacter, int largerFont)
        {
            this.nextLarger[character] = new WpfMath.CharFont(largerCharacter, largerFont);
        }

        public int[] GetExtension(char character)
        {
            return this.extensions[character];
        }

        public double GetKern(char leftChar, char rightChar, double factor)
        {
            try
            {
                return this.kerns[Tuple.Create(leftChar, rightChar)] * factor;
            }
            catch (KeyNotFoundException)
            {
                return 0;
            }
        }

        public WpfMath.CharFont GetLigature(char left, char right)
        {
            try
            {
                return new WpfMath.CharFont(this.ligatures[Tuple.Create(left, right)], this.FontId);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public WpfMath.CharFont GetNextLarger(char character)
        {
            return this.nextLarger[character];
        }

        public double GetQuad(double factor)
        {
            return this.Quad * factor;
        }

        public double GetSpace(double factor)
        {
            return this.Space * factor;
        }

        public double GetXHeight(double factor)
        {
            return this.XHeight * factor;
        }

        public double[] GetMetrics(char character)
        {
            return this.metrics[character];
        }
    }
}
