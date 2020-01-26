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

        AFTER_SESSION_INIT_REQ,
        AFTER_SESSION_INIT_ACK,

        WORLD_MAP_PROPERTIES_REQ,
        WORLD_MAP_PROPERTIES_ACK,

        CHANGE_SUBWORLD_BLOCK_PUSH,

        SUBWORLD_DATAS_REQ,
        SUBWORLD_DATAS_ACK,

        END
    }
}
