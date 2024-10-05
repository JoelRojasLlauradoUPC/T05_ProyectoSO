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
            this.labelRepeatPassword = new System.Windows.Forms.Label();
            this.RepeatRegisterBox = new System.Windows.Forms.TextBox();
            this.Register_Button = new System.Windows.Forms.Button();
            this.labelPassword2 = new System.Windows.Forms.Label();
            this.labelUsername1 = new System.Windows.Forms.Label();
            this.PassRegisterBox = new System.Windows.Forms.TextBox();
            this.UserRegisterBox = new System.Windows.Forms.TextBox();
            this.labelError = new System.Windows.Forms.Label();
            this.LogIn_groupBox.SuspendLayout();
            this.Register_groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // Username_TextBox
            // 
            this.Username_TextBox.Location = new System.Drawing.Point(35, 65);
            this.Username_TextBox.Name = "Username_TextBox";
            this.Username_TextBox.Size = new System.Drawing.Size(164, 22);
            this.Username_TextBox.TabIndex = 0;
            // 
            // Password_TextBox
            // 
            this.Password_TextBox.Location = new System.Drawing.Point(35, 125);
            this.Password_TextBox.Name = "Password_TextBox";
            this.Password_TextBox.PasswordChar = '*';
            this.Password_TextBox.Size = new System.Drawing.Size(164, 22);
            this.Password_TextBox.TabIndex = 1;
            // 
            // LogIn_groupBox
            // 
            this.LogIn_groupBox.Controls.Add(this.LogIn_Button);
            this.LogIn_groupBox.Controls.Add(this.labelPassword);
            this.LogIn_groupBox.Controls.Add(this.labelUsername);
            this.LogIn_groupBox.Controls.Add(this.Password_TextBox);
            this.LogIn_groupBox.Controls.Add(this.Username_TextBox);
            this.LogIn_groupBox.Location = new System.Drawing.Point(37, 23);
            this.LogIn_groupBox.Name = "LogIn_groupBox";
            this.LogIn_groupBox.Size = new System.Drawing.Size(239, 244);
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
            this.Register_groupBox.Size = new System.Drawing.Size(239, 354);
            this.Register_groupBox.TabIndex = 3;
            this.Register_groupBox.TabStop = false;
            this.Register_groupBox.Text = "Register";
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
            this.Register_Button.Text = "Register:";
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
            // WelcomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Register_groupBox);
            this.Controls.Add(this.LogIn_groupBox);
            this.Name = "WelcomeForm";
            this.Text = "Welcome";
            this.LogIn_groupBox.ResumeLayout(false);
            this.LogIn_groupBox.PerformLayout();
            this.Register_groupBox.ResumeLayout(false);
            this.Register_groupBox.PerformLayout();
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
    }
}

