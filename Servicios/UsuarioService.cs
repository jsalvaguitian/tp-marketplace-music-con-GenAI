using Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Servicios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly MusicTradeDbContext _context;

        public UsuarioService(MusicTradeDbContext context)
        {
            _context = context;
        }


        public async Task<Usuario?> ObtenerPorIdConPublicacionesAsync(int id)
        {
            return await _context.Usuarios

                .Include(u => u.Provincia)
                .Include(u => u.Publicaciones)

                .FirstOrDefaultAsync(u => u.Id == id);


        }
    }
}
