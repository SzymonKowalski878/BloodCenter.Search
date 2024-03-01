namespace BloodCenter.Search.Client.Models
{
    public record AddUserRequestDto(Guid? Id, string Email, string Role, string FirstName, string LastName);
}
