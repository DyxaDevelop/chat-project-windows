namespace LogInForm
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.materialRaisedButton1 = new MaterialSkin.Controls.MaterialRaisedButton();
            this.InputPassword = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.InputNickname = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.SuspendLayout();
            // 
            // materialRaisedButton1
            // 
            this.materialRaisedButton1.Depth = 0;
            this.materialRaisedButton1.Location = new System.Drawing.Point(157, 268);
            this.materialRaisedButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialRaisedButton1.Name = "materialRaisedButton1";
            this.materialRaisedButton1.Primary = true;
            this.materialRaisedButton1.Size = new System.Drawing.Size(214, 53);
            this.materialRaisedButton1.TabIndex = 0;
            this.materialRaisedButton1.Text = "Log In";
            this.materialRaisedButton1.UseVisualStyleBackColor = true;
            this.materialRaisedButton1.Click += new System.EventHandler(this.materialRaisedButton1_Click);
            // 
            // InputPassword
            // 
            this.InputPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputPassword.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.InputPassword.Depth = 0;
            this.InputPassword.Font = new System.Drawing.Font("Adobe Gothic Std B", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.InputPassword.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.InputPassword.Hint = "Password";
            this.InputPassword.Location = new System.Drawing.Point(157, 194);
            this.InputPassword.MouseState = MaterialSkin.MouseState.HOVER;
            this.InputPassword.Name = "InputPassword";
            this.InputPassword.PasswordChar = '\0';
            this.InputPassword.SelectedText = "";
            this.InputPassword.SelectionLength = 0;
            this.InputPassword.SelectionStart = 0;
            this.InputPassword.Size = new System.Drawing.Size(213, 23);
            this.InputPassword.TabIndex = 1;
            this.InputPassword.UseSystemPasswordChar = false;
            // 
            // InputNickname
            // 
            this.InputNickname.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.InputNickname.Depth = 0;
            this.InputNickname.Hint = "Nickname";
            this.InputNickname.Location = new System.Drawing.Point(157, 125);
            this.InputNickname.MouseState = MaterialSkin.MouseState.HOVER;
            this.InputNickname.Name = "InputNickname";
            this.InputNickname.PasswordChar = '\0';
            this.InputNickname.SelectedText = "";
            this.InputNickname.SelectionLength = 0;
            this.InputNickname.SelectionStart = 0;
            this.InputNickname.Size = new System.Drawing.Size(214, 23);
            this.InputNickname.TabIndex = 3;
            this.InputNickname.UseSystemPasswordChar = false;
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel1.Location = new System.Drawing.Point(465, 324);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(64, 19);
            this.materialLabel1.TabIndex = 4;
            this.materialLabel1.Text = "Register";
            this.materialLabel1.Click += new System.EventHandler(this.materialLabel1_Click_2);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(540, 350);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.InputNickname);
            this.Controls.Add(this.InputPassword);
            this.Controls.Add(this.materialRaisedButton1);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.MaximumSize = new System.Drawing.Size(540, 350);
            this.MinimumSize = new System.Drawing.Size(540, 350);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login Form";
            this.TransparencyKey = System.Drawing.Color.Black;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialRaisedButton materialRaisedButton1;
        private MaterialSkin.Controls.MaterialSingleLineTextField InputPassword;
        private MaterialSkin.Controls.MaterialSingleLineTextField InputNickname;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
    }
}

