namespace BookManager
{
    /// <summary>
    ///     Representation of a Book domain object returned from the web service
    /// </summary>
    public class BookViewModel
    {
        public string Title { get; set; }

        public string LibraryCode { get; set; }

        public string Author { get; set; }
    }
}