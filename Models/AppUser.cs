using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace mchne_api.Models
{
    public class AppUser : IdentityUser
    {    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string alias { get; set; }   
    }
}