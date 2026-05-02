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
        private readonly BoardState _board;
        private readonly IBoardContext _boardContext;
        private readonly PowerUpAnimator _powerUpAnimator;
        private readonly BoardActivityTracker _tracker;

        private readonly Subject<MatchResolvedEvent> _onMatchResolved = new();
        public Observable<MatchResolvedEvent> OnMatchResolved => _onMatchResolved.AsObservable();

        public MatchResolver(
            BoardMutator mutator,
            GridManager gridManager,
            IBoardContext boardContext,
            PowerUpAnimator powerUpAnimator,
            BoardState board,
            BoardActivityTracker tracker)
        {
            _mutator = mutator;
            _gridManager = gridManager;
            _board = board;
            _boardContext = boardContext;
            _powerUpAnimator = powerUpAnimator;
            _tracker = tracker;
        }

        public async UniTask Resolve(List<MatchGroup> groups, Vector2Int? swapCell, int cascade)
        {
            using (_tracker.BeginActivity())
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
        }

        async UniTask PlayMergeAndSpawn(MatchGroup group, PowerUpSpawnPlan spawn)
        {
            var items = new List<IBoardItem>();
            var views = new List<PlayfieldItemView>();
            var slots = new List<CellSlot>();
            foreach (var cell in group.Cells)
            {
                var slot = _board.Get(cell);
                if (slot.Item != null)
                {
                    items.Add(slot.Item);
                    views.Add(((PlayfieldItem)slot.Item).View);
                }
                slots.Add(slot);
                slot.SetDestroying();
            }

            Vector2 targetPos = _gridManager.GetPositionForCell(spawn.Cell);
            await _powerUpAnimator.PlayMergeAnimation(views, targetPos);

            foreach (var item in items)
                item.DestroyItem(DestroyMode.Instant);

            foreach (var slot in slots)
                slot.ClearItem();

            ExecuteSpawn(spawn);

            foreach (var slot in slots)
            {
                if (slot.Position == spawn.Cell) continue;
                slot.SetEmpty();
            }
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
                case PlayfieldItemKind.Balloon:
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
            MatchShape.Match4Square => PlayfieldItemKind.Balloon,
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
            // Swap-cell приоритетен: если игрок свапнул фишку и она часть подформы — спавн там.
            if (swapCell.HasValue && group.ShapeCells.Contains(swapCell.Value))
                return swapCell.Value;

            return group.Shape switch
            {
                MatchShape.Match5LT => CellGeometry.GetIntersection(group.ShapeCells),
                MatchShape.Match4Square => CellGeometry.GetBottomLeft(group.ShapeCells),
                _ => CellGeometry.GetCenter(group.ShapeCells)
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