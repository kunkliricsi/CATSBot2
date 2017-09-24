using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using CATSBot2.Core;

namespace CATSBot2.DB
{
    public static class SettingsManager
    {
        public static Settings settings;
        public static Statistics currentStatistics;
        public static Statistics allStatistics;

        public static mainForm mainForm;

        static SettingsManager()
        {
            LoadSettings();
            LoadStatistics();
            currentStatistics = new Statistics();
        }

        private static void LoadSettings()
        {
            if (File.Exists("settings.xml"))
            {
                try
                {
                    var xmlserializer = new XmlSerializer(typeof(Settings));
                    using (StreamReader streamReader = new StreamReader("settings.xml"))
                    {
                        using (var reader = XmlReader.Create(streamReader))
                        {
                            settings = (Settings)xmlserializer.Deserialize(reader);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("LoadSettings Exception: " + e.Message, debug: true);
                }
            }
            else
                settings = new Settings();

            settings.boxTime = settings.boxTimeString == "" ? DateTime.MinValue : DateTime.Parse(settings.boxTimeString);
            settings.champTime = settings.champTimeString == "" ? DateTime.MinValue : DateTime.Parse(settings.champTimeString);
            settings.champMach = settings.champMachInt == 1 ? Resources.Mach1 : settings.champMachInt == 2 ? Resources.Mach2 : Resources.Mach3;
        }

        public static void SaveSettings()
        {
            try
            {
                settings.boxTimeString = settings.boxTime.ToString();
                settings.champTimeString = settings.champTime.ToString();
                settings.champMachInt = settings.champMach.Equals(Resources.Mach1) ? 1 : settings.champMach.Equals(Resources.Mach2) ? 2 : 3;
                var xmlserializer = new XmlSerializer(typeof(Settings));
                using (StringWriter stringWriter = new StringWriter())
                {
                    using (var writer = XmlWriter.Create(stringWriter))
                    {
                        xmlserializer.Serialize(writer, settings);
                        File.WriteAllText("settings.xml", stringWriter.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log("SaveSettings Exception: " + e.Message, debug: true);
            }
        }

        private static void LoadStatistics()
        {
            if (File.Exists("statistics.dat"))
            {
                try
                {
                    var xmlserializer = new XmlSerializer(typeof(Statistics));
                    using (StreamReader streamReader = new StreamReader("statistics.dat"))
                    {
                        using (var reader = XmlReader.Create(streamReader))
                        {
                            allStatistics = (Statistics)xmlserializer.Deserialize(reader);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("LoadStatistics Exception: " + e.Message, debug: true);
                }
            }
            else
                allStatistics = new Statistics();
        }

        public static void UpdateStatistics()
        {
            mainForm.UpdateStats();
        }

        public static void SaveStatistics()
        {
            allStatistics += currentStatistics;
            try
            {
                var xmlserializer = new XmlSerializer(typeof(Statistics));
                using (StringWriter stringWriter = new StringWriter())
                {
                    using (var writer = XmlWriter.Create(stringWriter))
                    {
                        xmlserializer.Serialize(writer, allStatistics);
                        File.WriteAllText("statistics.dat", stringWriter.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log("SaveStatistics Exception: " + e.Message, debug: true);
            }
        }
    }
}
