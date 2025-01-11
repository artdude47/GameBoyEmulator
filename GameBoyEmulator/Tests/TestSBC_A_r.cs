using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestSBC_A_r
    {
        public static string RunTest()
        {
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Test case 1: SBC A, B (Carry = 0)
            cpu.A = 0x30;
            cpu.B = 0x10;
            cpu.F = 0x00; // Clear carry flag
            cpu.Execute(0x98); // SBC A, B
            if (cpu.A != 0x20) return "Failed SBC A, B (Carry = 0)";

            // Test case 2: SBC A, B (Carry = 1)
            cpu.A = 0x30;
            cpu.B = 0x10;
            cpu.F = 0x10; // Set carry flag
            cpu.Execute(0x98); // SBC A, B
            if (cpu.A != 0x1F) return "Failed SBC A, B (Carry = 1)";

            return "TestSBC_A_r: Passed";
        }
    }
}
