using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.IO;
using Qwilight.Utilities;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Windows.Graphics.DirectX;

namespace Qwilight
{
    public sealed class PoolSystem
    {
        public static readonly PoolSystem Instance = new();

        readonly RecyclableMemoryStreamManager _rmsm = new();
        readonly ConcurrentDictionary<TextID, CanvasTextLayout> _textItems = new();
        readonly ConcurrentDictionary<DefaultTextID, FormattedText> _defaultTextItems = new();
        readonly ConcurrentDictionary<TargetID, CanvasRenderTarget> _targetItems = new();
        readonly ConcurrentDictionary<ValueTextID<int>, string> _valueIntTexts = new();
        readonly ConcurrentDictionary<ValueTextID<double>, string> _valueFloat64Texts = new();
        readonly ConcurrentDictionary<FormattedTextID, string> _formattedTexts = new();
        readonly ConcurrentDictionary<long, string> _formattedUnitTexts = new();

        PoolSystem()
        {
            _rmsm.GenerateCallStacks = !QwilightComponent.IsVS;
            _rmsm.AggressiveBufferReturn = true;
            _rmsm.MaximumFreeSmallPoolBytes = _rmsm.LargeBufferMultiple * 4;
            _rmsm.MaximumFreeLargePoolBytes = 100 * _rmsm.BlockSize;
        }

        public void Wipe(bool isWPFViewVisible)
        {
            if (isWPFViewVisible)
            {
                foreach (var (defaultTextID, defaultTextItem) in _defaultTextItems)
                {
                    _defaultTextItems.TryRemove(defaultTextID, out _);
                }
            }
            else
            {
                foreach (var (textID, textItem) in _textItems)
                {
                    using (textItem)
                    {
                        _textItems.TryRemove(textID, out _);
                    }
                }
                foreach (var (targetID, targetItem) in _targetItems)
                {
                    using (targetItem)
                    {
                        _targetItems.TryRemove(targetID, out _);
                    }
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
            foreach (var (value, text) in _formattedUnitTexts)
            {
                _formattedUnitTexts.TryRemove(value, out _);
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
                    defaultTextItem.SetMaxTextWidths(new[] { textLength });
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
            return _formattedUnitTexts.GetOrAdd(value, Utility.FormatUnit);
        }
    }
}
