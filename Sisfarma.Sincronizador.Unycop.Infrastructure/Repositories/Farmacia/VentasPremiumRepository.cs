using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia.DTO;
using System.Data.SqlClient;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public interface IVentasPremiumRepository
    {
        VentaPremiun GetOneOrDefaultByTarjeta(string tarjeta);

        VentaPremiun GetOneOrDefaultByClienteId(long id);
    }

    public class VentasPremiumRepository : FarmaciaRepository, IVentasPremiumRepository
    {
        public VentasPremiumRepository(LocalConfig config) 
            : base(config)
        { }

        public VentaPremiun GetOneOrDefaultByClienteId(long id)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = $"SELECT TOP 1  PuntosIniciales, PuntosVentas, PuntosACanjear FROM Ventas_FarmaPremium WHERE ClienteUW = {id} ORDER BY Fecha DESC";
                return db.Database.SqlQuery<VentaPremiun>(sql,
                        new SqlParameter("id", id))
                        .FirstOrDefault();
            }
        }

        public VentaPremiun GetOneOrDefaultByTarjeta(string tarjeta)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = $"SELECT TOP 1  PuntosIniciales, PuntosVentas, PuntosACanjear FROM Ventas_FarmaPremium WHERE ClienteFarma = '{tarjeta}' ORDER BY Fecha DESC";
                return db.Database.SqlQuery<VentaPremiun>(sql,
                        new SqlParameter("tarjeta", tarjeta))
                        .FirstOrDefault();
            }            
        }
    }
}
