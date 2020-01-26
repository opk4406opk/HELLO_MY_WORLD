using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    public class CPacket
    {
        public UserToken Owner { get; private set; }
        public byte[] Buffer { get; private set; }
        public int Position { get; private set; }
        public int Size { get; private set; }

        public Int16 ProtocolID { get; private set; }

        public static CPacket Create(Int16 protocol_id)
        {
            CPacket packet = new CPacket();
            //todo:다음 리팩토링 대상은 바로 여기다. CPacketBufferManager!!!
            //CPacket packet = CPacketBufferManager.pop();
            packet.SetProtocol(protocol_id);
            return packet;
        }

        public static CPacket Create(Int16 protocol_id, int bufferSize)
        {
            CPacket packet = new CPacket(bufferSize);
            //todo:다음 리팩토링 대상은 바로 여기다. CPacketBufferManager!!!
            //CPacket packet = CPacketBufferManager.pop();
            packet.SetProtocol(protocol_id);
            return packet;
        }

        public static void Destroy(CPacket packet)
        {
            //CPacketBufferManager.push(packet);
        }

        public CPacket(ArraySegment<byte> buffer, UserToken owner)
        {
            // 참조로만 보관하여 작업한다.
            // 복사가 필요하면 별도로 구현해야 한다.
            Buffer = buffer.Array;

            // 헤더는 읽을필요 없으니 그 이후부터 시작한다.
            Position = Defines.HEADER_SIZE;
            Size = buffer.Count;

            // 프로토콜 아이디만 확인할 경우도 있으므로 미리 뽑아놓는다.
            ProtocolID = PopProtocolID();
            Position = Defines.HEADER_SIZE;

            Owner = owner;
        }

        public CPacket(byte[] buffer, UserToken owner)
        {
            // 참조로만 보관하여 작업한다.
            // 복사가 필요하면 별도로 구현해야 한다.
            this.Buffer = buffer;

            // 헤더는 읽을필요 없으니 그 이후부터 시작한다.
            this.Position = Defines.HEADER_SIZE;

            this.Owner = owner;
        }

        public CPacket()
        {
            this.Buffer = new byte[1024];
        }

        public CPacket(int bufferSize)
        {
            this.Buffer = new byte[bufferSize];
        }

        public Int16 PopProtocolID()
        {
            return PopInt16();
        }

        public void CopyTo(CPacket target)
        {
            target.SetProtocol(this.ProtocolID);
            target.Overwrite(this.Buffer, this.Position);
        }

        public void Overwrite(byte[] source, int position)
        {
            Array.Copy(source, this.Buffer, source.Length);
            this.Position = position;
        }

        public byte Popbyte()
        {
            byte data = this.Buffer[this.Position];
            this.Position += sizeof(byte);
            return data;
        }

        public Int16 PopInt16()
        {
            Int16 data = BitConverter.ToInt16(this.Buffer, this.Position);
            this.Position += sizeof(Int16);
            return data;
        }

        public Int32 PopInt32()
        {
            Int32 data = BitConverter.ToInt32(this.Buffer, this.Position);
            this.Position += sizeof(Int32);
            return data;
        }

        public Int64 PopInt64()
        {
            Int64 data = BitConverter.ToInt64(this.Buffer, this.Position);
            this.Position += sizeof(Int64);
            return data;
        }

        public string PopString()
        {
            // 문자열 길이는 최대 2바이트 까지. 0 ~ 32767
            Int16 len = BitConverter.ToInt16(this.Buffer, this.Position);
            this.Position += sizeof(Int16);

            // 인코딩은 utf8로 통일한다.
            string data = System.Text.Encoding.UTF8.GetString(this.Buffer, this.Position, len);
            this.Position += len;

            return data;
        }

        public float Popfloat()
        {
            float data = BitConverter.ToSingle(this.Buffer, this.Position);
            this.Position += sizeof(float);
            return data;
        }



        public void SetProtocol(Int16 protocol_id)
        {
            this.ProtocolID = protocol_id;
            //this.buffer = new byte[1024];

            // 헤더는 나중에 넣을것이므로 데이터 부터 넣을 수 있도록 위치를 점프시켜놓는다.
            this.Position = Defines.HEADER_SIZE;

            PushInt16(protocol_id);
        }

        public void RecordSize()
        {
            // header + body 를 합한 사이즈를 입력한다.
            byte[] header = BitConverter.GetBytes(this.Position);
            header.CopyTo(this.Buffer, 0);
        }

        public void PushInt16(Int16 data)
        {
            byte[] temp_buffer = BitConverter.GetBytes(data);
            temp_buffer.CopyTo(this.Buffer, this.Position);
            this.Position += temp_buffer.Length;
        }

        public void Push(byte data)
        {
            byte[] temp_buffer = BitConverter.GetBytes(data);
            temp_buffer.CopyTo(this.Buffer, this.Position);
            this.Position += sizeof(byte);
        }

        public void Push(Int16 data)
        {
            byte[] temp_buffer = BitConverter.GetBytes(data);
            temp_buffer.CopyTo(this.Buffer, this.Position);
            this.Position += temp_buffer.Length;
        }

        public void Push(Int32 data)
        {
            byte[] temp_buffer = BitConverter.GetBytes(data);
            temp_buffer.CopyTo(this.Buffer, this.Position);
            this.Position += temp_buffer.Length;
        }

        public void Push(Int64 data)
        {
            byte[] temp_buffer = BitConverter.GetBytes(data);
            temp_buffer.CopyTo(this.Buffer, this.Position);
            this.Position += temp_buffer.Length;
        }

        public void Push(string data)
        {
            byte[] temp_buffer = Encoding.UTF8.GetBytes(data);

            Int16 len = (Int16)temp_buffer.Length;
            byte[] len_buffer = BitConverter.GetBytes(len);
            len_buffer.CopyTo(this.Buffer, this.Position);
            this.Position += sizeof(Int16);

            temp_buffer.CopyTo(this.Buffer, this.Position);
            this.Position += temp_buffer.Length;
        }

        public void Push(float data)
        {
            byte[] temp_buffer = BitConverter.GetBytes(data);
            temp_buffer.CopyTo(this.Buffer, this.Position);
            this.Position += temp_buffer.Length;
        }
    }
}
