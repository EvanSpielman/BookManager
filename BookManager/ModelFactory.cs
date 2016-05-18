using System.Collections.Generic;
using System.Linq;
using BookManager.LibraryServiceReference;

namespace BookManager
{
    /// <summary>
    /// Class to allow for converting domain objects from the web service into view models
    /// </summary>
    public class ModelFactory
    {
        #region Public Methods

        /// <summary>
        /// Create book view models from a collection of book domain objects
        /// </summary>
        /// <param name="books">A collection of book domain objects</param>
        /// <returns>A list of book view models</returns>
        public List<BookViewModel> CreateBookViewModels(ICollection<Book> books)
        {
            return books?.Select(CreateBookViewModel).ToList() ?? new List<BookViewModel>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a book view model from a book domain object
        /// </summary>
        /// <param name="book">A book domain object</param>
        /// <returns>A book view model</returns>
        private BookViewModel CreateBookViewModel(Book book)
        {
            return new BookViewModel
            {
                Title = book.Title,
                Author = book.Author,
                LibraryCode = book.LibraryCode
            };
        }

        #endregion
    }
}