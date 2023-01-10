using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _selectedHeroObject, _tileObject, _tileUnitObject, _roundCounterObject, _victoryDisplayObject;

    void Awake()
    {
        Instance = this;
    }

    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }

        _tileObject.GetComponentInChildren<UnityEngine.UI.Text>().text = tile.tileName;
        _tileObject.SetActive(true);

        if (tile.OccupiedUnit)
        {
            _tileUnitObject.GetComponentInChildren<UnityEngine.UI.Text>().text = tile.OccupiedUnit.GetInfo();
            _tileUnitObject.SetActive(true);
        }
    }

    public void ShowSelectedUnit(BaseUnit unit)
    {

        if (unit == null)
        {
            _selectedHeroObject.SetActive(false);
            return;
        }

        _selectedHeroObject.GetComponentInChildren<UnityEngine.UI.Text>().text = unit.GetInfo();
        _selectedHeroObject.SetActive(true);
    }

    public void UpdateRoundCounter(int turn)
    {
        _roundCounterObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Rounds remaining: " + turn;
    }

    public void showVictory(bool victory)
    {
        string win;
        if (victory) { win = "Victory!"; }
        else { win = "Defeat!"; }
        _victoryDisplayObject.GetComponentInChildren<UnityEngine.UI.Text>().text = win;
        _victoryDisplayObject.SetActive(true);
    }
}
