using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public interface ICategoriaRepository
    {
        Categoria GetOneOrDefaultById(long id);

        Subcategoria GetSubcategoriaOneOrDefaultByKey(long categoria, long id);
    }

    public class CategoriaRepository : FarmaciaRepository, ICategoriaRepository
    {
        public CategoriaRepository(LocalConfig config) : base(config)
        { }        

        public Categoria GetOneOrDefaultById(long id)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT Nombre FROM categorias WHERE IDCategoria = @id";
                return db.Database.SqlQuery<Categoria>(sql,
                    new SqlParameter("id", id))
                    .FirstOrDefault();
            }
                
        }

        public Subcategoria GetSubcategoriaOneOrDefaultByKey(long categoria, long id)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = "SELECT Nombre FROM subcategorias WHERE IdSubCategoria = @id AND IdCategoria = @categoria";
                return db.Database.SqlQuery<Subcategoria>(sql,
                    new SqlParameter("id", id),
                    new SqlParameter("categoria", categoria))
                        .FirstOrDefault();
            }
        }
    }
}
