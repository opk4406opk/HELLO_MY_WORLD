using System.Collections;
using System.Collections.Generic;
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
public class CustomAstar3D : MonoBehaviour
{
    private PathNode3D[,,] PathFindMapData;
    private List<PathNode3D> OpenList = new List<PathNode3D>();
    private List<PathNode3D> ClosedList = new List<PathNode3D>();

    private List<Vector3> LoopDirections = new List<Vector3>();

    private PathNode3D CurrentNode, GoalNode;

    private Block[,,] WorldBlockData;
    private Transform MoveObjectTrasnform;

    private Stack<PathNode3D> NavigatePath = new Stack<PathNode3D>();

    private int OffsetX = 0, OffsetY = 0, OffsetZ = 0;
    private WorldConfig GameWorldConfing;

    public void Init(PathFinderInitData data)
    {
        //
        GameWorldConfing = WorldConfigFile.Instance.GetConfig();
        //
        OffsetX = data.OffsetX;
        OffsetY = data.OffsetY;
        OffsetZ = data.OffsetZ;
        WorldBlockData = data.WorldBlockData;
        PathFindMapData = new PathNode3D[GameWorldConfing.sub_world_x_size, GameWorldConfing.sub_world_y_size, GameWorldConfing.sub_world_z_size];
        for (int x = 0; x < GameWorldConfing.sub_world_x_size; x++)
            for (int y = 0; y < GameWorldConfing.sub_world_y_size; y++)
                for (int z = 0; z < GameWorldConfing.sub_world_z_size; z++)
                {
                    PathFindMapData[x, y, z] = new PathNode3D();
                    PathFindMapData[x, y, z].PathMapDataX = x;
                    PathFindMapData[x, y, z].PathMapDataY = y;
                    PathFindMapData[x, y, z].PathMapDataZ = z;
                    PathFindMapData[x, y, z].ParentNode = null;
                    PathFindMapData[x, y, z].bGoalNode = false;
                }
        MoveObjectTrasnform = data.MoveObjTrans;
        InitLoopDirection();
    }
    private void ExtractNavigatePath()
    {
        PathNode3D path = GoalNode;
        while (path.ParentNode != null)
        {
            path.WorldCoordX = path.PathMapDataX + OffsetX;
            path.WorldCoordY = path.PathMapDataY + OffsetY;
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
    public Stack<PathNode3D> PathFinding()
    {
        InitPathFinding();
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

    private void SetStartPathNode()
    {
        // 각 sub World 마다 간격이 있으므로 해당 간격에 맞추어 offset 값을 추가.
        int x = Mathf.RoundToInt(MoveObjectTrasnform.position.x - OffsetX);
        int y = Mathf.RoundToInt(MoveObjectTrasnform.position.y - OffsetY);
        int z = Mathf.RoundToInt(MoveObjectTrasnform.position.z - OffsetZ);
        CurrentNode = PathFindMapData[x, y, z];
        CurrentNode.ParentNode = null;
        CurrentNode.CalcHValue(GoalNode);
        CurrentNode.CalcGValue();
        OpenList.Add(CurrentNode);
    }
    /// <summary>
    /// 길찾기 시작전에 반드시 호출되어야 하는 함수. 
    ///  목표위치 노드를 지정해야 합니다.
    /// </summary>
    public void SetGoalPathNode(int worldCoordX, int worldCoordY, int worldCoordZ)
    {
        // 각 sub World 마다 간격이 있으므로 해당 간격에 맞추어 offset 값을 추가.
        GoalNode = PathFindMapData[worldCoordX - OffsetX, worldCoordY - OffsetY, worldCoordZ - OffsetZ];
        GoalNode.bGoalNode = true;
    }

    /// <summary>
    /// 길찾기에 이용되는 맵정보 배열을 초기화 합니다.
    /// 실제 월드 블록 배열의 XZ평면의 범위값을 이용해 맵 정보 배열을 순회.
    /// </summary>
    private void InitPathFindMapData()
    {
        for (int x = 0; x < GameWorldConfing.sub_world_x_size; x++)
            for (int y = 0; y < GameWorldConfing.sub_world_y_size; y++)
                for (int z = 0; z < GameWorldConfing.sub_world_z_size; z++)
                {
                    PathFindMapData[x, y, z].PathMapDataX = x;
                    PathFindMapData[x, y, z].PathMapDataY = y;
                    PathFindMapData[x, y, z].PathMapDataZ = z;
                    PathFindMapData[x, y, z].WorldCoordY = 0; // ??
                    PathFindMapData[x, y, z].ParentNode = null;
                    PathFindMapData[x, y, z].bGoalNode = false;
                    PathFindMapData[x, y, z].bJumped = false;
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
        for (int x = 0; x < GameWorldConfing.sub_world_x_size; x++)
            for(int y = 0; y < GameWorldConfing.sub_world_y_size; y++)
                for (int z = 0; z < GameWorldConfing.sub_world_z_size; z++)
                {
                    int jumpedHeight = curHeight + 1;
                    if (jumpedHeight < WorldBlockData.GetLength(2))
                    {
                        if (WorldBlockData[x, jumpedHeight, z].isRendered)
                        {
                            PathFindMapData[x, y, z].bJumped = true;
                            PathFindMapData[x, y, z].WorldCoordY = jumpedHeight;
                        }
                    }
                    PathFindMapData[x, y, z].WorldCoordY = CalcDepth(curHeight, x, z);
                }
    }

    private int CalcDepth(int curHeight, int x, int z)
    {
        int height = 0;
        for (int y = curHeight; y > 0; y--)
        {
            if (WorldBlockData[x, y, z].isRendered)
            {
                height = y;
                break;
            }
        }
        return height;
    }


    private void SearchAdjacentNodes(PathNode3D selectNode)
    {
        foreach (Vector3 pos in LoopDirections)
        {
            int searchPosX = selectNode.PathMapDataX + (int)pos.x;
            int searchPosY = selectNode.PathMapDataY + (int)pos.y;
            int searchPosZ = selectNode.PathMapDataZ + (int)pos.z;
            if ((searchPosX >= 0 && searchPosX < GameWorldConfing.sub_world_x_size) &&
                (searchPosY >= 0 && searchPosY < GameWorldConfing.sub_world_y_size) &&
                (searchPosZ >= 0 && searchPosZ < GameWorldConfing.sub_world_z_size))
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
            if ((searchPosX >= 0 && searchPosX < GameWorldConfing.sub_world_x_size) &&
                (searchPosY >= 0 && searchPosY < GameWorldConfing.sub_world_y_size) &&
                (searchPosZ >= 0 && searchPosZ < GameWorldConfing.sub_world_z_size) )
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
