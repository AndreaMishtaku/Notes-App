
namespace Shared.Response;

public class PagedResult<T>
{
    public int RowCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<T> Rows { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
    public int TotalPages { get; set; }
}
