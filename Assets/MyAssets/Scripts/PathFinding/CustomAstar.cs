using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 참고 레퍼런스 문서 List.
// ref #1 : http://cozycoz.egloos.com/9748811
// ref #2 : http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html

public class PathNode
{
    // 길찾기용 맵 데이터 좌표 X,Z 이다. 그러나 실제 월드 블록 배열 X,Z 평면값과 동일한 의미로 쓰이기도 하니,
    // 이 값을 실제 길을 찾아가는 오브젝트가 월드 좌표 (x, z)값으로 사용 가능하다.
    #region variables.
   
    private int _pathMapDataX;
    public int pathMapDataX
    {
        set { _pathMapDataX = value; }
        get { return _pathMapDataX; }
    }
    private int _pathMapDataZ;
    public int pathMapDataZ
    {
        set { _pathMapDataZ = value; }
        get { return _pathMapDataZ; }
    }
    private int _worldCoordX;
    public int worldCoordX
    {
        set { _worldCoordX = value; }
        get { return _worldCoordX; }
    }
    private int _worldCoordY;
    public int worldCoordY
    {
        set { _worldCoordY = value; }
        get { return _worldCoordY; }
    }
    private int _worldCoordZ;
    public int worldCoordZ
    {
        set { _worldCoordZ = value; }
        get { return _worldCoordZ; }
    }

    private bool _isJumped;
    public bool isJumped
    {
        set { _isJumped = value; }
        get { return _isJumped; }
    }

    private PathNode _parentNode;
    public PathNode parentNode
    {
        set { _parentNode = value; }
        get { return _parentNode; }
    }

    private bool _isGoalNode;
    public bool isGoalNode
    {
        set { _isGoalNode = value; }
        get { return _isGoalNode; }
    }

    private int _gValue;
    public int gValue
    {
        get { return _gValue; }
    }
    #endregion
    #region method
    
    public void Calc_G_Value()
    {
        if (_parentNode == null)
        {
            _gValue = 0;
            return;
        }
        int x = _pathMapDataX - _parentNode.pathMapDataX;
        int y = _pathMapDataZ - _parentNode.pathMapDataZ;
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
            _gValue = (absX + absY) * 14 + _parentNode.gValue;
        }
        else
        {
            absX = Mathf.Abs(x);
            absY = Mathf.Abs(y);
            _gValue = (absX + absY) * 10 + _parentNode.gValue;
        }
    }
    private int _hValue;
    public int hValue
    {
        get { return _hValue; }
    }
    // 대각선 이동, 장애물들을 모두 무시한채 목표지점간의 x, y 이동값에 대한 결과를 구한다.
    public void Calc_H_Value(PathNode goal)
    {
        int x = Mathf.Abs(goal.pathMapDataX - _pathMapDataX);
        int y = Mathf.Abs(goal.pathMapDataZ - _pathMapDataZ);
        _hValue = (x + y) * 10;
    }
    #endregion
}

/// <summary>
/// 길찾기 시작에 필요한 초기화 데이터.
/// </summary>
public struct PathFinderInitData
{
    public Block[,,] worldBlockData;
    public Transform moveObjTrans;
    public int offsetX, offsetZ;

    public PathFinderInitData(Block[,,] blockData, Transform trans, int _offsetX, int _offsetZ)
    {
        worldBlockData = blockData;
        moveObjTrans = trans;
        offsetX = _offsetX;
        offsetZ = _offsetZ;
    }
}

public class CustomAstar
{
	private int MAP_SIZE_X, MAP_SIZE_Z;

    private PathNode[,] pathFindMapData;
    private List<PathNode> openList = new List<PathNode>();
    private List<PathNode> closedList = new List<PathNode>();

    private List<Vector2> eightDir = new List<Vector2>();

    private PathNode curNode , goalNode;

    private Block[,,] worldBlockData;
    private Transform trans;

    private Stack<PathNode> navigatePath = new Stack<PathNode>();

	private int offsetX = 0, offsetZ = 0;

    public CustomAstar() { }

	public void Init(PathFinderInitData data)
    {
        //
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        //
		offsetX = data.offsetX;
		offsetZ = data.offsetZ;
        worldBlockData = data.worldBlockData;
        MAP_SIZE_X = gameConfig.sub_world_x_size;
        MAP_SIZE_Z = gameConfig.sub_world_z_size;
        pathFindMapData = new PathNode[MAP_SIZE_X, MAP_SIZE_Z];
        for (int x = 0; x < MAP_SIZE_X; x++)
            for (int z = 0; z < MAP_SIZE_Z; z++)
            {
                pathFindMapData[x, z] = new PathNode();
                pathFindMapData[x, z].pathMapDataX = x;
                pathFindMapData[x, z].pathMapDataZ = z;
                pathFindMapData[x, z].parentNode = null;
                pathFindMapData[x, z].isGoalNode = false;
            }
        trans = data.moveObjTrans;
        InitEightDir();
    }

    private void ExtractNavigatePath()
    {
        PathNode path = goalNode;
        while (path.parentNode != null)
        {
            path.worldCoordX = path.pathMapDataX + offsetX;
            path.worldCoordZ = path.pathMapDataZ + offsetZ;
            navigatePath.Push(path);
            path = path.parentNode;
        }
    }
    private void InitPathFinding()
    {
        openList.Clear();
        closedList.Clear();
        navigatePath.Clear();
        InitPathFindMapData();
        BuildPathFindMapData();
    }
    /// <summary>
    /// 길찾기를 시작합니다.
    /// 길찾기에 앞서 목표 노드를 반드시 설정해야합니다.
    /// </summary>
    /// <returns>길 노드 목록을 Stack으로 반환합니다.</returns>
    public Stack<PathNode> PathFinding()
    {
        InitPathFinding();
        SetStartPathNode();
        while ((openList.Count != 0) && (!IsGoalInOpenList()))
        {
            SetOpenList();
            PathNode selectNode = SelectLowCostPath();
            if (selectNode != null) SearchAdjacentNodes(selectNode);
        }
        ExtractNavigatePath();
        // Stack<T> 의 복사 생성자는 오리지널의 스택 순서에서 반대로 카피를 한다.
        // # 1 : https://msdn.microsoft.com/en-us/library/76atxd68(v=vs.110).aspx
        // # 2 : http://stackoverflow.com/questions/7391348/c-sharp-clone-a-stack
        return KojeomUtility.ReversePathStack(new Stack<PathNode>(navigatePath));
    }

    private void SetStartPathNode()
    {
        // 각 sub World 마다 간격이 있으므로 해당 간격에 맞추어 offset 값을 추가.
        int x = Mathf.RoundToInt(trans.position.x - offsetX);
		int z = Mathf.RoundToInt(trans.position.z - offsetZ);
        curNode = pathFindMapData[x, z];
        curNode.parentNode = null;
        curNode.Calc_H_Value(goalNode);
        curNode.Calc_G_Value();
        openList.Add(curNode);
    }
    /// <summary>
    /// 길찾기 시작전에 반드시 호출되어야 하는 함수. 
    ///  목표위치 노드를 지정해야 합니다.
    /// </summary>
    public void SetGoalPathNode(int worldCoordX, int worldCoordZ)
    {
        // 각 sub World 마다 간격이 있으므로 해당 간격에 맞추어 offset 값을 추가.
		goalNode = pathFindMapData[worldCoordX - offsetX, worldCoordZ - offsetZ];
        goalNode.isGoalNode = true;
    }

    /// <summary>
    /// 길찾기에 이용되는 맵정보 배열을 초기화 합니다.
    /// 실제 월드 블록 배열의 XZ평면의 범위값을 이용해 맵 정보 배열을 순회.
    /// </summary>
    private void InitPathFindMapData()
    {
        for (int x = 0; x < MAP_SIZE_X; x++)
            for (int z = 0; z < MAP_SIZE_Z; z++)
            {
                pathFindMapData[x, z].pathMapDataX = x;
                pathFindMapData[x, z].pathMapDataZ = z;
                pathFindMapData[x, z].worldCoordY = 0;
                pathFindMapData[x, z].parentNode = null;
                pathFindMapData[x, z].isGoalNode = false;
                pathFindMapData[x, z].isJumped = false;
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
        int curHeight = Mathf.RoundToInt(trans.position.y);
        for(int x = 0; x < MAP_SIZE_X; x++)
            for(int z=0; z < MAP_SIZE_Z; z++)
            {
                int jumpedHeight = curHeight + 1;
                if (jumpedHeight < worldBlockData.GetLength(2))
                {
                    if (worldBlockData[x, jumpedHeight, z].isRendered){
                        pathFindMapData[x, z].isJumped = true;
                        pathFindMapData[x, z].worldCoordY = jumpedHeight;
                    }
                }
                pathFindMapData[x, z].worldCoordY = CalcDepth(curHeight, x, z);
            }
    }

    private int CalcDepth(int curHeight, int x, int z)
    {
        int height = 0;
        for(int y = curHeight; y > 0; y--)
        {
            if (worldBlockData[x, y, z].isRendered)
            {
                height = y;
                break;
            }
        }
        return height;
    }


    private void SearchAdjacentNodes(PathNode selectNode)
    {
        foreach (Vector2 pos in eightDir)
        {
            int searchPosX = selectNode.pathMapDataX + (int)pos.x;
            int searchPosY = selectNode.pathMapDataZ + (int)pos.y;
            if ((searchPosX >= 0 && searchPosX < MAP_SIZE_X) && (searchPosY >= 0 && searchPosY < MAP_SIZE_Z))
            {
                if (!IsInClosedList(searchPosX, searchPosY))
                {
                    if (!IsInOpenList(searchPosX, searchPosY))
                    {
                        openList.Add(pathFindMapData[searchPosX, searchPosY]);
                        pathFindMapData[searchPosX, searchPosY].parentNode = curNode;
                        pathFindMapData[searchPosX, searchPosY].Calc_H_Value(goalNode);
                        pathFindMapData[searchPosX, searchPosY].Calc_G_Value();
                    }
                    else
                    {
                        if ((pathFindMapData[searchPosX, searchPosY].gValue < selectNode.gValue))
                        {
                            selectNode.parentNode = pathFindMapData[searchPosX, searchPosY];
                            selectNode.Calc_H_Value(goalNode);
                            selectNode.Calc_G_Value();
                        }
                    }
                }

            }
        }

    }

    private PathNode SelectLowCostPath()
    {
        int minCost = 0;
        PathNode lowCostPath = null;
        if (openList.Count > 0)
        {
            minCost = openList[0].gValue + openList[0].hValue;
            lowCostPath = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                int cost = openList[i].gValue + openList[i].hValue;
                if (minCost > cost)
                {
                    minCost = cost;
                    lowCostPath = openList[i];
                }
            }
            openList.Remove(lowCostPath);
            closedList.Add(lowCostPath);
            curNode = lowCostPath;
        }
        return lowCostPath;
    }

    private void SetOpenList()
    {
        foreach (Vector2 pos in eightDir)
        {
            int searchPosX = curNode.pathMapDataX + (int)pos.x;
            int searchPosY = curNode.pathMapDataZ + (int)pos.y;
            if ((searchPosX >= 0 && searchPosX < MAP_SIZE_X) && (searchPosY >= 0 && searchPosY < MAP_SIZE_Z))
            {
                if ((!IsInClosedList(searchPosX, searchPosY)) && (!IsInOpenList(searchPosX, searchPosY)))
                {
                    pathFindMapData[searchPosX, searchPosY].parentNode = curNode;
                    pathFindMapData[searchPosX, searchPosY].Calc_H_Value(goalNode);
                    pathFindMapData[searchPosX, searchPosY].Calc_G_Value();
                    openList.Add(pathFindMapData[searchPosX, searchPosY]);
                }
            }
        }
        CurNodeToClosedList();
    }

    private bool IsInClosedList(int searchPosX, int searchPosY)
    {
        return (closedList.Exists((PathNode p) =>
        {
            if ((p.pathMapDataX == searchPosX)
            && (p.pathMapDataZ == searchPosY))
            {
                return true;
            }
            return false;
        }));
    }

    private bool IsInOpenList(int searchPosX, int searchPosY)
    {
        return (openList.Exists((PathNode p) =>
        {
            if ((p.pathMapDataX == searchPosX)
            && (p.pathMapDataZ == searchPosY))
            {
                return true;
            }
            return false;
        }));
    }

    private bool IsGoalInOpenList()
    {
        return (openList.Exists((PathNode p) =>
        {
            if (p.isGoalNode)
            {
                return true;
            }
            return false;
        }));
    }

    private void CurNodeToClosedList()
    {
        closedList.Add(curNode);
        openList.Remove(curNode);
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

        eightDir.Add(north);
        eightDir.Add(south);
        eightDir.Add(west);
        eightDir.Add(east);
        eightDir.Add(northWest);
        eightDir.Add(northEast);
        eightDir.Add(southWest);
        eightDir.Add(southEast);
    }
}
