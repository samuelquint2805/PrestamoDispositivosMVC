
using PrestamoDispositivos.Core.Pagination.Abstractions;

namespace PrestamoDispositivos.Core.Pagination
{
    public class PaginationResponse<T> : IPagination
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int RecordsPerPage { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public string? Filter { get; set; }

        public List<int> Pages => Enumerable.Range(1, TotalPages).ToList();

        public List<T> Items { get; set; }

        public PaginationResponse() { }

        public PaginationResponse(List<T> items, int totalCount, int pageSize, int currentPage)
        {
            Items = items;
            TotalCount = totalCount;
            RecordsPerPage = pageSize;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}
