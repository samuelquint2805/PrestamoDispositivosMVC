namespace PrestamoDispositivos.Core.Pagination.Abstractions
{
    public interface IPagination
    {
        int CurrentPage { get; set; }
        int TotalPages { get; set; }
        int RecordsPerPage { get; set; }
        int TotalCount { get; set; }
        bool HasPrevious { get; }
        bool HasNext { get; }
        string? Filter { get; set; }
        List<int> Pages { get; }
    }
}
