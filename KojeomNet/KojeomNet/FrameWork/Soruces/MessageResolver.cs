using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    public class Defines
    {
        public static readonly short HEADER_SIZE = 2;
    }
    public class MessageResolver
    {
        public delegate void CompletedMessageCallback(Utils.Const<byte[]> buffer);
        int MessageSize = 0;
        byte[] MessageBuffer = new byte[1024];
        int CurrentPosition = 0;
        int PositionToRead = 0;
        int RemainBytes = 0;
        public MessageResolver()
        {
            MessageSize = 0;
            CurrentPosition = 0;
            PositionToRead = 0;
            RemainBytes = 0;
        }
        
        bool ReadUntil(byte[] buffer, ref int srcPosition, int offset, int transffered)
        {
            if(CurrentPosition >= offset + transffered)
            {
                return false;
            }

            int copySize = PositionToRead - CurrentPosition;
            if(RemainBytes < copySize)
            {
                copySize = RemainBytes;
            }

            Array.Copy(buffer, srcPosition, MessageBuffer, CurrentPosition, copySize);
            srcPosition += copySize;
            CurrentPosition = copySize;
            RemainBytes -= copySize;
            if(CurrentPosition < PositionToRead)
            {
                return false;
            }
            return true;
        }
        public void OnReceive(byte[] buffer, int offset, int transffered, CompletedMessageCallback callback)
        {
            RemainBytes = transffered;
            int srcPosition = offset;

            while(RemainBytes >0)
            {
                bool completed = false;
                if(CurrentPosition < Defines.HEADER_SIZE)
                {
                    PositionToRead = Defines.HEADER_SIZE;
                    completed = ReadUntil(buffer, ref srcPosition, offset, transffered);
                    if(completed == false)
                    {
                        return;
                    }
                    MessageSize = GetBodySize();
                    PositionToRead = MessageSize + Defines.HEADER_SIZE;
                }
                completed = ReadUntil(buffer, ref srcPosition, offset, transffered);
                if(completed == true)
                {
                    callback(new Utils.Const<byte[]>(MessageBuffer));
                    ClearBuffer();
                }
            }
        }

        private int GetBodySize()
        {
            Type type = Defines.HEADER_SIZE.GetType();
            if(type.Equals(typeof(Int16)))
            {
                return BitConverter.ToInt16(MessageBuffer, 0);
            }
            return BitConverter.ToInt32(MessageBuffer, 0);
        }

        private void ClearBuffer()
        {
            Array.Clear(MessageBuffer, 0, MessageBuffer.Length);
            CurrentPosition = 0;
            MessageSize = 0;
        }
    }
}
