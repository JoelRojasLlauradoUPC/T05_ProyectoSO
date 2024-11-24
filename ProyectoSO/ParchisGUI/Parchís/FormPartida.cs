using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace Parchís
{
    public partial class FormPartida : Form
    {
        Socket server; //EN USO
        Thread atender; //EN USO
        string UsernamePlayer1; //EN USO
        string UsernamePlayer2; //EN USO
        string UsernamePlayer3; //EN USO
        string UsernamePlayer4; //EN USO
        bool pintarHomes = false; //EN USO
        string[] fichasPosicionRecibida; //EN USO
        bool game_started = false; //EN USO
        int current_Numeric_Pos = 0; //EN USO
        string[] enSalaEspera = null;//EN USO
        int currentGameID; //EN USO
        bool invitando = false; //EN USO
        bool isHost = false; //EN USO
        string localUsername = ""; //EN USO
        bool loginResult = true; //EN USO
        private bool threadRunning = false; //EN USO
        private bool server_on = false; //EN USO
        private readonly object socketLock = new object(); //EN USO
        int allowed_to_paint = 0;//EN USO
        double centroX1 = 0; //EN USO
        double centroY1 = 0; //EN USO
        double centroX2 = 0; //EN USO
        double centroY2 = 0; //EN USO
        string colorFichaDibujar = "Green"; //START THE VARIABLE WITH THE COLOR THAT HAS ID = 1
        int[] estadoCasillas = new int[75]; //EN USO
        int current_vector_pos = 0; //EN USO
        string[] trozos; //EN USO
        string currentUsername;
        // delegate void DelegadoParaEscribir(string mensaje);


        public FormPartida()
        {
            InitializeComponent();
            medioCentro.Paint += new PaintEventHandler(medioCentro_Paint);
            panelSeleccionPartidas.Dock = DockStyle.Fill;
            //CheckForIllegalCrossThreadCalls = false;
            panel_Bienvenida.Dock = DockStyle.Fill;
        }

        private void receiveThreadServer()
        {
            //MessageBox.Show("receiveThread SERVER FNCT STARTED");

            while (server_on == true)
            {
                // We receive the server's response
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                //MessageBox.Show(trozos[0]);
                string mensaje = trozos[1].Split('\0')[0];
                //MessageBox.Show(Convert.ToString(codigo));
                switch (codigo)
                {
                    case 1:
                        mensaje = mensaje.Split('\n')[0];
                        if (mensaje == "1")
                        {
                            MessageBox.Show("User created successfully.");
                            localUsername = UserRegisterBox.Text;
                            pasarelaLogin();
                        }
                        else if (mensaje == "0")
                        {
                            MessageBox.Show("The entered username is already registered.");
                        }
                        else
                        {
                            MessageBox.Show("There was an error creating the user. Please try again later.");
                        }
                        break;

                    case 2:
                        mensaje = mensaje.Split('\n')[0];
                        if (mensaje == "1")
                        {
                            localUsername = Username_TextBox.Text;
                            pasarelaLogin();    
                        }
                        else if (mensaje == "0")
                        {
                            MessageBox.Show("Log In failed.");
                        }
                        else
                        {
                            MessageBox.Show("There was an error with the log in. Please try again later.");
                        }
                        break;

                    case 7:
                        //MessageBox.Show("CASO 7.");
                        //DelegadoParaEscribir delegado = new DelegadoParaEscribir(PonConectados);
                        //ConnectedPlayersTextBox.Invoke(delegado, new object[] { mensaje });
                        //ConnectedPlayersTextBox.Text = mensaje;
                        ConnectedPlayersTextBox.Invoke((MethodInvoker)delegate
                        {
                            ConnectedPlayersTextBox.Text = mensaje;
                            if (invitando == true)
                            {
                                ponConectadosEnTablaInvitar(mensaje);
                            }
                            

                        });
                        break;

                    case 101:
                        string[] recievedMessage = mensaje.Split('/');
                        Invoke(new Action(() => chatViewerTextBox.Items.Add(recievedMessage[0])));
                        break;

                    case 201:
                        mensaje = mensaje.Split('\n')[0]; 
                        currentGameID = Convert.ToInt32(mensaje);
                        break;

                    case 211:
                        pasarelaNotificacionInvitacionRecibida(mensaje);
                        break;

                    case 221:
                        //MessageBox.Show(mensaje);
                        //chatInputTextBox.Text = mensaje;
                        pasarelaPonPlayersPanel(mensaje);
                        break;
                    case 231:
                        //MessageBox.Show(mensaje);
                        pasarelaIniciarJuego();
                        break;
                    case 241:
                        //MessageBox.Show(mensaje);
                        vectorPosicionRecibido(mensaje);
                        break;


                }
            }
        }

        private void pasarelaLogin()
        {
            this.Invoke((MethodInvoker)(() =>
            {
                panel_Bienvenida.Dock = DockStyle.Fill;
                panel_Bienvenida.Visible = false;
                //panelCasillas.Visible = true;
                //panelStatus.Visible = true;
                //panel_Estadisticas.Visible = true;
                panelSeleccionPartidas.Visible = true;
                ChatReceiverBox.Visible = true;

            }));
            if (server_on == true)
            {
                string mensaje = "7/"+localUsername;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                MessageBox.Show("Log In successful.");
            }
            else
            {
                MessageBox.Show("ACCESSED VIA PASARELA LOGIN WITHOUT LOGIN CREDENTIALS.\n SOME FUNCTIONALITIES MAY NOT BE AVAILABLE!");
            }
            
            
            loginResult = true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            CrearTablon(); //CREAR EL TABLÓN

            allowed_to_paint = 1; //PERMITIR GENERAR LA PARTE CENTRAL
            casilla_baseVerde.Invalidate();
            casilla_baseRoja.Invalidate();
            casilla_baseAzul.Invalidate();
            casilla_baseAmarilla.Invalidate();
            medioCentro.Invalidate();
            medioCentro.Refresh();
            //panelCasillas.Visible = true; //MOSTRAR EL FORMULARIO UNA VEZ SE HAYA TERMINADO DE GENERAR
        }

        private void CrearTablon()
        {
            //FUNCIÓN QUE AJUSTA EL TAMAÑO DE TODO EL TABLÓN


            //ajuste de los margenes y tablero
            margenSuperior.Height = Convert.ToInt32(Convert.ToDouble(gamePanel.Height) * 0.05);
            margenIzquierdo.Width = Convert.ToInt32(Convert.ToDouble(gamePanel.Width) * 0.05);
            panelCasillas.Width = panelCasillas.Height;

            //ajuste inicial de filas
            fila1.Height = Convert.ToInt32(Convert.ToDouble(panelCasillas.Height) * 0.30);
            fila2.Height = Convert.ToInt32(Convert.ToDouble(panelCasillas.Height) * 0.30);
            fila3.Height = Convert.ToInt32(Convert.ToDouble(panelCasillas.Height) * 0.30);

            //fila1
            casilla_baseRoja.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);
            casillasSuperiores.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);
            casilla_baseAzul.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);

            //ajuste de las columnas de las casillas superiores
            //columna1
            casillaSuperior1.Width = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Width) * 1 / 3);
            casilla35.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla36.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla37.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla38.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla39.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla40.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla41.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);

            //columna2
            casillaSuperior2.Width = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Width) * 1 / 3);
            casilla34.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla_mr1.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla_mr2.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla_mr3.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla_mr4.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla_mr5.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla_mr6.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);

            //columna3
            casillaSuperior3.Width = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Width) * 1 / 3);
            casilla33.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla32.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla31.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla30.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla29.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla28.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);
            casilla27.Height = Convert.ToInt32(Convert.ToDouble(casillasSuperiores.Height) * 1 / 7);



            //fila2
            medioIzquierda.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);
            medioCentro.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);
            medioDerecha.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);

            //fila2 - filaizquierda1
            casillaIzquierda1.Height = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Height) * 1 / 3);
            casilla44.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla45.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla46.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla47.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla48.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla49.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla50.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);

            //fila2 - filaizquierda2
            casillaIzquierda2.Height = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Height) * 1 / 3);
            casilla51.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla_mv1.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla_mv2.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla_mv3.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla_mv4.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla_mv5.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla_mv6.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);

            //fila2 - filaizquierda3
            casillaIzquierda3.Height = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Height) * 1 / 3);
            casilla52.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla53.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla54.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla55.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla56.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla57.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);
            casilla58.Width = Convert.ToInt32(Convert.ToDouble(medioIzquierda.Width) * 1 / 7);

            //FILAS CENTRALES
            centroSuperior.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 7);
            centroInferior.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 7);
            centroIzquierda.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 7);
            centroDerecha.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 7);
            //DIBUJAR GEOMETRIAS (VERTICES TRIANGULARES)
            casilla42.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 3);
            casilla_mr7.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 3);
            casilla26.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 3);

            casilla60.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 3);
            casilla_ma7.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 3);
            casilla8.Width = Convert.ToInt32(Convert.ToDouble(medioCentro.Width) * 1 / 3);

            casilla43.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 3) - (Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 7));
            casilla_mv7.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 3);
            casilla59.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 3) - (Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 7));

            casilla25.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 3) - (Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 7));
            casilla_mb7.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 3);
            casilla9.Height = Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 3) - (Convert.ToInt32(Convert.ToDouble(medioCentro.Height) * 1 / 7));



            //fila2 - filaderecha1
            casillaDerecha1.Height = Convert.ToInt32(Convert.ToDouble(medioDerecha.Height) * 1 / 3);
            casilla24.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla23.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla22.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla21.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla20.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla19.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla18.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);

            //fila2 - filaderecha2
            casillaDerecha2.Height = Convert.ToInt32(Convert.ToDouble(medioDerecha.Height) * 1 / 3);
            casilla17.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla_mb1.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla_mb2.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla_mb3.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla_mb4.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla_mb5.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla_mb6.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);

            //fila2 - filaderecha3
            casillaDerecha3.Height = Convert.ToInt32(Convert.ToDouble(medioDerecha.Height) * 1 / 3);
            casilla10.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla11.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla12.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla13.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla14.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla15.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);
            casilla16.Width = Convert.ToInt32(Convert.ToDouble(medioDerecha.Width) * 1 / 7);


            //fila3
            casilla_baseVerde.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);
            casillasInferiores.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);
            casilla_baseAmarilla.Width = Convert.ToInt32(Convert.ToDouble(panelCasillas.Width) * 0.30);

            //ajuste de las columnas de las casillas inferiores
            //columna1
            casillaInferior1.Width = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Width) * 1 / 3);
            casilla61.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla62.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla63.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla64.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla65.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla66.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla67.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);



            //columna2
            casillaInferior2.Width = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Width) * 1 / 3);
            casilla68.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla_ma1.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla_ma2.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla_ma3.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla_ma4.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla_ma5.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla_ma6.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);


            //columna3
            casillaInferior3.Width = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Width) * 1 / 3);
            casilla7.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla6.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla5.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla4.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla3.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla2.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
            casilla1.Height = Convert.ToInt32(Convert.ToDouble(casillasInferiores.Height) * 1 / 7);
        }

        private void medioCentro_Paint(object sender, PaintEventArgs e)
        {
            //FUNCIÓN QUE GENERA EL CENTRO DEL TABLÓN DEL PARCHÍS


            if (allowed_to_paint == 1)
            {

                using (Pen pen = new Pen(Color.Gray, 4)) // Grosor de 2 píxeles
                {
                    // Obtener el objeto Graphics del evento Paint
                    Graphics g = e.Graphics;

                    // Dibujar una línea diagonal desde (0,0) hasta (Width, Height)
                    g.DrawLine(pen, 0, 0, medioCentro.Width, medioCentro.Height);
                    g.DrawLine(pen, medioCentro.Width, 0, 0, medioCentro.Height);

                }

                //TRIANGULO ROJO
                Point[] puntos = new Point[]
                {
                    new Point(medioCentro.Width/7, medioCentro.Width/7),  // Punto A (vértice superior)
                    new Point(medioCentro.Width-medioCentro.Width/7, medioCentro.Height/7), // Punto B (vértice inferior derecho)
                    new Point(medioCentro.Width/2, medioCentro.Height/2)   // Punto C (vértice inferior izquierdo)
                };

                // Crear un objeto Pen para definir el color y grosor del borde
                using (Pen pen = new Pen(Color.Gray, 3)) // Borde negro de 3 píxeles
                {
                    // Dibujar el triángulo
                    e.Graphics.DrawPolygon(pen, puntos);
                }

                // Crear un objeto Brush para llenar el triángulo
                using (Brush brush = new SolidBrush(Color.Red)) // Relleno azul claro
                {
                    // Llenar el triángulo
                    e.Graphics.FillPolygon(brush, puntos);
                }

                //TRIANGULO AMARILLO
                Point[] puntos2 = new Point[]
                {
                    new Point(medioCentro.Width/7, medioCentro.Height - medioCentro.Width/7),  // Punto A (vértice superior)
                    new Point(medioCentro.Width-medioCentro.Width/7, medioCentro.Height - medioCentro.Height/7), // Punto B (vértice inferior derecho)
                    new Point(medioCentro.Width/2, medioCentro.Height/2)   // Punto C (vértice inferior izquierdo)
                };

                // Crear un objeto Pen para definir el color y grosor del borde
                using (Pen pen = new Pen(Color.Gray, 3)) // Borde negro de 3 píxeles
                {
                    // Dibujar el triángulo
                    e.Graphics.DrawPolygon(pen, puntos2);
                }

                // Crear un objeto Brush para llenar el triángulo
                using (Brush brush = new SolidBrush(Color.FromArgb(255, 255, 192))) // Relleno azul claro
                {
                    // Llenar el triángulo
                    e.Graphics.FillPolygon(brush, puntos2);
                }

                //TRIANGULO VERDE
                Point[] puntos3 = new Point[]
                {
                    new Point(medioCentro.Width/7, medioCentro.Height - medioCentro.Width/7),  // Punto A (vértice superior)
                    new Point(medioCentro.Width/7, medioCentro.Width/7), // Punto B (vértice inferior derecho)
                    new Point(medioCentro.Width/2, medioCentro.Height/2)   // Punto C (vértice inferior izquierdo)
                };

                // Crear un objeto Pen para definir el color y grosor del borde
                using (Pen pen = new Pen(Color.Gray, 3)) // Borde negro de 3 píxeles
                {
                    // Dibujar el triángulo
                    e.Graphics.DrawPolygon(pen, puntos3);
                }

                // Crear un objeto Brush para llenar el triángulo
                using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 128))) // Relleno azul claro
                {
                    // Llenar el triángulo
                    e.Graphics.FillPolygon(brush, puntos3);
                }

                //TRIANGULO AZUL
                Point[] puntos4 = new Point[]
                {
                    new Point(medioCentro.Width-medioCentro.Width/7, medioCentro.Height - medioCentro.Width/7),  // Punto A (vértice superior)
                    new Point(medioCentro.Width-medioCentro.Width/7, medioCentro.Width/7), // Punto B (vértice inferior derecho)
                    new Point(medioCentro.Width/2, medioCentro.Height/2)   // Punto C (vértice inferior izquierdo)
                };

                // Crear un objeto Pen para definir el color y grosor del borde
                using (Pen pen = new Pen(Color.Gray, 3)) // Borde negro de 3 píxeles
                {
                    // Dibujar el triángulo
                    e.Graphics.DrawPolygon(pen, puntos4);
                }

                // Crear un objeto Brush para llenar el triángulo
                using (Brush brush = new SolidBrush(Color.Blue)) // Relleno azul claro
                {
                    // Llenar el triángulo
                    e.Graphics.FillPolygon(brush, puntos4);
                }
            }
        }

        private void turnoJugador(int numeroJugador)
        {
            if (numeroJugador == 1)
            {
                turno_P1.Visible = true;
            }
            else if (numeroJugador == 2)
            {
                turno_P2.Visible = true;
            }
            else if (numeroJugador == 3)
            {
                turno_P3.Visible = true;
            }
            else if (numeroJugador == 4)
            {
                turno_P4.Visible = true;
            }
        }

        private void terminarTurnos()
        {
            turno_P1.Visible = false;
            turno_P2.Visible = false;
            turno_P3.Visible = false;
            turno_P4.Visible = false;
        }

        private double[] obtenerPosicion(int numeroCasilla, int colorFicha, int posicionVector)
        {

            double x1 = 0;
            double y1 = 0;
            double x2 = 0;
            double y2 = 0;

            int[] horizontal = { 1, 2, 3, 4, 5, 6, 7, 8, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 60, 61, 62, 63, 64, 65, 66, 67, 68 };
            int[] vertical = { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 };

            int i = 0;
            bool encontrado = false;
            bool cond_horizontal = false;
            bool cond_vertical = false;

            while ((i < 34) && (encontrado == false) && (numeroCasilla != 0)) //comprobamos si esta en el vector horizontal
            {
                
                if (numeroCasilla == horizontal[i])
                {
                    cond_horizontal = true;
                    encontrado = true;
                }
                else if (numeroCasilla == vertical[i])
                {
                    cond_vertical = true;
                    encontrado = true;
                }

                i = i + 1;

            }

            if (numeroCasilla != 0)
            {
                if (cond_horizontal == true)
                {
                    x1 = casilla1.Width / 4;
                    y1 = casilla1.Height / 2;
                    x2 = casilla1.Width - casilla1.Width / 4;
                    y2 = y1;

                }
                else if (cond_vertical == true)
                {
                    x1 = casilla23.Width / 2;
                    y1 = casilla23.Height / 4;
                    x2 = x1;
                    y2 = casilla23.Height - (casilla23.Height / 4);
                }
                else
                {
                    if ((colorFicha == 2) || (colorFicha == 4))
                    {
                        x1 = casilla1.Width / 4;
                        y1 = casilla1.Height / 2;
                        x2 = casilla1.Width / 4;
                        y2 = y1;
                    }
                    else
                    {
                        x1 = casilla23.Height / 4;
                        y1 = casilla23.Width / 2;
                        x2 = casilla23.Height / 4;
                        y2 = y1;
                    }
                }
            }

            double[] result = {x1, y1, x2, y2};

            return result;
        }

        private bool dibujarFicha(string sufijo)
        {
            string numeroCasilla = sufijo;
            string nombreCasillaADibujar = "casilla" + numeroCasilla;

            // Buscar el control por su nombre
            Control[] mapaCasillas = this.Controls.Find(nombreCasillaADibujar, true);

            if (mapaCasillas.Length > 0 && mapaCasillas[0] is Panel)
            {
                Panel miCasilla = (Panel)mapaCasillas[0];
                // Cambiar el color de fondo del panel
                //miCasilla.BackColor = Color.Gray;
                miCasilla.Refresh();
                miCasilla.Paint -= Panel_Paint;
            }

            return true;

        }

        private void vectorPosicionRecibido(string vector_sin_tratar)
        {
            estadoCasillas = new int[75];
            //int i = 69;
            //while (i<=75)
            //{
            //    estadoCasillas[i] = new int[4];

            //}
            FinalizarCasillas();
            
            int colorFicha = 0;
            trozos = vector_sin_tratar.Split('-');
            fichasPosicionRecibida = trozos;
            //MessageBox.Show(fichasPosicionRecibida.ToString());
            // Obtenemos el primer elemento
            current_vector_pos = 0;
            while (current_vector_pos < 16) //Para cada posición del vector
            {
                current_Numeric_Pos = Convert.ToInt32(trozos[current_vector_pos]);
                if ((current_vector_pos >= 0) && (current_vector_pos <= 3))
                {
                    colorFicha = 1;
                    colorFichaDibujar = "Green";
                }
                else if ((current_vector_pos >= 4) && (current_vector_pos <= 7))
                {
                    colorFicha = 2;
                    colorFichaDibujar = "DarkRed";
                }
                else if ((current_vector_pos >= 8) && (current_vector_pos <= 11))
                {
                    colorFicha = 3;
                    colorFichaDibujar = "DarkBlue";
                }
                else
                {
                    colorFicha = 4;
                    colorFichaDibujar = "Yellow";
                }
                double[] result = obtenerPosicion(Convert.ToInt32(trozos[current_vector_pos]), colorFicha, current_vector_pos);
                centroX1 = result[0];
                centroY1 = result[1];
                centroX2 = result[2];
                centroY2 = result[3];
                string sufijo;
                int posicionNumerica = Convert.ToInt32(trozos[current_vector_pos]);
                if (posicionNumerica > 68)
                {
                    string sufijoNumerico = Convert.ToString(posicionNumerica - 68);
                    if (colorFicha == 1)
                    {
                        sufijo = "_mv" + sufijoNumerico;
                    }
                    else if (colorFicha == 2)
                    {
                        sufijo = "_mr" + sufijoNumerico;
                    }
                    else if (colorFicha == 3)
                    {
                        sufijo = "_mb" + sufijoNumerico;
                    }
                    else //color 4
                    {
                        sufijo = "_ma" + sufijoNumerico;
                    }
                }
                else
                {
                    sufijo = trozos[current_vector_pos];
                }

                if (current_Numeric_Pos != 0)
                {
                    InicializarCasillas(sufijo);
                    current_Numeric_Pos = posicionNumerica;
                    dibujarFicha(sufijo);
                    estadoCasillas[Convert.ToInt32(trozos[current_vector_pos])] = estadoCasillas[Convert.ToInt32(trozos[current_vector_pos])] + 1;
                }
                current_vector_pos = current_vector_pos + 1;
            }
            pintarHomes = true;
            casilla_baseVerde.Refresh();
            casilla_baseRoja.Refresh();
            casilla_baseAzul.Refresh();
            casilla_baseAmarilla.Refresh();
            pintarHomes = false;
        }


        private void InicializarCasillas(string sufijo_casilla)
        {
            // Formar el nombre del panel dinámicamente
            string nombrePanel = "casilla" + sufijo_casilla;

            // Buscar el panel por su nombre
            Control[] paneles = this.Controls.Find(nombrePanel, true);

            if (paneles.Length > 0 && paneles[0] is Panel)
            {
                Panel panel = (Panel)paneles[0];
                // Suscribir el evento Paint a cada panel encontrado
                panel.Paint += Panel_Paint;
            }

        }

        private void FinalizarCasillas() //Borra todas las fichas de las casillas
        {
            for (int i = 0; i <= 68; i++)
            {
                // Bucle para recorrer los paneles numerados de "casilla1" a "casilla68"
                // Formar el nombre del panel dinámicamente
                string nombrePanel = "casilla" + i;

                // Buscar el panel por su nombre
                Control[] paneles = this.Controls.Find(nombrePanel, true);
                if (paneles.Length > 0 && paneles[0] is Panel)
                {
                    Panel panel = (Panel)paneles[0];
                    // Suscribir el evento Paint a cada panel encontrado
                    panel.Paint -= Panel_Paint;
                    panel.Refresh();
                }
            }

            int a = 1;
            while (a <= 4)
            {
                // Bucle para recorrer los paneles numerados de "casilla_x1" a "casilla_x7"
                // Formar el nombre del panel dinámicamente
                string id_color = "";
                if (a == 1)
                {
                    id_color = "v";
                }
                else if (a == 2)
                {
                    id_color = "r";
                }
                else if (a == 3)
                {
                    id_color = "3";
                }
                else
                {
                    id_color = "a";
                }

                int b = 1;
                while (b <= 7)
                {
                    string nombrePanel = "casilla_" + a + b;

                    // Buscar el panel por su nombre
                    Control[] paneles = this.Controls.Find(nombrePanel, true);

                    if (paneles.Length > 0 && paneles[0] is Panel)
                    {
                        Panel panel = (Panel)paneles[0];
                        // Suscribir el evento Paint a cada panel encontrado
                        panel.Paint -= Panel_Paint;
                        panel.Refresh();
                    }

                    b = b + 1;
                }

                a = a + 1;
            }

        }


        // Método que dibuja dentro del panel, llamado por el evento Paint


        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            if (current_Numeric_Pos != 0)
            {

                Panel panel = sender as Panel;  // Obtener el panel que disparó el evento

                if (panel != null)
                {
                    Graphics g = e.Graphics;

                    // Dibujar una elipse centrada dentro del panel
                    int width = panel.Width;
                    int height = panel.Height;

                    // Definir el tamaño y la posición de la elipse
                    int diameter = Math.Min(width, height) - 10;
                    int x = Convert.ToInt32(centroX1 - diameter / 2);
                    int y = Convert.ToInt32(centroY1 - diameter / 2);



                    // Dibujar un círculo
                    Color colorDibujar = Color.FromName(colorFichaDibujar);
                    SolidBrush pincel = new SolidBrush(colorDibujar);
                    g.FillEllipse(pincel, x, y, diameter, diameter);
                    using (Pen pen = new Pen(colorDibujar))
                    {
                        e.Graphics.DrawEllipse(pen, x, y, diameter, diameter);
                    }

                    if (current_Numeric_Pos != 0)
                    {

                    
                        if (estadoCasillas[Convert.ToInt32(trozos[current_vector_pos])] == 1)
                        {
                            string numeroCasilla = trozos[current_vector_pos];
                            int i = 0;
                            int vez = 0;
                            while (i < 16)
                            {
                                int colorFicha = 0;
                                if ((i >= 0) && (i <= 3))
                                {
                                    colorFicha = 1;
                                    colorFichaDibujar = "Green";
                                }
                                else if ((i >= 4) && (i <= 7))
                                {
                                    colorFicha = 2;
                                    colorFichaDibujar = "DarkRed";
                                }
                                else if ((i >= 8) && (i <= 11))
                                {
                                    colorFicha = 3;
                                    colorFichaDibujar = "DarkBlue";
                                }
                                else
                                {
                                    colorFicha = 4;
                                    colorFichaDibujar = "Yellow";
                                }
                                if ((trozos[i] == numeroCasilla) && (vez == 0))
                                {
                                    // Dibujar una elipse centrada dentro del panel
                                    width = panel.Width;
                                    height = panel.Height;

                                    // Definir el tamaño y la posición de la elipse
                                    diameter = Math.Min(width, height) - 10;
                                    x = Convert.ToInt32(centroX1 - diameter / 2);
                                    y = Convert.ToInt32(centroY1 - diameter / 2);



                                    // Dibujar un círculo con lápiz negro
                                    colorDibujar = Color.FromName(colorFichaDibujar);
                                    pincel = new SolidBrush(colorDibujar);
                                    g.FillEllipse(pincel, x, y, diameter, diameter);
                                    using (Pen pen = new Pen(colorDibujar))
                                    {
                                        e.Graphics.DrawEllipse(pen, x, y, diameter, diameter);
                                    }
                                    vez = 1;
                                }
                                else if ((trozos[i] == numeroCasilla) && (vez == 1))
                                {
                                    if (1 == 1)
                                    {
                                        // Dibujar una elipse centrada dentro del panel
                                        width = panel.Width;
                                        height = panel.Height;

                                        // Definir el tamaño y la posición de la elipse
                                        diameter = Math.Min(width, height) - 10;
                                        x = Convert.ToInt32(centroX2 - diameter / 2);
                                        y = Convert.ToInt32(centroY2 - diameter / 2);



                                        // Dibujar un círculo con lápiz negro
                                        colorDibujar = Color.FromName(colorFichaDibujar);
                                        pincel = new SolidBrush(colorDibujar);
                                        g.FillEllipse(pincel, x, y, diameter, diameter);
                                        using (Pen pen = new Pen(colorDibujar))
                                        {
                                            e.Graphics.DrawEllipse(pen, x, y, diameter, diameter);
                                        }
                                        vez = 1;
                                    }
                                }

                                i = i + 1;

                            }
                        }
                    }
                }
            }
        }

        private void btt_pos_send_Click(object sender, EventArgs e)
        {
            string numeroCasilla = pos_rec.Text;
            vectorPosicionRecibido(numeroCasilla);
        }

        private void LogIn_Button_Click(object sender, EventArgs e)
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: LogIn_Button_Click                                                                              |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the user login process. If the connection is successful, it sends the login request  |
        // | and processes the server's response.                                                                      |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Displays a message indicating whether the connection to the server was successful or not.         |
        // |-----------------------------------------------------------------------------------------------------------|
        {

            if (server_on == false) //Si no esta el servidor iniciado lo iniciamos
            {
                CreateServerConnection();
            }
            if (server_on == true)
            {
                // Log In
                string mensaje = "2/" + Username_TextBox.Text + "/" + Password_TextBox.Text;
                // We send the entered username and password to the server
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else
            {
                MessageBox.Show("FAILURE IN LoginButton: Server connection unreachable.");
            }
            

        }


        private void Disconnect_bttn_Click(object sender, EventArgs e)
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: Disconnect_bttn_Click                                                                           |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the user disconnect process. It sends a request to the server to disconnect the user |
        // | and processes the server's response.                                                                      |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Displays a message indicating the user has been disconnected from the server.                     |
        // |-----------------------------------------------------------------------------------------------------------|
        {
            // Disconnect
            string mensaje = "0/";
            // We send just the code of the query to the server
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            // Disconnection
            atender.Abort();

            // We receive the server's response
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

            this.BackColor = Color.White;
            MessageBox.Show("The user has disconnected from the server.");
        }

        public void CreateServerConnection()
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse(IPBox.Text);
            IPEndPoint ipep = new IPEndPoint(direc, Convert.ToInt32(PortBox.Text));


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //MessageBox.Show("OBJETO SOCKET CREADO");

            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                server_on = true;
                this.BackColor = Color.LightGreen;
                //MessageBox.Show("Conectado");
                //pongo en marcha el thread que atenderá los mensajes del servidor
                ThreadStart ts = delegate { receiveThreadServer(); };
                atender = new Thread(ts);
                atender.Start();

            }
            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                server_on = false;
                return;
            }

        }

        private void Register_Button_Click(object sender, EventArgs e)
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: Register_Button_Click                                                                           |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the user registration process. If the required fields are filled and valid, it       |
        // | connects to the server via a socket, sends the registration request, and processes the server's response. |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Displays a success or error message depending on whether or not the user can be registered.       |
        // |-----------------------------------------------------------------------------------------------------------|
        {
            labelError.Visible = false;
            
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

            if (puntos_verificacion == 4) // If the username and passwords meet the requirements
            {
                if (server_on == false) //Si no esta el servidor iniciado lo iniciamos
                {
                    CreateServerConnection();
                }

                try
                {
                    // Attempt to connect the socket to the server

                    string mensaje = "1/" + UserRegisterBox.Text + "/" + PassRegisterBox.Text;
                    // We send the entered username and password to the server
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    server_on = true;
                    


                }
                catch (SocketException)
                {
                    // If there is an exception, an error is printed and the program exits with return. 
                    MessageBox.Show("Connection to the server failed.");
                }

                
            }

            else
            {
                labelError.Visible = true;
            }
        }
        private void ReceiveMessages()
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: ReceiveMessages                                                                                 |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Continuously listens for incoming messages from the server and displays them in the chat.    |
        // | If an error occurs (e.g., connection loss), it handles the exception and stops listening.                 |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input: None                                                                                               |
        // | Output:                                                                                                   |
        // |  - Displays incoming messages in the chat list box (ChatListBox).                                         |
        // |  - Handles connection errors by showing an error message and exiting the loop.                            |
        // |-----------------------------------------------------------------------------------------------------------|
        {
            // Displays the received message
            while (threadRunning)
            {
                try
                {
                    byte[] msg2 = new byte[80];
                    if (server.Poll(1000, SelectMode.SelectRead))
                    {
                        lock (socketLock)
                        {
                            int bytesReceived = server.Receive(msg2);
                            if (bytesReceived > 0)
                            {
                                string mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                                string[] recievedMessage = mensaje.Split('/');
                                if (recievedMessage[0] == "100a")
                                {
                                    Invoke(new Action(() => chatViewerTextBox.Items.Add(recievedMessage[1])));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error receiving message: " + ex.Message);
                    break;
                }
                Thread.Sleep(100);
            }
        }
        private void startChat()
        {
            // |-----------------------------------------------------------------------------------------------------------|
            // | Function: start_chat_bttn_Click                                                                           |
            // |-----------------------------------------------------------------------------------------------------------|
            // | Description: Manages the thread that listens for incoming messages from the server. If a listening thread |
            // | is already active, it safely stops it before starting a new one.                                          |
            // |-----------------------------------------------------------------------------------------------------------|
            // | Input:                                                                                                    |
            // |  - object sender: The source of the event (button click).                                                 |
            // |  - EventArgs e: The event data.                                                                           |
            // | Output:                                                                                                   |
            // |  - Starts a new thread to receive messages if none is currently running.                                  |
            // |-----------------------------------------------------------------------------------------------------------|
            {
                // We check if the thread is already running
                if (atender != null && atender.IsAlive)
                {
                    // If it is running, we try to stop it
                    threadRunning = false;
                    atender.Join();
                }

                // We start the new thread
                atender = new Thread(ReceiveMessages);
                atender.IsBackground = true;
                threadRunning = true;
                atender.Start();
            }
        }

        private void messageSenderButton_Click_1(object sender, EventArgs e)
        {
            // |-----------------------------------------------------------------------------------------------------------|
            // | Function: SendToAll_Click                                                                                 |
            // |-----------------------------------------------------------------------------------------------------------|
            // | Description: Sends a message to all connected players and updates the chat box to reflect the message.    |
            // |-----------------------------------------------------------------------------------------------------------|
            // | Input:                                                                                                    |
            // |  - object sender: The source of the event (button click).                                                 |
            // |  - EventArgs e: Event arguments for the click event.                                                      |
            // | Output:                                                                                                   |
            // |  - Sends the message to the server for broadcast to all players.                                          |
            // |  - Updates the local chat window with the sent message.                                                   |
            // |-----------------------------------------------------------------------------------------------------------|
            {
                pasarelaSendChatMessage();

            }
        }

        private void pasarelaSendChatMessage()
        {
            if (server_on == true)
            {
                // Send a message to all the players
                // We send just the code of the query to the server
                if (chatInputTextBox.Text != "")
                {
                    string mensaje = "100/" + Username_TextBox.Text + "/" + chatInputTextBox.Text;
                    byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    chatInputTextBox.Clear();
                }
            }
            else
            {
                MessageBox.Show("Host unreachable, connection not set.");
            }
        }

        private void btt_pos_send_Click_1(object sender, EventArgs e)
        {
            string numeroCasilla = pos_rec.Text;
            vectorPosicionRecibido(numeroCasilla);
        }


        private void bttnPasarela_Click(object sender, EventArgs e)
        {
            pasarelaLogin();
        }

        private void chatInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verificar si la tecla presionada es Enter
            {
                e.SuppressKeyPress = true; // Opcional: evita el sonido de "ding"
                pasarelaSendChatMessage();
            }
        }

        private void bttn_crearPartida_Click(object sender, EventArgs e)
        {
            

            if (server_on == true)
            {
                //panelSeleccionPartidas.Visible = false;
                //panelCasillas.Visible = true;
                SeleccionarOpcionPanel.Visible = false;
                panel_infoSeleccionar.Visible = false;
                panel_invitarJugadores.Visible = true;
                labelInvitar.Visible = true;
                dataGridInvitar.Visible = true;
                panelStatus.Visible = true;
                panel_Estadisticas.Visible = true;
                labelName_P1.Text = localUsername;
                isHost = true;
                invitando = true;
                // Evitar que el usuario cambie el tamaño de las columnas
                dataGridInvitar.AllowUserToResizeColumns = false;
                dataGridInvitar.BorderStyle = BorderStyle.None;

                // Evitar que el usuario cambie el tamaño de las filas
                dataGridInvitar.AllowUserToResizeRows = false;
                string mensaje = "200/" + localUsername;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                ponConectadosEnTablaInvitar(ConnectedPlayersTextBox.Text);

            }
            else
            {
                MessageBox.Show("ERROR: Connection with the server must be active before creating a new game.");
            }

        }

        private void ponConectadosEnTablaInvitar(string vectorRecividoConectados)
        {
            //MessageBox.Show("LLENANDO TABLA");
            // Desactivar la fila vacía al final
            dataGridInvitar.AllowUserToAddRows = false;

            // Limpiar las filas anteriores
            dataGridInvitar.Rows.Clear();

            // Separar el string recibido
            string[] trozosConectados = vectorRecividoConectados.Split('-');
            int maxConectados = trozosConectados.Length;
            int currentRegisterInsertion = 0;

            // Bucle while para recorrer el array
            while (currentRegisterInsertion < maxConectados)
            {
                string usuario = trozosConectados[currentRegisterInsertion];

                // Verificar que el trozo no esté vacío ni sea el usuario local
                if (!string.IsNullOrEmpty(usuario) && usuario != localUsername && usuario != 7.ToString())
                {
                    // Añadir una nueva fila solo si el nombre de usuario es válido
                    int rowIndex = dataGridInvitar.Rows.Add();
                    dataGridInvitar.Rows[rowIndex].Cells[0].Value = usuario;
                    dataGridInvitar.Rows[rowIndex].Cells[1].Value = "PULSA PARA INVITAR";
                    dataGridInvitar.Rows[rowIndex].Cells[1].Style.Font = new Font(dataGridInvitar.Font, FontStyle.Bold);
                    dataGridInvitar.Rows[rowIndex].Cells[1].Style.BackColor = Color.Gray;
                }

                // Incrementar el índice para continuar con el siguiente elemento
                currentRegisterInsertion++;
            }
            if (enSalaEspera != null)
            {
                actualizarTablaInvitaciones(enSalaEspera);
            }
        }



        private void centroInferior_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridInvitar_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica si la celda que fue clickeada es la que contiene "PULSA PARA INVITAR"
            if (e.RowIndex >= 0 && e.ColumnIndex == 1)  // Columna 1 (índice basado en 0)
            {
                if (dataGridInvitar.Rows[e.RowIndex].Cells[1].Value == "INVITACIÓN RECHAZADA")
                {
                    MessageBox.Show("No puedes invitarle ya que ha rechazado tu invitación.");
                }
                else if (dataGridInvitar.Rows[e.RowIndex].Cells[1].Value == "ESPERANDO RESPUESTA...")
                {
                    MessageBox.Show("Ya le has invitado. Espera un momento...");
                }
                else if (dataGridInvitar.Rows[e.RowIndex].Cells[1].Value == "INVITACIÓN ACEPTADA")
                {
                    MessageBox.Show("No puedes invitarle de nuevo. Ya está en la sala.");
                }
                else
                {
                    dataGridInvitar.Rows[e.RowIndex].Cells[1].Value = "ESPERANDO RESPUESTA...";
                    dataGridInvitar.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.LightYellow;
                    string playerToInviteUsername = dataGridInvitar.Rows[e.RowIndex].Cells[0].Value.ToString();
                    pasarelaInvitar(playerToInviteUsername);
                }

                
            }
        }

        private void pasarelaInvitar(string playerToInviteUsername)
        {
            string localGameID = currentGameID.ToString();
            string mensaje = "210/" + localUsername + "-" + playerToInviteUsername + "-" + localGameID+"/";
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }
        private void pasarelaNotificacionInvitacionRecibida(string mensaje)
        {
            string[] partes = mensaje.Split('-');

            // Obtener el primer nombre
            string jugadorAdmin = partes[0];
            // Crear una instancia de Formulario2
            InvitacionRecibida FormInvite = new InvitacionRecibida(jugadorAdmin, mensaje, this);

            // Mostrar Formulario2 de manera modal
            FormInvite.ShowDialog();
        }

        public void aceptarInvitacion(string mensajeInvitacion)
        {
            //MessageBox.Show("ACEPTADA");
            string[] partes = mensajeInvitacion.Split('-');

            // Obtener el primer nombre
            string idPartida = partes[2];

            string mensaje = "220/" + localUsername + "-" + idPartida;
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }

        public void rechazarInvitacion(string mensaje)
        {
            MessageBox.Show("RECHAZADA");
        }

        private void pasarelaPonPlayersPanel(string mensajeRecivido)
        {
            string[] partes = mensajeRecivido.Split('-');
            labelName_P1.Text = partes[0];

            if (localUsername != partes[0]) // Si no eres el host
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    SeleccionarOpcionPanel.Visible = false;
                    panel_infoSeleccionar.Visible = false;
                    panel_invitarJugadores.Visible = false;
                    labelInvitar.Visible = false;
                    dataGridInvitar.Visible = false;
                    panelStatus.Visible = true;
                    panel_Estadisticas.Visible = true;
                }));
            }
            else // Si eres el host
            {
                enSalaEspera = partes;
                actualizarTablaInvitaciones(partes);

                // Mostrar el botón de iniciar partida si hay al menos dos jugadores
                if (partes.Length >= 2)
                {
                    buttonIniciarPartida.Visible = true;
                }
                else
                {
                    buttonIniciarPartida.Visible = false;
                }
            }

            // Actualizar nombres de jugadores según la cantidad recibida
            if (partes.Length > 1)
            {
                labelName_P2.Text = partes[1];
                UsernamePlayer1 = partes[0];
                UsernamePlayer2 = partes[1];
            }
            if (partes.Length > 2)
            {
                labelName_P3.Text = partes[2];
                UsernamePlayer3 = partes[2];

            }
            if (partes.Length > 3)
            {
                labelName_P4.Text = partes[3];
                UsernamePlayer4 = partes[3];
            }
        }


        private void actualizarTablaInvitaciones(string[] partes)
        {
            int currentRow = 0;
            while ((currentRow < dataGridInvitar.RowCount))
            {
                int currentPiece = 0;
                while ((currentPiece < partes.Length))
                {
                    string dato1 = partes[currentPiece];
                    string dato2 = dataGridInvitar.Rows[currentRow].Cells[0].Value.ToString();

                    if (dato1 == dato2) //EL JUGADOR ESTA EN PARTIDA
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            dataGridInvitar.Rows[currentRow].Cells[1].Value = "INVITACIÓN ACEPTADA";
                            dataGridInvitar.Rows[currentRow].Cells[1].Style.BackColor = Color.LightGreen;
                        }));
                    }
                    currentPiece++;
                }

                currentRow++;
            }
        }

        private void pasarelaIniciarJuego()
        {
            this.Invoke((MethodInvoker)(() =>
            {
                panelCasillas.Visible = true;
                turno_P1.Visible = true;
                groupBox_P1.BackColor = Color.LightGreen;
            }));
        }

        private void casilla_baseVerde_Paint(object sender, PaintEventArgs e)
        {
            if (pintarHomes == true)
            {
                // Definimos las variables una sola vez fuera del bucle
                int currentPos = 0;
                int maxPos = 3;
                int x1 = 0;
                int y1 = 0;

                Graphics g = e.Graphics;  // Usamos el objeto Graphics una sola vez para todo el proceso
                int width = casilla1.Width;  // Dimensiones de la casilla
                int height = casilla1.Height;
                int diameter = Math.Min(width, height) - 10;  // Calculamos el diámetro de la elipse

                // Recorremos las posiciones del array
                while (currentPos <= maxPos)
                {
                    if (fichasPosicionRecibida[currentPos] == "0")  // Solo procesamos si la ficha está vacía
                    {
                        // Determinamos la posición (x1, y1) dependiendo del valor de currentPos
                        switch (currentPos)
                        {
                            case 0:
                            case 4:
                            case 8:
                            case 12:
                                x1 = casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height / 4;
                                break;
                            case 1:
                            case 5:
                            case 9:
                            case 13:
                                x1 = casilla_baseVerde.Width - casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height / 4;
                                break;
                            case 2:
                            case 6:
                            case 10:
                            case 14:
                                x1 = casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height - casilla_baseVerde.Height / 4;
                                break;
                            case 3:
                            case 7:
                            case 11:
                            case 15:
                                x1 = casilla_baseVerde.Width - casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height - casilla_baseVerde.Height / 4;
                                break;
                        }
                        colorFichaDibujar = "Green";
                        // Definir las coordenadas para la elipse centrada
                        int x = Convert.ToInt32(x1 - diameter / 2);
                        int y = Convert.ToInt32(y1 - diameter / 2);

                        // Dibujar la elipse
                        Color colorDibujar = Color.FromName(colorFichaDibujar);
                        using (SolidBrush pincel = new SolidBrush(colorDibujar))  // Usar 'using' para liberar recursos
                        {
                            g.FillEllipse(pincel, x, y, diameter, diameter);  // Llenar el círculo
                        }

                        // Dibujar el contorno de la elipse
                        using (Pen pen = new Pen(colorDibujar))
                        {
                            g.DrawEllipse(pen, x, y, diameter, diameter);  // Dibuja el contorno
                        }
                    }

                    currentPos++;  // Avanzamos al siguiente índice
                }
                
            }
        }

        private void casilla_baseRoja_Paint(object sender, PaintEventArgs e)
        {
            if (pintarHomes == true)
            {
                // Definimos las variables una sola vez fuera del bucle
                int currentPos = 4;
                int maxPos = 7;
                int x1 = 0;
                int y1 = 0;

                Graphics g = e.Graphics;  // Usamos el objeto Graphics una sola vez para todo el proceso
                int width = casilla1.Width;  // Dimensiones de la casilla
                int height = casilla1.Height;
                int diameter = Math.Min(width, height) - 10;  // Calculamos el diámetro de la elipse

                // Recorremos las posiciones del array
                while (currentPos <= maxPos)
                {
                    if (fichasPosicionRecibida[currentPos] == "0")  // Solo procesamos si la ficha está vacía
                    {
                        // Determinamos la posición (x1, y1) dependiendo del valor de currentPos
                        switch (currentPos)
                        {
                            case 0:
                            case 4:
                            case 8:
                            case 12:
                                x1 = casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height / 4;
                                break;
                            case 1:
                            case 5:
                            case 9:
                            case 13:
                                x1 = casilla_baseVerde.Width - casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height / 4;
                                break;
                            case 2:
                            case 6:
                            case 10:
                            case 14:
                                x1 = casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height - casilla_baseVerde.Height / 4;
                                break;
                            case 3:
                            case 7:
                            case 11:
                            case 15:
                                x1 = casilla_baseVerde.Width - casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height - casilla_baseVerde.Height / 4;
                                break;
                        }
                        colorFichaDibujar = "DarkRed";
                        // Definir las coordenadas para la elipse centrada
                        int x = Convert.ToInt32(x1 - diameter / 2);
                        int y = Convert.ToInt32(y1 - diameter / 2);

                        // Dibujar la elipse
                        Color colorDibujar = Color.FromName(colorFichaDibujar);
                        using (SolidBrush pincel = new SolidBrush(colorDibujar))  // Usar 'using' para liberar recursos
                        {
                            g.FillEllipse(pincel, x, y, diameter, diameter);  // Llenar el círculo
                        }

                        // Dibujar el contorno de la elipse
                        using (Pen pen = new Pen(colorDibujar))
                        {
                            g.DrawEllipse(pen, x, y, diameter, diameter);  // Dibuja el contorno
                        }
                    }

                    currentPos++;  // Avanzamos al siguiente índice
                }
                
            }
        }

        private void casilla_baseAzul_Paint(object sender, PaintEventArgs e)
        {
            if (pintarHomes == true)
            {
                // Definimos las variables una sola vez fuera del bucle
                int currentPos = 8;
                int maxPos = 11;
                int x1 = 0;
                int y1 = 0;

                Graphics g = e.Graphics;  // Usamos el objeto Graphics una sola vez para todo el proceso
                int width = casilla1.Width;  // Dimensiones de la casilla
                int height = casilla1.Height;
                int diameter = Math.Min(width, height) - 10;  // Calculamos el diámetro de la elipse

                // Recorremos las posiciones del array
                while (currentPos <= maxPos)
                {
                    if (fichasPosicionRecibida[currentPos] == "0")  // Solo procesamos si la ficha está vacía
                    {
                        // Determinamos la posición (x1, y1) dependiendo del valor de currentPos
                        switch (currentPos)
                        {
                            case 0:
                            case 4:
                            case 8:
                            case 12:
                                x1 = casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height / 4;
                                break;
                            case 1:
                            case 5:
                            case 9:
                            case 13:
                                x1 = casilla_baseVerde.Width - casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height / 4;
                                break;
                            case 2:
                            case 6:
                            case 10:
                            case 14:
                                x1 = casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height - casilla_baseVerde.Height / 4;
                                break;
                            case 3:
                            case 7:
                            case 11:
                            case 15:
                                x1 = casilla_baseVerde.Width - casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height - casilla_baseVerde.Height / 4;
                                break;
                        }
                        colorFichaDibujar = "DarkBlue";
                        // Definir las coordenadas para la elipse centrada
                        int x = Convert.ToInt32(x1 - diameter / 2);
                        int y = Convert.ToInt32(y1 - diameter / 2);

                        // Dibujar la elipse
                        Color colorDibujar = Color.FromName(colorFichaDibujar);
                        using (SolidBrush pincel = new SolidBrush(colorDibujar))  // Usar 'using' para liberar recursos
                        {
                            g.FillEllipse(pincel, x, y, diameter, diameter);  // Llenar el círculo
                        }

                        // Dibujar el contorno de la elipse
                        using (Pen pen = new Pen(colorDibujar))
                        {
                            g.DrawEllipse(pen, x, y, diameter, diameter);  // Dibuja el contorno
                        }
                    }

                    currentPos++;  // Avanzamos al siguiente índice
                }
            }
        }

        private void casilla_baseAmarilla_Paint(object sender, PaintEventArgs e)
        {
            if (pintarHomes == true)
            {
                // Definimos las variables una sola vez fuera del bucle
                int currentPos = 12;
                int maxPos = 15;
                int x1 = 0;
                int y1 = 0;

                Graphics g = e.Graphics;  // Usamos el objeto Graphics una sola vez para todo el proceso
                int width = casilla1.Width;  // Dimensiones de la casilla
                int height = casilla1.Height;
                int diameter = Math.Min(width, height) - 10;  // Calculamos el diámetro de la elipse

                // Recorremos las posiciones del array
                while (currentPos <= maxPos)
                {
                    if (fichasPosicionRecibida[currentPos] == "0")  // Solo procesamos si la ficha está vacía
                    {
                        // Determinamos la posición (x1, y1) dependiendo del valor de currentPos
                        switch (currentPos)
                        {
                            case 0:
                            case 4:
                            case 8:
                            case 12:
                                x1 = casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height / 4;
                                break;
                            case 1:
                            case 5:
                            case 9:
                            case 13:
                                x1 = casilla_baseVerde.Width - casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height / 4;
                                break;
                            case 2:
                            case 6:
                            case 10:
                            case 14:
                                x1 = casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height - casilla_baseVerde.Height / 4;
                                break;
                            case 3:
                            case 7:
                            case 11:
                            case 15:
                                x1 = casilla_baseVerde.Width - casilla_baseVerde.Width / 4;
                                y1 = casilla_baseVerde.Height - casilla_baseVerde.Height / 4;
                                break;
                        }
                        colorFichaDibujar = "LightYellow";
                        // Definir las coordenadas para la elipse centrada
                        int x = Convert.ToInt32(x1 - diameter / 2);
                        int y = Convert.ToInt32(y1 - diameter / 2);

                        // Dibujar la elipse
                        Color colorDibujar = Color.FromName(colorFichaDibujar);
                        using (SolidBrush pincel = new SolidBrush(colorDibujar))  // Usar 'using' para liberar recursos
                        {
                            g.FillEllipse(pincel, x, y, diameter, diameter);  // Llenar el círculo
                        }

                        // Dibujar el contorno de la elipse
                        using (Pen pen = new Pen(colorDibujar))
                        {
                            g.DrawEllipse(pen, x, y, diameter, diameter);  // Dibuja el contorno
                        }
                    }

                    currentPos++;  // Avanzamos al siguiente índice
                }
            }
        }

        private void dadoP1_Click(object sender, EventArgs e)
        {
            if (localUsername == UsernamePlayer1)
            {
                turno_P1.Visible = false;
                groupBox_P1.BackColor = Color.FromArgb(51, 153, 255); // Azul similar al "ActiveCaption"
                Random random = new Random();

                // Generar un número aleatorio entre 1 y 6 (inclusive)
                int numeroAleatorio = random.Next(1, 7);

                string mensaje = "300/" + currentGameID+ "-1-" + numeroAleatorio;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                turnoJugador(2);


            }
        }

        private void dadoP2_Click(object sender, EventArgs e)
        {
            if (localUsername == UsernamePlayer2)
            {
                turno_P2.Visible = false;
                groupBox_P2.BackColor = Color.FromArgb(51, 153, 255); // Azul similar al "ActiveCaption"
                Random random = new Random();

                // Generar un número aleatorio entre 1 y 6 (inclusive)
                int numeroAleatorio = random.Next(1, 7);

                string mensaje = "300/" + currentGameID + "-2-" + numeroAleatorio;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                turnoJugador(3);

            }
        }

        private void dadoP3_Click(object sender, EventArgs e)
        {
            if (localUsername == UsernamePlayer3)
            {
                turno_P3.Visible = false;
                groupBox_P3.BackColor = Color.FromArgb(51, 153, 255); // Azul similar al "ActiveCaption"
                Random random = new Random();

                // Generar un número aleatorio entre 1 y 6 (inclusive)
                int numeroAleatorio = random.Next(1, 7);

                string mensaje = "300/" + currentGameID + "-3-" + numeroAleatorio;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                turnoJugador(4);

            }
        }

        private void dadoP4_Click(object sender, EventArgs e)
        {
            if (localUsername == UsernamePlayer4)
            {
                turno_P4.Visible = false;
                groupBox_P4.BackColor = Color.FromArgb(51, 153, 255); // Azul similar al "ActiveCaption"
                Random random = new Random();

                // Generar un número aleatorio entre 1 y 6 (inclusive)
                int numeroAleatorio = random.Next(1, 7);

                string mensaje = "300/" + currentGameID + "-4-" + numeroAleatorio;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                turnoJugador(1);

            }
        }

        private void buttonIniciarPartida_Click(object sender, EventArgs e)
        {
            if (!game_started)
            {
                string mensaje = "230/" + currentGameID;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                game_started = true;
                labelInvitar.Visible = false;
                dataGridInvitar.Visible = false;
                MessageBox.Show("Partida iniciada por " + localUsername);
            }


        }
      
      
    }

}