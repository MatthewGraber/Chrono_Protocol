using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    // Dimensions of the grid
    [SerializeField] private int _width, _height;
    public int width => _width;
    public int height => _height;
    public int centerX => _width / 2;
    public int centerY => _height / 2;

    [SerializeField] private Tile _plainsTile, _mountainTile;

    [SerializeField] private Transform _cam;


    private Dictionary<Vector2, Tile> _tiles;

    void Awake()
    {
        Instance = this;
        
    }


    // Generates the grid that will be played on
    public void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var randomTile = Random.Range(0, 10) == 3 ? _mountainTile : _plainsTile;
                var spawnedTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x}, {y}";

                bool isOffset = (!(x % 2 == y % 2));
                spawnedTile.Init(isOffset, x, y);


                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3(_width / 2.0f - 0.5f, _height / 2.0f - 0.5f, -10);

        GameManager.Instance.UpdateGameState(GameState.SpawnHeroes);
    }


    // Returns a tile at a specified position
    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }

        return null;
    }


    // Returns a tile that the heroes can spawn on
    public Tile GetHeroSpawnTile()
    {
        return _tiles.Where(t => ((t.Key.x > centerX - 3) && (t.Key.x < centerX + 3))
                                && ((t.Key.y > centerY - 3) && (t.Key.y < centerY + 3))
                                && t.Value.walkable).OrderBy(t => UnityEngine.Random.value).First().Value;
    }


    // Returns a tile that the enemies can spawn on
    public Tile GetEnemySpawnTile()
    {
        return _tiles.Where(t => (((t.Key.x > _width - 2) || (t.Key.x < 1)) 
                                || ((t.Key.y > _height - 2) || (t.Key.y < 1)))
                                && t.Value.walkable).OrderBy(t => UnityEngine.Random.value).First().Value;
    }


    // Returns true if the space is on the grid
    public bool onGrid(Vector2 pos)
    {
        if (pos.x < 0 || pos.y < 0)
            return false;
        if (pos.x >= _width || pos.y >= _height)
            return false;

        return true;
    }


    // Override
    public bool onGrid(int x, int y)
    {
        if (x < 0 || y < 0)
            return false;
        if (x >= _width || y >= _height)
            return false;

        return true;
    }


    // Returns a list of all spaces adjacent to the chosen vector
    public List<Vector2> AdjacentSpaces(Vector2 pos)
    {
        List<Vector2> adjacent = new List<Vector2>();
        adjacent.Add(new Vector2(pos.x + 1, pos.y));
        adjacent.Add(new Vector2(pos.x - 1, pos.y));
        adjacent.Add(new Vector2(pos.x, pos.y + 1));
        adjacent.Add(new Vector2(pos.x, pos.y - 1));

        adjacent.RemoveAll(s => onGrid(s) == false);
        return adjacent;
    }


    // Distance functions
    public int DistanceFromCenter(int x, int y)
    {
        return Mathf.Abs(centerX - x) + Mathf.Abs(centerY - y);
    }

    public int DistanceFromCenter(Vector2 pos)
    {
        return DistanceFromCenter((int) pos.x, (int) pos.y);
    }

    public void ClearAllHighlights()
    {
        foreach (var tile in _tiles)
        {
            tile.Value.ClearHighlights();
        }
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
