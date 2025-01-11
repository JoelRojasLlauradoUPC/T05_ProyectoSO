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
        // |-------------------------------------------------------------------------------------------------------------|
        // | Function: bttn_aceptarInvitacion_Click                                                                      |
        // |-------------------------------------------------------------------------------------------------------------|
        // | Description: This event handler is triggered when the "Aceptar Invitación" button is clicked. It invokes    |
        // | the aceptarInvitacion method on the formularioEnvia object, passing the received message (mensajerecivido)  |
        // | as a parameter to handle the acceptance logic. Once the invitation is processed, the current form is closed.|
        // |-------------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                      |
        // |  - sender: The object that triggered the event (not used in this case).                                     |
        // |  - e: EventArgs containing the event data (not used in this case).                                          |
        // | Output:                                                                                                     |
        // |  - None. The method performs UI actions and communicates with another component.                            |
        // |-------------------------------------------------------------------------------------------------------------|
            formularioEnvia.aceptarInvitacion(mensajerecivido);
            this.Close();
        }

        private void bttn_rechazarInvitacion_Click(object sender, EventArgs e)
        {
        // |------------------------------------------------------------------------------------------------------------|
        // | Function: bttn_rechazarInvitacion_Click                                                                    |
        // |------------------------------------------------------------------------------------------------------------|
        // | Description: This event handler is triggered when the "Rechazar Invitación" button is clicked. It invokes  |
        // | the rechazarInvitacion method on the formularioEnvia object, passing the received message (mensajerecivido)|
        // | as a parameter to handle the rejection logic. Once the rejection is processed, the current form is closed. |
        // |------------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                     |
        // |  - sender: The object that triggered the event (not used in this case).                                    |
        // |  - e: EventArgs containing the event data (not used in this case).                                         |
        // | Output:                                                                                                    |
        // |  - None. The method performs UI actions and communicates with another component.                           |
        // |------------------------------------------------------------------------------------------------------------|
            formularioEnvia.rechazarInvitacion(mensajerecivido);
            this.Close();
        }
    }
}
