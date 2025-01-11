using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestCP_A_r
    {
        public static string RunTest()
        {
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Test case 1: CP A, B (A == B)
            cpu.A = 0x30;
            cpu.B = 0x30;
            cpu.Execute(0xB8); // CP A, B
            if ((cpu.F & 0x80) == 0) return "Failed CP A, B (A == B, Z flag not set)";
            if ((cpu.F & 0x10) != 0) return "Failed CP A, B (A == B, C flag set)";

            // Test case 2: CP A, B (A > B)
            cpu.A = 0x40;
            cpu.B = 0x30;
            cpu.Execute(0xB8); // CP A, B
            if ((cpu.F & 0x80) != 0) return "Failed CP A, B (A > B, Z flag set)";
            if ((cpu.F & 0x10) != 0) return "Failed CP A, B (A > B, C flag set)";

            // Test case 3: CP A, (HL) (A < memory value)
            cpu.A = 0x10;
            cpu.H = 0x12;
            cpu.L = 0x34;
            memory.WriteByte(0x1234, 0x20);
            cpu.Execute(0xBE); // CP A, (HL)
            if ((cpu.F & 0x80) != 0) return "Failed CP A, (HL) (A < memory value, Z flag set)";
            if ((cpu.F & 0x10) == 0) return "Failed CP A, (HL) (A < memory value, C flag not set)";

            return "TestCP_A_r: Passed";
        }
    }

}
