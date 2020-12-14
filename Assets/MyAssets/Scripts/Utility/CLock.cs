using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CustomThreadLock
{
    // Spin lock policy ( 5000번 이후, yield 방식으로 전환 )
    class CLock
    {
        private const int EMPTY_FLAG = 0x00000000;
        private const int WRITE_MASK = 0x7FFF0000;
        private const int READ_MASK = 0x0000FFFF;
        private const int MAX_SPINT_COUNT = 5000;

        // 첫번째 비트는 Unused,  15비트는 WriteThreadID,  나머지 16비트는 ReadCount
        private int _flag = EMPTY_FLAG;
        private int _writeCount = 0;

        public void WriteLock()
        {
            int lockThreadID = (_flag & WRITE_MASK) >> 16;
            if(Thread.CurrentThread.ManagedThreadId == lockThreadID)
            {
                _writeCount++;
                return;
            }

            // Nobody has Writelock or Readlock, and then compete to get acquire.
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;
            while(true)
            {
                for(int idx = 0; idx < MAX_SPINT_COUNT; idx++)
                {
                    if(Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                }
                Thread.Yield();
            }
            
        }

        public void WirteUnlock()
        {
            int lockCount = --_writeCount;
            if(lockCount == 0) Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            int lockTheadId = (_flag & WRITE_MASK) >> 16;
            if(Thread.CurrentThread.ManagedThreadId == lockTheadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            // if Nobody has writelock, increase read count.
            while(true)
            {
                for(int idx = 0; idx < MAX_SPINT_COUNT; idx++)
                {
                    int expected = _flag & READ_MASK;
                    if(Interlocked.CompareExchange(ref _flag, expected +1, expected) == expected)
                    {
                        return;
                    }
                }
                Thread.Yield();
            }
        }

        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }

    }
}
