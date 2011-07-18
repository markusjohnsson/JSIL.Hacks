using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSIL.Ui.Mvvm
{
    public class DelegateCommand<T>: ICommand where T: class
    {
        private Action<T> _executeAction;
        private Func<T, bool> _canExecuteFunc;

        public DelegateCommand(Action<T> executeAction, Func<T, bool> canExecuteFunc)
        {
            _executeAction = executeAction;
            _canExecuteFunc = canExecuteFunc;
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        public bool CanExecute(object o)
        {
            if (_canExecuteFunc != null)
            {
                return _canExecuteFunc((T)o);
            }
            else
            {
                return true;
            }
        }

        public void Execute(object o)
        {
            if (_executeAction != null)
            {
                _executeAction((T)o);
            }
        }
    }
}
