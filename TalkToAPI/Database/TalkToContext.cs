using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalkToAPI.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;

namespace TalkToAPI.Database
{
    public class TalkToContext : IdentityDbContext<AplicationUser>
    {
        public TalkToContext(DbContextOptions<TalkToContext> options) : base(options)
        {

        }

        public DbSet<Mensagem> Mensagem { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public object Token { get; internal set; }
    }
}
