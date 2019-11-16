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

    public static bool operator ==(PathNode3D a, PathNode3D b)
    {
        if (ReferenceEquals(a, b) == true) return true;
        if (ReferenceEquals(a, null) == true) return false;
        if (ReferenceEquals(b, null) == true) return false;
        return a.PathMapDataX == b.PathMapDataX && a.PathMapDataY == b.PathMapDataY && a.PathMapDataZ == b.PathMapDataZ;
    }

    public static bool operator !=(PathNode3D a, PathNode3D b)
    {
        return !(a == b);
    }

    public void CalcGValue()
    {
        if (ParentNode == null)
        {
            GValue = 0;
            return;
        }
        int x = PathMapDataX - ParentNode.PathMapDataX;
        int z = PathMapDataZ - ParentNode.PathMapDataZ;
        // 대각선 이동.
        if (((x == 1) && (z == 1)) ||
            ((x == 1) && (z == -1)) ||
            ((x == -1) && (z == 1)) ||
            ((x == -1) && (z == -1)))
        {
            GValue = (Mathf.Abs(x) + Mathf.Abs(z)) * 14 + ParentNode.GValue;
        }
        else
        {
            GValue = (Mathf.Abs(x) + Mathf.Abs(z)) * 10 + ParentNode.GValue;
        }
    }

    public int HValue { get; private set; }
    // 대각선 이동, 장애물들을 모두 무시한채 목표지점간의 x, y 이동값에 대한 결과를 구한다.
    public void CalcHValue(PathNode3D goal)
    {
        int x = Mathf.Abs(goal.PathMapDataX - PathMapDataX);
        int z = Mathf.Abs(goal.PathMapDataZ - PathMapDataZ);
        HValue = (x + z) * 10;
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
/// 길찾기 시작에 필요한 데이터.
/// </summary>
public struct PathFinderSettings
{
    public Block[,,] WorldBlockData;
    public int SubWorldOffsetX, SubWorldOffsetY, SubWorldOffsetZ;
    public int WorldAreaOffsetX, WorldAreaOffsetY, WorldAreaOffsetZ;

    public PathFinderSettings(Block[,,] blockData, Vector3 subWorldOffset, Vector3 worldAreaOffset)
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
    private PathFinderSettings PathFindingSettings;
    private int OffsetX = 0;
    private int OffsetY = 0;
    private int OffsetZ = 0;
    private bool bAlreadyAsyncCalcPaths = false;

    public delegate void Del_OnFinishAsyncPathFinding(Stack<PathNode3D> resultPath);
    public event Del_OnFinishAsyncPathFinding OnFinishAsyncPathFinding;

    public void Init(PathFinderSettings settings, SimpleVector3 actorPosition)
    {
        if (bAlreadyAsyncCalcPaths == true) return;
        //
        PathFindingSettings = settings;
        ActorPosition = actorPosition;
        GameWorldConfing = WorldConfigFile.Instance.GetConfig();
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
        int x = Mathf.Clamp(Mathf.Abs((int)worldPosition.x - OffsetX), 0, GameWorldConfing.SubWorldSizeX - 1);
        int y = Mathf.Clamp(Mathf.Abs((int)worldPosition.y - OffsetY), 0, GameWorldConfing.SubWorldSizeY - 1);
        int z = Mathf.Clamp(Mathf.Abs((int)worldPosition.z - OffsetZ), 0, GameWorldConfing.SubWorldSizeZ - 1);
        return new Vector3(x, y, z);
    }

    private Vector3 ConvertWorldToPathCoordinate(Vector3 worldPosition)
    {
        int x = Mathf.Clamp(Mathf.Abs((int)worldPosition.x - OffsetX), 0, GameWorldConfing.SubWorldSizeX - 1);
        int y = Mathf.Clamp(Mathf.Abs((int)worldPosition.y - OffsetY), 0, GameWorldConfing.SubWorldSizeY - 1);
        int z = Mathf.Clamp(Mathf.Abs((int)worldPosition.z - OffsetZ), 0, GameWorldConfing.SubWorldSizeZ - 1);
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
    private void InitializePathFinding()
    {
        // Offset 세팅.
        var mapData = WorldMapDataFile.Instance.WorldMapDataInstance;
        OffsetX = (PathFindingSettings.SubWorldOffsetX * GameWorldConfing.SubWorldSizeX) + (PathFindingSettings.WorldAreaOffsetX * mapData.SubWorldRow * GameWorldConfing.SubWorldSizeX);
        OffsetY = (PathFindingSettings.SubWorldOffsetY * GameWorldConfing.SubWorldSizeY) + (PathFindingSettings.WorldAreaOffsetY * mapData.SubWorldColumn * GameWorldConfing.SubWorldSizeY);
        OffsetZ = (PathFindingSettings.SubWorldOffsetZ * GameWorldConfing.SubWorldSizeZ) + (PathFindingSettings.WorldAreaOffsetZ * mapData.SubWorldLayer * GameWorldConfing.SubWorldSizeZ);
        // 길찾기용 맵 데이터 초기화.
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
        //
        OpenList.Clear();
        ClosedList.Clear();
        NavigatePath.Clear();
        //
        InitLoopDirection();
    }
    /// <summary>
    /// 길찾기를 시작합니다.
    /// 길찾기에 앞서 목표 노드를 반드시 설정해야합니다.
    /// </summary>
    /// <returns>길 노드 목록을 Stack으로 반환합니다.</returns>
    public Stack<PathNode3D> PathFinding(Vector3 goalWorldPosition)
    {
        InitializePathFinding();
        SetGoalPathNode(goalWorldPosition);
        SetStartPathNode();
        while (OpenList.Count > 0 && IsGoalInOpenList() == false)
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
        var resultPath = await TaskAsyncNavigating(goalWorldPosition);
        OnFinishAsyncPathFinding(resultPath);
        bAlreadyAsyncCalcPaths = false;
        KojeomLogger.DebugLog(string.Format("비동기 경로탐색이 완료되었습니다. [탐색 경로 Count : {0}]", resultPath.Count));
    }

    private async Task<Stack<PathNode3D>> TaskAsyncNavigating(Vector3 goalWorldPosition)
    {
        return await Task.Run(()=> {
            InitializePathFinding();
            SetGoalPathNode(goalWorldPosition);
            SetStartPathNode();
            while (OpenList.Count > 0 && IsGoalInOpenList() == false)
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

    public void SetGoalPathNode(Vector3 worldPosition)
    {
        Vector3 toPathCoordinate = ConvertWorldToPathCoordinate(new SimpleVector3(worldPosition));
        GoalNode = PathFindMapData[(int)toPathCoordinate.x, (int)toPathCoordinate.y, (int)toPathCoordinate.z];
        GoalNode.bGoalNode = true;
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
                PathNode3D searchNode = PathFindMapData[searchPosX, searchPosY, searchPosZ];
                if (IsInClosedList(searchNode) == false)
                {
                    if (IsInOpenList(searchNode) == false && IsCanMoveNode(searchNode) == true)
                    {
                        OpenList.Add(searchNode);
                        searchNode.ParentNode = CurrentNode;
                        searchNode.CalcHValue(GoalNode);
                        searchNode.CalcGValue();
                    }
                    else if (IsInOpenList(searchNode) == true && IsCanMoveNode(searchNode) == true)
                    {
                        if (searchNode.GValue < selectNode.GValue)
                        {
                            selectNode.ParentNode = searchNode;
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
                PathNode3D searchNode = PathFindMapData[searchPosX, searchPosY, searchPosZ];
                bool bNotInCloseAndOpen = (IsInClosedList(searchNode) == false) && (IsInOpenList(searchNode) == false);
                if (bNotInCloseAndOpen == true && IsCanMoveNode(searchNode) == true)
                {
                    searchNode.ParentNode = CurrentNode;
                    searchNode.CalcHValue(GoalNode);
                    searchNode.CalcGValue();
                    OpenList.Add(searchNode);
                }
            }
        }
        CurNodeToClosedList();
    }

    private bool IsCanMoveNode(PathNode3D node)
    {
        int x = node.PathMapDataX;
        int y = node.PathMapDataY;
        int z = node.PathMapDataZ;
        BlockTileType blockType = (BlockTileType)PathFindingSettings.WorldBlockData[x, y, z].Type;
        if (blockType == BlockTileType.EMPTY)
        {
            ClosedList.Add(node);
            return false;
        }

        //int upperHeight = y + 1;
        //if (upperHeight < PathFindingNeedData.SubWorldOffsetY)
        //{
        //    BlockTileType upperBlockType = (BlockTileType)PathFindingNeedData.WorldBlockData[x, upperHeight, z].Type;
        //    if(upperBlockType != BlockTileType.EMPTY)
        //    {
        //        return false;
        //    }
        //}

        return true;
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

    private bool IsInClosedList(Vector3 searchPos)
    {
        return (ClosedList.Exists((PathNode3D p) =>
        {
            if ((p.PathMapDataX == (int)searchPos.x)
            && (p.PathMapDataY == (int)searchPos.y)
            && (p.PathMapDataZ == (int)searchPos.z))
            {
                return true;
            }
            return false;
        }));
    }

    private bool IsInClosedList(PathNode3D node)
    {
        return (ClosedList.Exists((PathNode3D p) =>
        {
            if(p == node)
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

    private bool IsInOpenList(Vector3 searchPos)
    {
        return (OpenList.Exists((PathNode3D p) =>
        {
            if ((p.PathMapDataX == (int)searchPos.x)
             && (p.PathMapDataY == (int)searchPos.y)
             && (p.PathMapDataZ == (int)searchPos.z))
            {
                return true;
            }
            return false;
        }));
    }

    private bool IsInOpenList(PathNode3D node)
    {
        return (OpenList.Exists((PathNode3D p) =>
        {
            if (p == node)
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
        // 8방향으로만 계산.
        LoopDirections.Clear();
        LoopDirections.Add(new Vector3(0, 0, 1));
        LoopDirections.Add(new Vector3(0, 0, -1));
        LoopDirections.Add(new Vector3(1, 0, 0));
        LoopDirections.Add(new Vector3(-1, 0, 0));
        LoopDirections.Add(new Vector3(1, 0, 1));
        LoopDirections.Add(new Vector3(1, 0, -1));
        LoopDirections.Add(new Vector3(-1, 0, 1));
        LoopDirections.Add(new Vector3(-1, 0, -1));
        //LoopDirections.Add(new Vector3(0, -1, 1));
        //LoopDirections.Add(new Vector3(0, -1, -1));
        //LoopDirections.Add(new Vector3(1, -1, 0));
        //LoopDirections.Add(new Vector3(-1, -1, 0));
        //LoopDirections.Add(new Vector3(0, 1, 1));
        //LoopDirections.Add(new Vector3(0, 1, -1));
        //LoopDirections.Add(new Vector3(1, 1, 0));
        //LoopDirections.Add(new Vector3(-1, 1, 0));
        //
        //LoopDirections.Add(new Vector3(1, -1, 1));
        //LoopDirections.Add(new Vector3(1, -1, -1));
        //LoopDirections.Add(new Vector3(-1, -1, 1));
        //LoopDirections.Add(new Vector3(-1, -1, -1));
        //LoopDirections.Add(new Vector3(1, 1, 1));
        //LoopDirections.Add(new Vector3(1, 1, -1));
        //LoopDirections.Add(new Vector3(-1, 1, 1));
        //LoopDirections.Add(new Vector3(-1, 1, -1));
    }
}
