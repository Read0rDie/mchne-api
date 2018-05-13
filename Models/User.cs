using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace mchne_api.Models
{
    public class User
    {    
    public int Id { get; set; }
    public string IdentityId { get; set; }
    public AppUser Identity { get; set; }    
    public string Gender { get; set; }   
    }
}