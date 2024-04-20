using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
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
            public override BaseNoteFile.NoteVariety NoteVarietyValue => default;

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
                    StrongReferenceMessenger.Default.Send(new SetVoteWindowEdgeView
                    {
                        Www = value.Www
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

        public ObservableCollection<string> VoteNameCollection { get; } = new();

        public string VoteName
        {
            get => _voteName;

            set
            {
                if (SetProperty(ref _voteName, value, nameof(VoteName)))
                {
                    _ = Awaitable();
                    async Task Awaitable()
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
                                Genre = data.genre
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

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            ViewModels.Instance.MainValue.HandleAutoComputer();
        }

        public override void OnOpened()
        {
            base.OnOpened();
            ViewModels.Instance.MainValue.CloseAutoComputer();

            UIHandler.Instance.HandleParallel(async () =>
            {
                IsVoteGroupsLoading = true;
                var voteNames = await TwilightSystem.Instance.GetWwwParallel<string[]>($"{QwilightComponent.QwilightAPI}/vote");
                if (voteNames != null)
                {
                    Utility.SetUICollection(VoteNameCollection, voteNames);
                    VoteName ??= VoteNameCollection.FirstOrDefault();
                }
                IsVoteGroupsLoading = false;
            });
        }
    }
}