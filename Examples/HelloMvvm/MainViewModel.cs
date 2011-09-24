using System;
using System.Collections.Generic;
using System.Text;
using JSIL.Dom;
using JSIL.Ui;
using JSIL.Ui.Mvvm;

namespace HelloMvvm
{
    public class Post
    {
        public string Header { get; set; }
        public string Content { get; set; }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            _posts = new List<Post>();
        }

        private List<Post> _posts;
        public IEnumerable<Post> Posts
        {
            get
            {
                return _posts;
            }
        }

        #region HeaderInput

        private string _headerInput;
        public string HeaderInput
        {
            get { return _headerInput; }
            set
            {
                if (value == _headerInput) { return; }
                _headerInput = value;
                RaisePropertyChanged("HeaderInput");
            }
        }

        #endregion

        #region ContentInput

        private string _contentInput;
        public string ContentInput
        {
            get { return _contentInput; }
            set
            {
                if (value == _contentInput) { return; }
                _contentInput = value;
                RaisePropertyChanged("ContentInput");
            }
        }

        #endregion

        #region PostCommand

        private DelegateCommand _postCommand;
        public DelegateCommand PostCommand
        {
            get
            {
                if (_postCommand == null)
                {
                    _postCommand = new DelegateCommand(PostExecute, PostCanExecute);
                }
                return _postCommand;
            }
        }

        public bool PostCanExecute()
        {
            return true;
        }

        public void PostExecute()
        {
            _posts.Add(new Post { Header = HeaderInput, Content = ContentInput });
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
