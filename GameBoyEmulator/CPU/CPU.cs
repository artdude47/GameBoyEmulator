using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyEmulator.CPU
{
    class CPU
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

        public void Step()
        {
            //Fetch opcode
            byte opcode = ReadByte(PC);

            //Increment the PC
            PC++;

            //Execute the opcode
            Execute(opcode);
        }

        public byte ReadByte(ushort address)
        {
            return 0; //Need memory sorted out
        }

        public void Execute(byte opcode)
        {
            switch (opcode)
            {
                case 0x00: // NOP
                    break;

                default:
                    throw new NotImplementedException($"Unknown opcode: {opcode:X2}");
            }
        }
    }
}
