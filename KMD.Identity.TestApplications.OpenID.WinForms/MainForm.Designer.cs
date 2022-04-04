namespace KMD.Identity.TestApplications.OpenID.WinForms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tsMainMenu = new System.Windows.Forms.ToolStrip();
            this.bIdToken = new System.Windows.Forms.ToolStripButton();
            this.bAccessToken = new System.Windows.Forms.ToolStripButton();
            this.bCallApi = new System.Windows.Forms.ToolStripButton();
            this.tResult = new System.Windows.Forms.TextBox();
            this.tsMainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsMainMenu
            // 
            this.tsMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bIdToken,
            this.bAccessToken,
            this.bCallApi});
            this.tsMainMenu.Location = new System.Drawing.Point(0, 0);
            this.tsMainMenu.Name = "tsMainMenu";
            this.tsMainMenu.Size = new System.Drawing.Size(800, 25);
            this.tsMainMenu.TabIndex = 0;
            this.tsMainMenu.Text = "toolStrip1";
            // 
            // bIdToken
            // 
            this.bIdToken.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bIdToken.Image = ((System.Drawing.Image)(resources.GetObject("bIdToken.Image")));
            this.bIdToken.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bIdToken.Name = "bIdToken";
            this.bIdToken.Size = new System.Drawing.Size(88, 22);
            this.bIdToken.Text = "Show ID Token";
            this.bIdToken.Click += new System.EventHandler(this.bIdToken_Click);
            // 
            // bAccessToken
            // 
            this.bAccessToken.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bAccessToken.Image = ((System.Drawing.Image)(resources.GetObject("bAccessToken.Image")));
            this.bAccessToken.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAccessToken.Name = "bAccessToken";
            this.bAccessToken.Size = new System.Drawing.Size(113, 22);
            this.bAccessToken.Text = "Show Access Token";
            this.bAccessToken.Click += new System.EventHandler(this.bAccessToken_Click);
            // 
            // bCallApi
            // 
            this.bCallApi.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bCallApi.Image = ((System.Drawing.Image)(resources.GetObject("bCallApi.Image")));
            this.bCallApi.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bCallApi.Name = "bCallApi";
            this.bCallApi.Size = new System.Drawing.Size(52, 22);
            this.bCallApi.Text = "Call API";
            this.bCallApi.Click += new System.EventHandler(this.bCallApi_Click);
            // 
            // tResult
            // 
            this.tResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tResult.Location = new System.Drawing.Point(0, 25);
            this.tResult.Multiline = true;
            this.tResult.Name = "tResult";
            this.tResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tResult.Size = new System.Drawing.Size(800, 425);
            this.tResult.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tResult);
            this.Controls.Add(this.tsMainMenu);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Authenticated User Form";
            this.tsMainMenu.ResumeLayout(false);
            this.tsMainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsMainMenu;
        private System.Windows.Forms.ToolStripButton bIdToken;
        private System.Windows.Forms.ToolStripButton bAccessToken;
        private System.Windows.Forms.ToolStripButton bCallApi;
        private System.Windows.Forms.TextBox tResult;
    }
}

