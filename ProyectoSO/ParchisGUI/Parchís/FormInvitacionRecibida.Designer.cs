namespace Parchís
{
    partial class InvitacionRecibida
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InvitacionRecibida));
            this.labelPregunta = new System.Windows.Forms.Label();
            this.bttn_aceptarInvitacion = new System.Windows.Forms.Button();
            this.bttn_rechazarInvitacion = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelPregunta
            // 
            this.labelPregunta.AutoSize = true;
            this.labelPregunta.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelPregunta.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPregunta.Location = new System.Drawing.Point(0, 0);
            this.labelPregunta.Name = "labelPregunta";
            this.labelPregunta.Size = new System.Drawing.Size(349, 20);
            this.labelPregunta.TabIndex = 0;
            this.labelPregunta.Text = "Has recibido una invitación para jugar con:";
            // 
            // bttn_aceptarInvitacion
            // 
            this.bttn_aceptarInvitacion.Location = new System.Drawing.Point(127, 163);
            this.bttn_aceptarInvitacion.Name = "bttn_aceptarInvitacion";
            this.bttn_aceptarInvitacion.Size = new System.Drawing.Size(233, 23);
            this.bttn_aceptarInvitacion.TabIndex = 2;
            this.bttn_aceptarInvitacion.Text = "ACEPTAR INVITACIÓN";
            this.bttn_aceptarInvitacion.UseVisualStyleBackColor = true;
            this.bttn_aceptarInvitacion.Click += new System.EventHandler(this.bttn_aceptarInvitacion_Click);
            // 
            // bttn_rechazarInvitacion
            // 
            this.bttn_rechazarInvitacion.Location = new System.Drawing.Point(387, 163);
            this.bttn_rechazarInvitacion.Name = "bttn_rechazarInvitacion";
            this.bttn_rechazarInvitacion.Size = new System.Drawing.Size(233, 23);
            this.bttn_rechazarInvitacion.TabIndex = 3;
            this.bttn_rechazarInvitacion.Text = "RECHAZAR INVITACIÓN";
            this.bttn_rechazarInvitacion.UseVisualStyleBackColor = true;
            this.bttn_rechazarInvitacion.Click += new System.EventHandler(this.bttn_rechazarInvitacion_Click);
            // 
            // InvitacionRecibida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 281);
            this.Controls.Add(this.bttn_rechazarInvitacion);
            this.Controls.Add(this.bttn_aceptarInvitacion);
            this.Controls.Add(this.labelPregunta);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InvitacionRecibida";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Invitación para Jugar!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPregunta;
        private System.Windows.Forms.Button bttn_aceptarInvitacion;
        private System.Windows.Forms.Button bttn_rechazarInvitacion;
    }
}