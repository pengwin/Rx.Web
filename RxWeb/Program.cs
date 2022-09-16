using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using RxWeb;

var cts = new CancellationTokenSource();

IScheduler serverScheduler = new TaskPoolScheduler(new TaskFactory(cts.Token));
IScheduler clientScheduler = new TaskPoolScheduler(new TaskFactory(cts.Token));

Observable.Using(() => new Server("127.0.0.1", 5000), server => server.ToObservable(serverScheduler, cts.Token))
    .SelectMany(client => client.ToObservable(clientScheduler))
    .Select(request => request switch
    {
        (Request.HttpMethod.GET, "/") => new Response(request, HttpStatusCode.OK, "OK"),
        (Request.HttpMethod.GET, "/test") => new Response(request, HttpStatusCode.OK, "Test"),
        (Request.HttpMethod.GET, _) => new Response(request, HttpStatusCode.NotFound, "Not found"),
        var (_,  path) when !string.IsNullOrEmpty(path) => new Response(request, HttpStatusCode.MethodNotAllowed, "Not Allowed"),
        _ => new Response(request, HttpStatusCode.InternalServerError, "Unknown state"),
    })
    .SelectMany(response => Observable.FromAsync(response.SendToClientAsync, clientScheduler))
    .Subscribe(response => { response.Close(); });
;

Console.WriteLine("\nHit enter to continue...");
Console.Read();

internal static class RxWebExtensions
{
    public static IObservable<TcpClient> ToObservable(this Server server, IScheduler scheduler,
        CancellationToken cancellationToken)
    {
        return Observable.While(() => !cancellationToken.IsCancellationRequested,
            Observable.FromAsync(server.AcceptClientAsync, scheduler));
    }

    public static IObservable<Request> ToObservable(this TcpClient client, IScheduler scheduler)
    {
        return Observable.Using(
                () => new StreamReader(client.GetStream()),
                reader => reader.ToObservable(scheduler))
            .SplitHeaders()
            .Where(kv => kv.HasValue)
            .Select(kv => kv!.Value)
            .Aggregate(new Request(client), (request, kv) =>
            {
                Console.WriteLine("{0} : {1}", kv.Key, kv.Value);
                if (kv.Key != null)
                {
                    request.AddHeader(kv.Key, kv.Value);
                }
                else
                {
                    var regExp = new Regex("(?<method>.*) (?<path>.*) HTTP/(?<version>.*)");
                    var res = regExp.Match(kv.Value);
                    if (res.Success)
                    {
                        request.AddHeader("HttpMethod", res.Groups["method"].Value);
                        request.AddHeader("HttpPath", res.Groups["path"].Value);
                        request.AddHeader("HttpVersion", res.Groups["version"].Value);
                    }
                }

                return request;
            });
    }

    private static IObservable<string?> ToObservable(this StreamReader reader, IScheduler scheduler)
    {
        return Observable.FromAsync(reader.ReadLineAsync, scheduler)
            .Repeat()
            .TakeWhile(line => !string.IsNullOrEmpty(line));
    }

    private static IObservable<KeyValuePair<string?, string>?> SplitHeaders(this IObservable<string?> lines)
    {
        return lines.Select(line => line?.ToHttpKv());
    }

    private static KeyValuePair<string?, string>? ToHttpKv(this string? line) => (line, line?.IndexOf(':')) switch
    {
        (null, _) => null,
        (var l, -1) => new KeyValuePair<string?, string>(null, l),
        var (l, pos) => new KeyValuePair<string?, string>(l[..pos.Value].Trim(), l[(pos.Value + 1)..].Trim()),
    };
}