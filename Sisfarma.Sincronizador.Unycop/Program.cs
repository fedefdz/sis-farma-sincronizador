using Microsoft.Win32;
using Sisfarma.ClickOnce;
using Sisfarma.RestClient.RestSharp;
using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Domain.Core.Sincronizadores;
using Sisfarma.Sincronizador.Infrastructure.Fisiotes;
using Sisfarma.Sincronizador.Unycop.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sisfarma.Sincronizador.Unycop
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            if (!AppProcessHelper.SetSingleInstance())
            {
                Environment.Exit(-1);
            }

            ServicePointManager.DefaultConnectionLimit = 100;

            RegisterStartup(Globals.ProductName);
            var clickOnce = new ClickOnceHelper(Globals.PublisherName, Globals.ProductName);
            clickOnce.UpdateUninstallParameters();

            string
                _remoteServer = string.Empty,
                _remoteToken = string.Empty;

            //LeerFicherosConfiguracion(ref _remoteServer, ref _remoteToken);

            //RemoteConfig.Setup(_remoteServer, _remoteToken);
            //LocalConfig.Setup(GetConnexionLocal());           

            //Task.Factory.StartNew(() => new PowerSwitchProgramado(FisiotesFactory.New()).SincronizarAsync(Updater.GetCancellationToken(), delayLoop: 60000));
            //Task.Factory.StartNew(() => new PowerSwitchManual(FisiotesFactory.New()).SincronizarAsync(Updater.GetCancellationToken(), delayLoop: 60000));
            //Task.Factory.StartNew(() => new UpdateVersionSincronizador().SincronizarAsync(new CancellationToken(), delayLoop: 200));

            var notifyIcon = new NotifyIcon();
            notifyIcon.ContextMenuStrip = GetSincronizadorMenuStrip();
            notifyIcon.Icon = Resources.sync;
            notifyIcon.Visible = true;
            Application.ApplicationExit += ApplicationExit;
            Application.ApplicationExit += (sender, @event) => notifyIcon.Visible = false;
            Application.Run(new SincronizadorApplication());


        }

        private static void ApplicationExit(object sender, EventArgs e)
        {
            // last change for cleanup code here!

            // only restart if user requested, not an unhandled app exception...
            AppProcessHelper.RestartIfRequired();
        }

        private static ContextMenuStrip GetSincronizadorMenuStrip()
        {
            var cms = new ContextMenuStrip();
            cms.Items.Add($"Salir {ApplicationDeployment.CurrentDeployment.CurrentVersion}", null, (sender, @event) => Application.Exit());
            //cms.Items.Add($"Salir", null, (sender, @event) => Application.Exit());
            return cms;
        }

        private static void LeerFicherosConfiguracion(
            ref string _remoteServer,
            ref string _remoteToken)
        {
            try
            {
                var dir = ConfigurationManager.AppSettings["Directory.Setup"];

                var path = ConfigurationManager.AppSettings["File.Remote.Server"];
                var stream = new StreamReader(Path.Combine(dir, path));
                _remoteServer = stream.ReadLine();
                _remoteToken = stream.ReadLine();
            }
            catch (IOException)
            {
                throw new IOException("Ha habido un error en la lectura de algún fichero de configuración. Compruebe que existen dichos ficheros de configuración.");
            }
        }

        internal static void RegisterStartup(string productName)
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
                return;

            var location = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                @"Sisfarma.es", @"Sisfarma", "Sincronizador.appref-ms");

            RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            reg.SetValue(productName, location);
        }

        private static ConexionLocal GetConnexionLocal()
        {
            try
            {
                var restClient = new RestClient.RestSharp.RestClient();

                var remote = RemoteConfig.GetSingletonInstance();
                var config = FisiotesConfig.TestConfig(remote.Server, remote.Token);

                var conn = restClient.BaseAddress(config.BaseAddress)
                    .UseAuthenticationBasic(config.Credentials.Token)
                    .Resource(config.Configuraciones.ConexionLocal)
                    .SendGet<ConexionLocal>();

                return conn;
            }
            catch (Exception)
            {
                return GetConnexionLocal();
            }

        }
    }
}
