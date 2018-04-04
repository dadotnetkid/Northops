using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace NorthOps.Ops.ApiRepository
{
    public class ApiGenericRepository
    {
        HttpClient client;
        string BaseUrl = "http://localhost:65425/";
        public ApiGenericRepository()
        {
            client = new HttpClient();

            if (HttpContext.Current.Request.Cookies.Get(".AspNetAuthorizationToken") != null)
            {
                var token = HttpContext.Current.Request.Cookies.Get(".AspNetAuthorizationToken");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            }

        }
        public async Task<IdentityResult> Login(string UserName, string Password)
        {
            var keyvalues = new List<KeyValuePair<string, string>>
          {
              new KeyValuePair<string, string>("username",UserName),
              new KeyValuePair<string, string>("password",Password),
              new KeyValuePair<string, string>("grant_type","password")
          };
            var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "token");

            request.Content = new FormUrlEncodedContent(keyvalues);
            var jsonObject = await client.SendAsync(request);

            if (jsonObject.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var Token = JsonConvert.DeserializeObject<TokenResponse>(await jsonObject.Content.ReadAsStringAsync());
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(".AspNetAuthorizationToken", Token.AccessToken) { Expires = Token.ValidTo });
               /* HttpContext.Current.Response.Cookies.Add(new HttpCookie("userId", Token.UserName) { Expires = Token.ValidTo });
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("validFrom", Token.ValidFrom.ToString()) { Expires = Token.ValidTo });
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("ValidTo", Token.ValidTo.ToString()) { Expires = Token.ValidTo });*/
                HttpContext.Current.Response.Headers.Add("Authorization", $"Bearer {Token.AccessToken}");
                HttpContext.Current.Request.Headers.Add("Authorization", $"Bearer {Token.AccessToken}");
                var ctx = HttpContext.Current.Request.GetOwinContext();
                var authenticateResult = await ctx.Authentication.AuthenticateAsync(DefaultAuthenticationTypes.ExternalBearer);
                var applicationCookieIdentity = new ClaimsIdentity(authenticateResult.Identity.Claims, DefaultAuthenticationTypes.ApplicationCookie);
                ctx.Authentication.SignIn(applicationCookieIdentity);
                return IdentityResult.Success;
            }
            else
                return IdentityResult.Failed(jsonObject.StatusCode.ToString());

        }


        public async Task<T> GetFetchAsync<T>(string EndPoint)
        {
            var jsonObject = await client.GetAsync(BaseUrl + EndPoint);
            return JsonConvert.DeserializeObject<T>(await jsonObject.Content.ReadAsStringAsync());
        }
        public T GetFetch<T>(string EndPoint)
        {
            var jsonObject = client.GetAsync(BaseUrl + EndPoint).Result;
            return JsonConvert.DeserializeObject<T>(jsonObject.Content.ReadAsStringAsync().Result);
        }

        public T PostFetch<T>(string EndPoint, object model)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var jsonObject = client.PostAsync(BaseUrl + EndPoint, content).Result;
            return JsonConvert.DeserializeObject<T>(jsonObject.Content.ReadAsStringAsync().Result);
        }
        public async Task<T> PostFetchAsync<T>(string EndPoint, object model)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var jsonObject = await client.PostAsync(BaseUrl + EndPoint, content);
            return JsonConvert.DeserializeObject<T>(await jsonObject.Content.ReadAsStringAsync());
        }

        public async Task<HttpStatusCode> Update(string EndPoint, object model)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage jsonObject = await client.PostAsync(BaseUrl + EndPoint, content);
            return jsonObject.StatusCode;
        }
        public async Task<HttpStatusCode> DeleteAsync(string EndPoint)
        {
            HttpResponseMessage jsonObject = await client.GetAsync(BaseUrl + EndPoint);
            return jsonObject.StatusCode;
        }
        public async Task<HttpStatusCode> DeleteAsync(string EndPoint, object model)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage jsonObject = await client.PostAsync(BaseUrl + EndPoint,content);
            return jsonObject.StatusCode;
        }
        public HttpStatusCode Delete(string EndPoint, object model)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage jsonObject = client.PostAsync(BaseUrl + EndPoint, content).Result;
            return jsonObject.StatusCode;
        }
        public HttpStatusCode Delete(string EndPoint)
        {
            HttpResponseMessage jsonObject = client.GetAsync(BaseUrl + EndPoint).Result;
            return jsonObject.StatusCode;
        }

        public async Task<HttpStatusCode> InsertAsync(string EndPoint, object model)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage jsonObject = await client.PostAsync(BaseUrl + EndPoint, content);
            return jsonObject.StatusCode;
        }
        public  HttpStatusCode Insert(string EndPoint, object model)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage jsonObject = client.PostAsync(BaseUrl + EndPoint, content).Result;
            return jsonObject.StatusCode;
        }
    }

    internal class LoginViewModel
    {
        public LoginViewModel()
        {
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Grant_Type { get; set; }
    }
    internal class TokenResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = ".issued")]
        public DateTime ValidFrom { get; set; }
        [JsonProperty(PropertyName = ".expires")]
        public DateTime ValidTo { get; set; }
    }
}