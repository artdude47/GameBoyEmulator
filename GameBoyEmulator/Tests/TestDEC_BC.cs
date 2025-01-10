using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestDEC_BC
    {
        public static string RunTest()
        {
            // Initialize memory and CPU
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Load mock ROM: DEC BC (0x0B)
            byte[] mockRom = new byte[]
            {
                0x0B // DEC BC
            };
            memory.LoadRom(mockRom);

            // Set BC to a known value
            cpu.B = 0x12;
            cpu.C = 0x34;

            // Execute the instruction
            cpu.Step();

            // Expected BC value
            ushort expectedBC = 0x1233;

            // Verify the result
            if (cpu.BC == expectedBC)
            {
                return "TestDEC_BC: Passed";
            }
            else
            {
                return $"TestDEC_BC: Failed (Expected BC = {expectedBC:X4}, Got BC = {cpu.BC:X4})";
            }
        }
    }
}
