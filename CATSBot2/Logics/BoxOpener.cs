using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CATSBot2.Logics;
using CATSBot2.Core;
using CATSBot2.DB;
using System.Threading;

namespace CATSBot2.Logics
{
    static class BoxOpener
    {
        private static int arrows, boxes = 0;
        private static bool sponsor, timer;

        public static void SetBoxes()
        {
            boxes = 0;
        }

        private static Messages UnlockBox()
        {
            if (!Game.RepeatFindButton(Resources.QuickFight, 2, 200))
                return Messages.Restart;

            if (!Game.ClickButtonWithFind(Resources.RegularBox))
                Game.ClickButtonWithFind(Resources.SuperBox);

            if (Game.FindButton(Resources.Unlock, 3))
            {
                Logger.Log("Unlocking box...");
                Game.ClickButton(Resources.Unlock, 0.8);

                return Messages.Found;
            }

            return Messages.OK;
        }

        private static Messages WatchVideos()
        {
            if (!(SettingsManager.settings.boxSkip && (!SettingsManager.settings.quickFight || (QuickFight.reachedMax && !SettingsManager.settings.stageMax))))
                return Messages.NotInFuncion;

            if (!Game.RepeatFindButton(Resources.QuickFight, 4))
                return Messages.Restart;


            Game.ClickButtonWithFind(Resources.BoxTimer, tolerance: 0.8);

            Game.RandomSleep(500, 1000);
            if (Game.FindAllButtons(Resources.OpenNow) == 2)
            {
                Game.ClickBack();
                return Messages.NotInFuncion;
            }


            bool running = true;
            int openNow = 0;
            do
            {
                Logger.Log("Watching video...");

                if (!Game.FindButton(Resources.Watch, 2))
                {
                    Game.ClickBack();
                    return Messages.NotInFuncion;
                }

                if (openNow == 2)
                    running = false;

                Game.ClickButton(Resources.Watch, 1);
                SettingsManager.currentStatistics.watchedVideos += 0.50;

                if (!running)
                {
                    Game.RandomSleep(36000, 39000);
                    Game.ClickBack();
                    Game.RandomSleep(2400, 3000);
                    Game.ClickBack();
                    if (OpenBox() == Messages.Restart)
                        return Messages.Restart;
                }
                else
                {
                    Game.RandomSleep(36000, 39000);
                    int tries = 0;
                    do
                    {
                        Game.ClickBack();
                        Game.RandomSleep(1000, 4000);
                        openNow = Game.FindAllButtons(Resources.OpenNow);
                        tries++;
                    } while (openNow == 0 && tries < 60);

                    if (tries >= 100)
                        return Messages.Restart;
                }

            } while (running);

            if (!Game.RepeatFindButton(Resources.QuickFight, 4))
                return Messages.Restart;

            return Messages.OK;
        }

        public static Messages OpenBox(bool isSuperOrRegular = true)
        {
            Logger.Log("Opening box...");
            SettingsManager.currentStatistics.boxes++;

            int tries = 0;
            while (!Game.FindButton(Resources.Collect) && tries < 20)
            {
                Game.ClickButton(Resources.Middle);
            }

            if (tries >= 20)
                return Messages.Restart;

            Game.ClickButtonWithFind(Resources.Collect);

            if (isSuperOrRegular)
                boxes--;

            if (Game.ClickButtonWithFind(Resources.UncleTony, 2, 0.8))
            {
                Game.RandomSleep(36000, 39000);
                bool collect;
                tries = 0;
                do
                {
                    Game.ClickBack();
                    Game.RandomSleep(1000, 4000);
                    collect = Game.FindButton(Resources.Collect);
                } while (!collect && tries < 7);

                if (tries >= 7)
                    return Messages.Restart;
            }

            return Messages.OK;
        }

        public static Messages Run()
        {
            if (!SettingsManager.settings.box)
                return Messages.NotInFuncion;

            if (!Game.RepeatFindButton(Resources.QuickFight, 4))
                return Messages.Restart;

            Logger.Log("<<Starting Box Opening session>>");

            #region Sponsor Box
            Logger.Log("Looking for Sponsor Box...");
            sponsor = Game.FindButton(Resources.SponsorBox);

            if (sponsor)
            {
                Logger.Log("Found Sponsor Box, opening...");
                SettingsManager.currentStatistics.sponsorBoxes++;
                Game.ClickButton(Resources.SponsorBox, 0.1);
                if (OpenBox() == Messages.Restart)
                    return Messages.Restart;
            }
            #endregion

            if (boxes <= 0)
            {
                Logger.Log("Looking for available boxes");
                boxes = Game.FindAllButtons(Resources.RegularBox);
                boxes += Game.FindAllButtons(Resources.SuperBox);

                while (boxes <= 0)
                {
                    Logger.Log("No available boxes");
                    if (QuickFight.Run(false, false, 5) == Messages.Restart)
                        return Messages.Restart;

                    if (!Game.FindButton(Resources.QuickFight, 3))
                        return Messages.Restart;

                    Logger.Log("Looking for available boxes");
                    boxes = Game.FindAllButtons(Resources.RegularBox);
                    boxes += Game.FindAllButtons(Resources.SuperBox);
                }
            }

            if (boxes > 0)
            {
                #region Open Unlocked Boxes 
                Logger.Log("Looking for openable boxes...");

                if (!Game.RepeatFindButton(Resources.QuickFight, 4))
                    return Messages.Restart;

                arrows = Game.FindAllButtons(Resources.Arrow);

                if (arrows > 0)
                {
                    Game.ClickButtonWithFind(Resources.Arrow);
                    if (OpenBox() == Messages.Restart)
                        return Messages.Restart;
                }
                #endregion

                #region Start Unlocking
                Logger.Log("Looking for boxes to unlock...");

                if (!Game.RepeatFindButton(Resources.QuickFight, 4))
                    return Messages.Restart;

                timer = Game.FindButton(Resources.BoxTimer);

                if (timer)
                {
                    WatchVideos();
                }
                else
                {
                    if (UnlockBox() == Messages.Restart)
                        return Messages.Restart;

                    WatchVideos();
                }
                #endregion
            }

            SettingsManager.UpdateStatistics();

            return Messages.OK;
        }
    }
}
