namespace BloodCenter.Search.Client.Models
{
    public record GetUsersRequestDto(string? QueryString, IReadOnlyList<string>? Roles);
}
