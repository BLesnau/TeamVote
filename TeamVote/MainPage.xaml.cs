namespace TeamVote;

public partial class MainPage : ContentPage
{
   public MainViewModel ViewModel { get => (MainViewModel)BindingContext; }

   public MainPage()
   {
      BindingContext = new MainViewModel();

#if DEBUG
      ViewModel.IsDebug = true;
#else
      ViewModel.IsDebug = false;
#endif

      InitializeComponent();
   }
}