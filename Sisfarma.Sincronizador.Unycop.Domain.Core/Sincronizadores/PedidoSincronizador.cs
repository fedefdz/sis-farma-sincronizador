using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sisfarma.Sincronizador.Domain.Core.Services;
using Sisfarma.Sincronizador.Domain.Entities.Fisiotes;

using DC = Sisfarma.Sincronizador.Domain.Core.Sincronizadores;
using FAR = Sisfarma.Sincronizador.Domain.Entities.Farmacia;

namespace Sisfarma.Sincronizador.Unycop.Domain.Core.Sincronizadores
{
    public class PedidoSincronizador : DC.PedidoSincronizador
    {
        public PedidoSincronizador(IFarmaciaService farmacia, ISisfarmaService fisiotes) 
            : base(farmacia, fisiotes)
        { }

        public override void PreSincronizacion()
        {
            base.PreSincronizacion();
        }

        public override void Process()
        {
            var recepciones = (_lastPedido == null)
                ? _farmacia.Recepciones.GetAllByYear(_anioInicio)
                : _farmacia.Recepciones.GetAllByDate(_lastPedido.fechaPedido ?? DateTime.MinValue);

            if (!recepciones.Any())
            {
                _anioInicio++;
                _lastPedido = null;
                return;
            }
            

            foreach (var recepcion in recepciones)
            {
                Task.Delay(5);

                _cancellationToken.ThrowIfCancellationRequested();
         
                if (recepcion.Lineas > 0)
                {                    
                    _sisfarma.Pedidos.Sincronizar(GenerarPedido(recepcion));
                    if (_lastPedido == null)
                        _lastPedido = new Pedido();

                    _lastPedido.idPedido = recepcion.Id;
                    _lastPedido.fechaPedido = recepcion.Fecha;
                    
                    foreach (var linea in recepcion.Detalle)
                    {
                        Task.Delay(1);
                        
                        if (linea.Farmaco != null)
                            _sisfarma.Pedidos.Sincronizar(GenerarLineaDePedido(linea));
                    }
                }
            }
        }

        private LineaPedido GenerarLineaDePedido(FAR.RecepcionDetalle detalle)
        {            
            return new LineaPedido
            {
                idPedido = detalle.RecepcionId,
                idLinea = detalle.Linea,
                fechaPedido = detalle.Recepcion.Fecha,
                cod_nacional = detalle.Farmaco.Id,
                descripcion = detalle.Farmaco.Denominacion,
                familia = detalle.Farmaco.Familia?.Nombre ?? FAMILIA_DEFAULT,
                categoria = detalle.Farmaco.Categoria?.Nombre ?? string.Empty,
                subcategoria = detalle.Farmaco.Subcategoria?.Nombre ?? string.Empty,
                cantidad = detalle.Cantidad,
                cantidadBonificada = detalle.CantidadBonificada,
                pvp = (float) (detalle.Farmaco?.Precio ?? 0),
                puc = (float) (detalle.Farmaco?.PrecioCoste ?? 0),
                cod_laboratorio = detalle.Farmaco?.Laboratorio?.Codigo ?? "0",
                laboratorio = detalle.Farmaco?.Laboratorio?.Nombre ?? LABORATORIO_DEFAULT,
                proveedor = detalle.Farmaco?.Proveedor?.Nombre ?? string.Empty
            };
        }

        private Pedido GenerarPedido(FAR.Recepcion recepcion)
        {            
            return new Pedido
            {
                idPedido = recepcion.Id,
                fechaPedido = recepcion.Fecha,
                hora = DateTime.Now,
                numLineas = recepcion.Lineas,
                importePvp = (float) recepcion.ImportePVP,
                importePuc = (float)recepcion.ImportePUC,
                idProveedor = recepcion.Proveedor?.Id.ToString() ?? "0",
                proveedor = recepcion.Proveedor?.Nombre ?? string.Empty,
                trabajador = string.Empty // no se envía trabajador
            };
        }        
    }
}
