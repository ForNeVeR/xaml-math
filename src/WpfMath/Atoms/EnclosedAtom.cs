using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfMath.Boxes;
using WpfMath.Exceptions;

namespace WpfMath.Atoms
{
    /// <summary>
    /// Represents an atom that encloses another.
    /// </summary>
    internal class EnclosedAtom : Atom
    {
        /// <summary>
        /// Stores a list of notations supported by the <see cref="EnclosedAtom"/>.
        /// </summary>
        public readonly List<string> AllowedEncloseNotations = new List<string>
        {
            "bottom","box","circle","ddstrike","downdiagonalstrike","ellipse","horizontalstrike","hstrike","left",
            "obar","oval","overbar","rbox","right","roundedbox","strikethrough","top","udstrike","updiagonalstrike",
            "ubar","underbar","verticalstrike","vstrike"
        };
        /// <summary>
        /// Initializes an <see cref="EnclosedAtom"/> with the specified <paramref name="baseAtom"/> and <paramref name="encNots"/>.
        /// </summary>
        /// <param name="baseAtom">The base atom.</param>
        /// <param name="encNots">The enclose notations.</param>
        public EnclosedAtom(Atom baseAtom,string encNots="circle")
        {
            this.Type = TexAtomType.Ordinary;
            this.BaseAtom = baseAtom;
            EncloseNotations = encNots.Split(' ');
        }

        /// <summary>
        /// Gets or sets the enclosed base <see cref="Atom"/>.
        /// </summary>
        public Atom BaseAtom
        {
            get;private set;
        }

        /// <summary>
        /// Gets or sets the types of enclosures that should be added to the <see cref="BaseAtom"/>.
        /// </summary>
        public string[] EncloseNotations
        {
            get; private set;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            var baseBox = this.BaseAtom == null ? StrutBox.Empty : this.BaseAtom.CreateBox(environment);

            foreach (var item in EncloseNotations)
            {
                switch (item)
                {
                    case "bottom":
                        {
                            baseBox = Enclosure_bottom(environment, baseBox);
                            break;
                        }
                    case "box":
                        {
                            baseBox = Enclosure_box(environment, baseBox);
                            break;
                        }
                    case "circle":
                    case "ellipse":
                    case "oval":
                        {
                            baseBox= Enclosure_circle(environment,baseBox);
                            break;
                        }
                    case "ddstrike":
                    case "downdiagonalstrike":
                        {
                            baseBox = Enclosure_downdiagonalstrike(environment, baseBox);
                            break;
                        }
                    case "horizontalstrike":
                    case "hstrike":
                    case "strikethrough":
                        {
                            baseBox= Enclosure_horizontalstrike(environment,baseBox);
                            break;
                        }
                    case "left":
                        {
                            baseBox = Enclosure_left(environment, baseBox);
                            break;
                        }
                    case "obar":
                    case "overbar":
                        {
                            baseBox = Enclosure_overbar(environment, baseBox);
                            break;
                        }
                    case "right":
                        {
                            baseBox = Enclosure_right(environment, baseBox);
                            break;
                        }
                    case "rbox":
                    case "roundedbox":
                        {
                            baseBox = Enclosure_roundedbox(environment, baseBox);
                            break;
                        }
                    case "top":
                        {
                            baseBox = Enclosure_top(environment, baseBox);
                            break;
                        }
                    case "udstrike":
                    case "updiagonalstrike":
                        {
                            baseBox = Enclosure_updiagonalstrike(environment, baseBox);
                            break;
                        }
                    case "ubar":
                    case "underbar":
                        {
                            baseBox = Enclosure_underbar(environment, baseBox);
                            break;
                        }
                    case "verticalstrike":
                    case "vstrike":
                        {
                            baseBox = Enclosure_verticalstrike(environment, baseBox);
                            break;
                        }

                    default:
                    {
                        string helpStr = TexFormulaParser.HelpOutMessage(item, AllowedEncloseNotations);
                         throw new TexParseException($"Unknown enclose notation: {item}{helpStr} ");
                    }
                }
            }
            return baseBox;
        }

        #region Enclosure Types Handlers
        private Box Enclosure_circle(TexEnvironment environment, Box morphbox)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var orule = new OvalRule(environment,morphbox.Height-morphbox.Depth,morphbox.Width, 0);
            // Create result box.
            var resultBox = new HorizontalBox();
            resultBox.Add(morphbox);
            resultBox.Add(new StrutBox(-morphbox.Width, -morphbox.Height, 0, 0));
            resultBox.Add(orule);
            
            return resultBox;
        }

        private  Box Enclosure_horizontalstrike(TexEnvironment environment, Box morphbox)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var hrule = new HorizontalRule(environment, defaultLineThickness, morphbox.Width, -axis + defaultLineThickness);
            var resbx = new HorizontalBox();
            resbx.Add(morphbox);
            resbx.Add(new StrutBox(-morphbox.Width, 0, 0, 0));
            resbx.Add(hrule);
            return resbx;
        }

        private Box Enclosure_left(TexEnvironment environment, Box morphbox)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var vrule = new VerticalRule(environment, defaultLineThickness, morphbox.Height + morphbox.Depth, -morphbox.Height);
            var resbx = new HorizontalBox();
            resbx.Add(morphbox);
            resbx.Add(new StrutBox(-morphbox.Width, -morphbox.Height, 0, 0));
            resbx.Add(vrule);
            return resbx;
        }

        private Box Enclosure_right(TexEnvironment environment, Box morphbox)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var vrule = new VerticalRule(environment, defaultLineThickness, morphbox.Height + morphbox.Depth, -morphbox.Height);
            var resbx = new HorizontalBox();
            resbx.Add(morphbox);
            resbx.Add(new StrutBox(morphbox.Width-vrule.Width, morphbox.Height, 0, 0));
            resbx.Add(vrule);
            return resbx;
        }

        private Box Enclosure_overbar(TexEnvironment environment, Box morphbox)
        {
            // Create result box.
            var defaultLineThickness = environment.MathFont.GetDefaultLineThickness(environment.Style);
            var resultBox = new OverBar(environment, morphbox, 3 * defaultLineThickness, defaultLineThickness)
            {
                // Adjust height and depth of result box.
                Height = morphbox.Height + 5 * defaultLineThickness,
                Depth = morphbox.Depth
            };
            return resultBox;
        }

        private Box Enclosure_top(TexEnvironment environment, Box morphbox)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var hrule = new HorizontalRule(environment, defaultLineThickness, morphbox.Width, -morphbox.TotalHeight);
            var resbx = new HorizontalBox();
            resbx.Add(morphbox);
            resbx.Add(new StrutBox(-morphbox.Width, -morphbox.Height-hrule.Height,0, 0));
            resbx.Add(hrule);
            return resbx;
        }

        private Box Enclosure_underbar(TexEnvironment environment, Box morphbox)
        {
            var defaultLineThickness = environment.MathFont.GetDefaultLineThickness(environment.Style);

            var resultBox = new UnderBar(environment, morphbox, 3 * defaultLineThickness, defaultLineThickness)
            {
                Height = morphbox.Height + 5 * defaultLineThickness,
                Depth = morphbox.Depth
            };
            return resultBox;
        }

        private Box Enclosure_bottom(TexEnvironment environment,Box morphbox)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var hrule = new HorizontalRule(environment, defaultLineThickness, morphbox.Width, 0);
            var resbx = new HorizontalBox();
            resbx.Add(morphbox);
            resbx.Add(new StrutBox(-morphbox.Width, -morphbox.Height + hrule.Height,0, 0));
            resbx.Add(hrule);
            return resbx;
        }

        private Box Enclosure_verticalstrike(TexEnvironment environment, Box morphbox)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var vrule = new VerticalRule(environment, defaultLineThickness, morphbox.Height+morphbox.Depth, -morphbox.Height);
            var resbx = new HorizontalBox();
            resbx.Add(morphbox);
            resbx.Add(new StrutBox(-morphbox.Width/2, -morphbox.Height, 0, 0));
            resbx.Add(vrule);
            return resbx;
        }

        private Box Enclosure_box(TexEnvironment environment, Box morphbox)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var recbox = new RectangleBox(environment, morphbox.Height+morphbox.Depth, morphbox.TotalWidth, 0);
            var resultbox = new HorizontalBox();
            resultbox.Add(morphbox);
            resultbox.Add(new StrutBox(-morphbox.Width, -morphbox.Height, 0, 0));
            resultbox.Add(recbox);
            return resultbox;
        }

        private Box Enclosure_roundedbox(TexEnvironment environment, Box morphbox)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var recbox = new RoundedRectangleBox(environment, morphbox.Height+morphbox.Depth, morphbox.TotalWidth, 0,4,4);
            var resultbox = new HorizontalBox();
            resultbox.Add(morphbox);
            resultbox.Add(new StrutBox(-morphbox.Width, -morphbox.Height, 0, 0));
            resultbox.Add(recbox);
            return resultbox;
        }

        private Box Enclosure_updiagonalstrike(TexEnvironment environment, Box morphbox)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var ddrule = new UpDiagonalRule(environment, 1, -morphbox.TotalWidth, morphbox.TotalHeight, 0);
            var resbx = new HorizontalBox();
            resbx.Add(morphbox);
            resbx.Add(new StrutBox(-morphbox.Width, -morphbox.Height, 0, 0));
            resbx.Add(ddrule);
            return resbx;
        }

        private Box Enclosure_downdiagonalstrike(TexEnvironment environment, Box morphbox)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var ddrule = new DownDiagonalRule(environment,1,-morphbox.TotalWidth,morphbox.TotalHeight, -morphbox.TotalHeight);
            var resbx = new HorizontalBox();
            resbx.Add(morphbox);
            resbx.Add(new StrutBox(-morphbox.Width, -morphbox.Height, 0, 0));
            resbx.Add(ddrule);
            return resbx;
        }

        

        #endregion


    }
}
