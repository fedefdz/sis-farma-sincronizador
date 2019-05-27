using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sisfarma.Sincronizador.Domain.Core.Services;
using DC = Sisfarma.Sincronizador.Domain.Core.Sincronizadores;

namespace Sisfarma.Sincronizador.Unycop.Domain.Core.Sincronizadores
{
    public class PedidoSincronizador : DC.PedidoSincronizador
    {
        public PedidoSincronizador(IFarmaciaService farmacia, ISisfarmaService fisiotes) 
            : base(farmacia, fisiotes)
        { }

        public override void Process()
        {            
            //var recepciones = (_lastPedido == null)
            //    ? _farmacia.Recepciones.GetByYear(_anioInicio)
            //    : _farmacia.Recepciones.GetByIdAndYear(_anioInicio, _lastPedido.idPedido);

            //foreach (var recepcion in recepciones)
            //{
            //    Task.Delay(5);

            //    _cancellationToken.ThrowIfCancellationRequested();

            //    var resume = _farmatic.Recepciones.GetResumeById(recepcion.IdRecepcion);
            //    if (resume.numLineas > 0)
            //    {
            //        //if (!_fisiotes.Pedidos.Exists(recepcion.IdRecepcion))
            //        //{
            //        _fisiotes.Pedidos.Insert(GenerarPedido(_farmatic, recepcion, resume));
            //        if (_lastPedido == null)
            //            _lastPedido = new Fisiotes.Models.Pedido();

            //        _lastPedido.idPedido = recepcion.IdRecepcion;
            //        //}

            //        var lineas = _farmatic.Recepciones.GetLineasById(recepcion.IdRecepcion)
            //                .Where(l => !string.IsNullOrEmpty(l.XArt_IdArticu));

            //        foreach (var linea in lineas)
            //        {
            //            Task.Delay(1);

            //            var articulo = _farmatic.Articulos.GetOneOrDefaultById(linea.XArt_IdArticu);
            //            if (articulo != null /*&& !_fisiotes.Pedidos.ExistsLinea(linea.IdRecepcion, linea.IdNLinea)*/)
            //                _fisiotes.Pedidos.InsertLinea(GenerarLineaDePedido(_farmatic, recepcion, linea, articulo, _consejo));
            //        }
            //    }
            //}
        }

        //private Fisiotes.Models.LineaPedido GenerarLineaDePedido(FarmaciaService farmatic, Recepcion recepcion, LineaRecepcion linea, Articulo articulo, ConsejoService consejo)
        //{
        //    var puc = linea.ImportePuc;
        //    var pvp = linea.ImportePvp;

        //    var familia = farmatic.Familias.GetById(articulo.XFam_IdFamilia)?.Descripcion;
        //    if (string.IsNullOrWhiteSpace(familia))
        //        familia = FAMILIA_DEFAULT;

        //    var superFamilia = !familia.Equals(FAMILIA_DEFAULT)
        //        ? farmatic.Familias.GetSuperFamiliaDescripcionByFamilia(familia) ?? FAMILIA_DEFAULT
        //        : familia;

        //    var codLaboratorio = articulo.Laboratorio ?? string.Empty;
        //    var nombreLaboratorio = GetNombreLaboratorioFromLocalOrDefault(farmatic, consejo, codLaboratorio, LABORATORIO_DEFAULT);

        //    return new Fisiotes.Models.LineaPedido
        //    {
        //        idPedido = linea.IdRecepcion,
        //        idLinea = linea.IdNLinea,
        //        fechaPedido = recepcion.Hora,
        //        cod_nacional = Convert.ToInt64(articulo.IdArticu.Strip()),
        //        descripcion = articulo.Descripcion.Strip(),
        //        familia = familia.Strip(),
        //        superFamilia = superFamilia.Strip(),
        //        cantidad = linea.Recibidas - linea.UDevolver,
        //        pvp = Convert.ToSingle(pvp),
        //        puc = Convert.ToSingle(puc),
        //        cod_laboratorio = codLaboratorio.Strip(),
        //        laboratorio = nombreLaboratorio.Strip()
        //    };
        //}

        //private Fisiotes.Models.Pedido GenerarPedido(FarmaciaService farmatic, Recepcion recepcion, RecepcionResume resume)
        //{
        //    var proveedor = farmatic.Proveedores.GetOneOrDefault(recepcion.XProv_IdProveedor)?.FIS_NOMBRE
        //                    ?? string.Empty;

        //    var trabajador = farmatic.Vendedores.GetOneOrDefaultById(Convert.ToInt16(recepcion.XVend_IdVendedor))?.NOMBRE
        //        ?? string.Empty;

        //    return new Fisiotes.Models.Pedido
        //    {
        //        idPedido = recepcion.IdRecepcion,
        //        fechaPedido = recepcion.Hora,
        //        hora = DateTime.Now,
        //        numLineas = resume.numLineas,
        //        importePvp = Convert.ToSingle(resume.importePvp),
        //        importePuc = Convert.ToSingle(resume.importePuc),
        //        idProveedor = recepcion.XProv_IdProveedor,
        //        proveedor = proveedor,
        //        trabajador = trabajador
        //    };
        //}

        //private string GetNombreLaboratorioFromLocalOrDefault(FarmaciaService farmaticService, ConsejoService consejoService, string codigo, string byDefault = "")
        //{
        //    var nombreLaboratorio = byDefault;
        //    if (!string.IsNullOrEmpty(codigo?.Trim()) && !string.IsNullOrWhiteSpace(codigo))
        //    {
        //        var laboratorioDb = default(Consejo.Models.Labor); //consejoService.Laboratorios.Get(codigo);
        //        if (laboratorioDb == null)
        //        {
        //            var laboratorioLocal =
        //                farmaticService.Laboratorios.GetById(codigo);
        //            nombreLaboratorio = laboratorioLocal?.Nombre ?? byDefault;
        //        }
        //        else nombreLaboratorio = laboratorioDb.NOMBRE;
        //    }
        //    else nombreLaboratorio = byDefault;
        //    return nombreLaboratorio;
        //}        
    }
}
