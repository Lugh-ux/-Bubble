using BubbleApp.Estado;
using BubbleApp.Modelos;
using BubbleApp.Servicios;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BubbleApp.Formularios
{
    public partial class FormularioPerfil : Form
    {
        private readonly ApiCliente _apiClient = new ApiCliente();
        private readonly ModeloPerfilUsuario _profile;
        private string _selectedAvatarPath;

        public ModeloUsuario UpdatedUser { get; private set; }

        public FormularioPerfil(ModeloPerfilUsuario profile)
        {
            _profile = profile;
            InitializeComponent();
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            _nameTextBox.Text = _profile.User.Name;

            if (!string.IsNullOrWhiteSpace(_profile.User.AvatarUrl))
            {
                _avatarPictureBox.LoadAsync(_profile.User.AvatarUrl);
            }
        }

        private void SelectAvatar()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Imagenes|*.png;*.jpg;*.jpeg;*.gif";

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _selectedAvatarPath = dialog.FileName;

                    using (var stream = File.OpenRead(dialog.FileName))
                    {
                        _avatarPictureBox.Image = Image.FromStream(stream);
                    }
                }
            }
        }

        private async Task SaveAsync(Button saveButton)
        {
            saveButton.Enabled = false;
            _statusLabel.ForeColor = Color.FromArgb(59, 76, 202);
            _statusLabel.Text = "Guardando perfil...";

            try
            {
                var user = await _apiClient.UpdateProfileAsync(_nameTextBox.Text.Trim(), _selectedAvatarPath);
                UpdatedUser = user;
                SesionActual.Set(new SesionUsuario { Token = SesionActual.Current.Token, User = user });
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                _statusLabel.ForeColor = Color.Firebrick;
                _statusLabel.Text = ex.Message;
            }
            finally
            {
                saveButton.Enabled = true;
            }
        }

        private void buttonCambiarFoto_Click(object sender, EventArgs e)
        {
            SelectAvatar();
        }

        private async void _guardarButton_Click(object sender, EventArgs e)
        {
            await SaveAsync(_guardarButton);
        }
    }
}
