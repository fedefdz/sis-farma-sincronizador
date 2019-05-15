using Sisfarma.Sincronizador.Domain.Core.Services;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sisfarma.Sincronizador.Unycop.Domain.Core.Factories
{
    public static class FarmaciaFactory
    {
        public static FarmaciaService Create()
        {            
            return new FarmaciaService(
                categorias: new CategoriasRepository(),
                
                familias: new FamiliaRepository(),
                
                ventas: new VentasRepository(
                        clientesRepository: new ClientesRepository(
                                ventasPremium: new VentasPremiumRepository()),
                        ticketRepository: new TicketRepository(),
                        vendedoresRepository: new VendedoresRepository(),
                        farmacoRepository: new FarmacoRespository(),
                        barraRepository: new CodigoBarraRepository(),
                        proveedorRepository: new ProveedoresRepository(
                                recepcionRespository: new RecepcionRespository()),
                        categoriaRepository: new CategoriaRepository(),
                        familiaRepository: new FamiliaRepository(),
                        laboratorioRepository: new LaboratorioRepository()),
                
                clientes: new ClientesRepository(
                        ventasPremium: new VentasPremiumRepository()));
        }
    }
}
