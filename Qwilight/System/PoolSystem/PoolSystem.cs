using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.IO;
using Qwilight.Utilities;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Windows;
using Windows.Graphics.DirectX;
using Windows.UI;
using Brush = System.Windows.Media.Brush;
using FormattedText = System.Windows.Media.FormattedText;

namespace Qwilight
{
    public sealed class PoolSystem
    {
        public static readonly PoolSystem Instance = new();

        readonly RecyclableMemoryStreamManager _rmsm = new(new()
        {
            BlockSize = 1024,
            LargeBufferMultiple = 1024 * 1024,
            MaximumBufferSize = 16 * 1024 * 1024,
            GenerateCallStacks = !QwilightComponent.IsVS,
            AggressiveBufferReturn = true,
            MaximumLargePoolFreeBytes = 16 * 1024 * 1024 * 4,
            MaximumSmallPoolFreeBytes = 100 * 1024
        });
        readonly ConcurrentDictionary<TextID, CanvasTextLayout> _textItems = new();
        readonly ConcurrentDictionary<DefaultTextID, FormattedText> _defaultTextItems = new();
        readonly ConcurrentDictionary<TargetID, CanvasRenderTarget> _targetItems = new();
        readonly ConcurrentDictionary<Color, ICanvasBrush[]> _faintPaints = new();
        readonly ConcurrentDictionary<ValueTextID<int>, string> _valueIntTexts = new();
        readonly ConcurrentDictionary<ValueTextID<double>, string> _valueFloat64Texts = new();
        readonly ConcurrentDictionary<FormattedTextID, string> _formattedTexts = new();
        readonly ConcurrentDictionary<long, string> _formattedUnitTexts = new();
        readonly ConcurrentQueue<IDisposable> _pendingClosables = new();

        public void Wipe(bool isWPFViewVisible)
        {
            if (isWPFViewVisible)
            {
                foreach (var textID in _textItems.Keys)
                {
                    if (_textItems.TryRemove(textID, out var textItem))
                    {
                        _pendingClosables.Enqueue(textItem);
                    }
                }
                foreach (var targetID in _targetItems.Keys)
                {
                    if (_targetItems.TryRemove(targetID, out var targetItem))
                    {
                        _pendingClosables.Enqueue(targetItem);
                    }
                }
                foreach (var faintColor in _faintPaints.Keys)
                {
                    if (_faintPaints.TryRemove(faintColor, out var faintPaints))
                    {
                        foreach (var faintPaint in faintPaints)
                        {
                            _pendingClosables.Enqueue(faintPaint);
                        }
                    }
                }
            }
            else
            {
                foreach (var (defaultTextID, defaultTextItem) in _defaultTextItems)
                {
                    _defaultTextItems.TryRemove(defaultTextID, out _);
                }
            }
            foreach (var (valueTextID, text) in _valueIntTexts)
            {
                _valueIntTexts.TryRemove(valueTextID, out _);
            }
            foreach (var (valueTextID, text) in _valueFloat64Texts)
            {
                _valueFloat64Texts.TryRemove(valueTextID, out _);
            }
            foreach (var (value, text) in _formattedTexts)
            {
                _formattedTexts.TryRemove(value, out _);
            }
            foreach (var (value, text) in _formattedUnitTexts)
            {
                _formattedUnitTexts.TryRemove(value, out _);
            }
        }

        public void ClosePendingClosables()
        {
            while (_pendingClosables.TryDequeue(out var pendingAvatarEdge))
            {
                pendingAvatarEdge.Dispose();
            }
        }

        public MemoryStream GetDataFlow()
        {
            return _rmsm.GetStream();
        }

        public MemoryStream GetDataFlow(int dataLength)
        {
            return _rmsm.GetStream(null, dataLength);
        }

        public MemoryStream GetDataFlow(byte[] data)
        {
            return _rmsm.GetStream(data);
        }

        public CanvasRenderTarget GetTargetItem(float targetLength, float targetHeight)
        {
            var targetID = new TargetID
            {
                targetLength = targetLength,
                targetHeight = targetHeight
            };
            return _targetItems.GetOrAdd(targetID, targetID => new(CanvasDevice.GetSharedDevice(), targetID.targetLength, targetID.targetHeight, 96F, DirectXPixelFormat.B8G8R8A8UIntNormalized, CanvasAlphaMode.Ignore));
        }

        public ICanvasBrush[] GetFaintPaint(Color faintColor)
        {
            if (!_faintPaints.TryGetValue(faintColor, out var faintPaints))
            {
                faintPaints = new ICanvasBrush[101];
                DrawingSystem.Instance.SetFaintPaints(null, faintPaints, faintColor);
            }
            return faintPaints;
        }

        public CanvasTextLayout GetTextItem(string text, CanvasTextFormat font, float textLength = 0F, float textHeight = 0F)
        {
            var textID = new TextID
            {
                text = text,
                fontLength = font.FontSize,
                textLength = textLength,
                textHeight = textHeight,
                textSystem0 = font.HorizontalAlignment,
                textSystem1 = font.VerticalAlignment,
            };
            return _textItems.GetOrAdd(textID, (textID, font) =>
            {
                var textLength = textID.textLength;
                var textHeight = textID.textHeight;
                var textItem = new CanvasTextLayout(CanvasDevice.GetSharedDevice(), textID.text, font, textLength, textHeight);
                if (textLength > 0F && textHeight > 0F)
                {
                    textItem.Options = CanvasDrawTextOptions.Clip;
                }
                return textItem;
            }, font);
        }

        public FormattedText GetDefaultTextItem(string text, double fontLength, Brush fontPaint, double textLength = 0.0)
        {
            var defaultTextID = new DefaultTextID
            {
                text = text,
                fontLength = fontLength,
                textLength = textLength,
                fontPaint = fontPaint
            };
            return _defaultTextItems.GetOrAdd(defaultTextID, defaultTextID =>
            {
                var defaultTextItem = new FormattedText(defaultTextID.text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, Configure.Instance.FontFace, defaultTextID.fontLength, defaultTextID.fontPaint, 96.0)
                {
                    MaxLineCount = 1
                };
                var textLength = defaultTextID.textLength;
                if (textLength > 0.0)
                {
                    defaultTextItem.SetMaxTextWidths([textLength]);
                }
                return defaultTextItem;
            });
        }

        public string GetValueText(int value, string textFormat)
        {
            var valueTextID = new ValueTextID<int>
            {
                value = value,
                textFormat = textFormat
            };
            return _valueIntTexts.GetOrAdd(valueTextID, valueTextID =>
            {
                var value = valueTextID.value;
                var textFormat = valueTextID.textFormat;
                return string.IsNullOrEmpty(textFormat) ? value.ToString() : value.ToString(textFormat);
            });
        }

        public string GetValueText(double value, string textFormat)
        {
            var valueTextID = new ValueTextID<double>
            {
                value = value,
                textFormat = textFormat
            };
            return _valueFloat64Texts.GetOrAdd(valueTextID, valueTextID =>
            {
                var value = valueTextID.value;
                var textFormat = valueTextID.textFormat;
                return string.IsNullOrEmpty(textFormat) ? value.ToString() : value.ToString(textFormat);
            });
        }

        public string GetFormattedText(string textFormat, string param0, string param1 = null, string param2 = null, string param3 = null)
        {
            var formattedTextID = new FormattedTextID
            {
                textFormat = textFormat,
                param0 = param0,
                param1 = param1,
                param2 = param2,
                param3 = param3,
            };
            return _formattedTexts.GetOrAdd(formattedTextID, formattedTextID =>
            {
                return string.Format(formattedTextID.textFormat, formattedTextID.param0, formattedTextID.param1, formattedTextID.param2, formattedTextID.param3);
            });
        }

        public string GetFormattedUnitText(long value)
        {
            return _formattedUnitTexts.GetOrAdd(value, Utility.FormatLength);
        }
    }
}
