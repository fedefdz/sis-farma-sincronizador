using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using System.Collections.Generic;

namespace Sisfarma.Sincronizador.Domain.Core.Repositories.Farmacia
{
    public interface IRecepcionRepository
    {
        IEnumerable<Recepcion> GetByYear(int anioInicio);

        IEnumerable<Recepcion> GetByIdAndYear(int anioInicio, long idPedido);
    }
}
