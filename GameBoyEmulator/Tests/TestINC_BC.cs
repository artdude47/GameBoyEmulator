using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestINC_BC
    {
        public static string RunTest()
        {
            // Initialize memory and CPU
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Load mock ROM: INC BC (0x03)
            byte[] mockRom = new byte[]
            {
                0x03 // INC BC
            };
            memory.LoadRom(mockRom);

            // Set BC to a known value
            cpu.B = 0x12;
            cpu.C = 0x34;

            // Execute the instruction
            cpu.Step();

            // Expected BC value
            ushort expectedBC = 0x1235;

            // Verify the result
            if (cpu.BC == expectedBC)
            {
                return "TestINC_BC: Passed";
            }
            else
            {
                return $"TestINC_BC: Failed (Expected BC = {expectedBC:X4}, Got BC = {cpu.BC:X4})";
            }
        }
    }
}
