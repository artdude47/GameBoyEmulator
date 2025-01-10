using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameBoyEmulator.Tests;

namespace GameBoyEmulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTestMemory_Click(object sender, EventArgs e)
        {
            string results = MemoryTests.RunTests();
            MessageBox.Show(results, "Memory Test Result");
        }
    }
}
