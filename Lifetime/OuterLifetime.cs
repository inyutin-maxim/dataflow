namespace System.Contracts
{
    public struct OuterLifetime
    {
        private readonly Lifetime _lifetime;

        private OuterLifetime(Lifetime lifetime)
        {
            _lifetime = lifetime;
        }

        internal Lifetime Lifetime => _lifetime;

        public bool IsTerminated
        {
            get
            {
                return _lifetime == null || _lifetime.IsTerminated;
            }
        }

        public static implicit operator OuterLifetime(Lifetime lifetime)
        {
            return new OuterLifetime(lifetime);
        }

        public static implicit operator OuterLifetime(LifetimeDef lifetime)
        {
            return new OuterLifetime(lifetime.Lifetime);
        }
    }
}