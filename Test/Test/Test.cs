using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Moq;
using Qwilight;
using Qwilight.ViewModel;
using System.Text;
using Windows.UI;
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

            HandlingUISystem.Instance.Init(e => { });

            var audioSystem = new Mock<AudioSystem>();
            audioSystem.Setup(arg => arg.Init());

            var drawingSystem = new Mock<DrawingSystem>();
            drawingSystem.Setup(arg => arg.Init());
            drawingSystem.Setup(arg => arg.GetFont()).Returns(new CanvasTextFormat());
            drawingSystem.Setup(arg => arg.SetFaintPaints(It.IsAny<IDrawingContainer>(), It.IsAny<ICanvasBrush[]>(), It.IsAny<Color>()));
            drawingSystem.Setup(arg => arg.SetFontFamily(It.IsAny<CanvasTextFormat>()));
            drawingSystem.Setup(arg => arg.SetFontLevel(It.IsAny<CanvasTextFormat>(), It.IsAny<float>()));
            drawingSystem.Setup(arg => arg.SetFontSystem(It.IsAny<CanvasTextFormat>(), It.IsAny<int>(), It.IsAny<int>()));

            QwilightComponent.OnGetBuiltInData = data => data switch
            {
                nameof(DB) => new DB(),
                nameof(AudioSystem) => audioSystem.Object,
                nameof(DrawingSystem) => drawingSystem.Object,
                nameof(LanguageSystem) => new LanguageSystem(),
                nameof(Configure) => new Configure(),
                nameof(ViewModels) => new ViewModels(),
                nameof(UI) => new UI(),
                nameof(BaseUI) => new BaseUI(),
                nameof(TwilightSystem) => new TwilightSystem(),
                nameof(ControllerSystem) => new ControllerSystem(),
                nameof(MIDISystem) => new MIDISystem(),
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
