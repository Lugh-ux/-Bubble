namespace BubbleApp.Formularios
{
    partial class FormularioPerfil
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelTarjeta;
        private System.Windows.Forms.Label labelTitulo;
        private System.Windows.Forms.PictureBox _avatarPictureBox;
        private System.Windows.Forms.Button buttonCambiarFoto;
        private System.Windows.Forms.Label labelNombre;
        private System.Windows.Forms.TextBox _nameTextBox;
        private System.Windows.Forms.Label _statusLabel;
        private System.Windows.Forms.Button _guardarButton;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormularioPerfil));
            this.panelTarjeta = new System.Windows.Forms.Panel();
            this._guardarButton = new System.Windows.Forms.Button();
            this._statusLabel = new System.Windows.Forms.Label();
            this._nameTextBox = new System.Windows.Forms.TextBox();
            this.labelNombre = new System.Windows.Forms.Label();
            this.buttonCambiarFoto = new System.Windows.Forms.Button();
            this._avatarPictureBox = new System.Windows.Forms.PictureBox();
            this.labelTitulo = new System.Windows.Forms.Label();
            this.panelTarjeta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._avatarPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTarjeta
            // 
            this.panelTarjeta.BackColor = System.Drawing.Color.White;
            this.panelTarjeta.BackgroundImage = global::BubbleApp.Properties.Resources.animate_the_uploaded;
            this.panelTarjeta.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelTarjeta.Controls.Add(this._guardarButton);
            this.panelTarjeta.Controls.Add(this._statusLabel);
            this.panelTarjeta.Controls.Add(this._nameTextBox);
            this.panelTarjeta.Controls.Add(this.labelNombre);
            this.panelTarjeta.Controls.Add(this.buttonCambiarFoto);
            this.panelTarjeta.Controls.Add(this._avatarPictureBox);
            this.panelTarjeta.Controls.Add(this.labelTitulo);
            this.panelTarjeta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTarjeta.Location = new System.Drawing.Point(0, 0);
            this.panelTarjeta.Name = "panelTarjeta";
            this.panelTarjeta.Size = new System.Drawing.Size(465, 446);
            this.panelTarjeta.TabIndex = 0;
            // 
            // _guardarButton
            // 
            this._guardarButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(76)))), ((int)(((byte)(202)))));
            this._guardarButton.FlatAppearance.BorderSize = 0;
            this._guardarButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._guardarButton.ForeColor = System.Drawing.Color.White;
            this._guardarButton.Location = new System.Drawing.Point(56, 364);
            this._guardarButton.Name = "_guardarButton";
            this._guardarButton.Size = new System.Drawing.Size(340, 40);
            this._guardarButton.TabIndex = 0;
            this._guardarButton.Text = "Guardar cambios";
            this._guardarButton.UseVisualStyleBackColor = false;
            this._guardarButton.Click += new System.EventHandler(this._guardarButton_Click);
            // 
            // _statusLabel
            // 
            this._statusLabel.ForeColor = System.Drawing.Color.Firebrick;
            this._statusLabel.Location = new System.Drawing.Point(56, 367);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(340, 31);
            this._statusLabel.TabIndex = 1;
            // 
            // _nameTextBox
            // 
            this._nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._nameTextBox.Location = new System.Drawing.Point(56, 308);
            this._nameTextBox.Name = "_nameTextBox";
            this._nameTextBox.Size = new System.Drawing.Size(340, 34);
            this._nameTextBox.TabIndex = 2;
            // 
            // labelNombre
            // 
            this.labelNombre.AutoSize = true;
            this.labelNombre.Location = new System.Drawing.Point(56, 263);
            this.labelNombre.Name = "labelNombre";
            this.labelNombre.Size = new System.Drawing.Size(181, 28);
            this.labelNombre.TabIndex = 3;
            this.labelNombre.Text = "Nombre de usuario";
            // 
            // buttonCambiarFoto
            // 
            this.buttonCambiarFoto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCambiarFoto.Location = new System.Drawing.Point(163, 226);
            this.buttonCambiarFoto.Name = "buttonCambiarFoto";
            this.buttonCambiarFoto.Size = new System.Drawing.Size(142, 34);
            this.buttonCambiarFoto.TabIndex = 4;
            this.buttonCambiarFoto.Text = "Cambiar foto";
            this.buttonCambiarFoto.UseVisualStyleBackColor = true;
            this.buttonCambiarFoto.Click += new System.EventHandler(this.buttonCambiarFoto_Click);
            // 
            // _avatarPictureBox
            // 
            this._avatarPictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(247)))));
            this._avatarPictureBox.Location = new System.Drawing.Point(183, 97);
            this._avatarPictureBox.Name = "_avatarPictureBox";
            this._avatarPictureBox.Size = new System.Drawing.Size(110, 110);
            this._avatarPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._avatarPictureBox.TabIndex = 5;
            this._avatarPictureBox.TabStop = false;
            // 
            // labelTitulo
            // 
            this.labelTitulo.AutoSize = true;
            this.labelTitulo.BackColor = System.Drawing.Color.White;
            this.labelTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 24F, System.Drawing.FontStyle.Bold);
            this.labelTitulo.Location = new System.Drawing.Point(97, 20);
            this.labelTitulo.Name = "labelTitulo";
            this.labelTitulo.Size = new System.Drawing.Size(285, 65);
            this.labelTitulo.TabIndex = 6;
            this.labelTitulo.Text = "Editar perfil";
            // 
            // FormularioPerfil
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(24)))));
            this.ClientSize = new System.Drawing.Size(465, 446);
            this.Controls.Add(this.panelTarjeta);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormularioPerfil";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Editar perfil";
            this.panelTarjeta.ResumeLayout(false);
            this.panelTarjeta.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._avatarPictureBox)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
