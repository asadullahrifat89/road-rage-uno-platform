﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Frame = Microsoft.UI.Xaml.Controls.Frame;
using Microsoft.UI.Xaml.Media;
using Windows.UI.ViewManagement;
using System.ServiceProcess;
using AstroOdyssey;
#if DEBUG
using Microsoft.Extensions.Logging;
#endif

namespace SkyWay
{
    public sealed partial class App : Application
    {
        #region Fields

        private static Window _window;
        private readonly SystemNavigationManager _systemNavigationManager;
        private readonly List<Type> _goBackNotAllowedToPages;
        private readonly List<(Type IfGoingBackTo, Type RouteTo)> _goBackPageRoutes;
        private static string _baseUrl;

        #endregion

        #region Ctor

        public App()
        {
            InitializeLogging();

            InitializeComponent();
            Container = ConfigureDependencyInjection();

            Uno.UI.ApplicationHelper.RequestedCustomTheme = "Dark";

#if HAS_UNO || NETFX_CORE
            Suspending += OnSuspending;
#endif
            UnhandledException += App_UnhandledException;

            Uno.UI.FeatureConfiguration.Page.IsPoolingEnabled = true;

            _systemNavigationManager = SystemNavigationManager.GetForCurrentView();

            _goBackNotAllowedToPages = new List<Type>() { typeof(GamePlayPage) };
            //_goBackPageRoutes = new List<(Type IfGoingBackTo, Type RouteTo)>() { (IfGoingBackTo: typeof(GameOverPage), RouteTo: typeof(GamePlayPage)) };

            //CurrentCulture = "en";
        }

        #endregion

        #region Properties

        public static IServiceProvider Container { get; set; }

        //public static PlayerCredentials AuthCredentials { get; set; }

        //public static GameProfile GameProfile { get; set; }

        //public static AuthToken AuthToken { get; set; }

        //public static PlayerScore PlayerScore { get; set; }

        //public static bool GameScoreSubmissionPending { get; set; }

        //public static PlayerShip Ship { get; set; }

        //public static Session Session { get; set; }

        //public static string CurrentCulture { get; set; }

        //public static bool HasUserLoggedIn => GameProfile is not null && GameProfile.User is not null && !GameProfile.User.UserId.IsNullOrBlank() && !GameProfile.User.UserName.IsNullOrBlank();

        #endregion

        #region Events

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
#if DEBUG
            Console.WriteLine(e.Message);
#endif
            e.Handled = true;
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
            _window = new Window();
            _window.Activate();
#else
            _window = Window.Current;
#endif
            var rootFrame = _window.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.Background = App.Current.Resources["FrameBackgroundColor"] as SolidColorBrush; // App.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush;

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.IsNavigationStackEnabled = true;

                if (args.UWPLaunchActivatedEventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: App: Load state from previously suspended application
                }

                // Place the frame in the current Window
                _window.Content = rootFrame;
            }

#if !(NET6_0_OR_GREATER && WINDOWS)
            if (args.UWPLaunchActivatedEventArgs.PrelaunchActivated == false)
#endif
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(GamePlayPage), args.Arguments);
                }

                // Ensure the current window is active
                _window.Activate();
            }

            _systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            _systemNavigationManager.BackRequested += OnBackRequested;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var rootFrame = _window.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                var backPage = rootFrame.BackStack.LastOrDefault();

                if (_goBackNotAllowedToPages.Contains(backPage.SourcePageType))
                    return;

                if (_goBackPageRoutes.Any(x => x.IfGoingBackTo == backPage.SourcePageType))
                {
                    var reroute = _goBackPageRoutes.FirstOrDefault(x => x.IfGoingBackTo == backPage.SourcePageType).RouteTo;

                    rootFrame.Navigate(reroute);
                    return;
                }

                rootFrame.GoBack();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: App: Save application state and stop any background activity
            deferral.Complete();
        }

        #endregion

        #region Methods

        #region Public

        public static void EnterFullScreen(bool value)
        {
            var view = ApplicationView.GetForCurrentView();

            if (view is not null)
            {
                if (value)
                {
                    view.TryEnterFullScreenMode();
                }
                else
                {
                    view.ExitFullScreenMode();
                }
            }
        }

        public static void NavigateToPage(Type pageType, object parameter = null)
        {
            var rootFrame = _window.Content as Frame;
            rootFrame.Navigate(pageType, parameter);
        }

        public static string GetBaseUrl()
        {
            if (_baseUrl.IsNullOrBlank())
            {
                var indexUrl = Uno.Foundation.WebAssemblyRuntime.InvokeJS("window.location.href;");
                var appPackageId = Environment.GetEnvironmentVariable("UNO_BOOTSTRAP_APP_BASE");
                _baseUrl = $"{indexUrl}{appPackageId}";

#if DEBUG
                Console.WriteLine(_baseUrl);
#endif 
            }
            return _baseUrl;
        }

        #endregion

        #region Private

        private static void InitializeLogging()
        {
#if DEBUG
            // Logging is disabled by default for release builds, as it incurs a significant
            // initialization cost from Microsoft.Extensions.Logging setup. If startup performance
            // is a concern for your application, keep this disabled. If you're running on web or 
            // desktop targets, you can use url or command line parameters to enable it.
            //
            // For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

            var factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                builder.AddDebug();
#else
                builder.AddConsole();
#endif
                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Information);

                // Default filters for Uno Platform namespaces
                builder.AddFilter("Uno", LogLevel.Warning);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);


                // Generic Xaml events
                //builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug);
                //builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug);
                //builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug);
                //builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug);
                //builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace);

                // Layouter specific messages
                // builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug );
                // builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug );
                // builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug );

                builder.AddFilter("Windows.Storage", LogLevel.Debug);

                // Binding related messages
                //builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug);
                //builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug);

                // Binder memory references tracking
                builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug);

                // RemoteControl and HotReload related
                builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug);
            });

            Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
            Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
        }


        private IServiceProvider ConfigureDependencyInjection()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddHttpService(lifeTime: 300, retryCount: 3, retryWait: 1);
            serviceCollection.AddSingleton<IHttpRequestService, HttpRequestService>();

            return serviceCollection.BuildServiceProvider();
        }

        #endregion

        #endregion
    }
}
