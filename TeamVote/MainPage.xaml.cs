namespace TeamVote;

public partial class MainPage : ContentPage
{
   public MainViewModel ViewModel { get => (MainViewModel)BindingContext; }

   public MainPage()
   {
      InitializeComponent();

      BindingContext = new MainViewModel();

#if DEBUG
      //ViewModel.IsDebug = true; // Uncomment to show debug UI
      ViewModel.IsDebug = false; // Uncomment to always hide debug UI
#else
      ViewModel.IsDebug = false;
#endif

//#if MACCATALYST
//      this.Appearing += UIFocused;
//#else
//      this.Focused += UIFocused;
//#endif

#if WINDOWS
      this.Focused += UIFocused;
#else
      this.Appearing += UIFocused;
#endif
   }

   private void UIFocused( object sender, EventArgs e )
   {
      ViewModel.UIFocused();

#if WINDOWS
      this.Focused -= UIFocused;
#else
      this.Appearing -= UIFocused;
#endif
   }
}