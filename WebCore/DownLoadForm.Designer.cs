namespace WebCore
{
    partial class DownLoadForm
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
            this.netLabel = new System.Windows.Forms.Label();
            this.txtDownLoadUrl = new System.Windows.Forms.TextBox();
            this.DirLabel = new System.Windows.Forms.Label();
            this.txtSaveDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FileLenLabel = new System.Windows.Forms.Label();
            this.btnDownLoad = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOpenFloder = new System.Windows.Forms.Button();
            this.DownLoadPBar = new System.Windows.Forms.ProgressBar();
            this.ProLabel = new System.Windows.Forms.Label();
            this.btnFloder = new System.Windows.Forms.Button();
            this.DownLoadWorker = new System.ComponentModel.BackgroundWorker();
            this.nLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // netLabel
            // 
            this.netLabel.AutoSize = true;
            this.netLabel.Location = new System.Drawing.Point(33, 35);
            this.netLabel.Name = "netLabel";
            this.netLabel.Size = new System.Drawing.Size(59, 12);
            this.netLabel.TabIndex = 0;
            this.netLabel.Text = "网络路径:";
            // 
            // txtDownLoadUrl
            // 
            this.txtDownLoadUrl.Enabled = false;
            this.txtDownLoadUrl.Location = new System.Drawing.Point(125, 31);
            this.txtDownLoadUrl.Name = "txtDownLoadUrl";
            this.txtDownLoadUrl.Size = new System.Drawing.Size(300, 21);
            this.txtDownLoadUrl.TabIndex = 1;
            // 
            // DirLabel
            // 
            this.DirLabel.AutoSize = true;
            this.DirLabel.Location = new System.Drawing.Point(33, 73);
            this.DirLabel.Name = "DirLabel";
            this.DirLabel.Size = new System.Drawing.Size(59, 12);
            this.DirLabel.TabIndex = 2;
            this.DirLabel.Text = "保存路径:";
            // 
            // txtSaveDir
            // 
            this.txtSaveDir.BackColor = System.Drawing.Color.White;
            this.txtSaveDir.Location = new System.Drawing.Point(125, 69);
            this.txtSaveDir.Name = "txtSaveDir";
            this.txtSaveDir.ReadOnly = true;
            this.txtSaveDir.Size = new System.Drawing.Size(224, 21);
            this.txtSaveDir.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "文件大小:";
            // 
            // FileLenLabel
            // 
            this.FileLenLabel.AutoSize = true;
            this.FileLenLabel.Location = new System.Drawing.Point(125, 113);
            this.FileLenLabel.Name = "FileLenLabel";
            this.FileLenLabel.Size = new System.Drawing.Size(23, 12);
            this.FileLenLabel.TabIndex = 5;
            this.FileLenLabel.Text = "0 M";
            // 
            // btnDownLoad
            // 
            this.btnDownLoad.Location = new System.Drawing.Point(149, 152);
            this.btnDownLoad.Name = "btnDownLoad";
            this.btnDownLoad.Size = new System.Drawing.Size(75, 23);
            this.btnDownLoad.TabIndex = 6;
            this.btnDownLoad.Text = "下载";
            this.btnDownLoad.UseVisualStyleBackColor = true;
            this.btnDownLoad.Click += new System.EventHandler(this.btnDownLoad_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(350, 152);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOpenFloder
            // 
            this.btnOpenFloder.Enabled = false;
            this.btnOpenFloder.Location = new System.Drawing.Point(233, 152);
            this.btnOpenFloder.Name = "btnOpenFloder";
            this.btnOpenFloder.Size = new System.Drawing.Size(107, 23);
            this.btnOpenFloder.TabIndex = 8;
            this.btnOpenFloder.Text = "打开文件夹";
            this.btnOpenFloder.UseVisualStyleBackColor = true;
            this.btnOpenFloder.Click += new System.EventHandler(this.btnOpenFloder_Click);
            // 
            // DownLoadPBar
            // 
            this.DownLoadPBar.Location = new System.Drawing.Point(35, 196);
            this.DownLoadPBar.Name = "DownLoadPBar";
            this.DownLoadPBar.Size = new System.Drawing.Size(390, 23);
            this.DownLoadPBar.TabIndex = 9;
            // 
            // ProLabel
            // 
            this.ProLabel.AutoSize = true;
            this.ProLabel.Location = new System.Drawing.Point(35, 237);
            this.ProLabel.Name = "ProLabel";
            this.ProLabel.Size = new System.Drawing.Size(77, 12);
            this.ProLabel.TabIndex = 10;
            this.ProLabel.Text = "下载进度: 0%";
            // 
            // btnFloder
            // 
            this.btnFloder.Location = new System.Drawing.Point(355, 68);
            this.btnFloder.Name = "btnFloder";
            this.btnFloder.Size = new System.Drawing.Size(70, 23);
            this.btnFloder.TabIndex = 11;
            this.btnFloder.Text = "浏览";
            this.btnFloder.UseVisualStyleBackColor = true;
            this.btnFloder.Click += new System.EventHandler(this.btnFloder_Click);
            // 
            // DownLoadWorker
            // 
            this.DownLoadWorker.WorkerReportsProgress = true;
            this.DownLoadWorker.WorkerSupportsCancellation = true;
            this.DownLoadWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DownLoadWorker_DoWork);
            this.DownLoadWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.DownLoadWorker_ProgressChanged);
            this.DownLoadWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.DownLoadWorker_RunWorkerCompleted);
            // 
            // nLabel
            // 
            this.nLabel.AutoSize = true;
            this.nLabel.Location = new System.Drawing.Point(373, 237);
            this.nLabel.Name = "nLabel";
            this.nLabel.Size = new System.Drawing.Size(0, 12);
            this.nLabel.TabIndex = 12;
            // 
            // DownLoadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 267);
            this.ControlBox = false;
            this.Controls.Add(this.nLabel);
            this.Controls.Add(this.btnFloder);
            this.Controls.Add(this.ProLabel);
            this.Controls.Add(this.DownLoadPBar);
            this.Controls.Add(this.btnOpenFloder);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDownLoad);
            this.Controls.Add(this.FileLenLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSaveDir);
            this.Controls.Add(this.DirLabel);
            this.Controls.Add(this.txtDownLoadUrl);
            this.Controls.Add(this.netLabel);
            this.DoubleBuffered = true;
            this.Name = "DownLoadForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "下载";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DownLoadForm_FormClosing);
            this.Load += new System.EventHandler(this.DownLoadForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label netLabel;
        private System.Windows.Forms.TextBox txtDownLoadUrl;
        private System.Windows.Forms.Label DirLabel;
        private System.Windows.Forms.TextBox txtSaveDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label FileLenLabel;
        private System.Windows.Forms.Button btnDownLoad;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOpenFloder;
        private System.Windows.Forms.ProgressBar DownLoadPBar;
        private System.Windows.Forms.Label ProLabel;
        private System.Windows.Forms.Button btnFloder;
        private System.ComponentModel.BackgroundWorker DownLoadWorker;
        private System.Windows.Forms.Label nLabel;
    }
}