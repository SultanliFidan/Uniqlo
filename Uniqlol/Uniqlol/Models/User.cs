﻿using Microsoft.AspNetCore.Identity;

namespace Uniqlol.Models
{
    public class User : IdentityUser
    {
        public string Fullname { get; set; }
        public string? Address { get; set; }
        public string? ProfileImageUrl { get; set; }

    }
}
