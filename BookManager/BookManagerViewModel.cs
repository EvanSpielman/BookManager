using System;
using System.Collections.Generic;
using BookManager.LibraryServiceReference;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BookManager
{
    public class BookManagerViewModel : BaseViewModel
    {
        private readonly LibraryServiceManager _libraryServiceManager;
        private BookViewModel _selectedBook;
        private string _bookFilter;
        private ICollectionView _bookCollectionView;

        public BookManagerViewModel()
        {
            _libraryServiceManager = new LibraryServiceManager();
            RefreshBooks();
        }

        private bool CanClearSelection()
        {
            return SelectedBook != null && !IsBusy;
        }

        private void ClearSelection()
        {
            SelectedBook = null;
        }

        private bool CanDeleteBook()
        {
            return SelectedBook != null && !IsBusy;
        }

        private void DeleteBook()
        {
            NotifyDeleteBook = new NotifyTaskCompletion<bool>(
                _libraryServiceManager.DeleteBook(_selectedBook.Title, _selectedBook.Author), "DeleteBook", 
                null, 
                ClearSelectionCommand, 
                DeleteCommand);
            RefreshBooks();
        }

        public DelegateCommand DeleteCommand
        {
            get { return new DelegateCommand(o => DeleteBook(), o => CanDeleteBook()); }
        }

        public DelegateCommand ClearSelectionCommand
        {
            get { return new DelegateCommand(o => ClearSelection(), o => CanClearSelection()); }
        }

        public BookViewModel SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                _selectedBook = value;
                NotifyPropertyChanged("SelectedBook");
                DeleteCommand.RaiseCanExecuteChanged();
                ClearSelectionCommand.RaiseCanExecuteChanged();
            }
        }

        public string BookFilter
        {
            get { return _bookFilter; }
            set
            {
                _bookFilter = value;
                NotifyPropertyChanged("BookFilter");
                if (value.Length > 0)
                {
                    BookCollectionView.Filter = o => ((BookViewModel) o).Title.Contains(value);
                }
                else
                {
                    BookCollectionView.Filter = null;
                }
                
            }
        }

        public ICollectionView BookCollectionView
        {
            get { return _bookCollectionView; }
            set
            {
                _bookCollectionView = value;
                NotifyPropertyChanged("BookCollectionView");           
            }
        }

        public NotifyTaskCompletion<List<BookViewModel>> NotifyGetBookViewModels { get; set; }

        public NotifyTaskCompletion<bool> NotifyDeleteBook { get; set; }

        private void RefreshBooks()
        {
            NotifyGetBookViewModels = new NotifyTaskCompletion<List<BookViewModel>>(
                _libraryServiceManager.GetBookViewModels(),
                "GetAllBooks",
                books => BookCollectionView = CollectionViewSource.GetDefaultView(books),
                DeleteCommand,
                ClearSelectionCommand);
        }

        private bool IsBusy
        {
            get
            {
                var deleting = NotifyDeleteBook != null && NotifyDeleteBook.IsRunning;
                var refreshing = NotifyGetBookViewModels != null && NotifyGetBookViewModels.IsRunning;

                return deleting || refreshing;
            }
        }
    }
}