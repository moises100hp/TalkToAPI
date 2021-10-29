using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Models
{
    public class Token
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        [ForeignKey("Usuario")]
        public string UsuarioId { get; set; }
        public AplicationUser Usuario { get; set; }
        public bool utilizado { get; set; }
        public DateTime ExpirationToken { get; set; }
        public DateTime ExpirationRefreshToken { get; set; }
        public DateTime Criado { get; set; }
        public DateTime? Atualizado { get; set; }
    }
}
