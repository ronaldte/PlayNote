namespace PlayNote.Services
{
    /// <summary>
    /// Represents paging metadata for response about the collection.
    /// </summary>
    public class PaginationMetadata
    {
        /// <summary>
        /// Number of objects inside the collection.
        /// </summary>
        public int TotalItemCount { get; private set; }

        /// <summary>
        /// Number of pages for the collection based on PageSize.
        /// </summary>
        public int TotalPageCount { get; private set; }

        /// <summary>
        /// Number of objects on single page.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Page number for loaded objects from collection.
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="PaginationMetadata"/> using given parameters.
        /// </summary>
        /// <param name="totalItemCount">Total number of objects inside the collection</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="currentPage">Current page</param>
        public PaginationMetadata(int totalItemCount, int pageSize, int currentPage)
        {
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
        }
    }
}

