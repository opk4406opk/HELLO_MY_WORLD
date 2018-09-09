﻿using System.Collections.Generic;
using UnityEngine;

public class KojeomUtility
{
    //https://docs.microsoft.com/ko-kr/dotnet/csharp/programming-guide/generics/generic-methods
    /// <summary>
    /// T타입의 모든 자식 컴포넌트들을 리턴합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static T[] GetChilds<T>(GameObject parent)
    {
        return parent.GetComponentsInChildren<T>();
    }

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

    public static void SetRandomSeed(int seed)
    {
        Random.InitState(seed);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="min"> inclusive </param>
    /// <param name="max"> exclusive </param>
    /// <returns></returns>
    public static int RandomInteger(int min, int max)
    {
        return Random.Range(min, max);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float RandomFloat(float min, float max)
    {
        return Random.Range(min, max);
    }
}