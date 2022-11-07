using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace TeamVote;

public partial class VoteData : ObservableObject
{
   [ObservableProperty]
   public string _userId;

   [ObservableProperty]
   public int _voteValue;

   public VoteData Clone()
   {
      return (VoteData)this.MemberwiseClone();
   }
}

public partial class MainViewModel : ObservableObject
{
   [ObservableProperty]
   public string _userId;

   [ObservableProperty]
   public string _teamId;

   [ObservableProperty]
   public int _teamMemberCount = 0;

   [ObservableProperty]
   public int _voteSum = 0;

   [ObservableProperty]
   public double _voteAverage = 0;

   [ObservableProperty]
   public double _voteMedian = 0;

   [ObservableProperty]
   public int _voteMode = 0;

   [ObservableProperty]
   public ObservableCollection<VoteData> _votes = new ObservableCollection<VoteData>();

   [ObservableProperty]
   public bool _isDebug;

   public MainViewModel()
   {
      App.ServerConnection.VoteReceived += VoteReceived;
      App.ServerConnection.NewVoteReceived += NewVoteReceived;
   }

   private bool _joinedTeam = false;
   private string _joinedTeamName = string.Empty;

   [RelayCommand]
   public void DebugVote( VoteData vote )
   {
      VoteReceived( vote.UserId, vote.VoteValue );
   }

   [RelayCommand]
   public async void Vote( int voteVal )
   {
      if ( !(await CheckInput()) )
      {
         return;
      }

      await App.ServerConnection.SendVote( TeamId, UserId, voteVal );
   }

   [RelayCommand]
   public async void NewVote()
   {
      if ( !(await CheckInput()) )
      {
         return;
      }

      await App.ServerConnection.NewVote( TeamId );
   }

   public void UIFocused()
   {
      var rand = new Random();
      var teamNum = rand.Next( 1, 100000 );
      var userNum = rand.Next( 1, 100000 );

      TeamId = Preferences.Default.Get( "TeamId", string.Empty );
      if ( string.IsNullOrWhiteSpace( TeamId ) )
      {
         TeamId = $"Team_{teamNum}";
      }

      UserId = Preferences.Default.Get( "UserId", string.Empty );
      if ( string.IsNullOrWhiteSpace( UserId ) )
      {
         UserId = $"User_{userNum}";
      }
   }

   async partial void OnTeamIdChanged( string value )
   {
      Preferences.Default.Set( "TeamId", TeamId );

      if ( _joinedTeam )
      {
         await App.ServerConnection.LeaveTeam( _joinedTeamName, UserId );
         _joinedTeam = false;
         _joinedTeamName = string.Empty;
         NewVoteReceived();
      }

      await JoinTeam();
   }

   partial void OnUserIdChanged( string value )
   {
      Preferences.Default.Set( "UserId", UserId );
   }

   private async void VoteReceived( string userId, int voteVal )
   {
      var v = Votes.FirstOrDefault( x => x.UserId == userId );
      if ( v != null )
      {
         v.VoteValue = voteVal;
      }
      else
      {
         await MainThread.InvokeOnMainThreadAsync( () =>
         {
            Votes.Add( new VoteData() { UserId = userId, VoteValue = voteVal } );
         } );
      }

      CalculateVotes();
   }

   private async void NewVoteReceived()
   {
      await MainThread.InvokeOnMainThreadAsync( () =>
      {
         Votes.Clear();
         CalculateVotes();
      } );
   }

   private void CalculateVotes()
   {
      TeamMemberCount = Votes.Count();

      if ( TeamMemberCount > 0 )
      {
         VoteSum = Votes.Sum( x => x.VoteValue );
         VoteAverage = Math.Round( Votes.Average( x => x.VoteValue ), 2 );
         VoteMedian = Math.Round( Votes.Median( x => x.VoteValue ), 2 );
         VoteMode = Votes.ModeWithMaxTiebreaker( x => x.VoteValue );
      }
      else
      {
         VoteSum = 0;
         VoteAverage = 0;
         VoteMedian = 0;
         VoteMode = 0;
      }
   }

   private async Task<bool> CheckInput()
   {
      if ( string.IsNullOrWhiteSpace( TeamId ) )
      {
         App.AlertService.Error( "A Team ID must be provided." );
         return false;
      }

      if ( string.IsNullOrWhiteSpace( UserId ) )
      {
         App.AlertService.Error( "A User ID must be provided." );
         return false;
      }

      await JoinTeam();

      return true;
   }

   private async Task JoinTeam()
   {
      if ( !_joinedTeam )
      {
         await App.ServerConnection.JoinTeam( TeamId, UserId );
         _joinedTeam = true;
         _joinedTeamName = TeamId;
      }
   }
}