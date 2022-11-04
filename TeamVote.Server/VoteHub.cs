using Microsoft.AspNetCore.SignalR;

namespace TeamVote.Server
{
   public class VoteHub : Hub
   {
      public async Task<bool> SendVote( string userId, int voteVal )
      {
         try
         {
            Console.WriteLine( $"Vote received from '{userId}': {voteVal}" );
            await Clients.All.SendAsync( "VoteReceived", userId, voteVal );

            return true;
         }
         catch
         {
            return false;
         }
      }

      public async Task<bool> NewVote()
      {
         try
         {
            Console.WriteLine( $"New vote message received" );
            await Clients.All.SendAsync( "NewVoteReceived" );

            return true;
         }
         catch
         {
            return false;
         }
      }
   }
}