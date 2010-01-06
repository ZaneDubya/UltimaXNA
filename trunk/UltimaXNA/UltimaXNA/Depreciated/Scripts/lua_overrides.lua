
local oldprint = print
print = function(x)
	if UI then
		UI:PrintDebug(tostring(x))
	end
	
	oldprint(x)
end