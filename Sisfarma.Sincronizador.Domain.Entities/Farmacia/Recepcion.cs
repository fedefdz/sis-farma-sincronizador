using System;

namespace Sisfarma.Sincronizador.Domain.Entities.Farmacia
{    
    public partial class Recepcion
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime Hora { get; set; }

        public string XProv_IdProveedor { get; set; }

        public short? XCPro_IdCondicion { get; set; }

        public short XVend_IdVendedor { get; set; }

        public int Actualizado { get; set; }

        public short? Ejercicio { get; set; }

        public short? Mes { get; set; }

    }
}
