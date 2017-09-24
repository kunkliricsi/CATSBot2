using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using CATSBot2.Core;
using System.Drawing;

namespace CATSBot2.DB
{
    public class Settings
    {
        public string adbPath;
        public string champTimeString;
        public string boxTimeString;

        [XmlIgnore]
        public DateTime champTime;
        [XmlIgnore]
        public DateTime boxTime;

        [XmlIgnore]
        public CATSimage champMach;

        public int latency;
        public int healthThreshold;
        public int champMachInt;
        public int coins;
        public List<int> champTimeInt;

        [XmlIgnore]
        public double hiberTime;

        public bool quickFight;
        public bool skip;
        public bool coinStop;
        public bool champ;
        public bool box;
        public bool boxSkip;
        public bool crownMax;
        public bool crownMaxEnabled;
        public bool stageMax;
        public bool hiber;

        public Settings()
        {
            adbPath = "";
            healthThreshold = 1000;
            coins = 0;
            latency = 1;
            champTime = new DateTime();
            boxTime = new DateTime();
            champTimeString = "";
            boxTimeString = "";
            champMach = Resources.Mach1;
            hiberTime = 0.0d;
            quickFight = false;
            skip = false;
            coinStop = false;
            champ = false;
            box = false;
            boxSkip = false;
            crownMax = false;
            crownMaxEnabled = false;
            stageMax = false;
            hiber = false;
        }

        public int GetLatency()
        {
            return latency + 2;
        }

        public bool Initialize()
        {
            if (champTime.CompareTo(DateTime.Now) <= 0)
            {
                Logger.Log("Trying to get Championship time, this may take a while...");
                int tries = 0;
                do
                {
                    tries++;
                    do
                    {
                        champTimeInt = Game.ReadImageValue(Resources.ChampTime);
                    } while (champTimeInt == null || champTimeInt.Count == 0);

                    string time = "";
                    foreach (int i in champTimeInt)
                        time += i.ToString();

                    if (time.Length != 6)
                        continue;
                
                    string hours = time.Substring(0, 2);
                    string mins = time.Substring(2, 2);

                    double hoursD, minsD;
                    double.TryParse(hours, out hoursD);
                    double.TryParse(mins, out minsD);

                    champTime = DateTime.Now.AddHours(hoursD).AddMinutes(minsD);
                    Logger.Log("Champtionship ends at : " + champTime.ToString());

                    return true;
                } while (tries < 100);

                Logger.Log("Couldn't initialize Championship time");
                return false;
            }

            Logger.Log("Champtionship ends at : " + champTime.ToString());
            return true;
        }
    }
}
