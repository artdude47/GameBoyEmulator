
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
            this.btnTestINC_BC = new System.Windows.Forms.Button();
            this.btnTestDEC_BC = new System.Windows.Forms.Button();
            this.btnTest_LD_A_BC = new System.Windows.Forms.Button();
            this.btnTestLD_BC_A = new System.Windows.Forms.Button();
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
            this.btnTestLD_BC_d16.Text = "Test LD_BC_d16";
            this.btnTestLD_BC_d16.UseVisualStyleBackColor = true;
            this.btnTestLD_BC_d16.Click += new System.EventHandler(this.btnTestLD_BC_d16_Click);
            // 
            // btnTestINC_BC
            // 
            this.btnTestINC_BC.Location = new System.Drawing.Point(228, 12);
            this.btnTestINC_BC.Name = "btnTestINC_BC";
            this.btnTestINC_BC.Size = new System.Drawing.Size(102, 34);
            this.btnTestINC_BC.TabIndex = 2;
            this.btnTestINC_BC.Text = "Test INC_BC";
            this.btnTestINC_BC.UseVisualStyleBackColor = true;
            this.btnTestINC_BC.Click += new System.EventHandler(this.btnTestINC_BC_Click);
            // 
            // btnTestDEC_BC
            // 
            this.btnTestDEC_BC.Location = new System.Drawing.Point(336, 12);
            this.btnTestDEC_BC.Name = "btnTestDEC_BC";
            this.btnTestDEC_BC.Size = new System.Drawing.Size(102, 34);
            this.btnTestDEC_BC.TabIndex = 3;
            this.btnTestDEC_BC.Text = "Test DEC_BC";
            this.btnTestDEC_BC.UseVisualStyleBackColor = true;
            this.btnTestDEC_BC.Click += new System.EventHandler(this.btnTestDEC_BC_Click);
            // 
            // btnTest_LD_A_BC
            // 
            this.btnTest_LD_A_BC.Location = new System.Drawing.Point(552, 12);
            this.btnTest_LD_A_BC.Name = "btnTest_LD_A_BC";
            this.btnTest_LD_A_BC.Size = new System.Drawing.Size(102, 34);
            this.btnTest_LD_A_BC.TabIndex = 4;
            this.btnTest_LD_A_BC.Text = "Test LD_A_BC";
            this.btnTest_LD_A_BC.UseVisualStyleBackColor = true;
            this.btnTest_LD_A_BC.Click += new System.EventHandler(this.btnTest_LD_A_BC_Click);
            // 
            // btnTestLD_BC_A
            // 
            this.btnTestLD_BC_A.Location = new System.Drawing.Point(444, 12);
            this.btnTestLD_BC_A.Name = "btnTestLD_BC_A";
            this.btnTestLD_BC_A.Size = new System.Drawing.Size(102, 34);
            this.btnTestLD_BC_A.TabIndex = 5;
            this.btnTestLD_BC_A.Text = "Test LD_BC_A";
            this.btnTestLD_BC_A.UseVisualStyleBackColor = true;
            this.btnTestLD_BC_A.Click += new System.EventHandler(this.btnTestLD_BC_A_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnTestLD_BC_A);
            this.Controls.Add(this.btnTest_LD_A_BC);
            this.Controls.Add(this.btnTestDEC_BC);
            this.Controls.Add(this.btnTestINC_BC);
            this.Controls.Add(this.btnTestLD_BC_d16);
            this.Controls.Add(this.btnTestMemory);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTestMemory;
        private System.Windows.Forms.Button btnTestLD_BC_d16;
        private System.Windows.Forms.Button btnTestINC_BC;
        private System.Windows.Forms.Button btnTestDEC_BC;
        private System.Windows.Forms.Button btnTest_LD_A_BC;
        private System.Windows.Forms.Button btnTestLD_BC_A;
    }
}

