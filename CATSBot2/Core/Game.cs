using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CATSBot2.DB;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using CATSBot2.Logics;

namespace CATSBot2.Core
{
    public static class Game
    {
        public static void RandomSleep(int min = 0, int max = 300)
        {
            ADB.RandomSleep(min, max);
        }

        public static void ClickBack()
        {
            ADB.ClickBack();
        }

        public static void ClickButton(CATSimage button, double tolerance = 0.2)
        {
            ADB.ClickButton(button, tolerance);
        }

        public static bool FindButton(CATSimage button, int tries = 1)
        {
            return ImageRecognition.FindButton(button, tries) > 0;
        }

        public static int FindAllButtons(CATSimage button, int tries = 1)
        {
            return ImageRecognition.FindButton(button, tries);
        }

        public static bool RepeatFindButton(CATSimage button, int seconds = 2, int sleep = 0)
        {
            if (ImageRecognition.FindButton(button, SettingsManager.settings.GetLatency()) > 0)
                return true;

            for (int i = 0; i < seconds; i++)
            {
                if (ImageRecognition.FindButton(button, 2) > 0)
                    return true;

                ADB.ClickBack();

                if (sleep > 0)
                    Thread.Sleep(sleep);
            }

            return false;
        }

        public static bool FindAndClickButton(CATSimage button, int tries = 1, double tolerance = 0.2)
        {
            if (ImageRecognition.FindButton(button, tries) == 0)
                return false;

            ADB.ClickButton(button, tolerance);

            return true;
        }

        public static bool FindAndClickButtonWithSleep(CATSimage button, int seconds = 2, double tolerance = 0.2)
        {
            for (int i = 0; i < seconds; i++)
            {
                if (ImageRecognition.FindButton(button, 2) > 0)
                {
                    ADB.ClickButton(button, tolerance);
                    return true;
                }

                Thread.Sleep(1000);
            }

            return false;
        }

        public static bool ClickButtonWithFind(CATSimage button, int tries = 1, double tolerance = 0.2)
        {
            Rectangle r = ImageRecognition.FindButtonRectangle(button, tries);
            if (r.IsEmpty)
                return false;
            else
                ADB.ClickButtonRectangle(r, tolerance);

            return true;
        }

        public static List<int> ReadImageValue(CATSimage button)
        {
            return ImageRecognition.GetButtonNumbers(button);
        }

        public static void Hibernate()
        {
            Logger.Log("Hibernating...");
            ADB.Hibernate();
        }

        public static void RestartProtocol()
        {
            if (!RepeatFindButton(Resources.QuickFight, 4))
            {
                if (FindButton(Resources.LabelRegular))
                    BoxOpener.OpenBox(recount: true);
                else if (ClickButtonWithFind(Resources.Choose))
                {
                    BoxOpener.OpenBox(false, true);
                    SettingsManager.settings.champTime.AddHours(48);
                }
                else if (FindButton(Resources.LabelLegendary))
                    BoxOpener.OpenBox(false, true);
                else if (FindButton(Resources.LabelSuper))
                    BoxOpener.OpenBox(false, true);
                else if (FindButton(Resources.LabelGangbox))
                    BoxOpener.OpenBox(false, true);
                else
                    RestartApp();
            }
        }

        public static void RestartApp()
        {
            Logger.Log("Something went wrong, restarting...");
            ADB.RestartApp();

            if (!RepeatFindButton(Resources.QuickFight, 30, 1000))
            {
                ADB.ExitMemu();
                Application.Exit();
            }
        }

        public static void ExitMemu()
        {
            ADB.ExitMemu();
        }

        public static Bitmap TakeScreenshot(CATSimage button = null, bool keepSize = true)
        {
            return ADB.TakeScreenshot(button, keepSize);
        }

    }
}
