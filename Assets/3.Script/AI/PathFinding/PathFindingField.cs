using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

    public bool isWall;
    public Node ParentNode;

    // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}

public class PathFindingField : MonoBehaviour
{
    #region 싱글톤
    public static PathFindingField Instance { get; private set; } = null;

/*    private static PathFindingField _instance = null;
    public static PathFindingField Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<PathFindingField>();
            }
            _instance.InitData(_instance._map);
            return _instance;
        }
    }*/

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    #endregion
    private bool[,] _mapData;

    public int MinX { get; private set; }
    public int MinY { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }


    // 맵 데이터 초기화
    public void InitData(Transform map)
    {
        MinX = Mathf.RoundToInt(map.GetChild(0).GetChild(0).localPosition.x);
        MinY = Mathf.RoundToInt(map.GetChild(0).GetChild(0).localPosition.z);

        Width = (MinX * -1 + 1) * 2;
        Height = (MinY * -1 + 1) * 2;

        _mapData = new bool[Height, Width];
        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
                _mapData[i,j] = true;
    }

    public void UpdateMapData(float x, float y, bool flag)
    {
        int xPos = Mathf.RoundToInt(x) - MinX;
        int yPos = Mathf.RoundToInt(y) - MinY;

        _mapData[yPos, xPos] = flag;
    }

    public bool GetMapData(int x, int y)
    {
        return _mapData[y, x];
    }
}
