using System;
using System.Collections.Generic;

namespace DataFlow
{
  public class Lifetime
  {
      private readonly List<Action> _actions = new List<Action>();

      internal List<Action> Actions
      {
          get { return _actions; }
      }

      public static Lifetime Etheral = Lifetime.Define().Lifetime;

      public bool IsTerminated { get; internal set; }

      public Lifetime()
      {
          Add(() => IsTerminated = true);
      }

    public static LifetimeDef Define(Lifetime parent = null)
    {
      var def = new LifetimeDef();
        parent?.Add(() => def.Terminate());
        return def;
    }

    public void Add(Action action)
    {
      Actions.Add(action);
    }

    public Lifetime Intersect(Lifetime other)
    {
      var def = Lifetime.Define();
      other.Actions.Insert(0, () => { if (this.IsTerminated) def.Terminate(); });
      Actions.Insert(0, () => { if (other.IsTerminated) def.Terminate(); });
      return def.Lifetime;
    }
  }
}