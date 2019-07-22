local mylib = require "MyLib"

local function start()
    mylib.push("---- start ----")
    mylib.push(mylib.add(10,9))
end

return {
    start  = start
}