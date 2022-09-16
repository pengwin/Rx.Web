using System.Net.Sockets;

namespace RxWeb;



class Request
{
    public enum HttpMethod
    {
        UNKNOWN, GET, POST, UPDATE, DELETE
    }
    
    public void Deconstruct(out HttpMethod? method, out string? path)
    {
        method = Method;
        path = Path?.ToString();
    }

    private readonly Dictionary<string, string> _headers;

    public Request(TcpClient client)
    {
        Client = client;
        _headers = new Dictionary<string, string>();
    }

    public TcpClient Client { get; }

    public IReadOnlyDictionary<string, string> Headers => _headers;

    public HttpMethod? Method { get; private set; }

    public Uri? Path { get; private set; }

    public void AddHeader(string key, string value)
    {
        if (key == "HttpMethod" && Enum.TryParse(value, out HttpMethod method))
        {
            Method = method;
        }
        else if (key == "HttpPath" && Uri.TryCreate(value, UriKind.Relative, out var uri))
        {
            Path = uri;
        }
        else
        {
            _headers.Add(key, value);
        }
    }
}