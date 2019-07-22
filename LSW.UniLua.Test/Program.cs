using System;

namespace LSW.UniLua.Test
{
    class Program
    {
        static ILuaState Lua;
        static void Main(string[] args)
        {
            // 创建 Lua 虚拟机
            Lua = LuaAPI.NewState();

            // 加载基本库
            Lua.L_OpenLibs();

            // 注册自定义类
            Lua.L_RequireF(MyLib.LIB_NAME, MyLib.OpenLib, false);

            var LuaScriptFile = "main.lua";
            var status = Lua.L_DoFile(LuaScriptFile);

            // 捕获错误
            // capture errors
            if (status != ThreadStatus.LUA_OK)
            {
                throw new Exception(Lua.ToString(-1));
            }

            // 确保 main.lua 执行结果是一个 Lua table
            if (!Lua.IsTable(-1))
            {
                throw new Exception(
                      "framework main's return value is not a table");
            }

            var start = StoreMethod("start");

            CallMethod(start);

            Console.WriteLine(MyLib.Data);

            Console.ReadLine();
        }

        private static int StoreMethod(string name)
        {
            Lua.GetField(-1, name);
            if (!Lua.IsFunction(-1))
            {
                throw new Exception(string.Format(
                    "method {0} not found!", name));
            }
            return Lua.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        }

        private static void CallMethod(int funcRef)
        {
            Lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
            _ = Lua.PCall(0, 0, 0);
        }
    }
}
