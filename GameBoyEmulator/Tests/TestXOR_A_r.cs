using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestXOR_A_r
    {
        public static string RunTest()
        {
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Test case 1: XOR A, B
            cpu.A = 0b11001100;
            cpu.B = 0b10101010;
            cpu.Execute(0xA8); // XOR A, B
            if (cpu.A != 0b01100110) return "Failed XOR A, B";

            // Test case 2: XOR A, (HL)
            cpu.A = 0b11110000;
            cpu.H = 0x12;
            cpu.L = 0x34;
            memory.WriteByte(0x1234, 0b10101010);
            cpu.Execute(0xAE); // XOR A, (HL)
            if (cpu.A != 0b01011010) return "Failed XOR A, (HL)";

            return "TestXOR_A_r: Passed";
        }
    }

}
