using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace TeamVote;

public partial class VoteData : ObservableObject
{
   [ObservableProperty]
   public string _userId;

   [ObservableProperty]
   public string _voteDisplay;

   private int _voteValue;
   public int VoteValue
   {
      set
      {
         _voteValue = value;
         VoteDisplay = _voteValue < 0 ? "?" : value.ToString();
      }
      get { return _voteValue; }
   }

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
      App.ServerConnection.UserCheckInReceived += UserCheckInReceived;
      App.ServerConnection.VoteReceived += VoteReceived;
      App.ServerConnection.NewVoteReceived += NewVoteReceived;
   }

   private bool _isInitialized = false;
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

   public async void UIFocused()
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

      await LeaveTeam();
      await JoinTeam();

      _isInitialized = true;
   }

   async partial void OnTeamIdChanged( string value )
   {
      Preferences.Default.Set( "TeamId", TeamId );

      if ( _isInitialized )
      {
         await LeaveTeam();
         await JoinTeam();
      }
   }

   async partial void OnUserIdChanged( string value )
   {
      Preferences.Default.Set( "UserId", UserId );

      if ( _isInitialized )
      {
         await LeaveTeam();
         await JoinTeam();
      }
   }

   private async void UserCheckInReceived( string userId )
   {
      var v = Votes.FirstOrDefault( x => x.UserId == userId );
      if ( v == null )
      {
         await MainThread.InvokeOnMainThreadAsync( () =>
         {
            Votes.Add( new VoteData() { UserId = userId, VoteValue = -1 } );
         } );
      }

      CalculateVotes();
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

      await CheckIn();
   }

   private void CalculateVotes()
   {
      var votesToUse = Votes.Where( x => x.VoteValue >= 0 );
      TeamMemberCount = votesToUse.Count();

      if ( TeamMemberCount > 0 )
      {
         VoteSum = votesToUse.Sum( x => x.VoteValue );
         VoteAverage = Math.Round( votesToUse.Average( x => x.VoteValue ), 2 );
         VoteMedian = Math.Round( votesToUse.Median( x => x.VoteValue ), 2 );
         VoteMode = votesToUse.ModeWithMaxTiebreaker( x => x.VoteValue );
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

         await CheckIn();
      }
   }

   private async Task LeaveTeam()
   {
      if ( _joinedTeam )
      {
         await App.ServerConnection.LeaveTeam( _joinedTeamName, UserId );
         _joinedTeam = false;
         _joinedTeamName = string.Empty;
         NewVoteReceived();
      }
   }

   private async Task CheckIn()
   {
      await App.ServerConnection.CheckUserIn( TeamId, UserId );
   }
}