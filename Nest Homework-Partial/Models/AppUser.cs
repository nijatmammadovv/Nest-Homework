using Microsoft.AspNetCore.Identity;

namespace Nest_Homework_Partial.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
