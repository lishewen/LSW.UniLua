using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSW.UniLua.Test
{
    public static class MyLib
    {
        public const string LIB_NAME = "MyLib"; // 库的名称, 可以是任意字符串
        public static string Data;

        public static int OpenLib(ILuaState lua) // 库的初始化函数
        {
            var define = new NameFuncPair[]
        {
            new NameFuncPair("add", Add),
            new NameFuncPair("sub", Sub),
            new NameFuncPair("push",PushData)
        };

            lua.L_NewLib(define);
            return 1;
        }

        public static int Add(ILuaState lua)
        {
            var a = lua.L_CheckNumber(1); // 第一个参数
            var b = lua.L_CheckNumber(2); // 第二个参数
            var c = a + b; // 执行加法操作
            lua.PushNumber(c); // 将返回值入栈
            return 1; // 有一个返回值
        }

        public static int Sub(ILuaState lua)
        {
            var a = lua.L_CheckNumber(1); // 第一个参数
            var b = lua.L_CheckNumber(2); // 第二个参数
            var c = a - b; // 执行减法操作
            lua.PushNumber(c); // 将返回值入栈
            return 1; // 有一个返回值
        }

        public static int PushData(ILuaState lua)
        {
            var a = lua.L_CheckString(1);
            Data += a + "\r\n";
            lua.PushString(a); // 将返回值入栈
            return 1; // 有一个返回值
        }
    }
}
