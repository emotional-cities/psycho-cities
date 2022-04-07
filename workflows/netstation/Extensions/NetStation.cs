using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

public class NetStation : IDisposable
{
    private bool disposed;
    private readonly TcpClient client;
    private NetworkStream stream;

    public NetStation()
    {
        client = new TcpClient();
        client.NoDelay = true;
    }

    private async Task WriteBytesAsync(string value)
    {
        if (stream == null)
        {
            throw new InvalidOperationException("Client connection is not open.");
        }

        
        using (var memoryStream = new MemoryStream())
        using (var writer = new StreamWriter(memoryStream))
        {
            writer.Write(value);
            var buffer = memoryStream.ToArray();
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }

    private async Task WriteBytesAsync(int value)
    {
        var buffer = BitConverter.GetBytes(value);
        await stream.WriteAsync(buffer, 0, buffer.Length);
    }

    private async Task WriteBytesAsync(ushort value)
    {
        var buffer = BitConverter.GetBytes(value);
        await stream.WriteAsync(buffer, 0, buffer.Length);
    }

    private async Task WriteBytesAsync(uint value)
    {
        var buffer = BitConverter.GetBytes(value);
        await stream.WriteAsync(buffer, 0, buffer.Length);
    }

    private async Task WriteBytesAsync(byte value)
    {
        var buffer = new byte[] { value };
        await stream.WriteAsync(buffer, 0, buffer.Length);
    }

    private async Task<byte> ReadByteAsync()
    {
        if (stream == null)
        {
            throw new InvalidOperationException("Client connection is not open.");
        }

        var buffer = new byte[1];
        var received = await stream.ReadAsync(buffer, 0, buffer.Length);
        return buffer[0];
    }

    public async Task ConnectAsync(string host, int port)
    {
        await client.ConnectAsync(host, port);
        stream = client.GetStream();

        await WriteBytesAsync("QMAC-");
        var response = await ReadByteAsync();
        switch (response)
        {
            // 'F'
            case 0x46: throw new InvalidOperationException("ECI error.");

            // 'I'
            case 0x49:
                var version = await ReadByteAsync();
                if (version != 1)
                {
                    throw new InvalidOperationException("Unknown ECI version.");
                }
                break;
        }
    }

    public async Task StartRecordingAsync()
    {
        await WriteBytesAsync("B");
        var response = await ReadByteAsync();
    }

    public async Task StopRecordingAsync()
    {
        await WriteBytesAsync("E");
        var response = await ReadByteAsync();
    }

    public async Task EventAsync()
    {
        await WriteBytesAsync("D");
        await WriteBytesAsync((ushort)15);
        await WriteBytesAsync((ushort)15);
        var response = await ReadByteAsync();
    }

    public async Task DisconnectAsync()
    {
        await WriteBytesAsync("X");
        var response = await ReadByteAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                client.Dispose();
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}