using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    class SocketAsyncEventArgsPool
    {
        private Stack<SocketAsyncEventArgs> PoolInstance;
        public SocketAsyncEventArgsPool(int poolCapacity)
        {
            PoolInstance = new Stack<SocketAsyncEventArgs>(poolCapacity);
        }

        public SocketAsyncEventArgs Pop()
        {
            return PoolInstance.Pop();
        }

        public void Push(SocketAsyncEventArgs eventArgs)
        {
            PoolInstance.Push(eventArgs);
        }
    }
}
