using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestLD_HL_Instructions
    {
        public static string RunTest()
        {
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Load mock ROM
            byte[] mockRom = new byte[]
            {
                0x22, // LD (HL+), A
                0x32, // LD (HL-), A
                0x2A, // LD A, (HL+)
                0x3A  // LD A, (HL-)
            };
            memory.LoadRom(mockRom);

            // Set initial values
            cpu.H = 0x12;
            cpu.L = 0x34;
            cpu.A = 0x56;

            // Execute LD (HL+), A
            cpu.Step();
            if (memory.ReadByte(0x1234) != 0x56) return "Failed LD (HL+), A";
            if (cpu.HL != 0x1235) return "Failed HL increment after LD (HL+), A";

            // Execute LD (HL-), A
            cpu.Step();
            if (memory.ReadByte(0x1235) != 0x56) return "Failed LD (HL-), A";
            if (cpu.HL != 0x1234) return "Failed HL decrement after LD (HL-), A";

            // Write value at 0x1234 for LD A, (HL+)
            memory.WriteByte(0x1234, 0x78);

            // Execute LD A, (HL+)
            cpu.Step();
            if (cpu.A != 0x78) return "Failed LD A, (HL+)";
            if (cpu.HL != 0x1235) return "Failed HL increment after LD A, (HL+)";

            // Write value at 0x1235 for LD A, (HL-)
            memory.WriteByte(0x1235, 0x9A);

            // Execute LD A, (HL-)
            cpu.Step();
            if (cpu.A != 0x9A) return "Failed LD A, (HL-)";
            if (cpu.HL != 0x1234) return "Failed HL decrement after LD A, (HL-)";

            return "TestLD_HL_Instructions: Passed";
        }
    }
}
