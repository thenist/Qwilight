using CommunityToolkit.Mvvm.Messaging;
using Qwilight.NoteFile;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Qwilight.ViewModel
{
    public sealed class VoteViewModel : BaseViewModel
    {
        public override double TargetLength => 0.9;

        VoteComputing _valueComputing;
        string _voteName;
        bool _isEdgeViewLoading;
        bool _isVoteGroupsLoading;
        bool _isVoteGroupLoading;

        public sealed class VoteComputing : Computing
        {
            public override BaseNoteFile.NoteVariety NoteVarietyValue => VoteNoteVariety;

            public BaseNoteFile.NoteVariety VoteNoteVariety { get; set; }

            public Brush PointedPaint => Paints.DefaultPointedPaint;

            public string Www { get; set; }

            public bool IsFavorite { get; set; }

            public override void OnCompiled()
            {
            }

            public override void OnFault(Exception e)
            {
            }
        }

        public ObservableCollection<VoteComputing> ComputingValues { get; } = new();

        public VoteComputing ComputingValue
        {
            get => _valueComputing;

            set
            {
                if (SetProperty(ref _valueComputing, value, nameof(ComputingValue)) && value != null)
                {
                    WeakReferenceMessenger.Default.Send<ICC>(new()
                    {
                        IDValue = ICC.ID.SetVoteWindowEdgeView,
                        Contents = value.Www
                    });
                }
            }
        }

        public bool IsVoteGroupLoading
        {
            get => _isVoteGroupLoading;

            set => SetProperty(ref _isVoteGroupLoading, value, nameof(IsVoteGroupLoading));
        }

        public bool IsEdgeViewLoading
        {
            get => _isEdgeViewLoading;

            set => SetProperty(ref _isEdgeViewLoading, value, nameof(IsEdgeViewLoading));
        }

        public override bool OpeningCondition => ViewModels.Instance.MainValue.IsNoteFileMode;

        public ObservableCollection<string> VoteNames { get; } = new();

        public string VoteName
        {
            get => _voteName;

            set
            {
                if (SetProperty(ref _voteName, value, nameof(VoteName)))
                {
                    GetVoteComputings();
                    async void GetVoteComputings()
                    {
                        IsVoteGroupLoading = true;

                        var twilightWwwVotes = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwVote[]>($"{QwilightComponent.QwilightAPI}/vote?voteName={value}");
                        if (twilightWwwVotes != null && VoteName == value)
                        {
                            Utility.SetUICollection(ComputingValues, twilightWwwVotes.Select(data => new VoteComputing
                            {
                                Www = data.www,
                                IsFavorite = data.isFavorite,
                                Title = data.title,
                                Artist = data.artist,
                                Genre = data.genre,
                                VoteNoteVariety = data.noteVariety
                            }).ToArray());
                        }

                        IsVoteGroupLoading = false;
                    }
                }
            }
        }

        public bool IsVoteGroupsLoading
        {
            get => _isVoteGroupsLoading;

            set => SetProperty(ref _isVoteGroupsLoading, value, nameof(IsVoteGroupsLoading));
        }

        public void OnEdgeViewLoading(bool isLoading)
        {
            IsEdgeViewLoading = isLoading;
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            ViewModels.Instance.MainValue.HandleAutoComputer();
        }

        public override async void OnOpened()
        {
            base.OnOpened();
            ViewModels.Instance.MainValue.CloseAutoComputer();

            IsVoteGroupsLoading = true;

            var voteNames = await TwilightSystem.Instance.GetWwwParallel<string[]>($"{QwilightComponent.QwilightAPI}/vote");
            if (voteNames != null)
            {
                HandlingUISystem.Instance.HandleParallel(() =>
                {
                    Utility.SetUICollection(VoteNames, voteNames);
                    VoteName = VoteName ?? VoteNames.FirstOrDefault();
                });
            }

            IsVoteGroupsLoading = false;
        }
    }
}