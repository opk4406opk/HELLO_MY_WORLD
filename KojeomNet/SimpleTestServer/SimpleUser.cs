using KojeomNet.FrameWork.Soruces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTestServer
{
    class SimpleUser : IPeer
    {
        private UserToken UserTokenInstance;
        public SimpleUser(UserToken userToken)
        {
            UserTokenInstance = userToken;
        }

        public void Disconnect()
        {
        }

        public void OnMessage(CPacket msg)
        {
            
        }

        public void OnRemoved()
        {
            TestServerMain.OnSessionRemoved(this);
        }

        public void Send(CPacket msg)
        {
            UserTokenInstance.Send(msg);
        }
    }
}
