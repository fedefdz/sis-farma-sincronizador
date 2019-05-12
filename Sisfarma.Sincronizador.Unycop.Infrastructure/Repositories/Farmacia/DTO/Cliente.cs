namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia.DTO
{
    internal class Cliente
    {     
        public long Id { get; set; }

        public string Tarjeta { get; set; }

        public string EstadoCivil { get; set; }

        public string Celular { get; set; }

        public string Telefono { get; set; }

        public string Email { get; set; }

        public long FechaNacimiento { get; set; }

        public long Puntos { get; set; }

        public string NumeroIdentificacion { get; set; }

        public short LOPD { get; set; }

        public string Sexo { get; set; }

        public long Baja { get; set; }

        public long FechaAlta { get; set; }

        public string Direccion { get; set; }

        public string Localidad { get; set; }

        public string CodigoPostal { get; set; }

        public string NombreCompleto { get; set; }
    }
}
