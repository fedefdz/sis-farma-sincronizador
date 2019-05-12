using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia.DTO;
using System.Data.SqlClient;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories
{
    public interface ITicketRepository
    {
        Ticket GetOneOrdefaultByVentaId(long venta);
    }


    public class TicketRepository : FarmaciaRepository, ITicketRepository
    {
        public TicketRepository(LocalConfig config) : base(config)
        { }

        public Ticket GetOneOrdefaultByVentaId(long venta)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT Id_Ticket as Numero, Serie FROM Tickets_D WHERE Id_Venta = @venta";
                return db.Database.SqlQuery<Ticket>(sql,
                    new SqlParameter("venta", venta))
                    .FirstOrDefault();
            }
        }
    }
}
