using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using iRobot;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace iRobotCreateControl
{
    public partial class Form1 : Form
    {

        string getMyIP_URL = "http://checkip.dyndns.com/";

        public PowerStatus power = SystemInformation.PowerStatus;


        iRobotCreate robot;
        Bitmap joyBitmap;
        Graphics joyGraphics;
        bool mouseDown = false;
        long lastNetworkMessageTimestamp = 0;
        UdpClient udpClientListener;
        IPEndPoint listenerEndPoint = null;
        UdpClient udpClientSender = null;
        IPEndPoint senderEndPoint = null;
        bool updating = false;

        bool networkConfiguationTasksComplete = false;


        bool bControlAllowed = false;

        Point mousePoint = new Point();
        Thread senderThread;
        Thread recieveHeartbeatThread = null;
        Thread delayedConnectThread;
        Thread networkConfigurationThread;
        
        long lastSentNetworkMessageTimestamp = 0;
        long lastRecievedNetworkMessageTimestamp = 0;


        bool[] KeyState = new bool[500];

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }


        [StructLayout(LayoutKind.Explicit, Size = 28)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public uint type;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
        };

        public enum Win32Consts
        {
            // For use with the INPUT struct, see SendInput for an example
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2,
        }


        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowTextLength(IntPtr hWnd);
        //  int GetWindowText(        
        //      __in   HWND hWnd,        
        //      __out  LPTSTR lpString,        
        //      __in   int nMaxCount        
        //  );        
        [DllImport("user32.dll")]        
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        //  DWORD GetWindowThreadProcessId(        
        //      __in   HWND hWnd,        
        //      __out  LPDWORD lpdwProcessId        
        //  );        
        [DllImport("user32.dll")]        
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        //HANDLE WINAPI OpenProcess(        
        //  __in  DWORD dwDesiredAccess,        
        //  __in  BOOL bInheritHandle,        
        //  __in  DWORD dwProcessId        //);        
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr handle);
        //  DWORD WINAPI GetModuleBaseName(        
        //      __in      HANDLE hProcess,        
        //      __in_opt  HMODULE hModule,        
        //      __out     LPTSTR lpBaseName,        
        //      __in      DWORD nSize        
        //  );        
        [DllImport("psapi.dll")]        
        private static extern uint GetModuleBaseName(IntPtr hWnd, IntPtr hModule, StringBuilder lpFileName, int nSize);
        //  DWORD WINAPI GetModuleFileNameEx(        
        //      __in      HANDLE hProcess,        
        //      __in_opt  HMODULE hModule,        
        //      __out     LPTSTR lpFilename,        
        //      __in      DWORD nSize        
        //  );        
        [DllImport("psapi.dll")]        
        private static extern uint GetModuleFileNameEx(IntPtr hWnd, IntPtr hModule, StringBuilder lpFileName, int nSize);
        public static string GetTopWindowText()
        {
            IntPtr hWnd = GetForegroundWindow();
            int length = GetWindowTextLength(hWnd);
            StringBuilder text = new StringBuilder(length + 1);
            GetWindowText(hWnd, text, text.Capacity);
            return text.ToString();
        }


        [DllImport("user32.dll", CharSet=CharSet.Auto,ExactSpelling=true)]
        public static extern IntPtr SetFocus(HandleRef hWnd);

        [DllImport("User32.dll")]
        public static extern Int32 SetForegroundWindow(IntPtr hWnd);
        
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private void BringToFront(string windowName)
        {
            foreach (Process p in Process.GetProcesses("."))
            {
                try
                {
                    if (p.MainWindowTitle.Length > 0)
                    {

                        string windowTitle = p.MainWindowTitle.ToString();
                        Console.WriteLine("Window Title:" + windowTitle);
                        Console.WriteLine("Process Name:" + p.ProcessName.ToString());
                        //                    Console.WriteLine("Window Handle:" + p.MainWindowHandle.ToString());

                        if (windowTitle.CompareTo(windowName) == 0)
                        {
                          Console.WriteLine("\tmatch---------------------");
                            IntPtr hWnd = p.MainWindowHandle;
                            SetForegroundWindow(hWnd);

                            Thread.Sleep(500);
                            Console.Out.WriteLine("pressing key");

                            INPUT structInput;
                           structInput = new INPUT();
                            structInput.type = (uint)Win32Consts.INPUT_KEYBOARD;

                            // Key down shift, ctrl, and/or alt
                            structInput.ki.wScan = 0;
                            structInput.ki.time = 0;
                            structInput.ki.dwFlags = 0x0;
                            structInput.ki.wVk = (ushort)0x0D;
//                            SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));

                        }
                    }
                }
                catch { }
            }
            //SetForegroundWindow(FindWindow(className, CaptionName)); 
        }

        public static string GetTopWindowName()
        {
            IntPtr hWnd = GetForegroundWindow();
            uint lpdwProcessId;
            GetWindowThreadProcessId(hWnd, out lpdwProcessId);
            IntPtr hProcess = OpenProcess(0x0410, false, lpdwProcessId);
            StringBuilder text = new StringBuilder(1000);

            //GetModuleBaseName(hProcess, IntPtr.Zero, text, text.Capacity);            
            GetModuleFileNameEx(hProcess, IntPtr.Zero, text, text.Capacity);
            CloseHandle(hProcess);
            return text.ToString();
        }





        public Form1()
        {
            InitializeComponent();

            joyBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            joyGraphics = Graphics.FromImage(joyBitmap);

            Point center = new Point(joyBitmap.Width / 2, joyBitmap.Height / 2);
            joyGraphics.Clear(Color.White);
            int size = 10;
            joyGraphics.DrawEllipse(new Pen(Color.Red), center.X - size / 2, center.Y - size / 2, size, size);
            pictureBox1.Image = joyBitmap;
            mousePoint = new Point(joyBitmap.Width / 2, joyBitmap.Height / 2);
        }


        void PerformNetworkConfigurationTasks()
        {
            lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = "Status: Getting IP addresses"));

            //get ip address
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            for (int i = 0; i < localIPs.Length; i++)
            {
                if (localIPs[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.Out.WriteLine("Local IP: " + localIPs[i]);
                    lblLocalIP.Invoke((MethodInvoker)(() => lblLocalIP.Text = "Local IP Address: " + localIPs[i]));
                    break;
                }
            }

            if (true)//query external ip
            {
                Console.Out.WriteLine("Querying for external IP: " + getMyIP_URL);
                string externalIP = WebQuery(getMyIP_URL);
                char[] separators = { ' ', '<' };
                string[] parts = externalIP.Split(new char[] { ' ', '<' });
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].CompareTo("Address:") == 0)
                    {
                        externalIP = parts[i + 1];
                        break;
                    }
                }
                Console.Out.WriteLine("Returned External IP: " + externalIP);
                lblExternalIP.Invoke((MethodInvoker)(() => lblExternalIP.Text = "External IP Address: " + externalIP));
            }

            Console.Out.WriteLine("Configuring router port forwarding: " + tbServerPort.Text);
            lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = "Status: Configuring router..."));
            if (UPnP.OpenFirewallPort("iRobot", UPnP.Protocol.UDP, int.Parse(tbServerPort.Text)))
            {
                lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = "Status: Port forwarding set: " + tbServerPort.Text));
            }
            else
            {
                lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = "Status: Deleting old port forwarding..."));
                if (!UPnP.CloseFirewallPort(UPnP.Protocol.UDP, int.Parse(tbServerPort.Text)))
                {
                    lblStatus.Invoke((MethodInvoker)(() =>lblStatus.Text = "Status: Port forwarding change FAILED"));
                    MessageBox.Show("Error occurred during router configuration.");
                    return;
                }
                Thread.Sleep(500);
                lblStatus.Invoke((MethodInvoker)(() =>lblStatus.Text = "Status: Adding new port forwarding..."));
                if (UPnP.OpenFirewallPort("iRobot", UPnP.Protocol.UDP, int.Parse(tbServerPort.Text)))
                {
                    lblStatus.Invoke((MethodInvoker)(() =>lblStatus.Text = "Status: Port forwarded to this computer"));
                }
                else
                {
                    lblStatus.Invoke((MethodInvoker)(() =>lblStatus.Text = "Status: Port forwarding change FAILED"));
                    MessageBox.Show("Error occurred during router configuration.");
                }
            }

            if (true)
            {
                lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = "Status: Starting server on port " + tbServerPort.Text));
                listenerEndPoint = new IPEndPoint(IPAddress.Any, int.Parse(tbServerPort.Text));
                udpClientListener = new UdpClient(listenerEndPoint);
                Console.Out.WriteLine("Listening on port " + tbServerPort.Text);
                udpClientListener.BeginReceive(new AsyncCallback(ReceiveCallback), this);
            }

            lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = "Status: Server Started on port " + tbServerPort.Text));
            btnStartServer.Invoke((MethodInvoker)(() => btnStartServer.Text = "Disable Remote Control"));
            btnStartServer.Invoke((MethodInvoker)(() => btnStartServer.Enabled = true));

            networkConfiguationTasksComplete = true;
        }

        void QueryTopWindow(object o)
        {
            Thread.Sleep(4000);
            while(this.Visible)
            {
//                BringToFront("Sponsored session");
                BringToFront("TeamViewer Panel");

                Thread.Sleep(2000);
            }
        }

        void SaveSettings()
        {
            if (File.Exists("config.txt"))
            {
                File.Delete("config.txt");
                Thread.Sleep(100);
            }

            FileStream fs = new FileStream("config.txt", FileMode.CreateNew);
            StreamWriter filewrite = new StreamWriter(fs);
            filewrite.WriteLine("AutoConnectLocalRobot: " + cbAutoConnectCOM.Checked);
            filewrite.WriteLine("COMPort: "+ tbCOMPort.Text);
            filewrite.WriteLine("AutoStartServer: " + cbAutoStartServer.Checked);
            filewrite.WriteLine("ServerPort: " + tbServerPort.Text);
            filewrite.WriteLine("ServerPassword: " + tbServerPassword.Text);
            filewrite.WriteLine("AutoConnectRemoteRobot: " + cbAutoConnectNetwork.Checked);
            filewrite.WriteLine("RemoteIP: " + tbRemoteIP.Text);
            filewrite.WriteLine("RemotePort: " + tbRemotePort.Text);
            filewrite.WriteLine("RemotePassword: " + tbRemotePassword.Text);
            filewrite.WriteLine("KeyboardControlMotorSpeed: " + tbKeyBoardSpeed.Text);
            filewrite.Flush();
            filewrite.Close();
            fs.Close();
        }

        private string WebQuery(string url)
        {
            // used to build entire input
            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];
            string tempString = null;

            try
            {
                // prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 10000;

                // execute the request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();

                int count = 0;

                do
                {
                    // fill the buffer with data
                    count = resStream.Read(buf, 0, buf.Length);

                    // make sure we read some data
                    if (count != 0)
                    {
                        // translate from bytes to ASCII text
                        tempString = Encoding.ASCII.GetString(buf, 0, count);

                        // continue building the string
                        sb.Append(tempString);
                    }
                }
                while (count > 0); // any more data to read?

            }
            catch (Exception x)
            {
                Console.Out.WriteLine("could no contact IP address server: " + x.ToString());
            }

            return tempString;
        }


        void DelayedConnectCallback()
        {
            Thread.Sleep(1000);
            if (cbAutoConnectCOM.Checked)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnControl_Click(this, null);
                });
            }
            if (cbAutoConnectNetwork.Checked)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnConnectIP_Click(this, null);
                });
            }
        }


        public static void ResponseCallback(IAsyncResult ar)
        {
            try
            {
                Form1 f = (Form1)ar.AsyncState;
                if (f.udpClientSender == null)
                    return;
                Byte[] receiveBytes = f.udpClientSender.EndReceive(ar, ref f.senderEndPoint);
                string receiveString = Encoding.ASCII.GetString(receiveBytes);

                string[] parts = receiveString.Split(' ');
                if (parts[0].CompareTo("control") == 0)
                {
                    if (parts[1].CompareTo("ok") == 0)
                    {
                        f.Invoke((MethodInvoker)delegate
                        {
                            Console.Out.WriteLine("Controlling robot at " + f.tbRemoteIP.Text);
                            f.lblStatus.Text = "Status: Controlling robot at " + f.tbRemoteIP.Text;
                        });
                    }

                    if (parts[1].CompareTo("no") == 0)
                    {
                        f.Invoke((MethodInvoker)delegate
                        {
                            f.lblStatus.Text = "Status: " + f.tbRemoteIP.Text + " refused, password is incorrect!";
                        });
                    }
                }
                if (parts[0].CompareTo("s") == 0)
                {
                    f.Invoke((MethodInvoker)delegate
                    {

                        int batteryStateEnum = int.Parse(parts[1]);
                        int batteryCharge = int.Parse(parts[2]);
                        int sensorStatus = int.Parse(parts[3]);

                        f.robot.sensorState.BumpLeft = ((sensorStatus & 0x0001)>0);
                        f.robot.sensorState.BumpRight = ((sensorStatus & 0x0002)>0);
                        f.robot.sensorState.WheelDropLeft = ((sensorStatus & 0x0004)>0);
                        f.robot.sensorState.WheelDropCaster = ((sensorStatus & 0x0008)>0);
                        f.robot.sensorState.WheelDropRight = ((sensorStatus & 0x0010)>0);
                        f.robot.sensorState.CliffLeft = ((sensorStatus & 0x0020)>0);
                        f.robot.sensorState.CliffFrontLeft = ((sensorStatus & 0x0040)>0);
                        f.robot.sensorState.CliffFrontRight = ((sensorStatus & 0x0080)>0);
                        f.robot.sensorState.CliffRight = ((sensorStatus & 0x0100) > 0);

                        string batteryState = "Idle";
                        if (batteryStateEnum == 1)
                            batteryState = "Reconditioning";
                        if (batteryStateEnum == 2)
                            batteryState = "Charging";
                        if (batteryStateEnum == 3)
                            batteryState = "Trickle";
                        if (batteryStateEnum == 4)
                            batteryState = "Waiting";
                        if (batteryStateEnum == 5)
                            batteryState = "Fault";

                        f.lblBatteryR.Text = "Remote Batteries: " + batteryState + " " + batteryCharge + "%";
                        if (batteryCharge > 100) batteryCharge = 100;
                        if (batteryCharge < 0) batteryCharge = 0;
                        f.pbBatteryR.Value = batteryCharge;

                        f.lblBumpersR.Text = "Bumpers: " + (f.robot.sensorState.BumpLeft ? 0 : 1) + " " + (f.robot.sensorState.BumpRight ? 0 : 1);
                        f.lblWheelDropR.Text = "WheelDrop: " + (f.robot.sensorState.WheelDropLeft ? 0 : 1) + " " + (f.robot.sensorState.WheelDropCaster ? 0 : 1) + " " + (f.robot.sensorState.WheelDropRight ? 0 : 1);
                        f.lblCliffR.Text = "Cliff: " + (f.robot.sensorState.CliffLeft ? 0 : 1) + " " + (f.robot.sensorState.CliffFrontLeft ? 0 : 1) + " " + (f.robot.sensorState.CliffFrontRight ? 0 : 1) + " " + (f.robot.sensorState.CliffRight ? 0 : 1);

                        f.lblStatus.Text = "Status: Controlling robot at " + f.tbRemoteIP.Text +":" + f.tbRemotePort.Text;

                    });
                }

                if (f.udpClientSender == null)
                    return;
                f.udpClientSender.BeginReceive(new AsyncCallback(ResponseCallback), f);
            }
            catch (Exception x)
            {
                Console.Out.WriteLine(x);
            }

        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            Form1 f = (Form1)ar.AsyncState;

            f.lastRecievedNetworkMessageTimestamp = System.Environment.TickCount;
            if (f.recieveHeartbeatThread == null)
            {
                //we received a network message, now require that we regularly receive it
                f.recieveHeartbeatThread = new Thread(new ThreadStart(f.RecieveHeartbeatCallback));
                f.recieveHeartbeatThread.Start();
            }
            if (f.udpClientListener.Client == null)
                return;

            Byte[] receiveBytes = f.udpClientListener.EndReceive(ar, ref f.listenerEndPoint);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);
            string[] parts = receiveString.Split(' ');
            if (parts.Length == 2)
            {
                if (parts[0].CompareTo("control") == 0)
                {
                    Console.Out.WriteLine("received connection request");
                    if (parts[1].CompareTo(f.tbServerPassword.Text) == 0)
                    {
                        f.btnControl_Click(f, null);
                        f.bControlAllowed = true;

                        Console.Out.WriteLine("remote control granted!");
                        f.robot.StartInFullMode();

                        f.lblStatus.Invoke((MethodInvoker)(() => f.lblStatus.Text = "Status: Controlled by remote host"));

                        Byte[] sendBytes = Encoding.ASCII.GetBytes("control ok");
                        f.udpClientListener.Send(sendBytes, sendBytes.Length,f.listenerEndPoint);
                        
                        //crazy hack for when the robot doesn't send the battery status.
                        if(f.robot.sensorState.BatteryCapacity == 0)
                            	f.robot.sensorState.BatteryCapacity = 1;
                        if(f.robot.sensorState.BatteryCharge == 0)
                            	f.robot.sensorState.BatteryCharge = 1;

                        if (true)//send status message
                        {

                            int sensorState = (f.robot.sensorState.BumpLeft ? 0x0001 : 0) |
                                            (f.robot.sensorState.BumpRight ? 0x0002 : 0) |
                                            (f.robot.sensorState.WheelDropLeft ? 0x0004 : 0) |
                                            (f.robot.sensorState.WheelDropCaster ? 0x0008 : 0) |
                                            (f.robot.sensorState.WheelDropRight ? 0x0010 : 0) |
                                            (f.robot.sensorState.CliffLeft ? 0x0020 : 0) |
                                            (f.robot.sensorState.CliffFrontLeft ? 0x0040 : 0) |
                                            (f.robot.sensorState.CliffFrontRight ? 0x0080 : 0) |
                                            (f.robot.sensorState.CliffRight ? 0x0100 : 0);

                            bool computerIsCharging = (f.power.PowerLineStatus == PowerLineStatus.Online);
                            int robotCharge = (f.robot.sensorState.BatteryCharge * 100 / f.robot.sensorState.BatteryCapacity);
                            int batteryCharge = (int)(f.power.BatteryLifePercent * 100);
                            if (robotCharge < batteryCharge)
                                batteryCharge = robotCharge;
                            Byte[] sendBytes2 = Encoding.ASCII.GetBytes("s " + f.robot.sensorState.ChargingState + " " + batteryCharge + " " + sensorState);
                            f.udpClientListener.Send(sendBytes2, sendBytes2.Length, f.listenerEndPoint);
                        }

                    }
                    else
                    {
                        f.bControlAllowed = false;
                        Console.Out.WriteLine("password mismatch, control denied");

                        Byte[] sendBytes = Encoding.ASCII.GetBytes("control no");
                        f.udpClientListener.Send(sendBytes, sendBytes.Length, f.listenerEndPoint);
                    }
                }
                else
                {
                    int x = int.Parse(parts[0]);
                    int y = int.Parse(parts[1]);
                    if ((!f.updating)&&f.bControlAllowed)
                    {
                        f.Invoke((MethodInvoker)delegate
                        {
                            f.UpdateJoystickAndDriveCommand(new MouseEventArgs(MouseButtons.Left, 0, x, y, 0));
                        });

                        if (true)//send status message
                        {

                            int sensorState = (f.robot.sensorState.BumpLeft ? 0x0001 : 0) |
                                            (f.robot.sensorState.BumpRight ? 0x0002 : 0) |
                                            (f.robot.sensorState.WheelDropLeft ? 0x0004 : 0) |
                                            (f.robot.sensorState.WheelDropCaster ? 0x0008 : 0) |
                                            (f.robot.sensorState.WheelDropRight ? 0x0010 : 0) |
                                            (f.robot.sensorState.CliffLeft ? 0x0020 : 0) |
                                            (f.robot.sensorState.CliffFrontLeft ? 0x0040 : 0) |
                                            (f.robot.sensorState.CliffFrontRight ? 0x0080 : 0) |
                                            (f.robot.sensorState.CliffRight ? 0x0100 : 0);
                            
                            int robotCharge = (f.robot.sensorState.BatteryCharge * 100 / f.robot.sensorState.BatteryCapacity);
                            int batteryCharge = (int)(f.power.BatteryLifePercent * 100);
                            if (robotCharge < batteryCharge)
                                batteryCharge = robotCharge;

                            Byte[] sendBytes2 = Encoding.ASCII.GetBytes("s " + f.robot.sensorState.ChargingState + " " + batteryCharge + " " + sensorState);
                            f.udpClientListener.Send(sendBytes2, sendBytes2.Length, f.listenerEndPoint);
                        }
                    }
                }
            }
            if (parts.Length == 1)
            {
                if (f.bControlAllowed)
                {
                    if (parts[0].CompareTo("cover_and_dock") == 0)
                        f.btnCoverAndDock_Click(f, null);
                    if (parts[0].CompareTo("home") == 0)
                        f.btnStop_Click(f, null);
                    if (parts[0].CompareTo("robot_power") == 0)
                    	f.BtnRobotPwrClick(f, null);
                }
            }

            if (f.Visible)
                f.udpClientListener.BeginReceive(new AsyncCallback(ReceiveCallback), f);
        }


        private void OnWheelDropChanged(object sender, EventArgs e)
        {
            if (!robot.prevSensorState.WheelDropLeft && robot.sensorState.WheelDropLeft)
                Console.WriteLine("wheel left dropped!");
            if (!robot.prevSensorState.WheelDropRight && robot.sensorState.WheelDropRight)
                Console.WriteLine("wheel right dropped!");
            if (!robot.prevSensorState.WheelDropCaster && robot.sensorState.WheelDropCaster)
                Console.WriteLine("wheel caster dropped!");
        }

        private void OnCliffDetectChanged(object sender, EventArgs e)
        {
            if (!robot.prevSensorState.CliffLeft && robot.sensorState.CliffLeft)
                Console.WriteLine("cliff left!");
            if (!robot.prevSensorState.CliffFrontLeft && robot.sensorState.CliffFrontLeft)
                Console.WriteLine("cliff front left!");
            if (!robot.prevSensorState.CliffFrontRight && robot.sensorState.CliffFrontRight)
                Console.WriteLine("cliff front right!");
            if (!robot.prevSensorState.CliffRight && robot.sensorState.CliffRight)
                Console.WriteLine("cliff right!");
        }

        private void OnBumperChanged(object sender, EventArgs e)
        {
            if(!robot.prevSensorState.BumpLeft && robot.sensorState.BumpLeft)
                Console.WriteLine("bump left!");
            if (!robot.prevSensorState.BumpRight && robot.sensorState.BumpRight)
                Console.WriteLine("bump right!");

        }

        private void OnSensorUpdate(object sender, EventArgs e)
        {
            try
            {
                if (this.IsDisposed)
                    return;
                this.Invoke((MethodInvoker)delegate
                {
                    UpdateSensorIU();
                });
            }
            catch (Exception x)
            {
                Console.Out.WriteLine(x);

            }
        }

        private void UpdateSensorIU()
        {
            if (!this.Visible)
                return;
            if (this.IsDisposed)
                return;
            lblBumpers.Text = "Bumpers: " + (robot.sensorState.BumpLeft ? 0 : 1) + " " + (robot.sensorState.BumpRight ? 0 : 1);
            lblWheelDrop.Text = "WheelDrop: " + (robot.sensorState.WheelDropLeft ? 0 : 1) + " " + (robot.sensorState.WheelDropCaster ? 0 : 1) + " " + (robot.sensorState.WheelDropRight ? 0 : 1);
            lblCliff.Text = "Cliff: " + (robot.sensorState.CliffLeft ? 0 : 1) + " " + (robot.sensorState.CliffFrontLeft ? 0 : 1) + " " + (robot.sensorState.CliffFrontRight ? 0 : 1) + " " + (robot.sensorState.CliffRight ? 0 : 1);
            string state = "Idle";
            if (robot.sensorState.ChargingState == 1)
                state = "Reconditioning";
            if (robot.sensorState.ChargingState == 2)
                state = "Charging";
            if (robot.sensorState.ChargingState == 3)
                state = "Trickle";
            if (robot.sensorState.ChargingState == 4)
                state = "Waiting";
            if (robot.sensorState.ChargingState == 5)
                state = "Fault";

            int batteryCharge =  (robot.sensorState.BatteryCharge * 100 / robot.sensorState.BatteryCapacity);

            lblBattery.Text = "iRobot Battery: " + state + " " + batteryCharge + "% (" + ((robot.sensorState.Current > 0) ? "+" : "") + robot.sensorState.Current / 1000.0 + "A " + robot.sensorState.Voltage / 1000.0 + "V " + robot.sensorState.BatteryTempurature + "C)";

            if (batteryCharge > 100) batteryCharge = 100;
            if (batteryCharge < 0) batteryCharge = 0;
            pbBattery.Value = batteryCharge;

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            UpdateJoystickAndDriveCommand(e);
        }


        void UpdateJoystickAndDriveCommand(MouseEventArgs e)
        {
            updating = true;
            try
            {
                mousePoint.X = e.X;
                mousePoint.Y = e.Y;
                Point center = new Point(joyBitmap.Width / 2, joyBitmap.Height / 2);
                joyGraphics.Clear(Color.White);
                int size = 10;
                int dx = (e.X - center.X)/2;
                int dy = -1*(center.Y - e.Y);
                float mag = (float)Math.Sqrt(dx * dx + dy * dy);
                float maxRad = 200;
                float scale = maxRad / mag;
                if (scale < 1)
                {
                    dx = (int)(dx * scale);
                    dy = (int)(dy * scale);
                }

                Point newE = new Point(center.X + 2*dx,center.Y + dy);

                joyGraphics.DrawEllipse(new Pen(Color.Red), newE.X - size / 2, newE.Y - size / 2, size, size);
                joyGraphics.DrawLine(new Pen(Color.Red), center, newE);
                pictureBox1.Image = joyBitmap;

                float maxVel = 400;
                dx = (int)(dx*maxVel/ maxRad);
                dy = (int)(dy*maxVel / maxRad);

                int left = (dy - dx);
                int right = (dy + dx);

                robot.DriveDirect(left, right);
                lblWheelVelocity.Invoke((MethodInvoker)(() => lblWheelVelocity.Text = "WheelVelocity: " + left + " " + right));

                if ((Environment.TickCount - lastNetworkMessageTimestamp) > 50)
                {
                    lastNetworkMessageTimestamp = Environment.TickCount;
                    if (udpClientSender != null)
                    {
                        if (udpClientSender.Client.Connected)
                        {
                            lblWheelVelocityR.Invoke((MethodInvoker)(() => lblWheelVelocityR.Text = "WheelVelocity: " + left + " " + right));

                            Byte[] sendBytes = Encoding.ASCII.GetBytes(mousePoint.X + " " + mousePoint.Y);
                            udpClientSender.Send(sendBytes, sendBytes.Length);
                            lastSentNetworkMessageTimestamp = System.Environment.TickCount;
                        }
                    }
                }
            }
            catch (Exception x)
            {
                Console.Out.WriteLine(x);
            }
            updating = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                UpdateJoystickAndDriveCommand(e);
            }
            else
            {
                robot.DriveDirect(0, 0);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            mousePoint = new Point(joyBitmap.Width / 2, joyBitmap.Height / 2);
            UpdateJoystickAndDriveCommand(new MouseEventArgs(MouseButtons.Left, 0, mousePoint.X, mousePoint.Y, 0));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (robot != null)
            {
                lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = "Status: Powering Off Robot"));
                Console.Out.WriteLine("Shutting down local robot...");
                Thread.Sleep(100);
                robot.Disconnect();
            }

            if (udpClientSender != null)
            {
                udpClientSender.Close();
            }
            if (udpClientListener != null)
            {
                udpClientListener.Close();
            }
            SaveSettings();
        }

        private void btnCoverAndDock_Click(object sender, EventArgs e)
        {
            if (robot.connected)
            {
                robot.StartInPassiveMode();
                Thread.Sleep(500);
                robot.CoverAndDock();
            }
            //try remote send
            if (udpClientSender == null)
                return;
            if (udpClientSender.Client == null)
                return;
            if (udpClientSender.Client.Connected)
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes("cover_and_dock");
                udpClientSender.Send(sendBytes, sendBytes.Length);
                lastSentNetworkMessageTimestamp = System.Environment.TickCount;
            }
        }

        private void btnControl_Click(object sender, EventArgs e)
        {
        	if (!robot.connected)
            {
                if (robot.Connect(tbCOMPort.Text))
                {
                    Console.Out.WriteLine("Connected to iRobot Create");
                    lblStatus.Text = "Status: " + "connected to Create on port " + tbCOMPort.Text;
                }
                else
                {
                    MessageBox.Show("Could not Connect to iRobot Create on port:" + tbCOMPort.Text);

                    this.Invoke((MethodInvoker)delegate
                    {
                        lblStatus.Text = "Status: Failed to connect to iRobot Create";
                    });

                }
            }
            robot.StartInFullMode();
            robot.StartSensorStreaming();
        }

        void RecieveHeartbeatCallback()
        {
            try
            {
                bool beenReset = false;
                while (this.Visible && !this.IsDisposed)
                {
                    Thread.Sleep(250);
                    long timeSinceLastRecievedMessage = System.Environment.TickCount - lastRecievedNetworkMessageTimestamp;
                    if (timeSinceLastRecievedMessage > 3000)
                    {
                        //don't bother resetting if we are already charging
                        if (robot.sensorState.ChargingState == 2)
                            continue;

                        if (!beenReset)
                        {
                            //this will make it go into "off" state
                            //which is lower power and can will accept a charge if already docked.
                            Console.Out.WriteLine("Remote host disconnected, powering down");
                            robot.SoftReset();
                            beenReset = true;
                            lblBattery.Invoke((MethodInvoker)(() => lblBattery.Text = "iRobot Battery: N/A"));
                            pbBattery.Invoke((MethodInvoker)(() => pbBattery.Value = 0));
                            lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = "Status: Powered Off - Remote Control Disconnected"));
                        }
                        continue;
                    }

                    if (timeSinceLastRecievedMessage > 1000)
                    {
                        Console.Out.WriteLine("remote control heartbeat lost, stopping (poor network connection?)");
                        mousePoint = new Point(joyBitmap.Width / 2, joyBitmap.Height / 2);
                        this.Invoke((MethodInvoker)delegate
                        {
                            UpdateJoystickAndDriveCommand(new MouseEventArgs(MouseButtons.Left, 0, mousePoint.X, mousePoint.Y, 0));
                        });

                        lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = "Status: Stopping - Remote Control Comunication Lost"));

                        continue;
                    }

                    //we are recieving and running, so no reset
                    beenReset = false;
                }
            }
            catch (Exception x)
            {
                Console.Out.WriteLine(x);
            }
        }

        void SendCallback()
        {
            while (this.Visible)
            {
                if (udpClientSender.Client == null)
                    return;
                if (udpClientSender.Client.Connected)
                {
                    if ((System.Environment.TickCount - lastSentNetworkMessageTimestamp) > 100)
                    {
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(mousePoint.X + " " + mousePoint.Y);
                        udpClientSender.Send(sendBytes, sendBytes.Length);
                        lastSentNetworkMessageTimestamp = System.Environment.TickCount;
                    }
                }
                else
                {
                    return;
                }
                Thread.Sleep(100);
            }
        }

        private void btnConnectIP_Click(object sender, EventArgs e)
        {

            if (udpClientSender == null)
            {
                senderEndPoint = new IPEndPoint(IPAddress.Any, int.Parse(tbRemotePort.Text));
                udpClientSender = new UdpClient(senderEndPoint);
            }


            if (udpClientSender.Client.Connected)
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes("control "+tbRemotePassword.Text);
                udpClientSender.Send(sendBytes, sendBytes.Length);
                lastSentNetworkMessageTimestamp = System.Environment.TickCount;
            }
            else
            {
                lblStatus.Text = "Status: Connecting to " + tbRemoteIP.Text + ":" + tbRemotePort.Text + " ...";

                Console.Out.WriteLine("Connecting to:" + tbRemoteIP.Text + ":" + tbRemotePort.Text);
                udpClientSender.Connect(tbRemoteIP.Text, int.Parse(tbRemotePort.Text));
                if (udpClientSender.Client.Connected)
                {
                    Console.Out.WriteLine("Awaiting repsonse...");
                    lblStatus.Text = "Status: Awaiting repsonse from " + tbRemoteIP.Text + ":" + tbRemotePort.Text + " ...";

                    senderThread = new Thread(new ThreadStart(SendCallback));
                    senderThread.Start();

                    udpClientSender.BeginReceive(new AsyncCallback(ResponseCallback), this);

                    Thread.Sleep(500);
                    Byte[] sendBytes = Encoding.ASCII.GetBytes("control " + tbRemotePassword.Text);
                    udpClientSender.Send(sendBytes, sendBytes.Length);
                    lastSentNetworkMessageTimestamp = System.Environment.TickCount;
                }
            }
            
            //if we get to here without errors then we are connected
            btnConnectIP.BackColor = Color.Green;
        }

        private void cbAutoConnect_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            robot = new iRobotCreate();
            robot.OnSensorUpdateRecieved += new iRobotCreate.SensorUpdateHandler(OnSensorUpdate);
            robot.OnBumperChanged += new iRobotCreate.BumperChangedHandler(OnBumperChanged);
            robot.OnCliffDetectChanged += new iRobotCreate.CliffDetectChangedHandler(OnCliffDetectChanged);
            robot.OnWheelDropChanged += new iRobotCreate.WheelDropChangedHandler(OnWheelDropChanged);


            if (File.Exists("config.txt"))
            {
                FileStream fs = new FileStream("config.txt", FileMode.OpenOrCreate);
                StreamReader filereader = new StreamReader(fs);
                try
                {
                    string[] parts;
                    parts = (filereader.ReadLine()).Split(' ');
                    cbAutoConnectCOM.Checked = Boolean.Parse(parts[1]);
                    
                    parts = (filereader.ReadLine()).Split(' ');
                    tbCOMPort.Text = parts[1];
                    
                    parts = (filereader.ReadLine()).Split(' ');
                    cbAutoStartServer.Checked = Boolean.Parse(parts[1]);
                    
                    parts = (filereader.ReadLine()).Split(' ');
                    tbServerPort.Text = int.Parse(parts[1]).ToString();

                    parts = (filereader.ReadLine()).Split(' ');
                    tbServerPassword.Text = parts[1];                     
                    
                    parts = (filereader.ReadLine()).Split(' ');
                    cbAutoConnectNetwork.Checked = Boolean.Parse(parts[1]);

                    if (cbAutoConnectNetwork.Checked)
                    {
                        tabControl.SelectedTab = tabControl.TabPages[1];
                    }
                    
                    parts = (filereader.ReadLine()).Split(' ');
                    tbRemoteIP.Text = parts[1];
                    
                    parts = (filereader.ReadLine()).Split(' ');
                    tbRemotePort.Text = int.Parse(parts[1]).ToString();

                    parts = (filereader.ReadLine()).Split(' ');
                    tbRemotePassword.Text = parts[1];                     


                    parts = (filereader.ReadLine()).Split(' ');
                    tbKeyBoardSpeed.Text = parts[1];

                    Thread.Sleep(1000);

                    //connect the local robot
                    if (cbAutoConnectCOM.Checked || cbAutoConnectNetwork.Checked)
                    {
                        delayedConnectThread = new Thread(new ThreadStart(DelayedConnectCallback));
                        delayedConnectThread.Start();
                    }
                    if (cbAutoStartServer.Checked)
                    {
                        btnStartServer_Click(this, null);
                    }
                }
                catch (Exception x)
                {
                    Console.Out.WriteLine("File read error: " + x.ToString());
                }
                filereader.Close();
                fs.Close();
            }
            else
            {
                if(udpClientListener != null)
                    udpClientListener.Close();
                SaveSettings();
            }

        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            if (networkConfiguationTasksComplete)
            {
                udpClientListener.Close();
                lblStatus.Text = "Status: Server Stopped";

                btnStartServer.Text = "Enable Remote Control";
                btnConnectIP.Enabled = true;
                networkConfiguationTasksComplete = false;
            }
            else
            {
                btnStartServer.Enabled = false;
                btnConnectIP.Enabled = false;
                btnStartServer.Text = "Starting server...";
                networkConfigurationThread = new Thread(new ThreadStart(PerformNetworkConfigurationTasks));
                networkConfigurationThread.Start();
            }
        }

        /// <summary>
        /// This button is used to toggle the robot's power.
        /// It calls the wakeup() method which uses the Serial
        /// DTR signal line connected to the power toggle line
        /// in the robot's DB25 port to turn the robot on or off
        /// every time the line goes from low to high.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnRobotPwrClick(object sender, System.EventArgs e)
        {
        	robot.wakeup();
        	//try remote send too
            if (udpClientSender == null)
                return;
            if (udpClientSender.Client == null)
                return;
            if (udpClientSender.Client.Connected)
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes("robot_power");
                udpClientSender.Send(sendBytes, sendBytes.Length);
                lastSentNetworkMessageTimestamp = System.Environment.TickCount;
            }
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            int offset = 100;
            try
            {
                offset = int.Parse(tbKeyBoardSpeed.Text);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);
            }

            KeyState[(int)e.KeyCode] = true;
            int x = joyBitmap.Width / 2;
            int y = joyBitmap.Height / 2;

            if (KeyState[(int)Keys.W])
                y -= offset;
            if (KeyState[(int)Keys.S])
                y += offset;
            if (KeyState[(int)Keys.A])
                x -= offset;
            if (KeyState[(int)Keys.D])
                x += offset;
            {
                MouseEventArgs me = new MouseEventArgs(MouseButtons.Left, 0, x,y, 0);
                UpdateJoystickAndDriveCommand(me);
            }
        }

        private void button1_KeyUp(object sender, KeyEventArgs e)
        {
            KeyState[(int)e.KeyCode] = false;

            int offset = 100;
            try
            {
                offset = int.Parse(tbKeyBoardSpeed.Text);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);

            }

            int x = joyBitmap.Width / 2;
            int y = joyBitmap.Height / 2;

            if (KeyState[(int)Keys.W])
                y -= offset;
            if (KeyState[(int)Keys.S])
                y += offset;
            if (KeyState[(int)Keys.A])
                x -= offset;
            if (KeyState[(int)Keys.D])
                x += offset;
            {
                MouseEventArgs me = new MouseEventArgs(MouseButtons.Left, 0, x, y, 0);
                UpdateJoystickAndDriveCommand(me);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (robot.connected)
            {
                robot.RunDemo(6);
            }

            //try remote send
            if (udpClientSender == null)
                return;
            if (udpClientSender.Client == null)
                return;
            if (udpClientSender.Client.Connected)
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes("home");
                udpClientSender.Send(sendBytes, sendBytes.Length);
                lastSentNetworkMessageTimestamp = System.Environment.TickCount;
            }
        }

        private void tbKeyBoardSpeed_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            if (!Int32.TryParse(tbKeyBoardSpeed.Text, out v))
                return;
            if (v >= iRobotCreate.MaxMotorSpeed)
            {
                tbKeyBoardSpeed.Text = iRobotCreate.MaxMotorSpeed.ToString();
            }
            if (v < 0)
            {
                tbKeyBoardSpeed.Text = "0";
            }
        }

        private void tbRemotePassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbKeyBoardSpeed_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 8)
                return;
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+"))
                e.Handled = true;
        }

        private void btnKeyboardControl_Click(object sender, EventArgs e)
        {
            robot.StartInFullMode();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void tbServerPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 8)
                return;
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "[\\w!@#$%^&*-+=()]+"))
                e.Handled = true;
        }

        private void tbRemotePassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 8)
                return;
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "[\\w!@#$%^&*-+=()]+"))
                e.Handled = true;
        }

        private void tbRemotePort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 8)
                return;
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+"))
                e.Handled = true;
        }

        private void tbRemoteIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 8)
                return;
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "[\\d\\.]+"))
                e.Handled = true;
        }

        private void tbServerPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 8)
                return;
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+"))
                e.Handled = true;
        }
    }
}
