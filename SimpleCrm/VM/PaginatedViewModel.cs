namespace SimpleCrm.VM
{
    public class PaginatedViewModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        // Pagination logic to show pages in sets of 10
        public int StartPage
        {
            get
            {
                return Math.Max(1, (PageNumber - 1) / 10 * 10 + 1);
            }
        }

        public int EndPage
        {
            get
            {
                return Math.Min(TotalPages, StartPage + 9);
            }
        }
    }
}
