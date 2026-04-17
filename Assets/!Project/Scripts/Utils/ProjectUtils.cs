using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Utils
{
    public static class ProjectUtils
    {
        public static Vector2Int GetSwipeDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                return delta.x > 0 ? Vector2Int.right : Vector2Int.left;
            else
                return delta.y > 0 ? Vector2Int.up : Vector2Int.down;
        }
    }
}
