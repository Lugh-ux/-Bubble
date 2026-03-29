using BubbleApp.Modelos;
using BubbleApp.Estado;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BubbleApp.Servicios
{
    public class ApiCliente
    {
        private static readonly HttpClient _httpClient = CreateHttpClient();
        private readonly JavaScriptSerializer _serializer;

        private static HttpClient CreateHttpClient()
        {
            var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://127.0.0.1:8000/api/";
            return new HttpClient
            {
                BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/")
            };
        }

        public ApiCliente()
        {
            _serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
        }

        public async Task<SesionUsuario> LoginAsync(string email, string password)
        {
            var payload = new Dictionary<string, object>
            {
                ["email"] = email,
                ["password"] = password
            };

            var response = await _httpClient.PostAsync("desktop/login", CreateJsonContent(payload));
            return await ReadSessionAsync(response);
        }

        public async Task<SesionUsuario> RegisterAsync(string name, string email, string password)
        {
            var payload = new Dictionary<string, object>
            {
                ["name"] = name,
                ["email"] = email,
                ["password"] = password
            };

            var response = await _httpClient.PostAsync("desktop/register", CreateJsonContent(payload));
            return await ReadSessionAsync(response);
        }

        public async Task LogoutAsync()
        {
            using (var request = CreateAuthenticatedRequest(HttpMethod.Post, "desktop/logout"))
            {
                var response = await _httpClient.SendAsync(request);
                await EnsureSuccessAsync(response);
            }
        }

        public async Task<ModeloPanelPrincipal> GetDashboardAsync(double latitude, double longitude)
        {
            var url = string.Format(CultureInfo.InvariantCulture, "desktop/dashboard?lat={0}&lng={1}", latitude, longitude);

            using (var request = CreateAuthenticatedRequest(HttpMethod.Get, url))
            {
                var response = await _httpClient.SendAsync(request);
                var data = await ReadJsonAsync(response);

                System.Diagnostics.Debug.WriteLine($"[API] Dashboard response keys: {string.Join(", ", data.Keys)}");

                var dashboard = new ModeloPanelPrincipal
                {
                    CurrentUser = ParseUser(GetDictionary(data, "current_user")),
                    MyBubbleActive = GetBool(data, "my_bubble_active")
                };

                System.Diagnostics.Debug.WriteLine($"[API] MyBubbleActive: {dashboard.MyBubbleActive}");

                var myBubbleData = GetDictionary(data, "my_bubble");
                if (myBubbleData.Count > 0)
                {
                    dashboard.MyBubble = ParseBubble(myBubbleData);
                    System.Diagnostics.Debug.WriteLine($"[API] MyBubble encontrado");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[API] MyBubble es NULL o vacio");
                }

                var bubblesList = GetList(data, "bubbles");
                System.Diagnostics.Debug.WriteLine($"[API] Burbujas en response: {bubblesList.Count}");

                foreach (Dictionary<string, object> item in bubblesList)
                {
                    var bubble = ParseBubble(item);
                    dashboard.Bubbles.Add(bubble);
                }

                System.Diagnostics.Debug.WriteLine($"[API] Total burbujas en dashboard: {dashboard.Bubbles.Count}");
                return dashboard;
            }
        }

        public async Task<ModeloPerfilUsuario> GetProfileAsync()
        {
            using (var request = CreateAuthenticatedRequest(HttpMethod.Get, "desktop/profile"))
            {
                var response = await _httpClient.SendAsync(request);
                var data = await ReadJsonAsync(response);
                var stats = GetDictionary(data, "stats");

                return new ModeloPerfilUsuario
                {
                    User = ParseUser(GetDictionary(data, "user")),
                    BubbleCount = GetInt(stats, "bubble_count")
                };
            }
        }

        public async Task<ModeloUsuario> UpdateProfileAsync(string name, string avatarPath)
        {
            using (var request = CreateAuthenticatedRequest(HttpMethod.Post, "desktop/profile"))
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(name, Encoding.UTF8), "name");

                if (!string.IsNullOrWhiteSpace(avatarPath) && File.Exists(avatarPath))
                {
                    var bytes = File.ReadAllBytes(avatarPath);
                    var fileContent = new ByteArrayContent(bytes);
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                    formData.Add(fileContent, "avatar", Path.GetFileName(avatarPath));
                }

                request.Content = formData;

                var response = await _httpClient.SendAsync(request);
                var data = await ReadJsonAsync(response);
                return ParseUser(GetDictionary(data, "user"));
            }
        }

        public async Task ActivateBubbleAsync(double latitude, double longitude)
        {
            var payload = new Dictionary<string, object>
            {
                ["lat"] = latitude,
                ["lng"] = longitude
            };

            using (var request = CreateAuthenticatedRequest(HttpMethod.Post, "desktop/bubble"))
            {
                request.Content = CreateJsonContent(payload);
                var response = await _httpClient.SendAsync(request);
                await EnsureSuccessAsync(response);
            }
        }

        public async Task DeleteBubbleAsync()
        {
            using (var request = CreateAuthenticatedRequest(HttpMethod.Delete, "desktop/bubble"))
            {
                var response = await _httpClient.SendAsync(request);
                await EnsureSuccessAsync(response);
            }
        }

        private HttpRequestMessage CreateAuthenticatedRequest(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);

            if (SesionActual.IsAuthenticated)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SesionActual.Current.Token);
            }

            return request;
        }

        private StringContent CreateJsonContent(Dictionary<string, object> payload)
        {
            return new StringContent(_serializer.Serialize(payload), Encoding.UTF8, "application/json");
        }

        private async Task<SesionUsuario> ReadSessionAsync(HttpResponseMessage response)
        {
            var data = await ReadJsonAsync(response);

            return new SesionUsuario
            {
                Token = GetString(data, "token"),
                User = ParseUser(GetDictionary(data, "user"))
            };
        }

        private async Task<Dictionary<string, object>> ReadJsonAsync(HttpResponseMessage response)
        {
            await EnsureSuccessAsync(response);
            var content = await response.Content.ReadAsStringAsync();
            return _serializer.Deserialize<Dictionary<string, object>>(content);
        }

        private async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var json = _serializer.Deserialize<Dictionary<string, object>>(content);
                throw new InvalidOperationException(GetString(json, "message") ?? "Error en la API.");
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(content) ? "Error en la API." : content);
            }
        }

        private ModeloUsuario ParseUser(Dictionary<string, object> data)
        {
            return new ModeloUsuario
            {
                Id = GetInt(data, "id"),
                Name = GetString(data, "name"),
                Email = GetString(data, "email"),
                AvatarUrl = GetString(data, "avatar_url")
            };
        }

        private ModeloBurbuja ParseBubble(Dictionary<string, object> data)
        {
            return new ModeloBurbuja
            {
                Id = GetInt(data, "id"),
                Message = GetString(data, "message"),
                Latitude = GetDouble(data, "latitude"),
                Longitude = GetDouble(data, "longitude"),
                DistanceKm = GetDouble(data, "distance_km"),
                DistanceLabel = GetString(data, "distance_label"),
                IsCurrentUser = GetBool(data, "is_current_user"),
                User = ParseUser(GetDictionary(data, "user"))
            };
        }

        private static Dictionary<string, object> GetDictionary(Dictionary<string, object> data, string key)
        {
            if (!data.ContainsKey(key) || data[key] == null)
            {
                return new Dictionary<string, object>();
            }

            return data[key] as Dictionary<string, object> ?? new Dictionary<string, object>();
        }

        private static List<Dictionary<string, object>> GetList(Dictionary<string, object> data, string key)
        {
            var list = new List<Dictionary<string, object>>();

            if (!data.ContainsKey(key) || data[key] == null)
            {
                return list;
            }

            var rawList = data[key] as ArrayList;
            if (rawList == null)
            {
                return list;
            }

            foreach (var item in rawList)
            {
                if (item is Dictionary<string, object> dict)
                {
                    list.Add(dict);
                }
            }

            return list;
        }

        private static string GetString(Dictionary<string, object> data, string key)
        {
            return data.ContainsKey(key) && data[key] != null ? Convert.ToString(data[key]) : string.Empty;
        }

        private static int GetInt(Dictionary<string, object> data, string key)
        {
            return data.ContainsKey(key) && data[key] != null ? Convert.ToInt32(data[key]) : 0;
        }

        private static bool GetBool(Dictionary<string, object> data, string key)
        {
            return data.ContainsKey(key) && data[key] != null && Convert.ToBoolean(data[key]);
        }

        private static double GetDouble(Dictionary<string, object> data, string key)
        {
            return data.ContainsKey(key) && data[key] != null
                ? Convert.ToDouble(data[key], CultureInfo.InvariantCulture)
                : 0d;
        }
    }
}
