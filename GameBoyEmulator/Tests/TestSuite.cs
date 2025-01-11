using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestSuite
    {
        public static string RunAllTests()
        {
            StringBuilder results = new StringBuilder();

            results.AppendLine(TestBitManipulation());
            results.AppendLine(TestMiscALU());
            results.AppendLine(TestInterruptHandling());
            results.AppendLine(TestRotateShift());
            results.AppendLine(TestLoadStore());
            results.AppendLine(TestHaltStop());
            results.AppendLine(TestTimers());

            return results.ToString();
        }

        // Test Phase 1: Bit Manipulation
        private static string TestBitManipulation()
        {
            try
            {
                GameBoyMemory memory = new GameBoyMemory();
                GameBoyCPU cpu = new GameBoyCPU(memory);

                // Test BIT instruction
                cpu.B = 0xFE; // All bits except bit 0 are set
                cpu.ExecuteCB(0x40); // BIT 0, B
                if ((cpu.F & 0x80) == 0) return "Failed BIT instruction";

                // Test SET instruction
                cpu.ExecuteCB(0xC0); // SET 0, B
                if (cpu.B != 0xFF) return "Failed SET instruction";

                // Test RES instruction
                cpu.ExecuteCB(0x80); // RES 0, B
                if (cpu.B != 0xFE) return "Failed RES instruction";

                return "TestBitManipulation: Passed";
            }
            catch (Exception ex)
            {
                return $"TestBitManipulation: Failed ({ex.Message})";
            }
        }

        // Test Phase 2: Miscellaneous ALU Instructions
        private static string TestMiscALU()
        {
            try
            {
                GameBoyMemory memory = new GameBoyMemory();
                GameBoyCPU cpu = new GameBoyCPU(memory);

                // Test CPL instruction
                cpu.A = 0x55;
                cpu.Execute(0x2F); // CPL
                if (cpu.A != 0xAA) return "Failed CPL instruction";

                // Test SCF instruction
                cpu.F = 0x00;
                cpu.Execute(0x37); // SCF
                if ((cpu.F & 0x10) == 0) return "Failed SCF instruction";

                // Test DAA instruction
                cpu.A = 0x45;
                cpu.F = 0x00;
                cpu.Execute(0x27); // DAA
                if (cpu.A != 0x45) return "Failed DAA instruction (normal case)";

                return "TestMiscALU: Passed";
            }
            catch (Exception ex)
            {
                return $"TestMiscALU: Failed ({ex.Message})";
            }
        }

        // Test Phase 3: Interrupt Handling
        private static string TestInterruptHandling()
        {
            try
            {
                GameBoyMemory memory = new GameBoyMemory();
                GameBoyCPU cpu = new GameBoyCPU(memory);

                // Set up an interrupt request
                memory.WriteByte(0xFFFF, 0x01); // Enable V-Blank interrupt
                memory.WriteByte(0xFF0F, 0x01); // Request V-Blank interrupt
                cpu.IME = true;
                cpu.PC = 0x100;
                cpu.Step(); // Handle interrupt
                if (cpu.PC != 0x0040) return $"Failed V-Blank interrupt (PC = {cpu.PC:X4})";
                if (memory.ReadByte(0xFF0F) != 0x00) return "Failed V-Blank interrupt (IF not cleared)";

                return "TestInterruptHandling: Passed";
            }
            catch (Exception ex)
            {
                return $"TestInterruptHandling: Failed ({ex.Message})";
            }
        }

        // Test Phase 4: Rotate and Shift Instructions
        private static string TestRotateShift()
        {
            try
            {
                GameBoyMemory memory = new GameBoyMemory();
                GameBoyCPU cpu = new GameBoyCPU(memory);

                // Test RLC instruction
                cpu.A = 0x80;
                cpu.ExecuteCB(0x07); // RLC A
                if (cpu.A != 0x01 || (cpu.F & 0x10) == 0) return "Failed RLC instruction";

                // Test SLA instruction
                cpu.A = 0x40;         // Initial value where bit 7 is not set
                cpu.ExecuteCB(0x27);  // SLA A
                if (cpu.A != 0x80) return "Failed SLA instruction (Result Incorrect)";
                if ((cpu.F & 0x10) != 0) return "Failed SLA instruction (Carry Flag Incorrect)";

                return "TestRotateShift: Passed";
            }
            catch (Exception ex)
            {
                return $"TestRotateShift: Failed ({ex.Message})";
            }
        }

        // Test Phase 5: Load/Store Instructions
        private static string TestLoadStore()
        {
            try
            {
                GameBoyMemory memory = new GameBoyMemory();
                GameBoyCPU cpu = new GameBoyCPU(memory);

                // Test LD A, (C)
                cpu.C = 0x10;
                memory.WriteByte(0xFF10, 0xAB);
                cpu.Execute(0xF2); // LD A, (C)
                if (cpu.A != 0xAB) return "Failed LD A, (C)";

                // Test LD (C), A
                cpu.A = 0xCD;
                cpu.Execute(0xE2); // LD (C), A
                if (memory.ReadByte(0xFF10) != 0xCD) return "Failed LD (C), A";

                return "TestLoadStore: Passed";
            }
            catch (Exception ex)
            {
                return $"TestLoadStore: Failed ({ex.Message})";
            }
        }

        // Test Phase 6: HALT and STOP
        private static string TestHaltStop()
        {
            try
            {
                GameBoyMemory memory = new GameBoyMemory();
                GameBoyCPU cpu = new GameBoyCPU(memory);

                // Test HALT instruction
                cpu.IME = true;
                cpu.Execute(0x76); // HALT
                if (!cpu.Halted) return "Failed HALT instruction";

                // Test STOP instruction
                memory.WriteByte(cpu.PC, 0x00); // Ensure 0x00 follows STOP
                cpu.Execute(0x10); // STOP
                if (!cpu.Stopped) return "Failed STOP instruction";

                return "TestHaltStop: Passed";
            }
            catch (Exception ex)
            {
                return $"TestHaltStop: Failed ({ex.Message})";
            }
        }

        private static string TestTimers()
        {
            try
            {
                GameBoyMemory memory = new GameBoyMemory();
                GameBoyCPU cpu = new GameBoyCPU(memory);

                // Test DIV increment
                for (int i = 0; i < 64; i++) cpu.Step();
                if (memory.ReadByte(0xFF04) == 0) return "Failed DIV increment test";

                // Test TIMA overflow
                memory.WriteByte(0xFF07, 0x05); // Enable timer, set frequency to 4,096 Hz
                memory.WriteByte(0xFF06, 0xAA); // Set TMA to 0xAA
                memory.WriteByte(0xFF05, 0xFF); // Set TIMA to 0xFF
                cpu.Step();
                if (memory.ReadByte(0xFF05) != 0xAA) return "Failed TIMA overflow test";
                if ((memory.ReadByte(0xFF0F) & 0x04) == 0) return "Failed Timer Interrupt Request";

                return "TestTimers: Passed";
            }
            catch (Exception ex)
            {
                return $"TestTimers: Failed ({ex.Message})";
            }
        }
    }

}
