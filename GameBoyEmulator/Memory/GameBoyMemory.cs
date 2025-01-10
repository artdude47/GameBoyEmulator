using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyEmulator.Memory
{
    class GameBoyMemory
    {
        private byte[] _memory;

        public GameBoyMemory()
        {
            //Initialize 64kb of memory
            _memory = new byte[0x10000];
        }

        public byte ReadByte(ushort address)
        {
            if (address >= 0xE000 && address <= 0xFDFF)
            {
                //Mirror of work RAM
                return _memory[address - 0x2000];
            }
            return _memory[address];
        }

        public void WriteByte(ushort address, byte value)
        {
            if (address >= 0xE000 && address <= 0xFDFF)
            {
                _memory[address - 0x2000] = value;
            }
            else if (address >= 0xFEA0 && address <= 0xFEFF)
            {
                //Unused memory; writes are ignored
                return;
            }
            else
            {
                _memory[address] = value;
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
