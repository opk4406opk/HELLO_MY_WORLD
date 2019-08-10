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

public abstract class AGamePacket
{
}
