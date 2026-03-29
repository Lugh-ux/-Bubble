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

namespace BubbleApp.Formularios
{
    public partial class FormularioPrincipal : Form
    {
        private readonly ApiCliente _apiClient = new ApiCliente();
        private readonly Timer _refreshTimer = new Timer();
        private bool _suppressBubbleToggle;
        private ModeloPerfilUsuario _profile;

        public FormularioPrincipal()
        {
            InitializeComponent();
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
            await RefreshDashboardAsync(true);
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

        private async Task RefreshDashboardAsync(bool showStatus)
        {
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
        }

        private async void BubbleToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressBubbleToggle)
            {
                return;
            }

            try
            {
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
                await RefreshDashboardAsync(false);
            }
            catch (Exception ex)
            {
                _statusLabel.ForeColor = Color.Firebrick;
                _statusLabel.Text = ex.Message;
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
                    await RefreshDashboardAsync(false);
                }
            }
        }

        private async void buttonActualizarRadar_Click(object sender, EventArgs e)
        {
            await RefreshDashboardAsync(true);
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
                    await RefreshDashboardAsync(true);
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
