using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parchís
{
    public partial class InvitacionRecibida : Form
    {
        string mensajerecivido;
        FormPartida formularioEnvia;
        public InvitacionRecibida(string jugadorAdmin, string mensaje, FormPartida formularioPadre)
        {
            InitializeComponent();
            mensajerecivido = mensaje;
            labelPregunta.Text = labelPregunta.Text + " "+ jugadorAdmin;
            formularioEnvia = formularioPadre;
        }

        private void bttn_aceptarInvitacion_Click(object sender, EventArgs e)
        {
            formularioEnvia.aceptarInvitacion(mensajerecivido);
            this.Close();
        }

        private void bttn_rechazarInvitacion_Click(object sender, EventArgs e)
        {
            formularioEnvia.rechazarInvitacion(mensajerecivido);
            this.Close();
        }
    }
}
