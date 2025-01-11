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
using System.Runtime.InteropServices;


namespace Parchís
{
    public partial class FormPartida : Form
    {
        // Importa la función para obtener el menú del sistema.
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        // Importa la función para eliminar elementos del menú.
        [DllImport("user32.dll")]
        private static extern bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        // Constantes para identificar el botón de cerrar en el menú.
        private const uint SC_CLOSE = 0xF060;
        private const uint MF_BYCOMMAND = 0x00000000;

        // Este método se ejecuta cuando el formulario se carga.
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Obtén el menú del sistema del formulario.
            IntPtr hMenu = GetSystemMenu(this.Handle, false);

            // Elimina el botón de cerrar del menú.
            DeleteMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
        }


        bool global_f1 = false;
        bool global_f2 = false;
        bool global_f3 = false;
        bool global_f4 = false;
        string global_conectados;
        int maxPositionToEnd = 75;
        bool lobby = true;
        string[] global_temp_TokenTrozos;
        string[] global_temp_currentUserTokens;
        int global_temp_currentTurnID;
        int global_sg1 = 0; //EN USO
        int global_sg2 = 0; //EN USO
        int global_sg3 = 0; //EN USO
        int global_sg4 = 0; //EN USO
        string currentTurn = "0"; //EN USO
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
            iniciarSelectoresHora();
        }

        private void receiveThreadServer()
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: receiveThreadServer                                                                             |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the reception of messages from the server in a continuous loop while the server is   |
        // | active. Processes received messages based on predefined message codes and takes appropriate actions.      |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - None (relies on global variables and the server connection).                                           |
        // | Output: Executes various actions based on server responses, including updating UI elements, handling      |
        // | user authentication, managing game state, and processing chat messages.                                   |
        // |-----------------------------------------------------------------------------------------------------------|

            while (server_on == true)
            {
                // We receive the server's response
                byte[] msg2 = new byte[1000];
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
                            this.Invoke((MethodInvoker)(() =>
                            {
                                UserRegisterBox.Text = "";
                                PassRegisterBox.Text = "";
                                RepeatRegisterBox.Text = "";
                            }));
                        }
                        else if (mensaje == "0")
                        {
                            MessageBox.Show("The entered username is already registered.");
                        }
                        else
                        {
                            MessageBox.Show("There was an error creating the user. Please try again later.");
                        }
                        // Disconnect
                        string mensaje2 = "0/";
                        // We send just the code of the query to the server
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje2);
                        server.Send(msg);

                        // Disconnection
                        

                        server_on = false;

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
                    
                    case 3:
                        mensaje = mensaje.Split('\n')[0];
                        if (mensaje == "0")
                        {
                            MessageBox.Show("User could not be deleted from the database.");
                        }
                        else
                        {
                            MessageBox.Show("User deleted successfully from the database.");

                            pasarelaDesconectar();

                        }
                        break;

                    case 4:
                        if (trozos.Length >= 2)
                        {
                            string resultado = trozos[1];

                            if (resultado == "0")
                            {
                                MessageBox.Show("List of players with whom you have played could not be generated.");
                            }
                            else if (resultado == "1")
                            {
                                string listPlayers = string.Join("\n", trozos.Skip(2));
                                MessageBox.Show("List of players with whom you have played: \n" + listPlayers);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid response format received from server.");
                        }
                        break;

                    case 7:
                        this.Invoke((MethodInvoker)(() =>
                        {
                            global_conectados = mensaje;
                            if (invitando == true)
                            {
                                ponConectadosEnTablaInvitar(mensaje);
                            }

                            if (lobby == true)
                            {
                                labelConnected.Visible = true;
                                bttn_deleteAccount.Visible = true;
                                ConnectedPlayersTextBox.Visible = true;
                                ponPanelConectados(mensaje);
                            }
                            else
                            {
                                labelConnected.Visible = false;
                                bttn_deleteAccount.Visible = false;
                                ConnectedPlayersTextBox.Visible = false;
                            }
                        }));
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
                        pasarelaIniciarJuego();
                        //MessageBox.Show("HA SALIDO DE 241, PUNTO:CONTROL:1");
                        break;

                    case 301:
                        //MessageBox.Show(mensaje);
                        currentTurn = mensaje;
                        pasarelaAskForCurrentTokenVector();
                        //MessageBox.Show("SALIDO DE 301 EN LOCAL");

                        break;
                    case 351:
                        int longitudMensaje = mensaje.Length;
                        if (longitudMensaje > 3)
                        {
                            mensaje = mensaje.Substring(0, 3);
                        }
                        //MessageBox.Show("TURNO DE: "+mensaje);
                        pasarelaGestorDeTurnos(mensaje);
                        //btt_pos_send.Text = "TERMINADO 351";
                        break;
                    
                    case 401:
                        //MessageBox.Show("ENTRANDO 401 EN LOCAL");
                        //MessageBox.Show(mensaje);
                        pasarelaBuildTokenVector(mensaje, currentTurn);
                        //MessageBox.Show("SALIENDO 401 EN LOCAL");
                        break;

                    case 901:
                        MessageBox.Show("ATENCIÓN! El ganador es: " + mensaje);
                        Thread.Sleep(1000); // Espera de 1 segundo (1000 milisegundos)
                        pasarelaDesconectar();
                        break;

                    case 1001:
                        if (mensaje == "EMPTY")
                        {
                            MessageBox.Show("No se ha encontrado el registro de ninguna partida entre las fechas facilitadas.");
                        }
                        else
                        {
                            // Crear una instancia de Formulario2
                            FormRango FormRango = new FormRango(dateFirst.Value, dateLast.Value, mensaje, this);

                            // Mostrar Formulario2 de manera modal
                            FormRango.ShowDialog();
                        }
                        break;

                }
            }
        }

        private void ponPanelConectados(string mensajeRecivido)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: ponPanelConectados                                                                              |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Updates the UI element displaying connected players by parsing a received message,           |
        // | processing player data, and formatting it for display. Excludes the local user from the list.             |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - mensajeRecivido: A string containing player data separated by hyphens ("-").                           |
        // | Output: Updates the ConnectedPlayersTextBox UI element to show the list of connected players.             |
        // |-----------------------------------------------------------------------------------------------------------|
            // Limpiar las filas anteriores
            ConnectedPlayersTextBox.Text = "";

            // Separar el string recibido
            string[] trozosConectados = mensajeRecivido.Split('-');
            int maxConectados = trozosConectados.Length;
            int currentRegisterInsertion = 0;
            string cadenaTexto = "Jugadores conectados:";

            // Bucle while para recorrer el array
            while (currentRegisterInsertion < maxConectados)
            {
                string usuario = trozosConectados[currentRegisterInsertion];

                // Verificar que el trozo no esté vacío ni sea el usuario local
                if (!string.IsNullOrEmpty(usuario) && usuario != 7.ToString())
                {
                    // Añadir un nuevo registro
                    cadenaTexto = cadenaTexto + usuario + " ";

                }

                // Incrementar el índice para continuar con el siguiente elemento
                currentRegisterInsertion++;
            }

            this.Invoke((MethodInvoker)(() =>
            {

                ConnectedPlayersTextBox.Text = cadenaTexto;

            }));
        }

        private void pasarelaLogin()
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaLogin                                                                                   |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the login process by updating the UI to transition to the appropriate panels and     |
        // | sending login credentials to the server. Provides feedback to the user about the login status.            |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - None (relies on global variables and the server connection).                                           |
        // | Output: Updates UI elements to reflect a successful login, sends user credentials to the server, and      |
        // | provides login feedback through message boxes.                                                            |
        // |-----------------------------------------------------------------------------------------------------------|
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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: Form1_Load                                                                                      |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Executes when the form is loaded. It initializes the board and prepares the UI for further   |
        // | interaction by enabling painting and refreshing necessary components.                                     |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the load event (Form).                                                    |
        // |  - EventArgs e: Event arguments for the form load event.                                                  |
        // | Output: Prepares and initializes the board and ensures UI elements are ready for rendering and updates.   |
        // |-----------------------------------------------------------------------------------------------------------|

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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: CrearTablon                                                                                     |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Dynamically adjusts the sizes and dimensions of the game board and its components based on   |
        // | the current dimensions of the containing panel.                                                           |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input: None                                                                                               |
        // | Output: Dynamically resized board and its elements for consistent and proportional UI representation.     |
        // |-----------------------------------------------------------------------------------------------------------|

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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: medioCentro_Paint                                                                               |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Renders the central area of the Parchís game board, drawing a cross pattern with diagonal    |
        // | lines and four triangles of different colors (red, yellow, green, and blue) in the center. The function   |
        // | is only executed when painting is allowed, controlled by the "allowed_to_paint" flag. The triangles are   |
        // | drawn with borders and filled with solid colors to create a visually appealing design for the game.       |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click or other trigger).                                |
        // |  - PaintEventArgs e: Contains event data for painting, including the Graphics object to perform drawing.  |
        // | Output:                                                                                                   |
        // |  - Visual representation of the Parchís board's center.                                                   |
        // |-----------------------------------------------------------------------------------------------------------|

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

        private double[] obtenerPosicion(int numeroCasilla, int colorFicha, int posicionVector)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: obtenerPosicion                                                                                 |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Determines the position (coordinates) of a specific game piece within a given square based   |
        // | on its type and orientation. Handles different scenarios for horizontal and vertical positions,           |
        // | and defaults to specific settings for unclassified cases.                                                 |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - numeroCasilla: Integer representing the square's identifier.                                           |
        // |  - colorFicha: Integer indicating the color/type of the game piece.                                       |
        // |  - posicionVector: Integer providing additional context for positioning.                                  |
        // | Output:                                                                                                   |
        // |  - Returns an array of doubles containing the coordinates [x1, y1, x2, y2] for the calculated position.   |
        // |-----------------------------------------------------------------------------------------------------------|

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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: dibujarFicha                                                                                    |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Finds a specific game board square (panel) by its identifier and updates its visual state.   |
        // | Executes actions like refreshing the panel and removing a paint event handler.                            |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - sufijo: A string representing the numeric identifier of the square to update.                          |
        // | Output:                                                                                                   |
        // |  - Returns a boolean value indicating the successful execution of the update (always true).               |
        // |-----------------------------------------------------------------------------------------------------------|
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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: vectorPosicionRecibido                                                                          |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Processes and updates the position of pieces on the game board based on the received vector  |
        // | string. It splits the input string into segments representing individual piece positions and colors,      |
        // | then calculates and draws the corresponding positions of the pieces on the board. It also updates the     |
        // | state of each cell and refreshes the display of the player's home areas.                                  |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - vector_sin_tratar (string): A hyphen-separated string representing the positions of the pieces and     |
        // |    their colors.                                                                                          |
        // | Output:                                                                                                   |
        // |  - Updates the positions and visual representation of the game pieces on the board.                       |
        // |  - Refreshes the home areas for each player (Green, Red, Blue, Yellow).                                   |
        // |-----------------------------------------------------------------------------------------------------------|

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
                if (current_Numeric_Pos < 75)
                {
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
                        colorFichaDibujar = "Gold";
                    }
                    double[] result = obtenerPosicion(Convert.ToInt32(trozos[current_vector_pos]), colorFicha, current_vector_pos);
                    centroX1 = result[0];
                    centroY1 = result[1];
                    centroX2 = result[2];
                    centroY2 = result[3];
                    string sufijo;
                    int posicionNumerica = Convert.ToInt32(trozos[current_vector_pos]);
                    string sufijoNumerico = Convert.ToString(posicionNumerica - 68);
                    if (posicionNumerica > 68)
                    {

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
                        if (Convert.ToInt32(sufijoNumerico) < 75)
                        {
                            dibujarFicha(sufijo);
                            estadoCasillas[Convert.ToInt32(trozos[current_vector_pos])] = estadoCasillas[Convert.ToInt32(trozos[current_vector_pos])] + 1;
                        }

                    }

                }
                current_vector_pos = current_vector_pos + 1;
                
                pintarHomes = true;
                casilla_baseVerde.Refresh();
                casilla_baseRoja.Refresh();
                casilla_baseAzul.Refresh();
                casilla_baseAmarilla.Refresh();
                pintarHomes = false;
            }
        }

        private void InicializarCasillas(string sufijo_casilla)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: InicializarCasillas                                                                             |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Dynamically locates a panel control by its generated name based on the given suffix and      |
        // | subscribes the panel's Paint event to a handler. This allows each identified panel to be rendered         |
        // | correctly when needed.                                                                                    |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - sufijo_casilla (string): A suffix used to dynamically generate the name of the panel.                  |
        // | Output:                                                                                                   |
        // |  - Subscribes the Paint event for the panel found, allowing it to be drawn correctly in the UI.           |
        // |-----------------------------------------------------------------------------------------------------------|

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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: FinalizarCasillas                                                                               |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Clears all pieces from the game board by iterating through the panels representing the       |
        // | board's squares (casilla1 to casilla68) and the colored home areas (casilla_x1 to casilla_x7). The event  |
        // | handler for each panel's Paint event is unsubscribed, and the panels are refreshed to remove any previous |
        // | drawing.                                                                                                  |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input: None                                                                                               |
        // | Output:                                                                                                   |
        // |  - Clears any drawn pieces on the board by refreshing each relevant panel.                                |
        // |  - Unsubscribes the Paint event from each panel to stop further drawing actions.                          |
        // |-----------------------------------------------------------------------------------------------------------|

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
                    id_color = "b";
                }
                else
                {
                    id_color = "a";
                }

                int b = 1;

                while (b <= 7)
                {
                    string nombrePanel = "casilla_m" + id_color + b;

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
        // |--------------------------------------------------------------------------------------------------------------|
        // | Function: Panel_Paint                                                                                        |
        // |--------------------------------------------------------------------------------------------------------------|
        // | Description: Handles the Paint event for panels representing the game board. It draws ellipses (representing |
        // | the game pieces) inside each panel based on the current piece's position and state. The position of the      |
        // | pieces is determined by previously set coordinates. The function draws an ellipse to represent a piece in    |
        // | the correct location based on the stored data.                                                               |
        // |--------------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                       |
        // |  - object sender: The panel that triggered the Paint event.                                                  |
        // |  - PaintEventArgs e: The event arguments containing the graphics object to draw on.                          |
        // | Output:                                                                                                      |
        // |  - Draws an ellipse (game piece) inside the panel at the correct position based on its coordinates.          |
        // |  - Draws multiple pieces if necessary (for example, if a piece is moved to a new position).                  |
        // |--------------------------------------------------------------------------------------------------------------|

            if ((current_Numeric_Pos != 0) && (current_Numeric_Pos <= 74))
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
                                    colorFichaDibujar = "Gold";
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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: btt_pos_send_Click                                                                              |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event of the button to send the position of a game piece. It retrieves the |
        // | position from a text input (pos_rec), then calls the function to update the game board with the new       |
        // | position information.                                                                                     |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output:                                                                                                   |
        // |  - Retrieves the position input from the user and passes it to the vectorPosicionRecibido function to     |
        // |    update the game board with the new piece position.                                                     |
        // |-----------------------------------------------------------------------------------------------------------|

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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: CreateServerConnection                                                                          |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Establishes a connection to a server using TCP. It creates a socket and attempts to connect  |
        // | to the server using the IP address and port provided by the user in the UI (IPBox and PortBox). If the    |
        // | connection is successful, a new thread is started to listen for incoming messages from the server. If the |
        // | connection fails, an error message is shown and the connection attempt is aborted.                        |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - IPBox.Text: The IP address of the server (user input).                                                 |
        // |  - PortBox.Text: The port number of the server (user input).                                              |
        // | Output:                                                                                                   |
        // |  - Connects to the server and starts a listening thread for server messages if successful.                |
        // |  - Displays an error message and halts the connection attempt if an exception occurs.                     |
        // |-----------------------------------------------------------------------------------------------------------|

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
        // | Function: startChat                                                                                       |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Manages the thread that listens for incoming messages from the server. If a listening thread |
        // | is already active, it safely stops it before starting a new one.                                          |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input: None.                                                                                              |
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
        // | Function: messageSenderButton_Click_1                                                                     |
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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaSendChatMessage                                                                         |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Sends a chat message to all connected players via the server. Clears the input text box      |
        // | after sending and provides feedback if the server connection is unavailable.                              |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - None (relies on global variables and UI elements for input).                                           |
        // | Output:                                                                                                   |
        // |  - Sends a chat message to the server if the connection is active and the input box is not empty.         |
        // |  - Displays an error message if the server is unreachable.                                                |
        // |-----------------------------------------------------------------------------------------------------------|
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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: btt_pos_send_Click_1                                                                            |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for the position send button. Retrieves the position entered         |
        // | in the text box and passes it to the vectorPosicionRecibido function for processing.                      |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - sender: Object representing the event sender (the button clicked).                                     |
        // |  - e: EventArgs containing the event data.                                                                |
        // | Output:                                                                                                   |
        // |  - Passes the entered position (from pos_rec.Text) to vectorPosicionRecibido for further handling.        |
        // |-----------------------------------------------------------------------------------------------------------|
            string numeroCasilla = pos_rec.Text;
            vectorPosicionRecibido(numeroCasilla);
        }

        private void bttnPasarela_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: bttnPasarela_Click                                                                              |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for the "Pasarela" button. Initiates the login process by calling    |
        // | the pasarelaLogin function.                                                                               |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - sender: Object representing the event sender (the button clicked).                                     |
        // |  - e: EventArgs containing the event data.                                                                |
        // | Output:                                                                                                   |
        // |  - Calls the pasarelaLogin function to handle user login and UI updates.                                  |
        // |-----------------------------------------------------------------------------------------------------------|
            pasarelaLogin();
        }

        private void chatInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
        // |-------------------------------------------------------------------------------------------------------------|
        // | Function: chatInputTextBox_KeyDown                                                                          |
        // |-------------------------------------------------------------------------------------------------------------|
        // | Description: This event handler is triggered when a key is pressed while the chat input textbox is focused. |
        // | It checks if the Enter key is pressed and, if so, calls the method 'pasarelaSendChatMessage()' to send the  |
        // | message from the chat input.                                                                                |
        // |-------------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                      |
        // |  - e.KeyCode: The key that was pressed by the user (checks if it's Enter).                                  |
        // | Output:                                                                                                     |
        // |  - If Enter is pressed, sends the chat message via the 'pasarelaSendChatMessage' method.                    |
        // |-------------------------------------------------------------------------------------------------------------|

            if (e.KeyCode == Keys.Enter) // Verificar si la tecla presionada es Enter
            {
                e.SuppressKeyPress = true; // Opcional: evita el sonido de "ding"
                pasarelaSendChatMessage();
            }
        }

        private void bttn_crearPartida_Click(object sender, EventArgs e)
        {
        // |------------------------------------------------------------------------------------------------------------|
        // | Function: bttn_crearPartida_Click                                                                          |
        // |------------------------------------------------------------------------------------------------------------|
        // | Description: It checks if the server connection is active (server_on == true). If connected, it hides the  |
        // | selection panels and shows the player invitation panel, where the user can invite players to the game. It  |
        // | also updates the game state and sends a message to the server indicating the creation of the game. If the  |
        // | server is not connected, it shows an error message.                                                        |
        // |------------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                     |
        // |  - object sender: The source of the event (button click).                                                  |
        // |  - EventArgs e: Event arguments for the button click event.                                                |
        // | Output:                                                                                                    |
        // |  - Updates UI elements by showing/hiding panels based on game creation status.                             |
        // |  - Sends a message to the server to indicate the game creation request.                                    |
        // |------------------------------------------------------------------------------------------------------------|

            this.Invoke((MethodInvoker)(() =>
            {
                labelConnected.Visible = false;
                bttn_deleteAccount.Visible = false;
                ConnectedPlayersTextBox.Visible = false;
                PlayersPlayedWith_bttn.Visible = false;
                Disconnection_bttn.Visible = false;
                bttn_deleteAccount.Visible = false;
                lobby = false;
            }));

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

                ponConectadosEnTablaInvitar(global_conectados);

            }
            else
            {
                MessageBox.Show("ERROR: Connection with the server must be active before creating a new game.");
            }

        }

        private void ponConectadosEnTablaInvitar(string vectorRecividoConectados)
        {
        // |-------------------------------------------------------------------------------------------------------------|
        // | Function: ponConectadosEnTablaInvitar                                                                       |
        // |-------------------------------------------------------------------------------------------------------------|
        // | Description: This method is responsible for populating a DataGridView table with a list of connected users  |
        // | retrieved from the server. It processes a received string of connected users, splits it into individual     |
        // | usernames, and displays them in the table (excluding the local user and invalid entries). It also updates   |
        // | the invitation table if a waiting room exists.                                                              |
        // |-------------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                      |
        // |  - vectorRecividoConectados: A string containing the list of connected usernames, separated by a delimiter. |
        // | Output:                                                                                                     |
        // |  - Updates the DataGridView by adding rows with connected users and their respective invitation buttons.    |
        // |  - Calls 'actualizarTablaInvitaciones' if a waiting room exists.                                            |
        // |-------------------------------------------------------------------------------------------------------------|

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
                    bool encontradoTablaActual = false;
                    int currentSearchingRow = 0;
                    while ((currentSearchingRow < dataGridInvitar.RowCount) && (encontradoTablaActual == false))
                    {
                        //MessageBox.Show(usuario + " " + dataGridInvitar.Rows[currentSearchingRow].Cells[0].Value);
                        if (dataGridInvitar.Rows[currentSearchingRow].Cells[0].Value == usuario)
                        {
                            encontradoTablaActual = true;
                        }
                        currentSearchingRow++;
                    }
                    if (encontradoTablaActual == false) //si no esta ya en la tabla
                    {
                        // Añadir una nueva fila solo si el nombre de usuario es válido
                        int rowIndex = dataGridInvitar.Rows.Add();
                        dataGridInvitar.Rows[rowIndex].Cells[0].Value = usuario;
                        dataGridInvitar.Rows[rowIndex].Cells[1].Value = "PULSA PARA INVITAR";
                        dataGridInvitar.Rows[rowIndex].Cells[1].Style.Font = new Font(dataGridInvitar.Font, FontStyle.Bold);
                        dataGridInvitar.Rows[rowIndex].Cells[1].Style.BackColor = Color.Gray;
                    }
                    
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
        // |------------------------------------------------------------------------------------------------------------|
        // | Function: dataGridInvitar_CellContentClick                                                                 |
        // |------------------------------------------------------------------------------------------------------------|
        // | Description: This method handles the click event on a cell within the "dataGridInvitar" DataGridView.      |
        // | It verifies if the clicked cell is the one containing the "PULSA PARA INVITAR" text (column 1). Based on   |
        // | the current invitation status, it shows appropriate messages (e.g., invitation rejected, waiting for a     |
        // | response, or accepted). If the user can be invited, the invitation status is updated to "ESPERANDO         |
        // | RESPUESTA..." and the 'pasarelaInvitar' method is called to initiate the invitation process.               |
        // |------------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                     |
        // |  - object sender: The source of the event (DataGridView cell click).                                       |
        // |  - EventArgs e: Event arguments for the cell click event.                                                  |
        // | Output:                                                                                                    |
        // |  - Displays message boxes with information about the invitation status.                                    |
        // |  - Updates the invitation status in the DataGridView and changes the cell background color.                |
        // |  - Calls 'pasarelaInvitar' to send an invitation to the selected user.                                     |
        // |------------------------------------------------------------------------------------------------------------|

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
        // |------------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaInvitar                                                                                  |
        // |------------------------------------------------------------------------------------------------------------|
        // | Description: This method prepares and sends an invitation message to the server. It constructs the message |
        // | string with the current local username, the username of the player to invite, and the current game ID.     |
        // | The message is then converted to a byte array using ASCII encoding and sent to the server.                 |
        // |------------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                     |
        // |  - string playerToInviteUsername: The username of the player to be invited to the game.                    |
        // | Output:                                                                                                    |
        // |  - Sends the invitation message to the server as a byte array.                                             |
        // |------------------------------------------------------------------------------------------------------------|

            string localGameID = currentGameID.ToString();
            string mensaje = "210/" + localUsername + "-" + playerToInviteUsername + "-" + localGameID+"/";
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void pasarelaNotificacionInvitacionRecibida(string mensaje)
        {
        // |-------------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaNotificacionInvitacionRecibida                                                            |
        // |-------------------------------------------------------------------------------------------------------------|
        // | Description: This method processes a received invitation notification message. It splits the message string |
        // | into parts using the '-' delimiter. The first part is extracted as the sender's username. A new instance of |
        // | the 'InvitacionRecibida' form is created, passing the sender's name and the complete message. This form is  |
        // | then displayed modally, allowing the user to respond to the invitation.                                     |
        // |-------------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                      |
        // |  - string mensaje: The invitation message received, which contains the sender's username and other details. |
        // | Output:                                                                                                     |
        // |  - Displays the 'InvitacionRecibida' form to the user with the invitation details.                          |
        // |-------------------------------------------------------------------------------------------------------------|

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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: aceptarInvitacion                                                                               |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: This function processes the received invitation message, extracts the game ID, and sends a   |
        // |              response message to the server to accept the invitation. It constructs a message with the    |
        // |              local username and game ID, then sends it to the server.                                     |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - string mensajeInvitacion: The invitation message containing the game ID and other relevant information |
        // | Output: None.                                                                                             |
        // |-----------------------------------------------------------------------------------------------------------|

            this.Invoke((MethodInvoker)(() =>
            {
                labelConnected.Visible = false;
                bttn_deleteAccount.Visible = false;
                ConnectedPlayersTextBox.Visible = false;
                PlayersPlayedWith_bttn.Visible = false;
                Disconnection_bttn.Visible = false;
                bttn_deleteAccount.Visible = false;
                lobby = false;
            }));

            //MessageBox.Show("ACEPTADA");
            string[] partes = mensajeInvitacion.Split('-');
            
            //Obtener el primer nombre
            string idPartida = partes[2];
            currentGameID = Convert.ToInt32(idPartida);
            string mensaje = "220/" + localUsername + "-" + idPartida;
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }

        public void rechazarInvitacion(string mensaje)
        {
        // |----------------------------------------------------------------------------------------------------------|
        // | Function: rechazarInvitacion                                                                             |
        // |----------------------------------------------------------------------------------------------------------|
        // | Description: This function is triggered when a user rejects an invitation. It displays a message box     |
        // | indicating that the invitation has been rejected. In this case, the message box shows "RECHAZADA".       |
        // |----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                   |
        // |  - mensaje: A string message passed as an argument to the function. However, it is not used in this case.|
        // | Output:                                                                                                  |
        // |  - None. This function performs an action by showing a message box indicating the rejection of an invite.|
        // |----------------------------------------------------------------------------------------------------------|
            MessageBox.Show("RECHAZADA");
        }

        private void pasarelaPonPlayersPanel(string mensajeRecivido)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaPonPlayersPanel                                                                         |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: This function processes the received message and updates the player panel accordingly.       |
        // | It splits the received message into parts and updates the UI to reflect the players' names and game       |
        // | status. If the user is the host, it updates the invitations table and starts the  game when all players   |
        // | have joined. If the user is not the host, it hides the invitation panels and shows the game-related ones. |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - string mensajeRecivido: The message containing player names separated by a hyphen.                     |
        // | Output: None.                                                                                             |
        // |-----------------------------------------------------------------------------------------------------------|

            string[] partes = mensajeRecivido.Split('-');
            labelName_P1.Text = partes[0];


            if (localUsername != partes[0]) //SI NO ERES EL HOST
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
            else //SI ERES EL HOST
            {
                enSalaEspera = partes;
                actualizarTablaInvitaciones(partes);
            }
            
            if (partes.Length > 1)
            {
                labelName_P2.Text = partes[1];
            }
            if (partes.Length > 2)
            {
                labelName_P3.Text = partes[2];
            }
            if (partes.Length > 3)
            {
                labelName_P4.Text = partes[3];
                UsernamePlayer1 = partes[0];
                UsernamePlayer2 = partes[1];
                UsernamePlayer3 = partes[2];
                UsernamePlayer4 = partes[3];

                if (localUsername == partes[0]) //PARTIDA LLENA, OCULTAMOS MENÚ INVITACIÓN
                {
                    SeleccionarOpcionPanel.Visible = false;
                    panel_infoSeleccionar.Visible = false;
                    panel_invitarJugadores.Visible = false;
                    labelInvitar.Visible = false;
                    dataGridInvitar.Visible = false;

                    panelStatus.Visible = true;
                    panel_Estadisticas.Visible = true;

                    if (game_started == false)
                    {
                        //MessageBox.Show(localUsername+"REQUESTED 230");
                        string mensaje = "230/" + currentGameID;
                        byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                        game_started = true;
                    }
                    

                }
                

            }
        }

        private void actualizarTablaInvitaciones(string[] partes)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: actualizarTablaInvitaciones                                                                     |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: This function updates the invitation table by checking if the player is in a game. If the    |
        // | player is found in the current row of the invitation table, it updates the status in the second column to |
        // | "INVITACIÓN ACEPTADA" and changes the background color of the cell to light green. The changes are made   |
        // | using Invoke to ensure they happen on the UI thread.                                                      |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - string[] partes: Array containing data to check against the player list in the invitation table.       |
        // | Output: None.                                                                                             |
        // |-----------------------------------------------------------------------------------------------------------|

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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaIniciarJuego                                                                            |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: This function initializes the game by making the game board visible, displaying the player   |
        // | 1 turn label, and changing the background color of Player 1's group box to light green. It uses Invoke    |
        // | to ensure the changes happen on the UI thread.                                                            |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input: None.                                                                                              |
        // | Output: None.                                                                                             |
        // |-----------------------------------------------------------------------------------------------------------|

            this.Invoke((MethodInvoker)(() =>
            {
                lobby = false;
                panelCasillas.Visible = true;
                panel_reglas.Visible = true;
                turno_P1.Visible = true;
                groupBox_P1.BackColor = Color.LightGreen;
                groupBox_viewGames.Visible = false;
            }));
        }

        private void casilla_baseVerde_Paint(object sender, PaintEventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: casilla_baseVerde_Paint                                                                         |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: This event handler is triggered to handle the painting of the green home pieces on the board.|
        // | If the pintarHomes flag is true, it checks the positions of the pieces on the board and draws green       |
        // | circular pieces (or "homes") at specific locations (defined by currentPos to maxPos). The drawing is      |
        // | done by calculating the position (x1, y1) and the size of the ellipse (diameter).                         |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The object that triggered the event (in this case, the base green square).              |
        // |  - EventArgs e: PaintEventArgs containing the graphics object for custom drawing.                         |
        // | Output:                                                                                                   |
        // |  - The function does not return a value but performs drawing actions on the board. It places green        |
        // |    ellipses (representing the home pieces) at specific positions.                                         |
        // |-----------------------------------------------------------------------------------------------------------|

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
        // |----------------------------------------------------------------------------------------------------------|
        // | Function: casilla_baseRoja_Paint                                                                         |
        // |----------------------------------------------------------------------------------------------------------|
        // | Description: This event handler is triggered to handle the painting of the red home pieces on the board. |
        // | If the pintarHomes flag is true, it checks the positions of the pieces on the board and draws red        |
        // | circular pieces (or "homes") at specific locations (defined by currentPos to maxPos). The drawing is     |
        // | done by calculating the position (x1, y1) and the size of the ellipse (diameter).                        |
        // |----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                   |
        // |  - object sender: The object that triggered the event (in this case, the base red square).               |
        // |  - EventArgs e: PaintEventArgs containing the graphics object for custom drawing.                        |
        // | Output:                                                                                                  |
        // |  - The function does not return a value but performs drawing actions on the board. It places red         |
        // |    ellipses (representing the home pieces) at specific positions.                                        |
        // |----------------------------------------------------------------------------------------------------------|

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
        // |----------------------------------------------------------------------------------------------------------|
        // | Function: casilla_baseAzul_Paint                                                                         |
        // |----------------------------------------------------------------------------------------------------------|
        // | Description: This event handler is triggered to handle the painting of the blue home pieces on the       |
        // | board. If the pintarHomes flag is true, it checks the positions of the pieces on the board. It then      |
        // | draws blue circular pieces (or "homes") at specific locations (defined by currentPos to maxPos).         |
        // | The drawing is done by calculating the position (x1, y1) and the size of the ellipse (diameter).         |
        // |----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                   |
        // |  - object sender: The object that triggered the event (in this case, the base blue square).              |
        // |  - EventArgs e: PaintEventArgs containing the graphics object for custom drawing.                        |
        // | Output:                                                                                                  |
        // |  - The function does not return a value but performs drawing actions on the board. It places blue        |
        // |    ellipses (representing the home pieces) at specific positions.                                        |
        // |----------------------------------------------------------------------------------------------------------|

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
        // |----------------------------------------------------------------------------------------------------------|
        // | Function: casilla_baseAmarilla_Paint                                                                     |
        // |----------------------------------------------------------------------------------------------------------|
        // | Description: This event handler is triggered to handle the painting of the yellow home pieces on the     |
        // | board. If the pintarHomes flag is true, it checks the positions of the pieces on the board. It then      |
        // | draws yellow circular pieces (or "homes") at specific locations (defined by currentPos to maxPos).       |
        // | The drawing is done by calculating the position (x1, y1) and the size of the ellipse (diameter).         |
        // |----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                   |
        // |  - object sender: The object that triggered the event (in this case, the base yellow square).            |
        // |  - EventArgs e: PaintEventArgs containing the graphics object for custom drawing.                        |
        // | Output:                                                                                                  |
        // |  - The function does not return a value but performs drawing actions on the board. It places yellow      |
        // |    ellipses (representing the home pieces) at specific positions.                                        |
        // |----------------------------------------------------------------------------------------------------------|

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
                        colorFichaDibujar = "Gold";
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
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: dadoP1_Click                                                                                    |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for Player 1's dice button. If the local player is Player 1, it      |
        // | hides the turn indicator for Player 1, resets the background color, and asks for the current player's     |
        // | turn to be processed.                                                                                     |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Hides Player 1's turn indicator, resets the background color, and triggers the request for the    |
        // |         current player's turn.                                                                            |
        // |-----------------------------------------------------------------------------------------------------------|
            if (localUsername == UsernamePlayer1)
            {
                turno_P1.Visible = false;
                groupBox_P1.BackColor = Color.FromArgb(153, 180, 209); // Azul "ActiveCaption"

                Thread.Sleep(1000);
                pasarelaAskForCurrentPlayerTurn();

            }
        }

        private void pasarelaAskForCurrentPlayerTurn()
        {
        // |----------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaAskForCurrentPlayerTurn                                                                |
        // |----------------------------------------------------------------------------------------------------------|
        // | Description: Sends a request to the server asking for the current player turn in the specified game ID.  |
        // | The function sends a message containing the current game ID using a predefined format (300/).            |
        // | This message is then sent to the server for processing.                                                  |
        // |----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                   |
        // |  - None. The function uses the globally accessible currentGameID to form the message.                    |
        // | Output:                                                                                                  |
        // |  - None. The function sends a request message to the server.                                             |
        // |----------------------------------------------------------------------------------------------------------|
            string mensaje = "300/" + currentGameID;
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            //MessageBox.Show("ENVIADO 300");
        }

        private void pasarelaAskForCurrentTokenVector()
        {
        // |----------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaAskForCurrentTokenVector                                                               |
        // |----------------------------------------------------------------------------------------------------------|
        // | Description: Sends a request to the server for the current token vector associated with the specified    |
        // | game ID. The function sends a message containing the current game ID using a predefined format (400/).   |
        // | This message is then sent to the server for processing.                                                  |
        // |----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                   |
        // |  - None. The function uses the globally accessible currentGameID to form the message.                    |
        // | Output:                                                                                                  |
        // |  - None. The function sends a request message to the server.                                             |
        // |----------------------------------------------------------------------------------------------------------|
            string mensaje = "400/" + currentGameID;
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            //MessageBox.Show("ENVIADO 400: "+ mensaje);
        }

        private void pasarelaBuildTokenVector(string temp_givenTokenVector, string givenCurrentTurn)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaBuildTokenVector                                                                        |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: This function builds a token vector based on the current player's turn and a randomly rolled |
        // | dice number. It handles the movement of the player's tokens according to the rules, such as allowing      |
        // | tokens to move from the base and adjusting their positions based on the current turn. It also prevents    |
        // | two tokens from occupying the same position on the board.                                                 |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - string temp_givenTokenVector: The vector containing the current positions of all players' tokens.      |
        // |  - string givenCurrentTurn: The current turn color (green, red, blue, yellow).                            |
        // | Output: None.                                                                                             |
        // |-----------------------------------------------------------------------------------------------------------|
            //MessageBox.Show("inicio pasarelaBuildTokenVector");
            Random random = new Random();
            
            // Generar un número aleatorio entre 1 y 6 (inclusive)
            int numeroAleatorio = random.Next(1, 7);
            //numeroAleatorio = 5;


            int moverDirectamente = 0;
            int temp_currentTurnID = 0;
            int salidaBase = 0;
            int casillaMaxima = 0;
            int casilla_a_revisar = 2187;
            int BarrierVectorPos = 2187;
            bool UserHasBarrier = false;

            if (givenCurrentTurn == "101") //verde
            {
                temp_currentTurnID = 1;
                salidaBase = 56;
                casillaMaxima = 51;
            }
            else if (givenCurrentTurn == "102") //rojo
            {
                temp_currentTurnID = 2;
                salidaBase = 39;
                casillaMaxima = 34;
            }
            else if (givenCurrentTurn == "103") //azul
            {
                temp_currentTurnID = 3;
                salidaBase = 22;
                casillaMaxima = 17;

            }
            else if (givenCurrentTurn == "104") //amarillo
            {
                temp_currentTurnID = 4;
                salidaBase = 5;
                casillaMaxima = 68;
            }

            string[] temp_TokenTrozos = temp_givenTokenVector.Split('-');
            string[] temp_currentUserTokens = new string[4];

            if (temp_currentTurnID == 1)
            {
                temp_currentUserTokens[0] = temp_TokenTrozos[0];
                temp_currentUserTokens[1] = temp_TokenTrozos[1];
                temp_currentUserTokens[2] = temp_TokenTrozos[2];
                temp_currentUserTokens[3] = temp_TokenTrozos[3];
            }
            else if (temp_currentTurnID == 2)
            {
                temp_currentUserTokens[0] = temp_TokenTrozos[4];
                temp_currentUserTokens[1] = temp_TokenTrozos[5];
                temp_currentUserTokens[2] = temp_TokenTrozos[6];
                temp_currentUserTokens[3] = temp_TokenTrozos[7];
            }
            else if (temp_currentTurnID == 3)
            {
                temp_currentUserTokens[0] = temp_TokenTrozos[8];
                temp_currentUserTokens[1] = temp_TokenTrozos[9];
                temp_currentUserTokens[2] = temp_TokenTrozos[10];
                temp_currentUserTokens[3] = temp_TokenTrozos[11];
            }
            else if (temp_currentTurnID == 4)
            {
                temp_currentUserTokens[0] = temp_TokenTrozos[12];
                temp_currentUserTokens[1] = temp_TokenTrozos[13];
                temp_currentUserTokens[2] = temp_TokenTrozos[14];
                temp_currentUserTokens[3] = temp_TokenTrozos[15];
            }

            int currentVectorPos = 0;
            int zeroCounter = 0;
            while(currentVectorPos < 4) //Revisar si todas las fichas en base
            {

                if (temp_currentUserTokens[currentVectorPos] == "0")
                {
                    zeroCounter++;
                }
                currentVectorPos++;
            }

            bool siguiente = true;

            if (zeroCounter == 4) //Si todas las fichas estan en la base
            {
                if (numeroAleatorio == 5)
                {
                    bool casillaOcupada = revisarPosicion(temp_TokenTrozos, salidaBase);

                    if (casillaOcupada == true)
                    {
                        MessageBox.Show("No puedes salir, ya hay 2 fichas ocupando tu casilla de salida.");
                    }
                    else
                    {
                        temp_currentUserTokens[0] = salidaBase.ToString();
                    }
                    moverDirectamente = 1;
                    siguiente = false;
                }
                else
                {
                    MessageBox.Show("Debes sacar un 5 antes de poder sacar la ficha de la base.");
                    moverDirectamente = 1;
                    siguiente = false;
                }

                
 
            }
            else if ((numeroAleatorio == 5) && (zeroCounter != 0))
            {
                MessageBox.Show("¡HAS SACADO UN 5!");
                bool salidaAssignada = false;
                int currentPos = 0;

                while ((currentPos < 4) && (salidaAssignada == false))
                {
                    if (temp_currentUserTokens[currentPos] == "0")
                    {
                        bool casillaOcupada = revisarPosicion(temp_TokenTrozos, salidaBase);

                        if (casillaOcupada == true)
                        {
                            MessageBox.Show("No puedes salir, ya hay 2 fichas ocupando tu casilla de salida.");
                            salidaAssignada = true; //termino bucle, no puedo salir
                        }
                        else
                        {
                            temp_currentUserTokens[currentPos] = salidaBase.ToString();
                            salidaAssignada = true; //termino bucle, puedo salir
                            siguiente = false; //mi movimiento es sacar la ficha de la casilla
                            moverDirectamente = 1; //directamente hago el movimiento
                        }

                    }

                    currentPos++;
                }
            }
            if ((siguiente == true) && (numeroAleatorio == 6)) //si hay fichas fuera de la base y has sacado un 6
            {
                BarrierVectorPos = CheckUserBarrierPosition(temp_currentUserTokens); //almacena la posicion en el vector de fichas del usuario de la barrera
                
                if (BarrierVectorPos == 2187) //si no hay barrera
                {
                    UserHasBarrier = false;
                }
                else //si hay barrera
                {
                    UserHasBarrier = true;
                }
            }
            if (siguiente == true) //Si hay fichas fuera de la base y no he sacado
            {
                int sugerencia1 = 0;
                int sugerencia2 = 0;
                int sugerencia3 = 0;
                int sugerencia4 = 0;

                int fichaBuscandoPosicion = 0;

                while (fichaBuscandoPosicion < 4)
                {
                    //MessageBox.Show("entrando else");
                    int valorInicial;
                    int valorIntermedio;

                    valorInicial = Convert.ToInt32(temp_currentUserTokens[fichaBuscandoPosicion]);
                    valorIntermedio = valorInicial + numeroAleatorio;

                    if ((valorIntermedio > 68) && (valorInicial <= 68) && (givenCurrentTurn != "104")) //Para asegurar continuidad una vez alcanzada casilla 68
                    {
                        valorIntermedio = numeroAleatorio - (68 - valorInicial);
                    }

                    int casillasNoRecorridas = numeroAleatorio - (casillaMaxima - valorInicial);

                    //Para cuando se alcanza la casilla final
                    if ((valorIntermedio > 51) && (valorInicial < 56) && (givenCurrentTurn == "101")) //entrada verde
                    {
                        valorIntermedio = 68 + casillasNoRecorridas;
                    }
                    else if ((valorIntermedio > 34) && (valorInicial < 39) && (givenCurrentTurn == "102")) //entrada roja
                    {
                        valorIntermedio = 68 + casillasNoRecorridas;
                    }
                    else if ((valorIntermedio > 17) && (valorInicial < 22) && (givenCurrentTurn == "103")) //entrada azul
                    {
                        valorIntermedio = 68 + casillasNoRecorridas;
                    }


                    
                    
                    if (valorIntermedio >= 75) //si es superior al numero maximo de casilla
                    {
                        valorIntermedio = 75;
                    }
                    else if ((valorIntermedio <= 68) && (valorInicial != 0) && (valorIntermedio != 0)) //si mi posición final es alguna de las del circuito exterior y la ficha no está saliendo ni en la casilla inicial
                    {
                        int pasoLibreHasta = revisarCasillasMuevo(valorInicial, valorIntermedio, temp_TokenTrozos);

                        valorIntermedio = pasoLibreHasta;
                    }
                    else //aún así, haremos una comprobación final para por ejemplo el caso de sacar ficha de la base
                    {
                        bool casillaOcupada = revisarPosicion(temp_TokenTrozos, valorIntermedio);

                        if ((casillaOcupada == true) && (valorIntermedio != 0)) //intentar quitar
                        {
                            MessageBox.Show("No es posible mover la ficha a la posición " + valorIntermedio.ToString() + " a causa de una barrera.");
                            valorIntermedio = valorInicial;
                        }
                    }

                    if (UserHasBarrier == true)
                    {
                        if (fichaBuscandoPosicion != BarrierVectorPos)
                        {
                            valorInicial = 0;
                        }
                    }

                    //asignar a resultado de sugerencia
                    if (fichaBuscandoPosicion == 0)
                    {
                        if (valorInicial != 0)
                        {
                            sugerencia1 = valorIntermedio;
                        }
                        
                    }
                    else if (fichaBuscandoPosicion == 1)
                    {
                        if (valorInicial != 0)
                        {
                            sugerencia2 = valorIntermedio;
                        }
                    }
                    else if (fichaBuscandoPosicion == 2)
                    {
                        if (valorInicial != 0)
                        {
                            sugerencia3 = valorIntermedio;
                        }
                    }
                    else if (fichaBuscandoPosicion == 3)
                    {
                        if (valorInicial != 0)
                        {
                            sugerencia4 = valorIntermedio;
                        }
                    }

                    fichaBuscandoPosicion = fichaBuscandoPosicion + 1;

                }

                //Asignar valor final
                if (zeroCounter == 3) //solo 1 movimiento posible
                {
                    int valorSumado = sugerencia1 + sugerencia2 + sugerencia3 + sugerencia4;

                    if (valorSumado >= 75)
                    {
                        valorSumado = 75;
                    }


                    string cadenaIntermedia;
                    cadenaIntermedia = Convert.ToString(valorSumado);

                    casilla_a_revisar = valorSumado;


                    if (sugerencia1 != 0)
                    {
                        temp_currentUserTokens[0] = cadenaIntermedia;
                        
                    }
                    else if (sugerencia2 != 0)
                    {
                        temp_currentUserTokens[1] = cadenaIntermedia;
                    }
                    else if (sugerencia3 != 0)
                    {
                        temp_currentUserTokens[2] = cadenaIntermedia;
                    }
                    else if (sugerencia4 != 0)
                    {
                        temp_currentUserTokens[3] = cadenaIntermedia;
                    }

                    moverDirectamente = 1;

                    if (casilla_a_revisar != 2187) //si hay movimiento
                    {
                        temp_TokenTrozos = revisarMuertes(temp_TokenTrozos, casilla_a_revisar, temp_currentTurnID);
                    }

                }
                else //más de un movimiento posible
                {
                    global_temp_TokenTrozos = temp_TokenTrozos;
                    global_temp_currentUserTokens = temp_currentUserTokens;
                    global_temp_currentTurnID = temp_currentTurnID;

                    mostrarOpciones(sugerencia1, sugerencia2, sugerencia3, sugerencia4);
                }
                
            }

            if (moverDirectamente == 1)
            {
                //RECONSTRUIR LA CADENA A MANDAR
                string resultCadenaPosiciones = pasarelaReconstruirCadena(temp_currentTurnID, temp_TokenTrozos, temp_currentUserTokens); 
                pasarelaEnviarPosicionesAServidor(resultCadenaPosiciones);
            }

        }

        private int CheckUserBarrierPosition(string [] given_temp_currentUserTokens)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: CheckUserBarrierPosition                                                                        |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: This function checks if the given user's token positions form a barrier, i.e., two tokens are|
        // | in the same position on the board. If found, it returns the position of the barrier; otherwise, it returns|
        // | a default value of 2187, indicating no barrier was found. It iterates through the token positions and     |
        // | compares them against each other to identify a match.                                                     |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - given_temp_currentUserTokens: An array of strings representing the positions of the user's tokens.     |
        // | Output:                                                                                                   |
        // |  - An integer value representing the position of the barrier if found, or 2187 if no barrier exists.      |
        // |-----------------------------------------------------------------------------------------------------------|
            int currentVectorPos = 0;
            int barreraEn = 2187;
            bool barreraEncontrada = false;

            while ((currentVectorPos < 4) && (barreraEncontrada == false)) //mientras haya pos por revisar y no se haya encontrado barrera
            {
                int currentLooking = 0;
                int ValorEnCurrentVectorPos = Convert.ToInt32(given_temp_currentUserTokens[currentVectorPos]);

                if ((ValorEnCurrentVectorPos == 0)||(ValorEnCurrentVectorPos >= 75)) //si la ficha esta en casa o ya ha terminado
                {
                    currentVectorPos = currentVectorPos + 1; //siguiente posicion
                }
                else
                {
                    while ((currentLooking < 4) && (barreraEncontrada == false))
                    {
                        int ValorEnCurrentLookingPos = Convert.ToInt32(given_temp_currentUserTokens[currentLooking]);

                        if (currentVectorPos == currentLooking) //si estoy revisando una misma posición salto a la siguiente
                        {
                            currentLooking = currentLooking + 1;
                        }
                        else if (ValorEnCurrentVectorPos == ValorEnCurrentLookingPos) //si ambos coinciden, he encontrado barrera
                        {
                            barreraEncontrada = true;
                            barreraEn = currentVectorPos;
                        }
                        currentLooking = currentLooking + 1;
                    }
                    currentVectorPos = currentVectorPos + 1;
                }
                
            }
            return barreraEn;
        }

        private bool revisarPosicion(string[] temp_TokenTrozos, int posicionAComprovar)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: revisarPosicion                                                                                 |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Checks if a specific position is occupied by two tokens. The function iterates through the   |
        // | token positions array and counts how many times the specified position is found. If the position is found |
        // | twice, the function returns true, indicating that two tokens are already at the given position.           |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - string[] temp_TokenTrozos: Array of token positions as strings.                                        |
        // |  - int posicionAComprovar: The position to check for token occupancy.                                     |
        // | Output:                                                                                                   |
        // |  - bool: Returns true if the specified position has two tokens, otherwise false.                          |
        // |-----------------------------------------------------------------------------------------------------------|
            int current = 0;
            int max = temp_TokenTrozos.Length;
            int counter = 0;
            bool encontrados = false;

            while ((current < max) && (encontrados == false))
            {
                if (temp_TokenTrozos[current] == posicionAComprovar.ToString())
                {
                    counter++;
                    
                    if (counter == 2)
                    {
                        encontrados = true;
                    }
                }
                current++;
            }

            return encontrados; //if encontrados = true means 2 tokens already at position
        }

        private void mostrarOpciones(int sugerencia1, int sugerencia2, int sugerencia3, int sugerencia4)
        {
        // |------------------------------------------------------------------------------------------------------------|
        // | Function: mostrarOpciones                                                                                  |
        // |------------------------------------------------------------------------------------------------------------|
        // | Description: Displays the available options for the player based on the suggestions provided. The options  |
        // | correspond to possible moves for the player's token, and each suggestion updates a button's text and state.|
        // | If a suggestion is 0, the corresponding button will be disabled and display "Opcion no disponible".        |
        // |------------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                     |
        // |  - int sugerencia1, sugerencia2, sugerencia3, sugerencia4: Four possible token moves (suggestions) for the |
        // |    player. If a suggestion is 0, the corresponding option is disabled.                                     |
        // | Output: Updates the button states and texts, and makes the options visible to the player.                  |
        // |------------------------------------------------------------------------------------------------------------|
            //MessageBox.Show("GENERANDO BOTONES");

            //asignar a variables globales
            global_sg1 = sugerencia1;
            global_sg2 = sugerencia2;
            global_sg3 = sugerencia3;
            global_sg4 = sugerencia4;

            this.Invoke((MethodInvoker)(() =>
            {

                if (sugerencia1 != 0)
                {
                    opcion1.Enabled = true;
                    opcion1.Text = "Mover ficha a casilla " + sugerencia1.ToString();
                    
                    if (sugerencia1 == 75)
                    {
                        opcion1.Text = "¡MOVER FICHA A CASILLA FINAL!";   
                    }

                    if (global_f1 == true)
                    {
                        opcion1.Text = "Opción no disponible";
                        opcion1.Enabled = false;
                    }

                }
                else
                {
                    opcion1.Enabled = false;
                    opcion1.Text = "Opción no disponible";

                }

                if (sugerencia2 != 0)
                {
                    opcion2.Enabled = true;
                    opcion2.Text = "Mover ficha a casilla " + sugerencia2.ToString();

                    if (sugerencia2 == 75)
                    {
                        opcion2.Text = "¡MOVER FICHA A CASILLA FINAL!";
                    }

                    if (global_f2 == true)
                    {
                        opcion2.Text = "Opción no disponible";
                        opcion2.Enabled = false;
                    }

                }
                else
                {
                    opcion2.Enabled = false;
                    opcion2.Text = "Opción no disponible";
                }

                if (sugerencia3 != 0)
                {
                    opcion3.Enabled = true;
                    opcion3.Text = "Mover ficha a casilla " + sugerencia3.ToString();

                    if (sugerencia3 == 75)
                    {
                        opcion3.Text = "¡MOVER FICHA A CASILLA FINAL!";
                    }

                    if (global_f3 == true)
                    {
                        opcion3.Text = "Opción no disponible";
                        opcion3.Enabled = false;
                    }

                }
                else
                {
                    opcion3.Enabled = false;
                    opcion3.Text = "Opción no disponible";
                }

                if (sugerencia4 != 0)
                {
                    opcion4.Enabled = true;
                    opcion4.Text = "Mover ficha a casilla " + sugerencia4.ToString();

                    if (sugerencia4 == 75)
                    {
                        opcion4.Text = "¡MOVER FICHA A CASILLA FINAL!";
                    }

                    if (global_f4 == true)
                    {
                        opcion4.Text = "Opción no disponible";
                        opcion4.Enabled = false;
                    }

                }
                else
                {
                    opcion4.Enabled = false;
                    opcion4.Text = "Opción no disponible";
                }

                opcion1.Visible = true;
                opcion2.Visible = true;
                opcion3.Visible = true;
                opcion4.Visible = true;

            }));

            //MessageBox.Show("MOSTRADOS");

        }

        private void pasarelaEnviarPosicionesAServidor(string resultCadenaPosiciones)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaEnviarPosicionesAServidor                                                               |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Sends the reconstructed positions string to the server. The string is prefixed with the      |
        // | current game ID and a "500" identifier to indicate it's a message related to sending token positions.     |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - string resultCadenaPosiciones: A string containing the reconstructed token positions for all players.  |
        // | Output: Sends the message to the server with the game ID and the reconstructed token positions.           |
        // |-----------------------------------------------------------------------------------------------------------|
            string mensaje = "500/" + currentGameID + resultCadenaPosiciones;
            //MessageBox.Show(mensaje);
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private string pasarelaReconstruirCadena(int temp_currentTurnID, string[] temp_TokenTrozos, string[] temp_currentUserTokens)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaReconstruirCadena                                                                       |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Reconstructs a string representing the positions of tokens for each player based on the      |
        // | current turn ID and the provided user tokens. It populates a temporary array with the token positions     |
        // | and returns a reconstructed string with the token positions separated by dashes.                          |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - int temp_currentTurnID: The current turn identifier (1 to 4).                                          |
        // |  - string[] temp_TokenTrozos: An array that holds the token positions of all players.                     |
        // |  - string[] temp_currentUserTokens: An array of the current player's token positions.                     |
        // | Output: A reconstructed string with the token positions of all players, separated by dashes.              |
        // |-----------------------------------------------------------------------------------------------------------|
            if (temp_currentTurnID == 1)
            {
                temp_TokenTrozos[0] = temp_currentUserTokens[0];
                temp_TokenTrozos[1] = temp_currentUserTokens[1];
                temp_TokenTrozos[2] = temp_currentUserTokens[2];
                temp_TokenTrozos[3] = temp_currentUserTokens[3];
            }
            else if (temp_currentTurnID == 2)
            {
                temp_TokenTrozos[4] = temp_currentUserTokens[0];
                temp_TokenTrozos[5] = temp_currentUserTokens[1];
                temp_TokenTrozos[6] = temp_currentUserTokens[2];
                temp_TokenTrozos[7] = temp_currentUserTokens[3];
            }
            else if (temp_currentTurnID == 3)
            {
                temp_TokenTrozos[8] = temp_currentUserTokens[0];
                temp_TokenTrozos[9] = temp_currentUserTokens[1];
                temp_TokenTrozos[10] = temp_currentUserTokens[2];
                temp_TokenTrozos[11] = temp_currentUserTokens[3];
            }
            else if (temp_currentTurnID == 4)
            {
                temp_TokenTrozos[12] = temp_currentUserTokens[0];
                temp_TokenTrozos[13] = temp_currentUserTokens[1];
                temp_TokenTrozos[14] = temp_currentUserTokens[2];
                temp_TokenTrozos[15] = temp_currentUserTokens[3];
            }

            string cadenaPosiciones = "|";
            int currentReconstructPos = 0;

            while (currentReconstructPos < 16)
            {
                if (currentReconstructPos == 15)
                {
                    cadenaPosiciones = cadenaPosiciones + temp_TokenTrozos[currentReconstructPos].ToString();
                }
                else
                {
                    cadenaPosiciones = cadenaPosiciones + temp_TokenTrozos[currentReconstructPos].ToString() + "-";
                }

                currentReconstructPos++;
            }

            return cadenaPosiciones;

        }

        private string[] revisarMuertes(string[] given_TokenTrozos, int givenCasilla_a_revisar, int givenTurnoJugador)
        {
        // |------------------------------------------------------------------------------------------------------------|
        // | Function: revisarMuertes                                                                                   |
        // |------------------------------------------------------------------------------------------------------------|
        // | Description: This function checks the positions of tokens on the board and determines if any token needs   |
        // | to be reset due to a conflict with the given position (e.g., being "killed"). If a token's position matches|
        // | the specified position to check (givenCasilla_a_revisar), it resets that token's position to "0". The      |
        // | logic skips positions based on the current player's turn to ensure it does not affect their tokens.        |
        // |------------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                     |
        // |  - given_TokenTrozos: An array of strings representing the positions of tokens on the board.               |
        // |  - givenCasilla_a_revisar: An integer specifying the board position to check for conflicts.                |
        // |  - givenTurnoJugador: An integer (1 to 4) indicating the current player's turn.                            |
        // | Output:                                                                                                    |
        // |  - A corrected array of strings (corrected_TokenTrozos) with updated positions for tokens.                 |
        // |------------------------------------------------------------------------------------------------------------|
            string[] corrected_TokenTrozos = given_TokenTrozos;
            int currentVectorPos = 0;

            if (givenCasilla_a_revisar > 68)
            {
                currentVectorPos = 2187;
            }

            while (currentVectorPos < 16)
            {
                int NumeroCasillaRevisando = Convert.ToInt32(corrected_TokenTrozos[currentVectorPos]);

                if ((NumeroCasillaRevisando == 0) || (NumeroCasillaRevisando >= 75) || (NumeroCasillaRevisando == 17) || (NumeroCasillaRevisando == 34) || (NumeroCasillaRevisando == 51) || (NumeroCasillaRevisando == 68) || (NumeroCasillaRevisando == 5) || (NumeroCasillaRevisando == 22) || (NumeroCasillaRevisando == 39) || (NumeroCasillaRevisando == 56)) //pasamos
                {
                    currentVectorPos = currentVectorPos + 1;
                }
                else //continuamos revisando
                {
                    if ((givenTurnoJugador == 1) && (currentVectorPos < 4)) //si ha tirado el jugador 1 sigo a revisando fichas a partir de la posicion 4
                    {
                        currentVectorPos = 4;
                    }
                    else if ((givenTurnoJugador == 2) && (currentVectorPos >= 4) && (currentVectorPos < 8)) //si ha tirado el jugador 2 sigo a revisando fichas a partir de la posicion 8
                    {
                        currentVectorPos = 8;
                    }
                    else if ((givenTurnoJugador == 3) && (currentVectorPos >= 8) && (currentVectorPos < 12)) //si ha tirado el jugador 3 sigo a revisando fichas a partir de la posicion 12
                    {
                        currentVectorPos = 12;
                    }
                    else if ((givenTurnoJugador == 4) && (currentVectorPos >= 12)) //si ha tirado el jugador 4 sigo a revisando fichas a partir de la posicion 16, (fin bucle)
                    {
                        currentVectorPos = 16;
                    }
                    else //si no estoy revisando las casilla del jugador que ha tirado
                    {
                        if (NumeroCasillaRevisando == givenCasilla_a_revisar) //si la pos revisando coincide con la casilla buscando
                        {
                            corrected_TokenTrozos[currentVectorPos] = "0"; //pongo a 0 dicha pos
                        }
                        currentVectorPos = currentVectorPos + 1; //siguiente casilla
                    }

                }

                
            }

            return corrected_TokenTrozos;
        }

        private void dadoP2_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: dadoP2_Click                                                                                    |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for Player 2's dice button. If the local player is Player 2, it      |
        // | hides the turn indicator for Player 2, resets the background color, and asks for the current player's     |
        // | turn to be processed.                                                                                     |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Hides Player 2's turn indicator, resets the background color, and triggers the request for the    |
        // |         current player's turn.                                                                            |
        // |-----------------------------------------------------------------------------------------------------------|
            if (localUsername == UsernamePlayer2)
            {
                turno_P2.Visible = false;
                groupBox_P2.BackColor = Color.FromArgb(153, 180, 209); // Azul "ActiveCaption"
                Thread.Sleep(1000);
                pasarelaAskForCurrentPlayerTurn();
            }
        }

        private void dadoP3_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: dadoP3_Click                                                                                    |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for Player 3's dice button. If the local player is Player 3, it      |
        // | hides the turn indicator for Player 3, resets the background color, and asks for the current player's     |
        // | turn to be processed.                                                                                     |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Hides Player 3's turn indicator, resets the background color, and triggers the request for the    |
        // |         current player's turn.                                                                            |
        // |-----------------------------------------------------------------------------------------------------------|
            if (localUsername == UsernamePlayer3)
            {
                turno_P3.Visible = false;
                groupBox_P3.BackColor = Color.FromArgb(153, 180, 209); // Azul "ActiveCaption"
                Thread.Sleep(1000);
                pasarelaAskForCurrentPlayerTurn();
            }
        }

        private void dadoP4_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: dadoP4_Click                                                                                    |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for Player 4's dice button. If the local player is Player 4, it      |
        // | hides the turn indicator for Player 4, resets the background color, and asks for the current player's     |
        // | turn to be processed.                                                                                     |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Hides Player 4's turn indicator, resets the background color, and triggers the request for the    |
        // |         current player's turn.                                                                            |
        // |-----------------------------------------------------------------------------------------------------------|
            if (localUsername == UsernamePlayer4)
            {
                turno_P4.Visible = false;
                groupBox_P4.BackColor = Color.FromArgb(153, 180, 209); // Azul "ActiveCaption"
                Thread.Sleep(1000);
                pasarelaAskForCurrentPlayerTurn();
            }
        }

        private void pasarelaGestorDeTurnos(string givenCurrentTurn)
        {
        // |------------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaGestorDeTurnos                                                                           |
        // |------------------------------------------------------------------------------------------------------------|
        // | Description: Manages the turn transitions for the game. It calls methods to terminate the previous turn    |
        // | and updates the UI to show the current player's turn indicator.                                            |
        // |------------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                     |
        // |  - string givenCurrentTurn: The identifier of the current player's turn (101 to 104).                      |
        // | Output: Ends the previous turn, updates the current turn, and invokes methods to update the UI accordingly.|
        // |------------------------------------------------------------------------------------------------------------|
            this.Invoke((MethodInvoker)(() =>
            {
                terminarTurnos();
            }));
            if (givenCurrentTurn == "101")
            {
                currentTurn = "1"; 
            }
            else if (givenCurrentTurn == "102")
            {
                currentTurn = "2";
            }
            else if (givenCurrentTurn == "103")
            {
                currentTurn = "3";
            }
            else if (givenCurrentTurn == "104")
            {
                currentTurn = "4";
            }
            this.Invoke((MethodInvoker)(() =>
            {
                turnoJugador(currentTurn);
            }));

        }

        private void turnoJugador(string givenCurrentTurn)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: turnoJugador                                                                                    |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Displays the current player's turn indicator and changes the background color of the         |
        // | corresponding player group box to highlight the active player.                                            |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - string givenCurrentTurn: The identifier of the current player (1 to 4).                                |
        // | Output: Makes the current player's turn indicator visible and changes the corresponding group box's       |
        // |         background color to light green.                                                                  |
        // |-----------------------------------------------------------------------------------------------------------|
            if (givenCurrentTurn == "1")
            {
                turno_P1.Visible = true;
                groupBox_P1.BackColor = Color.LightGreen;
            }
            else if (givenCurrentTurn == "2")
            {
                turno_P2.Visible = true;
                groupBox_P2.BackColor = Color.LightGreen;
            }
            else if (givenCurrentTurn == "3")
            {
                turno_P3.Visible = true;
                groupBox_P3.BackColor = Color.LightGreen;
            }
            else if (givenCurrentTurn == "4")
            {
                turno_P4.Visible = true;
                groupBox_P4.BackColor = Color.LightGreen;
            }
        }

        private void terminarTurnos()
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: terminarTurnos                                                                                  |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Ends the current turn for all players, hides the turn indicators, and resets the background  |
        // | color of the player group boxes to a default color.                                                       |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - None.                                                                                                  |
        // | Output: Hides the turn indicators and resets the player group boxes' background color to the default.     |
        // |-----------------------------------------------------------------------------------------------------------|
            turno_P1.Visible = false;
            turno_P2.Visible = false;
            turno_P3.Visible = false;
            turno_P4.Visible = false;
            groupBox_P1.BackColor = Color.FromArgb(153, 180, 209); // Azul "ActiveCaption"
            groupBox_P2.BackColor = Color.FromArgb(153, 180, 209); // Azul "ActiveCaption"
            groupBox_P3.BackColor = Color.FromArgb(153, 180, 209); // Azul "ActiveCaption"
            groupBox_P4.BackColor = Color.FromArgb(153, 180, 209); // Azul "ActiveCaption"
        }

        private void opcion1_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: opcion1_Click                                                                                   |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for the option 1 button. It calls the method to process the user's   |
        // | selection and updates the corresponding token based on the selected option.                               |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Calls the method to process the selection of option 1 and updates the token array.                |
        // |-----------------------------------------------------------------------------------------------------------|
            pasarelaProcesarEleccionSugerencia(1);
        }

        private void opcion2_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: opcion2_Click                                                                                   |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for the option 2 button. It calls the method to process the user's   |
        // | selection and updates the corresponding token based on the selected option.                               |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Calls the method to process the selection of option 2 and updates the token array.                |
        // |-----------------------------------------------------------------------------------------------------------|
            pasarelaProcesarEleccionSugerencia(2);
        }

        private void opcion3_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: opcion3_Click                                                                                   |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for the option 3 button. It calls the method to process the user's   |
        // | selection and updates the corresponding token based on the selected option.                               |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Calls the method to process the selection of option 3 and updates the token array.                |
        // |-----------------------------------------------------------------------------------------------------------|
            pasarelaProcesarEleccionSugerencia(3);
        }

        private void opcion4_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: opcion4_Click                                                                                   |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for the option 4 button. It calls the method to process the user's   |
        // | selection and updates the corresponding token based on the selected option.                               |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Calls the method to process the selection of option 4 and updates the token array.                |
        // |-----------------------------------------------------------------------------------------------------------|
            pasarelaProcesarEleccionSugerencia(4);
        }

        private void pasarelaProcesarEleccionSugerencia(int selected_Option)
        {
            // |-----------------------------------------------------------------------------------------------------------|
            // | Function: pasarelaProcesarEleccionSugerencia                                                              |
            // |-----------------------------------------------------------------------------------------------------------|
            // | Description: Processes the user's selected option, updates the token positions accordingly, and disables  |
            // | the option buttons. It then reconstructs the token position string and sends it to the server.            |
            // |-----------------------------------------------------------------------------------------------------------|
            // | Input:                                                                                                    |
            // |  - int selected_Option: The option selected by the user (1 to 4).                                         |
            // | Output: Updates the token array with the selected option's value, disables and hides the option buttons,  |
            // |         and sends the reconstructed token string to the server.                                           |
            // |-----------------------------------------------------------------------------------------------------------|
            int casilla_a_revisar = 2187;
            if (selected_Option == 1)
            {
                global_temp_currentUserTokens[0] = global_sg1.ToString();
                casilla_a_revisar = global_sg1;
                
                if (global_sg1 >= 75)
                {
                    global_f1 = true;
                }
            }
            else if (selected_Option == 2)
            {
                global_temp_currentUserTokens[1] = global_sg2.ToString();
                casilla_a_revisar = global_sg2;

                if (global_sg2 >= 75)
                {
                    global_f2 = true;
                }

            }
            else if (selected_Option == 3)
            {
                global_temp_currentUserTokens[2] = global_sg3.ToString();
                casilla_a_revisar = global_sg3;

                if (global_sg3 >= 75)
                {
                    global_f3 = true;
                }

            }
            else if (selected_Option == 4)
            {
                global_temp_currentUserTokens[3] = global_sg4.ToString();
                casilla_a_revisar = global_sg4;

                if (global_sg4 >= 75)
                {
                    global_f4 = true;
                }

            }

            global_temp_TokenTrozos = revisarMuertes(global_temp_TokenTrozos, casilla_a_revisar, global_temp_currentTurnID);


            this.Invoke((MethodInvoker)(() =>
            {

                opcion1.Enabled = true;
                opcion1.Visible = false;

                opcion2.Enabled = true;
                opcion2.Visible = false;

                opcion3.Enabled = true;
                opcion3.Visible = false;

                opcion4.Enabled = true;
                opcion4.Visible = false;

            }));

            //REVISAR SI HAY GANADOR
            if ((Convert.ToInt32(global_temp_currentUserTokens[0]) >= 75) && (Convert.ToInt32(global_temp_currentUserTokens[1]) >= 75) && (Convert.ToInt32(global_temp_currentUserTokens[2]) >= 75) && (Convert.ToInt32(global_temp_currentUserTokens[3]) >= 75))
            {
                //Request game end
                int puntosP1 = 0;
                int puntosP2 = 0;
                int puntosP3 = 0;
                int puntosP4 = 0;

                int currentVectorPosition = 0;

                while (currentVectorPosition < 16)
                {
                    int toAdd = 0;

                    if (Convert.ToInt32(global_temp_TokenTrozos[currentVectorPosition]) >= 75)
                    {
                        toAdd = 1;
                    }

                    if (currentVectorPosition <= 3)
                    {
                        puntosP1 = puntosP1 + toAdd;
                    }
                    else if (currentVectorPosition <= 7)
                    {
                        puntosP2 = puntosP2 + toAdd;
                    }
                    else if (currentVectorPosition <= 11)
                    {
                        puntosP3 = puntosP3 + toAdd;
                    }
                    else if (currentVectorPosition <= 15)
                    {
                        puntosP4 = puntosP4 + toAdd;
                    }

                    currentVectorPosition = currentVectorPosition + 1;

                }

                if (global_temp_currentTurnID == 1)
                {
                    puntosP1 = puntosP1 + 1;
                }
                else if (global_temp_currentTurnID == 2)
                {
                    puntosP2 = puntosP2 + 1;
                }
                else if (global_temp_currentTurnID == 3)
                {
                    puntosP3 = puntosP3 + 1;
                }
                else if (global_temp_currentTurnID == 4)
                {
                    puntosP4 = puntosP4 + 1;
                }


                string mensaje = "900/" + currentGameID+ "|" + localUsername + "|" + puntosP1.ToString() + "-" + puntosP2.ToString() + "-" + puntosP3.ToString() + "-" + puntosP4.ToString(); //900/gameID|ganador|puntosP1-puntosP2-puntosP3-puntosP4
                // We send the entered username to the server
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            else //el juego no ha terminado
            {
                //RECONSTRUIR LA CADENA A MANDAR
                string resultCadenaPosiciones = pasarelaReconstruirCadena(global_temp_currentTurnID, global_temp_TokenTrozos, global_temp_currentUserTokens);

                pasarelaEnviarPosicionesAServidor(resultCadenaPosiciones);
            }
            
        }

        private void bttn_deleteAccount_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: bttn_deleteAccount_Click                                                                              |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Sends a request to the server to sign off the user by removing their record from the         |
        // | database.                                                                                                 |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Sends a formatted request message to the server with the username to be deleted.                  |
        // |-----------------------------------------------------------------------------------------------------------|

            // Delete user from the database
            string mensaje = "3/" + Username_TextBox.Text;
            // We send the entered username to the server
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            Thread.Sleep(1000); // Espera de 1 segundo (1000 milisegundos)

            pasarelaDesconectar();

        }

        private void Disconnection_bttn_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: Disconnection_bttn_Click                                                                        |
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
                pasarelaDesconectar();
            }

        }

        private void pasarelaDesconectar()
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: pasarelaDesconectar                                                                             |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the process of disconnecting the user from the server. Sends a disconnect message    |
        // | to the server, waits for a specified period based on the username, and then closes the connection.        |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - None (relies on global variables such as localUsername and server connection).                         |
        // | Output:                                                                                                   |
        // |  - Sends a disconnect message to the server, waits for a specific duration based on the username, and     |
        // |    closes the server connection. Displays a message box confirming the disconnection.                     |
        // |-----------------------------------------------------------------------------------------------------------|
            // Disconnect
            string mensaje = "0/";
            // We send just the code of the query to the server
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Disconnection
            server.Close();
            atender.Abort();

            MessageBox.Show("The user has disconnected from the server.");
            this.Close();
        }

        private void PlayersPlayedWith_bttn_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: PlayersPlayedWith_bttn_Click                                                                    |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for the "Players Played With" button. Sends a request to the server  |
        // | to list the players with whom the user has already played.                                                |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - sender: Object representing the event sender (the button clicked).                                     |
        // |  - e: EventArgs containing the event data.                                                                |
        // | Output:                                                                                                   |
        // |  - Sends a request message to the server to retrieve the list of players the user has played with.        |
        // |-----------------------------------------------------------------------------------------------------------|
            // List players with whom I have already played
            string mensaje = "4/";
            // We send the entered username to the server
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private int revisarCasillasMuevo(int valorInicial, int valorIntermedio, string[] temp_TokenTrozos)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: revisarCasillasMuevo                                                                            |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Checks for obstacles (occupied squares) when moving a piece and adjusts the final position   |
        // | accordingly. The function searches for unoccupied squares between the starting and target positions,      |
        // | considering both linear and non-linear movement patterns.                                                 |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - valorInicial: Integer representing the starting square position.                                       |
        // |  - valorIntermedio: Integer representing the target square position.                                      |
        // |  - temp_TokenTrozos: Array of strings representing the state of the board (occupied or not).              |
        // | Output:                                                                                                   |
        // |  - Returns an integer representing the final position the piece can move to, considering obstacles.       |
        // |-----------------------------------------------------------------------------------------------------------|
            int posicionFinal = valorIntermedio; //por defecto asignamos como posición final la casilla a la cuál nos moveríamos si no hubiese barreras entre medio, (dicho valor cambiará si se encuentran barreras)

                if (valorInicial < valorIntermedio) //todo el tramo es lineal
                {
                    int casillaBuscandoActual = valorInicial + 1; //la siguiente casilla sera +1
                    int busquedasNecesarias = valorIntermedio - valorInicial; //busquedas needed = final - inicial

                    int busquedaActual = 0; //contador de busquedas
                    bool dosCoincidencias = false; //control de 2 coincidencias

                    while ((busquedaActual <= busquedasNecesarias) && (dosCoincidencias == false)) //si quedan busquedas y no ha habido ya dosCoincidencias
                    {
                        bool casillaOcupada = revisarPosicion(temp_TokenTrozos, casillaBuscandoActual);

                        if (casillaOcupada == true) //si la casilla en cuestion esta ocupada
                        {
                            dosCoincidencias = true; //variable de control a true
                            posicionFinal = casillaBuscandoActual - 1; //significa que nos podemos mover solo hasta la casilla n-1
                        }
                        else //si no esta ocupada
                        {
                            casillaBuscandoActual = casillaBuscandoActual + 1; //avanzamos +1 casilla
                        }
                        busquedaActual = busquedaActual + 1;
                    }
                }
                else if (valorInicial > valorIntermedio) //si no es todo lineal (ej: empiezo en casilla 67 y me toca ir a la 2)
                {
                    int casillaBuscandoActual = valorInicial + 1; //empezaré la búsqueda en la casilla x0 + 1
                    int busquedasHastaFinal = 68 - valorInicial; //el 1r tramo de busquedas tendra por longitud 68 - x0
                    int busquedasHastaLineal = valorIntermedio; //el 2o tramo de busquedas tendra por longitud xfinal
                    
                    if (busquedasHastaFinal < 0) //en caso que por algun motivo el tramo no lineal dé por resultado un número negativo se cambia a 0
                    {
                        busquedasHastaFinal = 0;
                    }

                    int busquedasNecesarias = busquedasHastaFinal + busquedasHastaLineal; //nº total de búsquedas será la suma de los 2 tramos

                    int busquedaActual = 0; 
                    bool dosCoincidencias = false;

                    while ((busquedaActual <= busquedasNecesarias) && (dosCoincidencias == false)) //mientras haya búsquedas por hacer y no haya habido una casilla ocupada
                    {
                        if (casillaBuscandoActual > 68) //si la casilla a la cual toca buscar es mayor de 68, se retoma la búsqueda en la casilla número 1 (ej: estoy revisando casilla 69)
                        {
                            casillaBuscandoActual = 1;
                        }

                        bool casillaOcupada = revisarPosicion(temp_TokenTrozos, casillaBuscandoActual); //asigna en una variable bool el estado de ocupación de la casilla

                        if (casillaOcupada == true) //si la casilla está ya ocupada por 2 fichas
                        {
                            dosCoincidencias = true;
                            posicionFinal = casillaBuscandoActual - 1; //podré mover la ficha hasta la casilla n-1

                            if (posicionFinal == 0) //si estava revisando la casilla 1 y esta está ocupada, la posicionFinal seria 0: por ende, debemos aplicar una corrección
                            {
                                posicionFinal = 68; //asignamos como final la 68
                            }
                        }
                        else //si la casilla actual no estaba ocupada: retomamos la búsqueda en n+1
                        {
                            casillaBuscandoActual = casillaBuscandoActual + 1;
                        }
                        busquedaActual = busquedaActual + 1;
                }

            }

            return posicionFinal;
        }

        private void iniciarSelectoresHora()
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: iniciarSelectoresHora                                                                           |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Initializes two DateTimePickers to select both date and time. Configures custom formats,     |
        // | sets maximum and minimum selectable dates, and initializes the values of the pickers to the specified     |
        // | ranges. The function also disables time selection to allow only date selection.                           |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - None (relies on predefined dateFirst and dateLast DateTimePicker controls).                            |
        // | Output:                                                                                                   |
        // |  - Configures the dateFirst and dateLast controls with the current date and specified date range.         |
        // |-----------------------------------------------------------------------------------------------------------|
            dateFirst.Value = DateTime.Now;
            dateLast.Value = DateTime.Now;

            // Opcional: Deshabilitar la selección de tiempo (solo fecha)
            dateFirst.ShowUpDown = false; // Esto muestra un calendario en lugar de un spinner
            dateLast.ShowUpDown = false; // Esto muestra un calendario en lugar de un spinner

            dateFirst.Format = DateTimePickerFormat.Custom;
            dateLast.Format = DateTimePickerFormat.Custom;

            dateFirst.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dateLast.CustomFormat = "yyyy-MM-dd HH:mm:ss";

            dateFirst.MaxDate = DateTime.Now; // Asigna el día actual como límite
            dateLast.MaxDate = DateTime.Now; // Asigna el día actual como límite

            // Configurar la fecha mínima en el DateTimePicker
            dateFirst.MinDate = new DateTime(2024, 9, 11);
            dateLast.MinDate = new DateTime(2024, 9, 11);

            dateFirst.Value = dateFirst.MinDate;
        }

        private void bttn_listGames_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: bttn_listGames_Click                                                                            |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for the "List Games" button. Sends a request to the server to fetch  |
        // | game data within a specified date range. Displays a message informing the user of data protection         |
        // | limitations, and then sends the query to the server with the start and end dates.                         |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - sender: Object representing the event sender (the button clicked).                                     |
        // |  - e: EventArgs containing the event data.                                                                |
        // | Output:                                                                                                   |
        // |  - Sends a request to the server with the start and end dates for retrieving game data within that range. |
        // |  - Displays a message box informing the user about data protection restrictions.                          |
        // |-----------------------------------------------------------------------------------------------------------|
            string cadenaEnviar;
            string fechaFirst = dateFirst.Value.ToString("yyyy-MM-dd HH:mm:ss");
            string fechaLast = dateLast.Value.ToString("yyyy-MM-dd HH:mm:ss");
            cadenaEnviar = "1000/" + fechaFirst + "|" + fechaLast;
            MessageBox.Show("Due to current data protection legislation, you will only be able to view the data corresponding to: GameID, Game Admin and Game Start Time.");
            // We send just the code of the query to the server
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(cadenaEnviar);
            server.Send(msg);
        }

        private void bttn_cerrarJuego_Click(object sender, EventArgs e)
        {
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: bttn_cerrarJuego_Click                                                                          |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles the click event for the "Close Game" button. Closes the current form or window.      |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - sender: Object representing the event sender (the button clicked).                                     |
        // |  - e: EventArgs containing the event data.                                                                |
        // | Output:                                                                                                   |
        // |  - Closes the current form or window.                                                                     |
        // |-----------------------------------------------------------------------------------------------------------|
            Close();
        }
    }

}