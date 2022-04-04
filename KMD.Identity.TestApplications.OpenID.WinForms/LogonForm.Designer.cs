namespace KMD.Identity.TestApplications.OpenID.WinForms
{
    partial class LogonForm
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
            this.lAuthRequired = new System.Windows.Forms.Label();
            this.bAuthenticateDefault = new System.Windows.Forms.Button();
            this.bAuthenticateWindows = new System.Windows.Forms.Button();
            this.bAuthenticateWithHint = new System.Windows.Forms.Button();
            this.bAuthenticateWebView2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lAuthRequired
            // 
            this.lAuthRequired.AutoSize = true;
            this.lAuthRequired.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lAuthRequired.Location = new System.Drawing.Point(38, 32);
            this.lAuthRequired.Name = "lAuthRequired";
            this.lAuthRequired.Size = new System.Drawing.Size(219, 25);
            this.lAuthRequired.TabIndex = 0;
            this.lAuthRequired.Text = "Authentication Required";
            // 
            // bAuthenticateDefault
            // 
            this.bAuthenticateDefault.Location = new System.Drawing.Point(43, 87);
            this.bAuthenticateDefault.Name = "bAuthenticateDefault";
            this.bAuthenticateDefault.Size = new System.Drawing.Size(214, 41);
            this.bAuthenticateDefault.TabIndex = 1;
            this.bAuthenticateDefault.Text = "Login with KMD Identity (HRD + external browser)";
            this.bAuthenticateDefault.UseVisualStyleBackColor = true;
            this.bAuthenticateDefault.Click += new System.EventHandler(this.bAuthenticateDefault_Click);
            // 
            // bAuthenticateWindows
            // 
            this.bAuthenticateWindows.Location = new System.Drawing.Point(43, 134);
            this.bAuthenticateWindows.Name = "bAuthenticateWindows";
            this.bAuthenticateWindows.Size = new System.Drawing.Size(214, 41);
            this.bAuthenticateWindows.TabIndex = 2;
            this.bAuthenticateWindows.Text = "Login with KMD Identity (Windows Token)";
            this.bAuthenticateWindows.UseVisualStyleBackColor = true;
            this.bAuthenticateWindows.Click += new System.EventHandler(this.bAuthenticateWindows_Click);
            // 
            // bAuthenticateWithHint
            // 
            this.bAuthenticateWithHint.Location = new System.Drawing.Point(43, 181);
            this.bAuthenticateWithHint.Name = "bAuthenticateWithHint";
            this.bAuthenticateWithHint.Size = new System.Drawing.Size(214, 41);
            this.bAuthenticateWithHint.TabIndex = 3;
            this.bAuthenticateWithHint.Text = "Login with KMD Identity (IdP redirect)";
            this.bAuthenticateWithHint.UseVisualStyleBackColor = true;
            this.bAuthenticateWithHint.Click += new System.EventHandler(this.bAuthenticateWithHint_Click);
            // 
            // bAuthenticateWebView2
            // 
            this.bAuthenticateWebView2.Location = new System.Drawing.Point(43, 228);
            this.bAuthenticateWebView2.Name = "bAuthenticateWebView2";
            this.bAuthenticateWebView2.Size = new System.Drawing.Size(214, 41);
            this.bAuthenticateWebView2.TabIndex = 4;
            this.bAuthenticateWebView2.Text = "Login with KMD Identity (WebView2 - experimental)";
            this.bAuthenticateWebView2.UseVisualStyleBackColor = true;
            this.bAuthenticateWebView2.Click += new System.EventHandler(this.bAuthenticateWebView2_Click);
            // 
            // LogonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 283);
            this.Controls.Add(this.bAuthenticateWebView2);
            this.Controls.Add(this.bAuthenticateWithHint);
            this.Controls.Add(this.bAuthenticateWindows);
            this.Controls.Add(this.bAuthenticateDefault);
            this.Controls.Add(this.lAuthRequired);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogonForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KMD Identity Test Application - Logon Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lAuthRequired;
        private System.Windows.Forms.Button bAuthenticateDefault;
        private System.Windows.Forms.Button bAuthenticateWindows;
        private System.Windows.Forms.Button bAuthenticateWithHint;
        private System.Windows.Forms.Button bAuthenticateWebView2;
    }
}