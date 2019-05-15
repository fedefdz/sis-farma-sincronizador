using Sisfarma.Sincronizador.Core.Config;
using System;
using System.Data.Common;
using System.Data.Entity;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Data
{
    public class FarmaciaContext : DbContext
    {        
        public FarmaciaContext(string server, string database, string username, string password)
            : base($@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = {database};Jet OLEDB:Database Password = {password};")
        { }

        public static FarmaciaContext Create(LocalConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            return new FarmaciaContext(config.Server, config.Database, config.Username, config.Password);
        }

        public static FarmaciaContext Default()
        {
            return new FarmaciaContext(
                server: "", 
                database: @"C:\Users\Federico\Documents\sisfarma\sincronizador\access\DATOS UNYCOP\DATOS UNYCOP\Tablas.accdb", 
                username: "", 
                password: "BIGOTES");
        }

        public static FarmaciaContext Ventas()
        {
            return new FarmaciaContext(
                server: "",
                database: @"C:\Users\Federico\Documents\sisfarma\sincronizador\access\DATOS UNYCOP\DATOS UNYCOP\Ventas.accdb",
                username: "",
                password: "BIGOTES");
        }

        public static FarmaciaContext Clientes()
        {
            return new FarmaciaContext(
                server: "",
                database: @"C:\Users\Federico\Documents\sisfarma\sincronizador\access\DATOS UNYCOP\DATOS UNYCOP\Clientes.accdb",
                username: "",
                password: "BIGOTES");
        }

        public static FarmaciaContext Fidelizacion()
        {
            return new FarmaciaContext(
                server: "",
                database: @"C:\Users\Federico\Documents\sisfarma\sincronizador\access\DATOS UNYCOP\DATOS UNYCOP\Fidelizacion.accdb",
                username: "",
                password: "BIGOTES");
        }

        public static FarmaciaContext Vendedor()
        {
            return new FarmaciaContext(
                server: "",
                database: @"C:\Users\Federico\Documents\sisfarma\sincronizador\access\DATOS UNYCOP\DATOS UNYCOP\Vendedor.accdb",
                username: "",
                password: "BIGOTES");
        }

        public static FarmaciaContext Farmacos()
        {
            return new FarmaciaContext(
                server: "",
                database: @"C:\Users\Federico\Documents\sisfarma\sincronizador\access\DATOS UNYCOP\DATOS UNYCOP\Farmacos.accdb",
                username: "",
                password: "BIGOTES");
        }

        public static FarmaciaContext Recepcion()
        {
            return new FarmaciaContext(
                server: "",
                database: @"C:\Users\Federico\Documents\sisfarma\sincronizador\access\DATOS UNYCOP\DATOS UNYCOP\FarmaDen.accdb",
                username: "",
                password: "BIGOTES");
        }

        public static FarmaciaContext Proveedores()
        {
            return new FarmaciaContext(
                server: "",
                database: @"C:\Users\Federico\Documents\sisfarma\sincronizador\access\DATOS UNYCOP\DATOS UNYCOP\Proveedo.accdb",
                username: "",
                password: "BIGOTES");
        }
    }
}