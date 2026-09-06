namespace FCloud3.Entities.Transport;

public class TransportItem
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; } = "";
    public string? Color { get; set; }
    public string FileUrl { get; set; } = ""; // relative URL under wwwroot
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
