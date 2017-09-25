using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using CATSBot2.Core;
using CATSBot2.Logics;
using CATSBot2.DB;
using System.ComponentModel;
using System.Drawing;

namespace CATSBot2
{
    using InvokeExtension;
    enum Messages
    {
        ClearScreen,
        OK,
        NotInFuncion,
        Hibernate,
        Restart,
        Found
    };

    public partial class mainForm : MetroFramework.Forms.MetroForm
    {
        private Thread thread;
        private CheckState toogleState;

        public mainForm()
        {
            InitializeComponent();
            this.StyleManager = metroStyleManager;
            Logger.SetForm(this);
            SettingsManager.mainForm = this;
            toogleState = CheckState.Unchecked;
            comboChampMach.Items.Add(Resources.Mach1);
            comboChampMach.Items.Add(Resources.Mach2);
            comboChampMach.Items.Add(Resources.Mach3);
        }

        private string Digitalize(string s, bool dotAllowed = true, bool twoAllowed = true)
        {
            string toReturn;
            if (dotAllowed)
                toReturn = new string((from c in s
                                    where (c >= '0' && c <= '9') || c == '.'
                                    select c
                                    ).ToArray());
            else
                toReturn = new string((from c in s
                                   where c >= '0' && c <= '9'
                                   select c
                                ).ToArray());

            if (twoAllowed && toReturn.Length > (toReturn.Contains(".") ? 3 : 2))
                toReturn = toReturn.Remove(toReturn.Length - 1);

            return toReturn;
        }

        private void BotMainLoop()
        {
            try
            {
                if (SettingsManager.settings.champ && !SettingsManager.settings.Initialize())
                {
                    checkChamp.Checked = false;
                    checkChamp.Enabled = false;
                }

                BoxOpener.SetBoxes();
                
                while (toogleState == CheckState.Checked && toogleStartStop.Checked)
                {
                    if (BoxOpener.Run() == Messages.Restart)
                    {
                        Game.RestartProtocol();
                    }

                    if (Championship.Run() == Messages.Restart)
                    {
                        Game.RestartProtocol();
                        Championship.SetChamp();
                    }

                    if (QuickFight.Run(loops: 5) == Messages.Restart)
                    {
                        Game.RestartProtocol();
                    }

                    if (Hibernate.Run() == Messages.Hibernate)
                    {
                        toogleState = CheckState.Indeterminate;
                        this.WindowState = FormWindowState.Minimized;
                    }
                }

                toogleState = CheckState.Unchecked;
                toogleStartStop.Checked = false;
            }
            catch (Exception e)
            {
                Logger.Log("BotMainLoop Exception: " + e.Message, debug: true);
            }
        }

        private void OpenFolderBrowser()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Please select your MEmu installation folder.";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                // Allow both, the Microvirt folder or the MEmu folder inside
                if (File.Exists(Path.Combine(fbd.SelectedPath, "adb.exe")))
                {
                    SettingsManager.settings.adbPath = fbd.SelectedPath;
                    textMemuPath.Text = fbd.SelectedPath;
                }
                else if (File.Exists(Path.Combine(fbd.SelectedPath, @"MEmu\adb.exe")))
                {
                    SettingsManager.settings.adbPath = Path.Combine(fbd.SelectedPath, @"MEmu\");
                    textMemuPath.Text = Path.Combine(fbd.SelectedPath, @"MEmu\");
                }
                else
                {
                    MetroFramework.MetroMessageBox.Show(this, "Invalid folder selected. Please go into settings and try again.");
                }
            }
            else
            {
                MetroFramework.MetroMessageBox.Show(this, "Invalid folder selected. Please go into settings and try again.");
            }
        }

        public void AppendLog(string text)
        {
            textLog.SynchronizedInvoke(() =>
            {
                textLog.AppendText(text);
            });
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            UpdateStats();

            if (SettingsManager.settings.adbPath.Equals(""))
            {
                if (File.Exists(@"C:\Program Files\Microvirt\MEmu\adb.exe"))
                    SettingsManager.settings.adbPath = @"C:\Program Files\Microvirt\MEmu\";
                else if (File.Exists(@"D:\Program Files\Microvirt\MEmu\adb.exe"))
                    SettingsManager.settings.adbPath = @"D:\Program Files\Microvirt\MEmu\";
                else if (File.Exists(@"E:\Program Files\Microvirt\MEmu\adb.exe"))
                    SettingsManager.settings.adbPath = @"E:\Program Files\Microvirt\MEmu\";
                else
                    OpenFolderBrowser();
            }

            textSkip.Text = SettingsManager.settings.healthThreshold.ToString();
            textMemuPath.Text = SettingsManager.settings.adbPath + "adb.exe";
            checkQuickFight.Checked = SettingsManager.settings.quickFight;
            checkSkip.Checked = SettingsManager.settings.skip;
            checkCoinStop.Checked = SettingsManager.settings.coinStop;
            textCoinStop.Text = SettingsManager.settings.coins.ToString();
            checkBoxSkip.Checked = SettingsManager.settings.boxSkip;
            checkBox.Checked = SettingsManager.settings.box;
            checkCrownMax.Checked = SettingsManager.settings.crownMax;
            checkStageMax.Checked = SettingsManager.settings.stageMax;
            checkHiber.Checked = SettingsManager.settings.hiber;
            checkChamp.Checked = SettingsManager.settings.champ;
            comboChampMach.SelectedIndex = SettingsManager.settings.champMachInt - 1;
            comboLatency.SelectedIndex = SettingsManager.settings.latency - 1;
            
        }

        private void checkQuickFight_CheckedChanged(object sender, EventArgs e)
        {
            SettingsManager.settings.quickFight = checkQuickFight.Checked;
            if (checkQuickFight.Checked)
            {
                checkSkip.Enabled = true;
                if (checkSkip.Checked)
                    textSkip.Enabled = true;
                if (checkSkip.Checked)
                {
                    checkCoinStop.Enabled = true;
                    textCoinStop.Enabled = true;
                }
                if (checkSkip.Checked && checkCoinStop.Checked && checkBoxSkip.Enabled && checkBoxSkip.Checked)
                    checkCrownMax.Enabled = true;
            }
            else
            {
                checkSkip.Enabled = false;
                textSkip.Enabled = false;
                checkCoinStop.Enabled = false;
                textCoinStop.Enabled = false;
                checkCrownMax.Enabled = false;
            }
        }

        private void metroCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            SettingsManager.settings.skip = checkSkip.Checked;
            if (checkSkip.Checked)
            {
                if (checkQuickFight.Checked)
                {
                    textSkip.Enabled = true;
                    checkCoinStop.Enabled = true;
                    if (checkCoinStop.Checked)
                        textCoinStop.Enabled = true;
                    if (checkBoxSkip.Checked && checkBoxSkip.Enabled && checkCoinStop.Checked)
                        checkCrownMax.Enabled = true;
                }
            }
            else
            {
                textSkip.Enabled = false;
                checkCoinStop.Enabled = false;
                textCoinStop.Enabled = false;
                checkCrownMax.Enabled = false;
            }
        }

        private void metroCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            SettingsManager.settings.champ = checkChamp.Checked;
            if (checkChamp.Checked)
            {
                Championship.SetChamp();
                comboChampMach.Enabled = true;
            }
            else
                comboChampMach.Enabled = false;
        }

        private void metroCheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            SettingsManager.settings.box = checkBox.Checked;
            if (checkBox.Checked)
            {
                checkBoxSkip.Enabled = true;
            }
            else
            {
                checkBoxSkip.Enabled = false;
            }
        }

        private void checkHiber_CheckedChanged(object sender, EventArgs e)
        {
            SettingsManager.settings.hiber = checkHiber.Checked;
            if (checkHiber.Checked)
            {
                if (checkStageMax.Checked)
                    textHiber.Enabled = true;
            }
            else
                textHiber.Enabled = false;
        }

        private void textSkip_TextChanged(object sender, EventArgs e)
        {
            textSkip.Text = Digitalize(textSkip.Text, false, false);
            int healthThreshold;
            if (int.TryParse(textSkip.Text, out healthThreshold))
                SettingsManager.settings.healthThreshold = healthThreshold;
        }

        private void checkBoxSkip_CheckedChanged(object sender, EventArgs e)
        {
            SettingsManager.settings.boxSkip = checkBoxSkip.Checked;
            if (checkBoxSkip.Checked && checkSkip.Enabled && checkSkip.Checked && checkCoinStop.Checked)
                checkCrownMax.Enabled = true;
            else
                checkCrownMax.Enabled = false;
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SettingsManager.SaveSettings();
            SettingsManager.SaveStatistics();
            Logger.SaveLog();
            Application.Exit();
        }

        private void buttonMemuPath_Click(object sender, EventArgs e)
        {
            OpenFolderBrowser();
        }

        private void toogleStartStop_CheckedChanged(object sender, EventArgs e)
        {
            if (toogleStartStop.Checked && toogleState == CheckState.Unchecked)
            {
                SettingsManager.allStatistics += SettingsManager.currentStatistics;
                SettingsManager.currentStatistics = new Statistics();
                UpdateStats();
                AppendLog(Environment.NewLine + "STARTING---------------------------------------------------------------------");
                thread = new Thread(BotMainLoop);
                thread.IsBackground = true;
                thread.Start();
                toogleState = CheckState.Checked;
            }
            else if (toogleState == CheckState.Checked)
            {
                toogleState = CheckState.Indeterminate;
                AppendLog(Environment.NewLine + "ABORTING SOON----------------------------------------------------------");
                toogleStartStop.Checked = true;
            }
            else if (!toogleStartStop.Checked && toogleState == CheckState.Indeterminate)
            {
                AppendLog(Environment.NewLine + "ABORTING NOW-----------------------------------------------------------");
                thread.Abort();
                toogleState = CheckState.Unchecked;
            }
        }

        private void textHiber_TextChanged(object sender, EventArgs e)
        {
            textHiber.Text = Digitalize(textHiber.Text);

            double hiberTime = 0.0d;
            if (double.TryParse(textHiber.Text, out hiberTime))
                SettingsManager.settings.hiberTime = hiberTime;
        }

        public void UpdateStats()
        {
            Statistics stats = toogleStat.Checked ? SettingsManager.allStatistics + SettingsManager.currentStatistics : SettingsManager.currentStatistics;
            int wins = SettingsManager.currentStatistics.wins;
            int losses = SettingsManager.currentStatistics.losses;

            labelStat.SynchronizedInvoke(() =>
            {
                labelStat.Text = "Attacks: " + stats.attacks
                                + "\nWins: " + stats.wins
                                + "\nLosses: " + stats.losses
                                + $"{(stats.attacks != 0 ? "\nWinrate: " + ((stats.wins / (double)stats.attacks)).ToString("00.00%") : "")}"
                                + "\nSkips: " + stats.skips
                                + "\nMost Consequent Skips: " + stats.mostSkips
                                + "\nAvarage Skips: " + (stats.avarageSkips.Count > 0 ? stats.avarageSkips.Average().ToString("0.00") : "0")
                                + $"{(!toogleStat.Checked ? "\nCurrent Win Streak: " + stats.currentWinStreak : "")}"
                                + "\nHighest Win Streak: " + stats.hightestWinStreak
                                + "\nHighest Lose Streak: " + stats.highestLoseStreak
                                + "\nOpened Regular + Super boxes: " + stats.boxes
                                + "\nOpened Sponsor boxes: " + stats.sponsorBoxes
                                + "\nWatched " + stats.watchedVideos.ToString("0.00") + " minutes of videos";

                labelWinLoss.Text = "Wins: " + wins + " | Losses: " + losses;
            });
        }

        private void toogleStat_CheckedChanged(object sender, EventArgs e)
        {
            if (toogleStat.Checked)
                toogleStatText.Text = "All";
            else
                toogleStatText.Text = "Current";

            UpdateStats();
        }

        private void check_CheckedChanged(object sender, EventArgs e)
        {
            SettingsManager.settings.stageMax = checkStageMax.Checked;
            if (checkStageMax.Checked)
            {
                checkHiber.Enabled = true;
                if (checkHiber.Checked)
                    textHiber.Enabled = true;
            }
            else
            {
                checkHiber.Enabled = false;
                textHiber.Enabled = false;
            }
        }

        private void comboChampMach_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboChampMach.SelectedIndex == 0)
                SettingsManager.settings.champMach = Resources.Mach1;
            else if (comboChampMach.SelectedIndex == 1)
                SettingsManager.settings.champMach = Resources.Mach2;
            else
                SettingsManager.settings.champMach = Resources.Mach3;
        }

        private void checkCoinStop_CheckedChanged(object sender, EventArgs e)
        {
            SettingsManager.settings.coinStop = checkCoinStop.Checked;
            if (checkCoinStop.Checked)
            {
                if (checkSkip.Checked && checkQuickFight.Checked)
                    textCoinStop.Enabled = true;
                if (checkCoinStop.Enabled && checkBoxSkip.Enabled && checkBoxSkip.Checked)
                    checkCrownMax.Enabled = true;
            }
            else
            {
                textCoinStop.Enabled = false;
                checkCrownMax.Enabled = false;
            }
        }

        private void textCoinStop_TextChanged(object sender, EventArgs e)
        {
            textCoinStop.Text = Digitalize(textCoinStop.Text, false, false);
            int coinStop;
            if (int.TryParse(textCoinStop.Text, out coinStop))
                SettingsManager.settings.coins = coinStop;
        }

        private void metroTabControl_Selected(object sender, TabControlEventArgs e)
        {
            textLog.AppendText("");
        }

        private void linkAbout1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.catsbot.net");
        }

        private void linkAbout2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://catsbot.net/user-csirke");
        }

        private void linkAbout3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://catsbot.net/forum-General-Discussions");
        }

        private void linkAbout4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/kunkliricsi/CATSBot2/blob/master/README.md");
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SettingsManager.settings.latency = comboLatency.SelectedIndex + 1;
        }

        private void checkCrownMax_CheckedChanged(object sender, EventArgs e)
        {
            SettingsManager.settings.crownMax = checkCrownMax.Checked;
        }

        private void checkCrownMax_EnabledChanged(object sender, EventArgs e)
        {
            SettingsManager.settings.crownMaxEnabled = checkCrownMax.Enabled;
        }

        private void checkBoxSkip_EnabledChanged(object sender, EventArgs e)
        {
            if (!checkBoxSkip.Enabled)
                checkCrownMax.Enabled = false;
            else if (checkBoxSkip.Checked)
                checkCrownMax.Enabled = true;
        }
    }
}

namespace InvokeExtension
{
    public static class MainFormExtension
    {
        public static void SynchronizedInvoke(this Control sync, Action action)
        {
            if (!sync.InvokeRequired)
            {
                action();

                return;
            }

            sync.Invoke(action, new object[] { });
        }
    }
}
