using System;
using Sisfarma.Sincronizador.Domain.Core.Repositories.Farmacia;

namespace Sisfarma.Sincronizador.Domain.Core.Services
{
    public interface IFarmaciaService
    {
        ICategoriasRepository Categorias { get; }

        IFamiliaRepository Familias { get; }

        IVentasRepository Ventas { get; }

        IClientesRepository Clientes { get; }

        IVendedoresRepository Vendedores { get; }
    }

    public class FarmaciaService : IFarmaciaService
    {
        public ICategoriasRepository Categorias { get; }

        public IFamiliaRepository Familias { get; }

        public IVentasRepository Ventas { get; }

        public IClientesRepository Clientes { get; }

        public IVendedoresRepository Vendedores { get; }

        public FarmaciaService(
            ICategoriasRepository categorias,
            IFamiliaRepository familias,
            IVentasRepository ventas,
            IClientesRepository clientes)
        {
            Categorias = categorias ?? throw new ArgumentNullException(nameof(categorias));
            Familias = familias ?? throw new ArgumentNullException(nameof(familias));
            Ventas = ventas ?? throw new ArgumentNullException(nameof(ventas));
            Clientes = clientes ?? throw new ArgumentNullException(nameof(clientes));
        }
    }
}
