using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using BookManager.LibraryServiceReference;

namespace BookManager
{
    /// <summary>
    /// Manager class which wraps the LibraryServiceClient and provides additional functionality
    /// </summary>
    public class LibraryServiceManager : IDisposable
    {
        #region Fields

        private readonly LibraryServiceClient _libraryServiceClient;
        private readonly ModelFactory _modelFactory;

        #endregion

        #region Constructor

        public LibraryServiceManager()
        {
            _libraryServiceClient = new LibraryServiceClient();
            _modelFactory = new ModelFactory();
        }

        #endregion

        #region Finalizer

        /// <summary>
        /// Finalize the LibraryServiceManager
        /// </summary>
        ~LibraryServiceManager()
        {
            Dispose();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Async call to get the book view models
        /// </summary>
        /// <param name="beforeAction">Action to perform before beginning the call to the web service</param>
        /// <param name="afterAction">Action to perform after completing the call to the web service</param>
        /// <param name="errorAction">Action to perform if the web service encounters an error</param>
        /// <returns>The list of book view models</returns>
        public async Task<List<BookViewModel>> GetBookViewModels(Action beforeAction, Action afterAction,
            Action errorAction)
        {
            var books = await RunAsync(_libraryServiceClient.GetAllBooksAsync(), beforeAction, afterAction, errorAction);
            return _modelFactory.CreateBookViewModels(books);
        }

        /// <summary>
        /// Async call to delete a book from the web service
        /// </summary>
        /// <param name="beforeAction">Action to perform before beginning the call to the web service</param>
        /// <param name="afterAction">Action to perform after completing the call to the web service</param>
        /// <param name="errorAction">Action to perform if the web service encounters an error</param>
        /// <returns>The result of the deletion - true meaning the delete was successful.</returns>
        public async Task<bool> DeleteBook(string title, string author, Action beforeAction, Action afterAction,
            Action errorAction)
        {
            return
                await
                    RunAsync(_libraryServiceClient.DeleteBookAsync(title, author), beforeAction, afterAction,
                        errorAction);
        }

        #endregion

        /// <summary>
        /// Run an asynchronous task that doesn't return a value
        /// </summary>
        /// <param name="task">A task that does not return a value</param>
        /// <param name="beforeAction">Action to perform before beginning the task</param>
        /// <param name="afterAction">Action to perform after completing the task</param>
        /// <param name="errorAction">Action to perform if the task encounters an error</param>
        /// <returns>The result task</returns>
        private async Task RunAsync(Task task, Action beforeAction, Action afterAction, Action errorAction)
        {
            beforeAction();
            try
            {
                await task;
            }
            catch (Exception)
            {
                // Error handling is done by examining task in finally block
            }
            finally
            {
                afterAction();

                if (task.IsFaulted)
                {
                    errorAction();
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Run an asynchronous task that does return a value
        /// </summary>
        /// <param name="task">A task that does return a value</param>
        /// <param name="beforeAction">Action to perform before beginning the task</param>
        /// <param name="afterAction">Action to perform after completing the task</param>
        /// <param name="errorAction">Action to perform if the task encounters an error</param>
        /// <returns>The task's result</returns>
        private async Task<T> RunAsync<T>(Task<T> task, Action beforeAction, Action afterAction, Action errorAction)
        {
            beforeAction();

            try
            {
                await task;
            }
            catch (Exception)
            {
                // Error handling is done by examining task in finally block
            }
            finally
            {
                afterAction();

                if (task.IsFaulted)
                {
                    errorAction();
                }

                CommandManager.InvalidateRequerySuggested();
            }

            return task.Status == TaskStatus.RanToCompletion ? task.Result : default(T);
        }

        /// <summary>
        /// Dispose of any resources used by the LibraryServiceManager
        /// </summary>
        public void Dispose()
        {
            _libraryServiceClient.Close();
        }
    }
}