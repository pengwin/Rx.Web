using System.Net;
using System.Net.Sockets;

namespace RxWeb;

public class Server: IDisposable
{
    private readonly TcpListener _listener;
    
    public Server(string addr, int port)
    {
        var localAddr = IPAddress.Parse(addr);
        _listener = new TcpListener(localAddr, port);
        _listener.Start();
    }

    public async Task<TcpClient> AcceptClientAsync(CancellationToken cancellationToken)
    {
        return await _listener.AcceptTcpClientAsync(cancellationToken);
    }

    public void Dispose()
    {
        _listener.Stop();
    }
}