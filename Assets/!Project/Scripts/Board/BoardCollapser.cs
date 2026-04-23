using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEditor.Progress;

namespace Game
{
    public class BoardCollapser
    {
        readonly PlayfieldBoard _board;

        public BoardCollapser(PlayfieldBoard board)
        {
            _board = board;
        }

        public async UniTask Collapse()
        {
            var size = _board.Size;
            var byTargetRow = new Dictionary<int, List<(int x, int fromY)>>();

            for (int x = 0; x < size.x; x++)
            {
                int writeY = 0;
                for (int readY = 0; readY < size.y; readY++)
                {
                    if (_board.Get(new Vector2Int(x, readY)) == null) continue;
                    if (readY != writeY)
                    {
                        if (!byTargetRow.ContainsKey(writeY)) byTargetRow[writeY] = new();
                        byTargetRow[writeY].Add((x, readY));


                    }
                    writeY++;
                }
            }

            foreach (var kvp in byTargetRow.OrderBy(k => k.Key))
            {
                foreach (var (x, fromY) in kvp.Value)
                {
                    var from = new Vector2Int(x, fromY);
                    var to = new Vector2Int(x, kvp.Key);
                    var item = _board.Get(from);
                    _board.Set(to, item);
                    _board.Clear(from);
                    item.OccupyCell(to, MoveAnimationType.Bounce);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(0.075f));
            }
        }
    }
}
