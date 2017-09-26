using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CATSBot2.Core;

namespace CATSBot2.DB
{
    public class Statistics
    {
        public int attacks;
        public int wins;
        public int losses;
        public int skips;
        public int mostSkips;
        public List<int> avarageSkips;
        public int currentWinStreak;
        public int hightestWinStreak;
        public int highestLoseStreak;
        public int boxes;
        public int sponsorBoxes;
        public double watchedVideos;
        public double working;

        public Statistics()
        {
            attacks = 0;
            wins = 0;
            losses = 0;
            skips = 0;
            mostSkips = 0;
            avarageSkips = new List<int>();
            currentWinStreak = 0;
            highestLoseStreak = 0;
            hightestWinStreak = 0;
            boxes = 0;
            sponsorBoxes = 0;
            watchedVideos = 0.0;
            working = 0.0;
        }

        public static Statistics operator +(Statistics a, Statistics b)
        {
            Statistics toReturn = new Statistics();

            toReturn.attacks = a.attacks + b.attacks;
            toReturn.wins = a.wins + b.wins;
            toReturn.losses = a.losses + b.losses;
            toReturn.skips = a.skips + b.skips;
            toReturn.mostSkips = a.mostSkips < b.mostSkips ? b.mostSkips : a.mostSkips;
            toReturn.avarageSkips = (b.avarageSkips.Count > 0 ? a.avarageSkips.Count > 0 ? new List<int>(a.avarageSkips.Concat(b.avarageSkips)) : b.avarageSkips : a.avarageSkips.Count > 0 ? a.avarageSkips : new List<int>());
            toReturn.currentWinStreak = b.currentWinStreak;
            toReturn.hightestWinStreak = a.hightestWinStreak < b.hightestWinStreak ? b.hightestWinStreak : a.hightestWinStreak;
            toReturn.highestLoseStreak = a.highestLoseStreak < b.highestLoseStreak ? b.highestLoseStreak : a.highestLoseStreak;
            toReturn.boxes = a.boxes + b.boxes;
            toReturn.sponsorBoxes = a.sponsorBoxes + b.sponsorBoxes;
            toReturn.watchedVideos = a.watchedVideos + b.watchedVideos;
            toReturn.working = a.working + b.working;

            return toReturn;
        }
    }
}
