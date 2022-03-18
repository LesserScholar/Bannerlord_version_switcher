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
        const string AppmanifestFilename = "appmanifest_261550.acf";
        const string GameDirName = "Mount & Blade II Bannerlord";

        const string magicStringPrefix = "bannerlord_version_switcher(";
        const string magicStringFormat = "bannerlord_version_switcher({0}){1}";

        bool scanOk = false;

        static string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "bannerlord_version_switch_config.txt");

        static BackgroundWorker worker = new BackgroundWorker();
        static System.Windows.Forms.Timer tick = new System.Windows.Forms.Timer();

        private bool IsSteamRunning()
        {
            return Process.GetProcessesByName("steam").Length > 0 || Process.GetProcessesByName("steamservice").Length > 0;
        }

        private string SnapshotGameDirFromVersion(string storage, string version)
        {
            return Path.Combine(storage, "common", String.Format(magicStringFormat, version, GameDirName));
        }

        private string SnapshotAppmanifestFromVersion(string storage, string version)
        {
            return Path.Combine(storage, String.Format(magicStringFormat, version, AppmanifestFilename));
        }

        //=========================
        // UI STUFF
        //=========================

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
        string SteamPath => steamDirTextBox.Text;
        string CustomStoragePath => customStorageTextbox.Text;
        string InstalledVersion => installedVersionLabel.Text;
        string SelectedVersion => snapshotView.SelectedItem as string ?? "";
        string SnapshotStorage => customStorageCheckbox.Checked ? CustomStoragePath : SteamPath;

        void ProgressBarAnimate()
        {
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 30;
        }
        void ProgressBarStop()
        {
            progressBar.Style = ProgressBarStyle.Continuous;
        }
        private void useCustomStorageCheckboxChanged(object sender, EventArgs e)
        {
            ScanSteamAndSnapshots();
        }

        private void configChanged(object sender, EventArgs e)
        {
            ScanSteamAndSnapshots();
            try
            {
                WriteConfig();
            }
            catch
            {
                progressLabel.Text = "Unable to write config file. Changes to paths are not saved.";
            }
        }

        //=========================
        // INIT
        //=========================
        public BvsForm()
        {
            InitializeComponent();
        }
        private void BvsForm_Load(object sender, EventArgs e)
        {

            this.Text = "Bannerlord Version Switcher for Steam";
            if (File.Exists(ConfigPath))
            {
                string[] config = File.ReadAllLines(ConfigPath);
                if (config.Length > 0)
                    steamDirTextBox.Text = config[0].Trim();
                if (config.Length > 1)
                    customStorageTextbox.Text = config[1].Trim();
            }
            else
            {
                var result = MessageBox.Show("This software is provided WITHOUT WARRANTY OF ANY KIND.\n" +
                    "Doing file operations on Windows is kind of hot garbage and I'm sure there are edge cases where it fails spectacularly.\n" +
                    "Consider this software to be \"it works on my machine\" quality.\n\n" +
                    "By clicking OK you accept that this software might nuke your Bannerlord installation.", "Attention", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    try
                    {
                        WriteConfig();
                    }
                    catch
                    {
                        progressLabel.Text = "Unable to write config file. You'll get the warning message again next time...";
                    }
                }
                else
                {
                    Application.Exit();
                }
            }
            tick.Tick += Tick;
            tick.Interval = 10000;
            tick.Start();

            ScanSteamAndSnapshots();
        }

        private void WriteConfig()
        {
            List<string> list = new List<string>();
            list.Add(steamDirTextBox.Text);
            list.Add(customStorageTextbox.Text);
            File.WriteAllLines(ConfigPath, list);
        }

        private void Tick(object? sender, EventArgs e)
        {
            if (!worker.IsBusy)
            {
                ScanSteamAndSnapshots();
            }
        }

        //=========================
        // DIR SCAN
        //=========================
        static bool TryReadVersionFromAppmanifest(string path, out string version)
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
        List<string> Versions()
        {
            List<string> result = new List<string>();
            if (Directory.Exists(SnapshotStorage))
            {
                foreach (var f in Directory.EnumerateFiles(SnapshotStorage))
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

        private void PopulateSnapshots()
        {
            var oldVersions = new HashSet<string>();
            foreach (var v in snapshotView.Items)
            {
                oldVersions.Add((string)v);
            }
            foreach (var v in Versions())
            {
                if (!oldVersions.Contains(v))
                    snapshotView.Items.Add(v);
                else oldVersions.Remove(v);
            }
            var toRemove = new List<object>();
            foreach (var v in snapshotView.Items)
            {
                if (oldVersions.Contains(v))
                    toRemove.Add(v);
            }
            foreach (var v in toRemove)
                snapshotView.Items.Remove(v);
        }

        void ScanSteamAndSnapshots()
        {
            PopulateSnapshots();
            string currentVersion;
            if (TryReadVersionFromAppmanifest(SteamPath, out currentVersion))
            {
                installedVersionLabel.Text = currentVersion;
                scanOk = true;
            }
            else
            {
                installedVersionLabel.Text = "Can't find Bannerlord installation in given folder";
                scanOk = false;
            }
        }

        //=========================
        // CREATE SNAPSHOT
        //=========================
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
        public struct WorkResult
        {
            public WorkResult()
            {
                success = true;
                msg = "";
            }
            public WorkResult(string msg)
            {
                success = false;
                this.msg = msg;
            }
            public bool success;
            public string msg;
        }
        private void createSnapshot(object sender, EventArgs e)
        {
            if (guardInteraction()) return;

            if (Versions().Contains(InstalledVersion))
            {
                progressLabel.Text = "Snapshot with this version already exists! Did nothing.";
                return;
            }
            var appmanifestPath = Path.Combine(SteamPath, AppmanifestFilename);
            var gamePath = Path.Combine(SteamPath, "common", GameDirName);
            var targetGamePath = SnapshotGameDirFromVersion(SnapshotStorage, InstalledVersion);
            if (!File.Exists(appmanifestPath) || !Directory.Exists(gamePath))
            {
                progressLabel.Text = "Did not find gamefiles. Something weird happened.";
                return;
            }
            Directory.CreateDirectory(SnapshotStorage);
            File.Copy(appmanifestPath, SnapshotAppmanifestFromVersion(SnapshotStorage, InstalledVersion));

            ProgressBarAnimate();
            progressLabel.Text = "Copying";

            worker = new BackgroundWorker();
            worker.DoWork += copyDoWork;
            worker.RunWorkerCompleted += createSnapshotCompleted;
            worker.RunWorkerAsync(new CopyWorkItem(gamePath, targetGamePath));
        }
        private void copyDoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                var arg = e.Argument;
                if (arg == null) return;
                if (arg is CopyWorkItem work)
                {
                    Directory.CreateDirectory(work.targetGamePath);
                    foreach (var d in snapshotDirs)
                    {
                        var sourcePath = Path.Combine(work.gamePath, d);
                        if (Directory.Exists(sourcePath))
                        {
                            RecursiveCopy(sourcePath, Path.Combine(work.targetGamePath, d));
                        }
                    }
                }
                e.Result = new WorkResult();
            }
            catch (Exception ex)
            {
                e.Result = new WorkResult("Copy failed: " + ex.Message);
            }
        }
        private static void RecursiveCopy(string source, string target)
        {
            try
            {
                var jobs = new List<(string, string)>();
                if (Directory.Exists(target)) Directory.Delete(target, true);
                Directory.CreateDirectory(target);
                foreach (var d in Directory.GetDirectories(source))
                {
                    RecursiveCopy(d, Path.Combine(target, Path.GetFileName(d)));
                }
                foreach (var f in Directory.GetFiles(source))
                {
                    jobs.Add((f, Path.Combine(target, Path.GetFileName(f))));
                }
                foreach (var job in jobs)
                {
                    File.Copy(job.Item1, job.Item2);
                }
            }
            catch
            {
                throw;
            }
        }

        private void createSnapshotCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is WorkResult r)
            {
                if (r.success)
                {
                    progressLabel.Text = "Copy Done";
                }
                else
                {
                    progressLabel.Text = r.msg;
                    deleteVersionSnapshot(InstalledVersion, true);
                }
                ProgressBarStop();
                ScanSteamAndSnapshots();
            }
        }

        //=========================
        // RESTORE SNAPSHOT
        //=========================
        private void RestoreClicked(object sender, EventArgs e)
        {
            if (guardInteraction()) return;

            if (IsSteamRunning())
            {
                var result = MessageBox.Show("Looks like Steam is running. I recommend you close Steam before overriding game files or else it might get confused.\n\n" +
                    "Make sure it is actually closed and not just minimized.",
                    "Confirm restore", MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel) return;
            }

            var gamePath = Path.Combine(SteamPath, "common", GameDirName);
            var sourceGamePath = Path.Combine(SnapshotStorage, "common", String.Format(magicStringFormat, SelectedVersion, GameDirName));
            
            ProgressBarAnimate();
            progressLabel.Text = "Copying";

            worker = new BackgroundWorker();
            worker.DoWork += copyDoWork;
            worker.RunWorkerCompleted += RestoreCompleted;
            worker.RunWorkerAsync(new CopyWorkItem(sourceGamePath, gamePath));
        }

        private void RestoreCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is WorkResult r)
            {
                if (r.success)
                {
                    try
                    {
                        var appmanifestPath = Path.Combine(SteamPath, AppmanifestFilename);
                        File.Delete(appmanifestPath);
                        File.Copy(SnapshotAppmanifestFromVersion(SnapshotStorage, SelectedVersion), appmanifestPath);
                        progressLabel.Text = "Restore Done";
                    }
                    catch (Exception ex) {
                        progressLabel.Text = "Restore failed: " + ex.Message;
                    }
                }
                else
                {
                    progressLabel.Text = r.msg;
                }
                ProgressBarStop();
                ScanSteamAndSnapshots();
            }
        }

        //=========================
        // DELETE SNAPSHOT
        //=========================
        private void deleteSelectedSnapshot(object sender, EventArgs e)
        {
            if (guardInteraction()) return;
            if (SelectedVersion.Length == 0) return;
            deleteVersionSnapshot(SelectedVersion);
        }

        private void deleteVersionSnapshot(string snapshotVersion, bool silent = false)
        {
            File.Delete(SnapshotAppmanifestFromVersion(SnapshotStorage, snapshotVersion));
            ProgressBarAnimate();
            if (!silent)
                progressLabel.Text = "Deleting";

            worker = new BackgroundWorker();
            worker.DoWork += deleteDoWork;
            if (silent)
                worker.RunWorkerCompleted += deleteCompleteSilent;
            else
                worker.RunWorkerCompleted += deleteComplete;
            worker.RunWorkerAsync(SnapshotGameDirFromVersion(SnapshotStorage, snapshotVersion));
        }

        private void deleteCompleteSilent(object? sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarStop();
            ScanSteamAndSnapshots();
        }

        private void deleteComplete(object? sender, RunWorkerCompletedEventArgs e)
        {
            progressLabel.Text = "Delete Done";
            ProgressBarStop();
            ScanSteamAndSnapshots();
        }

        private void deleteDoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument is string dir)
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
            }
        }

        //=========================
        // SWAP SNAPSHOTS
        //=========================

        private void SwapSnapshot(object sender, EventArgs e)
        {
            guardInteraction();
            if (SelectedVersion.Length == 0) return;
            if (snapshotView.Items.Contains(InstalledVersion))
            {
                progressLabel.Text = "Version current installed in Steam already exists as a snapshot. Not swapping. The idea is that you should only have one copy of each version. Please either update your game or delete the snapshot.";
                return;
            }
            if (SnapshotStorage[0] != SteamPath[0])
            {
                MessageBox.Show("Swapping doesn't work between different storage devices. You need to use create snapshot, restore snapshot and delete to get your desired effect.");
                return;
            }
            if (IsSteamRunning())
            {
                var result = MessageBox.Show("Looks like Steam is running. I recommend you close Steam before swapping or else it might get confused.\n\n" +
                    "Make sure it is actually closed and not just minimized.",
                    "Confirm swap", MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel) return;
            }
            var steamManifest = Path.Combine(SteamPath, AppmanifestFilename);
            var steamGame = Path.Combine(SteamPath, "common", GameDirName);
            try
            {
                var targetDir = SnapshotGameDirFromVersion(SnapshotStorage, InstalledVersion);
                if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);
                Directory.CreateDirectory(Path.Combine(targetDir, "Modules"));
                foreach (var d in snapshotDirs)
                {
                    Directory.Move(Path.Combine(steamGame, d), Path.Combine(targetDir, d));
                }
                File.Move(steamManifest, SnapshotAppmanifestFromVersion(SnapshotStorage, InstalledVersion));
            }
            catch
            {
                MessageBox.Show("Version switcher was unable to move the game files. This is usually caused by another program keeping the files open. Close " +
                    "Visual Studio, Explorer.exe or any other programs that might have the files open.\n\nIf you are having hard time figuring what is holding the files, " +
                    "google \"process explorer which process is keeping file open\".");
                return;
            }
            {
                File.Move(SnapshotAppmanifestFromVersion(SnapshotStorage, SelectedVersion), steamManifest);
                var sourceDir = SnapshotGameDirFromVersion(SnapshotStorage, SelectedVersion);
                foreach (var d in snapshotDirs)
                {
                    Directory.Move(Path.Combine(sourceDir, d), Path.Combine(steamGame, d));
                }
            }
            ScanSteamAndSnapshots();
        }
    }
}
