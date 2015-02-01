using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider))]
public class Cell : MonoBehaviour
{
  #region Fields & Properties

  [SerializeField]
  bool _isAlive = false;

  bool _nextState = false;

  public bool isAlive
  {
    get { return _isAlive;}
    set
    {
      _nextState = value;
    }
  }

  float _timeSinceLastTick = 0;

  [SerializeField, HideInInspector]
  Vector2 _gridLocation = new Vector2(float.NaN, float.NaN);
  public Vector2 gridLocation
  {
    get { return _gridLocation;  }
  }

  SpriteRenderer _renderer = null;
  #endregion

  public void Initialize(bool active, Vector2 gridLocation)
  {
    isAlive = active;
    _gridLocation = gridLocation;
  }

  void Awake()
  {
    _renderer = this.GetComponent<SpriteRenderer>();

    isAlive = isAlive;
  }

  void Update () 
  {
    _timeSinceLastTick += Time.deltaTime * Time.timeScale;
    if (_timeSinceLastTick > CellManager.instance.tickTime)
    {
      _timeSinceLastTick = _timeSinceLastTick - CellManager.instance.tickTime;

      CellManager.instance.CalculateLife(this);
    }
  }

  void LateUpdate()
  {
    if (_nextState != _isAlive)
    {
      _isAlive = _nextState;

      if (_renderer != null)
      {
        _renderer.enabled = _isAlive;
      }
    }
  }

  public void OnValidate()
  {
    if (_renderer == null)
    {
      _renderer = this.GetComponent<SpriteRenderer>();
    }

    isAlive = isAlive;

    _renderer.enabled = _isAlive;
  }
}
