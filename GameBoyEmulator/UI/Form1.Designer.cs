
namespace GameBoyEmulator
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
            this.btnTestMemory = new System.Windows.Forms.Button();
            this.btnTestLD_BC_d16 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTestMemory
            // 
            this.btnTestMemory.Location = new System.Drawing.Point(12, 12);
            this.btnTestMemory.Name = "btnTestMemory";
            this.btnTestMemory.Size = new System.Drawing.Size(102, 34);
            this.btnTestMemory.TabIndex = 0;
            this.btnTestMemory.Text = "Test Memory";
            this.btnTestMemory.UseVisualStyleBackColor = true;
            this.btnTestMemory.Click += new System.EventHandler(this.btnTestMemory_Click);
            // 
            // btnTestLD_BC_d16
            // 
            this.btnTestLD_BC_d16.Location = new System.Drawing.Point(120, 12);
            this.btnTestLD_BC_d16.Name = "btnTestLD_BC_d16";
            this.btnTestLD_BC_d16.Size = new System.Drawing.Size(102, 34);
            this.btnTestLD_BC_d16.TabIndex = 1;
            this.btnTestLD_BC_d16.Text = "Test Opcodes";
            this.btnTestLD_BC_d16.UseVisualStyleBackColor = true;
            this.btnTestLD_BC_d16.Click += new System.EventHandler(this.btnTestLD_BC_d16_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnTestLD_BC_d16);
            this.Controls.Add(this.btnTestMemory);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTestMemory;
        private System.Windows.Forms.Button btnTestLD_BC_d16;
    }
}

