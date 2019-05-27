using Sisfarma.RestClient;
using Sisfarma.RestClient.Exceptions;
using Sisfarma.Sincronizador.Domain.Core.ExternalServices.Fisiotes;
using Sisfarma.Sincronizador.Domain.Entities.Fisiotes;
using Sisfarma.Sincronizador.Infrastructure.Fisiotes;
using System;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.ExternalServices
{
    public class PedidosExternalService : FisiotesExternalService, IPedidosExternalService
    {
        public PedidosExternalService(IRestClient restClient, FisiotesConfig config) 
            : base(restClient, config)
        { }

        public bool Exists(int pedido)
        {
            throw new NotImplementedException();
        }

        public bool ExistsLinea(int pedido, int linea)
        {
            throw new NotImplementedException();
        }

        public Pedido Get(int pedido)
        {
            throw new NotImplementedException();
        }

        public LineaPedido GetLineaByKey(int pedido, int linea)
        {
            throw new NotImplementedException();
        }

        public void Sincronizar(Pedido pp)
        {
            throw new NotImplementedException();
        }

        public void Sincronizar(LineaPedido ll)
        {
            throw new NotImplementedException();
        }

        public Pedido LastOrDefault()
        {
            try
            {
                return _restClient
                .Resource(_config.Pedidos.Ultimo)
                .SendGet<Pedido>();
            }
            catch (RestClientNotFoundException)
            {
                return null;
            }
        }
    }
}
