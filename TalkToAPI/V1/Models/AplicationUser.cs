using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Models
{
    public class AplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Slogan { get; set; }
    }
}
