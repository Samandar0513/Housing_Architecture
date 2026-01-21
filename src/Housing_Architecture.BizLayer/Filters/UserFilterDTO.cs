namespace Housing_Architecture.BizLayer.Filters;

public class UserFilterDTO
{
    public string? Search { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
