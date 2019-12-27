using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    public class LogicMessageEntry : IMessageDispatcher
    {
        NetworkServiceManager ServiceManager;
        ILogicQueue MessageQueue;
        AutoResetEvent LogicEvent;


        public LogicMessageEntry(NetworkServiceManager service)
        {
            ServiceManager = service;
            MessageQueue = new DoubleBufferingQueue();
            LogicEvent = new AutoResetEvent(false);
        }


        /// <summary>
        /// 로직 스레드 시작.
        /// </summary>
        public void Start()
        {
            Thread logic = new Thread(DoLogic);
            logic.Start();
        }


        void IMessageDispatcher.OnMessage(UserToken user, ArraySegment<byte> buffer)
        {
            // 여긴 IO스레드에서 호출된다.
            // 완성된 패킷을 메시지큐에 넣어준다.
            CPacket msg = new CPacket(buffer, user);
            MessageQueue.Enqueue(msg);

            // 로직 스레드를 깨워 일을 시킨다.
            LogicEvent.Set();
        }


        /// <summary>
        /// 로직 스레드.
        /// </summary>
        void DoLogic()
        {
            while (true)
            {
                // 패킷이 들어오면 알아서 깨워 주겠지.
                LogicEvent.WaitOne();

                // 메시지를 분배한다.
                DispatchAll(MessageQueue.GetAll());
            }
        }


        void DispatchAll(Queue<CPacket> queue)
        {
            while (queue.Count > 0)
            {
                CPacket msg = queue.Dequeue();
                if (ServiceManager.ServerUserManagerInstance.IsExist(msg.Owner) == false)
                {
                    continue;
                }

                msg.Owner.OnMessage(msg);
            }
        }
    }
}
