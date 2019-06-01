using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Domain.Core.Repositories.Farmacia;
using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public class ListaRepository : FarmaciaRepository , IListaRepository
    {        
        public ListaRepository(LocalConfig config) : base(config)
        { }

        public ListaRepository() { }

        public IEnumerable<Lista> GetAllByIdGreaterThan(int id)
        {            
            using (var db = FarmaciaContext.Proveedores())
            {
                var sql = @"SELECT ID_Bolsa as Id, Descripcion FROM DescrBolsas WHERE id_bolsa > @id";
                var rs = db.Database.SqlQuery<Lista>(sql,
                    new OleDbParameter("id", id))
                    .Take(10)
                    .ToList();

                foreach (var item in rs)
                {
                    var sqlFarmacos = @"SELECT id_bolsa as ListaId, id_farmaco as FarmacoId FROM Bolsas WHERE id_bolsa = @id GROUP BY id_bolsa, id_farmaco";
                    var farmacos = db.Database.SqlQuery<ListaDetalle>(sqlFarmacos,
                        new OleDbParameter("id", item.Id))
                            .ToList();

                    item.Farmacos = farmacos;
                }

                return rs;
            }
        }

        //public Lista GetOneOrDefault(int lista)
        //{
        //    using (var db = FarmaciaContext.Create(_config))
        //    {
        //        var sql = @"SELECT * FROM ListaArticu WHERE fecha >= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) AND idLista = @lista";
        //        return db.Database.SqlQuery<Lista>(sql,
        //            new SqlParameter("lista", lista))
        //            .FirstOrDefault();
        //    }
        //}

        //public ArticuloWithIva GetArticuloWithIva(int lista, string articulo)
        //{
        //    using (var db = FarmaciaContext.Create(_config))
        //    {
        //        var sql = @"select a.*, t.Piva AS iva from articu a INNER JOIN Tablaiva t ON t.IdTipoArt = a.XGrup_IdGrupoIva AND t.IdTipoPro = '05' " +
        //            @"INNER JOIN ItemListaArticu li ON li.XItem_IdArticu = a.idArticu AND li.XItem_IdLista = @lista " +
        //            @"WHERE a.idArticu = @articulo";
        //        return db.Database.SqlQuery<ArticuloWithIva>(sql,
        //                new SqlParameter("lista", lista),
        //                new SqlParameter("articulo", articulo))
        //                .FirstOrDefault();
        //    }
        //}

        //public IEnumerable<ListaArticulo> GetByIdGreaterThan(int lista)
        //{
        //    using (var db = FarmaciaContext.Create(_config))
        //    {
        //        var sql = @"SELECT * FROM ListaArticu WHERE idLista > @lista";
        //        return db.Database.SqlQuery<ListaArticulo>(sql,
        //            new SqlParameter("lista", lista))
        //            .ToList();
        //    }
        //}

        //public List<ListaArticulo> GetByFechaExceptList(int lista)
        //{
        //    using (var db = FarmaciaContext.Create(_config))
        //    {
        //        var sql = @"SELECT * FROM ListaArticu WHERE fecha >= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) AND idLista <> @lista";
        //        return db.Database.SqlQuery<ListaArticulo>(sql,
        //            new SqlParameter("lista", lista))
        //            .ToList();
        //    }
        //}

        //public List<ItemListaArticulo> GetArticulosByLista(int lista)
        //{
        //    using (var db = FarmaciaContext.Create(_config))
        //    {
        //        var sql = @"SELECT * FROM ItemListaArticu WHERE XItem_IdLista = @lista GROUP BY XItem_IdLista, XItem_IdArticu";
        //        return db.Database.SqlQuery<ItemListaArticulo>(sql,
        //            new SqlParameter("lista", lista))
        //            .ToList();
        //    }
        //}

        //public void Update(int lista)
        //{
        //    using (var db = FarmaciaContext.Create(_config))
        //    {
        //        var sql = @"UPDATE ListaArticu SET fecha = DATEADD(dd, -1, DATEDIFF(dd, 0, GETDATE())) WHERE idLista = @lista";
        //        db.Database.ExecuteSqlCommand(sql,
        //            new SqlParameter("lista", @lista));
        //    }
        //}        
    }
}