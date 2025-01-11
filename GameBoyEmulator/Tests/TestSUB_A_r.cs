using GameBoyEmulator.Memory;
using GameBoyEmulator.CPU;

class TestSUB_A_r
{
    public static string RunTest()
    {
        GameBoyMemory memory = new GameBoyMemory();
        GameBoyCPU cpu = new GameBoyCPU(memory);

        // Test case 1: SUB A, B
        cpu.A = 0x30;
        cpu.B = 0x10;
        cpu.Execute(0x90); // SUB A, B
        if (cpu.A != 0x20) return "Failed SUB A, B";

        // Test case 2: SUB A, (HL)
        cpu.A = 0x08;
        cpu.H = 0x12;
        cpu.L = 0x34;
        memory.WriteByte(0x1234, 0x03);
        cpu.Execute(0x96); // SUB A, (HL)
        if (cpu.A != 0x05) return "Failed SUB A, (HL)";

        return "TestSUB_A_r: Passed";
    }
}
