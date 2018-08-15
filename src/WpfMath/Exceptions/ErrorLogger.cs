using System;
using System.Collections.Generic;
using System.IO;

namespace WpfMath.Exceptions
{
    /// <summary>
    /// Represents a class that logs thrown exceptions.
    /// </summary>
    public class ErrorLogger
    {
        /// <summary>
        /// Contains the list of errors messages from exceptions that have been thrown.
        /// </summary>
        private static List<string> LoggedErrors = new List<string>();

        public ErrorLogger()
        {
            LoggedErrors = new List<string>();
        }

        /// <summary>
        /// Puts the error message into the error log list.
        /// </summary>
        /// <param name="errormsg">The error message.</param>
        public void LogError(string errormsg)
        {
            string errstr = "[" + DateTime.Now.ToShortDateString()+" >> " + DateTime.Now.ToShortTimeString() + " ! WpfMath Error] --> " + errormsg;
            LoggedErrors.Add(errstr);
        }


        /// <summary>
        /// Saves all logged errors to the specified <paramref name="filepath"/>.
        /// </summary>
        /// <param name="filepath">The error file's path.</param>
        public void SaveThrownErrors(string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    for (int i = 0; i < LoggedErrors.Count; i++)
                    {
                        sw.Write(LoggedErrors[i]);
                        sw.WriteLine();
                    }
                    sw.Dispose();
                }
                fs.Dispose();
            }
        }

        public List<string> GetLoggedErrors()
        {
            return LoggedErrors;
        }

        /// <summary>
        /// Returns the number of logged errors.
        /// </summary>
        /// <returns></returns>
        public int GetNumberofErrors()
        {
            return LoggedErrors.Count;
        }

    }
}
