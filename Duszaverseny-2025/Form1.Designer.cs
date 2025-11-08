namespace Duszaverseny_2025
{
    partial class Form1
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
            this.ÚjPakli = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ÚjPakli
            // 
            this.ÚjPakli.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ÚjPakli.Location = new System.Drawing.Point(150, 362);
            this.ÚjPakli.Name = "ÚjPakli";
            this.ÚjPakli.Size = new System.Drawing.Size(120, 40);
            this.ÚjPakli.TabIndex = 0;
            this.ÚjPakli.Text = "Új pakli";
            this.ÚjPakli.UseVisualStyleBackColor = true;
            this.ÚjPakli.Click += new System.EventHandler(this.ÚjPakli_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1445, 814);
            this.Controls.Add(this.ÚjPakli);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Damareen";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ÚjPakli;
    }
}

