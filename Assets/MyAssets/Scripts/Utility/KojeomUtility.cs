using System;
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

    // ref : https://stackoverflow.com/questions/16100/how-should-i-convert-a-string-to-an-enum-in-c
    // ref : https://docs.microsoft.com/en-us/dotnet/api/system.enum.tryparse?redirectedfrom=MSDN&view=netframework-4.7.2#overloads
    public static T StringToEnum<T>(string value, bool ignoreCase = true)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    public static string EnumToString<T>(T value)
    {
        return value.ToString();
    }

    public static T GetActorDetailTypeFromAssetPath<T>(string assetPath)
    {
        string prefabName = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
        var splits = prefabName.Split('.');
        var tokens = splits[0].Split('_');
        return StringToEnum<T>(tokens[1]);
    }

    public static T GetResourceIDFromAssetPath<T>(string assetPath)
    {
        string prefabName = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
        var splits = prefabName.Split('.');
        var tokens = splits[0].Split('_');
        return StringToEnum<T>(tokens[2]);
    }

    public static string GetActorNameFromAssetPath(string assetPath)
    {
        string prefabName = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
        var splits = prefabName.Split('.');
        var tokens = splits[0].Split('_');
        return tokens[2];
    }


    private static System.Random RandomInstance = new System.Random(0);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="min"> inclusive </param>
    /// <param name="max"> exclusive </param>
    /// <returns></returns>
    public static int RandomInteger(int min, int max)
    {
        return RandomInstance.Next(min, max);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="min">inclusive</param>
    /// <param name="max">inclusive</param>
    /// <returns></returns>
    public static float RandomFloat(float min, float max)
    {
        //return RandomInstance.Next(min, max);
        return 1.0f;
    }
}
