using System;
using System.Collections.Generic;
using System.Windows.Media;
using WpfMath.Exceptions;

namespace WpfMath
{
    // Specifies all information about single font.
    internal class TexFontInfo
    {
        public const int charCodesCount = 256;

        private readonly double[][] metrics;
        private readonly IDictionary<Tuple<char, char>, char> ligatures;
        private readonly IDictionary<Tuple<char, char>, double> kerns;
        private readonly CharFont[] nextLarger;
        private readonly int[][] extensions;

        public TexFontInfo(int fontId, GlyphTypeface font, double xHeight, double space, double quad)
        {
            this.metrics = new double[charCodesCount][];
            this.ligatures = new Dictionary<Tuple<char, char>, char>();
            this.kerns = new Dictionary<Tuple<char, char>, double>();
            this.nextLarger = new CharFont[charCodesCount];
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
            return this.Space > TexUtilities.FloatPrecision;
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
            this.nextLarger[character] = new CharFont(largerCharacter, largerFont);
        }

        public int[] GetExtension(char character)
        {
            return this.extensions[character];
        }

        public double GetKern(char leftChar, char rightChar, double factor)
        {
            Tuple<char, char> tpl = Tuple.Create(leftChar, rightChar);
            double kern = 0;
            kerns.TryGetValue(tpl, out kern);
            return kern * factor;
        }

        public CharFont GetLigature(char left, char right)
        {
            Tuple<char, char> tpl = Tuple.Create(left, right);
            char ch;
            return this.ligatures.TryGetValue(tpl, out ch) ? new CharFont(ch, this.FontId) : null;
        }

        public CharFont GetNextLarger(char character)
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
            if (metrics.Length <= character || metrics[character] == null)
            {
                throw new TexCharacterMappingNotFoundException(
                    $"Cannot determine metrics for '{character}' character in font {FontId}");
            }

            return this.metrics[character];
        }
    }
}
