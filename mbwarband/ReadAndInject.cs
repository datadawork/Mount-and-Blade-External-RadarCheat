using Binarysharp.MemoryManagement;
using ProcessMemoryReaderLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Warband
{
    public static class ReadAndInject
    {
        private static ProcessMemoryReader mem = new ProcessMemoryReader();
        private static List<int> enemyAddresses = new List<int>();
        private static int pointer = Convert.ToInt32(Convert.ToDecimal(0x009326D8));
        private static String[] tabelNames = { "active", "id", "xR", "yR", "zR", "x", "y", "z", "player", "team", "health" };
        private static int[] tabelOffsets = { 0x4, 0x8, 0x10, 0x14, 0x18, 0x40, 0x44, 0x48, 0x11c, 0x7b4, 0x6000 };
        private static Dictionary<string, int> offsets = new Dictionary<string, int>();

        public static ProcessMemoryReader Mem
        {
            get { return mem; }
            set { mem = value; }
        }

        public static Dictionary<string, int> Offsets
        {
            get { return offsets; }
            set { offsets = value; }
        }
        
        public static void AssemblyInject()
        {
            Process[] myProcess;
            ProcessModule mainModule;

            try
            {
                myProcess = Process.GetProcessesByName("mb_warband");
                mainModule = myProcess[0].MainModule;
                mem.ReadProcess = myProcess[0];
                mem.OpenProcess();
                MainPlayer.mem = mem;
                
            }
            catch (Exception e)
            {
                MessageBox.Show("Start up mount and blade you dummy!" + e.Message);
                mem.CloseHandle();
                Environment.Exit(0);
            }

            for (int i = 0; i < tabelNames.Length; i++)
            {
                offsets.Add(tabelNames[i], tabelOffsets[i]);
            }

            
            decimal number = Convert.ToDecimal(0x02C85AE4);
            decimal sum = Convert.ToDecimal(0x14124);
            IntPtr address = (IntPtr)(0x49E45A);
            IntPtr address2 = (IntPtr)(0x6082F2);

            try
            {
                mem.WriteInt((int)sum, 0);
                MemorySharp sharp = new MemorySharp(Process.GetProcessesByName("mb_warband")[0]);

                #region ----ASM----
                sharp.Assembly.Inject(new[] { "JMP " + Convert.ToString(address2), }, address);

                sharp.Assembly.Inject(
                    new[]{
                    "mov ["+number+"],esi",
                    "PUSHFD",
                    "PUSHAD",
                    "PUSH EAX",
                    "mov EAX,["+sum+"]",
                    "CMP EAX,3200",
                    "JE "+Convert.ToDecimal(0x608323),
                    "add EAX,16",
                    "mov EDX,["+number+"]",
                    "mov ["+pointer+"+EAX],EDX",
                    "mov ["+sum+"],EAX",
                    "POP EAX",
                    "POPAD",
                    "POPFD",
                    "JMP "+Convert.ToString(address+0x06),
                    "mov ebp,0",
                    "mov ["+sum+"],ebp",
                    "mov eax,["+sum+"]",
                    "JMP "+Convert.ToDecimal(0x608307),
                },
                        address2);
                #endregion
            }
            catch (Exception e) 
            {
                mem.CloseHandle();
                MessageBox.Show("Memory injection failed exiting programming please consult the programer, yes me Frank. Also I am going to print out the error message not like I am going to understand it");
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }

        }

        public static List<int> ReadAddress()
        {
           
            for (int i = 0; i < 100; i++)
            {
                int number = Mem.ReadInt(pointer + (i * 32));
                if (number != 0)
                {
                    enemyAddresses.Add(number);
                }
            }

            return enemyAddresses.Distinct().ToList();
        }
    }
}
