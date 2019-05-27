using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Core.Extensions;
using Sisfarma.Sincronizador.Domain.Core.Repositories.Farmacia;
using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using DTO = Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia.DTO;

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

        private readonly decimal _factorCentecimal = 0.01m;
        
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

        public VentasRepository(
            IClientesRepository clientesRepository,
            ITicketRepository ticketRepository,
            IVendedoresRepository vendedoresRepository,
            IFarmacoRepository farmacoRepository,
            ICodigoBarraRepository barraRepository,
            IProveedorRepository proveedorRepository,
            ICategoriaRepository categoriaRepository,
            IFamiliaRepository familiaRepository,
            ILaboratorioRepository laboratorioRepository)
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
            using (var db = FarmaciaContext.Ventas())
            {
                var sql = @"SELECT * FROM lineaventa WHERE IdVenta = @venta AND IdNLinea = @linea";
                return db.Database.SqlQuery<VentaDetalle>(sql,
                    new SqlParameter("venta", venta),
                    new SqlParameter("linea", linea))
                    .FirstOrDefault();
            }
        }

        public Venta GetOneOrDefaultById(long id)
        {
            var year = int.Parse($"{id}".Substring(0, 4));
            var ventaId = int.Parse($"{id}".Substring(4));
            
            DTO.Venta ventaAccess;
            try
            {
                using (var db = FarmaciaContext.VentasByYear(year))
                {
                    var sql = @"SELECT ID_VENTA as Id, Fecha, NPuesto as Puesto, Cliente, Vendedor, Descuento, Pago, Tipo, Importe FROM ventas WHERE ID_VENTA = @id";
                    ventaAccess = db.Database.SqlQuery<DTO.Venta>(sql,
                        new OleDbParameter("id", ventaId))
                        .FirstOrDefault();
                }
            }
            catch (FarmaciaContextException)
            {
                ventaAccess = null;
            }

            if (ventaAccess == null)
                return null;

            var venta = new Venta
            {
                Id = ventaAccess.Id,
                Tipo = ventaAccess.Tipo.ToString(),
                FechaHora = ventaAccess.Fecha,
                Puesto = ventaAccess.Puesto,
                ClienteId = ventaAccess.Cliente,
                VendedorId = ventaAccess.Vendedor,
                TotalDescuento = ventaAccess.Descuento * _factorCentecimal,
                TotalBruto = ventaAccess.Pago * _factorCentecimal,
                Importe = ventaAccess.Importe * _factorCentecimal,
            };


            if (ventaAccess.Cliente > 0)
                venta.Cliente = _clientesRepository.GetOneOrDefaultById(ventaAccess.Cliente);

            var ticket = _ticketRepository.GetOneOrdefaultByVentaId(ventaAccess.Id);
            if (ticket != null)
            {
                venta.Ticket = new Ticket
                {
                    Numero = ticket.Numero,
                    Serie = ticket.Serie
                };
            }

            venta.VendedorNombre = _vendedoresRepository.GetOneOrDefaultById(ventaAccess.Vendedor)?.Nombre;
            venta.Detalle = GetDetalleDeVentaByVentaId(ventaAccess.Id);

            return venta;
        }

        public List<Venta> GetAllByIdGreaterOrEqual(int year, long value)
        {
            // Access no handlea long
            var valueInteger = (int)value;
            List<DTO.Venta> ventasAccess;

            try
            {
                using (var db = FarmaciaContext.Ventas(year))
                {
                    var sql = @"SELECT ID_VENTA as Id, Fecha, NPuesto as Puesto, Cliente, Vendedor, Descuento, Pago, Tipo, Importe FROM ventas WHERE year(fecha) >= @year AND ID_VENTA >= @value ORDER BY ID_VENTA ASC";

                    ventasAccess = db.Database.SqlQuery<DTO.Venta>(sql,
                        new OleDbParameter("year", year),
                        new OleDbParameter("value", valueInteger))
                        .Take(10)
                        .ToList();
                }
            }
            catch (FarmaciaContextException)
            {
                ventasAccess =  new List<DTO.Venta>();
            }
            
            var ventas = new List<Venta>();
            foreach (var ventaAccess in ventasAccess)
            {
                var venta = new Venta
                {
                    Id = ventaAccess.Id,
                    Tipo = ventaAccess.Tipo.ToString(),
                    FechaHora = ventaAccess.Fecha,
                    Puesto = ventaAccess.Puesto,
                    ClienteId = ventaAccess.Cliente,
                    VendedorId = ventaAccess.Vendedor,
                    TotalDescuento = ventaAccess.Descuento * _factorCentecimal,
                    TotalBruto = ventaAccess.Pago * _factorCentecimal,
                    Importe = ventaAccess.Importe * _factorCentecimal,                        
                };
                    

                if (ventaAccess.Cliente > 0)
                        venta.Cliente =_clientesRepository.GetOneOrDefaultById(ventaAccess.Cliente);
                    
                var ticket = _ticketRepository.GetOneOrdefaultByVentaId(ventaAccess.Id);
                if (ticket != null)
                {
                    venta.Ticket = new Ticket
                    {
                        Numero = ticket.Numero,
                        Serie = ticket.Serie
                    };
                }

                venta.VendedorNombre = _vendedoresRepository.GetOneOrDefaultById(ventaAccess.Vendedor)?.Nombre;
                venta.Detalle = GetDetalleDeVentaByVentaId(ventaAccess.Id);

                ventas.Add(venta);
            }

            return ventas;            
        }

        public List<Venta> GetVirtualesLessThanId(long venta)
        {
            using (var db = FarmaciaContext.Ventas())
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
            using (var db = FarmaciaContext.Ventas())
            {
                var sql =
                @"SELECT * FROM lineaventavirtual WHERE IdVenta = @venta AND (codigo = 'Pago' OR codigo = 'A Cuenta')";
                return db.Database.SqlQuery<LineaVentaVirtual>(sql,
                    new SqlParameter("venta", venta))
                    .ToList();
            }
        }        

        public List<Venta> GetGreatThanOrEqual(long venta, DateTime fecha)
        {
            using (var db = FarmaciaContext.Ventas())
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
            var ventaInteger = (int)venta;

            try
            {
                using (var db = FarmaciaContext.Ventas())
                {
                    var sql = @"SELECT ID_Farmaco as Farmaco, Organismo, Cantidad, PVP, DescLin as Descuento, Importe FROM lineas_venta WHERE ID_venta= @venta";
                    var lineas = db.Database.SqlQuery<DTO.LineaVenta>(sql,
                        new OleDbParameter("venta", ventaInteger))
                        .ToList();

                    var linea = 0;
                    var detalle = new List<VentaDetalle>();
                    foreach (var item in lineas)
                    {
                        var ventaDetalle = new VentaDetalle
                        {
                            Linea = ++linea,
                            Importe = item.Importe * _factorCentecimal,
                            PVP = item.PVP * _factorCentecimal,
                            Descuento = item.Descuento * _factorCentecimal,
                            Receta = item.Organismo,
                            Cantidad = item.Cantidad
                        };

                        var farmaco = _farmacoRepository.GetOneOrDefaultById(item.Farmaco);
                        if (farmaco != null)
                        {
                            var pcoste = farmaco.PrecioUnicoEntrada.HasValue && farmaco.PrecioUnicoEntrada != 0
                                ? (decimal)farmaco.PrecioUnicoEntrada.Value * _factorCentecimal
                                : ((decimal?)farmaco.PrecioMedio ?? 0m) * _factorCentecimal;

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

                            var familia = _familiaRepository.GetOneOrDefaultById(farmaco.Familia);
                            var laboratorio = _laboratorioRepository.GetOneOrDefaultByCodigo(farmaco.Laboratorio);

                            ventaDetalle.Farmaco = new Farmaco
                            {
                                Id = farmaco.Id,
                                Codigo = item.Farmaco.ToString(),
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
                        else ventaDetalle.Farmaco = new Farmaco { Id = item.Farmaco, Codigo = item.Farmaco.ToString() };

                        detalle.Add(ventaDetalle);
                    }

                    return detalle;
                }
            }
            catch (FarmaciaContextException)
            {
                return new List<VentaDetalle>();
            }            
        }

        public Ticket GetOneOrDefaultTicketByVentaId(long id)
        {
            var year = int.Parse($"{id}".Substring(0, 4));
            var ventaId = int.Parse($"{id}".Substring(4));

            using (var db = FarmaciaContext.VentasByYear(year))
            {
                var sql = @"SELECT Id_Ticket as Numero, Serie FROM Tickets_D WHERE Id_Venta = @venta";
                return db.Database.SqlQuery<Ticket>(sql,
                    new SqlParameter("venta", ventaId))
                    .FirstOrDefault();
            }
        }

        public LineaVentaRedencion GetOneOrDefaultLineaRedencionByKey(int venta, int linea)
        {
            using (var db = FarmaciaContext.Ventas())
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