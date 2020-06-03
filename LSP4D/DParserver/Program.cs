using System;
using System.IO;
using System.Threading.Tasks;

namespace D_Parserver
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var stdin = Console.OpenStandardInput()
                /*File.OpenRead("/tmp/stdin.test.log");*/
                /*new StreamDump(Console.OpenStandardInput(), "/tmp/stdin.log")*/;
            var stdout = Console.OpenStandardOutput() /* new StreamDump(Console.OpenStandardOutput(), "/tmp/stdout.log") */;

            var server = await DLanguageServerFactory.CreateServer(stdin, stdout);

            await server.WaitForExit;
        }

        class StreamDump : Stream
        {
            public readonly Stream baseStream;
            public readonly FileStream fileToDumpTo;

            public StreamDump(Stream baseStream, string fileToDumpTo)
            {
                this.baseStream = baseStream;
                this.fileToDumpTo = new FileStream(fileToDumpTo, FileMode.Create, FileAccess.Write, FileShare.Read);
            }

            void Log(byte[] buffer, int offset, int count)
            {
                fileToDumpTo.Write(buffer, offset, count);
                fileToDumpTo.Flush();
            }

            public override void Close()
            {
                baseStream.Close();
                fileToDumpTo.Close();
            }

            protected override void Dispose(bool disposing)
            {
                baseStream.Dispose();
                fileToDumpTo.Dispose();
            }

            public override void Flush()
            {
                baseStream.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var readb = baseStream.Read(buffer, offset, count);
                Log(buffer, offset, readb);
                return readb;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return baseStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                baseStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                Log(buffer, offset, count);
                baseStream.Write(buffer, offset, count);
            }

            public override bool CanRead => baseStream.CanRead;
            public override bool CanSeek => baseStream.CanSeek;
            public override bool CanWrite => baseStream.CanWrite;
            public override long Length => baseStream.Length;
            public override long Position
            {
                get => baseStream.Position;
                set => baseStream.Position = value;
            }
        }
        
    }
}