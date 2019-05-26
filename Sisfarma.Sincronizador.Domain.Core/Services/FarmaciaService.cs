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

        IFarmacosRepository Farmacos { get; }

        IPedidosRepository Pedidos { get; }

        IEncargosRepository Encargos { get; }
    }

    public class FarmaciaService : IFarmaciaService
    {
        public ICategoriasRepository Categorias { get; }

        public IFamiliaRepository Familias { get; }

        public IVentasRepository Ventas { get; }

        public IClientesRepository Clientes { get; }

        public IVendedoresRepository Vendedores { get; }

        public IFarmacosRepository Farmacos { get; }

        public IPedidosRepository Pedidos { get; }

        public IEncargosRepository Encargos { get; }

        public FarmaciaService(
            ICategoriasRepository categorias,
            IFamiliaRepository familias,
            IVentasRepository ventas,
            IClientesRepository clientes,
            IFarmacosRepository farmacos,
            IPedidosRepository pedidos,
            IEncargosRepository encargos)
        {
            Categorias = categorias ?? throw new ArgumentNullException(nameof(categorias));
            Familias = familias ?? throw new ArgumentNullException(nameof(familias));
            Ventas = ventas ?? throw new ArgumentNullException(nameof(ventas));
            Clientes = clientes ?? throw new ArgumentNullException(nameof(clientes));
            Farmacos = farmacos ?? throw new ArgumentNullException(nameof(farmacos));
            Pedidos = pedidos ?? throw new ArgumentNullException(nameof(pedidos));
            Encargos = encargos ?? throw new ArgumentNullException(nameof(encargos));
        }        
    }
}
