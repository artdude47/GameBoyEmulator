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

        private void btnTestLD_BC_d16_Click(object sender, EventArgs e)
        {
            // Use a StringBuilder to collect test results
            StringBuilder results = new StringBuilder();

            // Run each test method and append its result
            results.AppendLine(TestLD_BC_d16.RunTest());
            results.AppendLine(TestINC_BC.RunTest());
            results.AppendLine(TestDEC_BC.RunTest());
            results.AppendLine(TestLD_BC_A.RunTest());
            results.AppendLine(TestLD_A_BC.RunTest());
            results.AppendLine(TestLD_Immediate.RunTest());
            results.AppendLine(TestLD_HL_Instructions.RunTest());
            results.AppendLine(TestADD_A_r.RunTest());
            results.AppendLine(TestSUB_A_r.RunTest());
            results.AppendLine(TestSBC_A_r.RunTest());
            results.AppendLine(TestAND_A_r.RunTest());
            results.AppendLine(TestOR_A_r.RunTest());
            results.AppendLine(TestXOR_A_r.RunTest());
            results.AppendLine(TestCP_A_r.RunTest());
            results.AppendLine(TestJumps.RunTest());
            results.AppendLine(TestCallAndRet.RunTest());
            results.AppendLine(TestPushAndPop.RunTest());
            results.AppendLine(TestIncDec.RunTest());
            results.AppendLine(TestSuite.RunAllTests());

            // Return the full results
            MessageBox.Show(results.ToString());
        }
    }
}
