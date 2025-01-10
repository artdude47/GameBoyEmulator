
using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class TestLD_BC_d16
    {
        public static string RunTest()
        {
            GameBoyMemory memory = new GameBoyMemory();

            byte[] mockRom = new byte[]
            {
                0x01, 0x34, 0x12,
                0x00
            };
            memory.LoadRom(mockRom);

            GameBoyCPU cpu = new GameBoyCPU(memory);

            cpu.Step();

            ushort expectedBC = 0x1234;
            ushort actualBC = cpu.BC;

            if (actualBC == expectedBC)
            {
                return "Passed!";
            }
            else
            {
                return $"TestLD_BC_d16: Failed (Expected BC = {expectedBC:X4}, Got BC = {actualBC:X4})";
            }
        }
    }
}
