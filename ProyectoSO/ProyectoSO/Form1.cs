using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoSO
{
    public partial class WelcomeForm : Form
    {

        Socket server;

        public WelcomeForm()
        {
            InitializeComponent();
        }

        private void Register_Button_Click(object sender, EventArgs e)
        {
            labelError.Visible = false;
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9050);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int puntos_verificacion = 0;
            if (UserRegisterBox.Text != "")
            {
                puntos_verificacion = puntos_verificacion + 1;
            }

            if (PassRegisterBox.Text != "")
            {
                puntos_verificacion = puntos_verificacion + 1;
            }

            if (RepeatRegisterBox.Text != "")
            {
                puntos_verificacion = puntos_verificacion + 1;
            }

            if (PassRegisterBox.Text == RepeatRegisterBox.Text)
            {
                puntos_verificacion = puntos_verificacion + 1;
            }

            if (puntos_verificacion == 4) //Si nombre de usuario y contraseñas cumplen con los requisitos
            {

                try
                {
                    server.Connect(ipep);//Intentamos conectar el socket
                    this.BackColor = Color.Green;


                    if (puntos_verificacion == 4)
                    {
                        // Quiere saber la longitud
                        string mensaje = "1/" + UserRegisterBox.Text + "/" + PassRegisterBox.Text;
                        // Enviamos al servidor el nombre tecleado
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);

                        //Recibimos la respuesta del servidor
                        byte[] msg2 = new byte[80];
                        server.Receive(msg2);
                        mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                        if (mensaje == "1")
                        {
                            MessageBox.Show("Usuario creado satisfactoriamente. Puede iniciar sesión");
                        }
                        else if (mensaje == "2")
                        {
                            MessageBox.Show("El nombre de usuario introducido ya se encuentra registrado.");
                        }
                        else
                        {
                            MessageBox.Show("Ha habido un error creando su usuario. Inténtelo más tarde.");
                        }
                        
                    }


                    // Se terminó el servicio. 
                    // Nos desconectamos
                    server.Shutdown(SocketShutdown.Both);
                    server.Close();



                }
                catch (SocketException)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    return;
                }
            }
            else
            {
                labelError.Visible = true;
            }
        }


    }
}
