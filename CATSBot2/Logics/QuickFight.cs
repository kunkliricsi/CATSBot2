using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CATSBot2.Core;
using CATSBot2.DB;

namespace CATSBot2.Logics
{
    static class QuickFight
    {
        public static bool reachedMax = false;

        public static Messages Run(bool careForStageMax = true, bool careForSkips = true, int loops = 1)
        {
            if (!SettingsManager.settings.quickFight && careForStageMax)
                return Messages.NotInFuncion;
            
            for (int i = 0; i < loops; i++)
            {
                if (!Game.RepeatFindButton(Resources.QuickFight, SettingsManager.settings.GetLatency()))
                    return Messages.Restart;

                if (careForStageMax)
                    if (Game.FindButton(Resources.StageMax, 2))
                    {
                        Logger.Log("Reached Stage Max");
                        reachedMax = true;
                        return Messages.Hibernate;
                    }
                    else
                        reachedMax = false;

                Logger.Log("<<Starting Quick Fight session>>");
                Game.ClickButton(Resources.QuickFight);

                int enemyHealth, tries = 1;
                bool opponentFound = !SettingsManager.settings.skip;

                if (!careForSkips)
                    opponentFound = true;
                
                if (opponentFound && !Game.FindButton(Resources.Skip, 7))
                    return Messages.Restart;

                if (!opponentFound && SettingsManager.settings.coinStop && Game.ReadImageValue(Resources.Coins).ElementAt(0) <= SettingsManager.settings.coins)
                {
                    opponentFound = true;
                    if (!Game.FindButton(Resources.Skip, 7))
                        return Messages.Restart;
                }
            

                while (!opponentFound)
                {
                    if (!Game.FindButton(Resources.Skip, 7))
                        return Messages.Restart;

                    Logger.Log(tries.ToString() + ". Enemy Health: ");
                    enemyHealth = Game.ReadImageValue(Resources.EnemyHealth).ElementAt(0);
                    Logger.Log(enemyHealth.ToString(), false);

                    if (enemyHealth > SettingsManager.settings.healthThreshold)
                    {
                        Logger.Log(" Skipping...", newLine: false);
                        SettingsManager.currentStatistics.skips++;
                        tries++;
                        #region Most Skips Stats
                        if (tries - 1 > SettingsManager.currentStatistics.mostSkips)
                            SettingsManager.currentStatistics.mostSkips = tries - 1;
                        #endregion

                        Game.ClickButton(Resources.Skip);
                    }
                    else
                    {
                        opponentFound = true;
                        SettingsManager.currentStatistics.avarageSkips.Add(tries - 1);
                    }
                }

                Logger.Log("Opponent Found, Attacking...");
                SettingsManager.currentStatistics.attacks++;
                Game.ClickButton(Resources.Attack);

                if (!Game.FindButton(Resources.OK, 600))
                    return Messages.Restart;

                if (Game.FindButton(Resources.Victory))
                #region If Win Stats
                {
                    Logger.Log("We won", newLine: false);
                    SettingsManager.currentStatistics.wins++;

                    if (SettingsManager.currentStatistics.currentWinStreak < 1)
                        SettingsManager.currentStatistics.currentWinStreak = 1;
                    else
                        SettingsManager.currentStatistics.currentWinStreak++;

                    if (SettingsManager.currentStatistics.currentWinStreak > SettingsManager.currentStatistics.hightestWinStreak)
                        SettingsManager.currentStatistics.hightestWinStreak = SettingsManager.currentStatistics.currentWinStreak;
                }
                #endregion
                else
                #region If Lose Stats
                {
                    Logger.Log("We Lost", newLine: false);
                    SettingsManager.currentStatistics.losses++;

                    if (SettingsManager.currentStatistics.currentWinStreak > -1)
                        SettingsManager.currentStatistics.currentWinStreak = -1;
                    else
                        SettingsManager.currentStatistics.currentWinStreak--;

                    if (SettingsManager.currentStatistics.currentWinStreak < -1 * SettingsManager.currentStatistics.highestLoseStreak)
                        SettingsManager.currentStatistics.highestLoseStreak = SettingsManager.currentStatistics.currentWinStreak * -1;
                }
                #endregion

                SettingsManager.UpdateStatistics();
                Game.ClickButton(Resources.OK);
            }

            return Messages.OK;
        }
    }
}
