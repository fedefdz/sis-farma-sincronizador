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
    public class PedidosRepository : FarmaciaRepository
    {        
        public PedidosRepository(LocalConfig config) : base(config)
        { }

        public IEnumerable<Pedido> GetByIdGreaterOrEqual(long? pedido)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT * From pedido WHERE IdPedido >= @pedido Order by IdPedido ASC";
                return db.Database.SqlQuery<Pedido>(sql,
                    new SqlParameter("pedido", pedido ?? SqlInt64.Null))
                    .ToList();
            }
        }

        public IEnumerable<Pedido> GetByFechaGreaterOrEqual(DateTime fecha)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT * From pedido WHERE Fecha >= @fecha Order by IdPedido ASC";
                return db.Database.SqlQuery<Pedido>(sql,
                    new SqlParameter("fecha", fecha))
                    .ToList();
            }
        }

        public IEnumerable<LineaPedido> GetLineasByPedido(int pedido)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"select * from lineaPedido where IdPedido = @pedido";
                return db.Database.SqlQuery<LineaPedido>(sql,
                    new SqlParameter("pedido", pedido))
                    .ToList();
            }
        }
    }
}