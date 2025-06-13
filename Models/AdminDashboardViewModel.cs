using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Canela.Models
{
    public class AdminDashboardViewModel
    {
        public List<Pago> Pagos { get; set; }
        public List<IdentityUser> Usuarios { get; set; }
    }
}