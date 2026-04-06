using Avalonia.Collections;
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

    public partial class GroupViewModelBase<TMap> : BaseViewModel where TMap : class, new()
    {

        protected CancellationTokenSource _cts;

        public ReactiveCommand<Unit, Unit> AddCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> UpdCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> DelCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> FilterCommand { get; protected set; }

        public GroupViewModelBase(IScreen host) : base(host)
        {
            this.WhenActivated(d =>
            {
                _cts = new CancellationTokenSource();

                Disposable.Create(() =>
                {
                    _cts.Cancel();

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
        
        
    }




    //public abstract partial class GroupViewModelold<T, W> : BaseViewModel, IGroupViewModelBase
    //                                       where T : class where W : class, IGroupQ<T>, new()
    //{
    //    private IGroupQ<T> _q;
    //    public IGroupQ<T> Q { get => _q; set => this.RaiseAndSetIfChanged(ref _q, value); }

    //    protected IGroupScreen ConfigHost => HostScreen as IGroupScreen;

    //    private CancellationTokenSource _cts;


    //    public ReactiveCommand<Unit, Unit> AddCommand { get; protected set; }
    //    public ReactiveCommand<Unit, Unit> UpdCommand { get; protected set; }
    //    public ReactiveCommand<Unit, Unit> DelCommand { get; protected set; }
    //    public ReactiveCommand<Unit, Unit> FilterCommand { get; protected set; }

    //    public GroupViewModel(IScreen host) : base(host)
    //    {
    //        Q = new W();
    //        _cts = new CancellationTokenSource();

    //        FilterCommand = ReactiveCommand.CreateFromTask(OnLoading);

    //        // Colleghiamo l'esecuzione del comando alla proprietà IsLoading
    //        FilterCommand.IsExecuting.BindTo(this, x => x.IsLoading);

    //        this.WhenActivated(d =>
    //        {
    //            if (_cts.IsCancellationRequested)
    //                _cts = new CancellationTokenSource();

    //            Disposable.Create(() => {
    //                _cts.Cancel();

    //                // PULIZIA CRITICA: Sgancia la View della griglia
    //                GroupedDataSource = null;
    //                DataSource = null;

    //            }).DisposeWith(d);

    //            HandleCommandsDisposal(d);

    //        });
    //    }

    //    private void HandleCommandsDisposal(CompositeDisposable d)
    //    {
    //        AddCommand?.DisposeWith(d);
    //        UpdCommand?.DisposeWith(d);
    //        DelCommand?.DisposeWith(d);
    //        FilterCommand?.DisposeWith(d);
    //    }

    //    //chiedere
    //    protected override void OnFinalDestruction()
    //    {
    //        // Assicuriamoci che la collezione sia nulla per il GC
    //        GroupedDataSource = null;
    //        DataSource = null;

    //        Q?.Dispose();
    //        Q = null;
    //        base.OnFinalDestruction();
    //    }

    //    // Metodi core resi più robusti
    //    protected override async Task OnLoading()
    //    {
    //        var token = _cts?.Token ?? CancellationToken.None;

    //        IsLoading = true;
    //        try
    //        {
    //            var data = await Q.Load(0, token);

    //            token.ThrowIfCancellationRequested();

    //            if (data?.Count > 0)
    //            {
    //                UpdateCollection(data, 0);
    //                GroupBindingT = DataSource[0];
    //            }
    //        }
    //        catch (OperationCanceledException) { /* Caricamento interrotto dal cambio pagina */ }
    //        catch (Exception ex) { Debug.WriteLine($"Load Error: {ex.Message}"); }
    //        finally { IsLoading = false; }
    //    }

    //    public async Task CaricaDataSource(int id = 0)
    //    {
    //        var token = _cts.Token;
    //        try
    //        {
    //            var data = await Q.Load(id, token);
    //            token.ThrowIfCancellationRequested();
    //            UpdateCollection(data, id);
    //        }
    //        catch (OperationCanceledException) { }
    //    }

    //    public async Task CaricaByModel(object model)
    //    {
    //        var token = _cts.Token;
    //        try
    //        {
    //            var data = await Q.LoadByModel(model, token);
    //            token.ThrowIfCancellationRequested();
    //            UpdateCollection(data, 0);
    //        }
    //        catch (OperationCanceledException) { }
    //    }

    

    //    protected IObservable<Unit> NavigateToReset(IRoutableViewModel vm)
    //    {
    //        if (ConfigHost == null) return Observable.Return(Unit.Default);

    //        return ConfigHost.GroupRouter
    //            .NavigateAndReset
    //            .Execute(vm)
    //            .Select(_ => Unit.Default);
    //    }

    //    protected IObservable<Unit> NavigateToInput(IRoutableViewModel vm)
    //    {
    //        if (ConfigHost == null) return Observable.Return(Unit.Default);

    //        // Esegue il cambio di stato della UI e poi naviga
    //        return Observable.Start(() => ConfigHost.GroupEnabled = false, RxApp.MainThreadScheduler)
    //            .SelectMany(_ => ConfigHost.InputRouter.Navigate.Execute(vm))
    //            .Select(_ => Unit.Default);
    //    }

    //}

    //public partial class GroupViewModelOLD<T, W>
    //{
    //    #region DataSource

    //    private IList<T> _datasource = [];
    //    public IList<T> DataSource
    //    {
    //        get => _datasource;
    //        set => this.RaiseAndSetIfChanged(ref _datasource, value);
    //    }

    //    #endregion

    //    #region BindingT

    //    private T _mybindingt = Create<T>.Instance();

    //    public T BindingT
    //    {
    //        get => _mybindingt;
    //        set => this.RaiseAndSetIfChanged(ref _mybindingt, value);
    //    }

    //    #endregion

    //    #region CheckNullBindingT

    //    private bool _checknullbindingt = false;

    //    public bool CheckNullBindingT
    //    {
    //        get => _checknullbindingt;
    //        set => this.RaiseAndSetIfChanged(ref _checknullbindingt, value);
    //    }

    //    #endregion

    //    #region GroupBindingT

    //    private T _mygroupbindingt = null;

    //    public T GroupBindingT
    //    {
    //        get => _mygroupbindingt;
    //        set => this.RaiseAndSetIfChanged(ref _mygroupbindingt, value);
    //    }

    //    #endregion

    //    #region GroupFocus

    //    private bool _groupfocus = false;
    //    public bool GroupFocus
    //    {
    //        get => _groupfocus;
    //        set => this.RaiseAndSetIfChanged(ref _groupfocus, value);
    //    }

    //    #endregion

    //    #region IdValue

    //    private int _idvalue = 0;
    //    public int IdValue
    //    {
    //        get => _idvalue;
    //        set => this.RaiseAndSetIfChanged(ref _idvalue, value);
    //    }

    //    #endregion

    //    #region IdIndex

    //    private int _idindex = 0;
    //    public int IdIndex
    //    {
    //        get => _idindex;
    //        set => this.RaiseAndSetIfChanged(ref _idindex, value);
    //    }

    //    #endregion

    //    #region SelectedIndex

    //    private int _selectedindex = 0;
    //    public int SelectedIndex
    //    {
    //        get => _selectedindex;
    //        set => this.RaiseAndSetIfChanged(ref _selectedindex, value);
    //    }

    //    #endregion

    //    #region EnabledButton

    //    private bool _enabledbutton;
    //    public bool EnabledButton
    //    {
    //        get => _enabledbutton;
    //        set => this.RaiseAndSetIfChanged(ref _enabledbutton, value);
    //    }

    //    #endregion

    //    private DataGridCollectionView _groupedDataSource;
    //    public DataGridCollectionView GroupedDataSource
    //    {
    //        get => _groupedDataSource;
    //        set => this.RaiseAndSetIfChanged(ref _groupedDataSource, value);
    //    }

    //    // Stato di caricamento reattivo
    //    private bool _isLoading;
    //    public bool IsLoading
    //    {
    //        get => _isLoading;
    //        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    //    }
    //}
}
