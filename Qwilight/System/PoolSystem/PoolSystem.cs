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
        readonly Dictionary<TextID, CanvasTextLayout> _textItems = new();
        readonly Dictionary<DefaultTextID, FormattedText> _defaultTextItems = new();
        readonly Dictionary<TargetID, CanvasRenderTarget> _targetItems = new();
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
            if (!_targetItems.TryGetValue(targetID, out var targetItem))
            {
                targetItem = new(CanvasDevice.GetSharedDevice(), targetLength, targetHeight, 96F, DirectXPixelFormat.B8G8R8A8UIntNormalized, CanvasAlphaMode.Ignore);
                _targetItems[targetID] = targetItem;
            }
            return targetItem;
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
            if (!_textItems.TryGetValue(textID, out var textItem))
            {
                textItem = new(CanvasDevice.GetSharedDevice(), text, font, textLength, textHeight);
                if (textLength > 0F && textHeight > 0F)
                {
                    textItem.Options = CanvasDrawTextOptions.Clip;
                }
                _textItems[textID] = textItem;
            }
            return textItem;
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
            if (!_defaultTextItems.TryGetValue(defaultTextID, out var defaultTextItem))
            {
                defaultTextItem = new(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, Configure.Instance.FontFace, fontLength, fontPaint, 96.0)
                {
                    MaxLineCount = 1
                };
                if (textLength > 0.0)
                {
                    defaultTextItem.SetMaxTextWidths(new[] { textLength });
                }
                _defaultTextItems[defaultTextID] = defaultTextItem;
            }
            return defaultTextItem;
        }

        public string GetValueText(int value, string textFormat)
        {
            var valueTextID = new ValueTextID<int>
            {
                value = value,
                textFormat = textFormat
            };
            if (!_valueIntTexts.TryGetValue(valueTextID, out var valueText))
            {
                valueText = string.IsNullOrEmpty(textFormat) ? value.ToString() : value.ToString(textFormat);
                _valueIntTexts[valueTextID] = valueText;
            }
            return valueText;
        }

        public string GetValueText(double value, string textFormat)
        {
            var valueTextID = new ValueTextID<double>
            {
                value = value,
                textFormat = textFormat
            };
            if (!_valueFloat64Texts.TryGetValue(valueTextID, out var valueText))
            {
                valueText = string.IsNullOrEmpty(textFormat) ? value.ToString() : value.ToString(textFormat);
                _valueFloat64Texts[valueTextID] = valueText;
            }
            return valueText;
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
            if (!_formattedTexts.TryGetValue(formattedTextID, out var formattedText))
            {
                formattedText = string.Format(textFormat, param0, param1, param2, param3);
                _formattedTexts[formattedTextID] = formattedText;
            }
            return formattedText;
        }

        public string GetFormattedUnitText(long value)
        {
            if (!_formattedUnitTexts.TryGetValue(value, out var formattedUnitText))
            {
                formattedUnitText = Utility.FormatUnit(value);
                _formattedUnitTexts[value] = formattedUnitText;
            }
            return formattedUnitText;
        }
    }
}
