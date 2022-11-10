using CommunityToolkit.Mvvm.ComponentModel;

namespace TeamVoteCommon
{
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
				if ( _voteValue < 0 )
				{
					VoteDisplay = "<None>";
				}
				else
				{
					VoteDisplay = HideVotes ? "?" : value.ToString();
				}
			}
			get { return _voteValue; }
		}

		private bool _hideVotes;
		public bool HideVotes
		{
			get => _hideVotes;
			set
			{
				_hideVotes = value;

				VoteValue = VoteValue;
			}
		}

		public VoteData Clone()
		{
			return (VoteData)this.MemberwiseClone();
		}
	}
}