using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Proyecto_2_API.Modelos;

namespace Proyecto_2_API.Interfaces
{
    public interface Interface_JWT
    {

        Task<String>? Inicio_S_Async(Inicio_U usuario);
    }
}
