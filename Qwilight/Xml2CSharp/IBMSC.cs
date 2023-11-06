/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace Xml2CSharp
{
	[XmlRoot(ElementName = "Form")]
	public class Form
	{
		[XmlAttribute(AttributeName = "WindowState")]
		public string WindowState { get; set; }
		[XmlAttribute(AttributeName = "Width")]
		public string Width { get; set; }
		[XmlAttribute(AttributeName = "Height")]
		public string Height { get; set; }
		[XmlAttribute(AttributeName = "Top")]
		public string Top { get; set; }
		[XmlAttribute(AttributeName = "Left")]
		public string Left { get; set; }
	}

	[XmlRoot(ElementName = "Recent")]
	public class Recent
	{
		[XmlAttribute(AttributeName = "Recent0")]
		public string Recent0 { get; set; }
		[XmlAttribute(AttributeName = "Recent1")]
		public string Recent1 { get; set; }
		[XmlAttribute(AttributeName = "Recent2")]
		public string Recent2 { get; set; }
		[XmlAttribute(AttributeName = "Recent3")]
		public string Recent3 { get; set; }
		[XmlAttribute(AttributeName = "Recent4")]
		public string Recent4 { get; set; }
	}

	[XmlRoot(ElementName = "Edit")]
	public class Edit
	{
		[XmlAttribute(AttributeName = "NTInput")]
		public string NTInput { get; set; }
		[XmlAttribute(AttributeName = "Language")]
		public string Language { get; set; }
		[XmlAttribute(AttributeName = "ErrorCheck")]
		public string ErrorCheck { get; set; }
		[XmlAttribute(AttributeName = "AutoFocusMouseEnter")]
		public string AutoFocusMouseEnter { get; set; }
		[XmlAttribute(AttributeName = "FirstClickDisabled")]
		public string FirstClickDisabled { get; set; }
		[XmlAttribute(AttributeName = "ShowFileName")]
		public string ShowFileName { get; set; }
		[XmlAttribute(AttributeName = "MiddleButtonMoveMethod")]
		public string MiddleButtonMoveMethod { get; set; }
		[XmlAttribute(AttributeName = "AutoSaveInterval")]
		public string AutoSaveInterval { get; set; }
		[XmlAttribute(AttributeName = "PreviewOnClick")]
		public string PreviewOnClick { get; set; }
		[XmlAttribute(AttributeName = "ClickStopPreview")]
		public string ClickStopPreview { get; set; }
	}

	[XmlRoot(ElementName = "Save")]
	public class Save
	{
		[XmlAttribute(AttributeName = "TextEncoding")]
		public string TextEncoding { get; set; }
		[XmlAttribute(AttributeName = "BMSGridLimit")]
		public string BMSGridLimit { get; set; }
		[XmlAttribute(AttributeName = "BeepWhileSaved")]
		public string BeepWhileSaved { get; set; }
		[XmlAttribute(AttributeName = "BPMx1296")]
		public string BPMx1296 { get; set; }
		[XmlAttribute(AttributeName = "STOPx1296")]
		public string STOPx1296 { get; set; }
	}

	[XmlRoot(ElementName = "WAV")]
	public class WAV
	{
		[XmlAttribute(AttributeName = "WAVMultiSelect")]
		public string WAVMultiSelect { get; set; }
		[XmlAttribute(AttributeName = "WAVChangeLabel")]
		public string WAVChangeLabel { get; set; }
		[XmlAttribute(AttributeName = "BeatChangeMode")]
		public string BeatChangeMode { get; set; }
	}

	[XmlRoot(ElementName = "ShowHide")]
	public class ShowHide
	{
		[XmlAttribute(AttributeName = "showMenu")]
		public string ShowMenu { get; set; }
		[XmlAttribute(AttributeName = "showTB")]
		public string ShowTB { get; set; }
		[XmlAttribute(AttributeName = "showOpPanel")]
		public string ShowOpPanel { get; set; }
		[XmlAttribute(AttributeName = "showStatus")]
		public string ShowStatus { get; set; }
		[XmlAttribute(AttributeName = "showLSplit")]
		public string ShowLSplit { get; set; }
		[XmlAttribute(AttributeName = "showRSplit")]
		public string ShowRSplit { get; set; }
	}

	[XmlRoot(ElementName = "Grid")]
	public class Grid
	{
		[XmlAttribute(AttributeName = "gSnap")]
		public string GSnap { get; set; }
		[XmlAttribute(AttributeName = "gWheel")]
		public string GWheel { get; set; }
		[XmlAttribute(AttributeName = "gPgUpDn")]
		public string GPgUpDn { get; set; }
		[XmlAttribute(AttributeName = "gShow")]
		public string GShow { get; set; }
		[XmlAttribute(AttributeName = "gShowS")]
		public string GShowS { get; set; }
		[XmlAttribute(AttributeName = "gShowBG")]
		public string GShowBG { get; set; }
		[XmlAttribute(AttributeName = "gShowM")]
		public string GShowM { get; set; }
		[XmlAttribute(AttributeName = "gShowV")]
		public string GShowV { get; set; }
		[XmlAttribute(AttributeName = "gShowMB")]
		public string GShowMB { get; set; }
		[XmlAttribute(AttributeName = "gShowC")]
		public string GShowC { get; set; }
		[XmlAttribute(AttributeName = "gBPM")]
		public string GBPM { get; set; }
		[XmlAttribute(AttributeName = "gSTOP")]
		public string GSTOP { get; set; }
		[XmlAttribute(AttributeName = "gSCROLL")]
		public string GSCROLL { get; set; }
		[XmlAttribute(AttributeName = "gBLP")]
		public string GBLP { get; set; }
		[XmlAttribute(AttributeName = "gP2")]
		public string GP2 { get; set; }
		[XmlAttribute(AttributeName = "gCol")]
		public string GCol { get; set; }
		[XmlAttribute(AttributeName = "gDivide")]
		public string GDivide { get; set; }
		[XmlAttribute(AttributeName = "gSub")]
		public string GSub { get; set; }
		[XmlAttribute(AttributeName = "gSlash")]
		public string GSlash { get; set; }
		[XmlAttribute(AttributeName = "gxHeight")]
		public string GxHeight { get; set; }
		[XmlAttribute(AttributeName = "gxWidth")]
		public string GxWidth { get; set; }
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "WaveForm")]
	public class WaveForm
	{
		[XmlAttribute(AttributeName = "wLock")]
		public string WLock { get; set; }
		[XmlAttribute(AttributeName = "wPosition")]
		public string WPosition { get; set; }
		[XmlAttribute(AttributeName = "wLeft")]
		public string WLeft { get; set; }
		[XmlAttribute(AttributeName = "wWidth")]
		public string WWidth { get; set; }
		[XmlAttribute(AttributeName = "wPrecision")]
		public string WPrecision { get; set; }
	}

	[XmlRoot(ElementName = "Player")]
	public class Player
	{
		[XmlAttribute(AttributeName = "Index")]
		public string Index { get; set; }
		[XmlAttribute(AttributeName = "Path")]
		public string Path { get; set; }
		[XmlAttribute(AttributeName = "FromBeginning")]
		public string FromBeginning { get; set; }
		[XmlAttribute(AttributeName = "FromHere")]
		public string FromHere { get; set; }
		[XmlAttribute(AttributeName = "Stop")]
		public string Stop { get; set; }
	}

	[XmlRoot(ElementName = "Player")]
	public class Players
	{
		[XmlElement(ElementName = "Player")]
		public List<Player> Player { get; set; }
		[XmlAttribute(AttributeName = "Count")]
		public string Count { get; set; }
		[XmlAttribute(AttributeName = "CurrentPlayer")]
		public string CurrentPlayer { get; set; }
	}

	[XmlRoot(ElementName = "Column")]
	public class Column
	{
		[XmlAttribute(AttributeName = "Index")]
		public string Index { get; set; }
		[XmlAttribute(AttributeName = "Width")]
		public string Width { get; set; }
		[XmlAttribute(AttributeName = "Title")]
		public string Title { get; set; }
		[XmlAttribute(AttributeName = "NoteColor")]
		public string NoteColor { get; set; }
		[XmlAttribute(AttributeName = "TextColor")]
		public string TextColor { get; set; }
		[XmlAttribute(AttributeName = "LongNoteColor")]
		public string LongNoteColor { get; set; }
		[XmlAttribute(AttributeName = "LongTextColor")]
		public string LongTextColor { get; set; }
		[XmlAttribute(AttributeName = "BG")]
		public string BG { get; set; }
	}

	[XmlRoot(ElementName = "Columns")]
	public class Columns
	{
		[XmlElement(ElementName = "Column")]
		public List<Column> Column { get; set; }
	}

	[XmlRoot(ElementName = "ColumnTitle")]
	public class ColumnTitle
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "ColumnTitleFont")]
	public class ColumnTitleFont
	{
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "Size")]
		public string Size { get; set; }
		[XmlAttribute(AttributeName = "Style")]
		public string Style { get; set; }
	}

	[XmlRoot(ElementName = "Bg")]
	public class Bg
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "Sub")]
	public class Sub
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "VLine")]
	public class VLine
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "MLine")]
	public class MLine
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "BGMWav")]
	public class BGMWav
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "SelBox")]
	public class SelBox
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "TSCursor")]
	public class TSCursor
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "TSHalf")]
	public class TSHalf
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "TSDeltaMouseOver")]
	public class TSDeltaMouseOver
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "TSMouseOver")]
	public class TSMouseOver
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "TSSel")]
	public class TSSel
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "TSBPM")]
	public class TSBPM
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "TSBPMFont")]
	public class TSBPMFont
	{
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "Size")]
		public string Size { get; set; }
		[XmlAttribute(AttributeName = "Style")]
		public string Style { get; set; }
	}

	[XmlRoot(ElementName = "MiddleDeltaRelease")]
	public class MiddleDeltaRelease
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "kHeight")]
	public class KHeight
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "kFont")]
	public class KFont
	{
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "Size")]
		public string Size { get; set; }
		[XmlAttribute(AttributeName = "Style")]
		public string Style { get; set; }
	}

	[XmlRoot(ElementName = "kMFont")]
	public class KMFont
	{
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "Size")]
		public string Size { get; set; }
		[XmlAttribute(AttributeName = "Style")]
		public string Style { get; set; }
	}

	[XmlRoot(ElementName = "kLabelVShift")]
	public class KLabelVShift
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "kLabelHShift")]
	public class KLabelHShift
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "kLabelHShiftL")]
	public class KLabelHShiftL
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "kMouseOver")]
	public class KMouseOver
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "kMouseOverE")]
	public class KMouseOverE
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "kSelected")]
	public class KSelected
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "kOpacity")]
	public class KOpacity
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "VisualSettings")]
	public class VisualSettings
	{
		[XmlElement(ElementName = "ColumnTitle")]
		public ColumnTitle ColumnTitle { get; set; }
		[XmlElement(ElementName = "ColumnTitleFont")]
		public ColumnTitleFont ColumnTitleFont { get; set; }
		[XmlElement(ElementName = "Bg")]
		public Bg Bg { get; set; }
		[XmlElement(ElementName = "Grid")]
		public Grid Grid { get; set; }
		[XmlElement(ElementName = "Sub")]
		public Sub Sub { get; set; }
		[XmlElement(ElementName = "VLine")]
		public VLine VLine { get; set; }
		[XmlElement(ElementName = "MLine")]
		public MLine MLine { get; set; }
		[XmlElement(ElementName = "BGMWav")]
		public BGMWav BGMWav { get; set; }
		[XmlElement(ElementName = "SelBox")]
		public SelBox SelBox { get; set; }
		[XmlElement(ElementName = "TSCursor")]
		public TSCursor TSCursor { get; set; }
		[XmlElement(ElementName = "TSHalf")]
		public TSHalf TSHalf { get; set; }
		[XmlElement(ElementName = "TSDeltaMouseOver")]
		public TSDeltaMouseOver TSDeltaMouseOver { get; set; }
		[XmlElement(ElementName = "TSMouseOver")]
		public TSMouseOver TSMouseOver { get; set; }
		[XmlElement(ElementName = "TSSel")]
		public TSSel TSSel { get; set; }
		[XmlElement(ElementName = "TSBPM")]
		public TSBPM TSBPM { get; set; }
		[XmlElement(ElementName = "TSBPMFont")]
		public TSBPMFont TSBPMFont { get; set; }
		[XmlElement(ElementName = "MiddleDeltaRelease")]
		public MiddleDeltaRelease MiddleDeltaRelease { get; set; }
		[XmlElement(ElementName = "kHeight")]
		public KHeight KHeight { get; set; }
		[XmlElement(ElementName = "kFont")]
		public KFont KFont { get; set; }
		[XmlElement(ElementName = "kMFont")]
		public KMFont KMFont { get; set; }
		[XmlElement(ElementName = "kLabelVShift")]
		public KLabelVShift KLabelVShift { get; set; }
		[XmlElement(ElementName = "kLabelHShift")]
		public KLabelHShift KLabelHShift { get; set; }
		[XmlElement(ElementName = "kLabelHShiftL")]
		public KLabelHShiftL KLabelHShiftL { get; set; }
		[XmlElement(ElementName = "kMouseOver")]
		public KMouseOver KMouseOver { get; set; }
		[XmlElement(ElementName = "kMouseOverE")]
		public KMouseOverE KMouseOverE { get; set; }
		[XmlElement(ElementName = "kSelected")]
		public KSelected KSelected { get; set; }
		[XmlElement(ElementName = "kOpacity")]
		public KOpacity KOpacity { get; set; }
	}

	[XmlRoot(ElementName = "iBMSC")]
	public class IBMSC
	{
		[XmlElement(ElementName = "Form")]
		public Form Form { get; set; }
		[XmlElement(ElementName = "Recent")]
		public Recent Recent { get; set; }
		[XmlElement(ElementName = "Edit")]
		public Edit Edit { get; set; }
		[XmlElement(ElementName = "Save")]
		public Save Save { get; set; }
		[XmlElement(ElementName = "WAV")]
		public WAV WAV { get; set; }
		[XmlElement(ElementName = "ShowHide")]
		public ShowHide ShowHide { get; set; }
		[XmlElement(ElementName = "Grid")]
		public Grid Grid { get; set; }
		[XmlElement(ElementName = "WaveForm")]
		public WaveForm WaveForm { get; set; }
		[XmlElement(ElementName = "Player")]
		public Players Players { get; set; }
		[XmlElement(ElementName = "Columns")]
		public Columns Columns { get; set; }
		[XmlElement(ElementName = "VisualSettings")]
		public VisualSettings VisualSettings { get; set; }
		[XmlAttribute(AttributeName = "Major")]
		public string Major { get; set; }
		[XmlAttribute(AttributeName = "Minor")]
		public string Minor { get; set; }
		[XmlAttribute(AttributeName = "Build")]
		public string Build { get; set; }
	}

}
