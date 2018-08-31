using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using WpfMath.Exceptions;

namespace WpfMath.Utils
{
    /// <summary>
    /// Represents a class for the processing of colors.
    /// </summary>
    internal static class ColorUtilities
    {
        /// <summary>
        /// Returns a <see cref="Color"/> based on the specified <paramref name="colormodel"/> and <paramref name="colordefinition"/>.
        /// </summary>
        /// <param name="colormodel">The model for generating the <see cref="Color"/>.</param>
        /// <param name="colordefinition">The description of the color, based on the color model.</param>
        /// <returns></returns>
        public static Color Parse(string colormodel, string colordefinition)
        {
            Color resultcolor = new Color();
            switch (colormodel)
            {
                case "ARGB":
                    {
                        List<byte> channelbytes = Get_ARGB(colordefinition);
                        if (channelbytes != null)
                        {
                            resultcolor.A = channelbytes[0];
                            resultcolor.R = channelbytes[1];
                            resultcolor.G = channelbytes[2];
                            resultcolor.B = channelbytes[3];
                        }
                        else
                        {
                            throw new TexParseException($"The color definition \"{colordefinition}\", is invalid for the {colormodel} color model.");
                        }
                        break;
                    }
                case "gray":
                    {
                        List<byte> channelbytes = Get_gray(colordefinition);
                        if (channelbytes != null)
                        {
                            resultcolor.A = 255;
                            resultcolor.R = channelbytes[0];
                            resultcolor.G = channelbytes[0];
                            resultcolor.B = channelbytes[0];
                        }
                        else
                        {
                            throw new TexParseException($"The color definition \"{colordefinition}\", is invalid for the {colormodel} color model.");
                        }
                        break;
                    }
                case "hsb":
                case "hsl":
                case "hsv":
                    {
                        List<byte> channelbytes = Get_hsb(colordefinition);
                        if (channelbytes != null)
                        {
                            resultcolor.A = channelbytes[0];
                            resultcolor.R = channelbytes[1];
                            resultcolor.G = channelbytes[2];
                            resultcolor.B = channelbytes[3];
                        }
                        else
                        {
                            throw new TexParseException($"The color definition \"{colordefinition}\", is invalid for the {colormodel} color model.");
                        }
                        break;
                    }
                case "HTML":
                    {
                        List<byte> channelbytes = Get_HTML(colordefinition);
                        resultcolor.A = 255;
                        if (channelbytes != null)
                        {
                            resultcolor.R = channelbytes[0];
                            resultcolor.G = channelbytes[1];
                            resultcolor.B = channelbytes[2];
                        }
                        else
                        {
                            throw new TexParseException($"The color definition \"{colordefinition}\", is invalid for the {colormodel} color model.");
                        }
                        break;
                    }
                case "rgb":
                    {
                        List<byte> channelbytes = Get_rgb(colordefinition);
                        resultcolor.A = 255;
                        if (channelbytes != null)
                        {
                            resultcolor.R = channelbytes[0];
                            resultcolor.G = channelbytes[1];
                            resultcolor.B = channelbytes[2];
                        }
                        else
                        {
                            throw new TexParseException($"The color definition \"{colordefinition}\", is invalid for the {colormodel} color model.");
                        }
                        break;
                    }
                case "RGB":
                    {
                        List<byte> channelbytes = Get_RGB(colordefinition);
                        resultcolor.A = 255;
                        if (channelbytes != null)
                        {
                            resultcolor.R = channelbytes[0];
                            resultcolor.G = channelbytes[1];
                            resultcolor.B = channelbytes[2];
                        }
                        else
                        {
                            throw new TexParseException($"The color definition \"{colordefinition}\", is invalid for the {colormodel} color model.");
                        }
                        break;
                    }
                default:
                    throw new TexParseException("The color model " + '"' + colormodel + '"' + " is not supported");
            }
            return resultcolor;
        }

        /// <summary>
        /// Checks if the <paramref name="str"/> is an ARGB color model.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="resultbytes"></param>
        /// <returns></returns>
        /// <remarks> Used for strings with byte values for the argb channels.</remarks>
        public static List<byte> Get_ARGB(string str)
        {
            if (IsByteTrain(str, 4, out List<byte> resultbytes))
            {
                return resultbytes;
            }
            else { return null; }
        }

        //\define-color{orange}{cmyk}{0,0.5,1,0}


        public static List<byte> Get_gray(string str)
        {
            if (IsFloatColorString(str, 1, out List<byte> resultbytes))
            {
                return resultbytes;
            }
            else { return null; }
        }

        public static List<byte> Get_hsb(string str)
        {
            if (IsHSBColorString(str, out List<byte> resultbytes))
            {
                return resultbytes;
            }
            else { return null; }
        }

        public static List<byte> Get_HTML(string str)
        {
            if (IsByteHexTrain(str, 6, out List<byte> resultbytes))
            {
                return resultbytes;
            }
            else { return null; }
        }

        public static List<byte> Get_rgb(string str)
        {
            if (IsFloatColorString(str, 3, out List<byte> resultbytes))
            {
                return resultbytes;
            }
            else { return null; }
        }

        // <summary>
        // Checks if the <paramref name="str"/> is an RGB color model.
        // </summary>
        // <param name="str"></param>
        // <returns></returns>
        // <remarks>
        // Used for strings with byte values for the rgb channels.
        // </remarks>
        public static List<byte> Get_RGB(string str)
        {
            if (IsByteTrain(str, 3, out List<byte> resultbytes))
            {
                return resultbytes;
            }
            else { return null; }
        }

        /// <summary>
        /// Returns a value that tells if the <paramref name="input"/> contains byte values, separated by a ",".
        /// </summary>
        /// <example>
        /// </example>
        /// <param name="input">The string expected to contain the byte values.</param>
        /// <param name="num">The expected number of bytes in the <paramref name="input"/>.</param>
        /// <returns></returns>
        public static bool IsByteTrain(string input, int num, out List<byte> resultbytes)
        {
            bool StrCheck = false;
            resultbytes = new List<byte>();
            string[] arrStr = input.Trim().Split(',');
            if (num == arrStr.Length)
            {
                int i = 0;
                foreach (var item in arrStr)
                {
                    if (Byte.TryParse(item, out byte result) == true)
                    {
                        i++;
                        resultbytes.Add(result);
                    }
                    else { continue; }
                }
                StrCheck = i == num ? true : false;
            }
            else
            {
                StrCheck = false;
            }
            return StrCheck;
        }

        /// <summary>
        /// Returns a value that tells if the <paramref name="input"/> contains byte hex values.
        /// </summary>
        /// <param name="input">The <see cref="string"/> containing the hex codes.</param>
        /// <param name="num">The number of bytes (in hexadecimal form) contained in the <paramref name="input"/>.</param>
        /// <returns></returns>
        /// <remarks><paramref name="input"/> should be left in the raw state(e.g.;#56e245, not 56e245).</remarks>
        public static bool IsByteHexTrain(string input, int num, out List<byte> resultByteList)
        {
            bool StrCheck = false;
            resultByteList = new List<byte>();
            List<string> hexTwos = new List<string>();
            string subStr = input.StartsWith("#") ? input.Substring(1) : input;
            if (num == subStr.Length)
            {
                int c = 0;
                for (int i = 1; i < num; i += 2)
                {
                    string item = subStr[i - 1].ToString() + subStr[1].ToString();
                    if (Byte.TryParse(item, System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out byte result) == true)
                    {
                        c += 2;
                    }
                    else { continue; }
                }

                StrCheck = (c == num) ? true : false;
                for (int i = 0; i < input.Length; i++)
                {
                    if (i % 2 == 0 && i > 0)
                    {
                        //add the current and previous strings as one to the "hexTwos" list
                        string str = input[i].ToString() + input[i - 1].ToString();
                        hexTwos.Add(str);
                    }
                }

                if ((num / 2) == hexTwos.Count)
                {
                    foreach (string item in hexTwos)
                    {
                        if (Byte.TryParse(item, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out byte hexbyteres))
                        {
                            resultByteList.Add(hexbyteres);
                        }
                    }
                }
            }
            else
            {
                StrCheck = false;
            }
            return StrCheck;
        }

        private static bool IsHSBColorString(string input, out List<byte> resultbytes)
        {
            resultbytes = new List<byte>();
            double r, g, b = 0;
            string[] arrstr = input.Split(',');
            if (arrstr.Length == 3 && double.TryParse(arrstr[0], out double testasHue) && double.TryParse(arrstr[1], out double testasSaturation) && double.TryParse(arrstr[2], out double testasBrightness))
            {
                if (testasHue >= 0 && testasHue <= 360 && testasSaturation >= 0 && testasSaturation <= 1 && testasBrightness >= 0 && testasBrightness <= 1)
                {

                    if (testasSaturation == 0)
                    {
                        r = testasBrightness;
                        g = testasBrightness;
                        b = testasBrightness;
                    }
                    else
                    {
                        int i;
                        double f, p, q, t;

                        if (testasHue == 360)
                            testasHue = 0;
                        else
                            testasHue = testasHue / 60;

                        i = (int)Math.Truncate(testasHue);
                        f = testasHue - i;

                        p = testasHue * (1.0 - testasSaturation);
                        q = testasBrightness * (1.0 - (testasSaturation * f));
                        t = testasBrightness * (1.0 - (testasSaturation * (1.0 - f)));

                        switch (i)
                        {
                            case 0:
                                {
                                    r = testasBrightness;
                                    g = t;
                                    b = p;
                                    break;
                                }
                            case 1:
                                {
                                    r = q;
                                    g = testasBrightness;
                                    b = p;
                                    break;
                                }
                            case 2:
                                {
                                    r = p;
                                    g = testasBrightness;
                                    b = t;
                                    break;
                                }
                            case 3:
                                {
                                    r = p;
                                    g = q;
                                    b = testasBrightness;
                                    break;
                                }
                            case 4:
                                {
                                    r = t;
                                    g = p;
                                    b = testasBrightness;
                                    break;
                                }
                            default:
                                {
                                    r = testasBrightness;
                                    g = p;
                                    b = q;
                                    break;
                                }
                        }
                    }

                    resultbytes.Add(255);
                    resultbytes.Add((byte)Math.Round(r * 255));
                    resultbytes.Add((byte)Math.Round(g * 255));
                    resultbytes.Add((byte)Math.Round(b * 255));

                    return true;
                }
                return false;
            }
            return false;
        }

        private static bool IsFloatColorString(string input, int num, out List<byte> resultbytes)
        {
            resultbytes = new List<byte>();
            string[] arrstr = input.Trim().Split(',');
            if (arrstr.Length == num)
            {
                foreach (var item in arrstr)
                {
                    if (double.TryParse(item, out double floatvalue))
                    {
                        if (floatvalue >= 0 && floatvalue <= 1)
                        {
                            resultbytes.Add((byte)Math.Floor(floatvalue * 255));
                        }
                    }
                }
            }
            if (resultbytes.Count == num)
            {
                return true;
            }
            else
            {
                resultbytes = null;
                return false;
            }
        }

    }
}
