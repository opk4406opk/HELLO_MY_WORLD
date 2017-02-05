using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ActorStatData
{
    public float hp;
    public float mp;
    public float ap;
    public float dp;
}


abstract public class Actor : MonoBehaviour
{
    abstract public void Init(ActorStatData statData, PathFinderInitData pathData, Vector3 pos, World world);
}
