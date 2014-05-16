namespace iRobotCreateControl
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblBumpers = new System.Windows.Forms.Label();
            this.lblWheelDrop = new System.Windows.Forms.Label();
            this.lblCliff = new System.Windows.Forms.Label();
            this.lblWheelVelocity = new System.Windows.Forms.Label();
            this.btnCoverAndDock = new System.Windows.Forms.Button();
            this.btnControl = new System.Windows.Forms.Button();
            this.lblBattery = new System.Windows.Forms.Label();
            this.tbCOMPort = new System.Windows.Forms.TextBox();
            this.tbRemoteIP = new System.Windows.Forms.TextBox();
            this.btnConnectIP = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblLocalIP = new System.Windows.Forms.Label();
            this.lblExternalIP = new System.Windows.Forms.Label();
            this.cbAutoConnectCOM = new System.Windows.Forms.CheckBox();
            this.cbAutoConnectNetwork = new System.Windows.Forms.CheckBox();
            this.pbBattery = new System.Windows.Forms.ProgressBar();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.cbAutoStartServer = new System.Windows.Forms.CheckBox();
            this.btnKeyboardControl = new System.Windows.Forms.Button();
            this.tbKeyBoardSpeed = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbServerPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbServerPassword = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblWheelVelocityR = new System.Windows.Forms.Label();
            this.lblBumpersR = new System.Windows.Forms.Label();
            this.lblWheelDropR = new System.Windows.Forms.Label();
            this.lblCliffR = new System.Windows.Forms.Label();
            this.lblBatteryR = new System.Windows.Forms.Label();
            this.pbBatteryR = new System.Windows.Forms.ProgressBar();
            this.tbRemotePort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbRemotePassword = new System.Windows.Forms.TextBox();
            this.tabJoystick = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabJoystick.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(28, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(250, 250);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // lblBumpers
            // 
            this.lblBumpers.AutoSize = true;
            this.lblBumpers.Location = new System.Drawing.Point(151, 32);
            this.lblBumpers.Name = "lblBumpers";
            this.lblBumpers.Size = new System.Drawing.Size(69, 13);
            this.lblBumpers.TabIndex = 5;
            this.lblBumpers.Text = "Bumpers: 0 0";
            // 
            // lblWheelDrop
            // 
            this.lblWheelDrop.AutoSize = true;
            this.lblWheelDrop.Location = new System.Drawing.Point(5, 32);
            this.lblWheelDrop.Name = "lblWheelDrop";
            this.lblWheelDrop.Size = new System.Drawing.Size(91, 13);
            this.lblWheelDrop.TabIndex = 6;
            this.lblWheelDrop.Text = "WheelDrop: 0 0 0";
            // 
            // lblCliff
            // 
            this.lblCliff.AutoSize = true;
            this.lblCliff.Location = new System.Drawing.Point(151, 16);
            this.lblCliff.Name = "lblCliff";
            this.lblCliff.Size = new System.Drawing.Size(63, 13);
            this.lblCliff.TabIndex = 7;
            this.lblCliff.Text = "Cliff: 0 0 0 0";
            // 
            // lblWheelVelocity
            // 
            this.lblWheelVelocity.AutoSize = true;
            this.lblWheelVelocity.Location = new System.Drawing.Point(6, 16);
            this.lblWheelVelocity.Name = "lblWheelVelocity";
            this.lblWheelVelocity.Size = new System.Drawing.Size(99, 13);
            this.lblWheelVelocity.TabIndex = 8;
            this.lblWheelVelocity.Text = "Wheel Velocity: 0 0";
            // 
            // btnCoverAndDock
            // 
            this.btnCoverAndDock.Location = new System.Drawing.Point(229, 315);
            this.btnCoverAndDock.Name = "btnCoverAndDock";
            this.btnCoverAndDock.Size = new System.Drawing.Size(88, 23);
            this.btnCoverAndDock.TabIndex = 9;
            this.btnCoverAndDock.Text = "Dock";
            this.btnCoverAndDock.UseVisualStyleBackColor = true;
            this.btnCoverAndDock.Click += new System.EventHandler(this.btnCoverAndDock_Click);
            // 
            // btnControl
            // 
            this.btnControl.Location = new System.Drawing.Point(5, 7);
            this.btnControl.Name = "btnControl";
            this.btnControl.Size = new System.Drawing.Size(106, 48);
            this.btnControl.TabIndex = 10;
            this.btnControl.Text = "    Connect To     Local Robot";
            this.btnControl.UseVisualStyleBackColor = true;
            this.btnControl.Click += new System.EventHandler(this.btnControl_Click);
            // 
            // lblBattery
            // 
            this.lblBattery.AutoSize = true;
            this.lblBattery.Location = new System.Drawing.Point(6, 220);
            this.lblBattery.Name = "lblBattery";
            this.lblBattery.Size = new System.Drawing.Size(100, 13);
            this.lblBattery.TabIndex = 11;
            this.lblBattery.Text = "iRobot Battery: N/A";
            // 
            // tbCOMPort
            // 
            this.tbCOMPort.Location = new System.Drawing.Point(170, 11);
            this.tbCOMPort.Name = "tbCOMPort";
            this.tbCOMPort.Size = new System.Drawing.Size(100, 20);
            this.tbCOMPort.TabIndex = 12;
            this.tbCOMPort.Text = "COM3";
            // 
            // tbRemoteIP
            // 
            this.tbRemoteIP.Location = new System.Drawing.Point(193, 12);
            this.tbRemoteIP.Name = "tbRemoteIP";
            this.tbRemoteIP.Size = new System.Drawing.Size(100, 20);
            this.tbRemoteIP.TabIndex = 13;
            this.tbRemoteIP.Text = "127.0.0.1";
            this.tbRemoteIP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbRemoteIP_KeyPress);
            // 
            // btnConnectIP
            // 
            this.btnConnectIP.Location = new System.Drawing.Point(6, 11);
            this.btnConnectIP.Name = "btnConnectIP";
            this.btnConnectIP.Size = new System.Drawing.Size(110, 47);
            this.btnConnectIP.TabIndex = 14;
            this.btnConnectIP.Text = "Connect To Remote Robot";
            this.btnConnectIP.UseVisualStyleBackColor = true;
            this.btnConnectIP.Click += new System.EventHandler(this.btnConnectIP_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(14, 377);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(43, 13);
            this.lblStatus.TabIndex = 15;
            this.lblStatus.Text = "Status: ";
            // 
            // lblLocalIP
            // 
            this.lblLocalIP.AutoSize = true;
            this.lblLocalIP.Location = new System.Drawing.Point(111, 66);
            this.lblLocalIP.Name = "lblLocalIP";
            this.lblLocalIP.Size = new System.Drawing.Size(93, 13);
            this.lblLocalIP.TabIndex = 16;
            this.lblLocalIP.Text = "Local IP Address: ";
            // 
            // lblExternalIP
            // 
            this.lblExternalIP.AutoSize = true;
            this.lblExternalIP.Location = new System.Drawing.Point(111, 83);
            this.lblExternalIP.Name = "lblExternalIP";
            this.lblExternalIP.Size = new System.Drawing.Size(105, 13);
            this.lblExternalIP.TabIndex = 17;
            this.lblExternalIP.Text = "External IP Address: ";
            // 
            // cbAutoConnectCOM
            // 
            this.cbAutoConnectCOM.AutoSize = true;
            this.cbAutoConnectCOM.Location = new System.Drawing.Point(118, 36);
            this.cbAutoConnectCOM.Name = "cbAutoConnectCOM";
            this.cbAutoConnectCOM.Size = new System.Drawing.Size(88, 17);
            this.cbAutoConnectCOM.TabIndex = 18;
            this.cbAutoConnectCOM.Text = "AutoConnect";
            this.cbAutoConnectCOM.UseVisualStyleBackColor = true;
            this.cbAutoConnectCOM.CheckedChanged += new System.EventHandler(this.cbAutoConnect_CheckedChanged);
            // 
            // cbAutoConnectNetwork
            // 
            this.cbAutoConnectNetwork.AutoSize = true;
            this.cbAutoConnectNetwork.Location = new System.Drawing.Point(126, 88);
            this.cbAutoConnectNetwork.Name = "cbAutoConnectNetwork";
            this.cbAutoConnectNetwork.Size = new System.Drawing.Size(88, 17);
            this.cbAutoConnectNetwork.TabIndex = 19;
            this.cbAutoConnectNetwork.Text = "AutoConnect";
            this.cbAutoConnectNetwork.UseVisualStyleBackColor = true;
            // 
            // pbBattery
            // 
            this.pbBattery.Location = new System.Drawing.Point(6, 236);
            this.pbBattery.Name = "pbBattery";
            this.pbBattery.Size = new System.Drawing.Size(292, 23);
            this.pbBattery.TabIndex = 20;
            // 
            // btnStartServer
            // 
            this.btnStartServer.Location = new System.Drawing.Point(6, 70);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(102, 47);
            this.btnStartServer.TabIndex = 21;
            this.btnStartServer.Text = "Enable Network Control";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // cbAutoStartServer
            // 
            this.cbAutoStartServer.AutoSize = true;
            this.cbAutoStartServer.Location = new System.Drawing.Point(18, 121);
            this.cbAutoStartServer.Name = "cbAutoStartServer";
            this.cbAutoStartServer.Size = new System.Drawing.Size(81, 17);
            this.cbAutoStartServer.TabIndex = 22;
            this.cbAutoStartServer.Text = "AutoEnable";
            this.cbAutoStartServer.UseVisualStyleBackColor = true;
            // 
            // btnKeyboardControl
            // 
            this.btnKeyboardControl.Location = new System.Drawing.Point(19, 309);
            this.btnKeyboardControl.Name = "btnKeyboardControl";
            this.btnKeyboardControl.Size = new System.Drawing.Size(193, 35);
            this.btnKeyboardControl.TabIndex = 23;
            this.btnKeyboardControl.Text = "\"WASD\" Keyboard Control";
            this.btnKeyboardControl.UseVisualStyleBackColor = true;
            this.btnKeyboardControl.Click += new System.EventHandler(this.btnKeyboardControl_Click);
            this.btnKeyboardControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.button1_KeyDown);
            this.btnKeyboardControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.button1_KeyUp);
            // 
            // tbKeyBoardSpeed
            // 
            this.tbKeyBoardSpeed.Location = new System.Drawing.Point(79, 348);
            this.tbKeyBoardSpeed.Name = "tbKeyBoardSpeed";
            this.tbKeyBoardSpeed.Size = new System.Drawing.Size(34, 20);
            this.tbKeyBoardSpeed.TabIndex = 24;
            this.tbKeyBoardSpeed.Text = "200";
            this.tbKeyBoardSpeed.TextChanged += new System.EventHandler(this.tbKeyBoardSpeed_TextChanged);
            this.tbKeyBoardSpeed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbKeyBoardSpeed_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(113, 351);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Speed";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(229, 346);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(88, 23);
            this.btnStop.TabIndex = 26;
            this.btnStop.Text = "PushForward";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(112, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "Serial Port:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(130, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "IP Address:";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabJoystick);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(315, 291);
            this.tabControl.TabIndex = 29;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbServerPort);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.tbServerPassword);
            this.tabPage1.Controls.Add(this.btnControl);
            this.tabPage1.Controls.Add(this.tbCOMPort);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.lblLocalIP);
            this.tabPage1.Controls.Add(this.pbBattery);
            this.tabPage1.Controls.Add(this.lblExternalIP);
            this.tabPage1.Controls.Add(this.lblBattery);
            this.tabPage1.Controls.Add(this.cbAutoConnectCOM);
            this.tabPage1.Controls.Add(this.btnStartServer);
            this.tabPage1.Controls.Add(this.cbAutoStartServer);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(307, 265);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Local Robot";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbServerPort
            // 
            this.tbServerPort.Location = new System.Drawing.Point(173, 99);
            this.tbServerPort.Name = "tbServerPort";
            this.tbServerPort.Size = new System.Drawing.Size(56, 20);
            this.tbServerPort.TabIndex = 32;
            this.tbServerPort.Text = "25000";
            this.tbServerPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbServerPort_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(112, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Port:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblWheelVelocity);
            this.groupBox1.Controls.Add(this.lblBumpers);
            this.groupBox1.Controls.Add(this.lblWheelDrop);
            this.groupBox1.Controls.Add(this.lblCliff);
            this.groupBox1.Location = new System.Drawing.Point(6, 158);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(292, 56);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sensors";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(112, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Password:";
            // 
            // tbServerPassword
            // 
            this.tbServerPassword.Location = new System.Drawing.Point(173, 121);
            this.tbServerPassword.Name = "tbServerPassword";
            this.tbServerPassword.Size = new System.Drawing.Size(100, 20);
            this.tbServerPassword.TabIndex = 28;
            this.tbServerPassword.Text = "abracadabra";
            this.tbServerPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbServerPassword_KeyPress);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.lblBatteryR);
            this.tabPage2.Controls.Add(this.pbBatteryR);
            this.tabPage2.Controls.Add(this.tbRemotePort);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.tbRemotePassword);
            this.tabPage2.Controls.Add(this.btnConnectIP);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.tbRemoteIP);
            this.tabPage2.Controls.Add(this.cbAutoConnectNetwork);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(307, 265);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Remote Robot";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblWheelVelocityR);
            this.groupBox2.Controls.Add(this.lblBumpersR);
            this.groupBox2.Controls.Add(this.lblWheelDropR);
            this.groupBox2.Controls.Add(this.lblCliffR);
            this.groupBox2.Location = new System.Drawing.Point(6, 158);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(292, 56);
            this.groupBox2.TabIndex = 37;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sensors";
            // 
            // lblWheelVelocityR
            // 
            this.lblWheelVelocityR.AutoSize = true;
            this.lblWheelVelocityR.Location = new System.Drawing.Point(6, 16);
            this.lblWheelVelocityR.Name = "lblWheelVelocityR";
            this.lblWheelVelocityR.Size = new System.Drawing.Size(99, 13);
            this.lblWheelVelocityR.TabIndex = 8;
            this.lblWheelVelocityR.Text = "Wheel Velocity: 0 0";
            // 
            // lblBumpersR
            // 
            this.lblBumpersR.AutoSize = true;
            this.lblBumpersR.Location = new System.Drawing.Point(151, 32);
            this.lblBumpersR.Name = "lblBumpersR";
            this.lblBumpersR.Size = new System.Drawing.Size(69, 13);
            this.lblBumpersR.TabIndex = 5;
            this.lblBumpersR.Text = "Bumpers: 0 0";
            // 
            // lblWheelDropR
            // 
            this.lblWheelDropR.AutoSize = true;
            this.lblWheelDropR.Location = new System.Drawing.Point(5, 32);
            this.lblWheelDropR.Name = "lblWheelDropR";
            this.lblWheelDropR.Size = new System.Drawing.Size(91, 13);
            this.lblWheelDropR.TabIndex = 6;
            this.lblWheelDropR.Text = "WheelDrop: 0 0 0";
            // 
            // lblCliffR
            // 
            this.lblCliffR.AutoSize = true;
            this.lblCliffR.Location = new System.Drawing.Point(151, 16);
            this.lblCliffR.Name = "lblCliffR";
            this.lblCliffR.Size = new System.Drawing.Size(63, 13);
            this.lblCliffR.TabIndex = 7;
            this.lblCliffR.Text = "Cliff: 0 0 0 0";
            // 
            // lblBatteryR
            // 
            this.lblBatteryR.AutoSize = true;
            this.lblBatteryR.Location = new System.Drawing.Point(6, 220);
            this.lblBatteryR.Name = "lblBatteryR";
            this.lblBatteryR.Size = new System.Drawing.Size(114, 13);
            this.lblBatteryR.TabIndex = 35;
            this.lblBatteryR.Text = "Remote Batteries: N/A";
            // 
            // pbBatteryR
            // 
            this.pbBatteryR.Location = new System.Drawing.Point(6, 236);
            this.pbBatteryR.Name = "pbBatteryR";
            this.pbBatteryR.Size = new System.Drawing.Size(292, 23);
            this.pbBatteryR.TabIndex = 36;
            // 
            // tbRemotePort
            // 
            this.tbRemotePort.Location = new System.Drawing.Point(193, 36);
            this.tbRemotePort.Name = "tbRemotePort";
            this.tbRemotePort.Size = new System.Drawing.Size(56, 20);
            this.tbRemotePort.TabIndex = 34;
            this.tbRemotePort.Text = "25000";
            this.tbRemotePort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbRemotePort_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(131, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 33;
            this.label7.Text = "Port:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(130, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Password:";
            // 
            // tbRemotePassword
            // 
            this.tbRemotePassword.Location = new System.Drawing.Point(193, 62);
            this.tbRemotePassword.Name = "tbRemotePassword";
            this.tbRemotePassword.Size = new System.Drawing.Size(100, 20);
            this.tbRemotePassword.TabIndex = 30;
            this.tbRemotePassword.Text = "abracadabra";
            this.tbRemotePassword.TextChanged += new System.EventHandler(this.tbRemotePassword_TextChanged);
            this.tbRemotePassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbRemotePassword_KeyPress);
            // 
            // tabJoystick
            // 
            this.tabJoystick.Controls.Add(this.pictureBox1);
            this.tabJoystick.Location = new System.Drawing.Point(4, 22);
            this.tabJoystick.Name = "tabJoystick";
            this.tabJoystick.Padding = new System.Windows.Forms.Padding(3);
            this.tabJoystick.Size = new System.Drawing.Size(307, 265);
            this.tabJoystick.TabIndex = 2;
            this.tabJoystick.Text = "Visual Joystick";
            this.tabJoystick.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 403);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbKeyBoardSpeed);
            this.Controls.Add(this.btnKeyboardControl);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCoverAndDock);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "iRobot Create Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabJoystick.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblBumpers;
        private System.Windows.Forms.Label lblWheelDrop;
        private System.Windows.Forms.Label lblCliff;
        private System.Windows.Forms.Label lblWheelVelocity;
        private System.Windows.Forms.Button btnCoverAndDock;
        private System.Windows.Forms.Button btnControl;
        private System.Windows.Forms.Label lblBattery;
        private System.Windows.Forms.TextBox tbCOMPort;
        private System.Windows.Forms.TextBox tbRemoteIP;
        private System.Windows.Forms.Button btnConnectIP;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblLocalIP;
        private System.Windows.Forms.Label lblExternalIP;
        private System.Windows.Forms.CheckBox cbAutoConnectCOM;
        private System.Windows.Forms.CheckBox cbAutoConnectNetwork;
        private System.Windows.Forms.ProgressBar pbBattery;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.CheckBox cbAutoStartServer;
        private System.Windows.Forms.Button btnKeyboardControl;
        private System.Windows.Forms.TextBox tbKeyBoardSpeed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbServerPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbRemotePassword;
        private System.Windows.Forms.TabPage tabJoystick;
        private System.Windows.Forms.TextBox tbServerPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbRemotePort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblWheelVelocityR;
        private System.Windows.Forms.Label lblBumpersR;
        private System.Windows.Forms.Label lblWheelDropR;
        private System.Windows.Forms.Label lblCliffR;
        private System.Windows.Forms.Label lblBatteryR;
        private System.Windows.Forms.ProgressBar pbBatteryR;
    }
}

