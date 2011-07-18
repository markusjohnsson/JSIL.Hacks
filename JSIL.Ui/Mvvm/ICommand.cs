using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSIL.Ui
{
    public interface ICommand
    {
        event EventHandler CanExecuteChanged;
        bool CanExecute(object o);
        void Execute(object o);
    }
}
