using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class KojeomUtility
{
    //variables.
    private static readonly int SeedValue = 0;

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

    public static string GetResourceIDFromAssetPath(string assetPath)
    {
        string prefabName = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
        var splits = prefabName.Split('.');
        var tokens = splits[0].Split('_');
        var resoruceString = Regex.Match(tokens[2].Substring(3), "[a-zA-Z0-9]+").ToString();
        return resoruceString;
    }

    public static bool IsValidActorResourceGroup(ActorResourceGroup resourceGroup)
    {
        if (resourceGroup == null) return false;
        if (resourceGroup.Resoruces == null) return false;
        if (resourceGroup.Resoruces.Count == 0) return false;
        return true;
    }

    public static Block[,,] CopyWorldBlockData(Block[,,] original)
    {
        int lengthX = WorldConfigFile.Instance.GetConfig().SubWorldSizeX;
        int lengthY = WorldConfigFile.Instance.GetConfig().SubWorldSizeY;
        int lengthZ = WorldConfigFile.Instance.GetConfig().SubWorldSizeZ;
        Block[,,] copiedBlockData = new Block[lengthX, lengthY, lengthZ];
        for (int x = 0; x < lengthX; x++)
        {
            for (int y = 0; y < lengthY; y++)
            {
                for (int z = 0; z < lengthZ; z++)
                {
                    copiedBlockData[x, y, z] = original[x, y, z];
                }
            }
        }
        return copiedBlockData;
    }


    public static string ConvertAssetPathToResourcePath(string assetPath)
    {
        // ex)
        //"Assets/MyAssets/Resources/GamePrefabs/Actor/NPC/NPC_GUARD_N1_Utc.prefab"
        assetPath = assetPath.Replace("Assets/MyAssets/Resources/", "");
        var splits = assetPath.Split('.');
        return splits[0];
    }

    public static string GetActorNameFromAssetPath(string assetPath)
    {
        string prefabName = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
        var splits = prefabName.Split('.');
        var tokens = splits[0].Split('_');
        return tokens[2];
    }

    public static bool RandomBool()
    {
        // min( inclusive ) max (exclusive)
        int randValue = RandomInstance.Next(0, 2);
        if (randValue == 0) return false;
        return true;
    }
    /// <summary>
    /// 2차원 벡터 외적.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float CrossVector2D(Vector2 a, Vector2 b)
    {
        return (a.x * b.y - a.y * b.x);
    }

    // Seed 값은 일정하게 0으로 고정.
    private static System.Random RandomInstance = new System.Random(SeedValue);

    public static void ChangeSeed()
    {
        RandomInstance = new System.Random(DateTime.Now.Millisecond);
    }
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

    public static void InverseNormal(GameObject Target)
    {
        MeshFilter filter = Target.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (filter != null)
        {
            Mesh mesh = filter.mesh;

            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }
    }

    public static INPUT_DEVICE_TYPE GetInputDeviceType()
    {
        var currentPlatform = Application.platform;
        if (currentPlatform == RuntimePlatform.Android || currentPlatform == RuntimePlatform.IPhonePlayer)
        {
            return INPUT_DEVICE_TYPE.MOBILE;
        }
        else if (currentPlatform == RuntimePlatform.WindowsEditor || currentPlatform == RuntimePlatform.WindowsPlayer)
        {
            return INPUT_DEVICE_TYPE.WINDOW;
        }

        //
        return INPUT_DEVICE_TYPE.NONE;
    }
}
