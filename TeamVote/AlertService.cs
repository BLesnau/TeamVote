namespace TeamVote;

public class AlertService
{
   public void Error( string message )
   {
      MainThread.BeginInvokeOnMainThread( () =>
      {
         Application.Current.MainPage.DisplayAlert( "Error", message, "OK" );
      } );
   }
}