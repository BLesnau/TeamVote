namespace TeamVote;

public class AlertService
{
   public void Alert( string message )
   {
      MainThread.BeginInvokeOnMainThread( () =>
      {
         Application.Current.MainPage.DisplayAlert( "Alert", message, "OK" );
      } );
   }
}