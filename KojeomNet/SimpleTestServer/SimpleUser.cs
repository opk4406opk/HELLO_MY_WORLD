using KojeomNet.FrameWork.Soruces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTestServer
{
    class SimpleUser
    {
        private UserToken UserTokenInstance;
        public SimpleUser(UserToken userToken)
        {
            UserTokenInstance = userToken;
        }
    }
}
