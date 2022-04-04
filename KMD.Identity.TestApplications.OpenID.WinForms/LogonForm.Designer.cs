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
            this.bAuthenticate = new System.Windows.Forms.Button();
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
            // bAuthenticate
            // 
            this.bAuthenticate.Location = new System.Drawing.Point(43, 87);
            this.bAuthenticate.Name = "bAuthenticate";
            this.bAuthenticate.Size = new System.Drawing.Size(214, 41);
            this.bAuthenticate.TabIndex = 1;
            this.bAuthenticate.Text = "Login with KMD Identity";
            this.bAuthenticate.UseVisualStyleBackColor = true;
            this.bAuthenticate.Click += new System.EventHandler(this.bAuthenticate_Click);
            // 
            // LogonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 164);
            this.Controls.Add(this.bAuthenticate);
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
        private System.Windows.Forms.Button bAuthenticate;
    }
}