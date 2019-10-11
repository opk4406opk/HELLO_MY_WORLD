using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PathNode3D
{
    // 길찾기용 맵 데이터 좌표 X,Z 이다. 그러나 실제 월드 블록 배열 X,Z 평면값과 동일한 의미로 쓰이기도 하니,
    // 이 값을 실제 길을 찾아가는 오브젝트가 월드 좌표 (x, z)값으로 사용 가능하다.
    #region variables.

    public int PathMapDataX { set; get; }
    public int PathMapDataY { set; get; }
    public int PathMapDataZ { set; get; }
    public int WorldCoordX { set; get; }
    public int WorldCoordY { set; get; }
    public int WorldCoordZ { set; get; }
    public bool bJumped { set; get; }
    public PathNode3D ParentNode { set; get; }
    public bool bGoalNode { set; get; }
    public int GValue { get; private set; }
    #endregion
    #region method

    public Vector3 GetWorldPosition()
    {
        return new Vector3(WorldCoordX, WorldCoordY, WorldCoordZ);
    }

    public Vector3 GetPathMapPosition()
    {
        return new Vector3(PathMapDataX, PathMapDataY, PathMapDataZ);
    }

    public void CalcGValue()
    {
        if (ParentNode == null)
        {
            GValue = 0;
            return;
        }
        int x = PathMapDataX - ParentNode.PathMapDataX;
        int y = PathMapDataY - ParentNode.PathMapDataY;
        int z = PathMapDataZ - ParentNode.PathMapDataZ;
        int absX = 0;
        int absY = 0;
        int absZ = 0;
        // 대각선 이동.
        if (((x == -1) && (y == 1) && (z == 1))
            || ((x == 1) && (y == 1) && (z == 1))
            || ((x == -1) && (y == -1) && ( z == -1))
            || ((x == 1) && (y == -1) && ( z == -1)))
        {
            absX = Mathf.Abs(x);
            absY = Mathf.Abs(y);
            absZ = Mathf.Abs(z);
            GValue = (absX + absY + absZ) * 14 + ParentNode.GValue;
        }
        else
        {
            absX = Mathf.Abs(x);
            absY = Mathf.Abs(y);
            absZ = Mathf.Abs(z);
            GValue = (absX + absY + absZ) * 10 + ParentNode.GValue;
        }
    }

    public int HValue { get; private set; }
    // 대각선 이동, 장애물들을 모두 무시한채 목표지점간의 x, y 이동값에 대한 결과를 구한다.
    public void CalcHValue(PathNode3D goal)
    {
        int x = Mathf.Abs(goal.PathMapDataX - PathMapDataX);
        int y = Mathf.Abs(goal.PathMapDataY - PathMapDataY);
        int z = Mathf.Abs(goal.PathMapDataZ - PathMapDataZ);
        HValue = (x + y + z) * 10;
    }
    #endregion
}
public struct SimpleVector3
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public SimpleVector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
    public SimpleVector3(Vector3 vec3) { x = vec3.x; y = vec3.y; z = vec3.z; }
}
/// <summary>
/// 길찾기 시작에 필요한 초기화 데이터.
/// </summary>
public struct PathFinderInitData
{
    public Block[,,] WorldBlockData;
    public int SubWorldOffsetX, SubWorldOffsetY, SubWorldOffsetZ;
    public int WorldAreaOffsetX, WorldAreaOffsetY, WorldAreaOffsetZ;

    public PathFinderInitData(Block[,,] blockData, Vector3 subWorldOffset, Vector3 worldAreaOffset)
    {
        WorldBlockData = blockData;
        SubWorldOffsetX = (int)subWorldOffset.x;
        SubWorldOffsetY = (int)subWorldOffset.y;
        SubWorldOffsetZ = (int)subWorldOffset.z;
        WorldAreaOffsetX = (int)worldAreaOffset.x;
        WorldAreaOffsetY = (int)worldAreaOffset.y;
        WorldAreaOffsetZ = (int)worldAreaOffset.z;
    }
}
public class CustomAstar3D : MonoBehaviour
{
    private PathNode3D[,,] PathFindMapData;
    private List<PathNode3D> OpenList = new List<PathNode3D>();
    private List<PathNode3D> ClosedList = new List<PathNode3D>();
    private List<Vector3> LoopDirections = new List<Vector3>();
    private PathNode3D CurrentNode, GoalNode;
    private Stack<PathNode3D> NavigatePath = new Stack<PathNode3D>();
    private SimpleVector3 ActorPosition;
    //
    private WorldConfig GameWorldConfing;
    private PathFinderInitData InitData;
    private int OffsetX = 0;
    private int OffsetY = 0;
    private int OffsetZ = 0;
    private bool bAlreadyAsyncCalcPaths = false;


    public delegate void Del_OnFinishAsyncPathFinding(Stack<PathNode3D> resultPath);
    public event Del_OnFinishAsyncPathFinding OnFinishAsyncPathFinding;

    public void Init(PathFinderInitData data, SimpleVector3 actorPosition)
    {
        if (bAlreadyAsyncCalcPaths == true) return;
        //
        InitData = data;
        ActorPosition = actorPosition;
        GameWorldConfing = WorldConfigFile.Instance.GetConfig();
        //
        var mapData = WorldMapDataFile.Instance.WorldMapDataInstance;
        OffsetX = (InitData.SubWorldOffsetX * GameWorldConfing.SubWorldSizeX) + (InitData.WorldAreaOffsetX * mapData.SubWorldRow * GameWorldConfing.SubWorldSizeX);
        OffsetY = (InitData.SubWorldOffsetY * GameWorldConfing.SubWorldSizeY) + (InitData.WorldAreaOffsetY * mapData.SubWorldColumn * GameWorldConfing.SubWorldSizeY);
        OffsetZ = (InitData.SubWorldOffsetZ * GameWorldConfing.SubWorldSizeZ) + (InitData.WorldAreaOffsetZ * mapData.SubWorldLayer * GameWorldConfing.SubWorldSizeZ);
        //
        PathFindMapData = new PathNode3D[GameWorldConfing.SubWorldSizeX, GameWorldConfing.SubWorldSizeY, GameWorldConfing.SubWorldSizeZ];
        for (int x = 0; x < GameWorldConfing.SubWorldSizeX; x++)
        { 
            for (int y = 0; y < GameWorldConfing.SubWorldSizeY; y++)
            {
                for (int z = 0; z < GameWorldConfing.SubWorldSizeZ; z++)
                {
                    PathFindMapData[x, y, z] = new PathNode3D();
                    PathFindMapData[x, y, z].PathMapDataX = x;
                    PathFindMapData[x, y, z].PathMapDataY = y;
                    PathFindMapData[x, y, z].PathMapDataZ = z;
                    PathFindMapData[x, y, z].ParentNode = null;
                    PathFindMapData[x, y, z].bGoalNode = false;
                }
            }
        }
        InitLoopDirection();
    }

    private Vector3 ConvertPathToWorldCoordinate(SimpleVector3 pathPosition)
    {
        return new Vector3(pathPosition.x + OffsetX, pathPosition.y + OffsetY, pathPosition.z + OffsetZ);
    }

    private Vector3 ConvertPathToWorldCoordinate(Vector3 pathPosition)
    {
        return new Vector3(pathPosition.x + OffsetX, pathPosition.y + OffsetY, pathPosition.z + OffsetZ);
    }

    private Vector3 ConvertWorldToPathCoordinate(SimpleVector3 worldPosition)
    {
        int x = Mathf.Clamp((int)worldPosition.x - OffsetX, 0, GameWorldConfing.SubWorldSizeX - 1);
        int y = Mathf.Clamp((int)worldPosition.y - OffsetY, 0, GameWorldConfing.SubWorldSizeY - 1);
        int z = Mathf.Clamp((int)worldPosition.z - OffsetZ, 0, GameWorldConfing.SubWorldSizeZ - 1);
        return new Vector3(x, y, z);
    }

    private Vector3 ConvertWorldToPathCoordinate(Vector3 worldPosition)
    {
        int x = Mathf.Clamp((int)worldPosition.x - OffsetX, 0, GameWorldConfing.SubWorldSizeX - 1);
        int y = Mathf.Clamp((int)worldPosition.y - OffsetY, 0, GameWorldConfing.SubWorldSizeY - 1);
        int z = Mathf.Clamp((int)worldPosition.z - OffsetZ, 0, GameWorldConfing.SubWorldSizeZ - 1);
        return new Vector3(x, y, z);
    }

    private void ExtractNavigatePath()
    {
        PathNode3D path = GoalNode;
        while (path.ParentNode != null)
        {
            Vector3 worldPos = ConvertPathToWorldCoordinate(new SimpleVector3(path.PathMapDataX, path.PathMapDataY, path.PathMapDataZ));
            path.WorldCoordX = (int)worldPos.x;
            path.WorldCoordY = (int)worldPos.y;
            path.WorldCoordZ = (int)worldPos.z;
            NavigatePath.Push(path);
            path = path.ParentNode;
        }
    }
    private void InitPathFinding()
    {
        OpenList.Clear();
        ClosedList.Clear();
        NavigatePath.Clear();
        InitPathFindMapData();
    }
    /// <summary>
    /// 길찾기를 시작합니다.
    /// 길찾기에 앞서 목표 노드를 반드시 설정해야합니다.
    /// </summary>
    /// <returns>길 노드 목록을 Stack으로 반환합니다.</returns>
    public Stack<PathNode3D> PathFinding(Vector3 goalWorldPosition)
    {
        InitPathFinding();
        SetGoalPathNode(goalWorldPosition);
        SetStartPathNode();
        while ((OpenList.Count != 0) && (!IsGoalInOpenList()))
        {
            SetOpenList();
            PathNode3D selectNode = SelectLowCostPath();
            if (selectNode != null) SearchAdjacentNodes(selectNode);
        }
        ExtractNavigatePath();
        // Stack<T> 의 복사 생성자는 오리지널의 스택 순서에서 반대로 카피를 한다.
        // # 1 : https://msdn.microsoft.com/en-us/library/76atxd68(v=vs.110).aspx
        // # 2 : http://stackoverflow.com/questions/7391348/c-sharp-clone-a-stack
        return KojeomUtility.ReverseStack(new Stack<PathNode3D>(NavigatePath));
    }

    /// <summary>
    /// 길찾기를 비동기로 시작합니다.
    /// 길찾기에 앞서 목표 노드를 반드시 설정해야합니다.
    /// </summary>
    /// <param name="goalWorldPosition"></param>
    /// <returns>길 노드 목록을 Stack으로 반환합니다.</returns>
    public async void AsyncPathFinding(Vector3 goalWorldPosition)
    {
        if (bAlreadyAsyncCalcPaths == true) return;
        //
        KojeomLogger.DebugLog("비동기 경로탐색을 시작합니다.");
        bAlreadyAsyncCalcPaths = true;
        var resultPath = await AsyncNavigating(goalWorldPosition);
        bAlreadyAsyncCalcPaths = false;
        KojeomLogger.DebugLog(string.Format("비동기 경로탐색이 완료되었습니다. [탐색 경로 Count : {0}]", resultPath.Count));
        OnFinishAsyncPathFinding(resultPath);
    }

    private async Task<Stack<PathNode3D>> AsyncNavigating(Vector3 goalWorldPosition)
    {
        return await Task.Run(()=> {
            InitPathFinding();
            SetGoalPathNode(goalWorldPosition);
            SetStartPathNode();
            while ((OpenList.Count != 0) && (!IsGoalInOpenList()))
            {
                SetOpenList();
                PathNode3D selectNode = SelectLowCostPath();
                if (selectNode != null) SearchAdjacentNodes(selectNode);
            }
            ExtractNavigatePath();
            // Stack<T> 의 복사 생성자는 오리지널의 스택 순서에서 반대로 카피를 한다.
            // # 1 : https://msdn.microsoft.com/en-us/library/76atxd68(v=vs.110).aspx
            // # 2 : http://stackoverflow.com/questions/7391348/c-sharp-clone-a-stack
            return KojeomUtility.ReverseStack(new Stack<PathNode3D>(NavigatePath));
        });
    }


    private void SetStartPathNode()
    {
        Vector3 toPathCoordinate = ConvertWorldToPathCoordinate(ActorPosition);
        CurrentNode = PathFindMapData[(int)toPathCoordinate.x, (int)toPathCoordinate.y, (int)toPathCoordinate.z];
        CurrentNode.ParentNode = null;
        CurrentNode.CalcHValue(GoalNode);
        CurrentNode.CalcGValue();
        OpenList.Add(CurrentNode);
    }

    public void SetGoalPathNode(int worldCoordX, int worldCoordY, int worldCoordZ)
    {
        Vector3 toPathCoordinate = ConvertWorldToPathCoordinate(ActorPosition);
        GoalNode = PathFindMapData[(int)toPathCoordinate.x, (int)toPathCoordinate.y, (int)toPathCoordinate.z];
        GoalNode.bGoalNode = true;
    }

    public void SetGoalPathNode(Vector3 worldPosition)
    {
        Vector3 toPathCoordinate = ConvertWorldToPathCoordinate(new SimpleVector3(worldPosition));
        GoalNode = PathFindMapData[(int)toPathCoordinate.x, (int)toPathCoordinate.y, (int)toPathCoordinate.z];
        GoalNode.bGoalNode = true;
    }

    /// <summary>
    /// 길찾기에 이용되는 맵정보 배열을 초기화 합니다.
    /// 실제 월드 블록 배열의 XZ평면의 범위값을 이용해 맵 정보 배열을 순회.
    /// </summary>
    private void InitPathFindMapData()
    {
        for (int x = 0; x < GameWorldConfing.SubWorldSizeX; x++)
            for (int y = 0; y < GameWorldConfing.SubWorldSizeY; y++)
                for (int z = 0; z < GameWorldConfing.SubWorldSizeZ; z++)
                {
                    PathFindMapData[x, y, z].PathMapDataX = x;
                    PathFindMapData[x, y, z].PathMapDataY = y;
                    PathFindMapData[x, y, z].PathMapDataZ = z;
                    PathFindMapData[x, y, z].ParentNode = null;
                    PathFindMapData[x, y, z].bGoalNode = false;
                    PathFindMapData[x, y, z].bJumped = false;
                }
    }

    private void SearchAdjacentNodes(PathNode3D selectNode)
    {
        foreach (Vector3 pos in LoopDirections)
        {
            int searchPosX = selectNode.PathMapDataX + (int)pos.x;
            int searchPosY = selectNode.PathMapDataY + (int)pos.y;
            int searchPosZ = selectNode.PathMapDataZ + (int)pos.z;
            if ((searchPosX >= 0 && searchPosX < GameWorldConfing.SubWorldSizeX) &&
                (searchPosY >= 0 && searchPosY < GameWorldConfing.SubWorldSizeY) &&
                (searchPosZ >= 0 && searchPosZ < GameWorldConfing.SubWorldSizeZ))
            {
                if (!IsInClosedList(searchPosX, searchPosY, searchPosZ))
                {
                    if (!IsInOpenList(searchPosX, searchPosY, searchPosZ))
                    {
                        OpenList.Add(PathFindMapData[searchPosX, searchPosY, searchPosZ]);
                        PathFindMapData[searchPosX, searchPosY, searchPosZ].ParentNode = CurrentNode;
                        PathFindMapData[searchPosX, searchPosY, searchPosZ].CalcHValue(GoalNode);
                        PathFindMapData[searchPosX, searchPosY, searchPosZ].CalcGValue();
                    }
                    else
                    {
                        if ((PathFindMapData[searchPosX, searchPosY, searchPosZ].GValue < selectNode.GValue))
                        {
                            selectNode.ParentNode = PathFindMapData[searchPosX, searchPosY, searchPosZ];
                            selectNode.CalcHValue(GoalNode);
                            selectNode.CalcGValue();
                        }
                    }
                }

            }
        }

    }

    private PathNode3D SelectLowCostPath()
    {
        int minCost = 0;
        PathNode3D lowCostPath = null;
        if (OpenList.Count > 0)
        {
            minCost = OpenList[0].GValue + OpenList[0].HValue;
            lowCostPath = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
            {
                int cost = OpenList[i].GValue + OpenList[i].HValue;
                if (minCost > cost)
                {
                    minCost = cost;
                    lowCostPath = OpenList[i];
                }
            }
            OpenList.Remove(lowCostPath);
            ClosedList.Add(lowCostPath);
            CurrentNode = lowCostPath;
        }
        return lowCostPath;
    }

    private void SetOpenList()
    {
        foreach (Vector3 pos in LoopDirections)
        {
            int searchPosX = CurrentNode.PathMapDataX + (int)pos.x;
            int searchPosY = CurrentNode.PathMapDataY + (int)pos.y;
            int searchPosZ = CurrentNode.PathMapDataZ + (int)pos.z;
            if ((searchPosX >= 0 && searchPosX < GameWorldConfing.SubWorldSizeX) &&
                (searchPosY >= 0 && searchPosY < GameWorldConfing.SubWorldSizeY) &&
                (searchPosZ >= 0 && searchPosZ < GameWorldConfing.SubWorldSizeZ) )
            {
                if ((!IsInClosedList(searchPosX, searchPosY, searchPosZ)) && (!IsInOpenList(searchPosX, searchPosY, searchPosZ)))
                {
                    PathFindMapData[searchPosX, searchPosY, searchPosZ].ParentNode = CurrentNode;
                    PathFindMapData[searchPosX, searchPosY, searchPosZ].CalcHValue(GoalNode);
                    PathFindMapData[searchPosX, searchPosY, searchPosZ].CalcGValue();
                    OpenList.Add(PathFindMapData[searchPosX, searchPosY, searchPosZ]);
                }
            }
        }
        CurNodeToClosedList();
    }

    private bool IsInClosedList(int searchPosX, int searchPosY, int searchPosZ)
    {
        return (ClosedList.Exists((PathNode3D p) =>
        {
            if ((p.PathMapDataX == searchPosX)
            && (p.PathMapDataY == searchPosY)
            && (p.PathMapDataZ == searchPosZ))
            {
                return true;
            }
            return false;
        }));
    }

    private bool IsInOpenList(int searchPosX, int searchPosY, int searchPosZ)
    {
        return (OpenList.Exists((PathNode3D p) =>
        {
            if ((p.PathMapDataX == searchPosX) 
            && (p.PathMapDataY == searchPosY) 
            && (p.PathMapDataZ == searchPosZ))
            {
                return true;
            }
            return false;
        }));
    }

    private bool IsGoalInOpenList()
    {
        return (OpenList.Exists((PathNode3D p) =>
        {
            if (p.bGoalNode)
            {
                return true;
            }
            return false;
        }));
    }

    private void CurNodeToClosedList()
    {
        ClosedList.Add(CurrentNode);
        OpenList.Remove(CurrentNode);
    }

    private void InitLoopDirection()
    {
        // 6면 방향.
        Vector3 top = new Vector3(0, 1, 0);
        Vector3 bottom = new Vector3(0, -1, 0);
        Vector3 front = new Vector3(0, 0, 1);
        Vector3 back = new Vector3(0, 0, -1);
        Vector3 left = new Vector3(1, 0, 0);
        Vector3 right = new Vector3(-1, 0, 0);
        // 대각선 방향.
        Vector3 diagonal1 = new Vector3(1, 1, 1);
        Vector3 diagonal2 = new Vector3(1, 1, -1);
        Vector3 diagonal3 = new Vector3(-1, 1, 1);
        Vector3 diagonal4 = new Vector3(-1, 1, -1);
        Vector3 diagonal5 = new Vector3(1, -1, 1);
        Vector3 diagonal6 = new Vector3(1, -1, -1);
        Vector3 diagonal7 = new Vector3(-1, -1, 1);
        Vector3 diagonal8 = new Vector3(-1, -1, -1);

        LoopDirections.Add(top);
        LoopDirections.Add(bottom);
        LoopDirections.Add(front);
        LoopDirections.Add(back);
        LoopDirections.Add(left);
        LoopDirections.Add(right);
        //
        LoopDirections.Add(diagonal1);
        LoopDirections.Add(diagonal2);
        LoopDirections.Add(diagonal3);
        LoopDirections.Add(diagonal4);
        LoopDirections.Add(diagonal5);
        LoopDirections.Add(diagonal6);
        LoopDirections.Add(diagonal7);
        LoopDirections.Add(diagonal8);
    }
}
