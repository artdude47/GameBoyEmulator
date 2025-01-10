using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestLD_BC_A
    {
        public static string RunTest()
        {
            // Initialize memory and CPU
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Load mock ROM: LD (BC), A (0x02)
            byte[] mockRom = new byte[]
            {
                0x02 // LD (BC), A
            };
            memory.LoadRom(mockRom);

            // Set BC and A to known values
            cpu.B = 0x12;
            cpu.C = 0x34;
            cpu.A = 0x56;

            // Execute the instruction
            cpu.Step();

            // Verify the result
            byte expectedValue = 0x56;
            byte actualValue = memory.ReadByte(0x1234);

            if (actualValue == expectedValue)
            {
                return "TestLD_BC_A: Passed";
            }
            else
            {
                return $"TestLD_BC_A: Failed (Expected Memory[0x1234] = {expectedValue:X2}, Got {actualValue:X2})";
            }
        }
    }
}
