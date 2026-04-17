using UnityEngine;

public interface IMovable
{
    void StartDrag();
    void StopDrag();
    void MoveTo(Vector2Int targetCell);
}
