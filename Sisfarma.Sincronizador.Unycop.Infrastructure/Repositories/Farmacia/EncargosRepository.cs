using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Farmatic.Models;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public class EncargosRepository : FarmaciaRepository
    {        
        public EncargosRepository(LocalConfig config) : base(config)
        { }

        public IEnumerable<Encargo> GetAllByContadorGreaterOrEqual(int year, long? contador)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT TOP 1000 * From Encargo WHERE year(idFecha) >= @year AND IdContador >= @contador Order by IdContador ASC";
                return db.Database.SqlQuery<Encargo>(sql,
                    new SqlParameter("year", year),
                    new SqlParameter("contador", contador ?? SqlInt64.Null))
                    .ToList();
            }
        }

        public IEnumerable<Encargo> GetAllGreaterOrEqualByFecha(DateTime fecha)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT * From Encargo WHERE idFecha >= @fecha AND estado > 0 Order by idFecha DESC";
                return db.Database.SqlQuery<Encargo>(sql,
                    new SqlParameter("fecha", fecha))
                    .ToList();
            }
        }
    }
}