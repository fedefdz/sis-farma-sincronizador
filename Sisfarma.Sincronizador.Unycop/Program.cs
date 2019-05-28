﻿using Microsoft.Win32;
using Sisfarma.ClickOnce;
using Sisfarma.Sincronizador.Domain.Core.Sincronizadores;
using Sisfarma.Sincronizador.Infrastructure.Fisiotes;
using Sisfarma.Sincronizador.Unycop.Domain.Core.Config;
using Sisfarma.Sincronizador.Unycop.Domain.Core.Factories;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using Sisfarma.Sincronizador.Unycop.IoC.Factories;
using Sisfarma.Sincronizador.Unycop.Properties;
using System;
using System.Configuration;
using System.Deployment.Application;
using System.IO;
using System.Net;
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

            Initialize();

            SincronizadorTaskManager.TaskSincronizadores
                .AddSincronizador(new Domain.Core.Sincronizadores.PuntoPendienteSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.ClienteSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create())
                            .SetHorarioVaciemientos("1500", "2300"),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.HuecoSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.CategoriaSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.ControlStockFechaEntradaSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.ControlStockFechaSalidaSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.ControlStockSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.ControlSinStockSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.ProductoCriticoSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.EncargoSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.FamiliaSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.ListaSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.SinonimoSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create())
                            .SetHorarioVaciamientos("1000", "1230", "1730", "1930"),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.PuntoPendienteActualizacionSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.TicketPendienteActualizacionSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.PagoPendienteActualizarSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1)
                .AddSincronizador(new Domain.Core.Sincronizadores.PedidoSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: 1);

            //Task.Factory.StartNew(() => new Domain.Core.Sincronizadores.SinonimoSincronizador(FarmaciaFactory.Create(), SisfarmaFactory.Create())
            //    .SetHorarioVaciamientos("1000", "1230", "1730", "1930")
            //        .SincronizarAsync(Updater.GetCancellationToken(), delayLoop: 1));
            Task.Factory.StartNew(() => new PowerSwitchProgramado(SisfarmaFactory.Create()).SincronizarAsync(Updater.GetCancellationToken(), delayLoop: 60000));
            Task.Factory.StartNew(() => new PowerSwitchManual(SisfarmaFactory.Create()).SincronizarAsync(Updater.GetCancellationToken(), delayLoop: 60000));
            Task.Factory.StartNew(() => new UpdateVersionSincronizador().SincronizarAsync(new CancellationToken(), delayLoop: 20000));

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
            //cms.Items.Add($"Salir {ApplicationDeployment.CurrentDeployment.CurrentVersion}", null, (sender, @event) => Application.Exit());
            cms.Items.Add($"Salir", null, (sender, @event) => Application.Exit());
            return cms;
        }

        private static void Initialize()
        {
            try
            {
                var dir = ConfigurationManager.AppSettings["Directory.Setup"];

                var path = ConfigurationManager.AppSettings["File.Remote.Server"];
                var stream = new StreamReader(Path.Combine(dir, path));
                var remoteServer = stream.ReadLine();
                var remoteToken = stream.ReadLine();
                SisfarmaFactory.Setup(remoteServer, remoteToken);

                var local = GetConnexionLocal(remoteServer, remoteToken);

                FarmaciaContext.Setup(local.pathFicheros, local.password);

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

        private static LocalConfiguracion GetConnexionLocal(string server, string token)
        {
            //return new LocalConfiguracion
            //{
            //    pathFicheros = @"C:\Users\Federico\Documents\sisfarma\sincronizador\access\DATOS UNYCOP\DATOS UNYCOP",
            //    password = "BIGOTES",
            //    marketCodeList = -1
            //};

            try
            {
                var restClient = new RestClient.RestSharp.RestClient();

                var config = FisiotesConfig.TestConfig(server, token);

                var conn = restClient.BaseAddress(config.BaseAddress)
                    .UseAuthenticationBasic(config.Credentials.Token)
                    .Resource(config.Configuraciones.ConexionLocal)
                    .SendGet<LocalConfiguracion>();

                return conn;
            }
            catch (Exception)
            {
                return GetConnexionLocal(server, token);
            }

        }
    }
}
