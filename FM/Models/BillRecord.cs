public class BillRecord
{
    public string BillId { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Amount { get; set; }
    public string Type { get; set; } = "Permanent";
    public string Length { get; set; } = "Not Applicable";
    public DateTime DueDate { get; set; }
    public string Description { get; set; } = "";
}