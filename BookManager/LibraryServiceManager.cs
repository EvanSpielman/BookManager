using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using BookManager.LibraryServiceReference;

namespace BookManager
{
    public class LibraryServiceManager : IDisposable
    {
        private readonly LibraryServiceClient _libraryServiceClient;
        private readonly ModelFactory _modelFactory;

        public LibraryServiceManager()
        {
            _libraryServiceClient = new LibraryServiceClient();
            _modelFactory = new ModelFactory();
        }

        public async Task<List<BookViewModel>> GetBookViewModels(Action beforeAction, Action afterAction, Action errorAction)
        {
            var books = await RunAsync(_libraryServiceClient.GetAllBooksAsync(), beforeAction, afterAction, errorAction);
            return _modelFactory.CreateBookViewModels(books);
        }
   
        public async Task<bool> DeleteBook(string title, string author, Action beforeAction, Action afterAction, Action errorAction)
        {
            return await RunAsync(_libraryServiceClient.DeleteBookAsync(title, author), beforeAction, afterAction, errorAction);
        }

        private async Task RunAsync(Task task, Action beforeAction, Action afterAction, Action errorAction)
        {
            beforeAction();
            await task;
            afterAction();

            if (task.IsFaulted)
            {
                errorAction();
            }

            CommandManager.InvalidateRequerySuggested();
        }

        private async Task<T> RunAsync<T>(Task<T> task, Action beforeAction, Action afterAction, Action errorAction)
        {
            beforeAction();
            var result = await task;
            afterAction();

            if (task.IsFaulted)
            {
                errorAction();
            }

            CommandManager.InvalidateRequerySuggested();

            return result;
        }

        public void Dispose()
        {
            _libraryServiceClient.Close();
        }
    }
}
