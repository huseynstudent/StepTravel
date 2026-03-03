using StoreApp.Domain.BaseEntities;
namespace StoreApp.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } 
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }
        public string Fin { get; set; } = string.Empty;
    }
}