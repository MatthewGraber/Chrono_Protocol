using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
   
    [SerializeField] private Color _baseColor, offsetColor;
    [SerializeField] protected SpriteRenderer _renderer;            // Derived classes can also access this
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _moveHighlight;
    [SerializeField] private GameObject _attackHighlight;
    [SerializeField] private bool _isWalkable;
    [SerializeField] private bool _blocksAttacks;

    public string tileName;
    public BaseUnit OccupiedUnit;
    public bool walkable => _isWalkable && OccupiedUnit == null;
    public bool blocked => _blocksAttacks || OccupiedUnit != null;
    public Vector2 pos;

    public virtual void Init(bool isOffset, int x, int y)
    {
        _renderer.color = (isOffset ? offsetColor : _baseColor);
        pos = new Vector2(x, y);
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.State != GameState.PlayerTurn) return;

        // If tile is occupied
        if (OccupiedUnit != null)
        {
            // If hero is clicked, select it
            if (OccupiedUnit.player) UnitManager.Instance.SetSelectedUnit(OccupiedUnit);
            else
            {
                // If enemy is clicked
                if (UnitManager.Instance.SelectedUnit != null)
                {
                    var enemy = (BaseEnemy) OccupiedUnit;
                    
                    UnitManager.Instance.SelectedUnit.Attack(enemy);
                    UnitManager.Instance.SetSelectedUnit(null);
                }
            }
        }
        // If tile isn't occupied
        else
        {
            if (UnitManager.Instance.SelectedUnit != null)
            {
                if (UnitManager.Instance.SelectedUnit.availableSpaces.Contains(pos))
                {
                    UnitManager.Instance.SelectedUnit.Move(this);
                }
                UnitManager.Instance.SetSelectedUnit(null);

            }
        }
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit != null)
        {
            if (unit.OccupiedTile != null) { unit.OccupiedTile.OccupiedUnit = null; }

            unit.transform.position = transform.position;
            OccupiedUnit = unit;
            unit.OccupiedTile = this;
        }
        else
        {
            OccupiedUnit = null;
        }
    }

    public void MoveHighlight(bool high)
    {
        _moveHighlight.SetActive(high);
    }

    public void AttackHighlight(bool high)
    {
        _attackHighlight.SetActive(high);
    }

    public void ClearHighlights()
    {
        _attackHighlight.SetActive(false);
        _moveHighlight.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
