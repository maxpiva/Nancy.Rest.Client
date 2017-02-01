using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Nancy.Rest.Client.Exceptions;
using Newtonsoft.Json;

namespace Nancy.Rest.Client.Rest
{
    public class SmallWebClient
    {

        public static async Task<object> RestRequest(Request req)
        {
            using (var client = new HttpClient())
            {
                bool returnasstream = false;
                string accept = "application/json";
                if (req.ReturnType.IsAssignableFrom(typeof(Stream)))
                {
                    returnasstream = true;
                    accept = "*/*";
                }
                client.BaseAddress = req.BaseUri;
                client.Timeout = req.Timeout;
                HttpRequestMessage request = new HttpRequestMessage(req.Method, req.Path);
                request.Content.Headers.Add("Accept", accept);
                if (req.BodyObject != null)
                {
                    if (req.BodyObject.GetType().IsAssignableFrom(typeof(Stream)))
                    {
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                        request.Content= new StreamContent((Stream)req.BodyObject);
                    }
                    else
                    {
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                        request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req.BodyObject, Formatting.None)));
                    }
                }
                HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    response.Content?.Dispose();
                    throw new RestClientException(response.StatusCode, response.ReasonPhrase, content);
                }
                if (returnasstream)
                    return await response.Content.ReadAsStreamAsync();
                return JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), req.ReturnType, req.SerializerSettings);
            }
        }
    }
}
