using Sisfarma.Sincronizador.Core.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Data
{
    public class FarmaciaContext : DbContext
    {
        private static int _anioActual = 0;
        private static readonly string _pattern = @"Hst????.accdb";
        private static ICollection<int> _historicos;

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

        public static FarmaciaContext Ventas(int year)
        {
            _anioActual = year;
            return Ventas();

        }

        public static FarmaciaContext Ventas()
        {
            _historicos = GetHistoricos();

            if (_historicos.All(x => x > _anioActual))
                throw new FarmaciaContextException();

            if (_historicos.Contains(_anioActual))
            {
                return new FarmaciaContext(
                    server: "",
                    database: $@"C:\Users\Federico\Documents\sisfarma\sincronizador\access\DATOS UNYCOP\DATOS UNYCOP\Hst{_anioActual}.accdb",
                    username: "",
                    password: "BIGOTES");
            }


            return new FarmaciaContext(
                server: "",
                database: @"C:\Users\Federico\Documents\sisfarma\sincronizador\access\DATOS UNYCOP\DATOS UNYCOP\Ventas.accdb",
                username: "",
                password: "BIGOTES");
        }

        static ICollection<int> GetHistoricos()
        {
            if (_historicos == null)
            {
                var historicos = Directory.GetFiles(
                path: @"C:\Users\Federico\Documents\sisfarma\sincronizador\access\DATOS UNYCOP\DATOS UNYCOP",
                searchPattern: _pattern,
                searchOption: SearchOption.TopDirectoryOnly)
                    .Select(path => new string(path.Replace(".accdb", string.Empty).TakeLast(4).ToArray()))
                    .Where(yyyy => int.TryParse(yyyy, out var number))
                        .Select(anio => int.Parse(anio));

                _historicos = new HashSet<int>(historicos);
            }

            return _historicos;
            
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

    [Serializable]
    internal class FarmaciaContextException : Exception
    {
        public FarmaciaContextException()
        {
        }        
    }
}