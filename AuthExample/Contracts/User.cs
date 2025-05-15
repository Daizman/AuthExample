namespace AuthExample.Contracts;

public record SignUpDto(string UserName, string Password);
public record LogInDto(string UserName, string Password);
