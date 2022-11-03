using CommunityToolkit.Maui.Core.Extensions;
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
      var rand = new Random();
      var teamNum = rand.Next( 1, 100000 );
      var userNum = rand.Next( 1, 100000 );

      TeamId = $"Team_{teamNum}";
      UserId = $"User_{userNum}";
   }

   [RelayCommand]
   public void Vote( VoteData vote )
   {
      var v = Votes.FirstOrDefault( x => x.UserId == vote.UserId );
      if ( v != null )
      {
         v.VoteValue = vote.VoteValue;
      }
      else
      {
         Votes.Add( vote.Clone() );
      }

      CalculateVotes();
   }

   [RelayCommand]
   public void NewVote()
   {
      Votes.Clear();
      CalculateVotes();
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
}