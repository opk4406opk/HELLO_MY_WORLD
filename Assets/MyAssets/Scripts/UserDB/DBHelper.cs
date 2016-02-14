using UnityEngine;
using System.Collections;

/// <summary>
/// DB에서 유저 아이템정보를 담을 구조체.
/// </summary>
public struct USER_ITEM
{
    public string name;
    public int type;
    public int amount;
}

public class DBHelper : MonoBehaviour
{
}
