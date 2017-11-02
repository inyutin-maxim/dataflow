namespace DataFlow
{
    public class LifetimeDef
    {
        public Lifetime Lifetime { get; private set; }

        public LifetimeDef()
        {
            Lifetime = new Lifetime();
        }

        public void Terminate()
        {
            if (Lifetime.IsTerminated) return;

            while(Lifetime.Actions.Count > 0)
            {
                var index = Lifetime.Actions.Count - 1;
                Lifetime.Actions[index]();
            }

            Lifetime.Actions.Clear();
            Lifetime.IsTerminated = true;
        }
    }
}