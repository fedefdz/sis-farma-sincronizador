using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Core.Extensions;
using Sisfarma.Sincronizador.Domain.Core.Repositories.Farmacia;
using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public class VentasRepository : FarmaciaRepository, IVentasRepository
    {
        private readonly IClientesRepository _clientesRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IVendedoresRepository _vendedoresRepository;
        private readonly IFarmacoRepository _farmacoRepository;
        private readonly ICodigoBarraRepository _barraRepository;
        private readonly IProveedorRepository _proveedorRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IFamiliaRepository _familiaRepository;
        private readonly ILaboratorioRepository _laboratorioRepository;

        private readonly decimal _factorCentecimal;
        
        public VentasRepository(LocalConfig config, 
            IClientesRepository clientesRepository,
            ITicketRepository ticketRepository, 
            IVendedoresRepository vendedoresRepository,
            IFarmacoRepository farmacoRepository,
            ICodigoBarraRepository barraRepository,
            IProveedorRepository proveedorRepository,
            ICategoriaRepository categoriaRepository,
            IFamiliaRepository familiaRepository,
            ILaboratorioRepository laboratorioRepository) : base(config)
        {
            _clientesRepository = clientesRepository ?? throw new ArgumentNullException(nameof(clientesRepository));
            _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
            _farmacoRepository = farmacoRepository ?? throw new ArgumentNullException(nameof(farmacoRepository));
            _barraRepository = barraRepository ?? throw new ArgumentNullException(nameof(barraRepository));
            _proveedorRepository = proveedorRepository ?? throw new ArgumentNullException(nameof(proveedorRepository));
            _categoriaRepository = categoriaRepository ?? throw new ArgumentNullException(nameof(categoriaRepository));
            _familiaRepository = familiaRepository ?? throw new ArgumentNullException(nameof(familiaRepository));
            _laboratorioRepository = laboratorioRepository ?? throw new ArgumentNullException(nameof(laboratorioRepository));
            _vendedoresRepository = vendedoresRepository ?? throw new ArgumentNullException(nameof(vendedoresRepository));
        }

        public VentaDetalle GetLineaVentaOrDefaultByKey(long venta, long linea)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT * FROM lineaventa WHERE IdVenta = @venta AND IdNLinea = @linea";
                return db.Database.SqlQuery<VentaDetalle>(sql,
                    new SqlParameter("venta", venta),
                    new SqlParameter("linea", linea))
                    .FirstOrDefault();
            }
        }

        public List<Venta> GetAllByIdGreaterOrEqual(int year, long value)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT ID_VENTA as Id, Fecha as FechaHora, NPuesto as Puesto, Cliente as ClienteId, Vendedor as VendedorId, Descuento as TotalDescuento, Pago as TotalBruto, Tipo, Importe FROM ventas WHERE year(fecha) >= @year AND ID_VENTA >= @value ORDER BY ID_VENTA ASC";

                var ventas =  db.Database.SqlQuery<Venta>(sql,
                    new SqlParameter("year", year),
                    new SqlParameter("value", value))
                    .ToList();

                foreach (var venta in ventas)
                {
                    if (venta.ClienteId > 0)
                        venta.Cliente = _clientesRepository.GetOneOrDefaultById(venta.ClienteId);

                    venta.TotalBruto *= _factorCentecimal;
                    venta.TotalDescuento *= _factorCentecimal;
                    
                    var ticket = _ticketRepository.GetOneOrdefaultByVentaId(venta.Id);
                    if (ticket != null)
                    {
                        venta.Ticket = new Ticket
                        {
                            Numero = ticket.Numero,
                            Serie = ticket.Serie
                        };
                    }

                    venta.VendedorNombre = _vendedoresRepository.GetOneOrDefaultById(venta.VendedorId)?.Nombre;
                    venta.Detalle = GetDetalleDeVentaByVentaId(venta.Id);
                }

                return ventas;
            }
        }

        public List<Venta> GetVirtualesLessThanId(long venta)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT v.* FROM venta v INNER JOIN lineaventavirtual lvv ON lvv.idventa = v.idventa AND (lvv.codigo = 'Pago' OR lvv.codigo = 'A Cuenta') " +
                    @"WHERE v.ejercicio >= 2015 AND v.IdVenta < @venta ORDER BY v.IdVenta DESC";
                return db.Database.SqlQuery<Venta>(sql,
                    new SqlParameter("venta", venta))
                    .ToList();
            }
        }

        public List<LineaVentaVirtual> GetLineasVirtualesByVenta(int venta)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql =
                @"SELECT * FROM lineaventavirtual WHERE IdVenta = @venta AND (codigo = 'Pago' OR codigo = 'A Cuenta')";
                return db.Database.SqlQuery<LineaVentaVirtual>(sql,
                    new SqlParameter("venta", venta))
                    .ToList();
            }
        }

        public Venta GetOneOrDefaultById(long venta)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT * FROM venta WHERE IdVenta = @venta ORDER BY IdVenta ASC";
                return db.Database.SqlQuery<Venta>(sql,
                    new SqlParameter("venta", venta))
                    .FirstOrDefault();
            }
        }

        public List<Venta> GetGreatThanOrEqual(long venta, DateTime fecha)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var year = fecha.Year;
                var sql = "SELECT * FROM venta WHERE IdVenta >= @venta AND  ejercicio = @year AND FechaHora >= @fecha ORDER BY IdVenta ASC";

                return db.Database.SqlQuery<Venta>(sql,
                    new SqlParameter("venta", venta),
                    new SqlParameter("year", year),
                    new SqlParameter("fecha", fecha))
                    .ToList();
            }
        }

        public List<VentaDetalle> GetDetalleDeVentaByVentaId(long venta)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT ID_Farmaco as Codigo, Organismo as Receta, Cantidad, PVP, DescLin as Descuento, Importe FROM lineas_venta WHERE ID_venta= @venta";
                var detalle =  db.Database.SqlQuery<VentaDetalle>(sql,
                    new SqlParameter("venta", venta))
                    .ToList();

                var linea = 0;
                foreach (var item in detalle)
                {
                    item.Linea = ++linea;
                    item.Importe *= _factorCentecimal;
                    item.PVP *= _factorCentecimal;
                    item.Descuento *= _factorCentecimal;
                    var farmaco = _farmacoRepository.GetOneOrDefaultById(item.Codigo.ToLongOrDefault());
                    if (farmaco != null)
                    {
                        var pcoste = farmaco.PrecioUnicoEntrada.HasValue && farmaco.PrecioUnicoEntrada != 0
                            ? farmaco.PrecioUnicoEntrada.Value * _factorCentecimal
                            : (farmaco.PrecioMedio ?? 0) * _factorCentecimal;

                        var codigoBarra = _barraRepository.GetOneByFarmacoId(farmaco.Id);
                        var proveedor = _proveedorRepository.GetOneOrDefaultByCodigoNacional(farmaco.Id);

                        var categoria = farmaco.CategoriaId.HasValue
                            ? _categoriaRepository.GetOneOrDefaultById(farmaco.CategoriaId.Value)
                            : null;

                        var subcategoria = farmaco.CategoriaId.HasValue && farmaco.SubcategoriaId.HasValue
                            ? _categoriaRepository.GetSubcategoriaOneOrDefaultByKey(
                                farmaco.CategoriaId.Value,
                                farmaco.SubcategoriaId.Value)
                            : null;

                        var familia = _familiaRepository.GetOneOrDefaultById(farmaco.FamiliaId);
                        var laboratorio = _laboratorioRepository.GetOneOrDefaultByCodigo(farmaco.Laboratorio);

                        item.Farmaco = new Farmaco
                        {
                            Id = farmaco.Id,
                            Codigo = item.Codigo,
                            PrecioCoste = pcoste,
                            CodigoBarras = codigoBarra,
                            Proveedor = proveedor,
                            Categoria = categoria,
                            Subcategoria = subcategoria,
                            Familia = familia,
                            Laboratorio = laboratorio,
                            Denominacion = farmaco.Denominacion
                        };
                    }
                    else item.Farmaco = new Farmaco { Id = item.Codigo.ToLongOrDefault(), Codigo = item.Codigo };
                }

                return detalle;
            }
        }

        public Ticket GetOneOrDefaultTicketByVentaId(long venta)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT Id_Ticket as Numero, Serie FROM Tickets_D WHERE Id_Venta = @venta";
                return db.Database.SqlQuery<Ticket>(sql,
                    new SqlParameter("venta", venta))
                    .FirstOrDefault();
            }
        }

        public LineaVentaRedencion GetOneOrDefaultLineaRedencionByKey(int venta, int linea)
        {
            using (var db = FarmaciaContext.Create(_config))
            {
                var sql = @"SELECT * FROM LineaVentaReden WHERE IdVenta = @venta AND IdNLinea = @linea";
                return db.Database.SqlQuery<LineaVentaRedencion>(sql,
                        new SqlParameter("venta", venta),
                        new SqlParameter("linea", linea))
                    .FirstOrDefault();
            }
        }        
    }
}