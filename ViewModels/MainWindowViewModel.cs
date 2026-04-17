using Core;
using DTO.Repository;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using ReactiveUI;
using Splat;
using SysNet;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public partial class MainWindowViewModel : ReactiveObject, 
                                               IScreen, 
                                               IRoutableViewModel,
                                               IActivatableViewModel
                                                
    {
        int currentVersion = 3; // Versione attuale del database, da aggiornare quando si modificano le entità
        
        public RoutingState Router { get; } = new RoutingState();
        public string UrlPathSegment => "main";
        public IScreen HostScreen => this;

        public ViewModelActivator Activator { get; } = new();

        public MainWindowViewModel()
        {
            // Spostiamo la logica di navigazione all'attivazione
            this.WhenActivated(d =>
            {
                Observable.StartAsync(async () => await InitializeNavigation())
                .Subscribe()
                .DisposeWith(d);
            });
        }

        private async Task InitializeNavigation()
        {
            await Task.Run(() => Connection.TestConnection());

            if (Flags.ServerAttivo)
            {
                SettingR r = new();
                if (!await r.CheckAppVersion(currentVersion))
                    await VerificaNecessitaAggiornamento();
                RxApp.MainThreadScheduler.Schedule(() => GoToLogin());
            }
            else
                RxApp.MainThreadScheduler.Schedule(() => GoToConnection());
        }

        // Modificato in statico o assicurati che l'istanza di AppDbContext sia configurata correttamente
        private async Task VerificaNecessitaAggiornamento()
        {
            using var _ctx = new AppDbContext();

            if (await _ctx.Database.CanConnectAsync())
            {
                var pendingMigrations = await _ctx.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    // Applica le migrazioni se necessario
                    await _ctx.Database.MigrateAsync();
                    Debug.WriteLine("✅ Database aggiornato con successo.");
                }
            }
        }

        private void GoToLogin()
        {
            HostScreen.Router.NavigateAndReset.Execute(new LoginViewModel(HostScreen));
        }

        private void GoToConnection()
        {
            HostScreen.Router.NavigateAndReset.Execute(new ConnectionViewModel(HostScreen));
        }


    }
}
