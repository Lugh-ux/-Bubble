using BubbleApp.Estado;
using BubbleApp.Servicios;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BubbleApp.Formularios
{
    public partial class FormularioInicioSesion : Form
    {
        private readonly ApiCliente _apiClient = new ApiCliente();

        public FormularioInicioSesion()
        {
            InitializeComponent();
        }

        private async Task LoginAsync()
        {
            ToggleLoading(true, "Validando credenciales...");

            try
            {
                var session = await _apiClient.LoginAsync(_emailTextBox.Text.Trim(), _passwordTextBox.Text);
                SesionActual.Set(session);
                await AbrirPrincipalAsync();
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

        private void OpenRegister()
        {
            using (var registerForm = new FormularioRegistro())
            {
                if (registerForm.ShowDialog(this) == DialogResult.OK && SesionActual.IsAuthenticated)
                {
                    _ = AbrirPrincipalAsync();
                }
            }
        }

        private async Task AbrirPrincipalAsync()
        {
            Hide();

            using (var mainForm = new FormularioPrincipal())
            {
                mainForm.ShowDialog(this);
            }

            if (SesionActual.IsAuthenticated)
            {
                Close();
                return;
            }

            _passwordTextBox.Text = string.Empty;
            _statusLabel.Text = string.Empty;
            Show();
            await Task.CompletedTask;
        }

        private void ToggleLoading(bool isLoading, string message)
        {
            _loginButton.Enabled = !isLoading;
            _registerLink.Enabled = !isLoading;
            _statusLabel.ForeColor = isLoading ? Color.FromArgb(59, 76, 202) : Color.Firebrick;
            _statusLabel.Text = message;
        }

        private async void _loginButton_Click(object sender, EventArgs e)
        {
            await LoginAsync();
        }

        private void _registerLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenRegister();
        }

        private void labelBienvenida_Click(object sender, EventArgs e)
        {

        }

        private void _emailTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
