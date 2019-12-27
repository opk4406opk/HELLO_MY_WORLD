using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    class HeartbeatSender
    {
        private UserToken ServerToken;
        private Timer TimerHeartbeat;
        private uint Interval;

        private float ElapsedTime;


        public HeartbeatSender(UserToken server, uint interval)
        {
            ServerToken = server;
            Interval = interval;
            TimerHeartbeat = new Timer(this.OnTimer, null, Timeout.Infinite, this.Interval * 1000);
        }


        void OnTimer(object state)
        {
            Send();
        }


        void Send()
        {
            CPacket msg = CPacket.Create((short)UserToken.SYS_UPDATE_HEARTBEAT);
            ServerToken.Send(msg);
        }


        public void Update(float time)
        {
            ElapsedTime += time;
            if (ElapsedTime < Interval)
            {
                return;
            }

            ElapsedTime = 0.0f;
            Send();
        }


        public void Stop()
        {
            ElapsedTime = 0;
            TimerHeartbeat.Change(Timeout.Infinite, Timeout.Infinite);
        }


        public void Play()
        {
            ElapsedTime = 0;
            TimerHeartbeat.Change(0, this.Interval * 1000);
        }
    }
}
