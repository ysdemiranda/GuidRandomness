using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace GuidRandomness
{
    public class RelayCommand : ICommand
    {
        readonly Func<Boolean> _canExecute;
        readonly Action<object> _execute;

        public RelayCommand(Action<object> execute, Func<Boolean> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand implementation
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(Object parameter)
        {
            _execute(parameter);
        }

        #endregion
    }
}
