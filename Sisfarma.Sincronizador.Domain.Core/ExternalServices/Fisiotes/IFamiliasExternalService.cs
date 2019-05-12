using Sisfarma.Sincronizador.Domain.Entities.Fisiotes;

namespace Sisfarma.Sincronizador.Domain.Core.ExternalServices.Fisiotes
{
    public interface IFamiliasExternalService
    {
        bool Exists(string familia);
        Familia GetByFamilia(string familia);
        decimal GetPuntosByFamiliaTipoVerificado(string familia);
        void Insert(Familia ff);
    }
}