using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    private Texture2D _texture;
    private bool[,] _grid;
    private bool[,] _next;

    [Header("Configuration")]

    public Vector2Int size = new Vector2Int(512, 512);
    public State initialState;
    public Color aliveColor = Color.white;
    public Color deadColor = Color.black;

    [Header("Generation Info")]

    [SerializeField]
    private int _population;
    public int population => _population;

    [SerializeField]
    private int _iterations;
    public int iterations => _iterations;

    private void Start()
    {
        this.transform.localScale = new Vector3(Mathf.Sqrt(this.size.x), Mathf.Sqrt(this.size.y), 1.0f);

        _grid = new bool[this.size.x, this.size.y];
        _next = new bool[this.size.x, this.size.y];

        _texture = new Texture2D(this.size.x, this.size.y);
        _texture.filterMode = FilterMode.Point;
        _texture.wrapMode = TextureWrapMode.Clamp;

        Camera.main.backgroundColor = this.deadColor;
        GetComponent<Renderer>().material.mainTexture = _texture;

        for (int x = 0; x < this.size.x; x++)
        {
            for (int y = 0; y < this.size.y; y++)
            {
                _texture.SetPixel(x, y, this.deadColor);
            }
        }

        if (this.initialState != null)
        {
            _population = this.initialState.cells.Length;

            Vector2Int center = this.size / 2;

            for (int i = 0; i < _population; i++)
            {
                Vector2Int cell = this.initialState.cells[i];
                cell += center;

                _grid[cell.x, cell.y] = true;
                _texture.SetPixel(cell.x, cell.y, this.aliveColor);
            }
        }

        _texture.Apply();
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

        _texture.Apply();
        _iterations++;
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
            _texture.SetPixel(x, y, this.aliveColor);
            _population++;
        }
        else if (alive && (neighbors < 2 || neighbors > 3))
        {
            _next[x, y] = false;
            _texture.SetPixel(x, y, this.deadColor);
            _population--;
        }
        else
        {
            _next[x, y] = alive;
        }
    }

}
