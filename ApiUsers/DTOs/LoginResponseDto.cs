namespace UsersApi.DTOs
{
    public class LoginResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Token { get; set; }
    }
}
