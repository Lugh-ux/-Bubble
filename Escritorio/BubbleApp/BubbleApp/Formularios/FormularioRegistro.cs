using BubbleApp.Estado;
using BubbleApp.Servicios;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BubbleApp.Formularios
{
    public partial class FormularioRegistro : Form
    {
        private readonly ApiCliente _apiClient = new ApiCliente();

        public FormularioRegistro()
        {
            InitializeComponent();
        }

        private async Task RegisterAsync()
        {
            if (_passwordTextBox.Text != _confirmPasswordTextBox.Text)
            {
                _statusLabel.Text = "Las contrasenas no coinciden.";
                return;
            }

            ToggleLoading(true, "Creando cuenta...");

            try
            {
                var session = await _apiClient.RegisterAsync(
                    _nameTextBox.Text.Trim(),
                    _emailTextBox.Text.Trim(),
                    _passwordTextBox.Text);

                SesionActual.Set(session);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                _statusLabel.Text = ex.Message;
            }
            finally
            {
                ToggleLoading(false, string.Empty);
            }
        }

        private void ToggleLoading(bool isLoading, string message)
        {
            _registerButton.Enabled = !isLoading;
            _statusLabel.ForeColor = isLoading ? Color.FromArgb(59, 76, 202) : Color.Firebrick;
            _statusLabel.Text = message;
        }

        private async void _registerButton_Click(object sender, EventArgs e)
        {
            await RegisterAsync();
        }
    }
}
