using Sisfarma.RestClient.Exceptions;
using Sisfarma.Sincronizador.Core.Extensions;
using Sisfarma.Sincronizador.Core.Helpers;
using Sisfarma.Sincronizador.Domain.Core.Services;
using Sisfarma.Sincronizador.Domain.Entities.Fisiotes;
using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sisfarma.Sincronizador.Domain.Core.Sincronizadores.SuperTypes
{
    public abstract class BaseSincronizador : Sincronizador, ISincronizadorAsync
    {
        protected ISisfarmaService _sisfarma;
        protected string _pathLog = ConfigurationManager.AppSettings["Directory.Logs"];

        public BaseSincronizador(ISisfarmaService sisfarma)
            => _sisfarma = sisfarma ?? throw new ArgumentNullException(nameof(sisfarma));

        public override async Task SincronizarAsync(CancellationToken cancellationToken = default(CancellationToken), int delayLoop = 200)
        {
            if (!Directory.Exists(_pathLog))            
                Directory.CreateDirectory(_pathLog);
                        
            LogTimeMessage($"Init ...");
            _cancellationToken = cancellationToken;
            
            LogTimeMessage("LoadConfiguration");
            LoadConfiguration();

            LogTimeMessage("PreSincronizacion");
            PreSincronizacion();


            while (true)
            {
                try
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    LogTimeMessage("Process Init ...");
                    Process();
                    LogTimeMessage("Process Finished.");
                }
                catch (OperationCanceledException ex)
                {                   
                    LogTimeMessage("Shutdown ...");
                    throw ex;
                }
                catch (RestClientException ex)
                {
                    var error = ex.ToLogErrorMessage();
                    LogError(error);
                    LogTimeMessage($"Process finializado con error | {error}");
                }
                catch (Exception ex)
                {
                    var error = ex.ToLogErrorMessage();
                    LogError(error);
                    LogTimeMessage($"Process finializado con error | {error}");
                }
                finally
                {
                    await Task.Delay(delayLoop);
                }
            }
        }

        public virtual void LoadConfiguration()
        {
        }

        public virtual void PreSincronizacion()
        {
        }

        private void LogError(string message)
        {
            try
            {
                var hash = Cryptographer.GenerateMd5Hash(message);

                var logsPrevios = _sisfarma.Configuraciones.GetByCampo(Configuracion.FIELD_LOG_ERRORS);
                if (logsPrevios.Contains(hash))
                    return;

                var log = $@"$log{{{hash}}}{Environment.NewLine}{DateTime.UtcNow.ToString("o")}{Environment.NewLine}{message}";
                var logs = $@"{logsPrevios}{Environment.NewLine}{log}";
                _sisfarma.Configuraciones.Update(Configuracion.FIELD_LOG_ERRORS, logs);
            }
            catch (Exception)
            {
                // nothing
                // El sincro se detiene si lanzamos una excepción en este punto.
            }
        }

        public void LogTimeMessage(string message)
        {                        
            var log = $"{ DateTime.Now.ToString("o")} | {GetType().Name} | {message}";

            using (var writer = new StreamWriter(new FileStream(Path.Combine(_pathLog, $"{GetType().Name}.logs"), FileMode.Append)))
            {                
                writer.WriteLine(log);
            }


            //using (var wirter = new StreamWriter(new FileStream(Path.Combine(_pathLog, $"SincronizadorUnycop.logs"), FileMode.Append)))
            //{
            //    wirter.WriteLine(log);
            //}
        }
    }
}