using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookManager.Annotations;

namespace BookManager
{
    /// <summary>
    /// Implimentation of shared functionality of view models
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _status;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsBusy
        {
            get { return _isBusy; }

            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Status
        {
            get { return _status; }

            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }
    }
}
