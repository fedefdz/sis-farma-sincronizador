namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia.DTO
{
    public class Farmaco
    {
        public int Id { get; set; }

        public float? PrecioMedio { get; set; }

        public float? PrecioUnicoEntrada { get; set; }

        public byte Familia { get; set; }

        public int? CategoriaId { get; set; }

        public int? SubcategoriaId { get; set; }

        public string Laboratorio { get; set; }

        public string Denominacion { get; set; }
    }
}
