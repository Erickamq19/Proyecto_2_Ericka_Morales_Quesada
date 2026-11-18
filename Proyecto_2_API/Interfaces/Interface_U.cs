using Proyecto_2_API.Modelos;
using System.Threading.Tasks;

namespace Proyecto_2_API.Interfaces
{
    public interface Interface_U
    {
        Task<(bool Exito, string Mensaje)> RegistroAsync(Registro_U usuario);
    }
}
