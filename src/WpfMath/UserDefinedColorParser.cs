using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using WpfMath.Exceptions;

namespace WpfMath
{
    /// <summary>
    /// Parses definitions of colors from a <see cref="string"/>.
    /// </summary>
    public class UserDefinedColorParser
    {
        /// <summary>
        /// Parses the <paramref name="input"/> to its <see cref="Color"/> equivalent.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Color ParseUserColor(string input)
        {
            Color resultColor = new Color();
            ColorStringTypes CST = GetColorString(input.Trim());
            switch (CST)
            {
                case ColorStringTypes.Byte_longString:
                    {
                        byte[] channelbytes = ByteStringValues(input.Trim(), 4);
                        resultColor.A = channelbytes[0];
                        resultColor.R = channelbytes[1];
                        resultColor.G = channelbytes[2];
                        resultColor.B = channelbytes[3];
                        break;
                    }
                    
                case ColorStringTypes.Byte_shortString:
                    {
                        byte[] channelbytes = ByteStringValues(input.Trim(), 3);
                        resultColor.A = 255;
                        resultColor.R = channelbytes[0];
                        resultColor.G = channelbytes[1];
                        resultColor.B = channelbytes[2];
                        break;
                    }
                case ColorStringTypes.Hex_longString:
                    {
                        byte[] channelbytes = ByteHexStringValues(input.Trim(), 8);
                        resultColor.A = channelbytes[0];
                        resultColor.R = channelbytes[1];
                        resultColor.G = channelbytes[2];
                        resultColor.B = channelbytes[3];
                        break;
                    }
                case ColorStringTypes.Hex_shortString:
                    {
                        byte[] channelbytes = ByteHexStringValues(input.Trim(), 6);
                        resultColor.A = 255;
                        resultColor.R = channelbytes[0];
                        resultColor.G = channelbytes[1];
                        resultColor.B = channelbytes[2];
                        break;
                    }
                case ColorStringTypes.None:
                    throw new TexParseException($"The text: {input} could not be parsed.");
                default:
                    break;
            }
            return resultColor;
        }

        #region Color Parsing Helpers
        /// <summary>
        /// Specifies the type of <see cref="Color"/> the string might represent.
        /// </summary>
        private enum ColorStringTypes
        {
            /// <summary>
            /// Type is a <see cref="string"/> consisting of byte values of the color.
            /// </summary>
            Byte_longString,
            /// <summary>
            /// Type is a <see cref="string"/> consisting of byte values of the color.<para/>
            /// Alpha channel is strictly = 255.
            /// </summary>
            Byte_shortString,
            //FloatString,----->Will include this later.
            /// <summary>
            /// Type is a Hexadecimal <see cref="string"/> consisting of Hexadecimal values of the color.
            /// </summary>
            Hex_longString,
            /// <summary>
            /// Type is a Hexadecimal <see cref="string"/> consisting of Hexadecimal values of the color.<para/>
            /// Alpha channel is strictly = FF.
            /// </summary>
            Hex_shortString,


            /// <summary>
            /// An unrecognized ColorStringType.
            /// </summary>
            None
        }

        /// <summary>
        /// Gets the type of <see cref="ColorStringTypes"/> from the <paramref name="input"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static ColorStringTypes GetColorString(string input)
        {
            if (IsByteHexTrain(input,6)==true)
            {
                return ColorStringTypes.Hex_shortString;
            }
            else if (IsByteHexTrain(input,8))
            {
                return ColorStringTypes.Hex_longString;
            }
            else if (IsByteTrain(input, 3) == true)
            {
                return ColorStringTypes.Byte_shortString;
            }
            else if (IsByteTrain(input, 4) == true)
            {
                return ColorStringTypes.Byte_longString;
            }
            else
            {
                return ColorStringTypes.None;
            }
        }

        /// <summary>
        /// Returns a value that tells if the <paramref name="input"/> contains byte values, separated by a ",".
        /// </summary>
        /// <example>
        /// 23,78,56=>true
        /// 789,fp=>false
        /// </example>
        /// <param name="input"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private static bool IsByteTrain(string input, int num)
        {
            bool StrCheck = false;
            string[] arrStr = input.Trim().Split(',');
            if (num == arrStr.Length)
            {
                int i = 0;
                foreach (var item in arrStr)
                {
                    if (Byte.TryParse(item, out byte result) == true)
                    {
                        i += 1;
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
        /// Returns a value that tells if the <paramref name="input"/> contains byte hex values,
        /// </summary>
        /// <param name="input"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <remarks><paramref name="input"/> should be left in the raw state(e.g.;#56e245, not 56e245).</remarks>
        private static bool IsByteHexTrain(string input, int num)
        {
            bool StrCheck = false;
            string subStr = input.Substring(1);
            if (num == subStr.Length)
            {
                int c = 0;
                for (int i = 1; i < num; i+=2)
                {
                    string item = subStr[i - 1].ToString() + subStr[1].ToString();
                    if (Byte.TryParse(item, System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out byte result) == true)
                    {
                        c+=2;
                    }
                    else { continue; }
                }
 
                StrCheck = (c == num )? true : false;
            }
            else
            {
                StrCheck = false;
            }
            return StrCheck;
        }

        /// <summary>
        /// Gets the <see cref="byte"/> values from the <paramref name="input"/> if the number of items are ==<paramref name="num"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private static byte[] ByteStringValues(string input,int num)
        {
            List<byte> resultByteArr = new List<byte>();
            string[] arrStr = input.Split(',');
            if (num == arrStr.Length)
            {
                foreach (var item in arrStr)
                {
                    if (Byte.TryParse(item, out byte result) == true)
                    {
                        resultByteArr.Add(result);
                    }
                    else { continue; }
                }

            }
            return resultByteArr.ToArray();
        }

        /// <summary>
        /// Gets the <see cref="byte"/> values from the Hex <paramref name="input"/> if the number of items are ==<paramref name="num"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private static byte[] ByteHexStringValues(string input, int num)
        {
            List<string> hexTwos = new List<string>();
            List<byte> resultByteLst = new List<byte>();
            for (int i = 0; i <input.Length; i++)
            {
                if (i%2==0&&i>0)
                {
                    //add the current and previous strings as one to the "hexTwos" list
                    string str = input[i].ToString() + input[i - 1].ToString();
                    hexTwos.Add(str);
                }
            }

            if ((num/2)==hexTwos.Count)
            {
                foreach (string item in hexTwos)
                {
                    if (Byte.TryParse(item, NumberStyles.AllowHexSpecifier,CultureInfo.InvariantCulture, out byte hexbyteres) )
                    {
                        resultByteLst.Add(hexbyteres);
                    }
                }
            }
            return resultByteLst.ToArray();
        }

        #endregion

    }
}
