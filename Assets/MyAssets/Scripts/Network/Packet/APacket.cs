using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum PacketType
{
    LoginPacket,
    PingPacket,
    GamePacket     
}

public abstract class APacket
{
    public abstract byte[] ToBytes();
}

public class LoginPacket : APacket
{
    public LoginPacket(string id, string pw)
    {
        var idBuf = Encoding.UTF8.GetBytes(id);
        System.Buffer.BlockCopy(idBuf, 0, ID, 0, idBuf.Length);
        var pwBuf = Encoding.UTF8.GetBytes(pw);
        System.Buffer.BlockCopy(pwBuf, 0, PW, 0, pwBuf.Length);
    }
    public byte[] ID = new byte[20];
    public byte[] PW = new byte[20];

    public override byte[] ToBytes()
    {
        byte[] ret = new byte[ID.Length + PW.Length];
        System.Buffer.BlockCopy(ID, 0, ret, 0, ID.Length);
        System.Buffer.BlockCopy(PW, 0, ret, ID.Length, PW.Length);
        return ret;
    }
}

public class PingPacket : APacket
{
    public byte[] Alive = new byte[1] {byte.Parse("P")};
    public override byte[] ToBytes()
    {
        return Alive;
    }
}