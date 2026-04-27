using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using System.Linq;
using Game.Utils;
using UnityEngine;

namespace Game
{
    public class MatchResolver
    {
        readonly BoardMutator _mutator;
        readonly GridManager _gridManager;
        readonly IBoardContext _boardContext;
        
        readonly Subject<MatchResolvedEvent> _onMatchResolved = new();
        public Observable<MatchResolvedEvent> OnMatchResolved => _onMatchResolved.AsObservable();

        public MatchResolver(BoardMutator mutator, GridManager gridManager, IBoardContext boardContext)
        {
            _mutator = mutator;
            _gridManager = gridManager;
            _boardContext =  boardContext;
        }

        public async UniTask Resolve(List<MatchGroup> groups, Vector2Int? swapCell, int cascade)
        {
            EmitEvents(groups, cascade);

            var spawns = ResolveSpawns(groups, swapCell);
            var spawnCells = new HashSet<Vector2Int>(spawns.Select(s => s.Cell));

            foreach (var spawn in spawns)
                ExecuteSpawn(spawn);

            var cellsToDestroy = new List<Vector2Int>();
            foreach (var group in groups)
                foreach (var cell in group.Cells)
                    if (!spawnCells.Contains(cell))
                        cellsToDestroy.Add(cell);

            await _mutator.DestroyCells(cellsToDestroy, _boardContext);
        }

        void EmitEvents(List<MatchGroup> groups, int cascade)
        {
            foreach (var group in groups)
            {
                _onMatchResolved.OnNext(new MatchResolvedEvent(
                    cells: group.Cells,
                    color: group.Color,
                    shape: group.Shape,
                    cascadeLevel: cascade,
                    worldCenter: ComputeCenter(group.Cells)
                ));
            }
        }

        List<PowerUpSpawnPlan> ResolveSpawns(List<MatchGroup> groups, Vector2Int? swapCell)
        {
            var spawns = new List<PowerUpSpawnPlan>();
            foreach (var group in groups)
            {
                var kind = GetPowerUpKindForShape(group.Shape);
                if (kind == null) continue;
                var cell = GetSpawnCell(group, swapCell);
                spawns.Add(new PowerUpSpawnPlan
                {
                    Cell = cell,
                    Kind = kind.Value,
                    SourceShape = group.Shape
                });
            }
            return spawns;
        }

        void ExecuteSpawn(PowerUpSpawnPlan plan)
        {
            switch (plan.Kind)
            {
                case PlayfieldItemKind.Rocket:
                    _mutator.SpawnRocketAt(plan.Cell, GetRocketOrientation(plan.SourceShape));
                    break;
                case PlayfieldItemKind.Bomb:
                    _mutator.SpawnBombAt(plan.Cell);
                    break;
                case PlayfieldItemKind.Plane:
                    _mutator.SpawnPlaneAt(plan.Cell);
                    break;
                case PlayfieldItemKind.Disco:
                    _mutator.SpawnDiscoAt(plan.Cell);
                    break;
            }
        }

        PlayfieldItemKind? GetPowerUpKindForShape(MatchShape shape) => shape switch
        {
            MatchShape.Match4Horizontal => PlayfieldItemKind.Rocket,
            MatchShape.Match4Vertical => PlayfieldItemKind.Rocket,
            MatchShape.Match4Square => PlayfieldItemKind.Plane,
            MatchShape.Match5Line => PlayfieldItemKind.Disco,
            MatchShape.Match5LT => PlayfieldItemKind.Bomb,
            _ => null
        };

        RocketOrientation GetRocketOrientation(MatchShape shape) => shape switch
        {
            MatchShape.Match4Horizontal => RocketOrientation.Vertical,
            MatchShape.Match4Vertical => RocketOrientation.Horizontal,
            _ => RocketOrientation.Horizontal
        };

        Vector2Int GetSpawnCell(MatchGroup group, Vector2Int? swapCell)
        {
            if (swapCell.HasValue && group.Cells.Contains(swapCell.Value))
                return swapCell.Value;

            return group.Shape switch
            {
                MatchShape.Match5LT => CellGeometry.GetIntersection(group.Cells),
                MatchShape.Match4Square => CellGeometry.GetBottomLeft(group.Cells),
                _ => CellGeometry.GetCenter(group.Cells)
            };
        }

        Vector2 ComputeCenter(IReadOnlyList<Vector2Int> cells)
        {
            Vector2 sum = Vector2.zero;
            foreach (var c in cells)
                sum += _gridManager.GetPositionForCell(c);
            return sum / cells.Count;
        }
    }
}