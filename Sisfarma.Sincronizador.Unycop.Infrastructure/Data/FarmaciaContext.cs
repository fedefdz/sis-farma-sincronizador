using Sisfarma.Sincronizador.Core.Config;
using System;
using System.Data.Entity;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Data
{
    public class FarmaciaContext : DbContext
    {        
        public FarmaciaContext(string server, string database, string username, string password)            
            : base($@"data source={server}; initial catalog={database}; persist security info=True;user id={username}; password={password};MultipleActiveResultSets=True;App=EntityFramework")
        { }

        public static FarmaciaContext Create(LocalConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            return new FarmaciaContext(config.Server, config.Database, config.Username, config.Password);
        }        
    }
}