using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia.DTO;
using System.Data.SqlClient;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public interface IFarmacoRepository
    {
        Farmaco GetOneOrDefaultById(long id);
    }

    public class FarmacoRespository : FarmaciaRepository, IFarmacoRepository
    {
        public FarmacoRespository(LocalConfig config) 
            : base(config)
        { }

        public Farmaco GetOneOrDefaultById(long id)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT ID_Farmaco as Id, Denominacion, PC_U_Entrada as PrecioUnicoEntrada, PCMedio as PrecioMedio, Familia as FamiliaId, CategoriaId, SubcategoriaId, Laboratorio FROM Farmacos WHERE ID_Farmaco= @id";
                return db.Database.SqlQuery<Farmaco>(sql,
                    new SqlParameter("id", id))
                    .FirstOrDefault();
            }            
        }
    }
}
