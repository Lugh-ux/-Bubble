using BubbleApp.Modelos;
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

            InitializeMapAsync();
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
                return;
            }

            bool locationChanged = Math.Abs(_lastLatitude - latitude) > 0.0001 || 
                                   Math.Abs(_lastLongitude - longitude) > 0.0001;
            bool bubblesCountChanged = _lastBubbles.Count != newBubbles.Count;

            _lastLatitude = latitude;
            _lastLongitude = longitude;
            _lastBubbles = newBubbles;

            if (locationChanged || bubblesCountChanged)
            {
                _fullRenderPending = true;
                RenderMap();
            }
            else
            {
                UpdateBubblesPositionsAsync(newBubbles);
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

        private async void InitializeMapAsync()
        {
            if (string.IsNullOrWhiteSpace(_googleMapsApiKey))
            {
                _infoLabel.Text = "Falta la clave de Google Maps en App.config.";
                return;
            }

            try
            {
                await _mapa.EnsureCoreWebView2Async();
                _mapa.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                _mapa.CoreWebView2.Settings.AreDevToolsEnabled = true;
                _mapaInicializado = true;
                
                System.Threading.Tasks.Task.Delay(100).Wait();
                RenderMap();
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
                System.Diagnostics.Debug.WriteLine($"[ControlMapaRadar] RenderMap - Bubbles: {bubbles.Count}, JSON length: {bubblesJson.Length}");
                
                var html = BuildHtml(_lastLatitude, _lastLongitude, bubblesJson);
                _mapa.NavigateToString(html);
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
            html.AppendLine("#debug { position: absolute; bottom: 10px; left: 10px; background: rgba(0,0,0,0.8); color: #0f0; padding: 8px; font-family: monospace; font-size: 11px; z-index: 10; max-width: 300px; }");
            html.AppendLine(".gm-style-iw { font-family: Segoe UI, sans-serif; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<div id=\"map\"></div>");
            html.AppendLine("<div id=\"debug\"></div>");
            html.AppendLine("<script>");
            html.AppendLine("let map;");
            html.AppendLine("let markers = {};");
            html.AppendLine("let circles = {};");
            html.AppendLine("let infoWindows = {};");
            html.AppendLine("let bounds;");
            html.AppendLine("let userMarker;");
            html.AppendLine("");
            html.AppendLine("function debugLog(msg) {");
            html.AppendLine("  console.log(msg);");
            html.AppendLine("  const debugDiv = document.getElementById('debug');");
            html.AppendLine("  debugDiv.innerHTML = debugDiv.innerHTML + msg + '<br>';");
            html.AppendLine("}");
            html.AppendLine("");
            html.AppendLine("debugLog('Script iniciado');");
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
            html.AppendLine("  ");
            html.AppendLine("  userMarker = new google.maps.Marker({");
            html.AppendLine("    position: usuario,");
            html.AppendLine("    map: map,");
            html.AppendLine("    title: 'Tu ubicacion',");
            html.AppendLine("    label: { text: 'YO', color: '#ffffff', fontWeight: '700' },");
            html.AppendLine("    icon: { path: google.maps.SymbolPath.CIRCLE, scale: 11, fillColor: '#2563eb', fillOpacity: 1, strokeColor: '#ffffff', strokeWeight: 2 }");
            html.AppendLine("  });");
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
            html.AppendLine("  debugLog('MAPA LISTO');");
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
            html.AppendLine("window.addEventListener('load', function() {");
            html.AppendLine("  debugLog('Window load');");
            html.AppendLine("  initMap();");
            html.AppendLine("});");
            html.AppendLine("");
            html.AppendLine("if (document.readyState === 'loading') {");
            html.AppendLine("  document.addEventListener('DOMContentLoaded', function() {");
            html.AppendLine("    debugLog('DOMContentLoaded');");
            html.AppendLine("    initMap();");
            html.AppendLine("  });");
            html.AppendLine("} else if (document.readyState === 'interactive') {");
            html.AppendLine("  debugLog('Document interactive, llamando initMap');");
            html.AppendLine("  initMap();");
            html.AppendLine("}");
            html.AppendLine("</script>");
            html.AppendFormat(
                CultureInfo.InvariantCulture,
                "<script async defer src=\"https://maps.googleapis.com/maps/api/js?key={0}&callback=initMap\"></script>",
                _googleMapsApiKey).AppendLine();
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
