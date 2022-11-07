using Microsoft.AspNetCore.SignalR;

namespace TeamVote.Server
{
   public class VoteHub : Hub
   {
      public async Task<bool> JoinTeam( string teamId, string userId )
      {
         try
         {
            Console.WriteLine( $"'{userId}' is joining team '{teamId}'" );
            await Groups.AddToGroupAsync( Context.ConnectionId, teamId );

            return true;
         }  
         catch
         {
            return false;
         }
      }

      public async Task<bool> LeaveTeam( string teamId, string userId )
      {
         try
         {
            Console.WriteLine( $"'{userId}' is leaving team '{teamId}'" );
            await Groups.RemoveFromGroupAsync( Context.ConnectionId, teamId );

            return true;
         }
         catch
         {
            return false;
         }
      }

      public async Task<bool> CheckUserIn( string teamId, string userId )
      {
         try
         {
            Console.WriteLine( $"'{userId}' is checking in for team '{teamId}'" );
            await Clients.Group( teamId ).SendAsync( "UserCheckInReceived", userId );

            return true;
         }
         catch
         {
            return false;
         }
      }

      public async Task<bool> SendVote( string teamId, string userId, int voteVal )
      {
         try
         {
            Console.WriteLine( $"Vote received from '{userId}' on team '{teamId}': {voteVal}" );
            await Clients.Group( teamId ).SendAsync( "VoteReceived", userId, voteVal );

            return true;
         }
         catch
         {
            return false;
         }
      }

      public async Task<bool> NewVote( string teamId )
      {
         try
         {
            Console.WriteLine( $"New vote message received for team '{teamId}'" );
            await Clients.Group( teamId ).SendAsync( "NewVoteReceived" );

            return true;
         }
         catch
         {
            return false;
         }
      }

      public async Task<bool> ShowVotes( string teamId )
      {
         try
         {
            Console.WriteLine( $"Show votes message received for team '{teamId}'" );
            await Clients.Group( teamId ).SendAsync( "ShowVotesReceived" );

            return true;
         }
         catch
         {
            return false;
         }
      }
   }
}