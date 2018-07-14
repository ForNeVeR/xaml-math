using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WpfMath.Converters
{
    /// <summary>
    /// Contains methods for converting a MathML file to a TexFormula.
    /// </summary>
    public class MathMLtoTexFormula
    {
        /// <summary>
        /// Initializes component.
        /// </summary>
        public MathMLtoTexFormula()
        {
            elements_Attribute.Add("mfenced", mfenced_Attributes);
            elements_Attribute.Add("mfrac", mfrac_Attributes);
            elements_Attribute.Add("mi", mi_Attributes);
            elements_Attribute.Add("mn", mn_Attributes);
            elements_Attribute.Add("mo", mo_Attributes);
            elements_Attribute.Add("mroot", mroot_Attributes);
            elements_Attribute.Add("mrow", mrow_Attributes);
            elements_Attribute.Add("msqrt", msqrt_Attributes);
            elements_Attribute.Add("msub", msub_Attributes);
            elements_Attribute.Add("msubsup", msubsup_Attributes);
            elements_Attribute.Add("msup", msup_Attributes);
            elements_Attribute.Add("mover", mover_Attributes);
            elements_Attribute.Add("munder", munder_Attributes);
            elements_Attribute.Add("munderover", munderover_Attributes);
        }

        /// <summary>
        /// Converts an XML-File containing MathML data to a <see cref="TexFormula"/>.
        /// </summary>
        /// <param name="mmlDoc">The mathml document.</param>
        /// <returns></returns>
        public string Parse(XmlDocument mmlDoc)
        {
            StringBuilder sb = new StringBuilder();
            //XmlUrlResolver resolver = new XmlUrlResolver
            //{
            //    Credentials = CredentialCache.DefaultCredentials
            //};
            //xmlDoc.CreateNode(XmlNodeType.EntityReference, "CenterDot", "http://www.w3.org/1998/Math/MathML");
            mmlDoc.CreateEntityReference("&CenterDot;");
            //xmlDoc.CreateEntityReference("CenterDot");
            //xmlDoc.CreateEntityReference("CenterDot;");
            //xmlDoc.CreateEntityReference("&CenterDot");

            if (mmlDoc.DocumentElement.NamespaceURI == "http://www.w3.org/1998/Math/MathML" &&mmlDoc.DocumentElement.Name == "math"&& mmlDoc.DocumentElement.HasChildNodes==true)
            {
                foreach (XmlNode item in mmlDoc.DocumentElement.ChildNodes)
                {
                    MathMLElements elementtype = GetElementType(item.Name);
                    
                    switch (elementtype)
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                        case MathMLElements.menclose:
                            sb.Append(Visiting_menclose(item));
                            break;
                        case MathMLElements.mfenced:
                            sb.Append( Visiting_mfenced(item));
                            break;
                        case MathMLElements.mfrac:
                            sb.Append( Visiting_mfrac(item));
                            break;
                        case MathMLElements.mi:
                            sb.Append( Visiting_mi(item));
                            break;
                        case MathMLElements.mmultiscripts:
                            sb.Append(Visiting_mmultiscripts(item));
                            break;
                        case MathMLElements.mn:
                            sb.Append(Visiting_mn(item));
                            break;
                        case MathMLElements.mo:
                            sb.Append(Visiting_mo(item));
                            break;
                        case MathMLElements.mphantom:
                            sb.Append(Visiting_mphantom(item));
                            break;
                        case MathMLElements.mroot:
                            sb.Append(Visiting_mroot(item));
                            break;
                        case MathMLElements.mrow:
                            sb.Append(Visiting_mrow(item));
                            break;
                        case MathMLElements.mspace:
                            sb.Append(Visiting_mspace(item));
                            break;
                        case MathMLElements.msqrt:
                            sb.Append(Visiting_msqrt(item));
                            break;
                        case MathMLElements.msub:
                            sb.Append(Visiting_msub(item));
                            break;
                        case MathMLElements.msubsup:
                            sb.Append(Visiting_msubsup(item));
                            break;
                        case MathMLElements.msup:
                            sb.Append(Visiting_msup(item));
                            break;
                        case MathMLElements.mtable:
                            sb.Append(Visiting_mtable(item));
                            break;
                        case MathMLElements.mtext:
                            sb.Append(Visiting_mtext(item));
                            break;
                        case MathMLElements.munder:
                            sb.Append(Visiting_munder(item));
                            break;
                        case MathMLElements.munderover:
                            sb.Append(Visiting_munderover(item));
                            break;



                        case MathMLElements.NONE:
                            break;
                        default:
                            break;
                    }

                }

            }

            return sb.ToString();
        }
        
        /// <summary>
        /// Converts an XML-File containing MathML data to a <see cref="TexFormula"/>.
        /// </summary>
        /// <param name="filepath">The file path of the Math ML data.</param>
        /// <returns></returns>
        public string Parse(string input,bool isFilePath)
        {
            if (isFilePath==true)
            {
                XmlDocument mmlDoc = new XmlDocument();
                mmlDoc.Load(input);
                return Parse(mmlDoc);
            }
            else
            {
                XmlDocument mmlDoc = new XmlDocument();
                mmlDoc.LoadXml(input);
                return Parse(mmlDoc);
            }
            
        }

        #region MML Parsing Helpers
        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.menclose"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Enclose content with a stretching symbol such as a long division sign.
        /// </summary>
        /// <param name="inputNode">The menclose node.</param>
        /// <returns></returns>
        private string Visiting_menclose(XmlNode inputNode)
        {
            if(inputNode.HasChildNodes == true && inputNode.Name == "menclose")
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"\enclose");
                if (inputNode.Attributes.Count>0)
                {
                    sb.Append("[" + inputNode.Attributes["notation"].Value + "]");
                }
                sb.Append("{");
                foreach (XmlNode item in inputNode.ChildNodes)
                {
                    switch (GetElementType(item.Name))
                    {
                        case MathMLElements.math:
                            {
                                throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                            }
                        case MathMLElements.menclose:
                            {
                                sb.Append(Visiting_menclose(item));
                                break;
                            }
                        case MathMLElements.mfenced:
                            {
                                sb.Append(Visiting_mfenced(item));
                                //rowparts += Visiting_mfenced(item);
                                break;
                            }
                        case MathMLElements.mfrac:
                            {
                                sb.Append(Visiting_mfrac(item));
                                //rowparts += Visiting_mfrac(item);
                                break;
                            }
                        case MathMLElements.mi:
                            {
                                sb.Append(Visiting_mi(item));
                                //rowparts += Visiting_mi(item);
                                break;
                            }
                        case MathMLElements.mmultiscripts:
                            {
                                sb.Append(Visiting_mmultiscripts(item));
                                //rowparts += Visiting_mmultiscripts(item);
                                break;
                            }
                        case MathMLElements.mn:
                            {
                                sb.Append(Visiting_mn(item));
                                //rowparts += Visiting_mn(item);
                                break;
                            }
                        case MathMLElements.mo:
                            {
                                sb.Append(Visiting_mo(item));
                                //rowparts += Visiting_mo(item);
                                break;
                            }
                        case MathMLElements.mphantom:
                            {
                                sb.Append(Visiting_mphantom(item));
                                //rowparts += Visiting_mphantom(item);
                                break;
                            }
                        case MathMLElements.mroot:
                            {
                                sb.Append(Visiting_mroot(item));
                                //rowparts += Visiting_mroot(item);
                                break;
                            }
                        case MathMLElements.mrow:
                            {
                                sb.Append(Visiting_mrow(item));
                                //rowparts += Visiting_mrow(item);
                                break;
                            }
                        case MathMLElements.mspace:
                            {
                                sb.Append(Visiting_mspace(item));
                                //rowparts += Visiting_mspace(item);
                                break;
                            }
                        case MathMLElements.msqrt:
                            {
                                sb.Append(Visiting_msqrt(item));
                                //rowparts += Visiting_msqrt(item);
                                break;
                            }
                        case MathMLElements.msub:
                            {
                                sb.Append(Visiting_msub(item));
                                //rowparts += Visiting_msub(item);
                                break;
                            }
                        case MathMLElements.msubsup:
                            {
                                sb.Append(Visiting_msubsup(item));
                                //rowparts += Visiting_msubsup(item);
                                break;
                            }
                        case MathMLElements.msup:
                            {
                                sb.Append(Visiting_msup(item));
                                //rowparts += Visiting_msup(item);
                                break;
                            }
                        case MathMLElements.mtable:
                            {
                                sb.Append(Visiting_mtable(item));
                                //rowparts += Visiting_mtable(item);
                                break;
                            }
                        case MathMLElements.mtext:
                            {
                                sb.Append(Visiting_mtext(item));
                                //rowparts += Visiting_mtext(item);
                                break;
                            }
                        case MathMLElements.munder:
                            {
                                sb.Append(Visiting_munder(item));
                                //rowparts += Visiting_munder(item);
                                break;
                            }
                        case MathMLElements.munderover:
                            {
                                sb.Append(Visiting_munderover(item));
                                //rowparts += Visiting_munderover(item);
                                break;
                            }

                        case MathMLElements.NONE:
                            {
                                //This means the element is not recognized
                                sb.Append(" ");
                                //rowparts += " ";
                                break;
                            }
                        default:
                            break;
                    }

                }
                sb.Append("}");
                return sb.ToString();
            }
            else
            {
                throw new InvalidOperationException("The menclose element must have at least one child node.");
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mfenced"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Surround content with a pair of fences.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mfenced(XmlNode inputNode)
        {
            if (inputNode.HasChildNodes==true && inputNode.Name == "mfenced")
            {
                string openDelim="";
                string closeDelim = "";
                string mfencedresult = "";
                List<string> mfencedResults = new List<string>();
                foreach (XmlNode item in inputNode.ChildNodes)
                {
                    switch (GetElementType(item.Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                        case MathMLElements.mfenced:
                            mfencedResults.Add(Visiting_mfenced(item));
                            break;
                        case MathMLElements.mfrac:
                            mfencedResults.Add(Visiting_mfrac(item));
                            break;
                        case MathMLElements.mi:
                            mfencedResults.Add(Visiting_mi(item));
                            break;
                        case MathMLElements.mmultiscripts:
                            mfencedResults.Add(Visiting_mmultiscripts(item));
                            break;
                        case MathMLElements.mn:
                            mfencedResults.Add(Visiting_mn(item));
                            break;
                        case MathMLElements.mo:
                            mfencedResults.Add(Visiting_mo(item));
                            break;
                        case MathMLElements.mphantom:
                            mfencedResults.Add(Visiting_mphantom(item));
                            break;
                        case MathMLElements.mroot:
                            mfencedResults.Add(Visiting_mroot(item));
                            break;
                        case MathMLElements.mrow:
                            mfencedResults.Add(Visiting_mrow(item));
                            break;
                        case MathMLElements.mspace:
                            mfencedResults.Add(Visiting_mspace(item));
                            break;
                        case MathMLElements.msqrt:
                            mfencedResults.Add(Visiting_msqrt(item));
                            break;
                        case MathMLElements.msub:
                            mfencedResults.Add(Visiting_msub(item));
                            break;
                        case MathMLElements.msubsup:
                            mfencedResults.Add(Visiting_msubsup(item));
                            break;
                        case MathMLElements.msup:
                            mfencedResults.Add(Visiting_msup(item));
                            break;
                        case MathMLElements.mtable:
                            mfencedResults.Add(Visiting_mtable(item));
                            break;
                        case MathMLElements.mtext:
                            mfencedResults.Add(Visiting_mtext(item));
                            break;
                        case MathMLElements.munder:
                            mfencedResults.Add(Visiting_munder(item));
                            break;
                        case MathMLElements.munderover:
                            mfencedResults.Add(Visiting_munderover(item));
                            break;


                        case MathMLElements.NONE:
                            throw new InvalidOperationException($"The element: {item.Name} is not supported.");
                        default:
                            break;
                    }
                }

                #region Attributes
                bool hasOpenDelim= Attribute_Exists(inputNode, "mfenced", "open");
                string str1 = Attribute_Value(inputNode, "mfenced", "open");
                bool hasCloseDelim = Attribute_Exists(inputNode, "mfenced", "close");
                string str2 = Attribute_Value(inputNode, "mfenced", "close");
                openDelim=(hasOpenDelim&&str1.Trim()!=""&&str1.Trim().Length==1)?@"\left"+str1:@"\left(";
                closeDelim=(hasCloseDelim&&str2.Trim()!=""&&str2.Trim().Length==1)?@"\right"+str2:@"\right)";
                
                bool hasSeparators = Attribute_Exists(inputNode, "mfenced", "separators");
                string separators = Attribute_Value(inputNode, "mfenced", "separators");
                char[] sepaArr =separators.Length>0? separators.ToCharArray():null;
                if (hasSeparators == true&&sepaArr!=null)
                {
                    if (sepaArr.Length >= mfencedResults.Count - 1)
                    {
                        mfencedresult += mfencedResults[0] + sepaArr[0];
                        for (int i = 1; i < mfencedResults.Count; i++)
                        {
                            mfencedresult += mfencedResults[i] + sepaArr[i - 1];
                        }
                        mfencedresult.TrimEnd(new char[] { sepaArr[sepaArr.Length - 1] });
                    }
                    else
                    {
                        List<char> sepList = new List<char>(); int a = 0;
                        char scapegoatChr = sepaArr[sepaArr.Length - 1];
                        while (sepList.Count != mfencedResults.Count)
                        {
                            if (sepList.Count <= sepaArr.Length)
                            {
                                sepList.Add(sepaArr[a]);
                            }
                            else
                            {
                                sepList.Add(scapegoatChr);
                            }
                            a = a < sepaArr.Length ? a++ : a;
                        }
                        mfencedresult += mfencedResults[0] + sepList[0];
                        for (int i = 1; i < mfencedResults.Count; i++)
                        {
                            mfencedresult += mfencedResults[i] + sepList[i - 1];
                        }
                        mfencedresult.TrimEnd(new char[] { sepList[sepList.Count - 1] });
                    }
                }
                else
                {
                    for (int i = 0; i < mfencedResults.Count; i++)
                    {
                        var item = mfencedResults[i];
                        if (i<mfencedResults.Count-1)
                        {
                            mfencedresult += item + ",";
                        }
                        else
                        {
                            mfencedresult += item ;
                        }
                    }
                    
                }

                #endregion

                return openDelim + mfencedresult + closeDelim;
            }
            else
            {
                string openDelim =  @"\left(" ;
                string closeDelim =  @"\right)";
                return openDelim + closeDelim;
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mfrac"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Form a fraction from two sub-expressions.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mfrac(XmlNode inputNode)
        {
            if (inputNode.ChildNodes.Count==2 && inputNode.Name == "mfrac")
            {
                StringBuilder sb = new StringBuilder(@"\frac");
                
                string numerator = "";
                string denominator = "";
                for (int i = 0; i < 2; i++)
                {
                    switch (GetElementType(inputNode.ChildNodes[i].Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                        case MathMLElements.menclose:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfenced:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfrac:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mi:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mmultiscripts:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mn:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mo:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mphantom:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mroot:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mrow:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msqrt:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msub:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msubsup:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msup:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtable:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtext:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munder:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munderover:
                            {
                                if (i == 0)
                                {
                                    numerator += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    denominator += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                break;
                            }



                        case MathMLElements.NONE:
                            throw new InvalidOperationException($"The element: {inputNode.ChildNodes[i].Name} is not yet supported.");
                        default:
                            break;
                    }
                }

                sb.Append("{" + numerator + "}" + "{" + denominator + "}");
                return sb.ToString();
            }
            else
            {
                //throw an exception, >>"msqrt" cannot contain >1 child nodes.
                throw new FormatException(@"The mfrac element must contain 2 child nodes.");

            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mi"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Identifier.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mi(XmlNode inputNode)
        {
            if (inputNode.HasChildNodes==true&& inputNode.Name == "mi")
            {
                //Check if the next item has been registered in the symbols dictionary
                string miStr = "";
                if (MathSymbolsDict.ContainsKey(inputNode.FirstChild.Value.Trim()))
                {
                    miStr += MathSymbolsDict[inputNode.FirstChild.Value.Trim()];
                }
                else
                {
                    miStr += inputNode.FirstChild.Value.Trim();
                }
                //send---moStr---to the output
                return miStr;
                
            }
            else
            {
                //throw an exception, >>"mi" cannot contain be empty.
                throw new FormatException(@"The mi element cannot contain child nodes.");
                
            }
        }

        private void Visiting_msline(XmlNode inputNode)
        {
            if (inputNode.Name == "mline" && inputNode.ChildNodes.Count == 0)
            {

            }
            else
            {
                throw new FormatException(@"The mline element cannot contain child nodes.");
            }  
        }

        private void Visiting_mlongdiv(XmlNode inputNode)
        {
            if (inputNode.Name=="mlongdiv"&&inputNode.ChildNodes.Count>=3)
            {
                //Not yet implemented in the rendering program.
                string divisorStr = "";
                string dividendStr = "";
                string resultStr = "";
                string stepsStr = "";


                //return
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mmultiscripts"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/>CMD: Attach prescripts and tensor indices to a base.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mmultiscripts(XmlNode inputNode)
        {
            /* <mmultiscripts>
             * base
             * ( subscript superscript )*
             * [ <mprescripts/> ( presubscript presuperscript )* ]
             * </mmultiscripts>
             */
            if (inputNode.Name == "mmultiscripts" && inputNode.HasChildNodes == true)
            {
                List<string> bazsubsupscr = new List<string>();
                bool _prescriptactive = false;
                List<string> presubsupscr = new List<string>();
                foreach (XmlNode item in inputNode.ChildNodes)
                {
                    switch (GetElementType(item.Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                        case MathMLElements.mfenced:
                            {
                                //this is not a good idea
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_mfenced(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_mfenced(item));
                                }
                                break;
                            }
                        case MathMLElements.mfrac:
                            {
                                //this is not a good idea
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_mfrac(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_mfrac(item));
                                }
                                break;
                            }
                        case MathMLElements.mi:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_mi(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_mi(item));
                                }
                                break;
                            }
                        case MathMLElements.mmultiscripts:
                            {
                                //This is a very bad idea
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_mmultiscripts(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_mmultiscripts(item));
                                }
                                break;
                            }
                        case MathMLElements.mn:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_mn(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_mn(item));
                                }
                                break;
                            }
                        case MathMLElements.mo:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_mo(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_mo(item));
                                }
                                break;
                            }
                        case MathMLElements.mover:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_mover(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_mover(item));
                                }
                                break;
                            }
                        case MathMLElements.mphantom:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_mphantom(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_mphantom(item));
                                }
                                break;
                            }
                        case MathMLElements.mprescripts:
                            {
                                _prescriptactive = true;
                                presubsupscr.Add("{}");
                                break;
                            }
                        case MathMLElements.mroot:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_mroot(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_mroot(item));
                                }
                                break;
                            }
                        case MathMLElements.mrow:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_mrow(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_mrow(item));
                                }
                                break;
                            }
                        case MathMLElements.mspace:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_mspace(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_mspace(item));
                                }
                                break;
                            }
                        case MathMLElements.msqrt:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_msqrt(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_msqrt(item));
                                }
                                break;
                            }
                        case MathMLElements.msub:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_msub(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_msub(item));
                                }
                                break;
                            }
                        case MathMLElements.msubsup:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_msubsup(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_msubsup(item));
                                }
                                break;
                            }
                        case MathMLElements.msup:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_msup(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_msup(item));
                                }
                                break;
                            }
                        case MathMLElements.mtext:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_mtext(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_mtext(item));
                                }
                                break;
                            }
                        case MathMLElements.munder:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_munder(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_munder(item));
                                }
                                break;
                            }
                        case MathMLElements.munderover:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_munderover(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_munderover(item));
                                }
                                break;
                            }
                        case MathMLElements.none:
                            {
                                if (_prescriptactive == false)
                                {
                                    bazsubsupscr.Add(Visiting_none(item));
                                }
                                else
                                {
                                    presubsupscr.Add(Visiting_none(item));
                                }
                                break;
                            }
                        case MathMLElements.NONE:
                            break;
                        default:
                            break;
                    }
                }

                string baseStr = bazsubsupscr.Count>1? bazsubsupscr[0]:"";    string postStr = "";    string preStr =presubsupscr.Count>1? presubsupscr[0]:"";

                if (bazsubsupscr.Count>=2)
                {
                    for (int i = 1; i < bazsubsupscr.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            postStr += "^" + bazsubsupscr[i]+" \\thinspace";
                        }
                        else
                        {
                            postStr += "_" + bazsubsupscr[i];
                        }
                    }

                }

                if (presubsupscr.Count >= 2)
                {
                    for (int i = 1; i < presubsupscr.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            preStr += "^" + presubsupscr[i];
                        }
                        else
                        {
                            preStr += "_" + presubsupscr[i];
                        }
                    }

                }


                return preStr + baseStr + postStr;
            }
            else
            {
                throw new FormatException(@"The mmultiscripts element must contain child nodes.");
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mn"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Number.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mn(XmlNode inputNode)
        {
            if (inputNode.HasChildNodes==true && inputNode.Name == "mn")
            {
                //send---Value---to the output
                return inputNode.FirstChild.Value.Trim();
            }
            else
            {
                //throw an exception, >>"mn" cannot contain child nodes.
                throw new FormatException(@"The mn element cannot contain child nodes.");
                
            }
        }

        //Operator dictionary entries(Entity char ref)------>>Page 341-378 of MathML Documentation

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mo"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Operator, fence, or separator.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mo(XmlNode inputNode)
        {
            if (inputNode.HasChildNodes==true&&inputNode.Name=="mo")
            {
                 //string strchk=inputNode.OuterXml;
                //MessageBox.Show(strchk);
                //Compare the value in the Operator dictionary
                // and get its representative.
                string moStr = "";
                //inputNode.FirstChild.Value;
                //if (inputNode.FirstChild.NodeType==XmlNodeType.EntityReference)
                //TODO: create entity references for dtd unsupported entities.
                //Need to trim start and end of the "mo" value
                if (MathOperatorsDict.ContainsKey(inputNode.FirstChild.Value.Trim()))
                {
                    moStr += MathOperatorsDict[inputNode.FirstChild.Value.Trim()];
                }
                else
                {
                    moStr += inputNode.FirstChild.Value.Trim();
                    
                }
                //send---moStr---to the output
                return moStr;
            }
            else
            {
                throw new FormatException(@"The mo element cannot contain child elements.");
                
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mover"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Attach an overscript to a base.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mover(XmlNode inputNode)
        {
            string result = "";
            if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count == 2 && inputNode.Name == "msup")
            {
                string mainStr = "";
                string supStr = "";
                for (int i = 0; i < 2; i++)
                {
                    switch (GetElementType(inputNode.ChildNodes[i].Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                        case MathMLElements.menclose:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfenced:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfrac:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mi:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mmultiscripts:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mn:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mo:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mphantom:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mroot:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mrow:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mspace:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msqrt:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msub:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msubsup:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msup:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtable:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtext:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munder:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munderover:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                break;
                            }


                        case MathMLElements.NONE:
                            throw new InvalidOperationException($"The element: {inputNode.ChildNodes[i].Name} is not yet supported.");
                        default:
                            break;
                    }
                }

                result = mainStr + "^{" + supStr + "}";
            }
            else
            {
                if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count != 2)
                {
                    throw new InvalidOperationException("The msup element must have only two child elements or nodes.");
                }
                if (inputNode.HasChildNodes == false)
                {
                    result = @"{ \thinspace }";
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mphantom"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Make content invisible but preserve its size.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mphantom(XmlNode inputNode)
        {
            if (inputNode.Name=="mphantom"&& inputNode.HasChildNodes == true && inputNode.ChildNodes.Count >= 1)
            {
                string hiddenparts = "";
                foreach (XmlNode item in inputNode.ChildNodes)
                {
                    switch (GetElementType(item.Name))
                    {
                        case MathMLElements.math:
                            {
                                throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                            }
                        case MathMLElements.menclose:
                            {
                                hiddenparts += Visiting_menclose(item);
                                break;
                            }
                        case MathMLElements.mfenced:
                            {
                                hiddenparts += Visiting_mfenced(item);
                                break;
                            }
                        case MathMLElements.mfrac:
                            {
                                hiddenparts += Visiting_mfrac(item);
                                break;
                            }
                        case MathMLElements.mi:
                            {
                                hiddenparts += Visiting_mi(item);
                                break;
                            }
                        case MathMLElements.mmultiscripts:
                            {
                                hiddenparts += Visiting_mmultiscripts(item);
                                break;
                            }
                        case MathMLElements.mn:
                            {
                                hiddenparts += Visiting_mn(item);
                                break;
                            }
                        case MathMLElements.mo:
                            {
                                hiddenparts += Visiting_mo(item);
                                break;
                            }
                        case MathMLElements.mphantom:
                            {
                                //obsolete
                                hiddenparts += Visiting_mphantom(item);
                                break;
                            }
                        case MathMLElements.mroot:
                            {
                                hiddenparts += Visiting_mroot(item);
                                break;
                            }
                        case MathMLElements.mrow:
                            {
                                hiddenparts += Visiting_mrow(item);
                                break;
                            }
                        case MathMLElements.mspace:
                            {
                                hiddenparts += Visiting_mspace(item);
                                break;
                            }
                        case MathMLElements.msqrt:
                            {
                                hiddenparts += Visiting_msqrt(item);
                                break;
                            }
                        case MathMLElements.msub:
                            {
                                hiddenparts += Visiting_msub(item);
                                break;
                            }
                        case MathMLElements.msubsup:
                            {
                                hiddenparts += Visiting_msubsup(item);
                                break;
                            }
                        case MathMLElements.msup:
                            {
                                hiddenparts += Visiting_msup(item);
                                break;
                            }
                        case MathMLElements.mtable:
                            {
                                hiddenparts += Visiting_mtable(item);
                                break;
                            }
                        case MathMLElements.mtext:
                            {
                                hiddenparts += Visiting_mtext(item);
                                break;
                            }
                        case MathMLElements.munder:
                            {
                                hiddenparts += Visiting_munder(item);
                                break;
                            }
                        case MathMLElements.munderover:
                            {
                                hiddenparts += Visiting_munderover(item);
                                break;
                            }

                        case MathMLElements.NONE:
                            {
                                //This means the element is not recognized
                                hiddenparts += " ";
                                break;
                            }
                        default:
                            break;
                    }
                }

                return @"\phantom{ " + hiddenparts + " }";
            }
            else
            {
                throw new InvalidOperationException("The mphantom element must have at least one child elements or nodes.");
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mroot"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Form a radical with specified index.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mroot(XmlNode inputNode)
        {
            string result = @"\sqrt";
            if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count == 2 && inputNode.Name == "mroot")
            {
                string mrootBaseStr = "";
                string mrootRadStr = "";
                for (int i = 0; i < 2; i++)
                {
                    switch (GetElementType(inputNode.ChildNodes[i].Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                        case MathMLElements.menclose:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfenced:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfrac:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mi:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mmultiscripts:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mn:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mo:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mphantom:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mroot:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mrow:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mspace:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msqrt:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msub:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msubsup:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msup:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtable:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtext:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munder:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munderover:
                            {
                                if (i == 0)
                                {
                                    mrootBaseStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    mrootRadStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                break;
                            }


                        case MathMLElements.NONE:
                            throw new InvalidOperationException($"The element: {inputNode.ChildNodes[i].Name} is not yet supported.");
                        default:
                            break;
                    }
                }

                result += "[" + mrootRadStr + "]{" + mrootBaseStr + "}";
            }
            else
            {
                if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count != 2)
                {
                    throw new InvalidOperationException("The mroot element must have only two child elements or nodes");
                }
                else if (inputNode.HasChildNodes == false)
                {
                    result = @"{ \thinspace }";
                }
            }
            return result;

        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mrow"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Group any number of sub-expressions horizontally.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mrow(XmlNode inputNode)
        {
            
            if (inputNode.HasChildNodes==true && inputNode.Name == "mrow")
            {
                StringBuilder sb = new StringBuilder("{");
                //string rowparts = "";
                foreach (XmlNode item in inputNode.ChildNodes)
                {
                    switch (GetElementType(item.Name))
                    {
                        case MathMLElements.math:
                            {
                                throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                            }
                        case MathMLElements.menclose:
                            {
                                sb.Append(Visiting_menclose(item));
                                //rowparts += Visiting_mfenced(item);
                                break;
                            }
                        case MathMLElements.mfenced:
                            {
                                sb.Append(Visiting_mfenced(item));
                                //rowparts += Visiting_mfenced(item);
                                break;
                            }
                        case MathMLElements.mfrac:
                            {
                                sb.Append(Visiting_mfrac(item));
                                //rowparts += Visiting_mfrac(item);
                                break;
                            }
                        case MathMLElements.mi:
                            {
                                sb.Append(Visiting_mi(item));
                                //rowparts += Visiting_mi(item);
                                break;
                            }
                        case MathMLElements.mmultiscripts:
                            {
                                sb.Append(Visiting_mmultiscripts(item));
                                //rowparts += Visiting_mmultiscripts(item);
                                break;
                            }
                        case MathMLElements.mn:
                            {
                                sb.Append(Visiting_mn(item));
                                //rowparts += Visiting_mn(item);
                                break;
                            }
                        case MathMLElements.mo:
                            {
                                sb.Append(Visiting_mo(item));
                                //rowparts += Visiting_mo(item);
                                break;
                            }
                        case MathMLElements.mphantom:
                            {
                                sb.Append(Visiting_mphantom(item));
                                //rowparts += Visiting_mphantom(item);
                                break;
                            }
                        case MathMLElements.mroot:
                            {
                                sb.Append(Visiting_mroot(item));
                                //rowparts += Visiting_mroot(item);
                                break;
                            }
                        case MathMLElements.mrow:
                            {
                                sb.Append(Visiting_mrow(item));
                                //rowparts += Visiting_mrow(item);
                                break;
                            }
                        case MathMLElements.mspace:
                            {
                                sb.Append(Visiting_mspace(item));
                                //rowparts += Visiting_mspace(item);
                                break;
                            }
                        case MathMLElements.msqrt:
                            {
                                sb.Append(Visiting_msqrt(item));
                                //rowparts += Visiting_msqrt(item);
                                break;
                            }
                        case MathMLElements.msub:
                            {
                                sb.Append(Visiting_msub(item));
                                //rowparts += Visiting_msub(item);
                                break;
                            }
                        case MathMLElements.msubsup:
                            {
                                sb.Append(Visiting_msubsup(item));
                                //rowparts += Visiting_msubsup(item);
                                break;
                            }
                        case MathMLElements.msup:
                            {
                                sb.Append(Visiting_msup(item));
                                //rowparts += Visiting_msup(item);
                                break;
                            }
                        case MathMLElements.mtable:
                            {
                                sb.Append(Visiting_mtable(item));
                                //rowparts += Visiting_mtable(item);
                                break;
                            }
                        case MathMLElements.mtext:
                            {
                                sb.Append(Visiting_mtext(item));
                                //rowparts += Visiting_mtext(item);
                                break;
                            }
                        case MathMLElements.munder:
                            {
                                sb.Append(Visiting_munder(item));
                                //rowparts += Visiting_munder(item);
                                break;
                            }
                        case MathMLElements.munderover:
                            {
                                sb.Append(Visiting_munderover(item));
                                //rowparts += Visiting_munderover(item);
                                break;
                            }

                        case MathMLElements.NONE:
                            {
                                //This means the element is not recognized
                                sb.Append(" ");
                                //rowparts += " ";
                                break;
                            }
                        default:
                            break;
                    }
                }
                sb.Append("}");
                //return "{ " + rowparts + " }";
                return sb.ToString();
            }
            else
            {
                return "{ \thinspace }";
            }
        }
        
        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mspace"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Space.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mspace(XmlNode inputNode)
        {
            if (inputNode.Name=="mspace"&&inputNode.HasChildNodes==false)
            {
                //certain attributes like width, height, etc aren't supported
                return "\thickspace";
            }
            else
            {
                throw new FormatException(@"The mspace element cannot contain child nodes.");

            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.msqrt"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Form a square root (radical without an index).
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_msqrt(XmlNode inputNode)
        {
            
            if (inputNode.ChildNodes.Count>0 && inputNode.Name == "msqrt")
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"\sqrt{");
                //msqrt normally contains one child, but a row or root can let it contain more.
                XmlNode item = inputNode.FirstChild;
                    switch (GetElementType(item.Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                    case MathMLElements.menclose:
                        sb.Append(Visiting_menclose(item));
                        break;
                    case MathMLElements.mfenced:
                            sb.Append( Visiting_mfenced(item));
                            break;
                        case MathMLElements.mfrac:
                        sb.Append(Visiting_mfrac(item));
                            break;
                        case MathMLElements.mi:
                        sb.Append(Visiting_mi(item));
                            break;
                    case MathMLElements.mmultiscripts:
                        sb.Append(Visiting_mmultiscripts(item));
                        break;
                    case MathMLElements.mn:
                        sb.Append(Visiting_mn(item));
                            break;
                        case MathMLElements.mo:
                        sb.Append(Visiting_mo(item));
                            break;
                    case MathMLElements.mphantom:
                        sb.Append(Visiting_mphantom(item));
                        break;
                    case MathMLElements.mroot:
                        sb.Append(Visiting_mroot(item));
                            break;
                        case MathMLElements.mrow:
                        sb.Append(Visiting_mrow(item));
                            break;
                    case MathMLElements.mspace:
                        sb.Append(Visiting_mspace(item));
                        break;
                    case MathMLElements.msqrt:
                        sb.Append(Visiting_msqrt(item));
                            break;
                        case MathMLElements.msub:
                        sb.Append(Visiting_msub(item));
                            break;
                        case MathMLElements.msubsup:
                        sb.Append(Visiting_msubsup(item));
                            break;
                        case MathMLElements.msup:
                        sb.Append(Visiting_msup(item));
                            break;
                    case MathMLElements.mtable:
                        sb.Append(Visiting_mtable(item));
                        break;
                    case MathMLElements.mtext:
                        sb.Append(Visiting_mtext(item));
                        break;
                    case MathMLElements.munder:
                        sb.Append(Visiting_munder(item));
                            break;
                    case MathMLElements.munderover:
                        sb.Append(Visiting_munderover(item));
                        break;



                    case MathMLElements.NONE:
                            throw new InvalidOperationException($"The element: {item.Name} is not supported.");
                        default:
                            break;
                    }
                sb.Append("}");
                return sb.ToString();

            }
            else
            {
                throw new FormatException(@"The msqrt element must contain at least 1 child node.");

            }

        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.msub"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Attach a subscript to a base.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_msub(XmlNode inputNode)
        {
            string result = "";
            if (inputNode.HasChildNodes==true&&inputNode.ChildNodes.Count==2 && inputNode.Name == "msub")
            {
                string mainStr = "";
                string subStr = "";
                for (int i = 0; i < 2; i++)
                {
                    switch (GetElementType(inputNode.ChildNodes[i].Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                        case MathMLElements.menclose:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfenced:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfrac:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mi:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mmultiscripts:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mn:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mo:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mphantom:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mroot:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mrow:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msqrt:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msub:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msubsup:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msup:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtable:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtext:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munder:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munderover:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                break;
                            }



                        case MathMLElements.NONE:
                            throw new InvalidOperationException($"The element: {inputNode.ChildNodes[i].Name} is not yet supported.");
                        default:
                            break;
                    }
                }
                result = "{" + mainStr + "}_{" + subStr + "}";
            }
            else
            {
                if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count != 2)
                {
                    throw new InvalidOperationException("The msub element must have only two child elements or nodes");
                }
                else if (inputNode.HasChildNodes == false)
                {
                    result = @"{ \thinspace }";
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.msubsup"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Attach a subscript-superscript pair to a base.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_msubsup(XmlNode inputNode)
        {
            string result = "";
            if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count == 3 && inputNode.Name == "msubsup")
            {
                string mainStr = "";
                string subStr = "";
                string supStr = "";
                for (int i = 0; i < 3; i++)
                {
                    switch (GetElementType(inputNode.ChildNodes[i].Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException();
                        case MathMLElements.menclose:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfenced:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfrac:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mi:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mmultiscripts:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mn:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mo:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr+= Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mphantom:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mroot:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mrow:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mspace:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msqrt:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msub:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msubsup:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msup:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtable:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtext:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munder:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munderover:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        
                        
                        case MathMLElements.NONE:
                            throw new InvalidOperationException($"The element: {inputNode.ChildNodes[i].Name} is not yet supported.");
                        default:
                            break;
                    }
                }
                result = "{" + mainStr + "}_{" + subStr + "}^{"+supStr+"}";
            }
            else
            {
                if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count != 3)
                {
                    throw new InvalidOperationException("The msubsup element must have only three child elements or nodes");
                }
                if (inputNode.HasChildNodes == false)
                {
                    result = @"{ \thinspace }";
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.msup"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Attach a superscript to a base.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_msup(XmlNode inputNode)
        {
            string result = "";
            if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count == 2 && inputNode.Name == "msup")
            {
                string mainStr = "";
                string supStr = "";
                for (int i = 0; i < 2; i++)
                {
                    switch (GetElementType(inputNode.ChildNodes[i].Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                        case MathMLElements.menclose:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfenced:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfrac:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mi:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mmultiscripts:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mn:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mo:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mphantom:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mroot:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mrow:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mspace:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msqrt:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msub:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msubsup:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msup:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtable:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtext:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munder:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munderover:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        
                        
                        case MathMLElements.NONE:
                            throw new InvalidOperationException($"The element: {inputNode.ChildNodes[i].Name} is not yet supported.");
                        default:
                            break;
                    }
                }

                result = "{" + mainStr + "}^{" + supStr + "}";
            }
            else
            {
                if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count != 2)
                {
                    throw new InvalidOperationException("The msup element must have only two child elements or nodes.");
                }
                if (inputNode.HasChildNodes == false)
                {
                    result = @"{ \thinspace }";
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mtable"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Table or matrix.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mtable(XmlNode inputNode)
        {
            if (inputNode.Name=="mtable"&&inputNode.HasChildNodes==true)
            {
                int rowcount = inputNode.ChildNodes.Count;
                int columncount = 0;
                List<int> tblcolnos = new List<int>();
                List<string> tablerows = new List<string>();
                foreach (XmlNode item in inputNode.ChildNodes)
                {
                    if (item.Name=="mtr")
                    {
                        int a= item.HasChildNodes ? item.ChildNodes.Count : 0;
                        tblcolnos.Add(a);
                        columncount = item.HasChildNodes ? item.ChildNodes.Count : 0;
                        tablerows.Add(Visiting_mtr(item));
                    }
                    else if (item.Name == "mlabelledtr")
                    {
                        //Not yet implemented
                    }
                    else
                    {
                        throw new InvalidOperationException("The mtable element can only have mtr or mlabelledtr as its descendants.");
                    }
                }
                if (tblcolnos.Count>2)
                {
                    for (int i = 1; i < tblcolnos.Count; i++)
                    {
                        if (tblcolnos[i]==tblcolnos[i-1])
                        {
                            continue;
                        }
                        else
                        {
                            throw new InvalidOperationException("The columns in the table rows are unequal.");
                        }
                    }
                }
                string result = $"\\table[{rowcount},{columncount}]";
                foreach (var item in tablerows)
                {
                    result +=  item ;
                }

                return "{"+result+"}" ;
            }
            else
            {
                return @"\table[0,0]";
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mtd"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: One entry in a table or matrix.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mtd(XmlNode inputNode)
        {
            if (inputNode.Name=="mtd"&&inputNode.HasChildNodes==true)
            {
                string result = @"";
                foreach (XmlNode item in inputNode.ChildNodes)
                {
                    switch (GetElementType(item.Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException("The math element only occurs once in a mathml file, as a top-level element.");
                        case MathMLElements.menclose:
                            result += Visiting_menclose(item);
                            break;
                        case MathMLElements.mfenced:
                            result += Visiting_mfenced(item);
                            break;
                        case MathMLElements.mfrac:
                            result += Visiting_mfrac(item);
                            break;
                        case MathMLElements.mi:
                            result += Visiting_mi(item);
                            break;
                        case MathMLElements.mmultiscripts:
                            result += Visiting_mmultiscripts(item);
                            break;
                        case MathMLElements.mn:
                            result += Visiting_mn(item);
                            break;
                        case MathMLElements.mo:
                            result += Visiting_mo(item);
                            break;
                        case MathMLElements.mover:
                            result += Visiting_mover(item);
                            break;
                        case MathMLElements.mphantom:
                            result += Visiting_mphantom(item);
                            break;
                        case MathMLElements.mprescripts:
                            throw new InvalidOperationException("The mprescripts element can only have mmultiscripts as its parent.");
                        case MathMLElements.mroot:
                            result += Visiting_mroot(item);
                            break;
                        case MathMLElements.mrow:
                            result += Visiting_mrow(item);
                            break;
                        case MathMLElements.mspace:
                            result += Visiting_mspace(item);
                            break;
                        case MathMLElements.msqrt:
                            result += Visiting_msqrt(item);
                            break;
                        case MathMLElements.msub:
                            result += Visiting_msub(item);
                            break;
                        case MathMLElements.msubsup:
                            result += Visiting_msubsup(item);
                            break;
                        case MathMLElements.msup:
                            result += Visiting_msup(item);
                            break;
                        case MathMLElements.mtable:
                            result += Visiting_mtable(item);
                            break;
                        case MathMLElements.mtd:
                            result += Visiting_mtd(item);
                            break;
                        case MathMLElements.mtext:
                            result += Visiting_mtext(item);
                            break;
                        case MathMLElements.mtr:
                            result += Visiting_mtr(item);
                            break;
                        case MathMLElements.munder:
                            result += Visiting_munder(item);
                            break;
                        case MathMLElements.munderover:
                            result += Visiting_munderover(item);
                            break;
                        case MathMLElements.none:
                            throw new InvalidOperationException("Tensor indices can only be used within a mmultiscripts element.");
                        case MathMLElements.NONE:
                            break;
                        default:
                            break;
                    }
                }

                return result;
            }
            else
            {
                throw new InvalidOperationException("The mtd element must have at least 1 descendant.");
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mtext"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Text.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mtext(XmlNode inputNode)
        {
            if (inputNode.HasChildNodes==true)
            {
                if (inputNode.FirstChild.HasChildNodes==false)
                {
                    return @"\text{"+ inputNode.FirstChild.Value+"}";
                }
                else if (inputNode.FirstChild.Name=="input"&&inputNode.FirstChild.Attributes["type"].Value=="text")
                {
                    //NOTE: Only used in a html context.
                    return @"\text{" + inputNode.FirstChild.Attributes["placeholder"].Value + "}";
                }
                else
                {
                    string textchild = "";
                    XmlNode item = inputNode.FirstChild;
                    switch (GetElementType(item.Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException("The math element only occurs once in a mathml file.");
                        case MathMLElements.menclose:
                            textchild += Visiting_menclose(item);
                            break;
                        case MathMLElements.mfenced:
                            textchild += Visiting_mfenced(item);
                            break;
                        case MathMLElements.mfrac:
                            textchild += Visiting_mfrac(item);
                            break;
                        case MathMLElements.mi:
                            textchild += Visiting_mi(item);
                            break;
                        case MathMLElements.mmultiscripts:
                            textchild += Visiting_mmultiscripts(item);
                            break;
                        case MathMLElements.mn:
                            textchild += Visiting_mn(item);
                            break;
                        case MathMLElements.mo:
                            textchild += Visiting_mo(item);
                            break;
                        case MathMLElements.mphantom:
                            textchild += Visiting_mphantom(item);
                            break;
                        case MathMLElements.mroot:
                            textchild += Visiting_mroot(item);
                            break;
                        case MathMLElements.mrow:
                            textchild += Visiting_mrow(item);
                            break;
                        case MathMLElements.mspace:
                            textchild += Visiting_mspace(item);
                            break;
                        case MathMLElements.msqrt:
                            textchild += Visiting_msqrt(item);
                            break;
                        case MathMLElements.msub:
                            textchild += Visiting_msub(item);
                            break;
                        case MathMLElements.msubsup:
                            textchild += Visiting_msubsup(item);
                            break;
                        case MathMLElements.msup:
                            textchild += Visiting_msup(item);
                            break;
                        case MathMLElements.mtable:
                            textchild += Visiting_mtable(item);
                            break;
                        case MathMLElements.mtext:
                            textchild += Visiting_mtext(item);
                            break;
                        case MathMLElements.munder:
                            textchild += Visiting_munder(item);
                            break;
                        case MathMLElements.munderover:
                            textchild += Visiting_munderover(item);
                            break;



                        case MathMLElements.NONE:
                            throw new InvalidOperationException($"The element: {item.Name} is not supported.");
                        default:
                            break;
                    }

                    return textchild;
                }
            }
            else
            {
                throw new InvalidOperationException("The mtext attribute does not contain anything");
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.mtr"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Row in a table or matrix.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_mtr(XmlNode inputNode)
        {
            if (inputNode.Name=="mtr"&&inputNode.HasChildNodes==true)
            {
                string mtrStr = "";
                List<string> rowcells = new List<string>();
                foreach (XmlNode item in inputNode.ChildNodes)
                {
                    if (item.Name=="mtd")
                    {
                        rowcells.Add(Visiting_mtd(item));
                    }
                    else
                    {
                        throw new InvalidOperationException("The mtr element can only contain mtd elements.");
                    }
                }
                foreach (var item in rowcells)
                {
                    mtrStr +="{"+ item+"}";
                }
                return mtrStr;
            }
            else
            {
                //No need for curly braces since mtable handles it.
                return " ";
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.munder"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Attach an underscript to a base.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_munder(XmlNode inputNode)
        {
            string result = "";
            if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count == 2 && inputNode.Name == "munder")
            {
                string mainStr = "";
                string subStr = "";
                for (int i = 0; i < 2; i++)
                {
                    switch (GetElementType(inputNode.ChildNodes[i].Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException("Throw an error, the math element only occurs once in a mathml file.");
                        case MathMLElements.menclose:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfenced:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfrac:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mi:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mmultiscripts:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mn:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mo:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mphantom:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mroot:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mrow:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mspace:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msqrt:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msub:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msubsup:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msup:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtable:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtext:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munder:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munderover:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    subStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                break;
                            }



                        case MathMLElements.NONE:
                            throw new InvalidOperationException($"The element: {inputNode.ChildNodes[i].Name} is not yet supported.");
                        default:
                            break;
                    }
                }


                result = mainStr + "_{" + subStr + "}";
            }
            else
            {
                if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count != 2)
                {
                    throw new InvalidOperationException("The munder element must have only two child elements or nodes");
                }
                if (inputNode.HasChildNodes == false)
                {
                    result = @"{ \thinspace }";
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="MathMLElements.munderover"/> <paramref name="inputNode"/>, its attributes and its childnodes.
        /// <para/> CMD: Attach an underscript-overscript pair to a base.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private string Visiting_munderover(XmlNode inputNode)
        {
            string result = "";
            if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count == 3 && inputNode.Name == "munderover")
            {
                string mainStr = "";
                string subStr = "";
                string supStr = "";
                for (int i = 0; i < 3; i++)
                {
                    switch (GetElementType(inputNode.ChildNodes[i].Name))
                    {
                        case MathMLElements.math:
                            throw new InvalidOperationException();
                        case MathMLElements.menclose:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_menclose(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfenced:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mfenced(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mfrac:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mfrac(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mi:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mi(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mmultiscripts:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mmultiscripts(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mn:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mn(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mo:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mo(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mphantom:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mphantom(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mroot:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mroot(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mrow:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mrow(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mspace:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mspace(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msqrt:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msqrt(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msub:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msub(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msubsup:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msubsup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.msup:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_msup(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtable:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mtable(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.mtext:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_mtext(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munder:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_munder(inputNode.ChildNodes[i]);
                                }
                                break;
                            }
                        case MathMLElements.munderover:
                            {
                                if (i == 0)
                                {
                                    mainStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                else if (i == 1)
                                {
                                    subStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                else
                                {
                                    supStr += Visiting_munderover(inputNode.ChildNodes[i]);
                                }
                                break;
                            }



                        case MathMLElements.NONE:
                            throw new InvalidOperationException($"The element: {inputNode.ChildNodes[i].Name} is not yet supported.");
                        default:
                            break;
                    }
                }
                result =  mainStr + "_{" + subStr + "}^{" + supStr + "}";
            }
            else
            {
                if (inputNode.HasChildNodes == true && inputNode.ChildNodes.Count != 3)
                {
                    throw new InvalidOperationException("The munderover element must have only three child elements or nodes");
                }
                if (inputNode.HasChildNodes == false)
                {
                    result = @"{ \thinspace }";
                }
            }
            return result;
        }

        private string Visiting_none(XmlNode inputNode)
        {
            if (inputNode.Name=="none"&&inputNode.ChildNodes.Count==0)
            {
                return "{}";
            }
            else
            {
                throw new InvalidOperationException("The none element cannot have child elements or nodes");
            }
            
        }

        #region Attributes Helpers
        // I Need to make this more general
        private bool Attribute_Exists(XmlNode inputNode, string elementName, string attribute)
        {
            int i = 0;
            if (inputNode.Name == elementName && inputNode.Attributes.Count > 0)
            {
                foreach (XmlAttribute item in inputNode.Attributes)
                {
                    if (item.Name == attribute)
                    {
                        i += 1;
                    }
                    else
                    {
                        continue;
                    }
                }
                return i > 0;
            }
            else
            {
                throw new InvalidOperationException($"The element: {inputNode.Name} is not the same as {elementName}.");
            }
        }

        private string Attribute_Value(XmlNode inputNode, string elementName, string attribute)
        {
            if (inputNode.Name == elementName && inputNode.Attributes.Count > 0)
            {
                string attributeValue = "";
                foreach (XmlAttribute item in inputNode.Attributes)
                {
                    if (item.Name == attribute)
                    {
                        attributeValue = item.Value;
                    }
                    else
                    {
                         continue;
                    }
                }
                return attributeValue;
            }
            else
            {
                throw new InvalidOperationException($"The element: {inputNode.Name} is not the same as {elementName}.");
            }
        }


        #endregion


        #endregion

        #region Fields

        //xmlns="http://www.w3.org/1998/Math/MathML"
        /// <summary>
        /// Contains a Dictionary of supported Math Operators and their Tex equivalent.
        /// </summary>
        private Dictionary<string, string> MathOperatorsDict = new Dictionary<string, string>
        {
            {"+",@"\plus " },
            {"-",@"\minus " },
            {"*",@"\times " },
            {"×",@"\times" },
            {"/",@"\divide " },
            {"=",@"\equals " },
            {"%",@"\% " },
            {"&CenterDot;",@"\cdot " },
            {"CenterDot;",@"\cdot " },
            {"CenterDot",@"\cdot "  },
            {"·",@"\cdot" },

            #region Inequalities
            {"&le;",@"\le " },
            {"&lt;=",@"\lt \equals " },
            {"&gt;=",@"\gt \equals " },
            {"&lt;",@"\lt " },
            {"&gt;",@"\gt " },
            {"&lt;&gt;",@"\lt \gt " },
            {">",@"\gt " },
            {"<",@"\lt " },
            {"<=",@"\lt \equals " },
            {"<>",@"\lt \gt" },
            #endregion

            #region Arrows
            {"&OverBar;",@"\bar" },
            {"&UpArrow;",@"\uparrow " },
            {"&LeftArrow;",@"\leftarrow " },
            {"&RightArrow;",@"\rightarrow " },
            {"&DownArrow;",@"\downarrow " },
            {"&NorthWestArrow;",@"\nwarrow " },
            {"&NorthEastArrow;",@"\nearrow " },
            {"&SouthWestArrow;",@"\swarrow " },
            {"&SouthEastArrow;",@"\searrow " },
            {"&LeftRightArrow;",@"\lrarrow " },
            {":=",@"\to" },
	        #endregion     
            
            {"such that",@"\ni " },
            {"there exists",@"\exists " },

            {",",@"\comma " },
            {"int",@"\int" },
            {"&int;",@"\int " },
            {"∫",@"\int " },
            {"∏",@"\prod " },
            {"∑",@"\sum " },
            {"sum",@"\sum" },
            {"lim",@"\lim" },

            #region Delimiter Operators
             {"[",@"\lsqbracket" },
            {"]",@"\rsqbracket" },
            {"(",@"\lbrack" },
            {")",@"\rbrack" },
            {"{",@"\lbrace" },
            {"}",@"\rbrace" },
            {"|",@"\lbar" },
	        #endregion
        };

        /// <summary>
        /// Contains a Dictionary of supported Mathematical symbols and their Tex equivalent.
        /// </summary>
        private Dictionary<string, string> MathSymbolsDict = new Dictionary<string, string>()
        {
            {"λ",@"\lambda" },
            {"μ",@"\mu" },
            { "π",@"\pi "},
            {"∞",@"\infty" },
            {"→",@"\to" },
            {"&delta;",@"\delta " },
            {"lim",@"\lim" },

        };

        #region Elements and their Attributes
        //This dictionary key contains the element names and the values contain the names of its defined attributes.
        //NB: Keep the list of attributes in alphabetical order.
        Dictionary<string, List<string>> elements_Attribute = new Dictionary<string, List<string>>();
        List<string> mfenced_Attributes = new List<string>() {"close", "mathbackgroundcolor", "mathcolor","open", "separators" };
        List<string> mfrac_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        List<string> mi_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        List<string> mn_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        List<string> mo_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        List<string> mover_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        List<string> mroot_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        List<string> mrow_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        List<string> msqrt_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        List<string> msub_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        List<string> msubsup_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        List<string> msup_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        List<string> munder_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        List<string> munderover_Attributes = new List<string>() { "mathbackgroundcolor", "mathcolor", };
        #endregion

        #endregion


        /// <summary>
        /// Specifies the type of MathML element.
        /// <para>
        /// DOESN'T INCLUDE ALL ELEMENTS, WOULD DO SO LATER WHEN THEY ARE WELL SUPPORTED.
        /// </para>
        /// </summary>
        private enum MathMLElements
        {
            /// <summary>
            /// The First node in a MathML file from which subsequent elements can be added.
            /// <para/>
            /// (Top-level element)
            /// </summary>
            math,
            /// <summary>
            /// Enclosed contents.
            /// </summary>
            menclose,
            /// <summary>
            /// Parentheses.
            /// </summary>
            mfenced,
            /// <summary>
            /// Fraction.
            /// </summary>
            mfrac,
            /// <summary>
            /// Identifier.
            /// </summary>
            mi,
            /// <summary>
            /// Prescripts and tensor indices.
            /// </summary>
            mmultiscripts,
            /// <summary>
            /// Number.
            /// </summary>
            mn,
            /// <summary>
            /// Operator.
            /// </summary>
            mo,
            /// <summary>
            /// Overscript.
            /// </summary>
            mover,
            /// <summary>
            /// Invisible content with reserved space.
            /// </summary>
            mphantom,
            /// <summary>
            /// Prescripts in a mmultiscripts.
            /// </summary>
            /// <remarks>
            /// This is not a presentation element but a mmultiscript notifier for the beginning of a sequence of prescript.
            /// </remarks>
            mprescripts,
            /// <summary>
            /// Radical with specified index.
            /// </summary>
            mroot,
            /// <summary>
            /// Grouped sub-expressions.
            /// </summary>
            mrow,
            /// <summary>
            /// Space.
            /// </summary>
            mspace,
            /// <summary>
            /// Square root without an index.
            /// </summary>
            msqrt,
            /// <summary>
            /// Subscript.
            /// </summary>
            msub,
            /// <summary>
            /// Subscript - superscript pair.
            /// </summary>
            msubsup,
            /// <summary>
            /// Superscript.
            /// </summary>
            msup,
            /// <summary>
            /// Table or matrix.
            /// </summary>
            mtable,
            /// <summary>
            /// Cell in a table or a matrix.
            /// </summary>
            mtd,
            /// <summary>
            /// Text.
            /// </summary>
            mtext,
            /// <summary>
            /// Row in a table or a matrix.
            /// </summary>
            mtr,
            /// <summary>
            /// Underscript.
            /// </summary>
            munder,
            /// <summary>
            /// Underscript-overscript pair.
            /// </summary>
            munderover,

            /// <summary>
            /// Tensor index.
            /// </summary>
            /// <remarks>
            /// This is not a presentation element but a scape goat item that allows the insertion of simultaneous scripts and the like.
            /// </remarks>
            none,

            /// <summary>
            /// Unrecognized element.
            /// </summary>
            NONE
        }

        /// <summary>
        /// Gets the type of <see cref="MathMLElements"/>
        /// based on the <paramref name="test"/> string.
        /// </summary>
        /// <param name="test">The name of the element.</param>
        /// <returns></returns>
        private MathMLElements GetElementType(string test)
        {
            switch (test)
            {
                case "math":
                    return MathMLElements.math;
                case "menclose":
                    return MathMLElements.menclose;
                case "mfenced":
                    return MathMLElements.mfenced;
                case "mfrac":
                    return MathMLElements.mfrac;
                case "mi":
                    return MathMLElements.mi;
                case "mmultiscripts":
                    return MathMLElements.mmultiscripts;
                case "mn":
                    return MathMLElements.mn;
                case "mo":
                    return MathMLElements.mo;
                case "mover":
                    return MathMLElements.mover;
                case "mphantom":
                    return MathMLElements.mphantom;
                case "mprescripts":
                    return MathMLElements.mprescripts;
                case "mroot":
                    return MathMLElements.mroot;
                case "mrow":
                    return MathMLElements.mrow;
                case "mspace":
                    return MathMLElements.mspace;
                case "msqrt":
                    return MathMLElements.msqrt;
                case "msub":
                    return MathMLElements.msub;
                case "msubsup":
                    return MathMLElements.msubsup;
                case "msup":
                    return MathMLElements.msup;
                case "mtable":
                    return MathMLElements.mtable;
                case "mtd":
                    return MathMLElements.mtd;
                case "mtext":
                    return MathMLElements.mtext;
                case "mtr":
                    return MathMLElements.mtr;
                case "munder":
                    return MathMLElements.munder;
                case "munderover":
                    return MathMLElements.munderover;
                case "none":
                    return MathMLElements.none;
                default:
                    return MathMLElements.NONE;
            } 
        }






    }
}
