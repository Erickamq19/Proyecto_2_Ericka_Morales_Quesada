using Microsoft.EntityFrameworkCore;

namespace Proyecto_2_API.DB
{
    public class conexion_DB : DbContext
    {

        public conexion_DB(DbContextOptions<conexion_DB> options) : base (options)
        {


        }


    }
}
