namespace GLMS2.API2.Dtos
{
    public class ClientResponseDto
    {
        public int ClientId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}