namespace ALPilot
{
    partial class meteors
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
            this.crokite = new System.Windows.Forms.Button();
            this.kernite = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // crokite
            // 
            this.crokite.Location = new System.Drawing.Point(142, 51);
            this.crokite.Name = "crokite";
            this.crokite.Size = new System.Drawing.Size(91, 38);
            this.crokite.TabIndex = 3;
            this.crokite.Text = "crokite";
            this.crokite.UseVisualStyleBackColor = true;
            this.crokite.Click += new System.EventHandler(this.Crokite_Click);
            // 
            // kernite
            // 
            this.kernite.Location = new System.Drawing.Point(26, 51);
            this.kernite.Name = "kernite";
            this.kernite.Size = new System.Drawing.Size(91, 38);
            this.kernite.TabIndex = 2;
            this.kernite.Text = "kernite";
            this.kernite.UseVisualStyleBackColor = true;
            this.kernite.Click += new System.EventHandler(this.Kernite_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(83, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Meteors to all bots";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(86, 95);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 38);
            this.button1.TabIndex = 5;
            this.button1.Text = "lastconfig";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // meteors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 152);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.crokite);
            this.Controls.Add(this.kernite);
            this.Name = "meteors";
            this.Text = "meteors";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button crokite;
        private System.Windows.Forms.Button kernite;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}