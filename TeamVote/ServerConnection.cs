using Microsoft.AspNetCore.SignalR.Client;

namespace TeamVote;

public class ServerConnection
{
   public delegate void VoteEventHandler( string userId, int voteVal );
   public event VoteEventHandler VoteReceived;

   private readonly HubConnection _connection;

   public ServerConnection()
   {
      _connection = new HubConnectionBuilder()
        .WithUrl( "http://10.14.144.234:5049/vote" )
        .Build();

      _connection.On<string, int>( "VoteReceived", OnVoteReceived );

      _connection.StartAsync();
   }

   public async Task SendVote( string userId, int voteVal )
   {
      try
      {
         if ( _connection.State == HubConnectionState.Connected )
         {
            var response = await _connection.InvokeCoreAsync( "SendVote", typeof( bool ), new object[] { userId, voteVal } );
         }
         else
         {
            App.AlertService.Error( "Connection to server could not be established. Close and try again in a few minutes." );
         }
      }
      catch ( Exception ex )
      {
         App.AlertService.Error( $"Unexpected error occured: {ex.Message}" );
      }
   }

   private void OnVoteReceived( string userId, int voteVal )
   {
      VoteReceived?.Invoke( userId, voteVal );
   }
}