using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.MyAssets.Scripts.Network.Framework
{
    public delegate void CompletedMessageCallback(ArraySegment<byte> buffer);

    class CMessageResolver
    {
        // 메시지 사이즈.
        private int MessageSize;
        // 진행중인 버퍼.
        private byte[] MessageBuffer = new byte[1024];
        // 현재 진행중인 버퍼의 인덱스를 가리키는 변수.
        // 패킷 하나를 완성한 뒤에는 0으로 초기화 시켜줘야 한다.
        private int CurrentPosition;
        // 읽어와야 할 목표 위치.
        private int PositionToRead;
        // 남은 사이즈.
        private int RemainBytes;

        public CMessageResolver()
        {
            MessageSize = 0;
            CurrentPosition = 0;
            PositionToRead = 0;
            RemainBytes = 0;
        }

        /// <summary>
		/// 목표지점으로 설정된 위치까지의 바이트를 원본 버퍼로부터 복사한다.
		/// 데이터가 모자랄 경우 현재 남은 바이트 까지만 복사한다.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="size_to_read"></param>
		/// <returns>다 읽었으면 true, 데이터가 모자라서 못 읽었으면 false를 리턴한다.</returns>
		bool ReadUntil(byte[] buffer, ref int scrPosition)
        {
            // 읽어와야 할 바이트.
            // 데이터가 분리되어 올 경우 이전에 읽어놓은 값을 빼줘서 부족한 만큼 읽어올 수 있도록 계산해 준다.
            int copy_size = PositionToRead - CurrentPosition;

            // 앗! 남은 데이터가 더 적다면 가능한 만큼만 복사한다.
            if (RemainBytes < copy_size)
            {
                copy_size = RemainBytes;
            }

            // 버퍼에 복사.
            Array.Copy(buffer, scrPosition, MessageBuffer, CurrentPosition, copy_size);

            // 원본 버퍼 포지션 이동.
            scrPosition += copy_size;

            // 타겟 버퍼 포지션도 이동.
            CurrentPosition += copy_size;

            // 남은 바이트 수.
            RemainBytes -= copy_size;

            // 목표지점에 도달 못했으면 false
            if (CurrentPosition < PositionToRead)
            {
                return false;
            }

            return true;
        }

        /// <summary>
		/// 소켓 버퍼로부터 데이터를 수신할 때 마다 호출된다.
		/// 데이터가 남아 있을 때 까지 계속 패킷을 만들어 callback을 호출 해 준다.
		/// 하나의 패킷을 완성하지 못했다면 버퍼에 보관해 놓은 뒤 다음 수신을 기다린다.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="transffered"></param>
		public void OnReceive(byte[] buffer, int offset, int transffered, CompletedMessageCallback callback)
        {
            // 이번 receive로 읽어오게 될 바이트 수.
            RemainBytes = transffered;

            // 원본 버퍼의 포지션값.
            // 패킷이 여러개 뭉쳐 올 경우 원본 버퍼의 포지션은 계속 앞으로 가야 하는데 그 처리를 위한 변수이다.
            int srcPosition = offset;

            // 남은 데이터가 있다면 계속 반복한다.
            while (RemainBytes > 0)
            {
                bool completed = false;

                // 헤더만큼 못읽은 경우 헤더를 먼저 읽는다.
                if (CurrentPosition < NetworkFrameworkDefines.PACKET_HEADER_BYTE_SIZE)
                {
                    // 목표 지점 설정(헤더 위치까지 도달하도록 설정).
                    PositionToRead = NetworkFrameworkDefines.PACKET_HEADER_BYTE_SIZE;

                    completed = ReadUntil(buffer, ref srcPosition);
                    if (!completed)
                    {
                        // 아직 다 못읽었으므로 다음 receive를 기다린다.
                        return;
                    }

                    // 헤더 하나를 온전히 읽어왔으므로 메시지 사이즈를 구한다.
                    MessageSize = GetTotalMessageSize();

                    // 메시지 사이즈가 0이하라면 잘못된 패킷으로 처리한다.
                    // It was wrong message if size less than zero.
                    if (MessageSize <= 0)
                    {
                        ClearBuffer();
                        return;
                    }

                    // 다음 목표 지점.
                    PositionToRead = MessageSize;

                    // 헤더를 다 읽었는데 더이상 가져올 데이터가 없다면 다음 receive를 기다린다.
                    // (예를들어 데이터가 조각나서 헤더만 오고 메시지는 다음번에 올 경우)
                    if (RemainBytes <= 0)
                    {
                        return;
                    }
                }

                // 메시지를 읽는다.
                completed = ReadUntil(buffer, ref srcPosition);

                if (completed)
                {
                    // 패킷 하나를 완성 했다.
                    byte[] clone = new byte[PositionToRead];
                    Array.Copy(MessageBuffer, clone, PositionToRead);
                    ClearBuffer();
                    callback(new ArraySegment<byte>(clone, 0, PositionToRead));
                }
            }
        }

        /// <summary>
        /// 헤더+바디 사이즈를 구한다.
        /// 패킷 헤더부분에 이미 전체 메시지 사이즈가 계산되어 있으므로 헤더 크기에 맞게 변환만 시켜주면 된다.
        /// </summary>
        /// <returns></returns>
		int GetTotalMessageSize()
        {
            if (NetworkFrameworkDefines.PACKET_HEADER_BYTE_SIZE == 2)
            {
                return BitConverter.ToInt16(MessageBuffer, 0);
            }
            else if (NetworkFrameworkDefines.PACKET_HEADER_BYTE_SIZE == 4)
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
