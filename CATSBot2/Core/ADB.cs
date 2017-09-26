using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CATSBot2.DB;
using System.Threading;

namespace CATSBot2.Core
{
    internal static class ADB
    {
        private static bool firstExecution = true;

        private static string RunCommand(string program, string args)
        {
            if (program.Equals("adb.exe"))
            {
                if (firstExecution)
                {
                    firstExecution = false;
                    RunCommand("adb.exe", "start-server");
                }
            }

            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = program.Equals("adb.exe") ? SettingsManager.settings.adbPath + program : program;
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardInput = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.EnvironmentVariables.Add("VARIABLE1", "1");
            processInfo.Arguments = args;

            Process process = new Process();
            process.StartInfo = processInfo;
            process.Start();

            StreamReader sr = process.StandardError;
            string output = sr.ReadToEnd();
            process.WaitForExit();

            return output;
        }

        public static void RandomSleep(int min = 0, int max = 300)
        {
            Random r = new Random();
            Thread.Sleep(r.Next(min, max));
        }

        public static void ClickBack()
        {
            RandomSleep();
            RunCommand("adb.exe", "shell input keyevent KEYCODE_BACK");
        }

        public static void ClickButtonRectangle(Rectangle r, double tolerance = 0.2)
        {
            CATSimage c = new CATSimage("", r, "");
            ClickButton(c, tolerance);
        }

        public static void ClickButton(CATSimage button, double tolerance = 0.2)
        {
            RandomSleep();
            Random r = new Random();
            int toleranceX = Convert.ToInt32(button.ButtonRectangle.Width * (tolerance >= 1 ? 0.45 : tolerance / 2));
            int toleranceY = Convert.ToInt32(button.ButtonRectangle.Height * (tolerance >= 1 ? 0.45 : tolerance / 2));
            int touchX = r.Next(button.ButtonRectangle.X + toleranceX, button.ButtonRectangle.X + button.ButtonRectangle.Width - toleranceX);
            int touchY = r.Next(button.ButtonRectangle.Y + toleranceY, button.ButtonRectangle.Y + button.ButtonRectangle.Height - toleranceY);
            RunCommand("adb.exe", "shell input touchscreen tap " + touchX + " " + touchY);
        }

        public static void Hibernate()
        {
            Application.SetSuspendState(PowerState.Suspend, true, true);
        }

        public static void RestartApp()
        {
            bool worked = false;
            bool triedOnce = false;
            do
            {
                string output = RunCommand("adb.exe", "shell am force-stop com.zeptolab.cats.google");

                if (!(output == null || output == ""))
                {
                    if (!triedOnce)
                    {
                        RunCommand("adb.exe", "connect 127.0.0.1:21503");
                        triedOnce = true;
                    }
                    else
                        RestartMemu();

                    continue;
                }
                else
                    worked = true;

                RandomSleep(2000, 2500);

                RunCommand("adb.exe", "shell monkey -p com.zeptolab.cats.google 1");

                RandomSleep(2000, 2500);

            } while (!worked);
        }

        public static void RestartMemu()
        {
            Process memu = Process.GetProcessesByName("Memu")[0];
            string fileName = memu.MainModule.FileName;

            memu.Kill();
            memu.WaitForExit();

            Process.Start(fileName);
        }

        public static void ExitMemu()
        {
            foreach (Process proc in Process.GetProcessesByName("Memu"))
            {
                proc.Kill();
            }

            foreach (Process proc in Process.GetProcessesByName("adb"))
            {
                proc.Kill();
            }
        }

        public static Bitmap TakeScreenshot(CATSimage button = null, bool keepSize = true)
        {
            try
            {
                string error = "";
                error += RunCommand("adb.exe", "shell screencap -p /sdcard/screenshot.png");
                RunCommand("adb.exe", "pull /sdcard/screenshot.png");
                error += RunCommand("adb.exe", "shell rm /sdcard/screenshot.png");

                if (error != "")
                    Game.RestartApp();

                Bitmap screenBitmap;
                if (File.Exists("screenshot.png"))
                {
                    Image screenImage = Image.FromFile("screenshot.png");
                    screenBitmap = new Bitmap(screenImage.Width, screenImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(screenBitmap))
                    {
                        g.DrawImage(screenImage, new Rectangle(0, 0, screenImage.Width, screenImage.Height));
                    }
                    screenImage.Dispose();
                }
                else
                    throw new Exception("screenshot.png doesn't exist");

                if (button != null)
                    ImageRecognition.CropImage(button,ref screenBitmap);
                if (!keepSize)
                    ImageRecognition.ResizeImage(ref screenBitmap);
                return screenBitmap;
            }
            catch (Exception e)
            {
                Logger.Log("TakeScreenshot Exception: " + e.Message + e.StackTrace, debug: true);
                return null;
            } 
        }
    }
}
