using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;

namespace JSIL.Ui
{
    public class Button: Element
    {
        public Button(): base("button")
        {
            Click += OnClick;
        }

        #region Command
        private ICommand _command;
        public ICommand Command
        {
            get
            {
                return _command;
            }
            set
            {
                if (_command != null)
                {
                    _command.CanExecuteChanged -= OnCanExecuteChanged;
                }

                _command = value;

                if (_command != null)
                {
                    _command.CanExecuteChanged += OnCanExecuteChanged;
                }

                OnCanExecuteChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        public object CommandParameter { get; set; }

        private void OnClick(object sender, EventArgs args)
        {
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }

        private void OnCanExecuteChanged(object sender, EventArgs args)
        {
            if (Command != null)
            {
                Enabled = Command.CanExecute(CommandParameter);
            }
            else
            {
                Enabled = true;
            }
        }
    }
}
