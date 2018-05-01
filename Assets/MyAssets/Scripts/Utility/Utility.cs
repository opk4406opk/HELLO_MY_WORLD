using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour {

    public static Stack<PathNode> ReversePathStack(Stack<PathNode> src)
    {
        Stack<PathNode> v = new Stack<PathNode>();
        if (src != null)
        {
            foreach (PathNode p in src)
            {
                v.Push(p);
            }
        }
        return v;
    }

    public static float RandomFloat(float min, float max)
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        return Random.Range(min, max);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="min"> inclusive </param>
    /// <param name="max"> exclusive </param>
    /// <returns></returns>
    public static int RandomInteger(int min, int max)
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        return Random.Range(min, max);
    }
}
