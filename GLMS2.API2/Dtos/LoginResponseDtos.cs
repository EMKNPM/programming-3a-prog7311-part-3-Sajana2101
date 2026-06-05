namespace GLMS2.API2.Dtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
    }
}