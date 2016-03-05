using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessMemoryReaderLib;
using System.Diagnostics;

namespace Warband
{
    public static class MainPlayer
    {
        public static bool team;
        public static int address;
        public static float x;
        public static float y;
        public static float xR;
        public static float yR;
        public static ProcessMemoryReader mem;

        public static void CheckPlayer()
        {
            int address = mem.ReadMultiLevelPointer(0x03137044, 4, new int[] { 0 });
           
            if (address != MainPlayer.address && address > 500 && 51704380 != address)
            {
                Debug.WriteLine(MainPlayer.address + " " + address);
                MainPlayer.address = address;

            }

            team = Convert.ToBoolean(mem.ReadByte(MainPlayer.address + 0x7b4));
            x = mem.ReadFloat(MainPlayer.address + 0x40);
            y = mem.ReadFloat(MainPlayer.address + 0x44);
            xR = mem.ReadFloat(MainPlayer.address + 0x10);
            yR = mem.ReadFloat(MainPlayer.address + 0x14);
        }
    }
}
