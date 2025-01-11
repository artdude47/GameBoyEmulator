using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestJumps
    {
        public static string RunTest()
        {
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Unconditional JP
            memory.WriteByte(0x100, 0xC3); // JP 0x1234
            memory.WriteByte(0x101, 0x34);
            memory.WriteByte(0x102, 0x12);
            cpu.PC = 0x100;
            cpu.Step();
            if (cpu.PC != 0x1234) return "Failed JP nn";

            // Conditional JP
            cpu.F = 0x80; // Zero flag set
            memory.WriteByte(0x1234, 0xCA); // JP Z, 0x5678
            memory.WriteByte(0x1235, 0x78);
            memory.WriteByte(0x1236, 0x56);
            cpu.Step();
            if (cpu.PC != 0x5678) return "Failed JP Z, nn";

            // Relative JR
            memory.WriteByte(0x5678, 0x18); // JR +2
            memory.WriteByte(0x5679, 0x02);
            cpu.Step();
            if (cpu.PC != 0x567C) return "Failed JR n";

            return "TestJumps: Passed";
        }
    }

}
