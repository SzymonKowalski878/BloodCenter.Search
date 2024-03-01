namespace BloodCenter.Search.Client.Models
{
    public record UserDocumentDto(Guid Id, string Email, string Role, string FirstName, string LastName);
}