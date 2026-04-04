using Avalonia.Collections;
using Models.Interfaces;
using ReactiveUI;
using SysNet;
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
   

    public abstract partial class GroupViewModel<T, W> : BaseViewModel, IGroupViewModelBase
                                           where T : class where W : class, IGroupQ<T>, new()
    {
        private IGroupQ<T> _q;
        public IGroupQ<T> Q { get => _q; set => this.RaiseAndSetIfChanged(ref _q, value); }

        protected IGroupScreen ConfigHost => HostScreen as IGroupScreen;


        public ReactiveCommand<Unit, Unit> AddCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> UpdCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> DelCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> FilterCommand { get; protected set; }

        public GroupViewModel(IScreen host) : base(host)
        {
            Q = new W();
            FilterCommand = ReactiveCommand.CreateFromTask(OnLoading);

            // Colleghiamo l'esecuzione del comando alla proprietà IsLoading
            FilterCommand.IsExecuting.BindTo(this, x => x.IsLoading);

            this.WhenActivated(d =>
            {
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
            Q?.Dispose();
            Q = null;
            base.OnFinalDestruction();
        }

        // Metodi core resi più robusti
        protected override async Task OnLoading()
        {
            IsLoading = true;
            try
            {
                await CaricaDataSource();
                if (DataSource?.Count > 0)
                {
                    GroupBindingT = DataSource[0];
                }
            }
            catch (Exception ex)
            {
                // Usa un servizio di logging reale se disponibile
                System.Diagnostics.Debug.WriteLine($"[GroupViewModel] Load Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task CaricaDataSource(int id = 0)
        {
            var data = await Q.Load(id);
            UpdateCollection(data, id);
        }

        public async Task CaricaByModel(object model)
        {
            var data = await Q.LoadByModel(model);
            UpdateCollection(data, 0);
        }

        private void UpdateCollection(IList<T> data, int id)
        {
            DataSource = data;

            // Creazione View per la Grid: 
            // TIP: Se usi Avalonia/WPF, considera di esporre questa property come Observable
            var view = new DataGridCollectionView(DataSource);
            view.GroupDescriptions.Add(new DataGridPathGroupDescription("Titolo"));

            GroupedDataSource = view;
            GroupFocus = true;
            IdIndex = id;
        }

        protected IObservable<Unit> NavigateToReset(IRoutableViewModel vm)
        {
            if (ConfigHost == null) return Observable.Return(Unit.Default);

            return ConfigHost.GroupRouter
                .NavigateAndReset
                .Execute(vm)
                .Select(_ => Unit.Default);
        }

        protected IObservable<Unit> NavigateToInput(IRoutableViewModel vm)
        {
            if (ConfigHost == null) return Observable.Return(Unit.Default);

            // Esegue il cambio di stato della UI e poi naviga
            return Observable.Start(() => ConfigHost.GroupEnabled = false, RxApp.MainThreadScheduler)
                .SelectMany(_ => ConfigHost.InputRouter.Navigate.Execute(vm))
                .Select(_ => Unit.Default);
        }

    }

    public partial class GroupViewModel<T, W>
    {
        #region DataSource

        private IList<T> _datasource = [];
        public IList<T> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        #endregion

        #region BindingT

        private T _mybindingt = Create<T>.Instance();

        public T BindingT
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

        private T _mygroupbindingt = null;

        public T GroupBindingT
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

        // Stato di caricamento reattivo
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }
    }
}
