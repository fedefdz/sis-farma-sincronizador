using Sisfarma.Sincronizador.Domain.Entities.Fisiotes;

namespace Sisfarma.Sincronizador.Domain.Core.ExternalServices.Fisiotes
{
    public interface IPedidosExternalService
    {
        bool Exists(int pedido);
        bool ExistsLinea(int pedido, int linea);
        Pedido Get(int pedido);
        LineaPedido GetLineaByKey(int pedido, int linea);
        void Insert(Pedido pp);
        void InsertLinea(LineaPedido ll);
        Pedido LastOrDefault();
    }
}