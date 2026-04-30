using R3;
using System;

namespace Game
{
    
    
    
    
    
    
    
    
    
    /// доезжают до следующей клетки, новые не запускаются).
    /// 
    /// При Unfreeze эмитится OnUnfrozen — это сигнал для ColumnsCoordinator
    /// перепроверить колонки. Без этого Empty-события, случившиеся под freeze,
    /// останутся необработанными, и колонки зависнут с пустотами внутри.
    /// </summary>
    public class BoardActivityTracker
    {
        int _activeCount;
        int _freezeCount;

        readonly Subject<bool> _onIdleChanged = new();
        public Observable<bool> OnIdleChanged => _onIdleChanged.AsObservable();

        readonly Subject<Unit> _onUnfrozen = new();
        public Observable<Unit> OnUnfrozen => _onUnfrozen.AsObservable();

        public bool IsIdle => _activeCount == 0;
        public bool IsFrozen => _freezeCount > 0;

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

        public void Freeze() => _freezeCount++;

        public void Unfreeze()
        {
            if (_freezeCount <= 0) 
                throw new InvalidOperationException("Unfreeze without Freeze");
            _freezeCount--;
            if (_freezeCount == 0)
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