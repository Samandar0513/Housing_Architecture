namespace Housing_Architecture.BizLayer.Models.District;

public class DistrictDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RegionId { get; set; }
    public string? RegionName { get; set; }
}
