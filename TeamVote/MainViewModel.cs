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
}

public partial class MainViewModel : ObservableObject
{
   [ObservableProperty]
   public ObservableCollection<VoteData> _votes = new ObservableCollection<VoteData>();

   [ObservableProperty]
   public bool _isDebug;

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
         Votes.Add( vote );
      }
   }

   [RelayCommand]
   public void NewVote()
   {
      Votes.Clear();
   }
}