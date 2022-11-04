namespace TeamVote;

public partial class App : Application
{
   public static ServerConnection ServerConnection;

   public App( IServiceProvider provider )
   {
      InitializeComponent();

      ServerConnection = provider.GetService<ServerConnection>();

      MainPage = new AppShell();
   }
}

