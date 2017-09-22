using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WebUI.Data.Entities
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        public bool? IsApproved { get; set; }
        public bool IsRemoved { get; set; } = false;
        public ICollection<DiagnosticCard> DiagnosticCards { get; set; }
    }
    
}
