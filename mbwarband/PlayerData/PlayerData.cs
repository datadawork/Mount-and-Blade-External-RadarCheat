using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessMemoryReaderLib;

namespace Warband
{
    public class PlayerData
    {
        private float health;
        public int id;
        private float[] vec = new float[3];
        private float[] vecRotation = new float[3];
        private bool active;
        private int address;
        
        protected static ProcessMemoryReader mem;
        protected static Dictionary<string, int> offsets;

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        private int rider;

        public int Rider
        {
            get { return rider; }
            set { rider = value; }
        }

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

        public float Health
        {
            get { return health; }
            set { health = value; }

        }

        public float[] Vec
        {
            get { return vec; }
            set { vec = value; }
        }

        public int Address
        {
            get { return address; }
            set { address = value; }
        }
        
        public PlayerData(int address)
        {
            this.health = mem.ReadFloat(address + offsets["health"]);
            this.rider = mem.ReadInt(address + offsets["player"]);  
            vec[0] = mem.ReadFloat(address + offsets["x"]);
            vec[1] = mem.ReadFloat(address + offsets["y"]);
            vec[2] = mem.ReadFloat(address + offsets["y"]);
            vecRotation[0] = mem.ReadFloat(address + offsets["xR"]);
            vecRotation[1] = mem.ReadFloat(address + offsets["yR"]);
            vecRotation[2] = mem.ReadFloat(address + offsets["zR"]);
            this.active = Convert.ToBoolean(mem.ReadByte(address + offsets["active"]));
            this.id = mem.ReadInt(address + offsets["id"]);
            this.address = address;  
        }

        public int specialcheck(int number)
        {
            return mem.ReadInt(address + number);
        }
    }
}
