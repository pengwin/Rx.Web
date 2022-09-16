using System.Net;
using System.Text;

namespace RxWeb;

class Response
{
    private const string ServerName = "RX.WEB";
    private readonly Request _request;
    private readonly HttpStatusCode _statusCode;
    private readonly string _body;

    public Response(Request request, HttpStatusCode statusCode, string body)
    {
        _request = request;
        _statusCode = statusCode;
        _body = body;
    }

    public async Task<Response> SendToClientAsync(CancellationToken cancellationToken)
    {
        await using var stream = new StreamWriter(_request.Client.GetStream());
        await stream.WriteAsync(GetData(), cancellationToken);
        return this;
    }

    public void Close()
    {
        _request.Client.Close();
    }

    private StringBuilder GetData()
    {
        var sb = new StringBuilder();
        sb
            .AppendFormat("HTTP/1.0 {0} {1} \n", (int)_statusCode, _statusCode.ToString())
            .AppendFormat("Server: {0}\n", ServerName)
            .AppendLine("Content-Type: text/plain")
            .AppendFormat("Content-Length: {0}\n", _body.Length)
            .AppendLine()
            .Append(_body);
        return sb;
    }
}