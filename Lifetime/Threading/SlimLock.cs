using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Contracts.Threading
{
    public struct SlimLock
    {
        private int _ownerThreadId, _depth;

        public bool TryEnter()
        {
            return TryEnter(Thread.CurrentThread.ManagedThreadId);
        }

        public void Enter()
        {
            var currentThreadId = Thread.CurrentThread.GetHashCode();
            while(!TryEnter(currentThreadId))
            {
                Thread.Yield();
            }
        }

        public void Leave()
        {
            var currentThreadId = Thread.CurrentThread.GetHashCode();
            if (_depth > 1 && _ownerThreadId == currentThreadId)
            {
                Interlocked.Decrement(ref _depth);
            } 
            if (_depth == 1 && Interlocked.CompareExchange(ref _ownerThreadId, 0, currentThreadId) != currentThreadId)
            {
                _depth = 0;
                throw new Exception("Trying to leave from not entered block");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryEnter(int currentThreadId)
        {
            if (Interlocked.CompareExchange(ref _ownerThreadId, currentThreadId, 0) == 0
                || _ownerThreadId == currentThreadId)
            {
                Interlocked.Increment(ref _depth);
                return true;
            }

            return false;
        }
    }
}
