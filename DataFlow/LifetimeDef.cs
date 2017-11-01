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
            for(var i = Lifetime.Actions.Count - 1; i >= 0; i--)
            {
                Lifetime.Actions[i]();
            }

            Lifetime.Actions.Clear();
            Lifetime.IsTerminated = true;
        }
    }
}