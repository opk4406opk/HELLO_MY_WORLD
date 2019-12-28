using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KojeomNet.FrameWork.Soruces;

namespace HMWGameServer
{
    class MainEntry
    {
        static void Main(string[] args)
        {
            GameServerManager.GetInstance().StartListen();
            GameServerManager.GetInstance().StartServer();
        }
    }
}
