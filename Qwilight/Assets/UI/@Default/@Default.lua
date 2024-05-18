-- https://taehui.ddns.net/qwilight/@assistLS

function IsNoteFileModeBGA()
	return configures[2] == 2
end

function IsNoteFileModeUHD()
	return configures[2] == 1
end

function IsQuitModeBGA()
	return configures[3] == 2
end

function IsQuitModeUHD()
	return configures[3] == 1
end

function DefaultLength()
	if configures[1] == 2 then
		return 720 * 21 / 9
	elseif configures[1] == 3 then
		return 720 * 32 / 9
	else
		return 1280
	end
end

function DefaultHeight()
	if configures[1] == 0 then
		return 1280 * 9 / 16
	elseif configures[1] == 1 then
		return 1280 * 10 / 16
	else
		return 720
	end
end

function CommentViewLength(e)
	return 4 * (DefaultLength() - 12 - 12 - 12 - 34) / 13 + e
end

function CommentViewHeight(e)
	return DefaultHeight() - 330 + e
end

function InputNoteCountViewPosition1(e)
	return DefaultHeight() - 288 + e
end

function EntryViewLength(e)
	return 9 * (DefaultLength() - 12 - 12 - 12 - 34) / 13 + e
end

function EntryViewHeight()
	return DefaultHeight() - 12 - 12 - 10 - 24
end

function AssistViewPosition1()
	return 12 + EntryViewHeight() + 10
end

function FilePosition1(e)
	return DefaultHeight() - 210 + 29 * e
end

function AutoModePosition0(e)
	return DefaultLength() - 36 + e
end

function AutoModePosition1(e)
	return DefaultHeight() - 444 + 34 * e
end

function PaintProperty22Position1()
	return DefaultHeight() - 449
end

function TitleQuitLength(e)
	return DefaultLength() / 2 - 10 + e
end

function TotalNotesQuitLength()
	return (DefaultLength() - 12 * 3) / 6
end

function JudgmentMeterViewPosition0()
	return DefaultLength() - JudgmentMeterViewLength() - 12
end

function JudgmentMeterViewPosition1()
	return (DefaultHeight() - 288 * 2 - 5 * 2) / 2
end

function JudgmentMeterViewLength()
	return 1 * DefaultLength() / 3
end

function StatusViewPosition1()
	return JudgmentMeterViewPosition1() + 288 + 5 * 2
end

function ViewCommentPosition0()
	return DefaultLength() - 125
end

function ViewCommentPosition1()
	return JudgmentMeterViewPosition1() - 50.3
end

function HandleUndoPosition0()
	return DefaultLength() - 232.3
end

function QuitDrawingPosition0()
	return (DefaultLength() / 2) - 72
end

function QuitDrawingPosition1()
	return PaintProperty9Position1(0) - 5 * 2 - 144
end

function PaintProperty9Position0()
	return DefaultLength() / 2 - 5 - 48
end

function PaintProperty9Position1(e)
	return PaintProperty13Position1() - 5 * 2 - 48 + e
end

function PaintProperty10Position0()
	return DefaultLength() / 2 + 5
end

function StandQuitPosition0(e)
	return DefaultLength() / 2 - 216 + e
end

function StandQuitPosition1(e)
	return PaintProperty13Position1() + 5 + 58 * e
end

function PaintProperty13Position1()
	return PaintProperty12Position1() + 406 - 58 * 3
end

function StandContentsQuitPosition0()
	return DefaultLength() / 2
end

function AutoModeQuitPosition1(e)
	return DefaultHeight() - 5 - 24 + e
end

function JudgmentStageQuitPosition0(e)
	return JudgmentMeterViewPosition0() + 5 + e
end

function JudgmentStageQuitPosition1(e)
	return JudgmentMeterViewPosition1() + 288 - 5 - 12 - 17 * e
end

function JudgmentStageQuitLength()
	return JudgmentMeterViewLength() - 5 - 48 - 5
end

function LengthQuitPosition0(e)
	return JudgmentMeterViewPosition0() + 5 + e
end

function LengthQuitPosition1(e)
	return StatusViewPosition1() + 288 - 5 - 12 - 17 * e
end

function TotalNotesQuitPosition1(e)
	return 170 + (DefaultHeight() - 644) / 2 + 58 * e
end

function PaintProperty12Position1()
	return TotalNotesQuitPosition1(0) - 5
end

function PaintProperty23Position0(e)
	return DefaultLength(1.0) - e
end

function PaintProperty0Variety()
	if IsNoteFileModeBGA() then
		return 2
	else
		return 11
	end
end

function PaintProperty1Variety()
	if IsNoteFileModeBGA() then
		return 1
	else
		return 2
	end
end

function PaintProperty0Etc()
	if IsNoteFileModeUHD() then
		return "[UHD0.mp4, UHD1.mp4, UHD2.mp4, UHD3.mp4, UHD4.mp4, UHD5.mp4, UHD6.mp4, UHD7.mp4, UHD8.mp4, UHD9.mp4]"
	else
		return "[HD0.mp4, HD1.mp4, HD2.mp4, HD3.mp4, HD4.mp4, HD5.mp4, HD6.mp4, HD7.mp4, HD8.mp4, HD9.mp4]"
	end
end

function PaintProperty3Variety()
	if IsQuitModeBGA() then
		return 2
	else
		return 11
	end
end

function PaintProperty6Variety()
	if IsQuitModeBGA() then
		return 1
	else
		return 2
	end
end