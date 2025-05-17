using System.Text;
using AuthExample.Abstractions;
using AuthExample.Contracts;
using AuthExample.Database;
using AuthExample.Models;

namespace AuthExample.Services;

public class AuthService(
    AppDbContext dbContext
) : IAuthService
{
    public Guid SignUp(SignUpDto dto)
    {
        var user = new User
        {
            UserName = dto.UserName,
            Password = Encoding.UTF8.GetBytes(dto.Password),
        };

        dbContext.Users.Add(user);

        dbContext.SaveChanges();

        return user.Id;
    }
}
