using Microsoft.AspNetCore.Identity;

namespace FreeCourse.IdentityServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //Zaten kendi otomatik içersinde baya bir nesne tutuyor bende örnek olsun diye bir tane ekledim
        public string City { get; set; }
    }
}
