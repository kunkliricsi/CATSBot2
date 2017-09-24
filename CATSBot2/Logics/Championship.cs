using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CATSBot2.Core;
using System.Threading;
using CATSBot2.DB;

namespace CATSBot2.Logics
{
    static class Championship
    {
        private static bool attacked = false;
        
        private static void SetMach(CATSimage mach)
        {
            Game.ClickButton(Resources.Edit);
            Game.RandomSleep(1500, 2500);
            
            Game.ClickButton(mach);
            Game.RandomSleep(1500, 2500);

            Game.ClickBack();
            Game.RandomSleep(1000, 1700);
        }

        private static Messages AttackWithMach(params CATSimage[] buttons)
        {
            foreach (CATSimage button in buttons)
                if (AttackWithMach(button) != Messages.OK)
                {
                    attacked = false;
                    return Messages.Restart;
                }

            return Messages.OK;
        }

        private static Messages AttackWithMach(CATSimage button)
        {
            if (!Game.RepeatFindButton(Resources.Championship, SettingsManager.settings.GetLatency()))
                return Messages.Restart;

            Logger.Log("Setting to loadout: " + button.Name);
            SetMach(button);

            if (!Game.FindAndClickButtonWithSleep(Resources.Championship, 3))
                return Messages.Restart;

            if (!Game.FindAndClickButton(Resources.Fight, 3, 1))
                return Messages.Restart;
            Logger.Log("Started fighting. ");

            Logger.Log("Waiting to end...", newLine: false);
            if (!Game.FindAndClickButtonWithSleep(Resources.OK, 420, 0.5))
                return Messages.Restart;
            
            Logger.Log("Going back to main screen");
            if (!Game.RepeatFindButton(Resources.Championship, SettingsManager.settings.GetLatency() + 2))
            {
                if (Game.RepeatFindButton(Resources.Choose, SettingsManager.settings.GetLatency() - 1))
                {
                    Logger.Log("We got promoted");
                    Game.ClickButton(Resources.Choose, 0.1);
                    Game.RandomSleep(1000, 4000);
                    if (BoxOpener.OpenBox(false) == Messages.Restart)
                        return Messages.Restart;
                    if (!Game.RepeatFindButton(Resources.Championship, SettingsManager.settings.GetLatency()))
                        return Messages.Restart;
                }
                else
                {
                    Logger.Log("Something went wrong");
                    return Messages.Restart;
                }
            }
            
            return Messages.OK;
        }

        public static void SetChamp()
        {
            attacked = false;
        }

        public static Messages Run(bool careForNotInTime = true)
        {
            if (SettingsManager.settings.champTime == DateTime.MinValue)
                return Messages.NotInFuncion;

            DateTime compare = SettingsManager.settings.champTime.AddHours(-1);
            if (!(SettingsManager.settings.champ && (!careForNotInTime || (compare.CompareTo(DateTime.Now) <= 0 && !attacked))))
                return Messages.NotInFuncion;

            Logger.Log("<<Starting Championship session>>");
            attacked = true;

            Messages toReturn = AttackWithMach(Resources.Mach1, Resources.Mach2, Resources.Mach3);

            if (toReturn != Messages.OK)
                Logger.Log("Can't set original loadout back");
            else
            {
                Logger.Log("Settings back to machine: " + SettingsManager.settings.champMach.Name);
                SetMach(SettingsManager.settings.champMach);
            }

            return toReturn;
        }
    }
}
