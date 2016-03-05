using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warband
{
    class Horse : PlayerData
    {
        public Player player;
        

        public Horse(int address) : base(address)
        {
           
        }

        public void SetPlayerOnHorse(List<PlayerData> player)
        {
            this.player = null;

            for (int i = 0; i < player.Count; i++)
            {
                if (player[i] is Player)
                {
                    if (this.Rider == player[i].id)
                    {
                        this.player = (Player)player[i];
                       
                    }
                }
            }
        }
    }
}
