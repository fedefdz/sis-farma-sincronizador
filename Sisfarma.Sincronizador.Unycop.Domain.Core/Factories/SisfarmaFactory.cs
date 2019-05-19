using Sisfarma.RestClient.RestSharp;
using Sisfarma.Sincronizador.Domain.Core.Services;
using Sisfarma.Sincronizador.Infrastructure.Fisiotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sisfarma.Sincronizador.Unycop.Domain.Core.Factories
{
    public static class SisfarmaFactory
    {
        private static readonly string host = "http://localhost:8080/farmaciafedcm/";
        private static readonly string token = "372ea69830809e005bf55c507b0678ca";

        public static SisfarmaService Create()
        {
            var configuraciones = new ConfiguracionesExternalService(new RestClient.RestSharp.RestClient(), FisiotesConfig.TestConfig(host, token));
            
            return new SisfarmaService(
                configuraciones: configuraciones);
        }
    }
}
