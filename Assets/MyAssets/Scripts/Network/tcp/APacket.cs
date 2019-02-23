using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PacketType
{
    LoginPacket,
    GamePacket        
}

public abstract class APacket
{

}

public class LoginPacket : APacket
{
    public byte[] ID = new byte[20];
    public byte[] PW = new byte[20];
}