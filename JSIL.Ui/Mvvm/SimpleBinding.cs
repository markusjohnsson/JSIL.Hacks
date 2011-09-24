using System;

namespace JSIL.Ui.Mvvm
{
    public class SimpleBinding: IDisposable
    {
        private INotifyPropertyChanged _sourceHost;
        private object _targetHost;
        private string _targetProperty;
        private string _sourceProperty;

        public static IDisposable Create(object targetHost, string targetProperty, INotifyPropertyChanged sourceHost, string sourceProperty)
        {
            return new SimpleBinding(targetHost, targetProperty, sourceHost, sourceProperty);
        }

        public static IDisposable CreateTwoWay(INotifyPropertyChanged targetHost, string targetProperty, INotifyPropertyChanged sourceHost, string sourceProperty)
        {
            throw new NotImplementedException();
        }

        private SimpleBinding(object targetHost, string targetProperty, INotifyPropertyChanged sourceHost, string sourceProperty)
        {
            _sourceHost = sourceHost;
            _sourceProperty = sourceProperty;
            _targetHost = targetHost;
            _targetProperty = targetProperty;
            sourceHost.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            Verbatim.Expression(@"
                if (args.PropertyName == this._sourceProperty) {
                    this._targetHost[this._targetProperty] = this._sourceHost[this._sourceProperty];
                }
            ");
        }

        public void Dispose()
        {
            _sourceHost.PropertyChanged -= OnPropertyChanged;
        }
    }
}
