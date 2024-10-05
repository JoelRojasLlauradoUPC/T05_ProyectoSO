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
            this.button2 = new System.Windows.Forms.Button();
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
            this.Disconnect_bttn = new System.Windows.Forms.Button();
            this.IPBox = new System.Windows.Forms.TextBox();
            this.PortBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.LogIn_groupBox.SuspendLayout();
            this.Register_groupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // Username_TextBox
            // 
            this.Username_TextBox.Location = new System.Drawing.Point(35, 65);
            this.Username_TextBox.Name = "Username_TextBox";
            this.Username_TextBox.Size = new System.Drawing.Size(164, 22);
            this.Username_TextBox.TabIndex = 0;
            this.Username_TextBox.Text = "Joroll";
            // 
            // Password_TextBox
            // 
            this.Password_TextBox.Location = new System.Drawing.Point(35, 125);
            this.Password_TextBox.Name = "Password_TextBox";
            this.Password_TextBox.PasswordChar = '*';
            this.Password_TextBox.Size = new System.Drawing.Size(164, 22);
            this.Password_TextBox.TabIndex = 1;
            this.Password_TextBox.Text = "123";
            // 
            // LogIn_groupBox
            // 
            this.LogIn_groupBox.Controls.Add(this.Disconnect_bttn);
            this.LogIn_groupBox.Controls.Add(this.LogIn_Button);
            this.LogIn_groupBox.Controls.Add(this.labelPassword);
            this.LogIn_groupBox.Controls.Add(this.labelUsername);
            this.LogIn_groupBox.Controls.Add(this.Password_TextBox);
            this.LogIn_groupBox.Controls.Add(this.Username_TextBox);
            this.LogIn_groupBox.Location = new System.Drawing.Point(37, 23);
            this.LogIn_groupBox.Name = "LogIn_groupBox";
            this.LogIn_groupBox.Size = new System.Drawing.Size(239, 283);
            this.LogIn_groupBox.TabIndex = 2;
            this.LogIn_groupBox.TabStop = false;
            this.LogIn_groupBox.Text = "LogIn";
            // 
            // LogIn_Button
            // 
            this.LogIn_Button.Location = new System.Drawing.Point(35, 163);
            this.LogIn_Button.Name = "LogIn_Button";
            this.LogIn_Button.Size = new System.Drawing.Size(164, 30);
            this.LogIn_Button.TabIndex = 4;
            this.LogIn_Button.Text = "Log In";
            this.LogIn_Button.UseVisualStyleBackColor = true;
            this.LogIn_Button.Click += new System.EventHandler(this.LogIn_Button_Click);
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(35, 106);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(70, 16);
            this.labelPassword.TabIndex = 3;
            this.labelPassword.Text = "Password:";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(35, 43);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(73, 16);
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
            this.Register_groupBox.Location = new System.Drawing.Point(310, 23);
            this.Register_groupBox.Name = "Register_groupBox";
            this.Register_groupBox.Size = new System.Drawing.Size(239, 324);
            this.Register_groupBox.TabIndex = 3;
            this.Register_groupBox.TabStop = false;
            this.Register_groupBox.Text = "Register";
            // 
            // labelError
            // 
            this.labelError.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelError.ForeColor = System.Drawing.Color.Red;
            this.labelError.Location = new System.Drawing.Point(6, 257);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(227, 57);
            this.labelError.TabIndex = 7;
            this.labelError.Text = "Username and Password files must not be empty and passwords must coincide.";
            this.labelError.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelError.Visible = false;
            // 
            // labelRepeatPassword
            // 
            this.labelRepeatPassword.AutoSize = true;
            this.labelRepeatPassword.Location = new System.Drawing.Point(37, 157);
            this.labelRepeatPassword.Name = "labelRepeatPassword";
            this.labelRepeatPassword.Size = new System.Drawing.Size(118, 16);
            this.labelRepeatPassword.TabIndex = 6;
            this.labelRepeatPassword.Text = "Repeat Password:";
            // 
            // RepeatRegisterBox
            // 
            this.RepeatRegisterBox.Location = new System.Drawing.Point(37, 176);
            this.RepeatRegisterBox.Name = "RepeatRegisterBox";
            this.RepeatRegisterBox.PasswordChar = '*';
            this.RepeatRegisterBox.Size = new System.Drawing.Size(164, 22);
            this.RepeatRegisterBox.TabIndex = 5;
            // 
            // Register_Button
            // 
            this.Register_Button.Location = new System.Drawing.Point(35, 214);
            this.Register_Button.Name = "Register_Button";
            this.Register_Button.Size = new System.Drawing.Size(164, 30);
            this.Register_Button.TabIndex = 4;
            this.Register_Button.Text = "Register";
            this.Register_Button.UseVisualStyleBackColor = true;
            this.Register_Button.Click += new System.EventHandler(this.Register_Button_Click);
            // 
            // labelPassword2
            // 
            this.labelPassword2.AutoSize = true;
            this.labelPassword2.Location = new System.Drawing.Point(35, 106);
            this.labelPassword2.Name = "labelPassword2";
            this.labelPassword2.Size = new System.Drawing.Size(70, 16);
            this.labelPassword2.TabIndex = 3;
            this.labelPassword2.Text = "Password:";
            // 
            // labelUsername1
            // 
            this.labelUsername1.AutoSize = true;
            this.labelUsername1.Location = new System.Drawing.Point(35, 43);
            this.labelUsername1.Name = "labelUsername1";
            this.labelUsername1.Size = new System.Drawing.Size(73, 16);
            this.labelUsername1.TabIndex = 2;
            this.labelUsername1.Text = "Username:";
            // 
            // PassRegisterBox
            // 
            this.PassRegisterBox.Location = new System.Drawing.Point(35, 125);
            this.PassRegisterBox.Name = "PassRegisterBox";
            this.PassRegisterBox.PasswordChar = '*';
            this.PassRegisterBox.Size = new System.Drawing.Size(164, 22);
            this.PassRegisterBox.TabIndex = 1;
            // 
            // UserRegisterBox
            // 
            this.UserRegisterBox.Location = new System.Drawing.Point(35, 65);
            this.UserRegisterBox.Name = "UserRegisterBox";
            this.UserRegisterBox.Size = new System.Drawing.Size(164, 22);
            this.UserRegisterBox.TabIndex = 0;
            // 
            // GamesPlayer
            // 
            this.GamesPlayer.AutoSize = true;
            this.GamesPlayer.Location = new System.Drawing.Point(15, 99);
            this.GamesPlayer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GamesPlayer.Name = "GamesPlayer";
            this.GamesPlayer.Size = new System.Drawing.Size(213, 20);
            this.GamesPlayer.TabIndex = 12;
            this.GamesPlayer.TabStop = true;
            this.GamesPlayer.Text = "See games played by a player";
            this.GamesPlayer.UseVisualStyleBackColor = true;
            // 
            // Puntuation
            // 
            this.Puntuation.AutoSize = true;
            this.Puntuation.Location = new System.Drawing.Point(15, 71);
            this.Puntuation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Puntuation.Name = "Puntuation";
            this.Puntuation.Size = new System.Drawing.Size(201, 20);
            this.Puntuation.TabIndex = 10;
            this.Puntuation.TabStop = true;
            this.Puntuation.Text = "See the puntuation of a game";
            this.Puntuation.UseVisualStyleBackColor = true;
            // 
            // SelectPlayers
            // 
            this.SelectPlayers.AutoSize = true;
            this.SelectPlayers.Location = new System.Drawing.Point(15, 43);
            this.SelectPlayers.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SelectPlayers.Name = "SelectPlayers";
            this.SelectPlayers.Size = new System.Drawing.Size(198, 20);
            this.SelectPlayers.TabIndex = 11;
            this.SelectPlayers.TabStop = true;
            this.SelectPlayers.Text = "Select the players of a game";
            this.SelectPlayers.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(108, 145);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 28);
            this.button2.TabIndex = 13;
            this.button2.Text = "Enviar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SelectPlayers);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.Puntuation);
            this.groupBox1.Controls.Add(this.GamesPlayer);
            this.groupBox1.Location = new System.Drawing.Point(584, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(348, 197);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Queries (in development)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 16);
            this.label4.TabIndex = 12;
            this.label4.Text = "Points:";
            // 
            // pointsBox
            // 
            this.pointsBox.Location = new System.Drawing.Point(107, 77);
            this.pointsBox.Name = "pointsBox";
            this.pointsBox.Size = new System.Drawing.Size(164, 22);
            this.pointsBox.TabIndex = 11;
            // 
            // gameBox
            // 
            this.gameBox.Location = new System.Drawing.Point(107, 124);
            this.gameBox.Name = "gameBox";
            this.gameBox.Size = new System.Drawing.Size(164, 22);
            this.gameBox.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "Player:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 16);
            this.label1.TabIndex = 14;
            this.label1.Text = "Game:";
            // 
            // namePlayerBox
            // 
            this.namePlayerBox.Location = new System.Drawing.Point(107, 30);
            this.namePlayerBox.Name = "namePlayerBox";
            this.namePlayerBox.Size = new System.Drawing.Size(164, 22);
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
            this.groupBox2.Location = new System.Drawing.Point(980, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(291, 197);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Info to check the queries";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 163);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 16);
            this.label3.TabIndex = 16;
            this.label3.Text = "ID Player:";
            // 
            // IDPlayerBox
            // 
            this.IDPlayerBox.Location = new System.Drawing.Point(107, 163);
            this.IDPlayerBox.Name = "IDPlayerBox";
            this.IDPlayerBox.Size = new System.Drawing.Size(164, 22);
            this.IDPlayerBox.TabIndex = 15;
            // 
            // Disconnect_bttn
            // 
            this.Disconnect_bttn.Location = new System.Drawing.Point(35, 214);
            this.Disconnect_bttn.Name = "Disconnect_bttn";
            this.Disconnect_bttn.Size = new System.Drawing.Size(164, 23);
            this.Disconnect_bttn.TabIndex = 16;
            this.Disconnect_bttn.Text = "Disconnect";
            this.Disconnect_bttn.UseVisualStyleBackColor = true;
            this.Disconnect_bttn.Click += new System.EventHandler(this.Disconnect_bttn_Click);
            // 
            // IPBox
            // 
            this.IPBox.Location = new System.Drawing.Point(47, 61);
            this.IPBox.Name = "IPBox";
            this.IPBox.Size = new System.Drawing.Size(119, 22);
            this.IPBox.TabIndex = 16;
            this.IPBox.Text = "192.168.56.102";
            this.IPBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // PortBox
            // 
            this.PortBox.Location = new System.Drawing.Point(47, 103);
            this.PortBox.Name = "PortBox";
            this.PortBox.Size = new System.Drawing.Size(119, 22);
            this.PortBox.TabIndex = 17;
            this.PortBox.Text = "4247";
            this.PortBox.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.PortBox);
            this.groupBox3.Controls.Add(this.IPBox);
            this.groupBox3.Location = new System.Drawing.Point(37, 324);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(239, 198);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Connection Manager";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(22, 16);
            this.label5.TabIndex = 18;
            this.label5.Text = "IP:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 106);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 16);
            this.label6.TabIndex = 19;
            this.label6.Text = "Port:";
            // 
            // WelcomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1444, 569);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Register_groupBox);
            this.Controls.Add(this.LogIn_groupBox);
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
        private System.Windows.Forms.Button button2;
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
    }
}

