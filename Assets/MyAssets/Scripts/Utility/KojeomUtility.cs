using System.Collections.Generic;
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
        return parent.GetComponentsInChildren<T>(true);
    }

    public static Stack<T> ReverseStack<T>(Stack<T> src)
    {
        Stack<T> v = new Stack<T>();
        if (src != null)
        {
            foreach (T p in src)
            {
                v.Push(p);
            }
        }
        return v;
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
    /// <param name="min">inclusive</param>
    /// <param name="max">inclusive</param>
    /// <returns></returns>
    public static float RandomFloat(float min, float max)
    {
        return Random.Range(min, max);
    }
}
