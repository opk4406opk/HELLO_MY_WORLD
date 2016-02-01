using UnityEngine;
using System.Collections;

public class GameWorldConfig
{
    private static int _worldX = 256;
    public static int worldX
    {
        get { return _worldX; }
    }
    private static int _worldY = 32;
    public static int worldY
    {
        get { return _worldY; }
    }
    private static int _worldZ = 256;
    public static int worldZ
    {
        get { return _worldZ; }
    }
    private static int _chunkSize = 16;
    public static int chunkSize
    {
        get { return _chunkSize; }
    }
}

public class GameManager : MonoBehaviour {

    [SerializeField]
    private World worldA;
    [SerializeField]
    private World worldB;
    // Use this for initialization
    void Start ()
    {
        int offset = GameWorldConfig.worldX / GameWorldConfig.chunkSize;
        worldA.Init(0, 0);
        worldB.Init(offset, 0);
    }
	
}
