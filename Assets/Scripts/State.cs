using UnityEngine;

[CreateAssetMenu(menuName = "Game of Life/State")]
public class State : ScriptableObject
{
    public Vector2Int[] cells = new Vector2Int[0];
}
