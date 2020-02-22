using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    public class ServerUserManager
    {
        object CSUser;
        List<UserToken> Users;

        Timer TimerHeartbeat;
        long HeartbeatDuration;


        public ServerUserManager()
        {
            CSUser = new object();
            Users = new List<UserToken>();
        }


        public void StartHeartbeatChecking(uint checkIntervalSec, uint allowDurationSec)
        {
            HeartbeatDuration = allowDurationSec * 10000000;
            TimerHeartbeat = new Timer(CheckHeartbeat, null, 1000 * checkIntervalSec, 1000 * checkIntervalSec);
        }


        public void StopHeartbeatChecking()
        {
            TimerHeartbeat.Dispose();
        }


        public void Add(UserToken user)
        {
            lock (CSUser)
            {
                Users.Add(user);
            }
        }


        public void Remove(UserToken user)
        {
            lock (CSUser)
            {
                Users.Remove(user);
            }
        }


        public bool IsExist(UserToken user)
        {
            lock (CSUser)
            {
                return Users.Exists(obj => obj == user);
            }
        }


        public int GetTotalCount()
        {
            return Users.Count;
        }

        void CheckHeartbeat(object state)
        {
            long allowed_time = DateTime.Now.Ticks - HeartbeatDuration;

            lock (CSUser)
            {
                for (int i = 0; i < Users.Count; ++i)
                {
                    long heartbeat_time = Users[i].LatestHeartbeatTime;
                    if (heartbeat_time >= allowed_time)
                    {
                        continue;
                    }

                    Users[i].Disconnect();
                }
            }
        }
    }
}
