using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TeamVote;

public class VoteData
{
   public string UserId { get; set; }
   public int VoteValue { get; set; }
}

public partial class MainViewModel : ObservableObject
{
   [ObservableProperty]
   public bool isDebug;

   [RelayCommand]
   public void Vote( VoteData vote )
   {
   }
}