namespace Marathon.Toolkit.Forms
{
    partial class Options
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
            this.PictureBox_Logo = new System.Windows.Forms.PictureBox();
            this.Label_Title = new System.Windows.Forms.Label();
            this.FlowLayoutPanel_Options = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.KryptonRibbon_MarathonDockContent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Logo)).BeginInit();
            this.SuspendLayout();
            // 
            // KryptonRibbon_MarathonDockContent
            // 
            this.KryptonRibbon_MarathonDockContent.RibbonAppButton.AppButtonShowRecentDocs = false;
            this.KryptonRibbon_MarathonDockContent.RibbonAppButton.AppButtonVisible = false;
            // 
            // PictureBox_Logo
            // 
            this.PictureBox_Logo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PictureBox_Logo.BackgroundImage = global::Marathon.Toolkit.Properties.Resources.Toolkit_Small_Colour;
            this.PictureBox_Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.PictureBox_Logo.Location = new System.Drawing.Point(1209, 0);
            this.PictureBox_Logo.Name = "PictureBox_Logo";
            this.PictureBox_Logo.Size = new System.Drawing.Size(51, 50);
            this.PictureBox_Logo.TabIndex = 7;
            this.PictureBox_Logo.TabStop = false;
            // 
            // Label_Title
            // 
            this.Label_Title.AutoSize = true;
            this.Label_Title.Font = new System.Drawing.Font("Segoe UI Semilight", 14.8F);
            this.Label_Title.Location = new System.Drawing.Point(9, 9);
            this.Label_Title.Name = "Label_Title";
            this.Label_Title.Size = new System.Drawing.Size(78, 28);
            this.Label_Title.TabIndex = 6;
            this.Label_Title.Text = "Options";
            // 
            // FlowLayoutPanel_Options
            // 
            this.FlowLayoutPanel_Options.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FlowLayoutPanel_Options.AutoScroll = true;
            this.FlowLayoutPanel_Options.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.FlowLayoutPanel_Options.Location = new System.Drawing.Point(0, 49);
            this.FlowLayoutPanel_Options.Name = "FlowLayoutPanel_Options";
            this.FlowLayoutPanel_Options.Padding = new System.Windows.Forms.Padding(25, 5, 0, 0);
            this.FlowLayoutPanel_Options.Size = new System.Drawing.Size(1264, 632);
            this.FlowLayoutPanel_Options.TabIndex = 9;
            this.FlowLayoutPanel_Options.WrapContents = false;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.FlowLayoutPanel_Options);
            this.Controls.Add(this.PictureBox_Logo);
            this.Controls.Add(this.Label_Title);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.SystemColors.Control;
            this.Name = "Options";
            this.Text = "Options";
            this.UseRibbon = false;
            this.Controls.SetChildIndex(this.Label_Title, 0);
            this.Controls.SetChildIndex(this.PictureBox_Logo, 0);
            this.Controls.SetChildIndex(this.FlowLayoutPanel_Options, 0);
            this.Controls.SetChildIndex(this.KryptonRibbon_MarathonDockContent, 0);
            ((System.ComponentModel.ISupportInitialize)(this.KryptonRibbon_MarathonDockContent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Logo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PictureBox_Logo;
        private System.Windows.Forms.Label Label_Title;
        private System.Windows.Forms.FlowLayoutPanel FlowLayoutPanel_Options;
    }
}
