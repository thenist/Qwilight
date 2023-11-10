-- https://taehui.ddns.net/qwilight/assistLS

function IsO4U()
	return configures[2] == 1
end

function IsClassic()
	return configures[3] == 1
end

function IsFloating()
	return configures[4] == 1
end

function IsYellow()
	return configures[5] == 1
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

function Pipeline()
	if IsClassic() then
		return "1, 101, 26, 3, 4, 7, 8, 57, 31, 102, 103, 2, 6, 9, 10, 11, 12, 13, 14, 30, 28, 15, 16, 27, 29, 33, 35, 36, 37, 38, 39, 40, 43, 44, 46, 47, 48, 51, 58, 104"
	else
		return "1, 101, 26, 3, 4, 7, 8, 57, 31, 102, 103, 61, 2, 6, 9, 10, 11, 12, 13, 14, 30, 28, 15, 16, 27, 29, 33, 35, 36, 37, 38, 39, 40, 43, 44, 46, 47, 48, 51, 58, 104"
	end
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
		position = 555 - P1BuiltLength(1.0)
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

function NetPosition1()
	if IsClassic() then
		return 30
	else
		return 5
	end
end

function BandPosition1()
	return 120 + bandPosition
end

function JudgmentMeterPosition1()
	return 96 + bandPosition
end

function AltClassic()
	if IsClassic() then
		return 0
	else
		return 2
	end
end

function MultiplierPosition0()
	if IsClassic() then
		return 1275
	else
		return Wall(-5)
	end
end

function MultiplierPosition1()
	if IsClassic() then
		return 599
	else
		return JudgmentMainPosition(1.0) + 120 + 20 + 5
	end
end

function AudioMultiplierPosition0()
	if IsClassic() then
		return 1275
	else
		return Wall(-5)
	end
end

function AudioMultiplierPosition1()
	if IsClassic() then
		return 624
	else
		return JudgmentMainPosition(1.0) + 120 + 20 + 5 + 20 + 10
	end
end

function InputVisualizerSystem()
	if IsClassic() then
		return 0
	else
		return 2
	end
end

function InputVisualizerPosition0()
	if IsClassic() then
		return Wall(25)
	else
		return Wall(-5)
	end
end

function InputVisualizerPosition1()
	if IsClassic() then
		return 554
	else
		return JudgmentMainPosition(1.0) + 120 + 20 + 5 + 20 + 10 + 20 + 5
	end
end

function HmsSystem()
	if IsClassic() then
		return 0
	else
		if isNarrower() then
			return 2
		elseif isNarrow() then
			return 0
		else
			return 1
		end
	end
end

function HmsPosition0()
	if IsClassic() then
		return Wall(25)
	else
		if isNarrower() then
			return MainPosition(-5) - 5
		elseif isNarrow() then
			return MainPosition(5)
		else
			return Contents(0.5)
		end
	end
end

function HmsPosition1()
	if IsClassic() then
		return 5
	else
		return JudgmentMainPosition(1.0) + 120 + 20 + 5
	end
end

function StandSystem()
	if IsClassic() then
		return 2
	else
		if isNarrower() then
			return 2
		elseif isNarrow() then
			return 0
		else
			return 0
		end
	end
end

function StandPosition0()
	if IsClassic() then
		return 1275
	else
		if isNarrower() then
			return MainPosition(-5) - 5
		elseif isNarrow() then
			return MainPosition(5)
		else
			return MainPosition(5)
		end
	end
end

function StandPosition1()
	if IsClassic() then
		return 5
	else
		return JudgmentMainPosition(1.0) + 120 + 20 + 5 + 20 + 10
	end
end

function LowestJudgmentValuePosition1(e)
	return DefaultHeight(1.0) - (20 + 5) * e
end

function PointSystem()
	if IsClassic() then
		return 2
	else
		return 1
	end
end

function PointPosition0()
	if IsClassic() then
		return 1275
	else
		return Contents(0.5)
	end
end

function PointPosition1()
	if IsClassic() then
		return 46
	else
		return 175 + bandPosition
	end
end

function BPMSystem()
	if IsClassic() then
		return 2
	else
		if isNarrow() then
			return 2
		else
			return 0
		end
	end
end

function BPMPosition0()
	if IsClassic() then
		return 1232.6
	else
		if isNarrow() then
			return MainPosition(-5) - 5 - 42.4
		else
			return MainPosition(5)
		end
	end
end

function BPMPosition1()
	if IsClassic() then
		return 649
	else
		if isNarrower() then
			return 619
		else
			return JudgmentMainPosition(1.0) + 120 + 20 + 5
		end
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
	local position =  DefaultHeight(1.0) - 76 - 20 - 120 + judgmentMainPositionV3 + e
	if IsClassic() then
		position = position + 76
	end
	return position
end

function HitNotePaintPosition(e)
	return -2.0 * NoteLength(e)
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
	local position
	if IsClassic() then
		position = 190
	else
		position = 214
	end
	return position + bandPosition
end

function PaintProperty2Frame()
	if IsClassic() then
		return 0
	else
		return 1
	end
end

function JudgmentVisualizerPosition1()
	local position
	if IsClassic() then
		position = 175
	else
		position = 199
	end
	return position + bandPosition
end

function HunterPosition1()
	local position
	if IsClassic() then
		position = 199
	else
		position = 223
	end
	return position + bandPosition
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