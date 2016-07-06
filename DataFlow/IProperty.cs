using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFlow
{
    public class Property<T>
    {
        private T _value;

        private readonly ISignal<T> _changed;

        public T Value
        {
            get { return _value; }
            set {
                if (!value.Equals(_value))
                {
                    _value = value;
                    _changed.Fire(value);
                }
            }
        }

        public ISource<T> Changed
        {
            get { return _changed; }
        }

        public Property(Lifetime lf)
        {
            _changed = new Signal<T>(lf);
        }

        public static implicit operator T(Property<T> v)
        {
            return v.Value;
        }                    
    }
}
