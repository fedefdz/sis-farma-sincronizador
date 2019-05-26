using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using System.Collections.Generic;

namespace Sisfarma.Sincronizador.Domain.Core.Repositories.Farmacia
{
    public interface ICategoriasRepository
    {
        IEnumerable<Categoria> GetAll();

        IEnumerable<Categoria> GetByDescripcion();

        Categoria GetById(short id);

        IEnumerable<string> GetAllNombreSubcategoriaByCategoriaId(long id);

        string GetSuperFamiliaDescripcionById(short familia);
        string GetSubCategoriaById(string v);
    }
}
