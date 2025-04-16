using System.ComponentModel.DataAnnotations;

namespace UsersApi.DTOs
{
    public class CreateUserDto
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [MinLength(6)]
        public string password { get; set; }
    }
}
