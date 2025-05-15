using AuthExample.Contracts;

namespace AuthExample.Models;

public partial class Post
{
    public PostDetailedVm ToDetailedVm() => new(Id, Content, PublicationDateTime, User.UserName);

    public PostVm ToVm() => new(Id, Content);
}
