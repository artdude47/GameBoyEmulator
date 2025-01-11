using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBoyEmulator.Memory;
using GameBoyEmulator.CPU;

namespace GameBoyEmulator.Tests
{
    class TestCallAndRet
    {
        public static string RunTest()
        {
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Test case 1: CALL nn
            cpu.SP = 0xFFFE; // Initialize stack pointer
            memory.WriteByte(0x100, 0xCD); // CALL 0x1234
            memory.WriteByte(0x101, 0x34);
            memory.WriteByte(0x102, 0x12);
            cpu.PC = 0x100;
            cpu.Step();
            if (cpu.PC != 0x1234) return "Failed CALL nn";
            if (cpu.POP() != 0x103) return "Failed stack state after CALL nn";

            // Test case 2: RET
            cpu.PUSH(0x200); // Push return address
            cpu.Execute(0xC9); // RET
            if (cpu.PC != 0x200) return "Failed RET";

            return "TestCallAndRet: Passed";
        }
    }

}
