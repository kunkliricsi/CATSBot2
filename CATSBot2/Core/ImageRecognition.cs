using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using tessnet2;
using CATSBot2.DB;

namespace CATSBot2.Core
{
    internal static class ImageRecognition
    {
        public static void ResizeImage(ref Bitmap image)
        {
            Bitmap copy = new Bitmap(image.Width / 4, image.Height / 4, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(copy))
            {
                g.DrawImage(image, new Rectangle(0, 0, copy.Width, copy.Height));
            }

            image = copy;
        }

        public static void CropImage(CATSimage button,ref Bitmap screenshot)
        {
            screenshot = screenshot.Clone(button.ButtonRectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        }

        public static Rectangle FindButtonRectangle(CATSimage button, int tries = 1)
        {
            for (int i = 0; i < tries; i++)
            {
                Bitmap screen = ADB.TakeScreenshot(button);

                ExhaustiveTemplateMatching templateMatching = new ExhaustiveTemplateMatching(0.941f);
                TemplateMatch[] matches = templateMatching.ProcessImage(screen, button.image);

                if (matches.Count() != 0)
                {
                    Rectangle r = matches.ElementAt(0).Rectangle;
                    r.X += button.ButtonRectangle.X;
                    r.Y += button.ButtonRectangle.Y;
                    return r;
                }
            }

            return Rectangle.Empty;
        }

        public static int FindButton(CATSimage button, int tries = 1)
        {
            for (int i = 0; i < tries; i++)
            {
                Bitmap screen = ADB.TakeScreenshot(button);

                ExhaustiveTemplateMatching templateMatching = new ExhaustiveTemplateMatching(0.941f);
                TemplateMatch[] matches = templateMatching.ProcessImage(screen, button.image);

                if (matches.Count() != 0)
                    return matches.Count();
            }

            return 0;
        }

        private static void CorrectImage(ref Bitmap screenshot)
        {
            for (int i = 0; i < screenshot.Width; i++)
                for (int j = 0; j < screenshot.Height; j++)
                {
                    if (screenshot.GetPixel(i, j).R < 250 || screenshot.GetPixel(i, j).G < 250 || screenshot.GetPixel(i, j).B < 250)
                    {
                        screenshot.SetPixel(i, j, Color.Black);
                    }
                }
        }

        public static List<int> GetButtonNumbers(CATSimage button)
        {
            Bitmap screen = ADB.TakeScreenshot(button, true);
            CorrectImage(ref screen);
            List<int> toReturn = new List<int>();
            int parser = 0;

            try
            {
                Tesseract ocr = new Tesseract();
                ocr.SetVariable("tessedit_char_whitelist", "0123456789");
                ocr.Init(@"E:\Kunkli Richárd\Documents\GitHub\CATSBot2\CATSBot2\CATSBot2\english_tessdata\", "eng", false);

                List<Word> results = ocr.DoOCR(screen, Rectangle.Empty);
                if (results != null && results.Count != 0)
                {
                    foreach (Word w in results)
                    {
                        if (int.TryParse(w.Text, out parser))
                            toReturn.Add(parser);
                    }
                }


            }
            catch (Exception e)
            {
                Logger.Log("GetButtonNumbers Exception: " + e.Message, debug: true);
            }

            return toReturn;
        }
    }
}
