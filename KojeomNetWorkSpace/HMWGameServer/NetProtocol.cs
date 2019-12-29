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

        CHANGED_SUBWORLD_BLOCK_REQ = 1,
        CHANGED_SUBWORLD_BLOCK_ACK = 2,

        INIT_RANDOM_SEED_REQ = 3, // only host
        INIT_RANDOM_SEED_ACK = 4, // only host

        END
    }
}
