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
    public partial class FormRango : Form
    {
        public FormRango(DateTime fechaFirst, DateTime FechaLast, string mensajeRecivido, FormPartida formularioPadre)
        {
            InitializeComponent();
            label_fechas.Text = "entre el "+fechaFirst.ToString()+" y el "+ FechaLast.ToString();

            string[] trozos = mensajeRecivido.Split('|');

            int numeroFilas = trozos.Length;

            int filaActual = 0;

            while (filaActual < numeroFilas)
            {
                string[] filaTroceada = trozos[filaActual].Split('=');
                dataGridPartidas.Rows.Add();
                dataGridPartidas.Rows[filaActual].Cells[0].Value = filaTroceada[0];
                dataGridPartidas.Rows[filaActual].Cells[1].Value = filaTroceada[1];
                dataGridPartidas.Rows[filaActual].Cells[2].Value = filaTroceada[2];


                filaActual++;
            }

        }

        private void FormRango_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: button1_Click                                                                                   |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event in this form.                                                        |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - sender: Object representing the event sender.                                                          |
        // |  - e: EventArgs containing the event data.                                                                |
        // | Output:                                                                                                   |
        // |  - Closes the current form or window.                                                                     |
        // |-----------------------------------------------------------------------------------------------------------|
            Close();
        }
    }
}
