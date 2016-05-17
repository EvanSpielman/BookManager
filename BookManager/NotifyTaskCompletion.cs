using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BookManager
{
    public class NotifyTaskCompletion<T> : BaseViewModel
    {
        private bool _isRunning;
        private string _error;
        private readonly string _taskName;
        private readonly DelegateCommand[] _affectedCommands;
        private readonly Action<T> _afterCompletionAction;

        public NotifyTaskCompletion(Task<T> task, string taskName, Action<T> afterCompletionAction, params DelegateCommand[] affectedCommands)
        {
            Task = task;
            _taskName = taskName;
            _affectedCommands = affectedCommands;
            _afterCompletionAction = afterCompletionAction;

            if (!task.IsCompleted)
            {
                IsRunning = true;
                var result = WatchTaskAsync(task);
            }
        }

        public Task<T> Task { get; private set; }
    
        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }
            finally
            {
                IsRunning = false;
                if (task.IsFaulted)
                {
                    Error = $"The {_taskName} task encountered an error.";
                }
                
                foreach (var command in _affectedCommands)
                {
                    command.RaiseCanExecuteChanged();
                }

                _afterCompletionAction?.Invoke(Result);
            }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                NotifyPropertyChanged("IsRunning");
            }
        }

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                NotifyPropertyChanged("Error");
            }
        }

        public T Result
        {
            get
            {
                return Task.Status == TaskStatus.RanToCompletion ? Task.Result : default(T);
            }
        }
    }
}
