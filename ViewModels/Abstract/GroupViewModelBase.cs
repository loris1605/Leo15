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
            var canExecuteAdd = this.WhenAnyValue(
                 x => x.IsLoading,
                 loading => !loading
             ).CombineLatest(canAdd, (isNotLoading, childCanSave) => isNotLoading && childCanSave);

            var canExecuteDel = this.WhenAnyValue(
                 x => x.IsLoading,
                 loading => !loading
             ).CombineLatest(canDel, (isNotLoading, childCanSave) => isNotLoading && childCanSave);

            var canExecuteUpd = this.WhenAnyValue(
                 x => x.IsLoading,
                 loading => !loading
             ).CombineLatest(canUpd, (isNotLoading, childCanSave) => isNotLoading && childCanSave);

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
            IsLoading = true;
            try
            {
                await OnAdding();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} Caricamento annullato.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} ERRORE: {ex.Message}");
                // Qui puoi settare una InfoLabel comune se l'hai nella base
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async Task ExecuteDeleting()
        {
            IsLoading = true;
            try
            {
                await OnDeleting();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} Caricamento annullato.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} ERRORE: {ex.Message}");
                // Qui puoi settare una InfoLabel comune se l'hai nella base
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async Task ExecuteUpdating()
        {
            IsLoading = true;
            try
            {
                await OnUpdating();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} Caricamento annullato.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} ERRORE: {ex.Message}");
                // Qui puoi settare una InfoLabel comune se l'hai nella base
            }
            finally
            {
                IsLoading = false;
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
