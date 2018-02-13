using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 서버목록에 보여지는 항목 정보를 전달할 때 쓰이는 구조체.
/// </summary>
public struct ServerListElementData
{
    public string serverIP;
    public string roomName;
    public int roomIdx;
    public int curUsers;
    public int maxUsers;
}
public class ServerListElementInfo : MonoBehaviour {

    [SerializeField]
    private UILabel lbl_serverIP;
    [SerializeField]
    private UILabel lbl_roomName;
    [SerializeField]
    private UILabel lbl_roomIdx;
    [SerializeField]
    private UILabel lbl_curUsers;
    [SerializeField]
    private UILabel lbl_maxUsers;

    private string serverIP;
    private string roomName;
    private int roomIdx;
    private int curUsers;
    private int maxUsers;

    public void SetData(ServerListElementData info)
    {
        serverIP = info.serverIP;
        roomName = info.roomName;
        roomIdx = info.roomIdx;
        curUsers = info.curUsers;
        maxUsers = info.maxUsers;

        lbl_serverIP.text = serverIP;
        lbl_roomName.text = roomName;
        lbl_roomIdx.text = roomIdx.ToString();
        lbl_curUsers.text = curUsers.ToString();
        lbl_maxUsers.text = maxUsers.ToString();
    }
}
