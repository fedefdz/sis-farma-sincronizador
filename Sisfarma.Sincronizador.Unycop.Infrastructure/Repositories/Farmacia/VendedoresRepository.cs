using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Domain.Core.Repositories.Farmacia;
using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public class VendedoresRepository : FarmaciaRepository, IVendedoresRepository 
    {
        public VendedoresRepository(LocalConfig config) : base(config)
        { }

        public Vendedor GetOneOrDefaultById(long id)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT Nombre FROM vendedores WHERE ID_Vendedor = @id";
                return db.Database.SqlQuery<Vendedor>(sql,
                    new SqlParameter("id", id))
                        .FirstOrDefault();
            }
        }
    }
}