using System.Collections.Generic;
using Sisfarma.Sincronizador.Domain.Entities.Fisiotes;

namespace Sisfarma.Sincronizador.Domain.Core.ExternalServices.Fisiotes
{
    public interface IListasArticulosExternalService
    {
        void Delete(int codigo);
        void Insert(List<ListaArticulo> items);
        void Insert(ListaArticulo la);
    }
}