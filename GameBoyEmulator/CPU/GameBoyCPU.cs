using System;
using GameBoyEmulator.Memory;

namespace GameBoyEmulator.CPU
{
    class GameBoyCPU
    {
        #region variables
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
        public bool IME; // Interrupt Master Enable flag

        //Memory
        private readonly GameBoyMemory _memory;

        public GameBoyCPU(GameBoyMemory memory)
        {
            _memory = memory;
        }

        //Timers CPU runs at ~4.19 MHz => 4,194,304 cycles per second
        //DIV increments every 256 cycles => 16,384 Hz
        private int _divCounter; //For the DIV register increments
        private int _timerCounter; //For the TIMA increments

        #endregion

        #region OPCODE CYCLE LOOKUP
        public static readonly int[] BaseCycles = new int[256]
        {
            // 0x00-0x0F
            4,12, 8, 8, 4, 4, 8, 4, 20, 8, 8, 8, 4, 4, 8, 4,
            // 0x10-0x1F
            4,12, 8, 8, 4, 4, 8, 4, 12, 8, 8, 8, 4, 4, 8, 4,
            // 0x20-0x2F
            8,12, 8, 8, 4, 4, 8, 4,  8, 8, 8, 8, 4, 4, 8, 4,
            // 0x30-0x3F
            8,12, 8, 8,12,12,12, 4,  8, 8, 8, 8, 4, 4, 8, 4,
    
            // 0x40-0x4F
            4, 4, 4, 4, 4, 4, 8, 4,  4, 4, 4, 4, 4, 4, 8, 4,
            // 0x50-0x5F
            4, 4, 4, 4, 4, 4, 8, 4,  4, 4, 4, 4, 4, 4, 8, 4,
            // 0x60-0x6F
            4, 4, 4, 4, 4, 4, 8, 4,  4, 4, 4, 4, 4, 4, 8, 4,
            // 0x70-0x7F
            8, 8, 8, 8, 8, 8, 4, 8,  4, 4, 4, 4, 4, 4, 8, 4,
    
            // 0x80-0x8F
            4, 4, 4, 4, 4, 4, 8, 4,  4, 4, 4, 4, 4, 4, 8, 4,
            // 0x90-0x9F
            4, 4, 4, 4, 4, 4, 8, 4,  4, 4, 4, 4, 4, 4, 8, 4,
            // 0xA0-0xAF
            4, 4, 4, 4, 4, 4, 8, 4,  4, 4, 4, 4, 4, 4, 8, 4,
            // 0xB0-0xBF
            4, 4, 4, 4, 4, 4, 8, 4,  4, 4, 4, 4, 4, 4, 8, 4,
    
            // 0xC0-0xCF
            8,12,12,16,12,16, 8,16,  8,16,12, 4,12,24, 8,16,
            // 0xD0-0xDF
            8,12,12, 0,12,16, 8,16,  8,16,12, 0,12, 0, 8,16,
            // 0xE0-0xEF
            12,12, 8, 0, 0,16, 8,16, 16, 4,16, 0, 0, 0, 8,16,
            // 0xF0-0xFF
            12,12, 8, 4, 0,16, 8,16, 12, 8,16, 4, 0, 0, 8,16
        };

        public static readonly int[] CbCycles = new int[256]
        {
            // 0x00..0x07: RLC B..A  (Reg = 8, (HL) = 16) 
            8, 8, 8, 8, 8, 8,16, 8,
            // 0x08..0x0F: RRC B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0x10..0x17: RL B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0x18..0x1F: RR B..A
            8, 8, 8, 8, 8, 8,16, 8,

            // 0x20..0x27: SLA B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0x28..0x2F: SRA B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0x30..0x37: SWAP B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0x38..0x3F: SRL B..A
            8, 8, 8, 8, 8, 8,16, 8,

            // 0x40..0x47: BIT 0,B..A  (Reg=8, (HL)=12)
            8, 8, 8, 8, 8, 8,12, 8,
            // 0x48..0x4F: BIT 1,B..A
            8, 8, 8, 8, 8, 8,12, 8,
            // 0x50..0x57: BIT 2,B..A
            8, 8, 8, 8, 8, 8,12, 8,
            // 0x58..0x5F: BIT 3,B..A
            8, 8, 8, 8, 8, 8,12, 8,

            // 0x60..0x67: BIT 4,B..A
            8, 8, 8, 8, 8, 8,12, 8,
            // 0x68..0x6F: BIT 5,B..A
            8, 8, 8, 8, 8, 8,12, 8,
            // 0x70..0x77: BIT 6,B..A
            8, 8, 8, 8, 8, 8,12, 8,
            // 0x78..0x7F: BIT 7,B..A
            8, 8, 8, 8, 8, 8,12, 8,

            // 0x80..0x87: RES 0,B..A  (Reg=8, (HL)=16)
            8, 8, 8, 8, 8, 8,16, 8,
            // 0x88..0x8F: RES 1,B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0x90..0x97: RES 2,B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0x98..0x9F: RES 3,B..A
            8, 8, 8, 8, 8, 8,16, 8,

            // 0xA0..0xA7: RES 4,B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0xA8..0xAF: RES 5,B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0xB0..0xB7: RES 6,B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0xB8..0xBF: RES 7,B..A
            8, 8, 8, 8, 8, 8,16, 8,

            // 0xC0..0xC7: SET 0,B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0xC8..0xCF: SET 1,B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0xD0..0xD7: SET 2,B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0xD8..0xDF: SET 3,B..A
            8, 8, 8, 8, 8, 8,16, 8,

            // 0xE0..0xE7: SET 4,B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0xE8..0xEF: SET 5,B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0xF0..0xF7: SET 6,B..A
            8, 8, 8, 8, 8, 8,16, 8,
            // 0xF8..0xFF: SET 7,B..A
            8, 8, 8, 8, 8, 8,16, 8,
        };

        #endregion

        public int Step()
        {
            // Check and handle interrupts first
            HandleInterrupts(); 

            //IF HALT is active (and no interrupts woke it) or the CPU is stopped
            if (Halted)
            {
                // Return minimal cycles
                return 4;
            }
            if (Stopped)
            {
                return 4;
            }

            //Fetch opcode
            byte opcode = _memory.ReadByte(PC);

            // Get base cycles (minimum, not counting conditionals)
            int cyclesUsed = BaseCycles[opcode];

            //Increment the PC
            PC++;

            //Execute the opcode and add any additional cycles
            cyclesUsed += ExecuteInstructions(opcode);

            return cyclesUsed;
        }

        public void UpdateTimers(int cycles)
        {
            //UPDATE DIV
            _divCounter += cycles;
            while (_divCounter >= 256)
            {
                _divCounter -= 256;
                byte div = _memory.ReadByte(0xFF04); // Read DIV
                div++;
                _memory.WriteByte(0xFF04, div);
            }

            //Check TAC to see if TIMA is enabled
            byte tac = _memory.ReadByte(0xFF07);
            bool timerEnable = (tac & 0x04) != 0;
            if (!timerEnable) return;

            //Determine the timer's input clock frequency
            int inputClock;
            switch (tac & 0x03)
            {
                case 0: inputClock = 1024; break;
                case 1: inputClock = 16; break;
                case 2: inputClock = 64; break;
                case 3: inputClock = 256; break;
                default: inputClock = 1024; break;
            }

            // Increment TIMA
            _timerCounter += cycles;
            while (_timerCounter >= inputClock)
            {
                _timerCounter -= inputClock;

                byte tima = _memory.ReadByte(0xFF05);
                tima++;
                if (tima == 0)
                {
                    byte tma = _memory.ReadByte(0xFF06);
                    _memory.WriteByte(0xFF05, tma);

                    byte IF = _memory.ReadByte(0xFF0F);
                    IF |= 0x04;
                    _memory.WriteByte(0xFF0F, IF);
                }
                else
                {
                    _memory.WriteByte(0xFF05, tima);
                }
            }
        }

        private int ExecuteInstructions(byte opcode)
        {
            int extraCycles = 0; //Accumulate any extra cycles here
            bool taken = false;

            switch (opcode)
            {
                case 0x00: // NOP
                    break;

                case 0x01: // Example: LD BC, d16
                    LD_d16(ref B, ref C);
                    break;

                case 0x02: // LD (BC), A
                    LD_BC_A();
                    break;

                case 0x03: // INC BC
                    INC_BC();
                    break;

                case 0x06: // LD B, d8
                    LD_d8(ref B);
                    break;

                case 0x0A: // LD A, (BC)
                    LD_A_BC();
                    break;

                case 0x0B: // DEC BC
                    DEC_BC();
                    break;

                case 0x0E: // LD C, d8
                    LD_d8(ref C);
                    break;

                case 0x16: // LD D, d8
                    LD_d8(ref D);
                    break;

                case 0x1E: // LD E, d8
                    LD_d8(ref E);
                    break;

                case 0x21: // LD HL, d16
                    LD_d16(ref H, ref L);
                    break;

                case 0x22: // LD (HL+), A
                    LD_XXp_A(ref H, ref L);
                    break;

                case 0x23: // INC HL Register Pair
                    INC_HL_RegisterPair();
                    break;

                case 0x26: //LD H, d8
                    LD_d8(ref H);
                    break;

                case 0x2A: // LD A, (HL+)
                    LD_A_XXp(ref H, ref L);
                    break;

                case 0x2E: // LD L, d8
                    LD_d8(ref L);
                    break;

                case 0x32: // LD (HL-), A
                    LD_XXm_A(ref H, ref L);
                    break;

                case 0x3A: // LD A, (HL-)
                    LD_A_XXm(ref H, ref L);
                    break;

                case 0x3E: // LD A, d8
                    LD_d8(ref A);
                    break;

                case 0xE0: //LDH (a8), A
                    byte offset = _memory.ReadByte(PC);
                    PC++;

                    ushort address = (ushort)(0xFF00 + offset);
                    _memory.WriteByte(address, A);
                    break;

                case 0x47: //LD B, A
                    B = A;
                    break;

                case 0x78: // LD A, B
                    A = B;
                    break;

                case 0x79: // LD A, C
                    A = C;
                    break;

                case 0x7A: //LD A, D
                    A = D;
                    break;

                case 0x7B: //LD A, E
                    A = E;
                    break;

                case 0x7D: //LD A, L
                    A = L;
                    break;

                case 0x7C: //LD A, H
                    A = H;
                    break;

                case 0x7F: //LD A, A
                    A = A;
                    break;

                case 0xF0: //LDH A, (a8)
                    offset = _memory.ReadByte(PC);
                    PC++;
                    address = (ushort)(0xFF00 + offset);
                    A = _memory.ReadByte(address);
                    break;

                //ADDITION
                case 0x80: ADD_A_r(B); break; // ADD A, B
                case 0x81: ADD_A_r(C); break; // ADD A, C
                case 0x82: ADD_A_r(D); break; // ADD A, D
                case 0x83: ADD_A_r(E); break; // ADD A, E
                case 0x84: ADD_A_r(H); break; // ADD A, H
                case 0x85: ADD_A_r(L); break; // ADD A, L
                case 0x87: ADD_A_r(A); break; // ADD A, A
                case 0x86: ADD_A_r(_memory.ReadByte(HL)); break; // ADD A, (HL)

                //SUBTRACTION
                case 0x90: SUB_A_r(B); break; // SUB A, B
                case 0x91: SUB_A_r(C); break; // SUB A, C
                case 0x92: SUB_A_r(D); break; // SUB A, D
                case 0x93: SUB_A_r(E); break; // SUB A, E
                case 0x94: SUB_A_r(H); break; // SUB A, H
                case 0x95: SUB_A_r(L); break; // SUB A, L
                case 0x97: SUB_A_r(A); break; // SUB A, A
                case 0x96: SUB_A_r(_memory.ReadByte(HL)); break; // SUB A, (HL)

                //SUBTRACTION WITH CARRY
                case 0x98: SBC_A_r(B); break; // SBC A, B
                case 0x99: SBC_A_r(C); break; // SBC A, C
                case 0x9A: SBC_A_r(D); break; // SBC A, D
                case 0x9B: SBC_A_r(E); break; // SBC A, E
                case 0x9C: SBC_A_r(H); break; // SBC A, H
                case 0x9D: SBC_A_r(L); break; // SBC A, L
                case 0x9F: SBC_A_r(A); break; // SBC A, A
                case 0x9E: SBC_A_r(_memory.ReadByte(HL)); break; // SBC A, (HL)

                //BITWISE AND
                case 0xA0: AND_A_r(B); break; // AND A, B
                case 0xA1: AND_A_r(C); break; // AND A, C
                case 0xA2: AND_A_r(D); break; // AND A, D
                case 0xA3: AND_A_r(E); break; // AND A, E
                case 0xA4: AND_A_r(H); break; // AND A, H
                case 0xA5: AND_A_r(L); break; // AND A, L
                case 0xA7: AND_A_r(A); break; // AND A, A
                case 0xA6: AND_A_r(_memory.ReadByte(HL)); break; // AND A, (HL)

                //BITWISE OR
                case 0xB0: OR_A_r(B); break; // OR A, B
                case 0xB1: OR_A_r(C); break; // OR A, C
                case 0xB2: OR_A_r(D); break; // OR A, D
                case 0xB3: OR_A_r(E); break; // OR A, E
                case 0xB4: OR_A_r(H); break; // OR A, H
                case 0xB5: OR_A_r(L); break; // OR A, L
                case 0xB7: OR_A_r(A); break; // OR A, A
                case 0xB6: OR_A_r(_memory.ReadByte(HL)); break; // OR A, (HL)

                //BITWISE XOR
                case 0xA8: XOR_A_r(B); break; // XOR A, B
                case 0xA9: XOR_A_r(C); break; // XOR A, C
                case 0xAA: XOR_A_r(D); break; // XOR A, D
                case 0xAB: XOR_A_r(E); break; // XOR A, E
                case 0xAC: XOR_A_r(H); break; // XOR A, H
                case 0xAD: XOR_A_r(L); break; // XOR A, L
                case 0xAF: XOR_A_r(A); break; // XOR A, A
                case 0xAE: XOR_A_r(_memory.ReadByte(HL)); break; // XOR A, (HL)

                //COMPARE
                case 0xB8: CP_A_r(B); break; // CP A, B
                case 0xB9: CP_A_r(C); break; // CP A, C
                case 0xBA: CP_A_r(D); break; // CP A, D
                case 0xBB: CP_A_r(E); break; // CP A, E
                case 0xBC: CP_A_r(H); break; // CP A, H
                case 0xBD: CP_A_r(L); break; // CP A, L
                case 0xBF: CP_A_r(A); break; // CP A, A
                case 0xBE: CP_A_r(_memory.ReadByte(HL)); break; // CP A, (HL)
                case 0xFE: // CP A, d8
                    byte immediate = _memory.ReadByte(PC);
                    PC++;
                    CP_A_r(immediate);
                    break;

                //UNCONDITIONAL JUMPS
                case 0xC3: JP_nn(); break;

                //CONDITIONAL JUMPS
                case 0xCA: // JP Z, nn
                    taken = JP_cc_nn(true, 0x80);
                    if (taken) extraCycles += 4;
                    break; 
                case 0xC2: // JP NZ, nn
                    taken = JP_cc_nn(false, 0x80);
                    if (taken) extraCycles += 4;
                    break; 
                case 0xDA: // JP C, nn
                    taken = JP_cc_nn(true, 0x10);
                    if (taken) extraCycles += 4;
                    break; 
                case 0xD2: // JP NC, nn
                    taken = JP_cc_nn(false, 0x10);
                    if (taken) extraCycles += 4;
                    break; 

                //RELATIVE JUMPS
                case 0x18: JR_n(); break;
                case 0x28: // JR Z, n
                    taken = JR_cc_n(true, 0x80);
                    if (taken) extraCycles += 4;
                    break; 
                case 0x20: // JR NZ, n
                    taken = JR_cc_n(false, 0x80);
                    if (taken) extraCycles += 4;
                    break; 
                case 0x38: // JR C, n
                    taken = JR_cc_n(true, 0x10);
                    if (taken) extraCycles += 4;
                    break; 

                //CALL INSTRUCTIONS
                case 0xCD: CALL_nn(); break; //CALL nn
                case 0xCC: 
                    taken = CALL_cc_nn(true, 0x80);
                    if (taken) extraCycles += 12;
                    break; // CALL Z, nn
                case 0xC4:
                    taken = CALL_cc_nn(false, 0x80);
                    if (taken) extraCycles += 12;
                    break; // CALL NZ, nn
                case 0xDC:
                    taken = CALL_cc_nn(true, 0x10);
                    if (taken) extraCycles += 12;
                    break; // CALL C, nn
                case 0xD4:
                    taken = CALL_cc_nn(false, 0x10);
                    if (taken) extraCycles += 12;
                    break; // CALL NC, nn

                //RET INSTRUCTIONS
                case 0xC9: RET(); break; // RET
                case 0xC8: 
                    taken = RET_cc(true, 0x80);
                    if (taken) extraCycles += 12;
                    break; // RET Z
                case 0xC0:
                    taken = RET_cc(false, 0x80);
                    if (taken) extraCycles += 12;
                    break; // RET NZ
                case 0xD8:
                    taken = RET_cc(true, 0x10);
                    if (taken) extraCycles += 12;
                    break; // RET C
                case 0xD0:
                    taken = RET_cc(false, 0x10);
                    if (taken) extraCycles += 12;
                    break; // RET NC

                //PUSH INSTRUCTIONS
                case 0xC5: PUSH_16(BC); break; // PUSH BC
                case 0xD5: PUSH_16(DE); break; // PUSH DE
                case 0xE5: PUSH_16(HL); break; // PUSH HL
                case 0xF5: PUSH_16(AF); break; // PUSH AF


                //POP INSTRUCTIONS
                case 0xC1: POP_16(ref B, ref C); break; // POP BC
                case 0xD1: POP_16(ref D, ref E); break; // POP DE
                case 0xE1: POP_16(ref H, ref L); break; // POP HL
                case 0xF1: POP_16(ref A, ref F); break; // POP AF

                // Increment Instructions
                case 0x04: INC(ref B); break; // INC B
                case 0x0C: INC(ref C); break; // INC C
                case 0x14: INC(ref D); break; // INC D
                case 0x1C: INC(ref E); break; // INC E
                case 0x24: INC(ref H); break; // INC H
                case 0x2C: INC(ref L); break; // INC L
                case 0x3C: INC(ref A); break; // INC A
                case 0x34: INC_HL(); break;   // INC (HL)

                // Decrement Instructions
                case 0x05: DEC(ref B); break; // DEC B
                case 0x0D: DEC(ref C); break; // DEC C
                case 0x15: DEC(ref D); break; // DEC D
                case 0x1D: DEC(ref E); break; // DEC E
                case 0x25: DEC(ref H); break; // DEC H
                case 0x2D: DEC(ref L); break; // DEC L
                case 0x3D: DEC(ref A); break; // DEC A
                case 0x35: DEC_HL(); break;   // DEC (HL)

                case 0xCB: ExecuteCB(_memory.ReadByte(PC++)); break; // Handle CB-prefixed opcodes

                case 0x2F: CPL(); break; // CPL

                case 0x37: SCF(); break; // SCF

                case 0x3F: CCF(); break; // CCF

                case 0x27: DAA(); break; // DAA

                case 0xFB: EnableInterrupts(); break; // EI

                case 0xF3: DisableInterrupts(); break; // DI

                case 0xF2: LD_A_C(); break; // LD A, (C)
                case 0xE2: LD_C_A(); break; // LD (C), A
                case 0xFA: LD_A_nn(); break; // LD A, (n)
                case 0xEA: LD_nn_A(); break; // LD (n), A
                case 0xF9: LD_SP_HL(); break; // LD SP, HL
                case 0x31: LD_SP_d16(); break; // LD SP, d16

                case 0x76: HALT(); break; // HALT

                case 0x10: STOP(); break; // STOP

                default:
                    throw new NotImplementedException($"Unknown opcode: {opcode:X2}");
            }

            return extraCycles;
        }

        private void LD_d16(ref byte high, ref byte low)
        {
            low = _memory.ReadByte(PC);
            PC++;

            high = _memory.ReadByte(PC);
            PC++;
        }

        private void INC_BC()
        {
            ushort value = BC;
            value++;
            B = (byte)(value >> 8);
            C = (byte)(value & 0xFF);
        }

        private void INC(ref byte register)
        {
            byte result = (byte)(register + 1);

            // Update flags
            F &= 0x10; // Preserve the Carry flag (C)
            if ((result & 0xFF) == 0) F |= 0x80; // Set Zero flag (Z)
            if ((register & 0x0F) + 1 > 0x0F) F |= 0x20; // Set Half-Carry flag (H)

            register = result;
        }

        private void INC_HL()
        {
            byte value = _memory.ReadByte(HL);
            byte result = (byte)(value + 1);

            // Update flags
            F &= 0x10; // Preserve the Carry flag (C)
            if (result == 0) F |= 0x80; // Set Zero flag (Z)
            if ((value & 0x0F) + 1 > 0x0F) F |= 0x20; // Set Half-Carry flag (H)

            _memory.WriteByte(HL, result);
        }

        private void INC_HL_RegisterPair()
        {
            ushort value = HL;
            value++;
            H = (byte)(value >> 8);
            L = (byte)(value & 0xFF);
        }

        private void DEC_BC()
        {
            // Decrement the BC Register pair
            ushort value = BC;
            value--;
            B = (byte)(value >> 8);
            C = (byte)(value & 0xFF);
        }

        private void DEC(ref byte register)
        {
            byte result = (byte)(register - 1);

            // Update flags
            F = (byte)((F & 0x10) | 0x40); // Preserve Carry flag (C), set Subtract flag (N)
            if (result == 0) F |= 0x80; // Set Zero flag (Z)
            if ((register & 0x0F) == 0) F |= 0x20; // Set Half-Carry flag (H)

            register = result;
        }

        private void DEC_HL()
        {
            byte value = _memory.ReadByte(HL);
            byte result = (byte)(value - 1);

            // Update flags
            F = (byte)((F & 0x10) | 0x40); // Preserve Carry flag (C), set Subtract flag (N)
            if (result == 0) F |= 0x80; // Set Zero flag (Z)
            if ((value & 0x0F) == 0) F |= 0x20; // Set Half-Carry flag (H)

            _memory.WriteByte(HL, result);
        }

        private void LD_BC_A()
        {
            ushort address = BC;
            _memory.WriteByte(address, A);
        }

        private void LD_A_BC()
        {
            ushort address = BC;
            A = _memory.ReadByte(address);
        }

        private void LD_d8(ref byte register)
        {
            byte value = _memory.ReadByte(PC);
            PC++;

            register = value;
        }

        private void LD_XXp_A(ref byte high, ref byte low)
        {
            ushort address = GetValue(ref high, ref low);
            _memory.WriteByte(address, A);
            IncrementRegisterPair(ref high, ref low);
        }

        private void LD_XXm_A(ref byte high, ref byte low)
        {
            ushort address = GetValue(ref high, ref low);
            _memory.WriteByte(address, A);
            DecrementRegisterPair(ref high, ref low);
        }

        private void LD_A_XXp(ref byte high, ref byte low)
        {
            ushort address = GetValue(ref high, ref low);
            A = _memory.ReadByte(address);
            IncrementRegisterPair(ref high, ref low);
        }

        private void LD_A_XXm(ref byte high, ref byte low)
        {
            ushort address = GetValue(ref high, ref low);
            A = _memory.ReadByte(address);
            DecrementRegisterPair(ref high, ref low);
        }

        private void LD_A_C()
        {
            A = _memory.ReadByte((ushort)(0xFF00 + C));
        }

        private void LD_C_A()
        {
            _memory.WriteByte((ushort)(0xFF00 + C), A);
        }

        private void LD_A_nn()
        {
            ushort address = _memory.ReadByte(PC++);
            address |= (ushort)(_memory.ReadByte(PC++) << 8);
            A = _memory.ReadByte(address);
        }

        private void LD_SP_HL()
        {
            SP = HL;
        }

        private void LD_SP_d16()
        {
            SP = _memory.ReadByte(PC++);
            SP |= (ushort)(_memory.ReadByte(PC++) << 8);
        }

        private void LD_nn_A()
        {
            ushort address = _memory.ReadByte(PC++);
            address |= (ushort)(_memory.ReadByte(PC++) << 8);
            _memory.WriteByte(address, A);
        }

        private void IncrementRegisterPair(ref byte high, ref byte low)
        {
            ushort value = GetValue(ref high, ref low);
            value++;
            high = (byte)(value >> 8);
            low = (byte)(value & 0xFF);
        }

        private void DecrementRegisterPair(ref byte high, ref byte low)
        {
            ushort value = GetValue(ref high, ref low);
            value--;
            high = (byte)(value >> 8);
            low = (byte)(value & 0xFF);
        }

        private ushort GetValue(ref byte high, ref byte low)
        {
            return (ushort)((high << 8) | low);
        }

        private void ADD_A_r(byte value)
        {
            ushort result = (ushort)(A + value);

            //Update flags
            F = 0;
            if ((result & 0xFF) == 0) F |= 0x80; //Set Zero Flag
            if ((A & 0xF) + (value & 0xF) > 0xF) F |= 0x20; //Set half-carry flag
            if (result > 0xFF) F |= 0x10; //Set carry flag

            A = (byte)result;
        }

        private void SUB_A_r(byte value)
        {
            ushort result = (ushort)(A - value); // Perform subtraction

            // Update flags
            F = 0x40; // Set Subtract flag (N)
            if ((result & 0xFF) == 0) F |= 0x80; // Set Zero flag
            if ((A & 0xF) < (value & 0xF)) F |= 0x20; // Set Half-Carry flag
            if (result > 0xFF) F |= 0x10; // Set Carry flag

            A = (byte)result; // Store the result in A
        }

        private void SBC_A_r(byte value)
        {
            int carry = (F & 0x10) != 0 ? 1 : 0; // Get the carry flag
            ushort result = (ushort)(A - value - carry); // Perform subtraction with carry

            // Update flags
            F = 0x40; // Set Subtract flag (N)
            if ((result & 0xFF) == 0) F |= 0x80; // Set Zero flag
            if ((A & 0xF) < ((value & 0xF) + carry)) F |= 0x20; // Set Half-Carry flag
            if (result > 0xFF) F |= 0x10; // Set Carry flag

            A = (byte)result; // Store the result in A
        }

        private void AND_A_r(byte value)
        {
            A &= value; // Perform bitwise AND

            // Update flags
            F = 0x20; // Set Half-Carry flag (H), clear all others
            if (A == 0) F |= 0x80; // Set Zero flag
        }

        private void OR_A_r(byte value)
        {
            A |= value; // Perform bitwise OR

            // Update flags
            F = 0; // Clear all flags
            if (A == 0) F |= 0x80; // Set Zero flag
        }

        private void XOR_A_r(byte value)
        {
            A ^= value; // Perform bitwise XOR

            // Update flags
            F = 0; // Clear all flags
            if (A == 0) F |= 0x80; // Set Zero flag
        }

        private void CP_A_r(byte value)
        {
            ushort result = (ushort)(A - value); // Perform subtraction

            // Update flags
            F = 0x40; // Set Subtract flag (N)
            if ((result & 0xFF) == 0) F |= 0x80; // Set Zero flag
            if ((A & 0xF) < (value & 0xF)) F |= 0x20; // Set Half-Carry flag
            if (result > 0xFF) F |= 0x10; // Set Carry flag
        }

        private void JP_nn()
        {
            byte low = _memory.ReadByte(PC++);
            byte high = _memory.ReadByte(PC++);
            PC = (ushort)((high << 8) | low);
        }

        private bool JP_cc_nn(bool condition, byte flag)
        {
            bool taken = ((F & flag) != 0) == condition;
            if (taken)
            {
                JP_nn();
            }
            else
            {
                PC += 2;
            }
            return taken;
        }

        private void JR_n()
        {
            sbyte offset = (sbyte)_memory.ReadByte(PC++);
            PC = (ushort)(PC + offset);
        }

        private bool JR_cc_n(bool condition, byte flag)
        {
            bool taken = ((F & flag) != 0) == condition;
            if (taken)
            {
                JR_n();
            }
            else
            {
                PC++;
            }
            return taken;
        }

        private void CALL_nn()
        {
            byte low = _memory.ReadByte(PC++);
            byte high = _memory.ReadByte(PC++);
            ushort address = (ushort)((high << 8) | low);

            PUSH(PC); // Push current PC onto the stack
            PC = address; // Jump to the new address
        }

        private bool CALL_cc_nn(bool condition, byte flag)
        {
            bool taken = ((F & flag) != 0) == condition;
            if (taken)
            {
                CALL_nn();
            }
            else
            {
                PC += 2;
            }
            return taken;
        }

        private void RET()
        {
            PC = POP(); // Pop the return address from the stack
        }

        private bool RET_cc(bool condition, byte flag)
        {
            bool taken = ((F & flag) != 0) == condition;
            if (taken)
            {
                RET();
            }
            return taken;
        }

        public int ExecuteCB(byte opcode)
        {
            int bit = (opcode >> 3) & 0x07; // Extract bit number (0-7)
            int reg = opcode & 0x07;       // Extract register/memory (B, C, D, E, H, L, (HL), A)

            if (opcode >= 0x40 && opcode <= 0x7F) // BIT instructions
            {
                switch (reg)
                {
                    case 0: BIT(bit, B); return 8; // BIT b, B
                    case 1: BIT(bit, C); return 8; // BIT b, C
                    case 2: BIT(bit, D); return 8; // BIT b, D
                    case 3: BIT(bit, E); return 8; // BIT b, E
                    case 4: BIT(bit, H); return 8; // BIT b, H
                    case 5: BIT(bit, L); return 8; // BIT b, L
                    case 6: BIT_HL(bit); return 12; // BIT b, (HL)
                    case 7: BIT(bit, A); return 8; // BIT b, A
                }
            }

            if (opcode >= 0xC0 && opcode <= 0xFF) // SET instructions
            {
                bit = (opcode >> 3) & 0x07; // Extract the bit number (0–7)
                reg = opcode & 0x07;        // Extract the target (register or (HL))

                switch (reg)
                {
                    case 0: SET(bit, ref B); return 8; // SET b, B
                    case 1: SET(bit, ref C); return 8; // SET b, C
                    case 2: SET(bit, ref D); return 8; // SET b, D
                    case 3: SET(bit, ref E); return 8; // SET b, E
                    case 4: SET(bit, ref H); return 8; // SET b, H
                    case 5: SET(bit, ref L); return 8; // SET b, L
                    case 6: SET_HL(bit); return 16; // SET b, (HL)
                    case 7: SET(bit, ref A); return 8; // SET b, A
                }
            }

            if (opcode >= 0x80 && opcode <= 0xBF) // RES instructions
            {
                bit = (opcode >> 3) & 0x07;
                reg = opcode & 0x07;

                switch (reg)
                {
                    case 0: RES(bit, ref B); return 8; // RES b, B
                    case 1: RES(bit, ref C); return 8; // RES b, C
                    case 2: RES(bit, ref D); return 8; // RES b, D
                    case 3: RES(bit, ref E); return 8; // RES b, E
                    case 4: RES(bit, ref H); return 8; // RES b, H
                    case 5: RES(bit, ref L); return 8; // RES b, L
                    case 6: RES_HL(bit); return 16; // RES b, (HL)
                    case 7: RES(bit, ref A); return 8; // RES b, A
                }
            }

            if (opcode >= 0x00 && opcode <= 0x3F) // Rotate and Shift Instructions
            {
                reg = opcode & 0x07; // Extract register/memory (B, C, D, E, H, L, (HL), A)
                switch (opcode >> 3)
                {
                    case 0: RotateLeftCarry(reg); return reg == 6 ? 16 : 8; // RLC r
                    case 1: RotateRightCarry(reg); return reg == 6 ? 16 : 8; // RRC r
                    case 2: RotateLeft(reg); return reg == 6 ? 16 : 8; // RL r
                    case 3: RotateRight(reg); return reg == 6 ? 16 : 8; // RR r
                    case 4: ShiftLeftArithmetic(reg); return reg == 6 ? 16 : 8; // SLA r
                    case 5: ShiftRightArithmetic(reg); return reg == 6 ? 16 : 8; // SRA r
                    case 6: ShiftRightLogical(reg); return reg == 6 ? 16 : 8; // SRL r
                }
            }

            throw new NotImplementedException($"Unknown CB opcode: {opcode:X2}");
        }

        private void RotateLeftCarry(int reg)
        {
            byte value = GetRegisterValue(reg);
            byte carry = (byte)((value & 0x80) >> 7); // Extract bit 7

            value = (byte)((value << 1) | carry); // Rotate left through carry

            SetFlags(value, carry);
            SetRegisterValue(reg, value);
        }

        private void RotateRightCarry(int reg)
        {
            byte value = GetRegisterValue(reg);
            byte carry = (byte)(value & 0x01); // Extract bit 0

            value = (byte)((value >> 1) | (carry << 7)); // Rotate right through carry

            SetFlags(value, carry);
            SetRegisterValue(reg, value);
        }

        private void RotateLeft(int reg)
        {
            byte value = GetRegisterValue(reg);
            byte carry = (byte)((F & 0x10) >> 4); // Use Carry flag as input

            byte newCarry = (byte)((value & 0x80) >> 7); // Extract bit 7
            value = (byte)((value << 1) | carry);

            SetFlags(value, newCarry);
            SetRegisterValue(reg, value);
        }

        private void RotateRight(int reg)
        {
            byte value = GetRegisterValue(reg);
            byte carry = (byte)((F & 0x10) >> 4); // Use Carry flag as input

            byte newCarry = (byte)(value & 0x01); // Extract bit 0
            value = (byte)((value >> 1) | (carry << 7));

            SetFlags(value, newCarry);
            SetRegisterValue(reg, value);
        }

        private void ShiftLeftArithmetic(int reg)
        {
            byte value = GetRegisterValue(reg);
            byte carry = (byte)((value & 0x80) >> 7); // Extract bit 7 (Carry)

            value = (byte)(value << 1); // Perform arithmetic shift left

            // Update flags
            F = 0; // Clear all flags
            if (value == 0) F |= 0x80; // Set Zero flag (Z)
            if (carry == 1) F |= 0x10; // Set Carry flag (C)

            SetRegisterValue(reg, value);
        }

        private void ShiftRightArithmetic(int reg)
        {
            byte value = GetRegisterValue(reg);
            byte carry = (byte)(value & 0x01); // Extract bit 0

            value = (byte)((value >> 1) | (value & 0x80)); // Preserve sign bit

            SetFlags(value, carry);
            SetRegisterValue(reg, value);
        }

        private void ShiftRightLogical(int reg)
        {
            byte value = GetRegisterValue(reg);
            byte carry = (byte)(value & 0x01); // Extract bit 0

            value = (byte)(value >> 1); // Logical shift

            SetFlags(value, carry);
            SetRegisterValue(reg, value);
        }

        private byte GetRegisterValue(int reg)
        {
            switch (reg)
            {
                case 0: return B;
                case 1: return C;
                case 2: return D;
                case 3: return E;
                case 4: return H;
                case 5: return L;
                case 6: return _memory.ReadByte(HL);
                case 7: return A;
                default: throw new InvalidOperationException("Invalid register");
            }
        }

        private void SetRegisterValue(int reg, byte value)
        {
            switch (reg)
            {
                case 0: B = value; break;
                case 1: C = value; break;
                case 2: D = value; break;
                case 3: E = value; break;
                case 4: H = value; break;
                case 5: L = value; break;
                case 6: _memory.WriteByte(HL, value); break;
                case 7: A = value; break;
            }
        }

        private void SetFlags(byte result, byte carry)
        {
            F = 0;
            if (result == 0) F |= 0x80; // Zero flag
            if (carry != 0) F |= 0x10;  // Carry flag
        }

        private void BIT(int bit, byte value)
        {
            F &= 0x10; // Preserve the Carry flag (C), clear others
            F |= 0x20; // Set Half-Carry flag (H)
            if ((value & (1 << bit)) == 0) F |= 0x80; // Set Zero flag (Z) if the bit is 0
        }

        private void BIT_HL(int bit)
        {
            byte value = _memory.ReadByte(HL);
            BIT(bit, value);
        }

        private void SET(int bit, ref byte register)
        {
            register |= (byte)(1 << bit); // Set the specified bit in the register
        }

        private void SET_HL(int bit)
        {
            byte value = _memory.ReadByte(HL);
            value |= (byte)(1 << bit); // Set the specified bit in memory
            _memory.WriteByte(HL, value);
        }

        private void RES(int bit, ref byte register)
        {
            register &= (byte)~(1 << bit); // Clear the specified bit
        }

        private void RES_HL(int bit)
        {
            byte value = _memory.ReadByte(HL);
            value &= (byte)~(1 << bit);
            _memory.WriteByte(HL, value);
        }

        private void CPL()
        {
            A = (byte)~A; // Flip all bits in A
            F |= 0x60;    // Set N and H flags
        }

        private void SCF()
        {
            F &= 0x80; // Preserve the Zero (Z) flag, clear others
            F |= 0x10; // Set Carry (C) flag
        }

        private void CCF()
        {
            F ^= 0x10; // Toggle the Carry (C) flag
            F &= 0x80; // Preserve the Zero (Z) flag, clear N and H flags
        }

        private void DAA()
        {
            int adjustment = 0;
            if ((F & 0x20) != 0 || (A & 0x0F) > 9) adjustment |= 0x06; // Add 6 if lower nibble is invalid
            if ((F & 0x10) != 0 || A > 0x99) adjustment |= 0x60;      // Add 0x60 if upper nibble is invalid

            if ((F & 0x40) == 0) // If not a subtraction
                A += (byte)adjustment;
            else
                A -= (byte)adjustment;

            F &= 0x10; // Preserve the Carry flag
            if (A == 0) F |= 0x80; // Set Zero flag
            F &= unchecked((byte)~0x20); // Clear Half-Carry flag
            if (adjustment >= 0x60) F |= 0x10; // Set Carry flag if overflow
        }

        #region INTERRUPTIONS

        private void EnableInterrupts()
        {
            IME = true; // Enable interrupts
        }

        private void DisableInterrupts()
        {
            IME = false; // Disable interrupts
        }

        private void HandleInterrupts()
        {
            if (!IME) return;

            byte IE = _memory.ReadByte(0xFFFF);
            byte IF = _memory.ReadByte(0xFF0F);
            byte triggered = (byte)(IE & IF);

            Console.WriteLine($"HandleInterrupts - IE: {IE:X2}, IF: {IF:X2}, Triggered: {triggered:X2}");

            if (triggered == 0) return;

            for (int i = 0; i < 5; i++)
            {
                if ((triggered & (1 << i)) != 0)
                {
                    Console.WriteLine($"Handling Interrupt {i}, Vector: {0x0040 + (i * 8):X4}");
                    PUSH(PC);
                    PC = (ushort)(0x0040 + (i * 8));
                    _memory.WriteByte(0xFF0F, (byte)(IF & ~(1 << i)));
                    IME = false;
                    Halted = false;
                    return;
                }
            }
        }

        private void HALT()
        {
            if (IME)
            {
                Halted = true; // Enter low-power mode
            }
            else
            {
                // Halt bug behavior: Execution continues but PC does not advance
                // Commonly used for interrupt handling quirks
            }
        }

        private void STOP()
        {
            if (_memory.ReadByte(PC) == 0x00) // Ensure the next byte is 0x00
            {
                PC++; // Skip the 0x00 byte
                Stopped = true; // Enter ultra-low-power mode
            }
            else
            {
                throw new InvalidOperationException("Invalid STOP instruction sequence");
            }
        }


        #endregion

        #region STACK OPERATIONS
        public void PUSH(ushort value)
        {
            SP -= 2; //Decrement stack pointer by 2
            _memory.WriteByte(SP, (byte)(value & 0xFF)); //Low byte
            _memory.WriteByte((ushort)(SP + 1), (byte)(value >> 8)); //High byte;
        }

        public ushort POP()
        {
            ushort value = (ushort)(_memory.ReadByte(SP) | (_memory.ReadByte((ushort)(SP + 1)) << 8));
            SP += 2;
            return value;
        }

        private void PUSH_16(ushort value)
        {
            PUSH(value); // Use the existing PUSH method
        }

        private void POP_16(ref byte high, ref byte low)
        {
            ushort value = POP(); // Use the existing POP method
            high = (byte)(value >> 8); // Extract high byte
            low = (byte)(value & 0xFF); // Extract low byte
        }

        #endregion
    }
}
