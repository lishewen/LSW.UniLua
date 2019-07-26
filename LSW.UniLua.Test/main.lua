local mylib = require "MyLib"

local function start()
	mylib.push("---- start ----")
	mylib.push(mylib.add(10,9))

	mylib.push(table.concat{"hello", ":", "UniLua"})
	
	local a = 10
	local b = tostring(a)
	mylib.push(table.concat{type(a)," ",type(b)})
end

return {
	start  = start
}