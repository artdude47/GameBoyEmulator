using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.Tests
{
    class MemoryTests
    {
        public static string RunTests()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine(TestMemoryReadWrite());
            result.AppendLine(TestEchoRam());

            return result.ToString();
        }

        private static string TestMemoryReadWrite()
        {
            GameBoyMemory memory = new GameBoyMemory();

            //Write to an address
            ushort address = 0x1234;
            byte value = 0x32;
            memory.WriteByte(address, value);

            //Read it back from memory
            byte readValue = memory.ReadByte(address);

            return readValue == value
                ? "TestMemoryReadWrite: Passed"
                : $"TestMemoryReadWrite: Failed (Expected {value}, Got {readValue})";
        }

        private static string TestEchoRam()
        {
            GameBoyMemory memory = new GameBoyMemory();

            ushort workRamAddress = 0xC001;
            byte value = 0x37;
            memory.WriteByte(workRamAddress, value);

            // Read it from Echo RAM (0xE000 mirror)
            ushort echoRamAddress = (ushort)(workRamAddress + 0x2000);
            byte readValue = memory.ReadByte(echoRamAddress);

            return readValue == value
                ? "TestEchoRam: Passed"
                : $"TestEchoRam: Failed (Expected {value}, Got {readValue})";
        }
    }
}
