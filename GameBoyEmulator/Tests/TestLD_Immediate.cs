using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestLD_Immediate
    {
        public static string RunTest()
        {
            // Initialize memory and CPU
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Mock ROM to test LD instructions
            byte[] mockRom = new byte[]
            {
                0x06, 0x12, // LD B, 0x12
                0x0E, 0x34, // LD C, 0x34
                0x16, 0x56, // LD D, 0x56
                0x1E, 0x78, // LD E, 0x78
                0x26, 0x9A, // LD H, 0x9A
                0x2E, 0xBC, // LD L, 0xBC
            };
            memory.LoadRom(mockRom);

            // Set HL to point to memory address 0x1234
            cpu.H = 0x12;
            cpu.L = 0x34;

            // Execute each instruction
            for (int i = 0; i < mockRom.Length; i++)
            {
                cpu.Step();
            }

            // Verify results
            if (cpu.B != 0x12) return "TestLD_Immediate: Failed on LD B, d8";
            if (cpu.C != 0x34) return "TestLD_Immediate: Failed on LD C, d8";
            if (cpu.D != 0x56) return "TestLD_Immediate: Failed on LD D, d8";
            if (cpu.E != 0x78) return "TestLD_Immediate: Failed on LD E, d8";
            if (cpu.H != 0x9A) return "TestLD_Immediate: Failed on LD H, d8";
            if (cpu.L != 0xBC) return "TestLD_Immediate: Failed on LD L, d8";

            return "TestLD_Immediate: Passed";
        }
    }
}
