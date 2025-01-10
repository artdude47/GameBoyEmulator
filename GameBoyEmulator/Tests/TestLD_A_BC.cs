using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestLD_A_BC
    {
        public static string RunTest()
        {
            // Initialize memory and CPU
            GameBoyMemory memory = new GameBoyMemory();
            GameBoyCPU cpu = new GameBoyCPU(memory);

            // Load mock ROM: LD A, (BC) (0x0A)
            byte[] mockRom = new byte[]
            {
                0x0A // LD A, (BC)
            };
            memory.LoadRom(mockRom);

            // Set BC to a known value and write to memory
            cpu.B = 0x12;
            cpu.C = 0x34;
            memory.WriteByte(0x1234, 0x78); // Write value at address 0x1234

            // Execute the instruction
            cpu.Step();

            // Verify the result
            byte expectedValue = 0x78;
            byte actualValue = cpu.A;

            if (actualValue == expectedValue)
            {
                return "TestLD_A_BC: Passed";
            }
            else
            {
                return $"TestLD_A_BC: Failed (Expected A = {expectedValue:X2}, Got {actualValue:X2})";
            }
        }
    }
}
