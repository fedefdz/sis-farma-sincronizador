using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Domain.Core.Repositories.Farmacia;
using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public class CategoriasRepository : FarmaciaRepository, ICategoriasRepository
    {        
        public CategoriasRepository(LocalConfig config) : base(config)
        { }

        public Categoria GetById(short id)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT * FROM familia WHERE IdFamilia = @id";
                return db.Database.SqlQuery<Categoria>(sql,
                        new SqlParameter("id", id))
                    .FirstOrDefault();
            }
        }

        public IEnumerable<Categoria> Get()
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = "select * from familia";
                return db.Database.SqlQuery<Categoria>(sql)
                    .ToList();
            }
        }

        public IEnumerable<Categoria> GetByDescripcion()
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"select nombre from categorias WHERE nombre NOT IN ('ESPECIALIDAD', 'EFP', 'SIN FAMILIA') AND nombre NOT LIKE '%ESPECIALIDADES%' AND nombre NOT LIKE '%Medicamento%'";
                return db.Database.SqlQuery<Categoria>(sql)
                    .ToList();
            }
        }

        public IEnumerable<string> GetAllNombreSubcategoriaByCategoriaId(long id)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT nombre FROM Subcategorias WHERE  IdCategoria = @id";
                return db.Database.SqlQuery<string>(sql,
                    new SqlParameter("id", id))
                    .ToList();
            }
        }

        public string GetSuperFamiliaDescripcionById(short familia)
        {
            throw new System.NotImplementedException();
            //using (var db = FarmaticContext.Create(_config))
            //{
            //    var sql =
            //    @"SELECT sf.Descripcion FROM SuperFamilia sf INNER JOIN FamiliaAux fa ON sf.IdSuperFamilia = fa.IdSuperFamilia " +
            //    @"WHERE fa.IdFamilia = @familia";
            //    return db.Database.SqlQuery<string>(sql,
            //        new SqlParameter("familia", familia))
            //        .FirstOrDefault();
            //}
        }

        public string GetSubCategoriaById(string v)
        {
            throw new System.NotImplementedException();
        }
    }
}