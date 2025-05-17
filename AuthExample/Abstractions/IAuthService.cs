using AuthExample.Contracts;

namespace AuthExample.Abstractions;

public interface IAuthService
{
    Guid SignUp(SignUpDto dto);
}
