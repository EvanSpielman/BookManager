using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookManager.LibraryServiceReference;

namespace BookManager
{
    public class ModelFactory
    {

        public List<BookViewModel> CreateBookViewModels(ICollection<Book> books)
        {
            return books.Select(CreateBookViewModel).ToList();
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
