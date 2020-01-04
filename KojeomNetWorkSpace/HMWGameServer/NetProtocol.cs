using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer
{
    public enum NetProtocol
    {
        BEGIN = 0,

        CHANGED_SUBWORLD_BLOCK_REQ,
        CHANGED_SUBWORLD_BLOCK_ACK,

        INIT_RANDOM_SEED_REQ, // only host
        INIT_RANDOM_SEED_ACK, // only host

        USER_NET_TYPE_REQ,
        USER_NET_TYPE_ACK,
        
        CHANGED_WORLD_HISTORY_REQ,
        CHANGED_WORLD_HISTORY_ACK, 


        END
    }
}
