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
                var removalAction = Lifetime.Actions[Lifetime.Actions.Count - 1];
                removalAction();
                Lifetime.Actions.Remove(removalAction);
            }
            
            Lifetime.IsTerminated = true;
        }
    }
}