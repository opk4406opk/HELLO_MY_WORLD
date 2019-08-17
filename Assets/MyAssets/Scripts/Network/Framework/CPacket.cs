using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.MyAssets.Scripts.Network.Framework
{
    class CPacket
    {
        public byte[] Buffer { get; private set; }
        public int Length { get; private set; }
        public int Position { get; private set; }
        public Int16 ProtocolID { get; private set; }

        public CPacket()
        {
            Buffer = new byte[NetworkFrameworkDefines.MAX_BUFFER_BYTE_SIZE];
            Length = NetworkFrameworkDefines.MAX_BUFFER_BYTE_SIZE;
            // 헤더는 건너뛰고.
            Position = NetworkFrameworkDefines.PACKET_HEADER_BYTE_SIZE;
        }

        public static CPacket Create(Int16 protocolID)
        {
            CPacket newPacket = new CPacket();
            newPacket.SetProtocol(protocolID);
            return newPacket;
        }

        public byte PopByte()
        {
            byte data = Buffer[Position];
            Position += sizeof(byte);
            return data;
        }

        public Int16 PopInt16()
        {
            Int16 data = BitConverter.ToInt16(Buffer, Position);
            Position += sizeof(Int16);
            return data;
        }

        public Int32 PopInt32()
        {
            Int32 data = BitConverter.ToInt32(Buffer, Position);
            Position += sizeof(Int16);
            return data;
        }

        public float PopFloat()
        {
            float data = BitConverter.ToSingle(Buffer, Position);
            Position += sizeof(float);
            return data;
        }

        public Int16 PopProtocolID()
        {
            return PopInt16();
        }

        public void PushByte(byte data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            newData.CopyTo(Buffer, Position);
            Position += sizeof(byte);
        }
        public void PushInt16(Int16 data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            newData.CopyTo(Buffer, Position);
            Position += newData.Length;
        }
        public void PushInt32(Int32 data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            newData.CopyTo(Buffer, Position);
            Position += newData.Length;
        }

        public void PushFloat(float data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            newData.CopyTo(Buffer, Position);
            Position += newData.Length;
        }

        public void SetProtocol(Int16 protocolID)
        {
            ProtocolID = protocolID;
            // 헤더는 나중에 넣을것이므로 데이터 부터 넣을 수 있도록 위치를 점프시켜놓는다.
            Position = NetworkFrameworkDefines.PACKET_HEADER_BYTE_SIZE;
            PushInt16(protocolID);
        }

        public void RecordSize()
        {
            // header + body 를 합한 사이즈를 입력한다.
            byte[] header = BitConverter.GetBytes(Position);
            header.CopyTo(Buffer, 0);
        }
    }
}
