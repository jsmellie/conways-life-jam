using UnityEngine;
using System.Collections;

public class CellManager : MonoBehaviour
{
  #region Fields & Properties

  static CellManager _instance = null;
  public static CellManager instance
  {
    get
    {
      if (_instance == null)
      {
        GameObject instanceObj = new GameObject("GridMaker");

        _instance = instanceObj.AddComponent<CellManager>();
      }

      return _instance;
    }
  }

  [SerializeField]
  float _tickTime = 0;
  public float tickTime
  {
    get { return _tickTime; }
  }

  public Cell cellPrefab = null;

  public int height = 20;

  [SerializeField, HideInInspector]
  Cell[,] _cells = null;

  #endregion

  void Awake()
  {
    _instance = this;

    float aspect = Camera.main.aspect;
    float camHeight = Camera.main.orthographicSize * 2;

    float scale = camHeight / height;

    int width = (int)(height * aspect);

    _cells = new Cell[width, height];

    for (int i = 0; i < this.transform.childCount; ++i )
    {
      _cells[i / height, i % height] = transform.GetChild(i).GetComponent<Cell>();
    }
  }

  public void CalculateLife(Cell curCell)
  {
    Vector2 location = curCell.gridLocation;

    Vector2[] neighboars = new Vector2[8];

    neighboars[0] = location + new Vector2(-1, 1);
    neighboars[1] = location + new Vector2(0, 1);
    neighboars[2] = location + new Vector2(1, 1);

    neighboars[3] = location - new Vector2(-1, 0);
    neighboars[4] = location - new Vector2(1, 0);

    neighboars[5] = location + new Vector2(-1, -1);
    neighboars[6] = location + new Vector2(0, -1);
    neighboars[7] = location + new Vector2(1, -1);

    for (int i = 0; i < neighboars.Length; ++i)
    {
      if (neighboars[i].x < 0)
      {
        neighboars[i].x = _cells.GetLength(0) - 1;
      }
      else if (neighboars[i].x >= _cells.GetLength(0))
      {
        neighboars[i].x = 0;
      }

      if (neighboars[i].y < 0)
      {
        neighboars[i].y = _cells.GetLength(1) - 1;
      }
      else if (neighboars[i].y >= _cells.GetLength(1))
      {
        neighboars[i].y = 0;
      }
    }

    int liveCount = 0;

    for (int liveCounter = 0; liveCounter < neighboars.Length; ++liveCounter)
    {
      Vector2 curLocation = neighboars[liveCounter];

      if (_cells[(int)curLocation.x, (int)curLocation.y].isAlive)
      {
        liveCount += 1;
      }
    }

    if (curCell.isAlive)
    {
      if (liveCount < 2 || liveCount > 3)
      {
        curCell.isAlive = false;
      }
    }
    else
    {
      if (liveCount == 3)
      {
        curCell.isAlive = true;
      }
    }
  }

  [ContextMenu("Random Set")]
  void RandomSet()
  {
    if (_cells != null)
    {
      Cell curCell;

      for (int x = 0; x < _cells.GetLength(0); ++x)
      {
        for (int y = 0; y < _cells.GetLength(1); ++y)
        {
          curCell = _cells[x, y];
          if (curCell != null)
          {
            curCell.isAlive = (Random.Range(0, int.MaxValue) % 2) == 1;

            if (!Application.isPlaying)
            {
              curCell.OnValidate();
            }
          }
        }
      }
    }
  }

  [ContextMenu("Generate Grid")]
  void GenerateGrid()
  {
    int childs = transform.childCount;
    for (int i = childs - 1; i >= 0; i--)
    {
      if (Application.isPlaying)
      {
        GameObject.Destroy(transform.GetChild(i).gameObject);
      }
      else
      {
        GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
      }
    }

    float aspect = Camera.main.aspect;
    float camHeight = Camera.main.orthographicSize * 2;

    float scale = camHeight / height;

    int width = (int)(height * aspect);

    _cells = new Cell[width, height];

    for (int x = 0; x < width; ++x)
    {
      for (int y = 0; y < height; ++y)
      {
        _cells[x, y] = Instantiate(cellPrefab) as Cell;

        _cells[x, y].transform.parent = this.transform; 
        _cells[x, y].name = "Cell [" + x + "," + y + "]";
        _cells[x, y].Initialize(true, new Vector2(x, y));

        Vector2 pos = new Vector2(x, y) - (new Vector2(width, height) / 2) + new Vector2(0.5f, 0.5f);

        pos *= scale;

        _cells[x, y].transform.localPosition = new Vector3(pos.x, pos.y, 1);
        _cells[x, y].transform.localScale = new Vector3(scale, scale, 1);
      }
    }
  }
}
