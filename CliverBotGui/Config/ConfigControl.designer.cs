namespace Cliver.BotGui
{
    partial class ConfigControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.group_box = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // group_box
            // 
            this.group_box.AutoSize = true;
            this.group_box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.group_box.Location = new System.Drawing.Point(0, 0);
            this.group_box.Name = "group_box";
            this.group_box.Size = new System.Drawing.Size(513, 338);
            this.group_box.TabIndex = 143;
            this.group_box.TabStop = false;
            // 
            // ConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.group_box);
            this.Name = "ConfigControl";
            this.Size = new System.Drawing.Size(513, 338);
            this.Load += new System.EventHandler(this.ConfigControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.GroupBox group_box;
        protected System.Windows.Forms.ToolTip toolTip1;
    }
}
