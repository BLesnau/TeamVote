using Microsoft.AspNetCore.SignalR.Client;
using static System.Net.WebRequestMethods;

namespace TeamVote;

public class ServerConnection
{
   public delegate void UserCheckInEventHandler( string userId );
   public event UserCheckInEventHandler UserCheckInReceived;

   public delegate void VoteEventHandler( string userId, int voteVal );
   public event VoteEventHandler VoteReceived;

   public delegate void NewVoteEventHandler();
   public event NewVoteEventHandler NewVoteReceived;

   private readonly HubConnection _connection;

   public ServerConnection()
   {
      var serverUrl = "https://teamvoteserver.azurewebsites.net";
      var args = Environment.GetCommandLineArgs();
      if ( args != null && args.Any( x => x.Equals( "DEV", StringComparison.InvariantCultureIgnoreCase ) ) )
      {
         serverUrl = "http://10.14.144.234:5049";
      }

      _connection = new HubConnectionBuilder()
        .WithUrl( $"{serverUrl}/vote" )
        .Build();

      _connection.On<string>( "UserCheckInReceived", OnUserCheckInReceived );
      _connection.On<string, int>( "VoteReceived", OnVoteReceived );
      _connection.On( "NewVoteReceived", OnNewVoteReceived );

      _connection.StartAsync();
   }

   public async Task JoinTeam( string teamId, string userId )
   {
      try
      {
         if ( _connection.State == HubConnectionState.Connected )
         {
            var response = await _connection.InvokeCoreAsync( "JoinTeam", typeof( bool ), new object[] { teamId, userId } );
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

   public async Task LeaveTeam( string teamId, string userId )
   {
      try
      {
         if ( _connection.State == HubConnectionState.Connected )
         {
            var response = await _connection.InvokeCoreAsync( "LeaveTeam", typeof( bool ), new object[] { teamId, userId } );
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

   public async Task CheckUserIn( string teamId, string userId )
   {
      try
      {
         if ( _connection.State == HubConnectionState.Connected )
         {
            var response = await _connection.InvokeCoreAsync( "CheckUserIn", typeof( bool ), new object[] { teamId, userId } );
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

   private void OnUserCheckInReceived( string userId )
   {
      UserCheckInReceived?.Invoke( userId );
   }

   public async Task SendVote( string teamId, string userId, int voteVal )
   {
      try
      {
         if ( _connection.State == HubConnectionState.Connected )
         {
            var response = await _connection.InvokeCoreAsync( "SendVote", typeof( bool ), new object[] { teamId, userId, voteVal } );
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

   public async Task NewVote( string teamId )
   {
      try
      {
         if ( _connection.State == HubConnectionState.Connected )
         {
            var response = await _connection.InvokeCoreAsync( "NewVote", typeof( bool ), new object[] { teamId } );
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

   private void OnNewVoteReceived()
   {
      NewVoteReceived?.Invoke();
   }
}