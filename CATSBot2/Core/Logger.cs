using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace CATSBot2.Core
{
    internal static class Logger
    {
        private static string logText, previousText;
        private static int counter = 2;
        private static mainForm mainForm;

        public static void SetForm(mainForm formToSet)
        {
            mainForm = formToSet;
        }

        public static void Log(string text, bool newLine = true, bool debug = false)
        {
            if (debug)
            {
#if Debug
                mainform.AppendText(text);
#endif
            }
            else
            {
                Directory.CreateDirectory("logs");
                string formattedText;
                
                if (text.Equals(previousText))
                {
                    formattedText = counter.ToString() + " ";
                    counter++;
                }
                else
                {
                    formattedText = (newLine ? Environment.NewLine + "[" + DateTime.Now.ToString("dd.MM.yy H:mm:ss") + "] " : "") + text;
                    counter = 2;
                }
                logText += formattedText;
                mainForm.AppendLog(formattedText);
                previousText = text;
            }
        }

        public static void SaveLog()
        {
            string dateTimeString = DateTime.Now.ToString("yyyy_MM_dd", CultureInfo.InvariantCulture);
            if (Directory.Exists("logs") && File.Exists(@"logs\CatsBot_" + dateTimeString + ".log"))
                File.AppendAllText(@"logs\CatsBot_" + dateTimeString + ".log", logText);
        }
    }
}
