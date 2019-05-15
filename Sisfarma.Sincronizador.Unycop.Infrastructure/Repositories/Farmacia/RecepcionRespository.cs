using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public interface IRecepcionRespository
    {
        long? GetCodigoProveedorActualOrDefaultByFarmaco(long farmaco);
    }

    public class RecepcionRespository : FarmaciaRepository, IRecepcionRespository
    {
        public RecepcionRespository(LocalConfig config) : base(config)
        { }

        public RecepcionRespository() { }

        public long? GetCodigoProveedorActualOrDefaultByFarmaco(long farmaco)
        {
            var farmacoInteger = (int)farmaco;
            using (var db = FarmaciaContext.Recepcion())
            {
                var sql = "SELECT TOP 1 Proveedor FROM Recepcion WHERE ID_Farmaco = @farmaco ORDER BY ID_Fecha DESC";
                return db.Database.SqlQuery<int?>(sql,
                    new OleDbParameter("farmaco", farmaco))
                    .FirstOrDefault();
            }            
        }
    }
}
