namespace IdentityServer4.Dtos;

public record UserRegisterDto(string UserName, string Email, string Password);
public record UserLoginDto(string Email, string Password);