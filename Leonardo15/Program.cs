using Avalonia;
using DTO.Repository;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using ViewModels; // Sostituisci con i tuoi namespace reali
using Views;

namespace Leonardo15;

internal class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
    {
        var services = new ServiceCollection();

        // 1. Registra le tue cose (Repository, VM, View)
        RegisterServices(services);
        RegisterViews(services);
        RegisterViewModels(services);
        RegisterIViewFor(services);
        
        // 2. PREPARA il resolver di Splat PRIMA di buildare
        var resolver = new MicrosoftDependencyResolver(services);

        // 3. Collega Splat al resolver (senza inizializzazioni extra che scrivono nel provider)
        Locator.SetLocator(resolver);

        var appBuilder = AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithInterFont()
        .LogToTrace()
        .UseReactiveUI();

        // 4. Ora puoi buildare il provider di Microsoft
        var container = services.BuildServiceProvider();

        // 5. Opzionale: passa il container finale al resolver se necessario
        //resolver.UpdateContainer(container);

        return appBuilder;
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Repository
        services.AddTransient<ILoginRepository, LoginRepository>();
        services.AddTransient<IMenuRepository, MenuRepository>();
        services.AddTransient<IPersonRepository, PersonRepository>();
        services.AddTransient<IOperatoreRepository, OperatoreRepository>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        // ViewModels
        services.AddTransient<MainWindowViewModel>();

               
    }

    private static void RegisterIViewFor(IServiceCollection services)
    {
        // Views (Necessarie per il Routing di ReactiveUI)

        services.AddTransient<IViewFor<LoginViewModel>, LoginView>();
        services.AddTransient<IViewFor<ConnectionViewModel>, ConnectionView>();
        services.AddTransient<IViewFor<MenuViewModel>, MenuView>();
        //services.AddTransient<IViewFor<TestViewModel>, TestView>();
        
        RegisterIViewForSoci(services);
        RegisterIViewForConfigurazione(services);

    }

    private static void RegisterViews(IServiceCollection services)
    {
        services.AddTransient<MainWindow>();
    }

    private static void RegisterIViewForSoci(IServiceCollection services)
    {
        services.AddTransient<IViewFor<SociViewModel>, SociView>();

        services.AddTransient<IViewFor<PersonGroupViewModel>, PersonGroupView>();

        services.AddTransient<IViewFor<PersonAddViewModel>, PersonInputView>();
        services.AddTransient<IViewFor<PersonUpdViewModel>, PersonInputView>();
        services.AddTransient<IViewFor<PersonDelViewModel>, PersonInputView>();
        services.AddTransient<IViewFor<PersonSearchViewModel>, PersonSearchView>();

        services.AddTransient<IViewFor<CodiceSocioAddViewModel>, SocioInputView>();
        services.AddTransient<IViewFor<CodiceSocioDelViewModel>, SocioInputView>();
        services.AddTransient<IViewFor<CodiceSocioUpdViewModel>, SocioInputView>();

        services.AddTransient<IViewFor<TesseraAddViewModel>, TesseraInputView>();
        services.AddTransient<IViewFor<TesseraDelViewModel>, TesseraInputView>();
        services.AddTransient<IViewFor<TesseraUpdViewModel>, TesseraInputView>();
    }

    private static void RegisterIViewForConfigurazione(IServiceCollection services)
    {
        services.AddTransient<IViewFor<ConfigurazioneViewModel>, ConfigurazioneView>();

        services.AddTransient<IViewFor<OperatoreGroupViewModel>, OperatoreGroupView>();

        services.AddTransient<IViewFor<OperatoreAddViewModel>, OperatoreInputView>();
        services.AddTransient<IViewFor<OperatoreUpdViewModel>, OperatoreInputView>();
        services.AddTransient<IViewFor<OperatoreDelViewModel>, OperatoreInputView>();
    }


    private static void RegisterMappers()
    {

    }


}
