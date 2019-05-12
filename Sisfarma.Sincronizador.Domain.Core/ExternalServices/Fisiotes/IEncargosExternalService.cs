using System;
using Sisfarma.Sincronizador.Domain.Entities.Fisiotes;

namespace Sisfarma.Sincronizador.Domain.Core.ExternalServices.Fisiotes
{
    public interface IEncargosExternalService
    {
        bool Exists(int encargo);
        Encargo GetByEncargoOrDefault(int encargo);
        void Insert(Encargo ee);
        Encargo LastOrDefault();
        void UpdateFechaDeEntrega(DateTime fechaEntrega, long idEncargo);
        void UpdateFechaDeRecepcion(DateTime fechaRecepcion, long idEncargo);
    }
}