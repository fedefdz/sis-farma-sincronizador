namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia.DTO
{
    public class Farmaco
    {
        public long Id { get; set; }

        public decimal? PrecioMedio { get; set; }

        public decimal? PrecioUnicoEntrada { get; set; }

        public long FamiliaId { get; set; }

        public long? CategoriaId { get; set; }

        public long? SubcategoriaId { get; set; }

        public string Laboratorio { get; set; }

        public string Denominacion { get; set; }
    }
}
