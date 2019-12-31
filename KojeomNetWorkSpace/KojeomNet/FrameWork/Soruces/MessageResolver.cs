using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    public class Defines
    {
        public static readonly short HEADER_SIZE = 4;
    }
    public class MessageResolver
    {
        public delegate void CompletedMessageCallback(ArraySegment<byte> buffer);
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
        
        bool ReadUntil(byte[] buffer, ref int srcPosition)
        {
            // 읽어와야 할 바이트.
            // 데이터가 분리되어 올 경우 이전에 읽어놓은 값을 빼줘서 부족한 만큼 읽어올 수 있도록 계산해 준다.
            int copySize = PositionToRead - CurrentPosition;

            // 앗! 남은 데이터가 더 적다면 가능한 만큼만 복사한다.
            if (RemainBytes < copySize)
            {
                copySize = RemainBytes;
            }

            // 버퍼에 복사.
            Array.Copy(buffer, srcPosition, MessageBuffer, CurrentPosition, copySize);

            // 원본 버퍼 포지션 이동.
            srcPosition += copySize;

            // 타겟 버퍼 포지션도 이동.
            CurrentPosition += copySize;

            // 남은 바이트 수.
            RemainBytes -= copySize;

            // 목표지점에 도달 못했으면 false
            if (CurrentPosition < PositionToRead)
            {
                return false;
            }

            return true;
        }
        public void OnReceive(byte[] buffer, int offset, int transffered, CompletedMessageCallback callback)
        {
            // 이번 receive로 읽어오게 될 바이트 수.
            RemainBytes = transffered;

            // 원본 버퍼의 포지션값.
            // 패킷이 여러개 뭉쳐 올 경우 원본 버퍼의 포지션은 계속 앞으로 가야 하는데 그 처리를 위한 변수이다.
            int src_position = offset;

            // 남은 데이터가 있다면 계속 반복한다.
            while (this.RemainBytes > 0)
            {
                bool completed = false;

                // 헤더만큼 못읽은 경우 헤더를 먼저 읽는다.
                if (this.CurrentPosition < Defines.HEADER_SIZE)
                {
                    // 목표 지점 설정(헤더 위치까지 도달하도록 설정).
                    this.PositionToRead = Defines.HEADER_SIZE;

                    completed = ReadUntil(buffer, ref src_position);
                    if (completed == false)
                    {
                        // 아직 다 못읽었으므로 다음 receive를 기다린다.
                        return;
                    }

                    // 헤더 하나를 온전히 읽어왔으므로 메시지 사이즈를 구한다.
                    this.MessageSize = GetTotalMessageSize();

                    // 메시지 사이즈가 0이하라면 잘못된 패킷으로 처리한다.
                    // It was wrong message if size less than zero.
                    if (this.MessageSize <= 0)
                    {
                        ClearBuffer();
                        return;
                    }

                    // 다음 목표 지점.
                    this.PositionToRead = this.MessageSize;

                    // 헤더를 다 읽었는데 더이상 가져올 데이터가 없다면 다음 receive를 기다린다.
                    // (예를들어 데이터가 조각나서 헤더만 오고 메시지는 다음번에 올 경우)
                    if (this.RemainBytes <= 0)
                    {
                        return;
                    }
                }

                // 메시지를 읽는다.
                completed = ReadUntil(buffer, ref src_position);

                if (completed == true)
                {
                    // 패킷 하나를 완성 했다.
                    byte[] clone = new byte[this.PositionToRead];
                    Array.Copy(this.MessageBuffer, clone, this.PositionToRead);
                    ClearBuffer();
                    callback(new ArraySegment<byte>(clone, 0, this.PositionToRead));
                }
            }
        }

        /// <summary>
        /// 헤더+바디 사이즈를 구한다.
        /// 패킷 헤더부분에 이미 전체 메시지 사이즈가 계산되어 있으므로 헤더 크기에 맞게 변환만 시켜주면 된다.
        /// </summary>
        /// <returns></returns>
        private int GetTotalMessageSize()
        {
            if (Defines.HEADER_SIZE == 2)
            {
                return BitConverter.ToInt16(MessageBuffer, 0);
            }
            else if (Defines.HEADER_SIZE == 4)
            {
                return BitConverter.ToInt32(MessageBuffer, 0);
            }

            return 0;
        }

        public void ClearBuffer()
        {
            Array.Clear(MessageBuffer, 0, MessageBuffer.Length);
            CurrentPosition = 0;
            MessageSize = 0;
        }
    }
}
