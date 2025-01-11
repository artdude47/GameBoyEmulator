using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestOR_A_r
    {
        public static string RunTest()
        {
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Test case 1: OR A, B
            cpu.A = 0b11001100;
            cpu.B = 0b10101010;
            cpu.Execute(0xB0); // OR A, B
            if (cpu.A != 0b11101110) return "Failed OR A, B";

            // Test case 2: OR A, (HL)
            cpu.A = 0b00001111;
            cpu.H = 0x12;
            cpu.L = 0x34;
            memory.WriteByte(0x1234, 0b11110000);
            cpu.Execute(0xB6); // OR A, (HL)
            if (cpu.A != 0b11111111) return "Failed OR A, (HL)";

            return "TestOR_A_r: Passed";
        }
    }

}
