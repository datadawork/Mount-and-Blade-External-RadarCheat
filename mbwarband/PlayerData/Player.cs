using System;

namespace Warband
{
    class Player : PlayerData
    {
        public bool team;
       
        public bool Team
        {
            get { return team; }
            set { team = value; }
        }
        
          public Player(int address) : base(address)
        {
            team = Convert.ToBoolean(mem.ReadByte(address + offsets["team"]));
        }
        
    }
}
