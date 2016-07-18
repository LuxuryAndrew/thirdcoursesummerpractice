using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Task3.Models
{
    class FabricContext: DbContext
    {
        public DbSet<Fabric> Fabrics { get; set; }
    }
}
