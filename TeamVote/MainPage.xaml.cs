using CommunityToolkit.Mvvm.ComponentModel;

namespace TeamVote;

public partial class MainPage : ContentPage
{

   public MainViewModel ViewModel { get => (MainViewModel)BindingContext; }

   //int count = 0;
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

   private void OnCounterClicked( object sender, EventArgs e )
   {
      ((MainViewModel)BindingContext).IsDebug = !ViewModel.IsDebug;
   }
}