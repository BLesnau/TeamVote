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

#if MACCATALYST
      this.Appearing += UIFocused;
#else
      this.Focused += UIFocused;
#endif
   }

   private void UIFocused( object sender, EventArgs e )
   {
      ViewModel.UIFocused();

#if MACCATALYST
      this.Appearing -= UIFocused;
#else
      this.Focused -= UIFocused;
#endif
   }
}