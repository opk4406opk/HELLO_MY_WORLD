using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    public class SocketAsyncEventArgsPool
    {
        private Stack<SocketAsyncEventArgs> PoolInstance;
        public SocketAsyncEventArgsPool(int poolCapacity)
        {
            PoolInstance = new Stack<SocketAsyncEventArgs>(poolCapacity);
        }

        public SocketAsyncEventArgs Pop()
        {
            lock(PoolInstance)
            {
                return PoolInstance.Pop();
            }
        }

        public void Push(SocketAsyncEventArgs eventArgs)
        {
            if(eventArgs == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }

            lock (PoolInstance)
            {
                if(PoolInstance.Contains(eventArgs) == false) PoolInstance.Push(eventArgs);
                else throw new Exception("Already exist item.");
            }
        }

        public int Count()
        {
            return PoolInstance.Count;
        }
    }
}
