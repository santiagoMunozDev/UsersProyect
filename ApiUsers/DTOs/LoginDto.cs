using System.ComponentModel.DataAnnotations;

namespace UsersApi.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        public string Contraseña { get; set; }
    }
}
