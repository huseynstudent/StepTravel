using StoreApp.Domain.BaseEntities;
using StoreApp.Domain.Enums;
namespace StoreApp.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string? ProfilePicture { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? ConfirmCode { get; set; }
    public DateOnly Birthday { get; set; }
    public string Fin { get; set; } = string.Empty;
    public UserType Role { get; set; } = UserType.Customer;
    public bool IsConfirmed { get; set; } = false;
}