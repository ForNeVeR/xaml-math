using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using WpfMath.Exceptions;

namespace WpfMath.Parsers
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
        public static Color Parse(string input)
        {
            Color resultColor = new Color();
            ColorStringTypes CST = GetColorString(input.Trim());
            switch (CST)
            {
                case ColorStringTypes.Byte_longString:
                    {
                        bool wstchk=IsByteTrain(input.Trim(), 4, out List<byte> result);
                        byte[] channelbytes = result.ToArray();
                        resultColor.A = channelbytes[0];
                        resultColor.R = channelbytes[1];
                        resultColor.G = channelbytes[2];
                        resultColor.B = channelbytes[3];
                        break;
                    }
                    
                case ColorStringTypes.Byte_shortString:
                    {
                        bool wstchk=IsByteTrain(input.Trim(), 3, out List<byte> result);
                        byte[] channelbytes = result.ToArray();
                        resultColor.A=255;
                        resultColor.R = channelbytes[0];
                        resultColor.G = channelbytes[1];
                        resultColor.B = channelbytes[2];
                        break;
                    }
                case ColorStringTypes.Hex_longString:
                    {
                        bool wstchk=IsByteTrain(input.Trim(), 8, out List<byte> result);
                        byte[] channelbytes = result.ToArray();
                        resultColor.A = channelbytes[0];
                        resultColor.R = channelbytes[1];
                        resultColor.G = channelbytes[2];
                        resultColor.B = channelbytes[3];
                        break;
                    }
                case ColorStringTypes.Hex_shortString:
                    {
                        bool wstchk=IsByteTrain(input.Trim(), 6, out List<byte> result);
                        byte[] channelbytes = result.ToArray();
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
            if (IsByteHexTrain(input,6, out List<byte> hssres)==true)
            {
                return ColorStringTypes.Hex_shortString;
            }
            else if (IsByteHexTrain(input,8 , out List<byte> hlsres ))
            {
                return ColorStringTypes.Hex_longString;
            }
            else if (IsByteTrain(input, 3, out List<byte> bssres ) == true)
            {
                return ColorStringTypes.Byte_shortString;
            }
            else if (IsByteTrain(input, 4, out List<byte> blsres ) == true)
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
        private static bool IsByteTrain(string input, int num, out List<byte> resultbytes )
        {
            resultbytes = new List<byte>();
            bool valid = Utils.ColorUtilities.IsByteTrain(input, num, out List<byte> result);
            resultbytes = result;
            return valid;
        }
        
        /// <summary>
        /// Returns a value that tells if the <paramref name="input"/> contains byte hex values,
        /// </summary>
        /// <param name="input"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <remarks><paramref name="input"/> should be left in the raw state(e.g.;#56e245, not 56e245).</remarks>
        private static bool IsByteHexTrain(string input, int num, out List<byte> resultByteList)
        {
           resultByteList = new List<byte>();
            bool valid = Utils.ColorUtilities.IsByteHexTrain(input, num, out List<byte> result);
            resultByteList = result;
            return valid;
        }

      

        #endregion

    }
}
