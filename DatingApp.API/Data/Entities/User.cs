using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Data.Entities
{
    public class User
    {
        [Key]
        public int ID {get; set;}

        [MaxLength(256)]
        public string Username {get; set;}

        [MaxLength(256)]
        public string Email {get;set;}
        public byte[] PasswordHashed {get; set;}
        public byte[] PasswordSalt {get; set;}

    }
}