using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookManager.Annotations;

namespace BookManager
{
    /// <summary>
    ///     Implimentation of shared functionality of view models
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Notify when a specific property has been changed
        /// </summary>
        /// <param name="propertyName">Optional - specify the name of the property</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Fields

        private bool _isBusy;
        private string _status;

        #endregion

        #region Properties

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

        #endregion
    }
}