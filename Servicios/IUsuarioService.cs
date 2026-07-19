using Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Servicios
{
        public interface IUsuarioService
    {

        Task<Usuario?> ObtenerPorIdConPublicacionesAsync(int id);
    }
}
