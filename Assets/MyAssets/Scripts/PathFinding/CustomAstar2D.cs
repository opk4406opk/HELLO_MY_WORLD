using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 참고 레퍼런스 문서 List.
// ref #1 : http://cozycoz.egloos.com/9748811
// ref #2 : http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html

public class PathNode2D
{
    // 길찾기용 맵 데이터 좌표 X,Z 이다. 그러나 실제 월드 블록 배열 X,Z 평면값과 동일한 의미로 쓰이기도 하니,
    // 이 값을 실제 길을 찾아가는 오브젝트가 월드 좌표 (x, z)값으로 사용 가능하다.
    #region variables.

    public int PathMapDataX { set; get; }
    public int PathMapDataZ { set; get; }
    public int WorldCoordX { set; get; }
    public int WorldCoordY { set; get; }
    public int WorldCoordZ { set; get; }
    public bool bJumped { set; get; }
    public PathNode2D ParentNode { set; get; }
    public bool bGoalNode { set; get; }
    public int GValue { get; private set; }
    #endregion
    #region method

    public Vector2 GetWorldPosition()
    {
        return new Vector2(WorldCoordX, WorldCoordZ);
    }

    public Vector2 GetPathMapPosition()
    {
        return new Vector2(PathMapDataX, PathMapDataZ);
    }


    public void CalcGValue()
    {
        if (ParentNode == null)
        {
            GValue = 0;
            return;
        }
        int x = PathMapDataX - ParentNode.PathMapDataX;
        int y = PathMapDataZ - ParentNode.PathMapDataZ;
        int absX = 0;
        int absY = 0;
        // 대각선 이동.
        if (((x == -1) && (y == 1))
            || ((x == 1) && (y == 1))
            || ((x == -1) && (y == -1))
            || ((x == 1) && (y == -1)))
        {
            absX = Mathf.Abs(x);
            absY = Mathf.Abs(y);
            GValue = (absX + absY) * 14 + ParentNode.GValue;
        }
        else
        {
            absX = Mathf.Abs(x);
            absY = Mathf.Abs(y);
            GValue = (absX + absY) * 10 + ParentNode.GValue;
        }
    }

    public int HValue { get; private set; }
    // 대각선 이동, 장애물들을 모두 무시한채 목표지점간의 x, y 이동값에 대한 결과를 구한다.
    public void CalcHValue(PathNode2D goal)
    {
        int x = Mathf.Abs(goal.PathMapDataX - PathMapDataX);
        int y = Mathf.Abs(goal.PathMapDataZ - PathMapDataZ);
        HValue = (x + y) * 10;
    }
    #endregion
}

/// <summary>
/// 길찾기 시작에 필요한 초기화 데이터.
/// </summary>
public struct PathFinderInitData
{
    public Block[,,] WorldBlockData;
    public int OffsetX, OffsetY, OffsetZ;

    public PathFinderInitData(Block[,,] blockData, int _offsetX, int _offsetY, int _offsetZ)
    {
        WorldBlockData = blockData;
        OffsetX = _offsetX;
        OffsetY = _offsetY;
        OffsetZ = _offsetZ;
    }
}

public class CustomAstar2D
{
    private PathNode2D[,] PathFindMapData;
    private List<PathNode2D> OpenList = new List<PathNode2D>();
    private List<PathNode2D> ClosedList = new List<PathNode2D>();

    private List<Vector2> EightDir = new List<Vector2>();

    private PathNode2D CurrentNode , GoalNode;

    private Block[,,] WorldBlockData;
    private Transform MoveObjectTrasnform;

    private Stack<PathNode2D> NavigatePath = new Stack<PathNode2D>();

    private int OffsetX = 0, OffsetY = 0, OffsetZ = 0;
    private WorldConfig GameWorldConfing;

    public CustomAstar2D() { }

	public void Init(PathFinderInitData data, Transform actorTransform)
    {
        //
        GameWorldConfing = WorldConfigFile.Instance.GetConfig();
        //
        OffsetX = data.OffsetX;
        OffsetY = data.OffsetY;
		OffsetZ = data.OffsetZ;
        WorldBlockData = data.WorldBlockData;
        PathFindMapData = new PathNode2D[GameWorldConfing.sub_world_x_size, GameWorldConfing.sub_world_z_size];
        for (int x = 0; x < GameWorldConfing.sub_world_x_size; x++)
            for (int z = 0; z < GameWorldConfing.sub_world_z_size; z++)
            {
                PathFindMapData[x, z] = new PathNode2D();
                PathFindMapData[x, z].PathMapDataX = x;
                PathFindMapData[x, z].PathMapDataZ = z;
                PathFindMapData[x, z].ParentNode = null;
                PathFindMapData[x, z].bGoalNode = false;
            }
        MoveObjectTrasnform = actorTransform;
        InitEightDir();
    }

    private void ExtractNavigatePath()
    {
        PathNode2D path = GoalNode;
        while (path.ParentNode != null)
        {
            path.WorldCoordX = path.PathMapDataX + OffsetX;
            path.WorldCoordZ = path.PathMapDataZ + OffsetZ;
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
        BuildPathFindMapData();
    }
    /// <summary>
    /// 길찾기를 시작합니다.
    /// 길찾기에 앞서 목표 노드를 반드시 설정해야합니다.
    /// </summary>
    /// <returns>길 노드 목록을 Stack으로 반환합니다.</returns>
    public Stack<PathNode2D> PathFinding()
    {
        InitPathFinding();
        SetStartPathNode();
        while ((OpenList.Count != 0) && (!IsGoalInOpenList()))
        {
            SetOpenList();
            PathNode2D selectNode = SelectLowCostPath();
            if (selectNode != null) SearchAdjacentNodes(selectNode);
        }
        ExtractNavigatePath();
        // Stack<T> 의 복사 생성자는 오리지널의 스택 순서에서 반대로 카피를 한다.
        // # 1 : https://msdn.microsoft.com/en-us/library/76atxd68(v=vs.110).aspx
        // # 2 : http://stackoverflow.com/questions/7391348/c-sharp-clone-a-stack
        return KojeomUtility.ReverseStack(new Stack<PathNode2D>(NavigatePath));
    }

    private void SetStartPathNode()
    {
        // 각 sub World 마다 간격이 있으므로 해당 간격에 맞추어 offset 값을 추가.
        int x = Mathf.RoundToInt(MoveObjectTrasnform.position.x - OffsetX);
		int z = Mathf.RoundToInt(MoveObjectTrasnform.position.z - OffsetZ);
        CurrentNode = PathFindMapData[x, z];
        CurrentNode.ParentNode = null;
        CurrentNode.CalcHValue(GoalNode);
        CurrentNode.CalcGValue();
        OpenList.Add(CurrentNode);
    }
    /// <summary>
    /// 길찾기 시작전에 반드시 호출되어야 하는 함수. 
    ///  목표위치 노드를 지정해야 합니다.
    /// </summary>
    public void SetGoalPathNode(int worldCoordX, int worldCoordZ)
    {
        // 각 sub World 마다 간격이 있으므로 해당 간격에 맞추어 offset 값을 추가.
		GoalNode = PathFindMapData[worldCoordX - OffsetX, worldCoordZ - OffsetZ];
        GoalNode.bGoalNode = true;
    }

    /// <summary>
    /// 길찾기에 이용되는 맵정보 배열을 초기화 합니다.
    /// 실제 월드 블록 배열의 XZ평면의 범위값을 이용해 맵 정보 배열을 순회.
    /// </summary>
    private void InitPathFindMapData()
    {
        for (int x = 0; x < GameWorldConfing.sub_world_x_size; x++)
            for (int z = 0; z < GameWorldConfing.sub_world_z_size; z++)
            {
                PathFindMapData[x, z].PathMapDataX = x;
                PathFindMapData[x, z].PathMapDataZ = z;
                PathFindMapData[x, z].WorldCoordY = 0;
                PathFindMapData[x, z].ParentNode = null;
                PathFindMapData[x, z].bGoalNode = false;
                PathFindMapData[x, z].bJumped = false;
            }
    }
    /// <summary>
    /// 길찾기에 이용되는 맵정보 배열에 실질적인 정보를 저장합니다.
    /// </summary>
    private void BuildPathFindMapData()
    {
        //길을 찾아 움직이려는 오브젝트의 현재 높이.
        //실제 밟고 서있는 WorldBlock 배열의 Y값을 구한다.
        // p.s. 게임 내 월드블록 배열에서 XZ 평면에서의 데이터만 있으면 된다.
        int curHeight = Mathf.RoundToInt(MoveObjectTrasnform.position.y);
        for(int x = 0; x < GameWorldConfing.sub_world_x_size; x++)
            for(int z=0; z < GameWorldConfing.sub_world_z_size; z++)
            {
                int jumpedHeight = curHeight + 1;
                if (jumpedHeight < WorldBlockData.GetLength(2))
                {
                    if (WorldBlockData[x, jumpedHeight, z].bRendered){
                        PathFindMapData[x, z].bJumped = true;
                        PathFindMapData[x, z].WorldCoordY = jumpedHeight;
                    }
                }
                PathFindMapData[x, z].WorldCoordY = CalcDepth(curHeight, x, z);
            }
    }

    private int CalcDepth(int curHeight, int x, int z)
    {
        int height = 0;
        for(int y = curHeight; y > 0; y--)
        {
            if (WorldBlockData[x, y, z].bRendered)
            {
                height = y;
                break;
            }
        }
        return height;
    }


    private void SearchAdjacentNodes(PathNode2D selectNode)
    {
        foreach (Vector2 pos in EightDir)
        {
            int searchPosX = selectNode.PathMapDataX + (int)pos.x;
            int searchPosY = selectNode.PathMapDataZ + (int)pos.y;
            if ((searchPosX >= 0 && searchPosX < GameWorldConfing.sub_world_x_size) && (searchPosY >= 0 && searchPosY < GameWorldConfing.sub_world_z_size))
            {
                if (!IsInClosedList(searchPosX, searchPosY))
                {
                    if (!IsInOpenList(searchPosX, searchPosY))
                    {
                        OpenList.Add(PathFindMapData[searchPosX, searchPosY]);
                        PathFindMapData[searchPosX, searchPosY].ParentNode = CurrentNode;
                        PathFindMapData[searchPosX, searchPosY].CalcHValue(GoalNode);
                        PathFindMapData[searchPosX, searchPosY].CalcGValue();
                    }
                    else
                    {
                        if ((PathFindMapData[searchPosX, searchPosY].GValue < selectNode.GValue))
                        {
                            selectNode.ParentNode = PathFindMapData[searchPosX, searchPosY];
                            selectNode.CalcHValue(GoalNode);
                            selectNode.CalcGValue();
                        }
                    }
                }

            }
        }

    }

    private PathNode2D SelectLowCostPath()
    {
        int minCost = 0;
        PathNode2D lowCostPath = null;
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
        foreach (Vector2 pos in EightDir)
        {
            int searchPosX = CurrentNode.PathMapDataX + (int)pos.x;
            int searchPosY = CurrentNode.PathMapDataZ + (int)pos.y;
            if ((searchPosX >= 0 && searchPosX < GameWorldConfing.sub_world_x_size) && (searchPosY >= 0 && searchPosY < GameWorldConfing.sub_world_z_size))
            {
                if ((!IsInClosedList(searchPosX, searchPosY)) && (!IsInOpenList(searchPosX, searchPosY)))
                {
                    PathFindMapData[searchPosX, searchPosY].ParentNode = CurrentNode;
                    PathFindMapData[searchPosX, searchPosY].CalcHValue(GoalNode);
                    PathFindMapData[searchPosX, searchPosY].CalcGValue();
                    OpenList.Add(PathFindMapData[searchPosX, searchPosY]);
                }
            }
        }
        CurNodeToClosedList();
    }

    private bool IsInClosedList(int searchPosX, int searchPosY)
    {
        return (ClosedList.Exists((PathNode2D p) =>
        {
            if ((p.PathMapDataX == searchPosX)
            && (p.PathMapDataZ == searchPosY))
            {
                return true;
            }
            return false;
        }));
    }

    private bool IsInOpenList(int searchPosX, int searchPosY)
    {
        return (OpenList.Exists((PathNode2D p) =>
        {
            if ((p.PathMapDataX == searchPosX)
            && (p.PathMapDataZ == searchPosY))
            {
                return true;
            }
            return false;
        }));
    }

    private bool IsGoalInOpenList()
    {
        return (OpenList.Exists((PathNode2D p) =>
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

    private void InitEightDir()
    {
        Vector2 north = new Vector2(0 ,1);
        Vector2 south = new Vector2(0 ,-1);
        Vector2 west = new Vector2(-1, 0);
        Vector2 east = new Vector2(1, 0);
        Vector2 northWest = new Vector2(-1, 1);
        Vector2 northEast = new Vector2(1, 1);
        Vector2 southWest = new Vector2(-1 , -1);
        Vector2 southEast = new Vector2(1, -1);

        EightDir.Add(north);
        EightDir.Add(south);
        EightDir.Add(west);
        EightDir.Add(east);
        EightDir.Add(northWest);
        EightDir.Add(northEast);
        EightDir.Add(southWest);
        EightDir.Add(southEast);
    }
}
