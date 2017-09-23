using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CATSBot2.Core;

namespace CATSBot2.DB
{
    public class CATSimage
    {
        public string Name { get; set; }
        public Rectangle ButtonRectangle { get; set; }
        public Bitmap image;

        private void LoadImage(string name, bool keepSize = true)
        {
            Image resourceToGet = (Image)Properties.Resources.ResourceManager.GetObject(name + "_720");
            if (resourceToGet != null)
            {
                image = new Bitmap(resourceToGet);

                if (!keepSize)
                {
                    ImageRecognition.ResizeImage(ref image);
                    ButtonRectangle = new Rectangle(ButtonRectangle.X / 4, ButtonRectangle.Y / 4, ButtonRectangle.Width / 4, ButtonRectangle.Height / 4);
                }
                else
                    image = image.Clone(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public CATSimage(string name, Rectangle rec, string image)
        {
            Name = name;
            ButtonRectangle = rec;
            LoadImage(image);
        }
    }

    public static class Resources
    {
        public static CATSimage QuickFight;
        public static CATSimage Championship;
        public static CATSimage StageMax;
        public static CATSimage OK;
        public static CATSimage Victory;
        public static CATSimage Lose;
        public static CATSimage RegularBox;
        public static CATSimage SuperBox;
        public static CATSimage SponsorBox;
        public static CATSimage BoxTimer;
        public static CATSimage Arrow;
        public static CATSimage Watch;
        public static CATSimage Unlock;
        public static CATSimage Collect;
        public static CATSimage EnemyHealth;
        public static CATSimage Skip;
        public static CATSimage Fight;
        public static CATSimage Attack;
        public static CATSimage Edit;
        public static CATSimage Mach1;
        public static CATSimage Mach2;
        public static CATSimage Mach3;
        public static CATSimage ChampTime;
        public static CATSimage Coins;
        public static CATSimage Choose;
        public static CATSimage Crown;
        public static CATSimage Box1, Box2, Box3, Box4;
        public static CATSimage OpenNow;
        public static CATSimage Middle;
        public static CATSimage UncleTony;
        public static CATSimage LabelLegendary;
        public static CATSimage LabelSuper;
        public static CATSimage LabelRegular;

        static Resources()
        {
            QuickFight = new CATSimage("Quick Fight Button", new Rectangle(690, 560, 430, 130), "quick_fight");
            Championship = new CATSimage("Championship Button", new Rectangle(300, 550, 325, 160), "championship");
            StageMax = new CATSimage("Stage Max Label", new Rectangle(700, 65, 250, 65), "stage_max");
            OK = new CATSimage("OK Button", new Rectangle(560, 560, 170, 120), "button_ok");
            Victory = new CATSimage("Victory Label", new Rectangle(520, 110, 230, 110), "label_victory");
            Lose = new CATSimage("Defeat Label", new Rectangle(540, 90, 200, 100), "label_defeat");
            RegularBox = new CATSimage("Regular Box", new Rectangle(100, 135, 400, 425), "chest_regular");
            SuperBox = new CATSimage("Super Box", new Rectangle(100, 135, 400, 425), "chest_super");
            SponsorBox = new CATSimage("Sponsor Box", new Rectangle(785, 450, 125, 110), "chest_sponsor");
            BoxTimer = new CATSimage("Gem Sign", new Rectangle(100, 100, 460, 350), "box_timer");
            Arrow = new CATSimage("Open Box Arrow", new Rectangle(120, 65, 380, 400), "arrow");
            Watch = new CATSimage("Skip Button", new Rectangle(630, 550, 370, 130), "button_watch");
            Unlock = new CATSimage("Unlock Button", new Rectangle(490, 560, 310, 120), "unlock");
            Collect = new CATSimage("Collect Prizes", new Rectangle(495, 480, 290, 240), "button_collect");
            EnemyHealth = new CATSimage("Enemy Health Label", new Rectangle(645, 70, 200, 50), "");
            Skip = new CATSimage("Skip Button", new Rectangle(1030, 600, 250, 120), "button_skip");
            Fight = new CATSimage("Fight Button", new Rectangle(1030, 600, 250, 120), "fight");
            Attack = new CATSimage("Attack", new Rectangle(600, 300, 100, 100), "");
            Edit = new CATSimage("Edit Machine", new Rectangle(600, 230, 130, 270), "");
            Mach1 = new CATSimage("1", new Rectangle(1000, 620, 70, 70), "");
            Mach2 = new CATSimage("2", new Rectangle(1100, 620, 70, 70), "");
            Mach3 = new CATSimage("3", new Rectangle(1200, 620, 70, 70), "");
            ChampTime = new CATSimage("Championship Time", new Rectangle(215, 650, 100, 35), "");
            Coins = new CATSimage("Coins", new Rectangle(40, 10, 200, 60), "");
            Choose = new CATSimage("Choose Stickers", new Rectangle(440, 540, 400, 140), "button_choose");
            Crown = new CATSimage("Box Crown", new Rectangle(120, 65, 380, 400), "box_crown");
            Box1 = new CATSimage("Box 1", new Rectangle(135, 70, 175, 240), "arrow");
            Box2 = new CATSimage("Box 2", new Rectangle(310, 70, 175, 240), "arrow");
            Box3 = new CATSimage("Box 3", new Rectangle(135, 305, 175, 240), "arrow");
            Box4 = new CATSimage("Box 4", new Rectangle(310, 305, 175, 240), "arrow");
            OpenNow = new CATSimage("Button Open Now", new Rectangle(300, 560, 700, 120), "open_now");
            Middle = new CATSimage("Middle", new Rectangle(640, 350, 10, 10), "");
            UncleTony = new CATSimage("Uncle Tony", new Rectangle(525, 560, 220, 110), "button_watch");
            LabelLegendary = new CATSimage("Legendary Box", new Rectangle(485, 115, 320, 100), "label_legendary");
            LabelSuper = new CATSimage("Super Box", new Rectangle(530, 115, 230, 100), "label_super");
            LabelRegular = new CATSimage("Regular Box", new Rectangle(505, 115, 250, 85), "label_regular");
        }
    }
}
