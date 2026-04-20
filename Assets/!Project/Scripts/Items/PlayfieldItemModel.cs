using R3;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game
{
    public class PlayfieldItemModel
    {
        public PlayfieldItemType Type { get; private set; }
        public Vector2Int OccupiedCell { get; private set; }

        public void Init(PlayfieldItemConfig config)
        {
            Type = config.ItemType;
        }

        public void OccupyCell(Vector2Int cell)
        {
            OccupiedCell = cell;
        }

    }
}