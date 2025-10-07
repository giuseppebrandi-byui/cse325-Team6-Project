using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MyMuscleCars.Services
{
    public static class JsInteropHelpers
    {
        public class BrowserPostResult
        {
            public int Status { get; set; }
            public string? Body { get; set; }
        }

        public static async Task<BrowserPostResult?> PostJson(IJSRuntime js, string url, object data)
        {
            try
            {
                return await js.InvokeAsync<BrowserPostResult>("postJson", url, data);
            }
            catch (JSException)
            {
                return null;
            }
        }

        public static async Task<BrowserPostResult?> GetJson(IJSRuntime js, string url)
        {
            try
            {
                return await js.InvokeAsync<BrowserPostResult>("getJson", url);
            }
            catch (JSException)
            {
                return null;
            }
        }
    }
}
