using GameBoyEmulator.Memory;
using GameBoyEmulator.CPU;

class TestADD_A_r
{
    public static string RunTest()
    {
        GameBoyMemory memory = new GameBoyMemory();
        GameBoyCPU cpu = new GameBoyCPU(memory);

        // Test case 1: ADD A, B
        cpu.A = 0x10;
        cpu.B = 0x20;
        cpu.Execute(0x80); // ADD A, B
        if (cpu.A != 0x30) return "Failed ADD A, B";

        // Test case 2: ADD A, (HL)
        cpu.A = 0x01;
        cpu.H = 0x12;
        cpu.L = 0x34;
        memory.WriteByte(0x1234, 0x05);
        cpu.Execute(0x86); // ADD A, (HL)
        if (cpu.A != 0x06) return "Failed ADD A, (HL)";

        return "TestADD_A_r: Passed";
    }
}
