using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestAND_A_r
    {
        public static string RunTest()
        {
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Test case 1: AND A, B
            cpu.A = 0b11001100;
            cpu.B = 0b10101010;
            cpu.Execute(0xA0); // AND A, B
            if (cpu.A != 0b10001000) return "Failed AND A, B";

            // Test case 2: AND A, (HL)
            cpu.A = 0b11110000;
            cpu.H = 0x12;
            cpu.L = 0x34;
            memory.WriteByte(0x1234, 0b11001100);
            cpu.Execute(0xA6); // AND A, (HL)
            if (cpu.A != 0b11000000) return "Failed AND A, (HL)";

            return "TestAND_A_r: Passed";
        }
    }
}
