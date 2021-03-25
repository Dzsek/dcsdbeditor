using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace dcsdbeditor
{
    public class MyCommand : ICommand
    {
        private Action<object> _action;

        public event EventHandler CanExecuteChanged;

        public MyCommand(Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action.Invoke(parameter);
        }
    }
}
