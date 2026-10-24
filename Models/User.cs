using Microsoft.AspNetCore.Identity;

namespace Hotel.Models
{
    public class User : IdentityUser
    {
        public string PassportNumber { get; set; } = string.Empty;
    }
}
