using System.Collections.Generic;
using WpfMath.Boxes;
using WpfMath.Exceptions;

namespace WpfMath.Atoms
{
    /// <summary>
    /// Represents a group of atoms intended to display long division operations.
    /// </summary>
    internal class LongDivAtom:Atom
    {
        //most of the notations listed haven't been implemented
        /// <summary>
        /// Stores a list of notations supported by the <see cref="LongDivAtom"/>.
        /// </summary>
        public readonly List<string> LongDivisionStyles= new List<string>
        {
            @":right=right",@"left/\right",@"left)(right",@"lefttop",@"stackedleftleft",@"stackedrightright",@"mediumstackedrightright",@"shortstackedrightright",@"righttop"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="LongDivAtom"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="divstyle"></param>
        public LongDivAtom(SourceSpan source,List<Atom> input,string divstyle= "lefttop"):base(source)
        {
            LongDivStyle = divstyle;
            DivisorAtom = input[0];
            QuotientAtom = input[1];
            DividendAtom = input[2];
            OtherAtom =input.Count>3? input[3]:null;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the style of the <see cref="LongDivAtom"/>.
        /// </summary>
        public string LongDivStyle { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="Atom"/> that contains the divisor.
        /// </summary>
        public Atom DivisorAtom{get; private set;}

        /// <summary>
        /// Gets or sets the <see cref="Atom"/> that contains the quotient or result of the division.
        /// </summary>
        public Atom QuotientAtom{get; private set;}

        /// <summary>
        /// Gets or sets the <see cref="Atom"/> that contains the dividend.
        /// </summary>
        public Atom DividendAtom{get; private set;}

        /// <summary>
        /// Gets or sets the <see cref="Atom"/> that contains the steps of the long division.
        /// </summary>
        public Atom OtherAtom{get; private set;}

        #endregion

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            switch (LongDivStyle)
            {
                case @":right=right": { return LongDivision__colon__right__equals__right(environment); }
                case @"left/\right": { return LongDivisionStyle_left__forwardslash___backslash__right(environment); }
                case @"left)(right": { return LongDivisionStyle_left__curvedbrackright___curvedbrackleft__right(environment); }
                case @"lefttop":{return LongDivisionStyle_lefttop(environment);}
                case @"stackedleftleft":{ return LongDivisionStyle_stackedleftleft(environment); }
                case @"righttop": { return LongDivisionStyle_righttop(environment); }
                case @"stackedrightright": { return LongDivisionStyle_stackedrightright(environment); }
                case @"mediumstackedrightright": { return LongDivisionStyle_mediumstackedrightright(environment); }
                case @"shortstackedrightright": { return LongDivisionStyle_shortstackedrightright(environment); }


                default:
                    string helpStr = Parsers.TexFormulaParser.HelpOutMessage(LongDivStyle, LongDivisionStyles);
                    throw new TexParseException($"The style '{LongDivStyle}' is not supported{helpStr}.");
            }
        }

        #region Long division style handlers
        private Box LongDivisionStyle_lefttop(TexEnvironment environment )
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var dividend_divisor = new HorizontalBox();
            var divisorbox = this.DivisorAtom == null ? StrutBox.Empty : this.DivisorAtom.CreateBox(environment);
            var leftsep = SymbolAtom.GetAtom("rbrack", this.Source).CreateBox(environment);
            var dividendbox = this.DividendAtom == null ? StrutBox.Empty : this.DividendAtom.CreateBox(environment);
            double barwidth = leftsep.TotalWidth + dividendbox.TotalWidth;
            var separatorbar = new HorizontalRule(environment, defaultLineThickness, barwidth, -dividendbox.TotalHeight-defaultLineThickness);


            dividend_divisor.Add(divisorbox);
            dividend_divisor.Add(leftsep);
            dividend_divisor.Add(dividendbox);
            dividend_divisor.Add(new StrutBox(-barwidth, dividendbox.TotalHeight, 0, 0));
            dividend_divisor.Add(separatorbar);

            var quotient_pad = new HorizontalBox();
            var quotientbox = this.QuotientAtom == null ? StrutBox.Empty : this.QuotientAtom.CreateBox(environment);
            var leftspace = dividend_divisor.TotalWidth - quotientbox.TotalWidth;
            var quot_leftspacebox = new StrutBox(leftspace,quotientbox.TotalHeight,0,0);

            quotient_pad.Add(quot_leftspacebox);
            quotient_pad.Add(quotientbox);

            var otherbox = this.OtherAtom == null ? StrutBox.Empty : this.OtherAtom.CreateBox(environment);


            var resultBox = new VerticalBox();
            resultBox.Add(quotient_pad);
            resultBox.Add(dividend_divisor);
            resultBox.Add(otherbox);

            return resultBox;
        }

        private Box LongDivisionStyle_righttop(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var quotientbox = this.QuotientAtom == null ? StrutBox.Empty : this.QuotientAtom.CreateBox(environment);

            var dividend_divisor = new HorizontalBox();
            var dividendbox= this.DividendAtom == null ? StrutBox.Empty : this.DividendAtom.CreateBox(environment);
            var divisorbox= this.DivisorAtom == null ? StrutBox.Empty : this.DivisorAtom.CreateBox(environment);
            var divisorLpad = new StrutBox(divisorbox.TotalWidth / 3, divisorbox.TotalHeight, 0, 0);
            var minorVsepline = new VerticalRule(environment, defaultLineThickness, dividendbox.TotalHeight, 0);
            var minorHsepline = new HorizontalRule(environment, defaultLineThickness, divisorbox.TotalWidth+divisorLpad.Width, 0);


            dividend_divisor.Add(dividendbox);
            dividend_divisor.Add(divisorbox);
            dividend_divisor.Add(minorVsepline);
            dividend_divisor.Add(new StrutBox(-(divisorbox.TotalWidth + divisorLpad.Width), 0, 0, 0));
            dividend_divisor.Add(minorHsepline);

            var majorsepline = new HorizontalRule(environment, defaultLineThickness, dividend_divisor.TotalWidth, 0);

            var otherbox= this.OtherAtom == null ? StrutBox.Empty : this.OtherAtom.CreateBox(environment);

            var resultBox = new VerticalBox();
            resultBox.Add(quotientbox);
            resultBox.Add(majorsepline);
            resultBox.Add(dividend_divisor);
            resultBox.Add(otherbox);

            return resultBox;
        }

        private Box LongDivisionStyle_stackedleftleft(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var divisor_quotient = new VerticalBox();
            var divisorbox = this.DivisorAtom == null ? StrutBox.Empty : this.DivisorAtom.CreateBox(environment);
            var quotientbox = this.QuotientAtom == null ? StrutBox.Empty : this.QuotientAtom.CreateBox(environment);
            var hlineseparator = new HorizontalRule(environment, defaultLineThickness, quotientbox.TotalWidth, 0);
            divisor_quotient.Add(divisorbox);
            divisor_quotient.Add(hlineseparator);
            divisor_quotient.Add(quotientbox);

            var dividend_other = new VerticalBox();
            var dividendbox = this.DividendAtom == null ? StrutBox.Empty : this.DividendAtom.CreateBox(environment);
            var otherbox = this.OtherAtom == null ? StrutBox.Empty : this.OtherAtom.CreateBox(environment);
            dividend_other.Add(dividendbox);
            dividend_other.Add(otherbox);

            var vlineseparator = new VerticalRule(environment, defaultLineThickness, dividend_other.TotalHeight, -divisorbox.TotalHeight);

            var resultBox = new HorizontalBox();
            resultBox.Add(divisor_quotient);
            resultBox.Add(vlineseparator);
            resultBox.Add(dividend_other);

            return resultBox;
        }

        private Box LongDivisionStyle_mediumstackedleftleft(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var divisor_quotient = new VerticalBox();
            var divisorbox = this.DivisorAtom == null ? StrutBox.Empty : this.DivisorAtom.CreateBox(environment);
            var quotientbox = this.QuotientAtom == null ? StrutBox.Empty : this.QuotientAtom.CreateBox(environment);
            var hlineseparator = new HorizontalRule(environment, defaultLineThickness, quotientbox.TotalWidth, 0);
            divisor_quotient.Add(divisorbox);
            divisor_quotient.Add(hlineseparator);
            divisor_quotient.Add(quotientbox);

            var dividend_other = new VerticalBox();
            var dividendbox = this.DividendAtom == null ? StrutBox.Empty : this.DividendAtom.CreateBox(environment);
            var otherbox = this.OtherAtom == null ? StrutBox.Empty : this.OtherAtom.CreateBox(environment);
            dividend_other.Add(dividendbox);
            dividend_other.Add(otherbox);

            var vlineseparator = new VerticalRule(environment, defaultLineThickness, divisor_quotient.TotalHeight, -divisorbox.TotalHeight);

            var resultBox = new HorizontalBox();
            resultBox.Add(divisor_quotient);
            resultBox.Add(vlineseparator);
            resultBox.Add(dividend_other);

            return resultBox;
        }

        private Box LongDivisionStyle_stackedrightright(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var divisor_quotient = new VerticalBox();
            var divisorbox = this.DivisorAtom == null ? StrutBox.Empty : this.DivisorAtom.CreateBox(environment);
            var quotientbox = this.QuotientAtom == null ? StrutBox.Empty : this.QuotientAtom.CreateBox(environment);
            var hlineseparator = new HorizontalRule(environment, defaultLineThickness, quotientbox.TotalWidth, 0);
            divisor_quotient.Add(divisorbox);
            divisor_quotient.Add(hlineseparator);
            divisor_quotient.Add(quotientbox);

            var dividend_other = new VerticalBox();
            var dividendbox = this.DividendAtom == null ? StrutBox.Empty : this.DividendAtom.CreateBox(environment);
            var otherbox = this.OtherAtom == null ? StrutBox.Empty : this.OtherAtom.CreateBox(environment);
            dividend_other.Add(dividendbox);
            dividend_other.Add(otherbox);

            var vlineseparator = new VerticalRule(environment, defaultLineThickness, dividend_other.TotalHeight, -divisorbox.TotalHeight);

            var resultBox = new HorizontalBox();
            resultBox.Add(dividend_other);
            resultBox.Add(vlineseparator);
            resultBox.Add(divisor_quotient);

            return resultBox;
        }

        private Box LongDivisionStyle_mediumstackedrightright(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var divisor_quotient = new VerticalBox();
            var divisorbox = this.DivisorAtom == null ? StrutBox.Empty : this.DivisorAtom.CreateBox(environment);
            var quotientbox = this.QuotientAtom == null ? StrutBox.Empty : this.QuotientAtom.CreateBox(environment);
            var hlineseparator = new HorizontalRule(environment, defaultLineThickness, quotientbox.TotalWidth, 0);
            divisor_quotient.Add(divisorbox);
            divisor_quotient.Add(hlineseparator);
            divisor_quotient.Add(quotientbox);

            var dividend_other = new VerticalBox();
            var dividendbox = this.DividendAtom == null ? StrutBox.Empty : this.DividendAtom.CreateBox(environment);
            var otherbox = this.OtherAtom == null ? StrutBox.Empty : this.OtherAtom.CreateBox(environment);
            dividend_other.Add(dividendbox);
            dividend_other.Add(otherbox);

            //double divquotheightsum =divisor_quotient.TotalHeight;
            var vlineseparator = new VerticalRule(environment, defaultLineThickness, divisor_quotient.TotalHeight, -divisorbox.TotalHeight);

            var resultBox = new HorizontalBox();
            resultBox.Add(dividend_other);
            resultBox.Add(vlineseparator);
            resultBox.Add(divisor_quotient);

            return resultBox;
        }

        private Box LongDivisionStyle_shortstackedrightright(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var divisor_quotient = new VerticalBox();
            var divisorbox = this.DivisorAtom == null ? StrutBox.Empty : this.DivisorAtom.CreateBox(environment);
            var quotientbox = this.QuotientAtom == null ? StrutBox.Empty : this.QuotientAtom.CreateBox(environment);
            var hlineseparator = new HorizontalRule(environment, defaultLineThickness, quotientbox.TotalWidth, 0);
            divisor_quotient.Add(divisorbox);
            divisor_quotient.Add(hlineseparator);
            divisor_quotient.Add(quotientbox);

            var dividend_other = new VerticalBox();
            var dividendbox = this.DividendAtom == null ? StrutBox.Empty : this.DividendAtom.CreateBox(environment);
            var otherbox = this.OtherAtom == null ? StrutBox.Empty : this.OtherAtom.CreateBox(environment);
            dividend_other.Add(dividendbox);
            dividend_other.Add(otherbox);

            //double divquotheightsum =divisor_quotient.TotalHeight;
            var vlineseparator = new VerticalRule(environment, defaultLineThickness, divisorbox.TotalHeight+defaultLineThickness, -divisorbox.TotalHeight);

            var resultBox = new HorizontalBox();
            resultBox.Add(dividend_other);
            resultBox.Add(vlineseparator);
            resultBox.Add(divisor_quotient);

            return resultBox;
        }

        private Box LongDivisionStyle_left__curvedbrackright___curvedbrackleft__right(TexEnvironment environment)
        {
            var divisor_dividend_quotient = new HorizontalBox();

            var divisorbox = this.DivisorAtom == null ? StrutBox.Empty : this.DivisorAtom.CreateBox(environment);
            var leftsep = SymbolAtom.GetAtom("rbrack", this.Source).CreateBox(environment);
            var dividendbox = this.DividendAtom == null ? StrutBox.Empty : this.DividendAtom.CreateBox(environment);
            var rightsep = SymbolAtom.GetAtom("lbrack",this.Source).CreateBox(environment);
            var quotientbox = this.QuotientAtom == null ? StrutBox.Empty : this.QuotientAtom.CreateBox(environment);

            divisor_dividend_quotient.Add(divisorbox);
            divisor_dividend_quotient.Add(leftsep);
            divisor_dividend_quotient.Add(dividendbox);
            divisor_dividend_quotient.Add(rightsep);
            divisor_dividend_quotient.Add(quotientbox);

            var otherbox= this.OtherAtom == null ? StrutBox.Empty : this.OtherAtom.CreateBox(environment);

            var resultBox = new VerticalBox();
            resultBox.Add(divisor_dividend_quotient);
            resultBox.Add(otherbox);

            return resultBox;
        }

        private Box LongDivisionStyle_left__forwardslash___backslash__right(TexEnvironment environment)
        {
            var divisor_dividend_quotient = new HorizontalBox();

            var divisorbox = this.DivisorAtom == null ? StrutBox.Empty : this.DivisorAtom.CreateBox(environment);
            var leftsep = SymbolAtom.GetAtom("slash",  this.Source).CreateBox(environment);
            var dividendbox = this.DividendAtom == null ? StrutBox.Empty : this.DividendAtom.CreateBox(environment);
            var rightsep = SymbolAtom.GetAtom("rslash", this.Source).CreateBox(environment);
            var quotientbox = this.QuotientAtom == null ? StrutBox.Empty : this.QuotientAtom.CreateBox(environment);

            divisor_dividend_quotient.Add(divisorbox);
            divisor_dividend_quotient.Add(leftsep);
            divisor_dividend_quotient.Add(dividendbox);
            divisor_dividend_quotient.Add(rightsep);
            divisor_dividend_quotient.Add(quotientbox);

            var otherbox = this.OtherAtom == null ? StrutBox.Empty : this.OtherAtom.CreateBox(environment);

            var resultBox = new VerticalBox();
            resultBox.Add(divisor_dividend_quotient);
            resultBox.Add(otherbox);

            return resultBox;
        }

        private Box LongDivision__colon__right__equals__right(TexEnvironment environment)
        {
            var divisor_dividend_quotient = new HorizontalBox();

            var dividendbox = this.DividendAtom == null ? StrutBox.Empty : this.DividendAtom.CreateBox(environment);
            var leftsep = SymbolAtom.GetAtom("colon", this.Source).CreateBox(environment);
            var divisorbox = this.DivisorAtom == null ? StrutBox.Empty : this.DivisorAtom.CreateBox(environment);
            var rightsep = SymbolAtom.GetAtom("equals", this.Source).CreateBox(environment);
            var quotientbox = this.QuotientAtom == null ? StrutBox.Empty : this.QuotientAtom.CreateBox(environment);

            divisor_dividend_quotient.Add(dividendbox);
            divisor_dividend_quotient.Add(leftsep);
            divisor_dividend_quotient.Add(divisorbox);
            divisor_dividend_quotient.Add(rightsep);
            divisor_dividend_quotient.Add(quotientbox);

            var otherbox = this.OtherAtom == null ? StrutBox.Empty : this.OtherAtom.CreateBox(environment);


            var resultBox = new VerticalBox();
            resultBox.Add(divisor_dividend_quotient);
            resultBox.Add(otherbox);

            return resultBox;
        }


        #endregion

    }
}
