namespace DancingGoat.Models;

public record ImageViewModel
{
    public string ImageUrl { get; init; }
    public string Alt { get; set; }
    public string Title { get; set; }
}
