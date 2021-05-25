using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    [System.Serializable]
    public struct Generation
    {
        [ReadOnly]
        public int population;

        [ReadOnly]
        public int iterations;

        [ReadOnly]
        public int time;
    }

    private Texture2D _texture;
    private bool[,] _grid;
    private bool[,] _next;

    public Vector2Int size = new Vector2Int(512, 512);
    public State pattern;
    public Color aliveColor = Color.white;
    public Color deadColor = Color.black;

    [SerializeField]
    private Generation _info;
    public Generation info => _info;

    private void OnValidate()
    {
        Configure();
    }

    private void Awake()
    {
        Configure();
    }

    private void Configure()
    {
        Preconfiguration();
        CreateTexture();
        ClearAllCells();
        SetPattern(this.pattern);
    }

    private void Preconfiguration()
    {
        Camera.main.backgroundColor = this.deadColor;

        this.transform.localScale = new Vector3(Mathf.Sqrt(this.size.x), Mathf.Sqrt(this.size.y), 1.0f);

        _grid = new bool[this.size.x, this.size.y];
        _next = new bool[this.size.x, this.size.y];
    }

    private void CreateTexture()
    {
        if (_texture == null || _texture.width != this.size.x || _texture.height != this.size.y)
        {
            _texture = new Texture2D(this.size.x, this.size.y);
            _texture.filterMode = FilterMode.Point;
            _texture.wrapMode = TextureWrapMode.Clamp;
        }

        if (Application.isPlaying) {
            GetComponent<Renderer>().material.mainTexture = _texture;
        } else {
            GetComponent<Renderer>().sharedMaterial.mainTexture = _texture;
        }
    }

    private void ClearAllCells()
    {
        for (int x = 0; x < this.size.x; x++)
        {
            for (int y = 0; y < this.size.y; y++)
            {
                _texture.SetPixel(x, y, this.deadColor);
                _grid[x, y] = false;
            }
        }

        _info.population = 0;
        _info.iterations = 0;
    }

    private void SetPattern(State pattern)
    {
        if (pattern == null) {
            return;
        }

        _info.population = pattern.cells.Length;
        _info.iterations = 0;

        Vector2Int center = this.size / 2;
        center -= pattern.GetCenter();

        for (int i = 0; i < _info.population; i++)
        {
            Vector2Int cell = pattern.cells[i];
            cell += center;

            _grid[cell.x, cell.y] = true;
            _texture.SetPixel(cell.x, cell.y, this.aliveColor);
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
        _info.iterations++;
        _info.time = Mathf.RoundToInt(_info.iterations * Time.fixedDeltaTime);
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
            _info.population++;
        }
        else if (alive && (neighbors < 2 || neighbors > 3))
        {
            _next[x, y] = false;
            _texture.SetPixel(x, y, this.deadColor);
            _info.population--;
        }
        else
        {
            _next[x, y] = alive;
        }
    }

}
