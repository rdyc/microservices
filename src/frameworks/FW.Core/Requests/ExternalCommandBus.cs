using FW.Core.Commands;
using RestSharp;
using RestSharp.Serializers;

namespace FW.Core.Requests;

public interface IExternalCommandBus
{
    Task Post<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T : ICommand;
    Task Put<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T : ICommand;
    Task Delete<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T : ICommand;
}

public class ExternalCommandBus : IExternalCommandBus
{
    public async Task Post<T>(string url, string path, T command, CancellationToken cancellationToken) where T : ICommand
    {
        var client = new RestClient(url);
        var request = new RestRequest(path, Method.Post);
        request.AddBody(command, ContentType.Json);

        await client.PostAsync<dynamic>(request, cancellationToken);
    }

    public async Task Put<T>(string url, string path, T command, CancellationToken cancellationToken) where T : ICommand
    {
        var client = new RestClient(url);
        var request = new RestRequest(path, Method.Put);
        request.AddBody(command, ContentType.Json);

        await client.PutAsync<dynamic>(request, cancellationToken);
    }

    public async Task Delete<T>(string url, string path, T command, CancellationToken cancellationToken) where T : ICommand
    {
        var client = new RestClient(url);
        var request = new RestRequest(path, Method.Delete);
        request.AddBody(command, ContentType.Json);

        await client.DeleteAsync<dynamic>(request, cancellationToken);
    }
}