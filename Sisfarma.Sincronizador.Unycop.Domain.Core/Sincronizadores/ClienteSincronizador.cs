using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sisfarma.Sincronizador.Domain.Core.Services;
using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using CORE = Sisfarma.Sincronizador.Domain.Core.Sincronizadores;

namespace Sisfarma.Sincronizador.Unycop.Domain.Core.Sincronizadores
{
    public class ClienteSincronizador : CORE.ClienteSincronizador
    {
        public ClienteSincronizador(IFarmaciaService farmacia, ISisfarmaService fisiotes) 
            : base(farmacia, fisiotes)
        { }

        public override void LoadConfiguration()
        {
            _sisfarma.Clientes.ResetDniTracking();
            Reset();
        }

        public override void PreSincronizacion()
        {
            base.PreSincronizacion();
        }

        public override void Process()
        {
            if (IsHoraVaciamientos())            
                Reset();

            List<Cliente> localClientes = _farmacia.Clientes.GetGreatThanId(_ultimoClienteSincronizado);

            var hueco = -1L;
            foreach (var cliente in localClientes)
            {
                Task.Delay(5);
                _cancellationToken.ThrowIfCancellationRequested();

                if (hueco == -1) hueco = cliente.Id;

                InsertOrUpdateCliente(cliente);
                
                if (cliente.Id != hueco)
                {
                    var huecos = new List<string>();
                    for (long i = hueco; i < cliente.Id; i++)
                    {
                        huecos.Add(i.ToString());
                    }

                    if (huecos.Any())
                        _sisfarma.Huecos.Insert(huecos.ToArray());

                    hueco = cliente.Id;
                }
                hueco++;
            }
        }


        private void InsertOrUpdateCliente(Cliente cliente)
        {            
            if (_perteneceFarmazul)
            {
                var beBlue = _farmacia.Clientes.EsBeBlue($"{cliente.Id}");
                _sisfarma.Clientes.Sincronizar(cliente, beBlue, _debeCargarPuntos);
            }
            else _sisfarma.Clientes.Sincronizar(cliente, _debeCargarPuntos);

            _ultimoClienteSincronizado = cliente.Id;
        }            
    }
}
