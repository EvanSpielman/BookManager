using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace BookManager
{
    /// <summary>
    ///     The BookManagerViewModel handles interactions between the BookManagerView and the
    ///     web service.
    /// </summary>
    public class BookManagerViewModel : BaseViewModel, IDisposable
    {
        #region Constructor

        /// <summary>
        ///     Constructs a new BookManagerViewModel
        /// </summary>
        public BookManagerViewModel()
        {
            _libraryServiceManager = new LibraryServiceManager();
            RefreshBooks();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Properly dispose of the BookManagerViewModel
        /// </summary>
        public void Dispose()
        {
            BookCollectionView.Filter = null;
            _libraryServiceManager.Dispose();
        }

        #endregion

        #region Finalizer

        /// <summary>
        ///     Finalizes the BookManagerViewModel
        /// </summary>
        ~BookManagerViewModel()
        {
            Dispose();
        }

        #endregion

        #region Fields

        private readonly LibraryServiceManager _libraryServiceManager;
        private BookViewModel _selectedBook;
        private string _bookFilter;
        private ICollectionView _bookCollectionView;

        #endregion

        #region Commands

        /// <summary>
        ///     Command used to delete books
        /// </summary>
        public DelegateCommand DeleteCommand
        {
            get { return new DelegateCommand(o => DeleteBook(), o => CanDeleteBook()); }
        }

        /// <summary>
        ///     Command used to clear the currently selected book
        /// </summary>
        public DelegateCommand ClearSelectionCommand
        {
            get { return new DelegateCommand(o => ClearSelection(), o => CanClearSelection()); }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     The currently selected book on the grid
        /// </summary>
        public BookViewModel SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        ///     The filtering to apply to the grid
        /// </summary>
        public string BookFilter
        {
            get { return _bookFilter; }
            set
            {
                _bookFilter = value;
                OnPropertyChanged();
                SetFilter();
            }
        }

        /// <summary>
        ///     The collection of books displayed by the grid
        /// </summary>
        public ICollectionView BookCollectionView
        {
            get { return _bookCollectionView; }
            set
            {
                _bookCollectionView = value;
                OnPropertyChanged();
                SetFilter();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Check whether the current selection can be cleared
        /// </summary>
        /// <returns>True if a book is selected, and no process are running. Otherwise, false.</returns>
        private bool CanClearSelection()
        {
            return SelectedBook != null && !IsBusy;
        }

        /// <summary>
        ///     Clear the currently selected book
        /// </summary>
        private void ClearSelection()
        {
            SelectedBook = null;
        }

        /// <summary>
        ///     Check whether the currently selected book can be deleted
        /// </summary>
        /// <returns>True if a book is selected, and no process are running. Otherwise, false.</returns>
        private bool CanDeleteBook()
        {
            return SelectedBook != null && !IsBusy;
        }

        /// <summary>
        ///     Delete the currently selected book asynchronously.
        /// </summary>
        private async void DeleteBook()
        {
            var title = SelectedBook.Title;
            var author = SelectedBook.Author;

            await _libraryServiceManager.DeleteBook(title, author,
                () =>
                {
                    IsBusy = true;
                    Status = $"Deleting book '{title}' by '{author}'";
                }, () =>
                {
                    IsBusy = false;
                    Status = null;
                },
                () => { MessageBox.Show("An error occurred while deleting a book."); });
            RefreshBooks();
        }

        /// <summary>
        ///     Get an updated list of books from the service asynchronously
        /// </summary>
        private async void RefreshBooks()
        {
            var books = await _libraryServiceManager.GetBookViewModels(() =>
            {
                IsBusy = true;
                Status = "Refreshing books";
            }, () =>
            {
                IsBusy = false;
                Status = null;
            },
                () => { MessageBox.Show("An error occurred while refreshing books."); });

            BookCollectionView = CollectionViewSource.GetDefaultView(books);
        }

        /// <summary>
        ///     Set the filter on the book collection
        /// </summary>
        private void SetFilter()
        {
            if (!string.IsNullOrEmpty(_bookFilter))
            {
                BookCollectionView.Filter = o => ((BookViewModel) o).Title.Contains(_bookFilter);
            }
            else
            {
                BookCollectionView.Filter = null;
            }
        }

        #endregion
    }
}