namespace Bannerlord_version_switcher
{
    partial class BvsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.steamDirTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.createSnapshotButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.installedVersionLabel = new System.Windows.Forms.Label();
            this.snapshotView = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.loadSnapshot = new System.Windows.Forms.Button();
            this.deleteSnapshot = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.customStorageCheckbox = new System.Windows.Forms.CheckBox();
            this.customStorageTextbox = new System.Windows.Forms.TextBox();
            this.replaceWithSnapshot = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // steamDirTextBox
            // 
            this.steamDirTextBox.Location = new System.Drawing.Point(182, 25);
            this.steamDirTextBox.Name = "steamDirTextBox";
            this.steamDirTextBox.Size = new System.Drawing.Size(649, 23);
            this.steamDirTextBox.TabIndex = 0;
            this.steamDirTextBox.Text = "C:\\Program Files (x86)\\Steam\\steamapps";
            this.steamDirTextBox.TextChanged += new System.EventHandler(this.configChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(60, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Steamapps Directory";
            // 
            // createSnapshotButton
            // 
            this.createSnapshotButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.createSnapshotButton.Location = new System.Drawing.Point(36, 92);
            this.createSnapshotButton.Name = "createSnapshotButton";
            this.createSnapshotButton.Size = new System.Drawing.Size(305, 29);
            this.createSnapshotButton.TabIndex = 2;
            this.createSnapshotButton.Text = "Create new snapshot from installed version";
            this.createSnapshotButton.UseVisualStyleBackColor = true;
            this.createSnapshotButton.Click += new System.EventHandler(this.createSnapshot);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(36, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(161, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "Current installed version:";
            // 
            // installedVersionLabel
            // 
            this.installedVersionLabel.AutoSize = true;
            this.installedVersionLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.installedVersionLabel.Location = new System.Drawing.Point(203, 70);
            this.installedVersionLabel.Name = "installedVersionLabel";
            this.installedVersionLabel.Size = new System.Drawing.Size(136, 19);
            this.installedVersionLabel.TabIndex = 4;
            this.installedVersionLabel.Text = "installedVersionLabel";
            // 
            // snapshotView
            // 
            this.snapshotView.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.snapshotView.FormattingEnabled = true;
            this.snapshotView.ItemHeight = 17;
            this.snapshotView.Location = new System.Drawing.Point(36, 244);
            this.snapshotView.Name = "snapshotView";
            this.snapshotView.Size = new System.Drawing.Size(281, 174);
            this.snapshotView.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 215);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Saved snapshots:";
            // 
            // loadSnapshot
            // 
            this.loadSnapshot.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.loadSnapshot.Location = new System.Drawing.Point(36, 461);
            this.loadSnapshot.Name = "loadSnapshot";
            this.loadSnapshot.Size = new System.Drawing.Size(357, 32);
            this.loadSnapshot.TabIndex = 7;
            this.loadSnapshot.Text = "Swap current installation with the selected snapshot";
            this.loadSnapshot.UseVisualStyleBackColor = true;
            this.loadSnapshot.Click += new System.EventHandler(this.SwapSnapshot);
            // 
            // deleteSnapshot
            // 
            this.deleteSnapshot.Location = new System.Drawing.Point(823, 470);
            this.deleteSnapshot.Name = "deleteSnapshot";
            this.deleteSnapshot.Size = new System.Drawing.Size(272, 23);
            this.deleteSnapshot.TabIndex = 8;
            this.deleteSnapshot.Text = "Permanently delete selected snapshot";
            this.deleteSnapshot.UseVisualStyleBackColor = true;
            this.deleteSnapshot.Click += new System.EventHandler(this.deleteSelectedSnapshot);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(-1, 524);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1137, 23);
            this.progressBar.TabIndex = 9;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(-1, 506);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(39, 15);
            this.progressLabel.TabIndex = 10;
            this.progressLabel.Text = "Ready";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(36, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(918, 19);
            this.label4.TabIndex = 11;
            this.label4.Text = "NOTE: Snapshots take A LOT of disk space. It copies all the game files and assets" +
    ". Make sure you have enough disk space before creating a new copy.";
            // 
            // customStorageCheckbox
            // 
            this.customStorageCheckbox.AutoSize = true;
            this.customStorageCheckbox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.customStorageCheckbox.Location = new System.Drawing.Point(36, 177);
            this.customStorageCheckbox.Name = "customStorageCheckbox";
            this.customStorageCheckbox.Size = new System.Drawing.Size(332, 19);
            this.customStorageCheckbox.TabIndex = 12;
            this.customStorageCheckbox.Text = "Use custom storage for snapshots (may disable swapping)";
            this.customStorageCheckbox.UseVisualStyleBackColor = true;
            this.customStorageCheckbox.CheckedChanged += new System.EventHandler(this.useCustomStorageCheckboxChanged);
            // 
            // customStorageTextbox
            // 
            this.customStorageTextbox.Location = new System.Drawing.Point(374, 173);
            this.customStorageTextbox.Name = "customStorageTextbox";
            this.customStorageTextbox.Size = new System.Drawing.Size(432, 23);
            this.customStorageTextbox.TabIndex = 13;
            this.customStorageTextbox.TextChanged += new System.EventHandler(this.configChanged);
            // 
            // replaceWithSnapshot
            // 
            this.replaceWithSnapshot.Location = new System.Drawing.Point(476, 470);
            this.replaceWithSnapshot.Name = "replaceWithSnapshot";
            this.replaceWithSnapshot.Size = new System.Drawing.Size(284, 23);
            this.replaceWithSnapshot.TabIndex = 14;
            this.replaceWithSnapshot.Text = "Copy current snapshot over installed version";
            this.replaceWithSnapshot.UseVisualStyleBackColor = true;
            this.replaceWithSnapshot.Click += new System.EventHandler(this.RestoreClicked);
            // 
            // BvsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1133, 546);
            this.Controls.Add(this.replaceWithSnapshot);
            this.Controls.Add(this.customStorageTextbox);
            this.Controls.Add(this.customStorageCheckbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.deleteSnapshot);
            this.Controls.Add(this.loadSnapshot);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.snapshotView);
            this.Controls.Add(this.installedVersionLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.createSnapshotButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.steamDirTextBox);
            this.Name = "BvsForm";
            this.Text = "BvsForm";
            this.Load += new System.EventHandler(this.BvsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox steamDirTextBox;
        private Label label1;
        private Button createSnapshotButton;
        private Label label2;
        private Label installedVersionLabel;
        private ListBox snapshotView;
        private Label label3;
        private Button loadSnapshot;
        private Button deleteSnapshot;
        private ProgressBar progressBar;
        private Label progressLabel;
        private Label label4;
        private CheckBox customStorageCheckbox;
        private TextBox customStorageTextbox;
        private Button replaceWithSnapshot;
    }
}