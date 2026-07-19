
using Microsoft.EntityFrameworkCore;

namespace Entidades.Seed;

public static class ProvinciasSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Provincia>().HasData(
            new Provincia{ Id = 1, Nombre = "Buenos Aires"},
            new Provincia{ Id = 3, Nombre = "Catamarca"},
            new Provincia{ Id = 2, Nombre = "CABA"},
            new Provincia{ Id = 4, Nombre = "Chaco"},
            new Provincia{ Id = 5, Nombre = "Chubut"},
            new Provincia{ Id = 6, Nombre = "Córdoba"},
            new Provincia{ Id = 7, Nombre = "Corrientes"},
            new Provincia{ Id = 8, Nombre = "Entre Ríos"},
            new Provincia{ Id = 9, Nombre = "Formosa"},
            new Provincia{ Id = 10, Nombre = "Jujuy"},
            new Provincia{ Id = 11, Nombre = "La Pampa"},
            new Provincia{ Id = 12, Nombre = "La Rioja"},
            new Provincia{ Id = 13, Nombre = "Mendoza"},
            new Provincia{ Id = 14, Nombre = "Misiones"},
            new Provincia{ Id = 15, Nombre = "Neuquén"},
            new Provincia{ Id = 16, Nombre = "Río Negro"},
            new Provincia{ Id = 17, Nombre = "Salta"},
            new Provincia{ Id = 18, Nombre = "San Juan"},
            new Provincia{ Id = 19, Nombre = "San Luis"},
            new Provincia{ Id = 20, Nombre = "Santa Cruz"},
            new Provincia{ Id = 21, Nombre = "Santa Fe"},
            new Provincia{ Id = 22, Nombre = "Santiago del Estero"},
            new Provincia{ Id = 23, Nombre = "Tierra del Fuego"},
            new Provincia{ Id = 24, Nombre = "Tucumán"}
            
        );
    }
}