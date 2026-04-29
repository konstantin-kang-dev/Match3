using R3;
using System;

namespace Game
{
    /// <summary>
    /// Отслеживает занятость доски и состояние заморозки.
    /// 
    /// Активность — любая асинхронная операция, которая модифицирует доску:
    /// падение фишки, активация PowerUp'а, цикл MatchResolver.Resolve.
    /// Доска IsIdle = true когда все операции завершены.
    /// 
    /// Freeze — отдельный флаг для остановки колонок при активации PowerUp'ов
    /// (Homescapes-поведение: Disco замораживает доску, существующие падения
    /// доезжают до следующей клетки, новые не запускаются).
    /// 
    /// При Unfreeze эмитится OnUnfrozen — это сигнал для ColumnsCoordinator
    /// перепроверить колонки. Без этого Empty-события, случившиеся под freeze,
    /// останутся необработанными, и колонки зависнут с пустотами внутри.
    /// </summary>
    public class BoardActivityTracker
    {
        int _activeCount;
        bool _isFrozen;

        readonly Subject<bool> _onIdleChanged = new();
        public Observable<bool> OnIdleChanged => _onIdleChanged.AsObservable();

        readonly Subject<Unit> _onUnfrozen = new();
        public Observable<Unit> OnUnfrozen => _onUnfrozen.AsObservable();

        public bool IsIdle => _activeCount == 0;
        public bool IsFrozen => _isFrozen;

        public IDisposable BeginActivity()
        {
            bool wasIdle = IsIdle;
            _activeCount++;
            if (wasIdle) _onIdleChanged.OnNext(false);
            return new ActivityScope(this);
        }

        void EndActivity()
        {
            if (_activeCount <= 0)
            {
                throw new InvalidOperationException(
                    "BoardActivityTracker: EndActivity called with no active operations.");
            }

            _activeCount--;
            if (IsIdle) _onIdleChanged.OnNext(true);
        }

        public void Freeze() => _isFrozen = true;

        public void Unfreeze()
        {
            if (!_isFrozen) return;
            _isFrozen = false;
            _onUnfrozen.OnNext(Unit.Default);
        }

        readonly struct ActivityScope : IDisposable
        {
            readonly BoardActivityTracker _tracker;
            public ActivityScope(BoardActivityTracker tracker) => _tracker = tracker;
            public void Dispose() => _tracker.EndActivity();
        }
    }
}