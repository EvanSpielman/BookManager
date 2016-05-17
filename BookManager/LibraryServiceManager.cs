using System.Collections.Generic;
using System.Threading.Tasks;
using BookManager.LibraryServiceReference;

namespace BookManager
{
    public class LibraryServiceManager
    {
        private readonly LibraryServiceClient _libraryServiceClient;
        private readonly ModelFactory _modelFactory;

        public LibraryServiceManager()
        {
            _libraryServiceClient = new LibraryServiceClient();
            _modelFactory = new ModelFactory();
        }

        public async Task<List<BookViewModel>> GetBookViewModels()
        {
            var books = await _libraryServiceClient.GetAllBooksAsync();
            return _modelFactory.CreateBookViewModels(books);
        }

        public async Task<bool> DeleteBook(string title, string author)
        {
            return await _libraryServiceClient.DeleteBookAsync(title, author);
        }
    }
}
