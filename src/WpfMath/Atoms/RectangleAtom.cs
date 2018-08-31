using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    internal class RectangleAtom : Atom
    {
        public RectangleAtom(SourceSpan source,Atom baseatom) : base(source)
        {
            this.BaseAtom = baseatom;
        }

        /// <summary>
        /// Gets the <see cref="Atom"/> contained in this <see cref="RectangleAtom"/>.
        /// </summary>
        public Atom BaseAtom
        {
            get;
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var defaultRuleThickness = texFont.GetDefaultLineThickness(style);
            var basebox = this.BaseAtom == null ? StrutBox.Empty : this.BaseAtom.CreateBox(environment);

            var rectbox = new RectangleBox(environment, basebox.TotalHeight + 4 * defaultRuleThickness, basebox.TotalWidth + 4 * defaultRuleThickness, 0)
            {
                Shift = basebox.Depth + 2 * defaultRuleThickness
            };
            var resultbox = new HorizontalBox();
            resultbox.Add(basebox);
            resultbox.Add(new StrutBox(-rectbox.TotalWidth + 2 * defaultRuleThickness, -rectbox.TotalHeight , 0, 0));
            resultbox.Add(rectbox);
            return resultbox;
        }
    }
}
