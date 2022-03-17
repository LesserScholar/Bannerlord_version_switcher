using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bannerlord_version_switcher
{
    public partial class BvsForm : Form
    {
        static List<string> snapshotDirs = new List<string>
        {
            "bin",
            "Data",
            "GUI",
            "Icons",
            "music",
            "Shaders",
            "Sounds",
            "XmlSchemas",
            @"Modules\Native",
            @"Modules\SandBoxCore",
            @"Modules\SandBox",
            @"Modules\StoryMode",
            @"Modules\CustomBattle",
        };
        static string AppmanifestFilename => "appmanifest_261550.acf";
        static string GameDirName => "Mount & Blade II Bannerlord";

        static string magicStringPrefix = "bannerlord_version_switcher(";
        static string magicStringFormat = "bannerlord_version_switcher({0}){1}";

        bool scanOk = false;

        string ConfigPath;

        BackgroundWorker worker = new BackgroundWorker();

        public BvsForm()
        {
            InitializeComponent();
            ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "bannerlord_version_switch_config.txt");
        }
        private void BvsForm_Load(object sender, EventArgs e)
        {
            this.Text = "Bannerlord Version Switcher for Steam";
            if (File.Exists(ConfigPath))
            {
                string configStr = File.ReadAllText(ConfigPath, Encoding.UTF8).Trim();
                if (configStr.Length > 0)
                {
                    steamDirTextBox.Text = configStr;
                }
            } else
            {
                var result = MessageBox.Show("This software is provided WITHOUT WARRANTY OF ANY KIND.\n" +
                    "Doing file operations on Windows is kind of hot garbage and I'm sure there are edge cases where it fails spectacularly.\n" +
                    "Consider this software to be \"it works on my machine\" quality.\n\n" +
                    "By clicking OK you accept that this software might nuke your Bannerlord installation.", "Attention", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(ConfigPath, steamDirTextBox.Text);
                    } catch { 
                        progressLabel.Text = "Unable to write config file. You'll get the warning message again next time...";
                    }
                } else
                {
                    Application.Exit();
                }
            }

            ScanSteamPath();
        }

        static bool TryGetCurrentVersion(string path, out string version)
        {
            version = "";
            var file = Path.Combine(path, AppmanifestFilename);
            if (!File.Exists(file)) return false;
            foreach (var line in File.ReadLines(file))
            {
                Regex rx = new Regex(@"\s*""betakey""\s*""([\w\.]+)""");
                Match match = rx.Match(line);

                if (match.Success)
                {
                    version = match.Groups[1].Value;
                    return true;
                }
            }
            return false;
        }
        static bool TryExtractVersionFromPath(string path, out string version)
        {
            version = "";
            var f = Path.GetFileName(path);
            if (f.StartsWith(magicStringPrefix))
            {
                var s = f.Substring(magicStringPrefix.Length);
                version = new string(s.TakeWhile(c => c != ')').ToArray());
                if (version == null) return false;
                if (version.Length > 0 && version.Length < 10)
                    return true;
            }
            return false;
        }
        List<string> Versions(string steamPath)
        {
            List<string> result = new List<string>();
            if (Directory.Exists(steamPath))
            {
                foreach (var f in Directory.EnumerateFiles(steamPath))
                {
                    string version;
                    if (TryExtractVersionFromPath(f, out version))
                    {
                        result.Add(version);
                    }
                }
            }
            return result;
        }

        private void PopulateSnapshots(string steamPath)
        {
            snapshotView.Items.Clear();
            foreach (var v in Versions(steamPath))
            {
                snapshotView.Items.Add(v);
            }
        }
        string SteamPath => steamDirTextBox.Text;
        string InstalledVersion => installedVersionLabel.Text;

        string SelectedVersion => snapshotView.SelectedItem as string ?? "";

        void ScanSteamPath()
        {
            PopulateSnapshots(SteamPath);
            string currentVersion;
            if (TryGetCurrentVersion(SteamPath, out currentVersion))
            {
                installedVersionLabel.Text = currentVersion;
                scanOk = true;
            }
            else
            {
                installedVersionLabel.Text = "Can't find Steamapps";
                scanOk = false;
            }
        }

        private void steamVersionTextBox_TextChanged(object sender, EventArgs e)
        {
            try {
                File.WriteAllText(ConfigPath, steamDirTextBox.Text);
            } catch
            {
                progressLabel.Text = "Unable to write config. Steamapps path will not be saved.";
            }
            
            ScanSteamPath();
        }

        private static void RecursiveCopy(string source, string target)
        {
            var jobs = new List<(string, string)>();
            Directory.CreateDirectory(target);
            foreach (var d in Directory.GetDirectories(source))
            {
                RecursiveCopy(d, Path.Combine(target, Path.GetFileName(d)));
            }
            foreach (var f in Directory.GetFiles(source))
            {
                jobs.Add((f, Path.Combine(target, Path.GetFileName(f))));
            }
            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 6 };
            Parallel.ForEach(jobs, (job, i) => File.Copy(job.Item1, job.Item2));
        }

        void ProgressBarAnimate()
        {
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 30;
        }
        void ProgressBarStop()
        {
            progressBar.Style = ProgressBarStyle.Continuous;
        }

        struct CopyWorkItem
        {
            public string gamePath;
            public string targetGamePath;

            public CopyWorkItem(string gamePath, string targetGamePath)
            {
                this.gamePath = gamePath;
                this.targetGamePath = targetGamePath;
            }
        }
        bool guardInteraction()
        {
            if (worker.IsBusy) return true;

            if (!scanOk)
            {
                progressLabel.Text = "Check steam path";
                return true;
            }
            return false;
        }
        private void createSnapshot(object sender, EventArgs e)
        {
            if (guardInteraction()) return;
            
            if (Versions(SteamPath).Contains(InstalledVersion))
            {
                progressLabel.Text = "Snapshot with this version already exists! Did nothing.";
                return;
            }
            var appmanifestPath = Path.Combine(SteamPath, AppmanifestFilename);
            var gamePath = Path.Combine(SteamPath, "common", GameDirName);
            var targetGamePath = Path.Combine(SteamPath, "common", String.Format(magicStringFormat, InstalledVersion, GameDirName));
            if (!File.Exists(appmanifestPath) || !Directory.Exists(gamePath))
            {
                progressLabel.Text = "Did not find gamefiles. Something weird happened.";
                return;
            }
            File.Copy(appmanifestPath, Path.Combine(SteamPath, String.Format(magicStringFormat, InstalledVersion, AppmanifestFilename)));

            ProgressBarAnimate();
            progressLabel.Text = "Copying";

            worker.DoWork += createSnapshotDoWork;
            worker.RunWorkerCompleted += createSnapshotCompleted;
            worker.RunWorkerAsync(new CopyWorkItem(gamePath, targetGamePath));
        }

        private void createSnapshotCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            progressLabel.Text = "Copy Done";
            ProgressBarStop();
            ScanSteamPath();
        }

        private void createSnapshotDoWork(object? sender, DoWorkEventArgs e)
        {
            var arg = e.Argument;
            if (arg == null) return;
            if (arg is CopyWorkItem work)
            {
                if (Directory.Exists(work.targetGamePath)) Directory.Delete(work.targetGamePath, true);
                Directory.CreateDirectory(work.targetGamePath);
                foreach (var d in snapshotDirs)
                {
                    RecursiveCopy(Path.Combine(work.gamePath, d), Path.Combine(work.targetGamePath, d));
                }
            }
        }


        private void deleteSelectedSnapshot(object sender, EventArgs e)
        {
            if (guardInteraction()) return;
            if (SelectedVersion.Length == 0) return;
            File.Delete(AppmanifestFromVersion(SelectedVersion));
            ProgressBarAnimate();
            progressLabel.Text = "Deleting";

            worker.DoWork += deleteDoWork;
            worker.RunWorkerCompleted += deleteComplete;
            worker.RunWorkerAsync(GameDirFromVersion(SelectedVersion));
        }

        private void deleteComplete(object? sender, RunWorkerCompletedEventArgs e)
        {
            progressLabel.Text = "Delete Done";
            ProgressBarStop();
            ScanSteamPath();
        }

        private void deleteDoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument is string dir) {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
            }
        }

        private void SwapSnapshot(object sender, EventArgs e)
        {
            guardInteraction();
            if (SelectedVersion.Length == 0) return;
            if (Versions(SteamPath).Contains(InstalledVersion))
            {
                progressLabel.Text = "Version current installed in Steam already exists as a snapshot. Not swapping. The idea is that you should only have one copy of each version. Please either update your game or delete the snapshot.";
                return;
            }
            if (IsSteamRunning())
            {
                var result = MessageBox.Show("Looks like Steam is running. I recommend you close Steam before swapping or else it might get confused.\n\n"+
                    "Make sure it is actually closed and not just minimized.",
                    "Confirm swap", MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel) return;
            }
            var steamManifest = Path.Combine(SteamPath, AppmanifestFilename);
            var steamGame = Path.Combine(SteamPath, "common", GameDirName);
            try {
                var targetDir = GameDirFromVersion(InstalledVersion);
                if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);
                Directory.CreateDirectory(Path.Combine(targetDir, "Modules"));
                foreach (var d in snapshotDirs)
                {
                    Directory.Move(Path.Combine(steamGame, d), Path.Combine(targetDir, d));
                }
                File.Move(steamManifest, AppmanifestFromVersion(InstalledVersion));
            } catch
            {
                MessageBox.Show("Version switcher was unable to move the game files. This is usually caused by another program keeping the files open. Close " +
                    "Visual Studio, Explorer.exe or any other programs that might have the files open.\n\nIf you are having hard time figuring what is holding the files, " +
                    "google \"process explorer which process is keeping file open\".");
                return;
            }
            {
                File.Move(AppmanifestFromVersion(SelectedVersion), steamManifest);
                var sourceDir = GameDirFromVersion(SelectedVersion);
                foreach (var d in snapshotDirs)
                {
                    Directory.Move(Path.Combine(sourceDir, d), Path.Combine(steamGame, d));
                }
            }
            ScanSteamPath();
        }

        private bool IsSteamRunning()
        {
            return Process.GetProcessesByName("steam").Length > 0 || Process.GetProcessesByName("steamservice").Length > 0;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            ScanSteamPath();
        }
        private string GameDirFromVersion(string str)
        {
            return Path.Combine(SteamPath, "common", String.Format(magicStringFormat, str, GameDirName));
        }

        private string AppmanifestFromVersion(string str)
        {
            return Path.Combine(SteamPath, String.Format(magicStringFormat, str, AppmanifestFilename));
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

    }
}
