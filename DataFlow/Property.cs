using System.ComponentModel;
// ReSharper disable UseNullPropagation

namespace DataFlow
{
    public class Property<T> : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private T _value;

        private readonly ISourceAdapter<T> _changed;

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        public virtual T Value
        {
            get => _value;
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

        public string Name { get; }

        public ITarget<T> Changed => _changed;

        protected Property(Lifetime lf, string name = null)
        {
            Name = name;
            _changed = new SourceAdapter<T>(lf);
        }

        public static Property<T> Create(Lifetime lf, string name = null)
        {
            return new Property<T>(lf, name);
        }

        public static implicit operator T(Property<T> v)
        {
            return v.Value;
        }

        protected virtual void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        protected virtual void OnPropertyChanging()
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(Name));
        }
    }
}
