using Avalonia.Collections;
using DTO.Entity;
using ReactiveUI;
using SysNet;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public interface IGroupViewModelBase
    {
        bool GroupFocus { get; set; }
        Task CaricaDataSource(int id = 0);
        Task CaricaByModel(object model);

    }

    public abstract partial class GroupViewModelBase<TMap> : BaseViewModel where TMap : class, new()
    {

        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdCommand { get; }
        public ReactiveCommand<Unit, Unit> DelCommand { get; }
        public ReactiveCommand<Unit, Unit> FilterCommand { get; }

        protected virtual IObservable<bool> canAdd => Observable.Return(true);
        protected virtual IObservable<bool> canDel => Observable.Return(true);
        //protected virtual IObservable<bool> canUpd => Observable.Return(true);

        protected IObservable<bool> canUpd => this.WhenAnyValue(x => x.GroupBindingT)
                                                           .Select(item => item != null);

        public GroupViewModelBase(IScreen host) : base(host)
        {
            var isNotLoading = this.WhenAnyValue(x => x.IsLoading)
                .Select(loading => !loading)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .StartWith(true) // Al caricamento il pulsante è subito attivo
                .ObserveOn(RxApp.MainThreadScheduler);

            // 2. Applichiamo la logica ai vari canExecute
            var canExecuteAdd = isNotLoading
                .CombineLatest(canAdd, (notLoading, childCanAdd) => notLoading && childCanAdd);

            var canExecuteDel = isNotLoading
                .CombineLatest(canDel, (notLoading, childCanDel) => notLoading && childCanDel);

            var canExecuteUpd = isNotLoading
                .CombineLatest(canUpd, (notLoading, childCanUpd) => notLoading && childCanUpd);

            AddCommand = ReactiveCommand.CreateFromTask(ExecuteAdding, canExecuteAdd);
            DelCommand = ReactiveCommand.CreateFromTask(ExecuteDeleting, canExecuteDel);
            UpdCommand = ReactiveCommand.CreateFromTask(ExecuteUpdating, canExecuteUpd);
            FilterCommand = ReactiveCommand.CreateFromTask(ExecuteLoading);

            this.WhenActivated(d =>
            {
                Disposable.Create(() =>
                {
                    // PULIZIA CRITICA: Sgancia la View della griglia
                    GroupedDataSource = null;
                    DataSource = null;

                }).DisposeWith(d);

                HandleCommandsDisposal(d);

            });
        }

        private void HandleCommandsDisposal(CompositeDisposable d)
        {
            AddCommand?.DisposeWith(d);
            UpdCommand?.DisposeWith(d);
            DelCommand?.DisposeWith(d);
            FilterCommand?.DisposeWith(d);
        }

        protected override void OnFinalDestruction()
        {
            // Assicuriamoci che la collezione sia nulla per il GC
            GroupedDataSource = null;
            DataSource = null;
            base.OnFinalDestruction();
        }

        public virtual Task CaricaByModel(object model) { return Task.CompletedTask; }

        public async Task ExecuteAdding()
        {
            if (_isClosing) return; // Se stiamo già uscendo, ignora il secondo Esc

            await Task.Delay(50);

            IsLoading = true;
            try
            {
                await OnAdding();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} Esc annullato.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} ERRORE ESC: {ex.Message}");
            }
            finally
            {
                // Importante: se OnEsc ha avviato la navigazione (OnBack o simile),
                // manteniamo IsLoading = true per evitare che i pulsanti "lampeggino"
                if (!_isClosing)
                {
                    IsLoading = false;
                }
            }
        }
        public async Task ExecuteDeleting()
        {
            if (_isClosing) return; // Se stiamo già uscendo, ignora il secondo Esc

            await Task.Delay(50);

            IsLoading = true;
            try
            {
                await OnDeleting();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} Esc annullato.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} ERRORE ESC: {ex.Message}");
            }
            finally
            {
                // Importante: se OnEsc ha avviato la navigazione (OnBack o simile),
                // manteniamo IsLoading = true per evitare che i pulsanti "lampeggino"
                if (!_isClosing)
                {
                    IsLoading = false;
                }
            }
        }
        public async Task ExecuteUpdating()
        {
            if (_isClosing) return; // Se stiamo già uscendo, ignora il secondo Esc

            await Task.Delay(50);

            IsLoading = true;
            try
            {
                await OnUpdating();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} Esc annullato.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} ERRORE ESC: {ex.Message}");
            }
            finally
            {
                // Importante: se OnEsc ha avviato la navigazione (OnBack o simile),
                // manteniamo IsLoading = true per evitare che i pulsanti "lampeggino"
                if (!_isClosing)
                {
                    IsLoading = false;
                }
            }
        }

        protected abstract Task OnAdding();
        protected abstract Task OnDeleting();
        protected abstract Task OnUpdating();


        #region DataSource

        private IList<TMap> _datasource = [];
        public IList<TMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        #endregion

        #region BindingT

        private TMap _mybindingt = Create<TMap>.Instance();

        public TMap BindingT
        {
            get => _mybindingt;
            set => this.RaiseAndSetIfChanged(ref _mybindingt, value);
        }

        #endregion

        #region CheckNullBindingT

        private bool _checknullbindingt = false;

        public bool CheckNullBindingT
        {
            get => _checknullbindingt;
            set => this.RaiseAndSetIfChanged(ref _checknullbindingt, value);
        }

        #endregion

        #region GroupBindingT

        private TMap _mygroupbindingt = null;

        public TMap GroupBindingT
        {
            get => _mygroupbindingt;
            set => this.RaiseAndSetIfChanged(ref _mygroupbindingt, value);
        }

        #endregion

        #region GroupFocus

        private bool _groupfocus = false;
        public bool GroupFocus
        {
            get => _groupfocus;
            set => this.RaiseAndSetIfChanged(ref _groupfocus, value);
        }

        #endregion

        #region IdValue

        private int _idvalue = 0;
        public int IdValue
        {
            get => _idvalue;
            set => this.RaiseAndSetIfChanged(ref _idvalue, value);
        }

        #endregion

        #region IdIndex

        private int _idindex = 0;
        public int IdIndex
        {
            get => _idindex;
            set => this.RaiseAndSetIfChanged(ref _idindex, value);
        }

        #endregion

        #region SelectedIndex

        private int _selectedindex = 0;
        public int SelectedIndex
        {
            get => _selectedindex;
            set => this.RaiseAndSetIfChanged(ref _selectedindex, value);
        }

        #endregion

        #region EnabledButton

        private bool _enabledbutton;
        public bool EnabledButton
        {
            get => _enabledbutton;
            set => this.RaiseAndSetIfChanged(ref _enabledbutton, value);
        }

        #endregion

        private DataGridCollectionView _groupedDataSource;
        public DataGridCollectionView GroupedDataSource
        {
            get => _groupedDataSource;
            set => this.RaiseAndSetIfChanged(ref _groupedDataSource, value);
        }

        protected override Task OnLoading() => Task.CompletedTask;

        protected override Task OnSaving()
        {
            throw new NotImplementedException();
        }
    }


}
