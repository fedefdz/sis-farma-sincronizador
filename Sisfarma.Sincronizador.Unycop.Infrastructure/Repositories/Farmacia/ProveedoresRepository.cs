using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Core.Extensions;
using Sisfarma.Sincronizador.Domain.Core.Repositories.Farmacia;
using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public class ProveedoresRepository : FarmaciaRepository, IProveedorRepository
    {
        private readonly IRecepcionRespository _recepcionRespository;


        public ProveedoresRepository(LocalConfig config,
            IRecepcionRespository recepcionRespository) : base(config)
        {
            _recepcionRespository = recepcionRespository ?? throw new System.ArgumentNullException(nameof(recepcionRespository));
        }

        public Proveedor GetOneOrDefaultById(long id)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = "SELECT Nombre FROM Proveedores WHERE ID_Proveedor = @id";
                return db.Database.SqlQuery<Proveedor>(sql,
                    new SqlParameter("id", id))
                    .FirstOrDefault();
            }
        }

        public Proveedor GetOneOrDefaultByCodigoNacional(long codigoNacional)
        {
            var codigo = _recepcionRespository.GetCodigoProveedorActualOrDefaultByFarmaco(codigoNacional);

            return codigo.HasValue
                ? GetOneOrDefaultById(codigo.Value)
                : default(Proveedor);
        }

        public IEnumerable<Proveedor> GetAll()
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT * FROM proveedor";
                return db.Database.SqlQuery<Proveedor>(sql)
                    .ToList();
            }
        }          
    }
}