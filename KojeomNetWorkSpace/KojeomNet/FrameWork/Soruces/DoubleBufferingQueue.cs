using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    class DoubleBufferingQueue : ILogicQueue
    {
        // 실제 데이터가 들어갈 큐.
        Queue<CPacket> Queue1;
        Queue<CPacket> Queue2;

        // 각각의 큐에 대한 참조.
        Queue<CPacket> RefInput;
        Queue<CPacket> RefOutput;

        object CSWrite;


        public DoubleBufferingQueue()
        {
            // 초기 세팅은 큐와 참조가 1:1로 매칭되게 설정한다.
            // ref_input - queue1
            // ref)output - queue2
            this.Queue1 = new Queue<CPacket>();
            this.Queue2 = new Queue<CPacket>();
            this.RefInput = this.Queue1;
            this.RefOutput = this.Queue2;

            this.CSWrite = new object();
        }


        /// <summary>
        /// IO스레드에서 전달한 패킷을 보관한다.
        /// </summary>
        /// <param name="msg"></param>
        void ILogicQueue.Enqueue(CPacket msg)
        {
            lock (this.CSWrite)
            {
                this.RefInput.Enqueue(msg);
            }
        }


        Queue<CPacket> ILogicQueue.GetAll()
        {
            Swap();
            return this.RefOutput;
        }


        /// <summary>
        /// 입력큐와 출력큐를 뒤바꾼다.
        /// </summary>
        void Swap()
        {
            lock (this.CSWrite)
            {
                Queue<CPacket> temp = this.RefInput;
                this.RefInput = this.RefOutput;
                this.RefOutput = temp;
            }
        }
    }
}
