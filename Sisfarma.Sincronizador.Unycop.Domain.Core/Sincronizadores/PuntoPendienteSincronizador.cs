using Sisfarma.Sincronizador.Core.Extensions;
using Sisfarma.Sincronizador.Domain.Core.Services;
using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using Sisfarma.Sincronizador.Domain.Entities.Fisiotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC = Sisfarma.Sincronizador.Domain.Core.Sincronizadores;
using Farmacia = Sisfarma.Sincronizador.Domain.Entities.Farmacia;

namespace Sisfarma.Sincronizador.Unycop.Domain.Core.Sincronizadores
{
    public class PuntoPendienteSincronizador : DC.PuntoPendienteSincronizador
    {
        protected const string TIPO_CLASIFICACION_DEFAULT = "Familia";
        protected const string TIPO_CLASIFICACION_CATEGORIA = "Categoria";
        protected const string SISTEMA_UNYCOP = "unycop";

        private string _clasificacion;

        private ICollection<int> _aniosProcesados;


        public PuntoPendienteSincronizador(IFarmaciaService farmacia, ISisfarmaService fisiotes) 
            : base(farmacia, fisiotes)
        {
            _aniosProcesados = new HashSet<int>();
        }

        public override void LoadConfiguration()
        {
            base.LoadConfiguration();
            _clasificacion = !string.IsNullOrWhiteSpace(ConfiguracionPredefinida[Configuracion.FIELD_TIPO_CLASIFICACION])
                ? ConfiguracionPredefinida[Configuracion.FIELD_TIPO_CLASIFICACION]
                : TIPO_CLASIFICACION_DEFAULT;            
        }

        public override void Process()
        {
            var anioProcesando = _aniosProcesados.Any() ? _aniosProcesados.Last() : 2016;//_anioInicio;
                
            var ventas = _farmacia.Ventas.GetAllByIdGreaterOrEqual(anioProcesando, _ultimaVenta);

            foreach (var venta in ventas)
            {
                Task.Delay(5).Wait();
                _cancellationToken.ThrowIfCancellationRequested();

                if (venta.HasCliente())
                    InsertOrUpdateCliente(venta.Cliente);

                //var puntosPendientes = GenerarPuntosPendientes(venta);
                //foreach (var puntoPendiente in puntosPendientes)
                //{
                //    _fisiotes.PuntosPendientes.Insert(puntoPendiente);
                //}

                _ultimaVenta = venta.Id;
            }

            // <= 1 porque las ventas se recuperan con >= ventaID
            if (ventas.Count() <= 1)
            {
                _aniosProcesados.Add(anioProcesando + 1);
                _ultimaVenta = 1;
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

        private void InsertOrUpdateCliente(Farmacia.Cliente cliente)
        {                        
            var debeCargarPuntos = _puntosDeSisfarma.ToLower().Equals("no") || string.IsNullOrWhiteSpace(_puntosDeSisfarma);

            if (_perteneceFarmazul)
            {                
                var beBlue = _farmacia.Clientes.EsBeBlue($"{cliente.Id}");
                _fisiotes.Clientes.Sincronizar(cliente, beBlue, debeCargarPuntos);                
            }
            else _fisiotes.Clientes.Sincronizar(cliente, debeCargarPuntos);            
        }        
    }
}
