using System;
using System.Text;
using System.Windows.Forms;
using GameBoyEmulator.CPU;
using GameBoyEmulator.Memory;
using System.Threading.Tasks;
using System.IO;

namespace GameBoyEmulator
{
    public partial class Form1 : Form
    {
        private GameBoyMemory _memory;
        private GameBoyCPU _cpu;
        private bool _running;
        private Task _emulationTask;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _memory = new GameBoyMemory();
            _cpu = new GameBoyCPU(_memory);

            _running = true;
            _emulationTask = Task.Run(() => EmulationLoop());
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _running = false;
            _emulationTask?.Wait(1000);
        }

        private void EmulationLoop()
        {
            const int cyclesPerFrame = 70224; // ~4194304 Hz / ~59.7 FPS
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            long frameCount = 0;

            while (_running)
            {
                int cyclesThisFrame = 0;

                //Run CPU until we hit ~70224 cycles for one frame
                while (cyclesThisFrame < cyclesPerFrame)
                {
                    //Step the CPU for one instruction
                    int used = _cpu.Step();

                    //Update timers
                    _cpu.UpdateTimers(used);

                    cyclesThisFrame += used;
                }
            }
        }

        private void loadROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "GameBoy ROMs (*.gb,*.gpc)|*.gb;*.gbc|All files (*.*)|*.*";
                openFileDialog.Title = "Select a GameBoy ROM";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        //Read the file
                        var romData = File.ReadAllBytes(openFileDialog.FileName);

                        //Load into gameboy memory
                        _memory.LoadRom(romData);

                        //Reset CPU to starting values
                        _cpu.PC = 0x0100;
                        _cpu.SP = 0xFFFE;

                        Console.WriteLine("ROM loaded successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error Loading ROM: " + ex.Message);
                    }
                }
            }
        }
    }
}
