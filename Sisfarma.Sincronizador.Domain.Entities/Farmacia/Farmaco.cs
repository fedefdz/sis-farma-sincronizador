namespace Sisfarma.Sincronizador.Domain.Entities.Farmacia
{
    public class Farmaco
    {
        public long Id { get; set; }

        public string Codigo { get; set; }

        public string CodigoBarras { get; set; }

        public string Denominacion { get; set; }

        public decimal PrecioCoste { get; set; }        

        public Familia Familia { get; set; }

        public Categoria Categoria { get; set; }

        public Subcategoria Subcategoria { get; set; }

        public Proveedor Proveedor { get; set; }

        public Laboratorio Laboratorio { get; set; }

        public bool HasProveedor() => Proveedor != null;

        public bool HasCategoria() => Categoria != null;

        public bool HasSubcategoria() => Subcategoria != null;

        public bool HasFamilia() => Familia != null;

        public bool HasLaboratorio => Laboratorio != null;
    }
}
