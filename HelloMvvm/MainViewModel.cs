using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;
using JSIL.Ui;
using JSIL.Ui.Mvvm;

namespace HelloMvvm
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            _posts = new List<string>();
        }

        private List<string> _posts;
        public IEnumerable<string> Posts
        {
            get
            {
                return _posts;
            }
        }

        #region PostCommand

        private DelegateCommand<string> _postCommand;
        public DelegateCommand<string> PostCommand
        {
            get
            {
                if (_postCommand == null)
                {
                    _postCommand = new DelegateCommand<string>(PostExecute, PostCanExecute);
                }
                return _postCommand;
            }
        }

        public bool PostCanExecute(string param)
        {
            return true;
        }

        public void PostExecute(string param)
        {
            _posts.Add(param);
            RaisePropertyChanged("Posts");
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
