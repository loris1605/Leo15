using Microsoft.EntityFrameworkCore;
using Models.Context;
using ReactiveUI;
using Splat;
using SysNet;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public partial class ConnectionViewModel : BaseViewModel
    {
        #region Commands

        public ReactiveCommand<Unit, Unit> CheckCommand { get; }
        public ReactiveCommand<Unit, Unit> AvviaCommand { get; }
        
        #endregion

        public ConnectionViewModel(IScreen host) : base(host)
        {
            CheckCommand = ReactiveCommand.CreateFromTask(OnCheckConnectionAsync);
            AvviaCommand = ReactiveCommand.Create(GoToLogin);


            this.WhenActivated(d =>
            {
            this.WhenAnyValue(
            x => x.DatabaseText,
            x => x.PasswordText,
            x => x.UserIdText,
            x => x.SelectedInstance,
            (db, pass, user, server) =>
                !string.IsNullOrWhiteSpace(db) &&
                !string.IsNullOrWhiteSpace(pass) &&
                !string.IsNullOrWhiteSpace(user) &&
                !string.IsNullOrWhiteSpace(server))
            .Subscribe(value => EnabledCheck = value)
            .DisposeWith(d);
            
        });

        }

        protected override void OnFinalDestruction()
        {
            base.OnFinalDestruction();
        }

        private async Task OnCheckConnectionAsync()
        {
            try
            {
                var connectionString =
                    $"Server={SelectedInstance?.Trim()};" +
                    $"Database={DatabaseText?.Trim()};" +
                    $"User Id={UserIdText?.Trim()};" +
                    $"Password={PasswordText};" + // La password non va trimmata!
                    "TrustServerCertificate=true;Connect Timeout=5;";

                Connection.SetConnectionString(connectionString);
                
                using (var db = new AppDbContext())
                {
                    Debug.WriteLine($"Inizio Creazione Db");
                    // Applica le migrazioni e crea il DB (LocalDB) se non esiste
                    await db.Database.MigrateAsync();

                    // ATTENZIONE: Cancella fisicamente il DB se esiste
                    //db.Database.EnsureDeleted();

                    // Crea il DB da zero basandosi sul ModelBuilder attuale
                    //db.Database.EnsureCreated();
                    Debug.WriteLine("Database allineato correttamente.");
                }
                
                AvviaVisibile = true;
                await OnUserIdFocus();
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore: {ex.Message}");
                // Qui dovresti gestire il fallimento (es. password errata o server offline)
            }
        }

        private async Task OnUserIdFocus()
        {
            // Fondamentale: aspetta un attimo che la View sia "viva" e l'handler registrato
            await Task.Delay(200);

            try
            {
                await UserIdFocus.Handle(Unit.Default);
            }
            catch (Exception ex)
            {
                // Evita crash se l'handler non è ancora pronto o la vista è già chiusa
                System.Diagnostics.Debug.WriteLine("Interaction Focus fallita: " + ex.Message);
            }
        }


        private void GoToLogin()
        {
            HostScreen.Router.NavigateAndReset.Execute(new LoginViewModel(HostScreen));
            
        }

        //Carica la ComboBox con le IstanzeSql
        protected override async Task OnLoading()
        {
            IsLoading = true;
            try
            {
                // Esegue la ricerca pesante in background
                var instances = await Task.Run(() => SqlInstanceFinder.GetInstances())
                                ?? new List<string>();

                // Usiamo lo scheduler della UI per aggiornare la lista ed evitare crash
                RxApp.MainThreadScheduler.Schedule(() =>
                {
                    SqlInstances.Clear();
                    foreach (var i in instances)
                    {
                        SqlInstances.Add(i);
                    }

                    if (SqlInstances.Any())
                    {
                        SelectedInstance = SqlInstances[0];
                    }
                });
            }
            finally
            {
                // Garantisce che IsLoading torni false anche in caso di errore
                IsLoading = false;

                // Sposta il focus dopo che la UI ha finito di renderizzare
                await UserIdFocus.Handle(Unit.Default);
            }


        }

        protected override Task OnSaving()
        {
            throw new NotImplementedException();
        }
    }

    public partial class ConnectionViewModel
    {
        #region ListOfServers
        public ObservableCollection<string> SqlInstances { get; } = [];

        #endregion

        #region SelectedSqlInstance

        private string _selectedInstance;
        public string SelectedInstance
        {
            get => _selectedInstance;
            set => this.RaiseAndSetIfChanged(ref _selectedInstance, value);
        }

        #endregion

        
        //User Id
        private string useridtext = string.Empty;
        public string UserIdText
        {
            get => useridtext;
            set => this.RaiseAndSetIfChanged(ref useridtext, value);
        }

        //Password
        private string passwordtext = string.Empty;
        public string PasswordText
        {
            get => passwordtext;
            set => this.RaiseAndSetIfChanged(ref passwordtext, value);
        }

        //Database
        private string databasetext = string.Empty;
        public string DatabaseText
        {
            get => databasetext;
            set => this.RaiseAndSetIfChanged(ref databasetext, value);
        }

        //AvviaVisibile
        private bool avviavisibile = false;
        public bool AvviaVisibile
        {
            get => avviavisibile;
            set => this.RaiseAndSetIfChanged(ref avviavisibile, value);
        }
        

        private bool _enabledcheck;
        public bool EnabledCheck
        {
            get => _enabledcheck;
            set => this.RaiseAndSetIfChanged(ref _enabledcheck, value);
        }

        #region Observable

        
        public Interaction<Unit, Unit> UserIdFocus { get; } = new();

        #endregion
    }
}
