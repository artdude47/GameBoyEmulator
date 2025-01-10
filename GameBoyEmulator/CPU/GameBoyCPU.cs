using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.CPU
{
    class GameBoyCPU
    {
        //8-bit registers
        public byte A, B, C, D, E, H, L;

        //Flags register
        public byte F;

        //16-bit registers (can be accessed as pairs)
        public ushort AF => (ushort)((A << 8) | F);
        public ushort BC => (ushort)((B << 8) | C);
        public ushort DE => (ushort)((D << 8) | E);
        public ushort HL => (ushort)((H << 8) | L);

        //Program counter and stack pointer
        public ushort PC { get; set; }
        public ushort SP { get; set; }

        //CPU State 
        public bool Halted { get; set; }
        public bool Stopped { get; set; }

        //Memory
        private readonly GameBoyMemory _memory;

        public GameBoyCPU(GameBoyMemory memory)
        {
            _memory = memory;
        }

        public void Step()
        {
            //Fetch opcode
            byte opcode = _memory.ReadByte(PC);

            //Increment the PC
            PC++;

            //Execute the opcode
            Execute(opcode);
        }

        public void Execute(byte opcode)
        {
            switch (opcode)
            {
                case 0x00: // NOP
                    break;

                case 0x01: // Example: LD BC, d16
                    LD_BC_d16();
                    break;

                case 0x03: // INC BC
                    INC_BC();
                    break;

                case 0x0B: // DEC BC
                    DEC_BC();
                    break;

                default:
                    throw new NotImplementedException($"Unknown opcode: {opcode:X2}");
            }
        }

        private void LD_BC_d16()
        {
            byte low = _memory.ReadByte(PC);
            PC++;

            byte high = _memory.ReadByte(PC);
            PC++;

            B = high;
            C = low;
        }

        private void INC_BC()
        {
            ushort value = BC;
            value++;
            B = (byte)(value >> 8);
            C = (byte)(value & 0xFF);
        }

        private void DEC_BC()
        {
            // Decrement the BC Register pair
            ushort value = BC;
            value--;
            B = (byte)(value >> 8);
            C = (byte)(value & 0xFF);
        }
    }
}
