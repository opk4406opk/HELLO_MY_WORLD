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

        CHANGE_SUBWORLD_BLOCK_PUSH,

        SUBWORLD_DATAS_REQ, // 클라에서 서버로 월드맵(모든 서브월드) 요청.
        SUBWORLD_DATAS_ACK, // 서버에서 클라로 서브월드 데이터 전달.

        SUBWORLD_DATAS_SAFE_RECEIVED, // 클라이언트에서 제대로 수신했을경우 보낸다.
        SUBWORLD_DATAS_FINISH, // 서버에서 클라가 모든 서브월드 데이터 패킷을 수신했다면 종료 패킷으로 보낸다.

        END
    }
}
