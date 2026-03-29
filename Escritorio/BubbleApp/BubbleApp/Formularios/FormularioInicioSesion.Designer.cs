namespace BubbleApp.Formularios
{
    partial class FormularioInicioSesion
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelIzquierdo;
        private System.Windows.Forms.Label labelBienvenida;
        private System.Windows.Forms.Label labelBienvenidaTexto;
        private System.Windows.Forms.Panel panelTarjeta;
        private System.Windows.Forms.Label labelTitulo;
        private System.Windows.Forms.Label labelCorreo;
        private System.Windows.Forms.Label labelContrasena;
        private System.Windows.Forms.Label labelCuenta;
        private System.Windows.Forms.TextBox _emailTextBox;
        private System.Windows.Forms.TextBox _passwordTextBox;
        private System.Windows.Forms.Label _statusLabel;
        private System.Windows.Forms.Button _loginButton;
        private System.Windows.Forms.LinkLabel _registerLink;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormularioInicioSesion));
            this.panelIzquierdo = new System.Windows.Forms.Panel();
            this.labelBienvenidaTexto = new System.Windows.Forms.Label();
            this.labelBienvenida = new System.Windows.Forms.Label();
            this.panelTarjeta = new System.Windows.Forms.Panel();
            this._registerLink = new System.Windows.Forms.LinkLabel();
            this.labelCuenta = new System.Windows.Forms.Label();
            this._loginButton = new System.Windows.Forms.Button();
            this._statusLabel = new System.Windows.Forms.Label();
            this._passwordTextBox = new System.Windows.Forms.TextBox();
            this._emailTextBox = new System.Windows.Forms.TextBox();
            this.labelContrasena = new System.Windows.Forms.Label();
            this.labelCorreo = new System.Windows.Forms.Label();
            this.labelTitulo = new System.Windows.Forms.Label();
            this.panelIzquierdo.SuspendLayout();
            this.panelTarjeta.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelIzquierdo
            // 
            this.panelIzquierdo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelIzquierdo.Controls.Add(this.labelBienvenidaTexto);
            this.panelIzquierdo.Controls.Add(this.labelBienvenida);
            this.panelIzquierdo.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelIzquierdo.Location = new System.Drawing.Point(0, 0);
            this.panelIzquierdo.Name = "panelIzquierdo";
            this.panelIzquierdo.Size = new System.Drawing.Size(420, 620);
            this.panelIzquierdo.TabIndex = 1;
            // 
            // labelBienvenidaTexto
            // 
            this.labelBienvenidaTexto.BackColor = System.Drawing.Color.Transparent;
            this.labelBienvenidaTexto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.labelBienvenidaTexto.Location = new System.Drawing.Point(50, 334);
            this.labelBienvenidaTexto.Name = "labelBienvenidaTexto";
            this.labelBienvenidaTexto.Size = new System.Drawing.Size(290, 88);
            this.labelBienvenidaTexto.TabIndex = 0;
            this.labelBienvenidaTexto.Text = "Accede al radar, activa tu burbuja y gestiona tu perfil desde escritorio.";
            // 
            // labelBienvenida
            // 
            this.labelBienvenida.BackColor = System.Drawing.Color.Transparent;
            this.labelBienvenida.Font = new System.Drawing.Font("Segoe UI Semibold", 34F, System.Drawing.FontStyle.Bold);
            this.labelBienvenida.ForeColor = System.Drawing.Color.White;
            this.labelBienvenida.Location = new System.Drawing.Point(25, 147);
            this.labelBienvenida.Name = "labelBienvenida";
            this.labelBienvenida.Size = new System.Drawing.Size(392, 160);
            this.labelBienvenida.TabIndex = 1;
            this.labelBienvenida.Text = "Bienvenido";
            this.labelBienvenida.Click += new System.EventHandler(this.labelBienvenida_Click);
            // 
            // panelTarjeta
            // 
            this.panelTarjeta.BackColor = System.Drawing.Color.White;
            this.panelTarjeta.Controls.Add(this._registerLink);
            this.panelTarjeta.Controls.Add(this.labelCuenta);
            this.panelTarjeta.Controls.Add(this._loginButton);
            this.panelTarjeta.Controls.Add(this._statusLabel);
            this.panelTarjeta.Controls.Add(this._passwordTextBox);
            this.panelTarjeta.Controls.Add(this._emailTextBox);
            this.panelTarjeta.Controls.Add(this.labelContrasena);
            this.panelTarjeta.Controls.Add(this.labelCorreo);
            this.panelTarjeta.Controls.Add(this.labelTitulo);
            this.panelTarjeta.Location = new System.Drawing.Point(470, 78);
            this.panelTarjeta.Name = "panelTarjeta";
            this.panelTarjeta.Size = new System.Drawing.Size(420, 460);
            this.panelTarjeta.TabIndex = 0;
            // 
            // _registerLink
            // 
            this._registerLink.AutoSize = true;
            this._registerLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(76)))), ((int)(((byte)(202)))));
            this._registerLink.Location = new System.Drawing.Point(213, 420);
            this._registerLink.Name = "_registerLink";
            this._registerLink.Size = new System.Drawing.Size(121, 28);
            this._registerLink.TabIndex = 0;
            this._registerLink.TabStop = true;
            this._registerLink.Text = "Crear cuenta";
            this._registerLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._registerLink_LinkClicked);
            // 
            // labelCuenta
            // 
            this.labelCuenta.AutoSize = true;
            this.labelCuenta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.labelCuenta.Location = new System.Drawing.Point(40, 420);
            this.labelCuenta.Name = "labelCuenta";
            this.labelCuenta.Size = new System.Drawing.Size(167, 28);
            this.labelCuenta.TabIndex = 1;
            this.labelCuenta.Text = "No tienes cuenta?";
            // 
            // _loginButton
            // 
            this._loginButton.BackColor = System.Drawing.Color.Black;
            this._loginButton.FlatAppearance.BorderSize = 0;
            this._loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._loginButton.ForeColor = System.Drawing.Color.White;
            this._loginButton.Location = new System.Drawing.Point(40, 358);
            this._loginButton.Name = "_loginButton";
            this._loginButton.Size = new System.Drawing.Size(340, 46);
            this._loginButton.TabIndex = 2;
            this._loginButton.Text = "Iniciar sesion";
            this._loginButton.UseVisualStyleBackColor = false;
            this._loginButton.Click += new System.EventHandler(this._loginButton_Click);
            // 
            // _statusLabel
            // 
            this._statusLabel.ForeColor = System.Drawing.Color.Firebrick;
            this._statusLabel.Location = new System.Drawing.Point(40, 302);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(340, 46);
            this._statusLabel.TabIndex = 3;
            // 
            // _passwordTextBox
            // 
            this._passwordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._passwordTextBox.Location = new System.Drawing.Point(40, 253);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.Size = new System.Drawing.Size(340, 34);
            this._passwordTextBox.TabIndex = 4;
            this._passwordTextBox.UseSystemPasswordChar = true;
            // 
            // _emailTextBox
            // 
            this._emailTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._emailTextBox.Location = new System.Drawing.Point(40, 163);
            this._emailTextBox.Name = "_emailTextBox";
            this._emailTextBox.Size = new System.Drawing.Size(340, 34);
            this._emailTextBox.TabIndex = 5;
            this._emailTextBox.TextChanged += new System.EventHandler(this._emailTextBox_TextChanged);
            // 
            // labelContrasena
            // 
            this.labelContrasena.AutoSize = true;
            this.labelContrasena.Location = new System.Drawing.Point(40, 222);
            this.labelContrasena.Name = "labelContrasena";
            this.labelContrasena.Size = new System.Drawing.Size(110, 28);
            this.labelContrasena.TabIndex = 6;
            this.labelContrasena.Text = "Contrasena";
            // 
            // labelCorreo
            // 
            this.labelCorreo.AutoSize = true;
            this.labelCorreo.Location = new System.Drawing.Point(40, 132);
            this.labelCorreo.Name = "labelCorreo";
            this.labelCorreo.Size = new System.Drawing.Size(174, 28);
            this.labelCorreo.TabIndex = 7;
            this.labelCorreo.Text = "Correo electronico";
            // 
            // labelTitulo
            // 
            this.labelTitulo.AutoSize = true;
            this.labelTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 24F, System.Drawing.FontStyle.Bold);
            this.labelTitulo.Location = new System.Drawing.Point(36, 30);
            this.labelTitulo.Name = "labelTitulo";
            this.labelTitulo.Size = new System.Drawing.Size(367, 65);
            this.labelTitulo.TabIndex = 9;
            this.labelTitulo.Text = "Inicio de sesion";
            // 
            // FormularioInicioSesion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(19)))), ((int)(((byte)(19)))));
            this.BackgroundImage = global::BubbleApp.Properties.Resources.fondo10;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(980, 620);
            this.Controls.Add(this.panelTarjeta);
            this.Controls.Add(this.panelIzquierdo);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormularioInicioSesion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bubble";
            this.panelIzquierdo.ResumeLayout(false);
            this.panelTarjeta.ResumeLayout(false);
            this.panelTarjeta.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
