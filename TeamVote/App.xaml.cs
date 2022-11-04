namespace TeamVote;

public partial class App : Application
{
   public static ServerConnection ServerConnection;
   public static AlertService AlertService;

   public App( IServiceProvider provider )
   {
      InitializeComponent();

      ServerConnection = provider.GetService<ServerConnection>();
      AlertService = provider.GetService<AlertService>();

      MainPage = new AppShell();
   }
}