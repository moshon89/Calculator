using System;
using System.Windows.Input;

namespace Calculator.Commands
{
    class DelegateCommand<T> : ICommand
    {
        private Action<T> _execute; //the action we want to execute 
        private Func<bool> _canExecute; //to check if the command cannot be executed for some reason

        /// <summary>
        /// we have 2 classes "DelegateCommand" one is Generic for the command could get parameters
        /// end the second is for commands with no parameters
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public DelegateCommand(Action<T> execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute();
            }

            //in case it's null we want it to ran anyway
            return true;
        }

        public void Execute(object parameter)
        {
            //no need to call canExecute, if canExecute return false then Execute() will not be called
            _execute((T)parameter);
        }

        /// <summary>
        /// notify that can execute may be changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;

            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }

    class DelegateCommand : ICommand
    {
        private Action _execute;
        private Func<bool> _canExecute;

        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute();
            }

            //in case it's null we want it to ran anyway
            return true;
        }

        public void Execute(object parameter)
        {
            //no need to call canExecute, if canExecute return false then Execute() will not be called
            _execute();
        }

        /// <summary>
        /// notify that can execute may be changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;

            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
