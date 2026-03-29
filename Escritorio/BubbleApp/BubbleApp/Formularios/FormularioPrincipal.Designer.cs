namespace BubbleApp.Formularios
{
    partial class FormularioPrincipal
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelSuperior;
        private System.Windows.Forms.Label labelLogo;
        private System.Windows.Forms.Button buttonCerrarSesion;
        private System.Windows.Forms.TableLayoutPanel tablaPrincipal;
        private System.Windows.Forms.Panel panelRadar;
        private System.Windows.Forms.Label labelRadar;
        private System.Windows.Forms.ListView _radarList;
        private System.Windows.Forms.Panel panelMapa;
        private BubbleApp.Controles.ControlMapaRadar _mapControl;
        private System.Windows.Forms.Panel panelPerfil;
        private System.Windows.Forms.PictureBox _avatarPictureBox;
        private System.Windows.Forms.Label _userNameLabel;
        private System.Windows.Forms.Label _emailLabel;
        private System.Windows.Forms.Button buttonEditarPerfil;
        private System.Windows.Forms.Label _bubbleCountLabel;
        private System.Windows.Forms.Label labelUbicacion;
        private System.Windows.Forms.Label labelLatitud;
        private System.Windows.Forms.TextBox _latitudeTextBox;
        private System.Windows.Forms.Label labelLongitud;
        private System.Windows.Forms.TextBox _longitudeTextBox;
        private System.Windows.Forms.Button buttonActualizarRadar;
        private System.Windows.Forms.Button buttonUsarUbicacionActual;
        private System.Windows.Forms.CheckBox _bubbleToggle;
        private System.Windows.Forms.Label _statusLabel;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormularioPrincipal));
            this.panelSuperior = new System.Windows.Forms.Panel();
            this.buttonCerrarSesion = new System.Windows.Forms.Button();
            this.labelLogo = new System.Windows.Forms.Label();
            this.tablaPrincipal = new System.Windows.Forms.TableLayoutPanel();
            this.panelRadar = new System.Windows.Forms.Panel();
            this._radarList = new System.Windows.Forms.ListView();
            this.labelRadar = new System.Windows.Forms.Label();
            this.panelMapa = new System.Windows.Forms.Panel();
            this.panelPerfil = new System.Windows.Forms.Panel();
            this._statusLabel = new System.Windows.Forms.Label();
            this._bubbleToggle = new System.Windows.Forms.CheckBox();
            this.buttonActualizarRadar = new System.Windows.Forms.Button();
            this.buttonUsarUbicacionActual = new System.Windows.Forms.Button();
            this._longitudeTextBox = new System.Windows.Forms.TextBox();
            this.labelLongitud = new System.Windows.Forms.Label();
            this._latitudeTextBox = new System.Windows.Forms.TextBox();
            this.labelLatitud = new System.Windows.Forms.Label();
            this.labelUbicacion = new System.Windows.Forms.Label();
            this._bubbleCountLabel = new System.Windows.Forms.Label();
            this.buttonEditarPerfil = new System.Windows.Forms.Button();
            this._emailLabel = new System.Windows.Forms.Label();
            this._userNameLabel = new System.Windows.Forms.Label();
            this._avatarPictureBox = new System.Windows.Forms.PictureBox();
            this._mapControl = new BubbleApp.Controles.ControlMapaRadar();
            this.panelSuperior.SuspendLayout();
            this.tablaPrincipal.SuspendLayout();
            this.panelRadar.SuspendLayout();
            this.panelMapa.SuspendLayout();
            this.panelPerfil.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._avatarPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSuperior
            // 
            this.panelSuperior.BackColor = System.Drawing.Color.White;
            this.panelSuperior.Controls.Add(this.buttonCerrarSesion);
            this.panelSuperior.Controls.Add(this.labelLogo);
            this.panelSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSuperior.Location = new System.Drawing.Point(0, 0);
            this.panelSuperior.Name = "panelSuperior";
            this.panelSuperior.Size = new System.Drawing.Size(1400, 72);
            this.panelSuperior.TabIndex = 1;
            // 
            // buttonCerrarSesion
            // 
            this.buttonCerrarSesion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCerrarSesion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCerrarSesion.ForeColor = System.Drawing.Color.Firebrick;
            this.buttonCerrarSesion.Location = new System.Drawing.Point(1217, 12);
            this.buttonCerrarSesion.Name = "buttonCerrarSesion";
            this.buttonCerrarSesion.Size = new System.Drawing.Size(130, 38);
            this.buttonCerrarSesion.TabIndex = 0;
            this.buttonCerrarSesion.Text = "Cerrar sesion";
            this.buttonCerrarSesion.UseVisualStyleBackColor = true;
            this.buttonCerrarSesion.Click += new System.EventHandler(this.buttonCerrarSesion_Click);
            // 
            // labelLogo
            // 
            this.labelLogo.AutoSize = true;
            this.labelLogo.Font = new System.Drawing.Font("Segoe UI Semibold", 22F, System.Drawing.FontStyle.Bold);
            this.labelLogo.Location = new System.Drawing.Point(12, 9);
            this.labelLogo.Name = "labelLogo";
            this.labelLogo.Size = new System.Drawing.Size(179, 60);
            this.labelLogo.TabIndex = 1;
            this.labelLogo.Text = "Bubble ";
            this.labelLogo.Click += new System.EventHandler(this.labelLogo_Click);
            // 
            // tablaPrincipal
            // 
            this.tablaPrincipal.BackgroundImage = global::BubbleApp.Properties.Resources.fondo10;
            this.tablaPrincipal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tablaPrincipal.ColumnCount = 3;
            this.tablaPrincipal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tablaPrincipal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablaPrincipal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tablaPrincipal.Controls.Add(this.panelRadar, 0, 0);
            this.tablaPrincipal.Controls.Add(this.panelMapa, 1, 0);
            this.tablaPrincipal.Controls.Add(this.panelPerfil, 2, 0);
            this.tablaPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablaPrincipal.Location = new System.Drawing.Point(0, 72);
            this.tablaPrincipal.Name = "tablaPrincipal";
            this.tablaPrincipal.Padding = new System.Windows.Forms.Padding(20);
            this.tablaPrincipal.RowCount = 1;
            this.tablaPrincipal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablaPrincipal.Size = new System.Drawing.Size(1400, 788);
            this.tablaPrincipal.TabIndex = 0;
            // 
            // panelRadar
            // 
            this.panelRadar.BackColor = System.Drawing.Color.White;
            this.panelRadar.Controls.Add(this._radarList);
            this.panelRadar.Controls.Add(this.labelRadar);
            this.panelRadar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRadar.Location = new System.Drawing.Point(20, 20);
            this.panelRadar.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.panelRadar.Name = "panelRadar";
            this.panelRadar.Size = new System.Drawing.Size(310, 748);
            this.panelRadar.TabIndex = 0;
            // 
            // _radarList
            // 
            this._radarList.FullRowSelect = true;
            this._radarList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this._radarList.HideSelection = false;
            this._radarList.Location = new System.Drawing.Point(16, 62);
            this._radarList.Name = "_radarList";
            this._radarList.Size = new System.Drawing.Size(285, 720);
            this._radarList.TabIndex = 0;
            this._radarList.UseCompatibleStateImageBehavior = false;
            this._radarList.View = System.Windows.Forms.View.Details;
            // 
            // labelRadar
            // 
            this.labelRadar.AutoSize = true;
            this.labelRadar.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold);
            this.labelRadar.Location = new System.Drawing.Point(20, 18);
            this.labelRadar.Name = "labelRadar";
            this.labelRadar.Size = new System.Drawing.Size(115, 48);
            this.labelRadar.TabIndex = 1;
            this.labelRadar.Text = "Radar";
            // 
            // panelMapa
            // 
            this.panelMapa.BackColor = System.Drawing.Color.White;
            this.panelMapa.Controls.Add(this._mapControl);
            this.panelMapa.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMapa.Location = new System.Drawing.Point(350, 20);
            this.panelMapa.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.panelMapa.Name = "panelMapa";
            this.panelMapa.Size = new System.Drawing.Size(700, 748);
            this.panelMapa.TabIndex = 1;
            // 
            // panelPerfil
            // 
            this.panelPerfil.BackColor = System.Drawing.Color.White;
            this.panelPerfil.Controls.Add(this._statusLabel);
            this.panelPerfil.Controls.Add(this._bubbleToggle);
            this.panelPerfil.Controls.Add(this.buttonActualizarRadar);
            this.panelPerfil.Controls.Add(this.buttonUsarUbicacionActual);
            this.panelPerfil.Controls.Add(this._longitudeTextBox);
            this.panelPerfil.Controls.Add(this.labelLongitud);
            this.panelPerfil.Controls.Add(this._latitudeTextBox);
            this.panelPerfil.Controls.Add(this.labelLatitud);
            this.panelPerfil.Controls.Add(this.labelUbicacion);
            this.panelPerfil.Controls.Add(this._bubbleCountLabel);
            this.panelPerfil.Controls.Add(this.buttonEditarPerfil);
            this.panelPerfil.Controls.Add(this._emailLabel);
            this.panelPerfil.Controls.Add(this._userNameLabel);
            this.panelPerfil.Controls.Add(this._avatarPictureBox);
            this.panelPerfil.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPerfil.Location = new System.Drawing.Point(1073, 23);
            this.panelPerfil.Name = "panelPerfil";
            this.panelPerfil.Size = new System.Drawing.Size(304, 742);
            this.panelPerfil.TabIndex = 2;
            // 
            // _statusLabel
            // 
            this._statusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this._statusLabel.Location = new System.Drawing.Point(28, 610);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(244, 90);
            this._statusLabel.TabIndex = 0;
            // 
            // _bubbleToggle
            // 
            this._bubbleToggle.AutoSize = true;
            this._bubbleToggle.Location = new System.Drawing.Point(30, 572);
            this._bubbleToggle.Name = "_bubbleToggle";
            this._bubbleToggle.Size = new System.Drawing.Size(161, 32);
            this._bubbleToggle.TabIndex = 1;
            this._bubbleToggle.Text = "Burbuja activa";
            this._bubbleToggle.CheckedChanged += new System.EventHandler(this.BubbleToggle_CheckedChanged);
            // 
            // buttonActualizarRadar
            // 
            this.buttonActualizarRadar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(76)))), ((int)(((byte)(202)))));
            this.buttonActualizarRadar.FlatAppearance.BorderSize = 0;
            this.buttonActualizarRadar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonActualizarRadar.ForeColor = System.Drawing.Color.White;
            this.buttonActualizarRadar.Location = new System.Drawing.Point(30, 520);
            this.buttonActualizarRadar.Name = "buttonActualizarRadar";
            this.buttonActualizarRadar.Size = new System.Drawing.Size(240, 36);
            this.buttonActualizarRadar.TabIndex = 2;
            this.buttonActualizarRadar.Text = "Actualizar radar";
            this.buttonActualizarRadar.UseVisualStyleBackColor = false;
            this.buttonActualizarRadar.Click += new System.EventHandler(this.buttonActualizarRadar_Click);
            // 
            // buttonUsarUbicacionActual
            // 
            this.buttonUsarUbicacionActual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUsarUbicacionActual.Location = new System.Drawing.Point(30, 520);
            this.buttonUsarUbicacionActual.Name = "buttonUsarUbicacionActual";
            this.buttonUsarUbicacionActual.Size = new System.Drawing.Size(240, 36);
            this.buttonUsarUbicacionActual.TabIndex = 3;
            this.buttonUsarUbicacionActual.Text = "Usar mi ubicacion";
            this.buttonUsarUbicacionActual.UseVisualStyleBackColor = true;
            this.buttonUsarUbicacionActual.Click += new System.EventHandler(this.buttonUsarUbicacionActual_Click);
            // 
            // _longitudeTextBox
            // 
            this._longitudeTextBox.Location = new System.Drawing.Point(30, 474);
            this._longitudeTextBox.Name = "_longitudeTextBox";
            this._longitudeTextBox.Size = new System.Drawing.Size(240, 34);
            this._longitudeTextBox.TabIndex = 4;
            this._longitudeTextBox.Text = "-3.7037";
            // 
            // labelLongitud
            // 
            this.labelLongitud.AutoSize = true;
            this.labelLongitud.Location = new System.Drawing.Point(30, 452);
            this.labelLongitud.Name = "labelLongitud";
            this.labelLongitud.Size = new System.Drawing.Size(91, 28);
            this.labelLongitud.TabIndex = 5;
            this.labelLongitud.Text = "Longitud";
            // 
            // _latitudeTextBox
            // 
            this._latitudeTextBox.Location = new System.Drawing.Point(30, 414);
            this._latitudeTextBox.Name = "_latitudeTextBox";
            this._latitudeTextBox.Size = new System.Drawing.Size(240, 34);
            this._latitudeTextBox.TabIndex = 6;
            this._latitudeTextBox.Text = "40.4167";
            // 
            // labelLatitud
            // 
            this.labelLatitud.AutoSize = true;
            this.labelLatitud.Location = new System.Drawing.Point(30, 392);
            this.labelLatitud.Name = "labelLatitud";
            this.labelLatitud.Size = new System.Drawing.Size(73, 28);
            this.labelLatitud.TabIndex = 7;
            this.labelLatitud.Text = "Latitud";
            // 
            // labelUbicacion
            // 
            this.labelUbicacion.AutoSize = true;
            this.labelUbicacion.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelUbicacion.Location = new System.Drawing.Point(28, 356);
            this.labelUbicacion.Name = "labelUbicacion";
            this.labelUbicacion.Size = new System.Drawing.Size(149, 32);
            this.labelUbicacion.TabIndex = 8;
            this.labelUbicacion.Text = "Tu ubicacion";
            // 
            // _bubbleCountLabel
            // 
            this._bubbleCountLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold);
            this._bubbleCountLabel.Location = new System.Drawing.Point(24, 296);
            this._bubbleCountLabel.Name = "_bubbleCountLabel";
            this._bubbleCountLabel.Size = new System.Drawing.Size(250, 24);
            this._bubbleCountLabel.TabIndex = 9;
            this._bubbleCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonEditarPerfil
            // 
            this.buttonEditarPerfil.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEditarPerfil.Location = new System.Drawing.Point(60, 236);
            this.buttonEditarPerfil.Name = "buttonEditarPerfil";
            this.buttonEditarPerfil.Size = new System.Drawing.Size(180, 38);
            this.buttonEditarPerfil.TabIndex = 10;
            this.buttonEditarPerfil.Text = "Editar perfil";
            this.buttonEditarPerfil.UseVisualStyleBackColor = true;
            this.buttonEditarPerfil.Click += new System.EventHandler(this.buttonEditarPerfil_Click);
            // 
            // _emailLabel
            // 
            this._emailLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(108)))), ((int)(((byte)(108)))));
            this._emailLabel.Location = new System.Drawing.Point(24, 194);
            this._emailLabel.Name = "_emailLabel";
            this._emailLabel.Size = new System.Drawing.Size(250, 22);
            this._emailLabel.TabIndex = 11;
            this._emailLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _userNameLabel
            // 
            this._userNameLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 20F, System.Drawing.FontStyle.Bold);
            this._userNameLabel.Location = new System.Drawing.Point(24, 160);
            this._userNameLabel.Name = "_userNameLabel";
            this._userNameLabel.Size = new System.Drawing.Size(250, 34);
            this._userNameLabel.TabIndex = 12;
            this._userNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _avatarPictureBox
            // 
            this._avatarPictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(247)))));
            this._avatarPictureBox.Location = new System.Drawing.Point(95, 30);
            this._avatarPictureBox.Name = "_avatarPictureBox";
            this._avatarPictureBox.Size = new System.Drawing.Size(110, 110);
            this._avatarPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._avatarPictureBox.TabIndex = 13;
            this._avatarPictureBox.TabStop = false;
            // 
            // _mapControl
            // 
            this._mapControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(17)))), ((int)(((byte)(24)))));
            this._mapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mapControl.Location = new System.Drawing.Point(0, 0);
            this._mapControl.Name = "_mapControl";
            this._mapControl.Size = new System.Drawing.Size(700, 748);
            this._mapControl.TabIndex = 0;
            // 
            // FormularioPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(19)))), ((int)(((byte)(19)))));
            this.BackgroundImage = global::BubbleApp.Properties.Resources.fondo10;
            this.ClientSize = new System.Drawing.Size(1400, 860);
            this.Controls.Add(this.tablaPrincipal);
            this.Controls.Add(this.panelSuperior);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormularioPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bubble Desktop";
            this.panelSuperior.ResumeLayout(false);
            this.panelSuperior.PerformLayout();
            this.tablaPrincipal.ResumeLayout(false);
            this.panelRadar.ResumeLayout(false);
            this.panelRadar.PerformLayout();
            this.panelMapa.ResumeLayout(false);
            this.panelPerfil.ResumeLayout(false);
            this.panelPerfil.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._avatarPictureBox)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
