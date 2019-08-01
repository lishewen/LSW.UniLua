using System;
using System.IO;
using System.Collections.Generic;

namespace LSW.UniLua
{
    public delegate string PathHook(string filename);
    public class LuaFile
    {
        private static readonly string LUA_ROOT = Directory.GetCurrentDirectory();
        private static PathHook pathhook = (s) => Path.Combine(LUA_ROOT, s);
        public static void SetPathHook(PathHook hook)
        {
            pathhook = hook;
        }

        public static FileLoadInfo OpenFile(string filename)
        {
            var path = pathhook(filename);
            return new FileLoadInfo(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        }

        public static bool Readable(string filename)
        {
            var path = pathhook(filename);
            try
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class FileLoadInfo : ILoadInfo, IDisposable
    {
        public FileLoadInfo(FileStream stream)
        {
            Stream = stream;
            Reader = new StreamReader(Stream, System.Text.Encoding.UTF8);
            Buf = new Queue<char>();
        }

        public int ReadByte()
        {
            if (Buf.Count > 0)
                return (int)Buf.Dequeue();
            else
                return Reader.Read();
        }

        public int PeekByte()
        {
            if (Buf.Count > 0)
                return (int)Buf.Peek();
            else
            {
                var c = Reader.Read();
                if (c == -1)
                    return c;
                Save((char)c);
                return c;
            }
        }

        public void Dispose()
        {
            Stream.Dispose();
        }

        private const string UTF8_BOM = "\u00EF\u00BB\u00BF";
        private readonly FileStream Stream;
        private readonly StreamReader Reader;
        private readonly Queue<char> Buf;

        private void Save(char b)
        {
            Buf.Enqueue(b);
        }

        private void Clear()
        {
            Buf.Clear();
        }

        private int SkipBOM()
        {
            for (var i = 0; i < UTF8_BOM.Length; ++i)
            {
                var c = Stream.ReadByte();
                if (c == -1 || c != (byte)UTF8_BOM[i])
                    return c;
                Save((char)c);
            }
            // perfix matched; discard it
            Clear();
            return Stream.ReadByte();
        }

        public void SkipComment()
        {
            var c = Reader.Read();

            // first line is a comment (Unix exec. file)?
            if (c == '#')
            {
                do
                {
                    c = Reader.Read();
                } while (c != -1 && c != '\n');
                Save('\n'); // fix line number
            }
            else if (c != -1)
            {
                Save((char)c);
            }
        }
    }

}

