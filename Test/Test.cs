using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Moq;
using Qwilight;
using Qwilight.MSG;
using Qwilight.ViewModel;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.Graphics;
using Xunit;

namespace Test
{
    [CollectionDefinition("Test")]
    public sealed class TestCollection : ICollectionFixture<Test>
    {
    }

    public sealed class Test
    {
        public Test()
        {
            Bootstrap.Initialize(65540U);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            UIHandler.Instance.Init(Dispatcher.CurrentDispatcher);

            var audioSystem = new Mock<AudioSystem>();
            audioSystem.Setup(arg => arg.Init());

            var drawingSystem = new Mock<DrawingSystem>();
            drawingSystem.Setup(arg => arg.Init());

            StrongReferenceMessenger.Default.Register<GetWindowArea>(this, (recipient, message) => message.Reply(new RectInt32(0, 0, (int)Component.StandardLength, (int)Component.StandardHeight)));

            QwilightComponent.OnGetBuiltInData = data => data switch
            {
                nameof(DB) => new DB(),
                nameof(AudioSystem) => audioSystem.Object,
                nameof(DrawingSystem) => drawingSystem.Object,
                nameof(LanguageSystem) => new LanguageSystem(),
                nameof(LevelSystem) => new LevelSystem(),
                nameof(Configure) => new Configure(),
                nameof(ViewModels) => new ViewModels(),
                nameof(UI) => new UI(),
                nameof(BaseUI) => new BaseUI(),
                nameof(TwilightSystem) => new TwilightSystem(),
                nameof(ControllerSystem) => new ControllerSystem(),
                nameof(MIDISystem) => new MIDISystem(),
                "DefaultFontFamily" => new FontFamily(),
                _ => default
            };

            Directory.CreateDirectory(QwilightComponent.BundleEntryPath);
            Directory.CreateDirectory(QwilightComponent.CommentEntryPath);
            Directory.CreateDirectory(QwilightComponent.EdgeEntryPath);
            Directory.CreateDirectory(QwilightComponent.FaultEntryPath);
            Directory.CreateDirectory(QwilightComponent.MediaEntryPath);
            Directory.CreateDirectory(LevelSystem.EntryPath);

            Configure.Instance.Load();
            DB.Instance.Load();
        }
    }
}
