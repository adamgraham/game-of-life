using UnityEngine;
using UnityEngine.Tilemaps;

public class GameOfLife : MonoBehaviour
{
    private Tilemap _tilemap;
    private bool[,] _grid;
    private bool[,] _next;

    public Vector2Int size = new Vector2Int(512, 512);
    public State pattern;
    public Tile aliveTile;

    private void Awake()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        _tilemap.ClearAllTiles();

        _grid = new bool[this.size.x, this.size.y];
        _next = new bool[this.size.x, this.size.y];

        SetPattern(this.pattern);
    }

    private void SetPattern(State pattern)
    {
        Vector2Int center = this.size / 2;
        center -= pattern.GetCenter();

        for (int i = 0; i < pattern.cells.Length; i++)
        {
            Vector2Int cell = pattern.cells[i];
            cell += center;

            _grid[cell.x, cell.y] = true;
            _tilemap.SetTile(GetTilePosition(cell.x, cell.y), this.aliveTile);
        }
    }

    private void FixedUpdate()
    {
        for (int x = 1; x < this.size.x - 1; x++)
        {
            for (int y = 1; y < this.size.y - 1; y++)
            {
                bool alive = _grid[x, y];
                int neighbors = CountNeighbors(x, y, alive);
                Transition(x, y, alive, neighbors);
            }
        }

        for (int x = 0; x < this.size.x; x++)
        {
            for (int y = 0; y < this.size.y; y++)
            {
                _grid[x, y] = _next[x, y];
            }
        }
    }

    private int CountNeighbors(int x, int y, bool alive)
    {
        int neighbors = 0;

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (_grid[x + i, y + j]) {
                    neighbors++;
                }
            }
        }

        return alive ? neighbors - 1 : neighbors;
    }

    private void Transition(int x, int y, bool alive, int neighbors)
    {
        if (!alive && neighbors == 3)
        {
            _next[x, y] = true;
            _tilemap.SetTile(GetTilePosition(x, y), this.aliveTile);
        }
        else if (alive && (neighbors < 2 || neighbors > 3))
        {
            _next[x, y] = false;
            _tilemap.SetTile(GetTilePosition(x, y), null);
        }
        else
        {
            _next[x, y] = alive;
        }
    }

    private Vector3Int GetTilePosition(int x, int y)
    {
        return new Vector3Int(
            x - (this.size.x / 2),
            y - (this.size.y / 2), 0);
    }

}
