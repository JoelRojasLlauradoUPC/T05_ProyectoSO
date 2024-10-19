namespace ProyectoSO
{
    partial class WelcomeForm
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.Username_TextBox = new System.Windows.Forms.TextBox();
            this.Password_TextBox = new System.Windows.Forms.TextBox();
            this.LogIn_groupBox = new System.Windows.Forms.GroupBox();
            this.Disconnect_bttn = new System.Windows.Forms.Button();
            this.LogIn_Button = new System.Windows.Forms.Button();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.Register_groupBox = new System.Windows.Forms.GroupBox();
            this.labelError = new System.Windows.Forms.Label();
            this.labelRepeatPassword = new System.Windows.Forms.Label();
            this.RepeatRegisterBox = new System.Windows.Forms.TextBox();
            this.Register_Button = new System.Windows.Forms.Button();
            this.labelPassword2 = new System.Windows.Forms.Label();
            this.labelUsername1 = new System.Windows.Forms.Label();
            this.PassRegisterBox = new System.Windows.Forms.TextBox();
            this.UserRegisterBox = new System.Windows.Forms.TextBox();
            this.GamesPlayer = new System.Windows.Forms.RadioButton();
            this.Puntuation = new System.Windows.Forms.RadioButton();
            this.SelectPlayers = new System.Windows.Forms.RadioButton();
            this.enviar_bttn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pointsBox = new System.Windows.Forms.TextBox();
            this.gameBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.namePlayerBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.IDPlayerBox = new System.Windows.Forms.TextBox();
            this.IPBox = new System.Windows.Forms.TextBox();
            this.PortBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.listPlayers = new System.Windows.Forms.Button();
            this.SendToAll = new System.Windows.Forms.Button();
            this.MessageTextBox = new System.Windows.Forms.TextBox();
            this.ChatListBox = new System.Windows.Forms.ListBox();
            this.GUI_button = new System.Windows.Forms.Button();
            this.start_chat_bttn = new System.Windows.Forms.Button();
            this.dado_bttn = new System.Windows.Forms.Button();
            this.movement_TextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.accept_bttn = new System.Windows.Forms.Button();
            this.LogIn_groupBox.SuspendLayout();
            this.Register_groupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // Username_TextBox
            // 
            this.Username_TextBox.Location = new System.Drawing.Point(39, 81);
            this.Username_TextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Username_TextBox.Name = "Username_TextBox";
            this.Username_TextBox.Size = new System.Drawing.Size(184, 26);
            this.Username_TextBox.TabIndex = 0;
            this.Username_TextBox.Text = "Alex";
            // 
            // Password_TextBox
            // 
            this.Password_TextBox.Location = new System.Drawing.Point(39, 156);
            this.Password_TextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Password_TextBox.Name = "Password_TextBox";
            this.Password_TextBox.PasswordChar = '*';
            this.Password_TextBox.Size = new System.Drawing.Size(184, 26);
            this.Password_TextBox.TabIndex = 1;
            this.Password_TextBox.Text = "host2";
            // 
            // LogIn_groupBox
            // 
            this.LogIn_groupBox.Controls.Add(this.Disconnect_bttn);
            this.LogIn_groupBox.Controls.Add(this.LogIn_Button);
            this.LogIn_groupBox.Controls.Add(this.labelPassword);
            this.LogIn_groupBox.Controls.Add(this.labelUsername);
            this.LogIn_groupBox.Controls.Add(this.Password_TextBox);
            this.LogIn_groupBox.Controls.Add(this.Username_TextBox);
            this.LogIn_groupBox.Location = new System.Drawing.Point(42, 29);
            this.LogIn_groupBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.LogIn_groupBox.Name = "LogIn_groupBox";
            this.LogIn_groupBox.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.LogIn_groupBox.Size = new System.Drawing.Size(269, 354);
            this.LogIn_groupBox.TabIndex = 2;
            this.LogIn_groupBox.TabStop = false;
            this.LogIn_groupBox.Text = "Log In";
            // 
            // Disconnect_bttn
            // 
            this.Disconnect_bttn.Location = new System.Drawing.Point(39, 268);
            this.Disconnect_bttn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Disconnect_bttn.Name = "Disconnect_bttn";
            this.Disconnect_bttn.Size = new System.Drawing.Size(184, 29);
            this.Disconnect_bttn.TabIndex = 16;
            this.Disconnect_bttn.Text = "Disconnect";
            this.Disconnect_bttn.UseVisualStyleBackColor = true;
            this.Disconnect_bttn.Click += new System.EventHandler(this.Disconnect_bttn_Click);
            // 
            // LogIn_Button
            // 
            this.LogIn_Button.Location = new System.Drawing.Point(39, 204);
            this.LogIn_Button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.LogIn_Button.Name = "LogIn_Button";
            this.LogIn_Button.Size = new System.Drawing.Size(184, 38);
            this.LogIn_Button.TabIndex = 4;
            this.LogIn_Button.Text = "Log In";
            this.LogIn_Button.UseVisualStyleBackColor = true;
            this.LogIn_Button.Click += new System.EventHandler(this.LogIn_Button_Click);
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(39, 132);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(82, 20);
            this.labelPassword.TabIndex = 3;
            this.labelPassword.Text = "Password:";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(39, 54);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(87, 20);
            this.labelUsername.TabIndex = 2;
            this.labelUsername.Text = "Username:";
            // 
            // Register_groupBox
            // 
            this.Register_groupBox.Controls.Add(this.labelError);
            this.Register_groupBox.Controls.Add(this.labelRepeatPassword);
            this.Register_groupBox.Controls.Add(this.RepeatRegisterBox);
            this.Register_groupBox.Controls.Add(this.Register_Button);
            this.Register_groupBox.Controls.Add(this.labelPassword2);
            this.Register_groupBox.Controls.Add(this.labelUsername1);
            this.Register_groupBox.Controls.Add(this.PassRegisterBox);
            this.Register_groupBox.Controls.Add(this.UserRegisterBox);
            this.Register_groupBox.Location = new System.Drawing.Point(349, 29);
            this.Register_groupBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Register_groupBox.Name = "Register_groupBox";
            this.Register_groupBox.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Register_groupBox.Size = new System.Drawing.Size(269, 405);
            this.Register_groupBox.TabIndex = 3;
            this.Register_groupBox.TabStop = false;
            this.Register_groupBox.Text = "Register";
            // 
            // labelError
            // 
            this.labelError.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelError.ForeColor = System.Drawing.Color.Red;
            this.labelError.Location = new System.Drawing.Point(7, 321);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(255, 71);
            this.labelError.TabIndex = 7;
            this.labelError.Text = "Username and Password files must not be empty and passwords must coincide.";
            this.labelError.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelError.Visible = false;
            // 
            // labelRepeatPassword
            // 
            this.labelRepeatPassword.AutoSize = true;
            this.labelRepeatPassword.Location = new System.Drawing.Point(42, 196);
            this.labelRepeatPassword.Name = "labelRepeatPassword";
            this.labelRepeatPassword.Size = new System.Drawing.Size(139, 20);
            this.labelRepeatPassword.TabIndex = 6;
            this.labelRepeatPassword.Text = "Repeat Password:";
            // 
            // RepeatRegisterBox
            // 
            this.RepeatRegisterBox.Location = new System.Drawing.Point(42, 220);
            this.RepeatRegisterBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.RepeatRegisterBox.Name = "RepeatRegisterBox";
            this.RepeatRegisterBox.PasswordChar = '*';
            this.RepeatRegisterBox.Size = new System.Drawing.Size(184, 26);
            this.RepeatRegisterBox.TabIndex = 5;
            // 
            // Register_Button
            // 
            this.Register_Button.Location = new System.Drawing.Point(39, 268);
            this.Register_Button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Register_Button.Name = "Register_Button";
            this.Register_Button.Size = new System.Drawing.Size(184, 38);
            this.Register_Button.TabIndex = 4;
            this.Register_Button.Text = "Register";
            this.Register_Button.UseVisualStyleBackColor = true;
            this.Register_Button.Click += new System.EventHandler(this.Register_Button_Click);
            // 
            // labelPassword2
            // 
            this.labelPassword2.AutoSize = true;
            this.labelPassword2.Location = new System.Drawing.Point(39, 132);
            this.labelPassword2.Name = "labelPassword2";
            this.labelPassword2.Size = new System.Drawing.Size(82, 20);
            this.labelPassword2.TabIndex = 3;
            this.labelPassword2.Text = "Password:";
            // 
            // labelUsername1
            // 
            this.labelUsername1.AutoSize = true;
            this.labelUsername1.Location = new System.Drawing.Point(39, 54);
            this.labelUsername1.Name = "labelUsername1";
            this.labelUsername1.Size = new System.Drawing.Size(87, 20);
            this.labelUsername1.TabIndex = 2;
            this.labelUsername1.Text = "Username:";
            // 
            // PassRegisterBox
            // 
            this.PassRegisterBox.Location = new System.Drawing.Point(39, 156);
            this.PassRegisterBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PassRegisterBox.Name = "PassRegisterBox";
            this.PassRegisterBox.PasswordChar = '*';
            this.PassRegisterBox.Size = new System.Drawing.Size(184, 26);
            this.PassRegisterBox.TabIndex = 1;
            // 
            // UserRegisterBox
            // 
            this.UserRegisterBox.Location = new System.Drawing.Point(39, 81);
            this.UserRegisterBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.UserRegisterBox.Name = "UserRegisterBox";
            this.UserRegisterBox.Size = new System.Drawing.Size(184, 26);
            this.UserRegisterBox.TabIndex = 0;
            // 
            // GamesPlayer
            // 
            this.GamesPlayer.AutoSize = true;
            this.GamesPlayer.Location = new System.Drawing.Point(17, 124);
            this.GamesPlayer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GamesPlayer.Name = "GamesPlayer";
            this.GamesPlayer.Size = new System.Drawing.Size(244, 24);
            this.GamesPlayer.TabIndex = 12;
            this.GamesPlayer.TabStop = true;
            this.GamesPlayer.Text = "See games played by a player";
            this.GamesPlayer.UseVisualStyleBackColor = true;
            // 
            // Puntuation
            // 
            this.Puntuation.AutoSize = true;
            this.Puntuation.Location = new System.Drawing.Point(17, 89);
            this.Puntuation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Puntuation.Name = "Puntuation";
            this.Puntuation.Size = new System.Drawing.Size(212, 24);
            this.Puntuation.TabIndex = 10;
            this.Puntuation.TabStop = true;
            this.Puntuation.Text = "See the points of a game";
            this.Puntuation.UseVisualStyleBackColor = true;
            // 
            // SelectPlayers
            // 
            this.SelectPlayers.AutoSize = true;
            this.SelectPlayers.Location = new System.Drawing.Point(17, 54);
            this.SelectPlayers.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SelectPlayers.Name = "SelectPlayers";
            this.SelectPlayers.Size = new System.Drawing.Size(235, 24);
            this.SelectPlayers.TabIndex = 11;
            this.SelectPlayers.TabStop = true;
            this.SelectPlayers.Text = "Select the players of a game";
            this.SelectPlayers.UseVisualStyleBackColor = true;
            // 
            // enviar_bttn
            // 
            this.enviar_bttn.Location = new System.Drawing.Point(122, 181);
            this.enviar_bttn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.enviar_bttn.Name = "enviar_bttn";
            this.enviar_bttn.Size = new System.Drawing.Size(112, 35);
            this.enviar_bttn.TabIndex = 13;
            this.enviar_bttn.Text = "Accept";
            this.enviar_bttn.UseVisualStyleBackColor = true;
            this.enviar_bttn.Click += new System.EventHandler(this.enviar_bttn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SelectPlayers);
            this.groupBox1.Controls.Add(this.enviar_bttn);
            this.groupBox1.Controls.Add(this.Puntuation);
            this.groupBox1.Controls.Add(this.GamesPlayer);
            this.groupBox1.Location = new System.Drawing.Point(657, 29);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(392, 246);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Queries (in development)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "Points:";
            // 
            // pointsBox
            // 
            this.pointsBox.Location = new System.Drawing.Point(120, 91);
            this.pointsBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pointsBox.Name = "pointsBox";
            this.pointsBox.Size = new System.Drawing.Size(184, 26);
            this.pointsBox.TabIndex = 11;
            // 
            // gameBox
            // 
            this.gameBox.Location = new System.Drawing.Point(120, 149);
            this.gameBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gameBox.Name = "gameBox";
            this.gameBox.Size = new System.Drawing.Size(184, 26);
            this.gameBox.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Player:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 149);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 20);
            this.label1.TabIndex = 14;
            this.label1.Text = "Game:";
            // 
            // namePlayerBox
            // 
            this.namePlayerBox.Location = new System.Drawing.Point(120, 38);
            this.namePlayerBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.namePlayerBox.Name = "namePlayerBox";
            this.namePlayerBox.Size = new System.Drawing.Size(184, 26);
            this.namePlayerBox.TabIndex = 8;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.IDPlayerBox);
            this.groupBox2.Controls.Add(this.namePlayerBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.gameBox);
            this.groupBox2.Controls.Add(this.pointsBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(1102, 29);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(327, 246);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Info to check the queries";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 20);
            this.label3.TabIndex = 16;
            this.label3.Text = "ID Player:";
            // 
            // IDPlayerBox
            // 
            this.IDPlayerBox.Location = new System.Drawing.Point(120, 204);
            this.IDPlayerBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.IDPlayerBox.Name = "IDPlayerBox";
            this.IDPlayerBox.Size = new System.Drawing.Size(184, 26);
            this.IDPlayerBox.TabIndex = 15;
            // 
            // IPBox
            // 
            this.IPBox.Location = new System.Drawing.Point(53, 76);
            this.IPBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.IPBox.Name = "IPBox";
            this.IPBox.Size = new System.Drawing.Size(133, 26);
            this.IPBox.TabIndex = 16;
            this.IPBox.Text = "192.168.56.102";
            // 
            // PortBox
            // 
            this.PortBox.Location = new System.Drawing.Point(53, 129);
            this.PortBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PortBox.Name = "PortBox";
            this.PortBox.Size = new System.Drawing.Size(133, 26);
            this.PortBox.TabIndex = 17;
            this.PortBox.Text = "9060";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.PortBox);
            this.groupBox3.Controls.Add(this.IPBox);
            this.groupBox3.Location = new System.Drawing.Point(42, 405);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Size = new System.Drawing.Size(269, 248);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Connection Manager";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 132);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 20);
            this.label6.TabIndex = 19;
            this.label6.Text = "Port:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 20);
            this.label5.TabIndex = 18;
            this.label5.Text = "IP:";
            // 
            // listPlayers
            // 
            this.listPlayers.Location = new System.Drawing.Point(732, 340);
            this.listPlayers.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listPlayers.Name = "listPlayers";
            this.listPlayers.Size = new System.Drawing.Size(249, 41);
            this.listPlayers.TabIndex = 19;
            this.listPlayers.Text = "Show the list of players";
            this.listPlayers.UseVisualStyleBackColor = true;
            this.listPlayers.Click += new System.EventHandler(this.listPlayers_Click);
            // 
            // SendToAll
            // 
            this.SendToAll.Location = new System.Drawing.Point(754, 560);
            this.SendToAll.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SendToAll.Name = "SendToAll";
            this.SendToAll.Size = new System.Drawing.Size(195, 35);
            this.SendToAll.TabIndex = 20;
            this.SendToAll.Text = "Send Message";
            this.SendToAll.UseVisualStyleBackColor = true;
            this.SendToAll.Click += new System.EventHandler(this.SendToAll_Click);
            // 
            // MessageTextBox
            // 
            this.MessageTextBox.Location = new System.Drawing.Point(754, 509);
            this.MessageTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MessageTextBox.Name = "MessageTextBox";
            this.MessageTextBox.Size = new System.Drawing.Size(424, 26);
            this.MessageTextBox.TabIndex = 21;
            // 
            // ChatListBox
            // 
            this.ChatListBox.FormattingEnabled = true;
            this.ChatListBox.ItemHeight = 20;
            this.ChatListBox.Location = new System.Drawing.Point(1218, 418);
            this.ChatListBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ChatListBox.Name = "ChatListBox";
            this.ChatListBox.Size = new System.Drawing.Size(360, 244);
            this.ChatListBox.TabIndex = 22;
            // 
            // GUI_button
            // 
            this.GUI_button.Location = new System.Drawing.Point(1144, 282);
            this.GUI_button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.GUI_button.Name = "GUI_button";
            this.GUI_button.Size = new System.Drawing.Size(253, 115);
            this.GUI_button.TabIndex = 23;
            this.GUI_button.Text = "Open Game GUI";
            this.GUI_button.UseVisualStyleBackColor = true;
            this.GUI_button.Click += new System.EventHandler(this.GUI_button_Click);
            // 
            // start_chat_bttn
            // 
            this.start_chat_bttn.Location = new System.Drawing.Point(754, 454);
            this.start_chat_bttn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.start_chat_bttn.Name = "start_chat_bttn";
            this.start_chat_bttn.Size = new System.Drawing.Size(102, 35);
            this.start_chat_bttn.TabIndex = 24;
            this.start_chat_bttn.Text = "Chat";
            this.start_chat_bttn.UseVisualStyleBackColor = true;
            this.start_chat_bttn.Click += new System.EventHandler(this.start_chat_bttn_Click);
            // 
            // dado_bttn
            // 
            this.dado_bttn.Location = new System.Drawing.Point(349, 458);
            this.dado_bttn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dado_bttn.Name = "dado_bttn";
            this.dado_bttn.Size = new System.Drawing.Size(104, 48);
            this.dado_bttn.TabIndex = 25;
            this.dado_bttn.Text = "Dice";
            this.dado_bttn.UseVisualStyleBackColor = true;
            this.dado_bttn.Click += new System.EventHandler(this.dado_bttn_Click);
            // 
            // movement_TextBox
            // 
            this.movement_TextBox.Location = new System.Drawing.Point(349, 562);
            this.movement_TextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.movement_TextBox.Name = "movement_TextBox";
            this.movement_TextBox.Size = new System.Drawing.Size(162, 26);
            this.movement_TextBox.TabIndex = 26;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(345, 535);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(236, 20);
            this.label7.TabIndex = 27;
            this.label7.Text = "To check the movement\'s query:";
            // 
            // accept_bttn
            // 
            this.accept_bttn.Location = new System.Drawing.Point(388, 598);
            this.accept_bttn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.accept_bttn.Name = "accept_bttn";
            this.accept_bttn.Size = new System.Drawing.Size(94, 36);
            this.accept_bttn.TabIndex = 28;
            this.accept_bttn.Text = "Accept";
            this.accept_bttn.UseVisualStyleBackColor = true;
            this.accept_bttn.Click += new System.EventHandler(this.accept_bttn_Click);
            // 
            // WelcomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1624, 711);
            this.Controls.Add(this.accept_bttn);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.movement_TextBox);
            this.Controls.Add(this.dado_bttn);
            this.Controls.Add(this.start_chat_bttn);
            this.Controls.Add(this.GUI_button);
            this.Controls.Add(this.ChatListBox);
            this.Controls.Add(this.MessageTextBox);
            this.Controls.Add(this.SendToAll);
            this.Controls.Add(this.listPlayers);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Register_groupBox);
            this.Controls.Add(this.LogIn_groupBox);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "WelcomeForm";
            this.Text = "Welcome";
            this.LogIn_groupBox.ResumeLayout(false);
            this.LogIn_groupBox.PerformLayout();
            this.Register_groupBox.ResumeLayout(false);
            this.Register_groupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Username_TextBox;
        private System.Windows.Forms.TextBox Password_TextBox;
        private System.Windows.Forms.GroupBox LogIn_groupBox;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Button LogIn_Button;
        private System.Windows.Forms.GroupBox Register_groupBox;
        private System.Windows.Forms.Label labelRepeatPassword;
        private System.Windows.Forms.TextBox RepeatRegisterBox;
        private System.Windows.Forms.Button Register_Button;
        private System.Windows.Forms.Label labelPassword2;
        private System.Windows.Forms.Label labelUsername1;
        private System.Windows.Forms.TextBox PassRegisterBox;
        private System.Windows.Forms.TextBox UserRegisterBox;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.RadioButton GamesPlayer;
        private System.Windows.Forms.RadioButton Puntuation;
        private System.Windows.Forms.RadioButton SelectPlayers;
        private System.Windows.Forms.Button enviar_bttn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pointsBox;
        private System.Windows.Forms.TextBox gameBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox namePlayerBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox IDPlayerBox;
        private System.Windows.Forms.Button Disconnect_bttn;
        private System.Windows.Forms.TextBox IPBox;
        private System.Windows.Forms.TextBox PortBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button listPlayers;
        private System.Windows.Forms.Button SendToAll;
        private System.Windows.Forms.TextBox MessageTextBox;
        private System.Windows.Forms.ListBox ChatListBox;
        private System.Windows.Forms.Button GUI_button;
        private System.Windows.Forms.Button start_chat_bttn;
        private System.Windows.Forms.Button dado_bttn;
        private System.Windows.Forms.TextBox movement_TextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button accept_bttn;
    }
}

