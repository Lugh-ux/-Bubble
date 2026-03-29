using BubbleApp.Modelos;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace BubbleApp.Controles
{
    public class ControlMapaRadar : UserControl
    {
        private readonly Panel _panelSuperior;
        private readonly Label _tituloLabel;
        private readonly Label _infoLabel;
        private readonly WebView2 _mapa;
        private readonly JavaScriptSerializer _serializer;
        private readonly string _googleMapsApiKey;
        private bool _mapaInicializado;
        private bool _initializationStarted;
        private bool _isNavigating;
        private bool _jsMapReady;
        private int _renderGeneration;
        private double _lastLatitude = 42.2406;
        private double _lastLongitude = -8.7261;
        private List<ModeloBurbuja> _lastBubbles = new List<ModeloBurbuja>();
        private bool _fullRenderPending = false;
        
        public ControlMapaRadar()
        {
            _serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            BackColor = Color.FromArgb(14, 17, 24);
            _googleMapsApiKey = ConfigurationManager.AppSettings["GoogleMapsApiKey"] ?? string.Empty;

            _panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = Color.FromArgb(14, 17, 24)
            };

            _tituloLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(18, 14),
                Text = "Mapa de proximidad"
            };

            _infoLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(190, 255, 255, 255),
                Location = new Point(20, 42),
                Size = new Size(620, 20),
                Text = "Cargando mapa..."
            };

            _mapa = new WebView2
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(23, 27, 36)
            };

            _panelSuperior.Controls.Add(_tituloLabel);
            _panelSuperior.Controls.Add(_infoLabel);

            Controls.Add(_mapa);
            Controls.Add(_panelSuperior);

            // WebView2 necesita que el control tenga handle; inicializamos en HandleCreated.
            HandleCreated += (s, e) => StartInitializeMap();
            if (IsHandleCreated)
            {
                StartInitializeMap();
            }
        }

        public void UpdateData(double latitude, double longitude, IEnumerable<ModeloBurbuja> bubbles)
        {
            var newBubbles = bubbles?.ToList() ?? new List<ModeloBurbuja>();
            
            System.Diagnostics.Debug.WriteLine($"[ControlMapaRadar] UpdateData - Bubbles: {newBubbles.Count}");
            
            _infoLabel.Text = string.Format(
                CultureInfo.InvariantCulture,
                "Lat {0:F4} | Lng {1:F4} | {2} burbujas cerca",
                latitude,
                longitude,
                newBubbles.Count);

            if (!_mapaInicializado)
            {
                // Persistimos el estado aunque WebView2 aun no haya terminado de inicializarse,
                // para que el primer render use los datos correctos.
                _lastLatitude = latitude;
                _lastLongitude = longitude;
                _lastBubbles = newBubbles;
                _fullRenderPending = true;
                return;
            }

            var prevLat = _lastLatitude;
            var prevLng = _lastLongitude;
            var prevCount = _lastBubbles.Count;

            _lastLatitude = latitude;
            _lastLongitude = longitude;
            _lastBubbles = newBubbles;

            bool locationChanged = Math.Abs(prevLat - latitude) > 0.0001 || 
                                   Math.Abs(prevLng - longitude) > 0.0001;
            bool bubblesCountChanged = prevCount != newBubbles.Count;

            if (locationChanged || bubblesCountChanged)
            {
                _fullRenderPending = true;
                if (_jsMapReady && !_isNavigating)
                {
                    ReplaceMapDataInPlaceAsync();
                }
                else
                {
                    RenderMap();
                }
            }
            else
            {
                UpdateBubblesPositionsAsync(newBubbles);
            }
        }

        private async void ReplaceMapDataInPlaceAsync()
        {
            if (!_mapaInicializado || _mapa.CoreWebView2 == null || _isNavigating || !_jsMapReady)
            {
                return;
            }

            try
            {
                var bubbles = _lastBubbles.Select(bubble => new
                {
                    id = bubble.Id,
                    lat = bubble.Latitude,
                    lng = bubble.Longitude,
                    nombre = bubble.IsCurrentUser ? "Tu burbuja activa" : (bubble.User?.Name ?? "Usuario"),
                    mensaje = bubble.Message ?? string.Empty,
                    distancia = bubble.DistanceLabel ?? string.Empty,
                    propia = bubble.IsCurrentUser
                }).ToList();

                var bubblesJson = _serializer.Serialize(bubbles);
                var script = string.Format(
                    CultureInfo.InvariantCulture,
                    "if (typeof replaceMapData === 'function') {{ replaceMapData({{ lat: {0}, lng: {1} }}, {2}); }}",
                    _lastLatitude,
                    _lastLongitude,
                    bubblesJson);

                await _mapa.ExecuteScriptAsync(script);
                _fullRenderPending = false;
            }
            catch
            {
                // Si falla el script, volvemos al render completo.
                _fullRenderPending = true;
                RenderMap();
            }
        }

        public void InjectTestBubbles()
        {
            var testBubbles = new List<ModeloBurbuja>
            {
                new ModeloBurbuja
                {
                    Id = 1,
                    Message = "Burbuja de prueba 1",
                    Latitude = 42.2406,
                    Longitude = -8.7261,
                    DistanceLabel = "0 m",
                    IsCurrentUser = true,
                    User = new ModeloUsuario { Name = "Tu burbuja", Id = 1, Email = "test@test.com" }
                },
                new ModeloBurbuja
                {
                    Id = 2,
                    Message = "Burbuja de prueba 2",
                    Latitude = 42.2420,
                    Longitude = -8.7250,
                    DistanceLabel = "200 m",
                    IsCurrentUser = false,
                    User = new ModeloUsuario { Name = "Otro usuario", Id = 2, Email = "otro@test.com" }
                },
                new ModeloBurbuja
                {
                    Id = 3,
                    Message = "Burbuja de prueba 3",
                    Latitude = 42.2390,
                    Longitude = -8.7280,
                    DistanceLabel = "350 m",
                    IsCurrentUser = false,
                    User = new ModeloUsuario { Name = "Tercer usuario", Id = 3, Email = "tercero@test.com" }
                }
            };

            UpdateData(_lastLatitude, _lastLongitude, testBubbles);
        }

        public async Task FocusBubbleAsync(int bubbleId)
        {
            if (!_mapaInicializado || _mapa.CoreWebView2 == null || _isNavigating)
            {
                return;
            }

            try
            {
                await _mapa.ExecuteScriptAsync($"if (typeof focusBubble === 'function') focusBubble({bubbleId});");
            }
            catch
            {
            }
        }

        private void StartInitializeMap()
        {
            if (_initializationStarted)
            {
                return;
            }

            _initializationStarted = true;
            InitializeMapAsync();
        }

        private async void InitializeMapAsync()
        {
            if (string.IsNullOrWhiteSpace(_googleMapsApiKey))
            {
                _infoLabel.Text = "Falta la clave de Google Maps en App.config.";
                return;
            }

            try
            {
                _infoLabel.Text = "Inicializando WebView2...";
                // Workaround: en algunos equipos WebView2 puede "cargar" pero renderizar en blanco por GPU/driver.
                // Forzamos software rendering para este control.
                var envOptions = new CoreWebView2EnvironmentOptions("--disable-gpu");
                var env = await CoreWebView2Environment.CreateAsync(null, null, envOptions);
                await _mapa.EnsureCoreWebView2Async(env);
                _mapa.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                _mapa.CoreWebView2.Settings.AreDevToolsEnabled = true;
                _mapa.CoreWebView2.Settings.IsStatusBarEnabled = false;

                // Reenvia console.log/warn/error y errores globales JS a C# via postMessage,
                // porque en esta version no existe CoreWebView2.ConsoleMessageReceived.
                _mapa.CoreWebView2.Settings.IsWebMessageEnabled = true;
                _mapa.CoreWebView2.WebMessageReceived += (s, e) =>
                {
                    try
                    {
                        var msg = e.TryGetWebMessageAsString() ?? string.Empty;
                        System.Diagnostics.Debug.WriteLine($"[WebView2][msg] {msg}");

                        if (!string.IsNullOrWhiteSpace(msg) && msg.TrimStart().StartsWith("{", StringComparison.Ordinal))
                        {
                            var dict = _serializer.Deserialize<Dictionary<string, object>>(msg);
                            if (dict != null && dict.TryGetValue("type", out var typeObj) && (typeObj as string) == "console")
                            {
                                var level = dict.TryGetValue("level", out var levelObj) ? (levelObj as string) : null;
                                var message = dict.TryGetValue("message", out var messageObj) ? (messageObj as string) : null;

                                if (!string.IsNullOrWhiteSpace(message))
                                {
                                    if (message.IndexOf("gm_authFailure", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                        message.IndexOf("API key no es valida", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        BeginInvoke(new Action(() =>
                                        {
                                            _infoLabel.Text = "Google Maps no autorizado (API key/restricciones).";
                                        }));
                                    }
                                    else if (message.IndexOf("ERROR cargando Google Maps JS", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                             message.IndexOf("maps.googleapis.com", StringComparison.OrdinalIgnoreCase) >= 0 && (level == "error" || level == "onerror"))
                                    {
                                        BeginInvoke(new Action(() =>
                                        {
                                            _infoLabel.Text = "No se pudo cargar Google Maps (internet/bloqueo).";
                                        }));
                                    }
                                    else if (message.IndexOf("MAPA LISTO", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        BeginInvoke(new Action(() =>
                                        {
                                            _infoLabel.Text = "Mapa cargado.";
                                            _jsMapReady = true;
                                        }));
                                    }
                                    else if (message.IndexOf("tilesloaded", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        BeginInvoke(new Action(() =>
                                        {
                                            _infoLabel.Text = "Mapa cargado.";
                                            _jsMapReady = true;
                                        }));
                                    }
                                    else if (message.IndexOf("Mapa creado", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        BeginInvoke(new Action(() =>
                                        {
                                            _infoLabel.Text = "Cargando mapa...";
                                        }));
                                    }
                                    else if (message.IndexOf("JS ERROR:", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                             message.IndexOf("PROMISE ERROR:", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        var shortMsg = message.Length > 120 ? message.Substring(0, 120) : message;
                                        BeginInvoke(new Action(() =>
                                        {
                                            _infoLabel.Text = shortMsg;
                                        }));
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                };

                await _mapa.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"
(() => {
  function safeStringify(v) {
    try { return typeof v === 'string' ? v : JSON.stringify(v); } catch { return String(v); }
  }
  function post(level, args) {
    try {
      if (window.chrome && window.chrome.webview && typeof window.chrome.webview.postMessage === 'function') {
        const message = Array.prototype.slice.call(args).map(safeStringify).join(' ');
        window.chrome.webview.postMessage(JSON.stringify({ type: 'console', level, message }));
      }
    } catch { }
  }
  const orig = { log: console.log, warn: console.warn, error: console.error };
  console.log = function() { post('log', arguments); orig.log.apply(console, arguments); };
  console.warn = function() { post('warn', arguments); orig.warn.apply(console, arguments); };
  console.error = function() { post('error', arguments); orig.error.apply(console, arguments); };
  window.addEventListener('error', (ev) => post('onerror', [ev.message, ev.filename, ev.lineno, ev.colno]));
  window.addEventListener('unhandledrejection', (ev) => post('unhandledrejection', [safeStringify(ev.reason)]));
})();");

                _mapa.CoreWebView2.NavigationCompleted += (s, e) =>
                {
                    if (!e.IsSuccess)
                    {
                        _infoLabel.Text = "Error cargando el mapa (WebView2): " + e.WebErrorStatus;
                    }
                };

                _mapa.CoreWebView2.ProcessFailed += (s, e) =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        _infoLabel.Text = "WebView2 se ha reiniciado. Recargando mapa...";
                        try
                        {
                            _isNavigating = false;
                            _jsMapReady = false;
                            _fullRenderPending = true;
                            RenderMap();
                        }
                        catch
                        {
                            // Si Reload falla, forzamos un render completo.
                            _fullRenderPending = true;
                            RenderMap();
                        }
                    }));
                };

                // Evita romper el WebView2: no navegamos de nuevo mientras una navegacion este en curso.
                _mapa.NavigationStarting += (s, e) => { _isNavigating = true; _jsMapReady = false; };
                _mapa.NavigationCompleted += (s, e) =>
                {
                    _isNavigating = false;
                    if (_fullRenderPending)
                    {
                        _fullRenderPending = false;
                        // Si llegaron cambios durante la navegacion, rendereamos al terminar.
                        BeginInvoke(new Action(RenderMap));
                    }
                };

                _mapaInicializado = true;

                _infoLabel.Text = "Cargando mapa...";
                await Task.Delay(50);

                // Si llegaron datos antes de inicializar, renderizamos usando el ultimo estado.
                if (_fullRenderPending)
                {
                    RenderMap();
                    _fullRenderPending = false;
                }
                else
                {
                    RenderMap();
                }
            }
            catch (WebView2RuntimeNotFoundException ex)
            {
                _infoLabel.Text = "WebView2 Runtime no esta instalado: " + ex.Message;
            }
            catch (Exception ex)
            {
                _infoLabel.Text = "No se pudo iniciar el mapa: " + ex.Message;
            }
        }

        private void RenderMap()
        {
            if (!_mapaInicializado || _mapa.CoreWebView2 == null)
            {
                _infoLabel.Text = "Mapa no inicializado";
                return;
            }

            if (_isNavigating)
            {
                _fullRenderPending = true;
                return;
            }

            try
            {
                _jsMapReady = false;
                _infoLabel.Text = "Cargando mapa...";
                var myGeneration = ++_renderGeneration;

                var bubbles = _lastBubbles.Select(bubble => new
                {
                    id = bubble.Id,
                    lat = bubble.Latitude,
                    lng = bubble.Longitude,
                    nombre = bubble.IsCurrentUser ? "Tu burbuja activa" : (bubble.User?.Name ?? "Usuario"),
                    mensaje = bubble.Message ?? string.Empty,
                    distancia = bubble.DistanceLabel ?? string.Empty,
                    propia = bubble.IsCurrentUser
                }).ToList();

                var bubblesJson = _serializer.Serialize(bubbles);
                System.Diagnostics.Debug.WriteLine($"[ControlMapaRadar] RenderMap - Bubbles: {bubbles.Count}, JSON length: {bubblesJson.Length}");

                var html = BuildHtml(_lastLatitude, _lastLongitude, bubblesJson);
                _mapa.NavigateToString(html);

                // Watchdog: si el JS no llega a "MAPA LISTO", reintentamos una vez.
                _ = Task.Run(async () =>
                {
                    await Task.Delay(8000).ConfigureAwait(false);
                    if (myGeneration != _renderGeneration)
                    {
                        return;
                    }

                    if (_mapaInicializado && !_isNavigating && !_jsMapReady)
                    {
                        try
                        {
                            BeginInvoke(new Action(() =>
                            {
                                if (myGeneration != _renderGeneration)
                                {
                                    return;
                                }

                                _infoLabel.Text = "Reintentando cargar el mapa...";
                                _fullRenderPending = true;
                                RenderMap();
                            }));
                        }
                        catch
                        {
                        }
                    }
                });

                // Diagnostico: si a los 2-4s no hay google.maps o el contenedor sigue a 0, lo mostramos en UI.
                _ = Task.Run(async () =>
                {
                    await Task.Delay(2500).ConfigureAwait(false);
                    if (myGeneration != _renderGeneration || !_mapaInicializado || _mapa.CoreWebView2 == null)
                    {
                        return;
                    }

                    try
                    {
                        var script = @"
(() => {
  const el = document.getElementById('map');
  const size = el ? (el.offsetWidth + 'x' + el.offsetHeight) : 'no-map';
  const hasGoogle = (typeof google !== 'undefined');
  const hasMaps = (hasGoogle && !!google.maps);
  const ready = (typeof mapStarted !== 'undefined') ? mapStarted : null;
  return JSON.stringify({ size, hasGoogle, hasMaps, mapStarted: ready, readyState: document.readyState });
})();";

                        var raw = await _mapa.ExecuteScriptAsync(script).ConfigureAwait(false);
                        // raw viene como string JSON-quoted desde WebView2.
                        var cleaned = (raw ?? string.Empty).Trim().Trim('"').Replace("\\\"", "\"");
                        if (string.IsNullOrWhiteSpace(cleaned))
                        {
                            return;
                        }

                        BeginInvoke(new Action(() =>
                        {
                            if (myGeneration != _renderGeneration)
                            {
                                return;
                            }

                            if (!_jsMapReady)
                            {
                                _infoLabel.Text = "Diagnostico mapa: " + cleaned;
                            }
                        }));
                    }
                    catch
                    {
                    }
                });
            }
            catch (Exception ex)
            {
                _infoLabel.Text = "Error al renderizar mapa: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"[ControlMapaRadar] Error en RenderMap: {ex.Message}");
            }
        }

        private async void UpdateBubblesPositionsAsync(List<ModeloBurbuja> newBubbles)
        {
            if (!_mapaInicializado || _mapa.CoreWebView2 == null)
            {
                return;
            }

            if (_isNavigating)
            {
                return;
            }

            var bubblesJson = _serializer.Serialize(newBubbles.Select(bubble => new
            {
                id = bubble.Id,
                lat = bubble.Latitude,
                lng = bubble.Longitude,
                nombre = bubble.IsCurrentUser ? "Tu burbuja activa" : (bubble.User?.Name ?? "Usuario"),
                mensaje = bubble.Message ?? string.Empty,
                distancia = bubble.DistanceLabel ?? string.Empty,
                propia = bubble.IsCurrentUser
            }).ToList());

            var script = $@"
                if (typeof updateBubblePositions === 'function') {{
                    updateBubblePositions({bubblesJson});
                }}
            ";

            try
            {
                await _mapa.ExecuteScriptAsync(script);
            }
            catch
            {
                _fullRenderPending = true;
            }
        }

        private string BuildHtml(double latitude, double longitude, string bubblesJson)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset=\"utf-8\" />");
            html.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />");
            html.AppendLine("<style>");
            html.AppendLine("* { margin: 0; padding: 0; box-sizing: border-box; }");
            html.AppendLine("html, body { height: 100%; width: 100%; }");
            html.AppendLine("#map { height: 100%; width: 100%; background: #171b24; }");
            html.AppendLine(".gm-style-iw { font-family: Segoe UI, sans-serif; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<div id=\"map\"></div>");
            html.AppendLine("<script>");
            html.AppendLine("let map;");
            html.AppendLine("let markers = {};");
            html.AppendLine("let circles = {};");
            html.AppendLine("let infoWindows = {};");
            html.AppendLine("let bounds;");
            html.AppendLine("let userMarker;");
            html.AppendLine("");
            html.AppendLine("function debugLog(msg) {");
            html.AppendLine("  try { console.log(msg); } catch(e) {}");
            html.AppendLine("}");
            html.AppendLine("");
            html.AppendLine("debugLog('Script iniciado');");
            html.AppendLine("");
            html.AppendLine("window.addEventListener('error', function(ev) {");
            html.AppendLine("  debugLog('JS ERROR: ' + ev.message);");
            html.AppendLine("});");
            html.AppendLine("window.addEventListener('unhandledrejection', function(ev) {");
            html.AppendLine("  debugLog('PROMISE ERROR: ' + (ev.reason && ev.reason.message ? ev.reason.message : ev.reason));");
            html.AppendLine("});");
            html.AppendLine("");
            html.AppendLine("function initMap() {");
            html.AppendLine("  debugLog('initMap llamada');");
            html.AppendFormat(CultureInfo.InvariantCulture, "  const usuario = {{ lat: {0}, lng: {1} }};", latitude, longitude).AppendLine();
            html.AppendLine("  debugLog('Usuario: ' + usuario.lat + ', ' + usuario.lng);");
            html.AppendLine("  ");
            html.AppendLine("  let bubbles = [];");
            html.AppendLine("  try {");
            html.AppendLine("    const bubblesJson = \"" + EscapeJsonForJavaScript(bubblesJson) + "\";");
            html.AppendLine("    debugLog('JSON length: ' + bubblesJson.length);");
            html.AppendLine("    bubbles = JSON.parse(bubblesJson);");
            html.AppendLine("    debugLog('Burbujas parseadas: ' + bubbles.length);");
            html.AppendLine("  } catch(e) {");
            html.AppendLine("    debugLog('Error parseando JSON: ' + e.message);");
            html.AppendLine("  }");
            html.AppendLine("");
            html.AppendLine("  bounds = new google.maps.LatLngBounds();");
            html.AppendLine("  ");
            html.AppendLine("  const mapElement = document.getElementById('map');");
            html.AppendLine("  debugLog('Map element size: ' + mapElement.offsetWidth + 'x' + mapElement.offsetHeight);");
            html.AppendLine("  ");
            html.AppendLine("  map = new google.maps.Map(mapElement, {");
            html.AppendLine("    center: usuario,");
            html.AppendLine("    zoom: 15,");
            html.AppendLine("    mapTypeControl: true,");
            html.AppendLine("    streetViewControl: true,");
            html.AppendLine("    fullscreenControl: true,");
            html.AppendLine("    gestureHandling: 'greedy'");
            html.AppendLine("  });");
            html.AppendLine("  ");
            html.AppendLine("  debugLog('Mapa creado');");
            html.AppendLine("  function forceResize() {");
            html.AppendLine("    try { google.maps.event.trigger(map, 'resize'); } catch(e) {}");
            html.AppendLine("  }");
            html.AppendLine("  setTimeout(forceResize, 150);");
            html.AppendLine("  setTimeout(function() { try { map.setCenter(usuario); } catch(e) {} }, 250);");
            html.AppendLine("  window.addEventListener('resize', function() {");
            html.AppendLine("    setTimeout(forceResize, 50);");
            html.AppendLine("  });");
            html.AppendLine("  google.maps.event.addListenerOnce(map, 'tilesloaded', function() { debugLog('tilesloaded'); });");
            html.AppendLine("  google.maps.event.addListenerOnce(map, 'idle', function() {");
            html.AppendLine("    debugLog('idle');");
            html.AppendLine("    debugLog('MAPA LISTO');");
            html.AppendLine("  });");
            html.AppendLine("  ");
            html.AppendLine("  try {");
            html.AppendLine("    if (typeof google === 'undefined' || !google.maps) throw new Error('google.maps no disponible');");
            html.AppendLine("    if (typeof google.maps.Marker !== 'function') throw new Error('google.maps.Marker no disponible');");
            html.AppendLine("    userMarker = new google.maps.Marker({");
            html.AppendLine("      position: usuario,");
            html.AppendLine("      map: map,");
            html.AppendLine("      title: 'Tu ubicacion',");
            html.AppendLine("      label: { text: 'YO', color: '#ffffff', fontWeight: '700' },");
            html.AppendLine("      icon: { path: google.maps.SymbolPath.CIRCLE, scale: 11, fillColor: '#2563eb', fillOpacity: 1, strokeColor: '#ffffff', strokeWeight: 2 }");
            html.AppendLine("    });");
            html.AppendLine("  } catch(e) {");
            html.AppendLine("    debugLog('Error creando marcador usuario: ' + e.message);");
            html.AppendLine("  }");
            html.AppendLine("  ");
            html.AppendLine("  debugLog('Marcador usuario creado');");
            html.AppendLine("  bounds.extend(usuario);");
            html.AppendLine("  ");
            html.AppendLine("  bubbles.forEach(function(bubble, index) {");
            html.AppendLine("    debugLog('Renderizando burbuja ' + index + ' en ' + bubble.lat + ', ' + bubble.lng);");
            html.AppendLine("    try {");
            html.AppendLine("      const posicion = { lat: bubble.lat, lng: bubble.lng };");
            html.AppendLine("      const color = bubble.propia ? '#00a86b' : '#d62839';");
            html.AppendLine("      ");
            html.AppendLine("      const marcador = new google.maps.Marker({");
            html.AppendLine("        position: posicion,");
            html.AppendLine("        map: map,");
            html.AppendLine("        title: bubble.nombre,");
            html.AppendLine("        label: { text: bubble.propia ? 'TU' : 'B', color: '#ffffff', fontWeight: '700' },");
            html.AppendLine("        icon: { path: google.maps.SymbolPath.CIRCLE, scale: bubble.propia ? 14 : 12, fillColor: color, fillOpacity: 0.95, strokeColor: '#ffffff', strokeWeight: 2 }");
            html.AppendLine("      });");
            html.AppendLine("      ");
            html.AppendLine("      const contenido = '<div style=\"font-family: Segoe UI, sans-serif; padding: 8px;\"><strong>' + bubble.nombre + '</strong><br>' + (bubble.mensaje || '') + (bubble.distancia ? '<br><small>' + bubble.distancia + '</small>' : '') + '</div>';");
            html.AppendLine("      const info = new google.maps.InfoWindow({ content: contenido });");
            html.AppendLine("      marcador.addListener('click', function() { info.open({ anchor: marcador, map: map }); });");
            html.AppendLine("      ");
            html.AppendLine("      const circulo = new google.maps.Circle({");
            html.AppendLine("        strokeColor: color,");
            html.AppendLine("        strokeOpacity: 0.75,");
            html.AppendLine("        strokeWeight: 1.5,");
            html.AppendLine("        fillColor: color,");
            html.AppendLine("        fillOpacity: bubble.propia ? 0.24 : 0.18,");
            html.AppendLine("        map: map,");
            html.AppendLine("        center: posicion,");
            html.AppendLine("        radius: 50");
            html.AppendLine("      });");
            html.AppendLine("      ");
            html.AppendLine("      const markerKey = 'bubble_' + bubble.id;");
            html.AppendLine("      markers[markerKey] = { marker: marcador, info: info };");
            html.AppendLine("      circles[markerKey] = circulo;");
            html.AppendLine("      infoWindows[markerKey] = info;");
            html.AppendLine("      bounds.extend(posicion);");
            html.AppendLine("      debugLog('Burbuja ' + index + ' renderizada OK');");
            html.AppendLine("    } catch(e) {");
            html.AppendLine("      debugLog('Error burbuja ' + index + ': ' + e.message);");
            html.AppendLine("    }");
            html.AppendLine("  });");
            html.AppendLine("  ");
            html.AppendLine("  debugLog('Total burbujas: ' + bubbles.length);");
            html.AppendLine("  ");
            html.AppendLine("  if (bubbles.length > 0) {");
            html.AppendLine("    map.fitBounds(bounds);");
            html.AppendLine("    google.maps.event.addListenerOnce(map, 'bounds_changed', function() {");
            html.AppendLine("      if (map.getZoom() > 17) { map.setZoom(17); }");
            html.AppendLine("    });");
            html.AppendLine("    debugLog('Zoom ajustado para burbujas');");
            html.AppendLine("  } else {");
            html.AppendLine("    map.setCenter(usuario);");
            html.AppendLine("    map.setZoom(15);");
            html.AppendLine("    debugLog('Sin burbujas, zoom por defecto');");
            html.AppendLine("  }");
            html.AppendLine("  ");
            html.AppendLine("}");
            html.AppendLine("");
            html.AppendLine("function updateBubblePositions(newBubbles) {");
            html.AppendLine("  debugLog('Actualizando ' + newBubbles.length + ' burbujas');");
            html.AppendLine("  const newIds = new Set(newBubbles.map(b => 'bubble_' + b.id));");
            html.AppendLine("  const existingIds = new Set(Object.keys(markers));");
            html.AppendLine("  ");
            html.AppendLine("  existingIds.forEach(function(markerKey) {");
            html.AppendLine("    if (!newIds.has(markerKey)) {");
            html.AppendLine("      markers[markerKey].marker.setMap(null);");
            html.AppendLine("      if (infoWindows[markerKey]) infoWindows[markerKey].close();");
            html.AppendLine("      if (circles[markerKey]) circles[markerKey].setMap(null);");
            html.AppendLine("      delete markers[markerKey];");
            html.AppendLine("      delete infoWindows[markerKey];");
            html.AppendLine("      delete circles[markerKey];");
            html.AppendLine("      debugLog('Burbuja eliminada');");
            html.AppendLine("    }");
            html.AppendLine("  });");
            html.AppendLine("  ");
            html.AppendLine("  newBubbles.forEach(function(bubble) {");
            html.AppendLine("    const markerKey = 'bubble_' + bubble.id;");
            html.AppendLine("    if (markers[markerKey]) {");
            html.AppendLine("      const posicion = { lat: bubble.lat, lng: bubble.lng };");
            html.AppendLine("      markers[markerKey].marker.setPosition(posicion);");
            html.AppendLine("      if (circles[markerKey]) circles[markerKey].setCenter(posicion);");
            html.AppendLine("      debugLog('Burbuja ' + bubble.id + ' movida');");
            html.AppendLine("    }");
            html.AppendLine("  });");
            html.AppendLine("}");
            html.AppendLine("");
            html.AppendLine("function replaceMapData(usuario, bubbles) {");
            html.AppendLine("  debugLog('replaceMapData: ' + (bubbles ? bubbles.length : 0) + ' burbujas');");
            html.AppendLine("  try {");
            html.AppendLine("    if (!map || !google || !google.maps) { debugLog('replaceMapData: mapa no listo'); return; }");
            html.AppendLine("    bounds = new google.maps.LatLngBounds();");
            html.AppendLine("    bounds.extend(usuario);");
            html.AppendLine("");
            html.AppendLine("    try {");
            html.AppendLine("      if (userMarker && typeof userMarker.setPosition === 'function') {");
            html.AppendLine("        userMarker.setPosition(usuario);");
            html.AppendLine("      } else {");
            html.AppendLine("        userMarker = new google.maps.Marker({");
            html.AppendLine("          position: usuario,");
            html.AppendLine("          map: map,");
            html.AppendLine("          title: 'Tu ubicacion',");
            html.AppendLine("          label: { text: 'YO', color: '#ffffff', fontWeight: '700' },");
            html.AppendLine("          icon: { path: google.maps.SymbolPath.CIRCLE, scale: 11, fillColor: '#2563eb', fillOpacity: 1, strokeColor: '#ffffff', strokeWeight: 2 }");
            html.AppendLine("        });");
            html.AppendLine("      }");
            html.AppendLine("    } catch(e) {");
            html.AppendLine("      debugLog('replaceMapData: error userMarker: ' + e.message);");
            html.AppendLine("    }");
            html.AppendLine("");
            html.AppendLine("    Object.keys(markers).forEach(function(markerKey) {");
            html.AppendLine("      try {");
            html.AppendLine("        if (markers[markerKey] && markers[markerKey].marker) markers[markerKey].marker.setMap(null);");
            html.AppendLine("        if (infoWindows[markerKey]) infoWindows[markerKey].close();");
            html.AppendLine("        if (circles[markerKey]) circles[markerKey].setMap(null);");
            html.AppendLine("      } catch(e) {}");
            html.AppendLine("    });");
            html.AppendLine("    markers = {}; circles = {}; infoWindows = {};");
            html.AppendLine("");
            html.AppendLine("    (bubbles || []).forEach(function(bubble, index) {");
            html.AppendLine("      try {");
            html.AppendLine("        const posicion = { lat: bubble.lat, lng: bubble.lng };");
            html.AppendLine("        const color = bubble.propia ? '#00a86b' : '#d62839';");
            html.AppendLine("        const marcador = new google.maps.Marker({");
            html.AppendLine("          position: posicion,");
            html.AppendLine("          map: map,");
            html.AppendLine("          title: bubble.nombre,");
            html.AppendLine("          label: { text: bubble.propia ? 'TU' : 'B', color: '#ffffff', fontWeight: '700' },");
            html.AppendLine("          icon: { path: google.maps.SymbolPath.CIRCLE, scale: bubble.propia ? 14 : 12, fillColor: color, fillOpacity: 0.95, strokeColor: '#ffffff', strokeWeight: 2 }");
            html.AppendLine("        });");
            html.AppendLine("        const contenido = '<div style=\"font-family: Segoe UI, sans-serif; padding: 8px;\"><strong>' + bubble.nombre + '</strong><br>' + (bubble.mensaje || '') + (bubble.distancia ? '<br><small>' + bubble.distancia + '</small>' : '') + '</div>';");
            html.AppendLine("        const info = new google.maps.InfoWindow({ content: contenido });");
            html.AppendLine("        marcador.addListener('click', function() { info.open({ anchor: marcador, map: map }); });");
            html.AppendLine("        const circulo = new google.maps.Circle({");
            html.AppendLine("          strokeColor: color,");
            html.AppendLine("          strokeOpacity: 0.75,");
            html.AppendLine("          strokeWeight: 1.5,");
            html.AppendLine("          fillColor: color,");
            html.AppendLine("          fillOpacity: bubble.propia ? 0.24 : 0.18,");
            html.AppendLine("          map: map,");
            html.AppendLine("          center: posicion,");
            html.AppendLine("          radius: 50");
            html.AppendLine("        });");
            html.AppendLine("        const markerKey = 'bubble_' + bubble.id;");
            html.AppendLine("        markers[markerKey] = { marker: marcador, info: info };");
            html.AppendLine("        circles[markerKey] = circulo;");
            html.AppendLine("        infoWindows[markerKey] = info;");
            html.AppendLine("        bounds.extend(posicion);");
            html.AppendLine("      } catch(e) {");
            html.AppendLine("        debugLog('replaceMapData: error burbuja ' + index + ': ' + e.message);");
            html.AppendLine("      }");
            html.AppendLine("    });");
            html.AppendLine("");
            html.AppendLine("    if ((bubbles || []).length > 0) {");
            html.AppendLine("      map.fitBounds(bounds);");
            html.AppendLine("      google.maps.event.addListenerOnce(map, 'bounds_changed', function() { if (map.getZoom() > 17) map.setZoom(17); });");
            html.AppendLine("    } else {");
            html.AppendLine("      map.setCenter(usuario);");
            html.AppendLine("      map.setZoom(15);");
            html.AppendLine("    }");
            html.AppendLine("    try { google.maps.event.trigger(map, 'resize'); } catch(e) {}");
            html.AppendLine("  } catch(e) {");
            html.AppendLine("    debugLog('replaceMapData error: ' + e.message);");
            html.AppendLine("  }");
            html.AppendLine("}");
            html.AppendLine("");
            html.AppendLine("function focusBubble(id) {");
            html.AppendLine("  try {");
            html.AppendLine("    const key = 'bubble_' + id;");
            html.AppendLine("    if (!markers[key] || !markers[key].marker) {");
            html.AppendLine("      debugLog('focusBubble: no existe ' + key);");
            html.AppendLine("      return;");
            html.AppendLine("    }");
            html.AppendLine("    const m = markers[key].marker;");
            html.AppendLine("    const pos = m.getPosition();");
            html.AppendLine("    if (pos) {");
            html.AppendLine("      map.panTo(pos);");
            html.AppendLine("      if (map.getZoom() < 16) map.setZoom(16);");
            html.AppendLine("    }");
            html.AppendLine("    if (markers[key].info) markers[key].info.open(map, m);");
            html.AppendLine("    debugLog('focusBubble: ' + key);");
            html.AppendLine("  } catch(e) {");
            html.AppendLine("    debugLog('focusBubble error: ' + e.message);");
            html.AppendLine("  }");
            html.AppendLine("}");
            html.AppendLine("");
            // Google Maps llama a gm_authFailure si la clave/API esta restringida o no autorizada.
            html.AppendLine("window.gm_authFailure = function() {");
            html.AppendLine("  debugLog('gm_authFailure: la API key no es valida o esta restringida');");
            html.AppendLine("};");
            html.AppendLine("");
            html.AppendLine("let mapsApiLoaded = false;");
            html.AppendLine("let mapStarted = false;");
            html.AppendLine("");
            html.AppendLine("function onGoogleMapsApiLoaded() {");
            html.AppendLine("  mapsApiLoaded = true;");
            html.AppendLine("  debugLog('Google Maps API loaded');");
            html.AppendLine("  startMapWhenReady();");
            html.AppendLine("}");
            html.AppendLine("");
            html.AppendLine("function startMapWhenReady() {");
            html.AppendLine("  if (mapStarted) return;");
            html.AppendLine("  if (!mapsApiLoaded) {");
            html.AppendLine("    debugLog('Esperando a Google Maps API...');");
            html.AppendLine("    return;");
            html.AppendLine("  }");
            html.AppendLine("  const el = document.getElementById('map');");
            html.AppendLine("  if (!el || el.offsetWidth === 0 || el.offsetHeight === 0) {");
            html.AppendLine("    debugLog('Esperando tamanio del contenedor: ' + (el ? (el.offsetWidth + 'x' + el.offsetHeight) : 'no map'));");
            html.AppendLine("    setTimeout(startMapWhenReady, 50);");
            html.AppendLine("    return;");
            html.AppendLine("  }");
            html.AppendLine("  mapStarted = true;");
            html.AppendLine("  try {");
            html.AppendLine("    initMap();");
            html.AppendLine("  } catch(e) {");
            html.AppendLine("    debugLog('Error en initMap: ' + e.message);");
            html.AppendLine("  }");
            html.AppendLine("}");
            html.AppendLine("");
            html.AppendLine("function loadGoogleMaps() {");
            html.AppendLine("  debugLog('Cargando Google Maps JS...');");
            html.AppendLine("  const script = document.createElement('script');");
            html.AppendFormat(
                CultureInfo.InvariantCulture,
                "  script.src = 'https://maps.googleapis.com/maps/api/js?key={0}&callback=onGoogleMapsApiLoaded';",
                _googleMapsApiKey).AppendLine();
            html.AppendLine("  script.async = true;");
            html.AppendLine("  script.defer = true;");
            html.AppendLine("  script.onerror = function() {");
            html.AppendLine("    debugLog('ERROR cargando Google Maps JS (internet/bloqueo/API key)');");
            html.AppendLine("  };");
            html.AppendLine("  document.head.appendChild(script);");
            html.AppendLine("}");
            html.AppendLine("");
            html.AppendLine("loadGoogleMaps();");
            html.AppendLine("</script>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        private string EscapeJsonForJavaScript(string json)
        {
            return json
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }
}
