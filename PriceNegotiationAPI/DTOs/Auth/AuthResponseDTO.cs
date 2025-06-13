namespace PriceNegotiationAPI.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string? Token { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
