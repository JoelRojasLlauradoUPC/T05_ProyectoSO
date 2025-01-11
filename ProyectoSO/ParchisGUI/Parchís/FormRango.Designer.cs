namespace Parchís
{
    partial class FormRango
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRango));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_titulo = new System.Windows.Forms.Label();
            this.label_fechas = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.dataGridPartidas = new System.Windows.Forms.DataGridView();
            this.nombreJugador = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnAccion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.c3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bttn_close = new System.Windows.Forms.Button();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPartidas)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1070, 50);
            this.panel1.TabIndex = 7;
            // 
            // label_titulo
            // 
            this.label_titulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_titulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_titulo.Location = new System.Drawing.Point(0, 50);
            this.label_titulo.Name = "label_titulo";
            this.label_titulo.Size = new System.Drawing.Size(1070, 23);
            this.label_titulo.TabIndex = 8;
            this.label_titulo.Text = "LISTADO DE PARTIDAS JUGADAS";
            this.label_titulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_fechas
            // 
            this.label_fechas.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_fechas.Location = new System.Drawing.Point(0, 73);
            this.label_fechas.Name = "label_fechas";
            this.label_fechas.Size = new System.Drawing.Size(1070, 23);
            this.label_fechas.TabIndex = 9;
            this.label_fechas.Text = "ENTRE: ";
            this.label_fechas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 96);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1070, 25);
            this.panel2.TabIndex = 10;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 121);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(75, 556);
            this.panel3.TabIndex = 11;
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(995, 121);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(75, 556);
            this.panel4.TabIndex = 12;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.bttn_close);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(75, 627);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(920, 50);
            this.panel5.TabIndex = 14;
            // 
            // dataGridPartidas
            // 
            this.dataGridPartidas.AllowUserToAddRows = false;
            this.dataGridPartidas.AllowUserToDeleteRows = false;
            this.dataGridPartidas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridPartidas.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dataGridPartidas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridPartidas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nombreJugador,
            this.columnAccion,
            this.c3});
            this.dataGridPartidas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridPartidas.Location = new System.Drawing.Point(75, 121);
            this.dataGridPartidas.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridPartidas.Name = "dataGridPartidas";
            this.dataGridPartidas.ReadOnly = true;
            this.dataGridPartidas.RowHeadersVisible = false;
            this.dataGridPartidas.RowHeadersWidth = 51;
            this.dataGridPartidas.RowTemplate.Height = 24;
            this.dataGridPartidas.Size = new System.Drawing.Size(920, 506);
            this.dataGridPartidas.TabIndex = 15;
            // 
            // nombreJugador
            // 
            this.nombreJugador.HeaderText = "ID de partida";
            this.nombreJugador.MinimumWidth = 6;
            this.nombreJugador.Name = "nombreJugador";
            this.nombreJugador.ReadOnly = true;
            // 
            // columnAccion
            // 
            this.columnAccion.HeaderText = "Nombre Jugador Administrador";
            this.columnAccion.MinimumWidth = 6;
            this.columnAccion.Name = "columnAccion";
            this.columnAccion.ReadOnly = true;
            // 
            // c3
            // 
            this.c3.HeaderText = "Fecha de Inicio de la partida";
            this.c3.Name = "c3";
            this.c3.ReadOnly = true;
            // 
            // bttn_close
            // 
            this.bttn_close.Location = new System.Drawing.Point(313, 15);
            this.bttn_close.Name = "bttn_close";
            this.bttn_close.Size = new System.Drawing.Size(295, 25);
            this.bttn_close.TabIndex = 0;
            this.bttn_close.Text = "Cerrar Ventana";
            this.bttn_close.UseVisualStyleBackColor = true;
            this.bttn_close.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormRango
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 677);
            this.Controls.Add(this.dataGridPartidas);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label_fechas);
            this.Controls.Add(this.label_titulo);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormRango";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Partidas Jugadas";
            this.Load += new System.EventHandler(this.FormRango_Load);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPartidas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_titulo;
        private System.Windows.Forms.Label label_fechas;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button bttn_close;
        private System.Windows.Forms.DataGridView dataGridPartidas;
        private System.Windows.Forms.DataGridViewTextBoxColumn nombreJugador;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnAccion;
        private System.Windows.Forms.DataGridViewTextBoxColumn c3;
    }
}