luanet.load_assembly("Microsoft.Xna.Framework");
luanet.load_assembly("Microsoft.Xna.Framework.Game");
luanet.load_assembly("Ultima.Xna");

Keys = luanet.import_type("Microsoft.Xna.Framework.Input.Keys")
MouseButtons = luanet.import_type("Ultima.Xna.Input.MouseButtons")

luanet.load_assembly = function(assembly)
   print("this function is unsupported")
end

luanet.import_type = function (type)
   print("this function is unsupported")
end

function PrintDebug(msg)
	UI.PrintDebug(msg)
end
function PrintInfo(msg)
	UI.PrintInfo(msg)
end
function PrintWarn(msg)
	UI.PrintWarn(msg)
end
function PrintError(msg)
	UI.PrintError(msg)
end
function PrintFatal(msg)
	UI.PrintFatal(msg)
end

