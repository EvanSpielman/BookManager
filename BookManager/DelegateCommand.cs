using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManager
{
    /// <summary>
    /// A shared implimentation of a Command that takes delegates for Execute and CanExecute,
    /// and allows for manually notifying when CanExecute has changed
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region Fields

        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        #endregion

        #region Constructor

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region Events

        /// <summary>
        /// Hook into the existing CommandManager.RequerySuggested event,
        /// so the DelegateCommand's CanExecute is updated
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }

            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion

        #region Public Methods

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion
    }
}