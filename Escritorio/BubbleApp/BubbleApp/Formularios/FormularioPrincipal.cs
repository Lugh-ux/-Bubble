using BubbleApp.Controles;
using BubbleApp.Estado;
using BubbleApp.Modelos;
using BubbleApp.Servicios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace BubbleApp.Formularios
{
    public partial class FormularioPrincipal : Form
    {
        private readonly ApiCliente _apiClient = new ApiCliente();
        private readonly Timer _refreshTimer = new Timer();
        private readonly System.Threading.SemaphoreSlim _refreshLock = new System.Threading.SemaphoreSlim(1, 1);
        private bool _suppressBubbleToggle;
        private ModeloPerfilUsuario _profile;

        public FormularioPrincipal()
        {
            InitializeComponent();
            StyleRadarUi();
            ConfigureRadarList();
            _radarList.ItemSelectionChanged += RadarList_ItemSelectionChanged;
            _refreshTimer.Interval = 3000;
            _refreshTimer.Tick += async (sender, args) => await RefreshDashboardAsync(false);
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            InitializeDefaultLocation();
            await TryUseCurrentLocationAsync(false);
            await LoadDataAsync();
        }

        private void InitializeDefaultLocation()
        {
            _latitudeTextBox.Text = "42.2406";
            _longitudeTextBox.Text = "-8.7261";
        }

        private async Task LoadDataAsync()
        {
            var coordinates = GetCoordinates();
            _mapControl.UpdateData(coordinates.latitude, coordinates.longitude, new List<ModeloBurbuja>());
            await RefreshProfileAsync();
            await RefreshDashboardAsync(true, force: true);
            _refreshTimer.Start();
        }

        private async Task RefreshProfileAsync()
        {
            _profile = await _apiClient.GetProfileAsync();
            _userNameLabel.Text = _profile.User.Name;
            _emailLabel.Text = _profile.User.Email;
            _bubbleCountLabel.Text = string.Format("{0} burbujas activadas", _profile.BubbleCount);

            if (!string.IsNullOrWhiteSpace(_profile.User.AvatarUrl))
            {
                _avatarPictureBox.LoadAsync(_profile.User.AvatarUrl);
            }
            else
            {
                _avatarPictureBox.Image = null;
            }
        }

        private async Task RefreshDashboardAsync(bool showStatus, bool force = false)
        {
            if (force)
            {
                await _refreshLock.WaitAsync();
            }
            else
            {
                // Evita solapamientos del Timer (y acciones del usuario) que pueden dejar el WebView2 en blanco.
                if (!await _refreshLock.WaitAsync(0))
                {
                    return;
                }
            }

            try
            {
                var coordinates = GetCoordinates();

                if (showStatus)
                {
                    SetStatus("Consultando radar...");
                }

                var dashboard = await _apiClient.GetDashboardAsync(coordinates.latitude, coordinates.longitude);

                _suppressBubbleToggle = true;
                _bubbleToggle.Checked = dashboard.MyBubbleActive;
                _suppressBubbleToggle = false;

                _radarList.BeginUpdate();
                _radarList.Items.Clear();

                foreach (var bubble in dashboard.Bubbles)
                {
                    var item = new ListViewItem(bubble.User.Name);
                    item.SubItems.Add(bubble.Message);
                    item.SubItems.Add(bubble.DistanceLabel);
                    item.Tag = bubble.Id;
                    _radarList.Items.Add(item);
                }

                _radarList.EndUpdate();
                var bubblesForMap = dashboard.Bubbles.ToList();
                if (dashboard.MyBubble != null)
                {
                    bubblesForMap.Add(dashboard.MyBubble);
                }

                System.Diagnostics.Debug.WriteLine($"[DEBUG] Dashboard - Bubbles: {dashboard.Bubbles.Count}, MyBubble: {(dashboard.MyBubble != null ? "SI" : "NO")}, MyBubbleActive: {dashboard.MyBubbleActive}, Total para mapa: {bubblesForMap.Count}");

                _mapControl.UpdateData(coordinates.latitude, coordinates.longitude, bubblesForMap);
                _statusLabel.ForeColor = Color.FromArgb(95, 95, 95);
                _statusLabel.Text = string.Format("Detectadas {0} burbujas cerca.", dashboard.Bubbles.Count);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Error en RefreshDashboard: {ex.Message}\n{ex.StackTrace}");
                try
                {
                    var coordinates = GetCoordinates();
                    _mapControl.UpdateData(coordinates.latitude, coordinates.longitude, new List<ModeloBurbuja>());
                }
                catch
                {
                }

                _statusLabel.ForeColor = Color.Firebrick;
                _statusLabel.Text = ex.Message;
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        private async void BubbleToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressBubbleToggle)
            {
                return;
            }

            try
            {
                _refreshTimer.Stop();
                var coordinates = GetCoordinates();
                SetStatus(_bubbleToggle.Checked ? "Activando tu burbuja..." : "Eliminando tu burbuja...");

                if (_bubbleToggle.Checked)
                {
                    await _apiClient.ActivateBubbleAsync(coordinates.latitude, coordinates.longitude);
                }
                else
                {
                    await _apiClient.DeleteBubbleAsync();
                }

                await RefreshProfileAsync();
                await RefreshDashboardAsync(false, force: true);
            }
            catch (Exception ex)
            {
                _statusLabel.ForeColor = Color.Firebrick;
                _statusLabel.Text = ex.Message;
            }
            finally
            {
                if (SesionActual.IsAuthenticated)
                {
                    _refreshTimer.Start();
                }
            }
        }

        private async void buttonEditarPerfil_Click(object sender, EventArgs e)
        {
            if (_profile == null)
            {
                return;
            }

            using (var profileForm = new FormularioPerfil(_profile))
            {
                if (profileForm.ShowDialog(this) == DialogResult.OK)
                {
                    await RefreshProfileAsync();
                    await RefreshDashboardAsync(false, force: true);
                }
            }
        }

        private async void buttonActualizarRadar_Click(object sender, EventArgs e)
        {
            await RefreshDashboardAsync(true, force: true);
        }

        private async void buttonUsarUbicacionActual_Click(object sender, EventArgs e)
        {
            await TryUseCurrentLocationAsync(true);
        }

        private async void buttonCerrarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                await _apiClient.LogoutAsync();
            }
            catch
            {
            }

            SesionActual.Clear();
            Close();
        }

        private (double latitude, double longitude) GetCoordinates()
        {
            if (!double.TryParse(_latitudeTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var latitude))
            {
                throw new InvalidOperationException("La latitud no es valida.");
            }

            if (!double.TryParse(_longitudeTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var longitude))
            {
                throw new InvalidOperationException("La longitud no es valida.");
            }

            return (latitude, longitude);
        }

        private void SetStatus(string message)
        {
            _statusLabel.ForeColor = Color.FromArgb(59, 76, 202);
            _statusLabel.Text = message;
        }

        private async Task TryUseCurrentLocationAsync(bool refreshAfterUpdate)
        {
            try
            {
                SetStatus("Buscando tu ubicacion actual...");
                var coordinates = await GetCurrentLocationAsync();

                _latitudeTextBox.Text = coordinates.latitude.ToString("F6", CultureInfo.InvariantCulture);
                _longitudeTextBox.Text = coordinates.longitude.ToString("F6", CultureInfo.InvariantCulture);

                if (refreshAfterUpdate)
                {
                    await RefreshDashboardAsync(true, force: true);
                }
                else
                {
                    _mapControl.UpdateData(coordinates.latitude, coordinates.longitude, new List<ModeloBurbuja>());
                    SetStatus("Ubicacion actual detectada.");
                }
            }
            catch (Exception ex)
            {
                SetStatus("No se pudo detectar GPS: " + ex.Message + " (usando ubicación manual)");
            }
        }

        private async Task<(double latitude, double longitude)> GetCurrentLocationAsync()
        {
            var watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            var taskSource = new TaskCompletionSource<GeoCoordinate>();

            EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>> positionChanged = null;
            EventHandler<GeoPositionStatusChangedEventArgs> statusChanged = null;

            positionChanged = (sender, args) =>
            {
                if (!args.Position.Location.IsUnknown)
                {
                    taskSource.TrySetResult(args.Position.Location);
                }
            };

            statusChanged = (sender, args) =>
            {
                if (args.Status == GeoPositionStatus.Disabled)
                {
                    taskSource.TrySetException(new InvalidOperationException("La ubicacion esta desactivada en Windows."));
                }

                if (args.Status == GeoPositionStatus.NoData)
                {
                    taskSource.TrySetException(new InvalidOperationException("Windows no ha podido obtener tu ubicacion."));
                }
            };

            watcher.PositionChanged += positionChanged;
            watcher.StatusChanged += statusChanged;
            watcher.Start();

            try
            {
                var completedTask = await Task.WhenAny(taskSource.Task, Task.Delay(8000));
                if (completedTask != taskSource.Task)
                {
                    throw new InvalidOperationException("Tiempo agotado al intentar obtener tu ubicacion.");
                }

                var location = await taskSource.Task;
                return (location.Latitude, location.Longitude);
            }
            finally
            {
                watcher.PositionChanged -= positionChanged;
                watcher.StatusChanged -= statusChanged;
                watcher.Stop();
                watcher.Dispose();
            }
        }

        private void ConfigureRadarList()
        {
            // En View.Details, sin columnas los subitems (mensaje/distancia) no se muestran.
            _radarList.BeginUpdate();
            try
            {
                _radarList.Columns.Clear();
                _radarList.View = View.Details;
                _radarList.FullRowSelect = true;
                _radarList.MultiSelect = false;
                _radarList.HideSelection = false;
                _radarList.HeaderStyle = ColumnHeaderStyle.None;
                _radarList.BorderStyle = BorderStyle.None;

                // 0: Nombre+mensaje (se dibuja custom)
                // 1: Distancia (badge)
                _radarList.Columns.Add("Burbuja", 220);
                _radarList.Columns.Add("Dist", 55, HorizontalAlignment.Right);

                SetupRadarOwnerDraw();

                // Ajusta altura de fila para el render de 2 lineas (ListView no expone ItemHeight).
                // Truco: SmallImageList define el alto de fila en modo Details.
                var rowHeight = 54;
                if (_radarList.SmallImageList == null || _radarList.SmallImageList.ImageSize.Height != rowHeight)
                {
                    _radarList.SmallImageList = new ImageList
                    {
                        ImageSize = new Size(1, rowHeight),
                        ColorDepth = ColorDepth.Depth8Bit
                    };
                }

                _radarList.Resize -= RadarList_Resize;
                _radarList.Resize += RadarList_Resize;
                RadarList_Resize(this, EventArgs.Empty);
            }
            finally
            {
                _radarList.EndUpdate();
            }
        }

        private void RadarList_Resize(object sender, EventArgs e)
        {
            if (_radarList.Columns.Count < 2)
            {
                return;
            }

            var distW = 70;
            var contentW = Math.Max(120, _radarList.ClientSize.Width - distW - 8);
            _radarList.Columns[0].Width = contentW;
            _radarList.Columns[1].Width = distW;
        }

        private void StyleRadarUi()
        {
            // Radar: fondo blanco con texto negro, y badge de distancia negro.
            panelRadar.BackColor = Color.White;
            panelRadar.Padding = new Padding(14, 14, 14, 14);

            labelRadar.ForeColor = Color.Black;
            labelRadar.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);

            _radarList.BackColor = Color.White;
            _radarList.ForeColor = Color.Black;
            _radarList.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }

        private void SetupRadarOwnerDraw()
        {
            if (_radarList.OwnerDraw)
            {
                return;
            }

            _radarList.OwnerDraw = true;
            _radarList.DrawColumnHeader += (s, e) => { e.DrawDefault = false; };
            _radarList.DrawItem += RadarList_DrawItem;
            _radarList.DrawSubItem += RadarList_DrawSubItem;
            _radarList.MouseMove += (s, e) => _radarList.Invalidate();

            // Reduce parpadeo.
            try
            {
                typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.SetValue(_radarList, true, null);
            }
            catch
            {
            }
        }

        private void RadarList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // En Details, el fondo lo dibujamos nosotros (alternating + selected).
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var isSelected = e.Item.Selected;
            var isEven = (e.ItemIndex % 2) == 0;

            var bg = isSelected
                ? Color.FromArgb(225, 235, 255)
                : (isEven ? Color.White : Color.FromArgb(248, 249, 252));

            using (var b = new SolidBrush(bg))
            {
                g.FillRectangle(b, e.Bounds);
            }

            // Separador sutil.
            using (var pen = new Pen(Color.FromArgb(30, 0, 0, 0)))
            {
                g.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }
        }

        private void RadarList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var item = e.Item;
            var isSelected = item.Selected;

            // Datos vienen de RefreshDashboardAsync:
            // item.Text = nombre
            // item.SubItems[1] = mensaje
            // item.SubItems[2] = distancia
            var nombre = item.Text ?? string.Empty;
            var mensaje = item.SubItems.Count > 1 ? (item.SubItems[1].Text ?? string.Empty) : string.Empty;
            var distancia = item.SubItems.Count > 2 ? (item.SubItems[2].Text ?? string.Empty) : string.Empty;

            var rect = e.Bounds;
            rect.Inflate(-10, -6);

            var primary = Color.Black;
            var secondary = Color.FromArgb(120, 0, 0, 0);

            if (e.ColumnIndex == 0)
            {
                // Avatar (inicial)
                var avatarSize = 28;
                var avatarRect = new Rectangle(rect.Left, rect.Top + 3, avatarSize, avatarSize);
                var initial = string.IsNullOrWhiteSpace(nombre) ? "?" : nombre.Trim().Substring(0, 1).ToUpperInvariant();
                var avatarColor = ColorFromName(nombre);

                using (var b = new SolidBrush(avatarColor))
                using (var pen = new Pen(Color.FromArgb(60, 255, 255, 255), 1f))
                {
                    g.FillEllipse(b, avatarRect);
                    g.DrawEllipse(pen, avatarRect);
                }
                using (var f = new Font("Segoe UI Semibold", 10F, FontStyle.Bold))
                using (var sb = new SolidBrush(Color.White))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(initial, f, sb, avatarRect, sf);
                }

                var textLeft = avatarRect.Right + 10;
                var nameRect = new Rectangle(textLeft, rect.Top, rect.Width - (textLeft - rect.Left), 18);
                var msgRect = new Rectangle(textLeft, rect.Top + 18, rect.Width - (textLeft - rect.Left), 18);

                using (var fName = new Font("Segoe UI Semibold", 10F, FontStyle.Bold))
                using (var fMsg = new Font("Segoe UI", 9F, FontStyle.Regular))
                {
                    TextRenderer.DrawText(g, nombre, fName, nameRect, primary, TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                    TextRenderer.DrawText(g, mensaje, fMsg, msgRect, secondary, TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                }
            }
            else if (e.ColumnIndex == 1)
            {
                // Badge distancia
                var meters = TryParseDistanceMeters(distancia);
                var badgeColor = GetDistanceBadgeColor(meters, isSelected);

                var badgeText = string.IsNullOrWhiteSpace(distancia) ? "-" : distancia;
                using (var f = new Font("Segoe UI Semibold", 9F, FontStyle.Bold))
                {
                    var size = TextRenderer.MeasureText(g, badgeText, f);
                    var padX = 10;
                    var padY = 6;
                    var w = Math.Min(rect.Width, size.Width + padX * 2);
                    var h = size.Height + padY;
                    var x = rect.Right - w;
                    var y = rect.Top + (rect.Height - h) / 2;
                    var badgeRect = new Rectangle(x, y, w, h);

                    using (var path = RoundedRect(badgeRect, 10))
                    using (var b = new SolidBrush(badgeColor))
                    using (var pen = new Pen(Color.FromArgb(50, 255, 255, 255), 1f))
                    {
                        g.FillPath(b, path);
                        g.DrawPath(pen, path);
                    }

                    TextRenderer.DrawText(
                        g,
                        badgeText,
                        f,
                        badgeRect,
                        Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            var d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static double? TryParseDistanceMeters(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return null;
            }

            var s = label.Trim().ToLowerInvariant();
            s = s.Replace(',', '.');

            if (s.EndsWith("km"))
            {
                var num = s.Replace("km", string.Empty).Trim();
                if (double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out var km))
                {
                    return km * 1000d;
                }
            }
            else if (s.EndsWith("m"))
            {
                var num = s.Replace("m", string.Empty).Trim();
                if (double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out var m))
                {
                    return m;
                }
            }

            return null;
        }

        private static Color GetDistanceBadgeColor(double? meters, bool isSelected)
        {
            // Requisito: metros en negro con numeros en blanco.
            // Seleccion: ligeramente mas claro para no "desaparecer" en el highlight.
            return isSelected ? Color.FromArgb(255, 20, 20, 20) : Color.FromArgb(255, 0, 0, 0);
        }

        private static Color ColorFromName(string name)
        {
            unchecked
            {
                var hash = 17;
                foreach (var ch in (name ?? string.Empty))
                {
                    hash = hash * 31 + ch;
                }

                // Pastel-ish but vivid on dark bg.
                var r = 90 + (hash & 0x7F);
                var g = 90 + ((hash >> 7) & 0x7F);
                var b = 90 + ((hash >> 14) & 0x7F);
                return Color.FromArgb(255, r, g, b);
            }
        }

        private async void RadarList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected)
            {
                return;
            }

            if (e.Item?.Tag is int bubbleId)
            {
                await _mapControl.FocusBubbleAsync(bubbleId);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _refreshTimer.Stop();
            base.OnFormClosed(e);
        }

        private void labelLogo_Click(object sender, EventArgs e)
        {

        }
    }
}
