using System;

namespace GameBoyEmulator.Memory
{
    class GameBoyMemory
    {
        private byte[] _memory;
        private byte DIV; //Divider Register
        private byte TIMA; //Timer Counter
        private byte TMA; //Timer Modulo
        private byte TAC; //Timer Control

        public GameBoyMemory()
        {
            //Initialize 64kb of memory
            _memory = new byte[0x10000];
        }

        public byte ReadByte(ushort address)
        {
            switch (address)
            {
                // Timer registers
                case 0xFF04: return DIV;  // Divider register
                case 0xFF05: return TIMA; // Timer counter
                case 0xFF06: return TMA;  // Timer modulo
                case 0xFF07: return TAC;  // Timer control

                // Mirror of work RAM
                default:
                    if (address >= 0xE000 && address <= 0xFDFF)
                    {
                        return _memory[address - 0x2000];
                    }
                    return _memory[address];
            }
        }

        public void WriteByte(ushort address, byte value)
        {
            switch (address)
            {
                // Timer registers
                case 0xFF04: // Writing to DIV resets it
                    DIV = 0;
                    break;
                case 0xFF05: // TIMA
                    TIMA = value;
                    break;
                case 0xFF06: // TMA
                    TMA = value;
                    break;
                case 0xFF07: // TAC
                    TAC = value;
                    break;

                // Mirror of work RAM
                default:
                    if (address >= 0xE000 && address <= 0xFDFF)
                    {
                        _memory[address - 0x2000] = value;
                    }
                    else if (address >= 0xFEA0 && address <= 0xFEFF)
                    {
                        // Unused memory; writes are ignored
                        return;
                    }
                    else
                    {
                        _memory[address] = value;
                    }
                    break;
            }
        }

        public void LoadRom(byte[] romData)
        {
            if (romData.Length > _memory.Length)
                throw new ArgumentException("ROM data exceeds memory size!");

            Array.Copy(romData, _memory, romData.Length);
        }
    }
}
