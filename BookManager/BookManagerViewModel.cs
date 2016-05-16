using BookManager.LibraryServiceReference;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace BookManager
{
    public class BookManagerViewModel : BaseViewModel
    {
        private readonly LibraryServiceClient _client;
        private BookViewModel _selectedBook;
        private ObservableCollection<BookViewModel> _books;
        private string _bookFilter;

        public BookManagerViewModel()
        {
            _client = new LibraryServiceClient();
            RefreshBooks();
        }

        private bool CanClearSelection()
        {
            return SelectedBook != null;
        }

        private void ClearSelection()
        {
            SelectedBook = null;
        }

        private bool CanDeleteBook()
        {
            return SelectedBook != null;
        }

        private void DeleteBook()
        {
            _client.DeleteBook(_selectedBook.Title, _selectedBook.Author);
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
                    BookListView.Filter = o => ((BookViewModel) o).Title.Contains(value);
                }
                else
                {
                    BookListView.Filter = null;
                }
                
            }
        }

        public ICollectionView BookListView { get; set; }

        private void RefreshBooks()
        {
            var bookViewModels = (from book in _client.GetAllBooks()
                select CreateBookViewModel(book)).ToList();
            _books = new ObservableCollection<BookViewModel>(bookViewModels);
            BookListView = CollectionViewSource.GetDefaultView(_books);
        }

        private BookViewModel CreateBookViewModel(Book book)
        {
            return new BookViewModel
            {
                Title = book.Title,
                Author = book.Author,
                LibraryCode = book.LibraryCode
            };
        }
    }
}