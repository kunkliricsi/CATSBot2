using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CATSBot2.DB;
using CATSBot2.Core;

namespace CATSBot2.Logics
{
    static class Hibernate
    {
        private static DateTime now;
        private static DateTime toHiber;
        private static bool set = false;
        
        public static Messages Run()
        {
            if (!SettingsManager.settings.stageMax)
                return Messages.NotInFuncion;

            if (QuickFight.reachedMax && (!SettingsManager.settings.crownMax || !SettingsManager.settings.crownMaxEnabled))
            {
                ADB.ExitMemu();
                if (SettingsManager.settings.hiber)
                {
                    if (SettingsManager.settings.champ)
                        Championship.Run(false);
                    Logger.Log("Hibernating...");
                    ADB.Hibernate();
                }

                return Messages.Hibernate;
            }

            now = DateTime.Now;
            if (SettingsManager.settings.hiber && SettingsManager.settings.hiberTime > 0.500d)
            {
                if (set)
                {
                    if (toHiber.CompareTo(now) <= 0)
                    {
                        if (SettingsManager.settings.champ)
                            Championship.Run(false);
                        Logger.Log("Hibernating...");
                        ADB.ExitMemu();
                        ADB.Hibernate();
                    }
                }
                else
                {
                    toHiber = now.AddHours(SettingsManager.settings.hiberTime);
                    Logger.Log("Hibernation set, Hibernating in: " + toHiber.ToString());
                    set = true;
                }
            }
            else if (!SettingsManager.settings.hiber && set)
            {
                Logger.Log("Hibernation turned off");
                set = false;
            }

            return Messages.OK;
        }
    }
}
