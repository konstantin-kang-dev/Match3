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
        private readonly BoardMutator _mutator;
        private readonly GridManager _gridManager;
        private readonly IBoard _board;
        private readonly IBoardContext _boardContext;
        private readonly PowerUpAnimator _powerUpAnimator;
        
        private readonly Subject<MatchResolvedEvent> _onMatchResolved = new();
        public Observable<MatchResolvedEvent> OnMatchResolved => _onMatchResolved.AsObservable();

        public MatchResolver(
            BoardMutator mutator,
            GridManager gridManager,
            IBoardContext boardContext,
            PowerUpAnimator powerUpAnimator,
            IBoard board)
        {
            _mutator = mutator;
            _gridManager = gridManager;
            _board = board;
            _boardContext =  boardContext;
            _powerUpAnimator = powerUpAnimator;
        }

        public async UniTask Resolve(List<MatchGroup> groups, Vector2Int? swapCell, int cascade)
        {
            EmitEvents(groups, cascade);

            var spawns = ResolveSpawns(groups, swapCell);
            var spawnsByGroup = MapSpawnsToGroups(groups, spawns);

            var mergeTasks = new List<UniTask>();
            var cellsForRegularDestroy = new List<Vector2Int>();

            foreach (var group in groups)
            {
                if (spawnsByGroup.TryGetValue(group, out var spawn))
                {
                    mergeTasks.Add(PlayMergeAndSpawn(group, spawn));
                }
                else
                {
                    cellsForRegularDestroy.AddRange(group.Cells);
                }
            }

            var destroyTask = _mutator.DestroyCells(cellsForRegularDestroy, _boardContext, DestroyMode.Animated, playVfx: true);

            await UniTask.WhenAll(UniTask.WhenAll(mergeTasks), destroyTask);
        }
        async UniTask PlayMergeAndSpawn(MatchGroup group, PowerUpSpawnPlan spawn)
        {
            var items = new List<PlayfieldItem>();
            foreach (var cell in group.Cells)
            {
                var item = _board.Get(cell);
                if (item != null) items.Add(item);
            }

            foreach (var cell in group.Cells)
                _board.Clear(cell);

            Vector2 targetPos = _gridManager.GetPositionForCell(spawn.Cell);
            await _powerUpAnimator.PlayMergeAnimation(items, targetPos);

            foreach (var item in items)
                item.DestroyItem(DestroyMode.Instant);

            ExecuteSpawn(spawn);
        }
        Dictionary<MatchGroup, PowerUpSpawnPlan> MapSpawnsToGroups(List<MatchGroup> groups, List<PowerUpSpawnPlan> spawns)
        {
            var map = new Dictionary<MatchGroup, PowerUpSpawnPlan>();
            foreach (var spawn in spawns)
            {
                foreach (var group in groups)
                {
                    if (group.Cells.Contains(spawn.Cell))
                    {
                        map[group] = spawn;
                        break;
                    }
                }
            }
            return map;
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