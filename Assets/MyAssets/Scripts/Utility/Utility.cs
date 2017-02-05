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
}
