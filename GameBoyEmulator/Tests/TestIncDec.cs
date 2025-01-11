using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBoyEmulator.Memory;
using GameBoyEmulator.CPU;

namespace GameBoyEmulator.Tests
{
    class TestIncDec
    {
        public static string RunTest()
        {
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Test case 1: INC B
            cpu.B = 0x0F;
            cpu.Execute(0x04); // INC B
            if (cpu.B != 0x10 || (cpu.F & 0x20) == 0) return "Failed INC B (Half-Carry)";

            // Test case 2: DEC B
            cpu.B = 0x01;
            cpu.Execute(0x05); // DEC B
            if (cpu.B != 0x00 || (cpu.F & 0x80) == 0) return "Failed DEC B (Zero)";

            // Test case 3: INC (HL)
            cpu.H = 0x12;
            cpu.L = 0x34;
            memory.WriteByte(0x1234, 0xFF);
            cpu.Execute(0x34); // INC (HL)
            if (memory.ReadByte(0x1234) != 0x00 || (cpu.F & 0x80) == 0) return "Failed INC (HL) (Zero)";

            // Test case 4: DEC (HL)
            memory.WriteByte(0x1234, 0x01);
            cpu.Execute(0x35); // DEC (HL)
            if (memory.ReadByte(0x1234) != 0x00 || (cpu.F & 0x80) == 0) return "Failed DEC (HL) (Zero)";

            return "TestIncDec: Passed";
        }
    }

}
