namespace Web.DTOs
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
    }
}