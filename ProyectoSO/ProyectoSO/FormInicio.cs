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
using Parchís;

namespace ProyectoSO
{
    public partial class WelcomeForm : Form
    {
        Socket server; // Server
        Thread receiveThread; // Thread to receive messages

        public WelcomeForm()
        {
            InitializeComponent();
            receiveThread = new Thread(ReceiveMessages);
            receiveThread.IsBackground = true;

        }

        // ---------------------------------------------------------------------------------------------------------------------
        // ---------------------------- REGISTER + LOG IN + CONNECTION / DISCONNECTION SERVER ----------------------------------
        // ---------------------------------------------------------------------------------------------------------------------
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
            // We create an IPEndPoint with the server's IP address and the server port we want to connect to
            IPAddress direc = IPAddress.Parse(IPBox.Text);
            IPEndPoint ipep = new IPEndPoint(direc, Convert.ToInt32(PortBox.Text));

            // We create the socket
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

            if (puntos_verificacion == 4) // If the username and passwords meet the requirements
            {
                try
                {
                    server.Connect(ipep); // We attempt to connect the socket

                    if (puntos_verificacion == 4)
                    {
                        // Register
                        string mensaje = "1/" + UserRegisterBox.Text + "/" + PassRegisterBox.Text;
                        // We send the entered username and password to the server
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);

                        // We receive the server's response
                        byte[] msg2 = new byte[80];
                        server.Receive(msg2);
                        mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                        if (mensaje == "1")
                        {
                            MessageBox.Show("User created successfully.");
                        }
                        else if (mensaje == "0")
                        {
                            MessageBox.Show("The entered username is already registered.");
                        }
                        else
                        {
                            MessageBox.Show("There was an error creating the user. Please try again later.");
                        }
                    }

                    this.BackColor = Color.White;

                }
                catch (SocketException)
                {
                    // If there is an exception, an error is printed and the program exits with return. 
                    MessageBox.Show("Connection to the server failed.");
                    return;
                }
            }
            
            else
            {
                labelError.Visible = true;
            }
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
            // We create an IPEndPoint with the server's IP address and the server port we want to connect to
            IPAddress direc = IPAddress.Parse(IPBox.Text);
            IPEndPoint ipep = new IPEndPoint(direc, Convert.ToInt32(PortBox.Text));

            // We create the socket
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);// We attempt to connect the socket
                                     // MessageBox.Show("Connection to the server successful.");
            }
            catch (SocketException)
            {
                // If there is an exception, an error is printed and the program exits with return. 
                // MessageBox.Show("Connection to the server failed.");
                return;
            }

            // Log In
            string mensaje = "2/" + Username_TextBox.Text + "/" + Password_TextBox.Text;
            // We send the entered username and password to the server
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // We receive the server's response
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

            if (mensaje == "1")
            {
                this.BackColor = Color.LightGreen;
                MessageBox.Show("Log In successful.");
                
                //if (!receiveThread.IsAlive)
                //{
                //  receiveThread.Start();
                //}
            }
            else if (mensaje == "0")
            {
                MessageBox.Show("Log In failed.");
            }
            else
            {
                MessageBox.Show("There was an error with the log in. Please try again later.");
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
            string mensaje = "6/";
            // We send just the code of the query to the server
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // We receive the server's response
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

            this.BackColor = Color.White;
            MessageBox.Show("The user has disconnected from the server.");
        }

        // ---------------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------- QUERIES --------------------------------------------------------
        // ---------------------------------------------------------------------------------------------------------------------
        private void enviar_bttn_Click(object sender, EventArgs e)
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: enviar_bttn_Click                                                                               |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: Handles different queries based on user input. It sends a request to the server based on the |
        // | selected option and processes the server's response.                                                      |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Displays a message with the information retrieved from the server based on the selected query.    |
        // |-----------------------------------------------------------------------------------------------------------|
        {
            if (SelectPlayers.Checked)
            {
                // Select the players of a game
                string mensaje = "3/" + gameBox.Text;
                // We send the entered game to the server
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // We receive the server's response
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                MessageBox.Show("The players of the game are:\n" + mensaje);
            }
            else if (Puntuation.Checked)
            {
                // See the points of a game
                string mensaje = "4/" + gameBox.Text;
                // We send the entered game to the server
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // We receive the server's response
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                MessageBox.Show("The points of the game are:\n" + mensaje);
            }
            else if (GamesPlayer.Checked)
            {
                // See games played by a player
                string mensaje = "5/" + IDPlayerBox.Text;
                // We send the entered player to the server
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // We receive the server's response
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                MessageBox.Show("The player took part in games:\n" + mensaje);
            }
        }

        private void listPlayers_Click(object sender, EventArgs e)
        // |-----------------------------------------------------------------------------------------------------------|
        // | Function: listPlayers_Click                                                                               |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Description: It sends a request to the server to retrieve the current list of players connected and       |
        // | displays it in a message box.                                                                             |
        // |-----------------------------------------------------------------------------------------------------------|
        // | Input:                                                                                                    |
        // |  - object sender: The source of the event (button click).                                                 |
        // |  - EventArgs e: Event arguments for the click event.                                                      |
        // | Output: Displays a message box containing the list of players received from the server.                   |
        // |-----------------------------------------------------------------------------------------------------------|
        {
            // Show the list of players
            string mensaje = "7/";
            // We send just the code of the query to the server
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // We receive the server's response
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
            MessageBox.Show("List of Players:\n" + mensaje);
        }

        private void SendToAll_Click(object sender, EventArgs e)
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
            string mensaje = "100/" + Username_TextBox.Text + "/" + MessageTextBox.Text;
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            ChatListBox.Items.Add("Me: " + MessageTextBox.Text);
            MessageTextBox.Clear();
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
            while (true)
            {
                try
                {
                    byte[] msg2 = new byte[80];
                    int bytesReceived = server.Receive(msg2);
                    if (bytesReceived > 0)
                    {
                        string mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                        Invoke(new Action(() => ChatListBox.Items.Add(mensaje + Environment.NewLine)));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error receiving message: " + ex.Message);
                    break;
                }
            }
        }

        private void GUI_Button_Click(object sender, EventArgs e)
        {
            FormPartida formularioPartida = new FormPartida();  // Aquí corregí el nombre
            formularioPartida.Show();
        }

    }
}
