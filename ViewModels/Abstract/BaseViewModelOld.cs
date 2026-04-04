using Models.Repository;
using ReactiveUI;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reactive.Linq;

namespace ViewModels
{
    public abstract class BaseViewModelOld : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        public abstract string UrlPathSegment { get; }
        public IScreen HostScreen { get; }
        public ViewModelActivator Activator { get; } = new();

        private bool _isInsideStack = false;
        private bool _isFinalized = false;

        protected BaseViewModelOld(IScreen hostScreen)
        {
            HostScreen = hostScreen;
            var stack = HostScreen.Router.NavigationStack;

            // FONDAMENTALE: Usiamo WeakReference per non tenere in vita 'this' tramite il Router
            var weakThis = new WeakReference<BaseViewModelOld>(this);
            IDisposable routerSubscription = null;

            routerSubscription = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => stack.CollectionChanged += h,
                h => stack.CollectionChanged -= h)
                .Subscribe(_ =>
                {
                    // Se l'oggetto è già stato raccolto dal GC o non è più raggiungibile
                    if (!weakThis.TryGetTarget(out var self))
                    {
                        routerSubscription?.Dispose();
                        return;
                    }

                    // 1. Verifichiamo se il VM è entrato nello stack (per evitare il reset iniziale)
                    if (!self._isInsideStack && stack.Contains(self))
                    {
                        self._isInsideStack = true;
                        return;
                    }

                    // 2. Se era dentro ma ora è stato rimosso (NavigateBack o NavigateAndReset)
                    if (self._isInsideStack && !stack.Contains(self))
                    {
                        self.HandleFinalDestruction();
                        routerSubscription?.Dispose(); // Scolleghiamo l'evento definitivamente
                    }
                });
        }

        private void HandleFinalDestruction()
        {
            if (_isFinalized) return;
            _isFinalized = true;
            OnFinalDestruction();
        }

        protected virtual void OnFinalDestruction()
        {
            // Questo log conferma che la LOGICA di rimozione ha funzionato
            Debug.WriteLine($"[BASE] {this.GetType().Name} rimosso correttamente dallo stack.");
        }

        protected static void WriteFinalization(string typeName, int count)
        {
            // Usiamo variabili locali per il Task per non catturare l'istanza intera
            //_ = System.Threading.Tasks.Task.Run(async () => {
            //    try { await DisposeR.WriteDisposeAsync($"{typeName} #{count}"); }
            //    catch { /* ignore */ }
            //});
        }
    }
}

