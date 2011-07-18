using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSIL.Ui.Mvvm
{
    public interface INotifyPropertyChanged
    {
        event PropertyChangedEventHandler PropertyChanged;
    }
}
