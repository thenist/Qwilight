-- https://taehui.ddns.net/qwilight/assistLS

function IsO4U()
	return configures[2] == 1
end

function IsFloating()
	return configures[3] == 1
end

function IsYellow()
	return configures[4] == 1
end

function DefaultLength(e)
	if configures[1] == 2 then
		return e * 720 * 21 / 9
	elseif configures[1] == 3 then
		return e * 720 * 32 / 9
	else
		return e * 1280
	end
end

function DefaultHeight(e)
	if configures[1] == 0 then
		return e * 1280 * 9 / 16
	elseif configures[1] == 1 then
		return e * 1280 * 10 / 16
	else
		return e * 720
	end
end

function StatusPosition1()
	return DefaultHeight(2 / 3)
end

function StatusHeight()
	return DefaultHeight(1 / 3)
end

function JudgmentPaintPosition1()
	return JudgmentMainPosition(-154) + judgmentPaintPosition
end

function JudgmentMainHeight()
	if IsFloating() then
		return 0
	else
		return 99
	end
end

function MainJudgmentMeterHeight()
	if IsFloating() then
		return 0
	else
		return 199
	end
end

function AutoHeight1()
	if IsFloating() then
		return 0
	else
		return 19
	end
end

function NoteLength(e)
	if inputCount < 50 then
		return (51 + noteLength) * e
	else
		return (34 + noteLength) * e
	end
end

function NoteHeight1(e)
	if IsO4U() and not IsFloating() then
		return 2.0 * NoteHeight2(e)
	else
		return NoteHeight2(e)
	end
end

function NoteHeight2(e)
	if inputCount < 50 then
		return (51 + noteLength + noteHeight) * e
	else
		return (34 + noteLength + noteHeight) * e
	end
end

function FloatingNotePosition0()
	if IsFloating() then
		if has2P then
			return NoteLength(defaultInputCount / 2) + 299.2
		else
			return NoteLength(defaultInputCount / 2)
		end
	else
		return 0
	end
end

function FloatingNoteLength1()
	if IsFloating() then
		if autoableInputCount < 2 then
			return NoteLength(defaultInputCount)
		else
			return NoteLength(defaultInputCount / 2)
		end
	else
		return 0
	end
end

function FloatingNoteLength6()
	if IsFloating() then
		return NoteLength(7 * defaultInputCount / 24)
	else
		return 0
	end
end

function NoteHitFrame(e)
	return 7 * e
end

function MainPosition(e)
	local position
	if inputCount < 11 then
		position = DefaultLength(1.0) - DefaultHeight(1.0) - 5 - P1BuiltLength(1.0)
	elseif inputCount < 16 then
		position = 138.9
	else
		position = 20.0
	end
	return mainPosition + position + e
end

function isNarrow()
	if has2P then
		return inputCount / 2 < 7
	else
		return inputCount < 7
	end
end

function isNarrower()
	if has2P then
		return inputCount / 2 < 5
	else
		return inputCount < 5
	end
end

function BandPosition1()
	return 120 + bandPosition
end

function JudgmentMeterPosition1()
	return 96 + bandPosition
end

function MultiplierPosition1()
	return JudgmentMainPosition(1.0) + 120 + 20 + 5
end

function AudioMultiplierPosition1()
	return JudgmentMainPosition(1.0) + 120 + 20 + 5 + 20 + 10
end

function InputVisualizerPosition1()
	return JudgmentMainPosition(1.0) + 120 + 20 + 5 + 20 + 10 + 20 + 5
end

function HmsSystem()
	if isNarrower() then
		return 2
	elseif isNarrow() then
		return 0
	else
		return 1
	end
end

function HmsPosition0()
	if isNarrower() then
		return MainPosition(-5) - 5
	elseif isNarrow() then
		return MainPosition(5)
	else
		return Contents(0.5)
	end
end

function HmsPosition1()
	return StandPosition1() - 10 - 20
end

function StandSystem()
	if isNarrower() then
		return 2
	else
		return 0
	end
end

function StandPosition0()
	if isNarrower() then
		return MainPosition(-5) - 5
	else
		return MainPosition(5)
	end
end

function StandPosition1()
	return JudgmentMainPosition(1.0) + 120 + 20 + 5 + 20 + 10
end

function HighestJudgmentValuePosition1(e)
	return DefaultHeight(1.0) - (5 + 20) * (6 - e)
end

function PointPosition1()
	return 175 + bandPosition
end

function BPMSystem()
	if isNarrow() then
		return 2
	else
		return 0
	end
end

function BPMPosition0()
	if isNarrow() then
		return MainPosition(-5) - 5 - 42.4
	else
		return MainPosition(5)
	end
end

function BPMPosition1()
	if isNarrower() then
		return HmsPosition1() - 10 - 20
	else
		return JudgmentMainPosition(1.0) + 120 + 20 + 5
	end
end

function MediaPosition0()
	if inputCount < 11 then
		return MainPosition(0) + P1BuiltLength(1.0) + 5
	else
		return 0
	end
end

function MediaLength()
	if inputCount < 11 then
		return DefaultHeight(1.0)
	else
		return DefaultLength(1.0)
	end
end

function JudgmentMainPosition(e)
	return DefaultHeight(1.0) - 76 - 20 - 120 + judgmentMainPositionV3 + e
end

function HitNotePaintPosition(e)
	return -HitNotePaintLength(e) / 2
end

function HitNotePaintLength(e)
	return 4.0 * NoteLength(e) + hitNotePaintArea
end

function LongNoteEdgePosition1()
	if not IsFloating() and IsO4U() then
		return -51
	else
		return -6
	end
end

function LongNoteEdgePosition2()
	if IsO4U() then
		return -26
	else
		return -6
	end
end

function LongNoteEdgeHeight1()
	if not IsFloating() and IsO4U() then
		return 102
	else
		return 11
	end
end

function LongNoteEdgeHeight2()
	if IsO4U() then
		return 51
	else
		return 11
	end
end

function PaintProperty4Position0()
	return Contents(0.5) - 4
end

function PaintProperty4Position1()
	return 214 + bandPosition
end

function JudgmentVisualizerPosition1()
	return 199 + bandPosition
end

function HunterPosition1()
	return 223 + bandPosition
end

function TitlePosition1()
	return ArtistPosition1() - 5 - 20
end

function TitleLength()
	return DefaultLength(1.0) - 5 - 5
end

function ArtistPosition1()
	return DefaultHeight(1.0) - 5 - 16
end

function ArtistLength()
	return DefaultLength(1.0) - 5 - 5
end

function PausedUnpausePosition0()
	return DefaultLength(0.5) - 202 / 2
end

function PausedUnpausePosition1(e)
	return DefaultHeight(0.5) - (4 * 53 + 3 * 10) / 2 + (53 + 10) * e
end

function DrawingInputMode(e)
	if e == 5 then
		if IsYellow() then
			return "2, 3, 4, 3, 2"
		else
			return "2, 3, 2, 3, 2"
		end
	elseif e == 7 then
		if IsYellow() then
			return "2, 3, 2, 5, 2, 3, 2"
		else
			return "2, 3, 2, 3, 2, 3, 2"
		end
	elseif e == 9 then
		if IsYellow() then
			return "2, 3, 2, 3, 4, 3, 2, 3, 2"
		else
			return "2, 3, 2, 3, 2, 3, 2, 3, 2"
		end
	elseif e == 10 then
		if IsYellow() then
			return "1, 2, 3, 4, 3, 2"
		else
			return "1, 2, 3, 2, 3, 2"
		end
	elseif e == 11 then
		if IsYellow() then
			return "1, 2, 3, 2, 5, 2, 3, 2"
		else
			return "1, 2, 3, 2, 3, 2, 3, 2"
		end
	elseif e == 12 then
		if IsYellow() then
			return "1, 2, 3, 4, 3, 2, 2, 3, 4, 3, 2, 10"
		else
			return "1, 2, 3, 2, 3, 2, 2, 3, 2, 3, 2, 10"
		end
	elseif e == 13 then
		if IsYellow() then
			return "1, 2, 3, 2, 5, 2, 3, 2, 2, 3, 2, 5, 2, 3, 2, 10"
		else
			return "1, 2, 3, 2, 3, 2, 3, 2, 2, 3, 2, 3, 2, 3, 2, 10"
		end
	end
end

function _GetNote(args)
	if IsFloating() and args[1] == 1 and (args[3] == 0 or args[3] == 2 or args[3] == 4 or args[3] == 9 or args[3] == 14) then
		return "N2"
	elseif IsO4U() and args[1] != 0 and (not IsFloating() or args[1] != 1) then
		return "N1"
	else
		return "N"
	end
end

function _GetMain(args)
	if IsFloating() and args[1] == 1 then
		return "M2"
	else
		return "M"
	end
end

function _GetInput(args)
	if IsFloating() and args[1] == 1 then
		return ""
	else
		return "I"
	end
end