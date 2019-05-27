using Sisfarma.Sincronizador.Core.Extensions;
using Sisfarma.Sincronizador.Domain.Core.Services;
using Sisfarma.Sincronizador.Domain.Core.Sincronizadores.SuperTypes;
using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using Sisfarma.Sincronizador.Domain.Entities.Fisiotes;
using System.Collections.Generic;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Domain.Core.Sincronizadores
{
    public class PagoPendienteActualizarSincronizador : TaskSincronizador
    {
        protected const string TIPO_CLASIFICACION_DEFAULT = "Familia";
        protected const string TIPO_CLASIFICACION_CATEGORIA = "Categoria";
        protected const string SISTEMA_UNYCOP = "unycop";

        protected const string FAMILIA_DEFAULT = "<Sin Clasificar>";
        protected const string LABORATORIO_DEFAULT = "<Sin Laboratorio>";
        protected const string VENDEDOR_DEFAULT = "NO";

        protected bool _perteneceFarmazul;
        protected string _puntosDeSisfarma;
        protected string _cargarPuntos;
        protected string _fechaDePuntos;
        protected string _soloPuntosConTarjeta;
        protected string _canjeoPuntos;

        private string _clasificacion;        

        private long _ultimaVenta;

        public PagoPendienteActualizarSincronizador(IFarmaciaService farmacia, ISisfarmaService fisiotes) 
            : base(farmacia, fisiotes)
        { }

        public override void LoadConfiguration()
        {
            _perteneceFarmazul = bool.Parse(ConfiguracionPredefinida[Configuracion.FIELD_ES_FARMAZUL]);
            _puntosDeSisfarma = ConfiguracionPredefinida[Configuracion.FIELD_PUNTOS_SISFARMA];
            _cargarPuntos = ConfiguracionPredefinida[Configuracion.FIELD_CARGAR_PUNTOS] ?? "no";
            _fechaDePuntos = ConfiguracionPredefinida[Configuracion.FIELD_FECHA_PUNTOS];
            _soloPuntosConTarjeta = ConfiguracionPredefinida[Configuracion.FIELD_SOLO_PUNTOS_CON_TARJETA];
            _canjeoPuntos = ConfiguracionPredefinida[Configuracion.FIELD_CANJEO_PUNTOS];

            _clasificacion = !string.IsNullOrWhiteSpace(ConfiguracionPredefinida[Configuracion.FIELD_TIPO_CLASIFICACION])
                ? ConfiguracionPredefinida[Configuracion.FIELD_TIPO_CLASIFICACION]
                : TIPO_CLASIFICACION_DEFAULT;
        }

        public override void PreSincronizacion()
        {
            var valorConfiguracion = _sisfarma.Configuraciones.GetByCampo(Configuracion.FIELD_POR_DONDE_VOY_PAGOS);

            var venta = !string.IsNullOrEmpty(valorConfiguracion)
                ? valorConfiguracion.ToLongOrDefault()
                : 20130L;

            _ultimaVenta = venta;
        }

        public override void Process()
        {
            if (_sisfarma.PuntosPendientes.AnyWithoutPagoGreaterThanVentaId(_ultimaVenta))
            {
                var year = int.Parse($"{_ultimaVenta}".Substring(0, 4));
                var ventaId = int.Parse($"{_ultimaVenta}".Substring(4));

                var ventas = _farmacia.Ventas.GetAllByIdGreaterOrEqual(year, ventaId);
                foreach (var venta in ventas)
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    var puntosPendientes = GenerarPuntosPendientes(venta);
                    foreach (var puntoPendiente in puntosPendientes)
                    {                        
                        _sisfarma.PuntosPendientes.Sincronizar(puntoPendiente);                        
                        _ultimaVenta = puntoPendiente.VentaId;
                    }
                }
            }            
        }

        private IEnumerable<PuntosPendientes> GenerarPuntosPendientes(Venta venta)
        {
            if (!venta.HasCliente() && venta.Tipo != "1")
                return new PuntosPendientes[0];

            if (!venta.HasDetalle())
                return new PuntosPendientes[] { GenerarPuntoPendienteVentaSinDetalle(venta) };

            var puntosPendientes = new List<PuntosPendientes>();
            foreach (var item in venta.Detalle)
            {
                var familia = item.Farmaco.Familia?.Nombre ?? FAMILIA_DEFAULT;
                var puntoPendiente = new PuntosPendientes
                {
                    VentaId = $"{venta.FechaHora.Year}{venta.Id}".ToLongOrDefault(),
                    LineaNumero = item.Linea,
                    CodigoBarra = item.Farmaco.CodigoBarras ?? "847000" + item.Farmaco.Codigo.PadLeft(6, '0'),
                    CodigoNacional = item.Farmaco.Codigo,
                    Descripcion = item.Farmaco.Denominacion,

                    Familia = familia,
                    SuperFamilia = _clasificacion == TIPO_CLASIFICACION_CATEGORIA
                        ? item.Farmaco.Categoria?.Nombre ?? FAMILIA_DEFAULT
                        : string.Empty,
                    SuperFamiliaAux = string.Empty,
                    FamiliaAux = familia,
                    CambioClasificacion = _clasificacion == TIPO_CLASIFICACION_CATEGORIA ? 1 : 0,

                    Cantidad = item.Cantidad,
                    Precio = item.Importe,
                    Pago = item.Equals(venta.Detalle.First()) ? venta.TotalBruto : 0,
                    TipoPago = venta.Tipo,
                    Fecha = venta.FechaHora.Date.ToDateInteger(),
                    DNI = venta.Cliente?.Id.ToString() ?? "0",
                    Cargado = _cargarPuntos.ToLower().Equals("si") ? "no" : "si",
                    Puesto = $"{venta.Puesto}",
                    Trabajador = venta.VendedorNombre,
                    LaboratorioCodigo = item.Farmaco.Laboratorio?.Codigo ?? string.Empty,
                    Laboratorio = item.Farmaco.Laboratorio?.Nombre ?? LABORATORIO_DEFAULT,
                    Proveedor = item.Farmaco.Proveedor?.Nombre ?? string.Empty,
                    Receta = item.Receta,
                    FechaVenta = venta.FechaHora,
                    PVP = item.PVP,
                    PUC = item.Farmaco?.PrecioCoste ?? 0,
                    Categoria = item.Farmaco.Categoria?.Nombre ?? string.Empty,
                    Subcategoria = item.Farmaco.Subcategoria?.Nombre ?? string.Empty,
                    VentaDescuento = item.Equals(venta.Detalle.First()) ? venta.TotalDescuento : 0,
                    LineaDescuento = item.Descuento,
                    TicketNumero = venta.Ticket?.Numero,
                    Serie = venta.Ticket?.Serie ?? string.Empty,
                    Sistema = SISTEMA_UNYCOP
                };

                puntosPendientes.Add(puntoPendiente);
            }

            return puntosPendientes;
        }

        private PuntosPendientes GenerarPuntoPendienteVentaSinDetalle(Venta venta)
        {
            return new PuntosPendientes
            {
                VentaId = $"{venta.FechaHora.Year}{venta.Id}".ToLongOrDefault(),
                LineaNumero = 1,
                CodigoBarra = string.Empty,
                CodigoNacional = "9999999",
                Descripcion = "Pago Deposito",

                Familia = FAMILIA_DEFAULT,
                SuperFamilia = _clasificacion == TIPO_CLASIFICACION_CATEGORIA
                    ? FAMILIA_DEFAULT
                    : string.Empty,
                SuperFamiliaAux = string.Empty,
                FamiliaAux = FAMILIA_DEFAULT,
                CambioClasificacion = _clasificacion == TIPO_CLASIFICACION_CATEGORIA ? 1 : 0,

                Cantidad = 0,
                Precio = venta.Importe,
                Pago = venta.TotalBruto,
                TipoPago = venta.Tipo,
                Fecha = venta.FechaHora.Date.ToDateInteger(),
                DNI = venta.Cliente?.Id.ToString() ?? "0",
                Cargado = _cargarPuntos.ToLower().Equals("si") ? "no" : "si",
                Puesto = $"{venta.Puesto}",
                Trabajador = venta.VendedorNombre,
                LaboratorioCodigo = string.Empty,
                Laboratorio = LABORATORIO_DEFAULT,
                Proveedor = string.Empty,
                Receta = string.Empty,
                FechaVenta = venta.FechaHora,
                PVP = 0,
                PUC = 0,
                Categoria = string.Empty,
                Subcategoria = string.Empty,
                VentaDescuento = venta.TotalDescuento,
                LineaDescuento = 0,
                TicketNumero = venta.Ticket?.Numero,
                Serie = venta.Ticket?.Serie ?? string.Empty,
                Sistema = SISTEMA_UNYCOP
            };
        }
    }
}
