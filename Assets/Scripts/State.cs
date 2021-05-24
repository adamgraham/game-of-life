using UnityEngine;

[CreateAssetMenu(menuName = "Game of Life/State")]
public class State : ScriptableObject
{
    public Vector2Int[] cells = new Vector2Int[0];

    public Vector2Int GetCenter()
    {
        if (this.cells == null) {
            return Vector2Int.zero;
        }

        Vector2Int min = Vector2Int.zero;
        Vector2Int max = Vector2Int.zero;

        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector2Int cell = this.cells[i];
            min.x = Mathf.Min(min.x, cell.x);
            min.y = Mathf.Min(min.y, cell.y);
            max.x = Mathf.Max(max.x, cell.x);
            max.y = Mathf.Max(max.y, cell.y);
        }

        return new Vector2Int(
            (int)Mathf.Lerp(min.x, max.x, 0.5f),
            (int)Mathf.Lerp(min.y, max.y, 0.5f));
    }

}
