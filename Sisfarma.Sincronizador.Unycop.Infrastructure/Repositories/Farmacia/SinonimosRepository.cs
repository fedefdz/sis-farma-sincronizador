using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Farmatic.Models;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public class SinonimosRepository : FarmaciaRepository
    {        
        public SinonimosRepository(LocalConfig config) : base(config)
        { }

        public List<Sinonimos> GetAll()
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT * FROM Sinonimo";
                return db.Database.SqlQuery<Sinonimos>(sql)
                    .ToList();
            }
        }

        public Sinonimos GetOneOrDefaultByArticulo(string codigo)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT * FROM sinonimo WHERE IdArticu = @codigo";
                return db.Database.SqlQuery<Sinonimos>(sql,
                    new SqlParameter("codigo", codigo))
                    .FirstOrDefault();
            }
        }
    }
}