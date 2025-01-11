using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBoyEmulator.Memory;
using GameBoyEmulator.CPU;

namespace GameBoyEmulator.Tests
{
    class TestPushAndPop
    {
        public static string RunTest()
        {
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Initialize stack pointer
            cpu.SP = 0xFFFE;

            // Test case 1: PUSH BC
            cpu.B = 0x12;
            cpu.C = 0x34;
            cpu.Execute(0xC5); // PUSH BC
            if (memory.ReadByte(0xFFFD) != 0x12 || memory.ReadByte(0xFFFC) != 0x34) return "Failed PUSH BC";

            // Test case 2: POP DE
            memory.WriteByte(0xFFFD, 0x56); // High byte
            memory.WriteByte(0xFFFC, 0x78); // Low byte
            cpu.SP = 0xFFFC; // Set SP to start of data
            cpu.Execute(0xD1); // POP DE
            if (cpu.D != 0x56 || cpu.E != 0x78) return "Failed POP DE";

            return "TestPushAndPop: Passed";
        }
    }

}
