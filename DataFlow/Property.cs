using System.ComponentModel;
// ReSharper disable UseNullPropagation

namespace DataFlow
{
    public class Property<T> : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private readonly string _name;
        private T _value;

        private readonly ISignal<T> _changed;

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        public T Value
        {
            get { return _value; }
            set {
                if (!value.Equals(_value))
                {
                    OnPropertyChanging();

                    _value = value;
                    _changed.Fire(value);

                    OnPropertyChanged();
                }
            }
        }

        public string Name => _name;

        public ISource<T> Changed
        {
            get { return _changed; }
        }

        public Property(Lifetime lf, string name = null)
        {
            _name = name;
            _changed = new Signal<T>(lf);
        }

        public static implicit operator T(Property<T> v)
        {
            return v.Value;
        }

        protected virtual void OnPropertyChanged()
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        protected virtual void OnPropertyChanging()
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(nameof(Value)));
            }
        }
    }
}
