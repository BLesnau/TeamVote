namespace TeamVote;

public partial class MainPage : ContentPage
{
   public MainViewModel ViewModel { get => (MainViewModel)BindingContext; }

   public MainPage()
   {
      InitializeComponent();

      BindingContext = new MainViewModel();

#if DEBUG
      ViewModel.IsDebug = true;
#else
      ViewModel.IsDebug = false;
#endif

      this.Focused += ( sender, e ) =>
      {
         ViewModel.UIFocused();
      };
   }
}