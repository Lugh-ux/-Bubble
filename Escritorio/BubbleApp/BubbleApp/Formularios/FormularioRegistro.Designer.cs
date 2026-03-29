namespace BubbleApp.Formularios
{
    partial class FormularioRegistro
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelTarjeta;
        private System.Windows.Forms.Label labelTitulo;
        private System.Windows.Forms.Label labelNombre;
        private System.Windows.Forms.Label labelCorreo;
        private System.Windows.Forms.Label labelContrasena;
        private System.Windows.Forms.Label labelConfirmar;
        private System.Windows.Forms.TextBox _nameTextBox;
        private System.Windows.Forms.TextBox _emailTextBox;
        private System.Windows.Forms.TextBox _passwordTextBox;
        private System.Windows.Forms.TextBox _confirmPasswordTextBox;
        private System.Windows.Forms.Button _registerButton;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormularioRegistro));
            this.panelTarjeta = new System.Windows.Forms.Panel();
            this._registerButton = new System.Windows.Forms.Button();
            this._statusLabel = new System.Windows.Forms.Label();
            this._confirmPasswordTextBox = new System.Windows.Forms.TextBox();
            this._passwordTextBox = new System.Windows.Forms.TextBox();
            this._emailTextBox = new System.Windows.Forms.TextBox();
            this._nameTextBox = new System.Windows.Forms.TextBox();
            this.labelConfirmar = new System.Windows.Forms.Label();
            this.labelContrasena = new System.Windows.Forms.Label();
            this.labelCorreo = new System.Windows.Forms.Label();
            this.labelNombre = new System.Windows.Forms.Label();
            this.labelTitulo = new System.Windows.Forms.Label();
            this.panelTarjeta.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTarjeta
            // 
            this.panelTarjeta.BackColor = System.Drawing.Color.White;
            this.panelTarjeta.Controls.Add(this._registerButton);
            this.panelTarjeta.Controls.Add(this._statusLabel);
            this.panelTarjeta.Controls.Add(this._confirmPasswordTextBox);
            this.panelTarjeta.Controls.Add(this._passwordTextBox);
            this.panelTarjeta.Controls.Add(this._emailTextBox);
            this.panelTarjeta.Controls.Add(this._nameTextBox);
            this.panelTarjeta.Controls.Add(this.labelConfirmar);
            this.panelTarjeta.Controls.Add(this.labelContrasena);
            this.panelTarjeta.Controls.Add(this.labelCorreo);
            this.panelTarjeta.Controls.Add(this.labelNombre);
            this.panelTarjeta.Controls.Add(this.labelTitulo);
            this.panelTarjeta.Location = new System.Drawing.Point(70, 52);
            this.panelTarjeta.Name = "panelTarjeta";
            this.panelTarjeta.Size = new System.Drawing.Size(420, 450);
            this.panelTarjeta.TabIndex = 0;
            // 
            // _registerButton
            // 
            this._registerButton.BackColor = System.Drawing.Color.Black;
            this._registerButton.FlatAppearance.BorderSize = 0;
            this._registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._registerButton.ForeColor = System.Drawing.Color.White;
            this._registerButton.Location = new System.Drawing.Point(34, 396);
            this._registerButton.Name = "_registerButton";
            this._registerButton.Size = new System.Drawing.Size(340, 44);
            this._registerButton.TabIndex = 0;
            this._registerButton.Text = "Registrarse";
            this._registerButton.UseVisualStyleBackColor = false;
            this._registerButton.Click += new System.EventHandler(this._registerButton_Click);
            // 
            // _statusLabel
            // 
            this._statusLabel.ForeColor = System.Drawing.Color.Firebrick;
            this._statusLabel.Location = new System.Drawing.Point(34, 363);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(340, 30);
            this._statusLabel.TabIndex = 1;
            // 
            // _confirmPasswordTextBox
            // 
            this._confirmPasswordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._confirmPasswordTextBox.Location = new System.Drawing.Point(34, 327);
            this._confirmPasswordTextBox.Name = "_confirmPasswordTextBox";
            this._confirmPasswordTextBox.Size = new System.Drawing.Size(340, 34);
            this._confirmPasswordTextBox.TabIndex = 2;
            this._confirmPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // _passwordTextBox
            // 
            this._passwordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._passwordTextBox.Location = new System.Drawing.Point(34, 259);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.Size = new System.Drawing.Size(340, 34);
            this._passwordTextBox.TabIndex = 3;
            this._passwordTextBox.UseSystemPasswordChar = true;
            // 
            // _emailTextBox
            // 
            this._emailTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._emailTextBox.Location = new System.Drawing.Point(34, 191);
            this._emailTextBox.Name = "_emailTextBox";
            this._emailTextBox.Size = new System.Drawing.Size(340, 34);
            this._emailTextBox.TabIndex = 4;
            // 
            // _nameTextBox
            // 
            this._nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._nameTextBox.Location = new System.Drawing.Point(34, 123);
            this._nameTextBox.Name = "_nameTextBox";
            this._nameTextBox.Size = new System.Drawing.Size(340, 34);
            this._nameTextBox.TabIndex = 5;
            // 
            // labelConfirmar
            // 
            this.labelConfirmar.AutoSize = true;
            this.labelConfirmar.Location = new System.Drawing.Point(34, 296);
            this.labelConfirmar.Name = "labelConfirmar";
            this.labelConfirmar.Size = new System.Drawing.Size(199, 28);
            this.labelConfirmar.TabIndex = 6;
            this.labelConfirmar.Text = "Confirmar contrasena";
            // 
            // labelContrasena
            // 
            this.labelContrasena.AutoSize = true;
            this.labelContrasena.Location = new System.Drawing.Point(34, 228);
            this.labelContrasena.Name = "labelContrasena";
            this.labelContrasena.Size = new System.Drawing.Size(110, 28);
            this.labelContrasena.TabIndex = 7;
            this.labelContrasena.Text = "Contrasena";
            // 
            // labelCorreo
            // 
            this.labelCorreo.AutoSize = true;
            this.labelCorreo.Location = new System.Drawing.Point(34, 160);
            this.labelCorreo.Name = "labelCorreo";
            this.labelCorreo.Size = new System.Drawing.Size(174, 28);
            this.labelCorreo.TabIndex = 8;
            this.labelCorreo.Text = "Correo electronico";
            // 
            // labelNombre
            // 
            this.labelNombre.AutoSize = true;
            this.labelNombre.Location = new System.Drawing.Point(34, 92);
            this.labelNombre.Name = "labelNombre";
            this.labelNombre.Size = new System.Drawing.Size(181, 28);
            this.labelNombre.TabIndex = 9;
            this.labelNombre.Text = "Nombre de usuario";
            // 
            // labelTitulo
            // 
            this.labelTitulo.AutoSize = true;
            this.labelTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 24F, System.Drawing.FontStyle.Bold);
            this.labelTitulo.Location = new System.Drawing.Point(34, 24);
            this.labelTitulo.Name = "labelTitulo";
            this.labelTitulo.Size = new System.Drawing.Size(305, 65);
            this.labelTitulo.TabIndex = 10;
            this.labelTitulo.Text = "Crear cuenta";
            // 
            // FormularioRegistro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(76)))), ((int)(((byte)(202)))));
            this.BackgroundImage = global::BubbleApp.Properties.Resources.fondo10;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(560, 560);
            this.Controls.Add(this.panelTarjeta);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormularioRegistro";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Registro";
            this.panelTarjeta.ResumeLayout(false);
            this.panelTarjeta.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label _statusLabel;
    }
}
