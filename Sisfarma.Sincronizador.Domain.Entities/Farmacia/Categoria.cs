namespace Sisfarma.Sincronizador.Domain.Entities.Farmacia
{
    public class Categoria
    {
        public long Id { get; set; }

        public string Codigo { get; set; }

        public string Nombre { get; set; }

        public Familia Familia { get; set; }
    }
}
