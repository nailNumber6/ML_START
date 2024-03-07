using System;
using System.ComponentModel.DataAnnotations;
using ToolLibrary;


namespace Server.Models.TestDB;
public partial class User
{
    public static User Create(string login, string pswd)
    {
        return new User
        {
            Login = login,
            Password = Md5HashingTool.GetHash(pswd),
            AuthorizationDate = DateTime.Now
        };
    }

    [Key]
    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime AuthorizationDate { get; set; }
}
