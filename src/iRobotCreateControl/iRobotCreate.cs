using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace iRobot
{
    public class iRobotCreate
    {
        public enum OpCode
        {
            SoftReset = 7,
            Start = 128,
            Baud = 129,
            Control = 130,
            Safe = 131,
            Full = 132,
            Spot = 134,
            Cover = 135,
            Demo = 136,
            Drive = 137,
            LowSideDrivers = 138,
            LEDs = 139,
            Song = 140,
            Play = 141,
            Sensors = 142,
            CoverAndDock = 143,
            PWMLowSideDrivers = 144,
            DriveDirect = 145,
            DigitalOutputs = 147,
            Stream = 148,
            QueryList = 149,
            PauseResumeStream = 150,
            SendIR = 151,
            Script = 152,
            PlayScript = 153,
            ShowScript = 154,
            WaitTime = 155,
            WaitDistance = 156,
            WaitAngle = 157,
            WaitEvent = 158,
        }

        public enum SensorPacket
        {
            Group0 = 0,  //7-26
            Group1 = 1,  //7-16
            Group2 = 2,  //17-20
            Group3 = 3,  //21-26
            Group4 = 4,  //27-34
            Group5 = 5,  //35-42
            Group6 = 6,  //7-42
            BumpsAndWheelDrops = 7,
            Wall = 8,
            CliffLeft = 9,
            CliffFrontLeft = 10,
            CliffFrontRight = 11,
            CliffRight = 12,
            VirtualWall = 13,
            LowSideDriverAndWheelOverCurrents = 14,
            Infrared = 17,
            Buttons = 18,
            Distance = 19,
            Angle = 20,
            ChargingState = 21,
            Voltage = 22,
            Current = 23,
            BatteryTemperature = 24,
            BatteryCharge = 25,
            BatteryCapacity = 26,
            WallSignal = 27,
            CliffLeftSignal = 28,
            CliffFrontLeftSignal = 29,
            CliffFrontRightSignal = 30,
            CliffRightSignal = 31,
            CargoBayDigitalInputs = 32,
            CargoBayAnalogSignal = 33,
            ChargingSourcesAvailable = 34,
            OIMode = 35,
            SongNumber = 36,
            SongPlaying = 37,
            NumberOfStreamPackets = 38,
            RequestedVelocity = 39,
            RequestedRadius = 40,
            RequestedRightVelocity = 41,
            RequestedLeftVelocity = 42,
        }

        public struct SensorState
        {
            //only 7-31 implemented
            public bool WheelDropCaster;
            public bool WheelDropLeft;
            public bool WheelDropRight;
            public bool BumpLeft;
            public bool BumpRight;
            public bool Wall;
            public bool CliffLeft;
            public bool CliffFrontLeft;
            public bool CliffFrontRight;
            public bool CliffRight;
            public bool VirtualWall;
            public bool OverCurrentLeftWheel;
            public bool OverCurrentRightWheel;
            public bool OverCurrentLD2;
            public bool OverCurrentLD0;
            public bool OverCurrentLD1;

            public byte IR;
            public bool ButtonPlay;
            public bool ButtonAdvance;
            public int Distance;
            public int Angle;
            public int ChargingState;
            public int Voltage;
            public int Current;
            public int BatteryTempurature;
            public int BatteryCharge;
            public int BatteryCapacity;
            public int WallSignal;
            public int CliffLeftSignal;
            public int CliffLeftFrontSignal;
            public int CliffRightFrontSignal;
            public int CliffRightSignal;
        }


        public delegate void SensorUpdateHandler(object sender, EventArgs e);
        public delegate void BumperChangedHandler(object sender, EventArgs e);
        public delegate void CliffDetectChangedHandler(object sender, EventArgs e);
        public delegate void WheelDropChangedHandler(object sender, EventArgs e);

        byte[] messageBuffer = new byte[64];
        byte[] recieveBuffer = new byte[1024];
        byte[] buffer = new byte[64];
        int recieveBufferReadIndex = 0;
        int recieveBufferWriteIndex = 0;
        int messageIndex = 0;

        int targetLeftWheelSpeed = 0;
        int targetRightWheelSpeed = 0;
        int lastSentLeftWheelSpeed = 0;
        int lastSentRightWheelSpeed = 0;

        Thread drivingThread;

        public int maxAcclerationPerSec = 2500;
        public const int MaxMotorSpeed = 500;
        public const int MaxAngularVelocity = 2000;//with weird forward exception of 32678
        public int updateIntervalMS = 50;

        public bool connected = false;
        SerialPort port = null;
        public SensorState sensorState = new SensorState();
        public SensorState prevSensorState = new SensorState();

        public event SensorUpdateHandler OnSensorUpdateRecieved;

        public event BumperChangedHandler OnBumperChanged;
        public event CliffDetectChangedHandler OnCliffDetectChanged;
        public event WheelDropChangedHandler OnWheelDropChanged;

        public iRobotCreate()
        {
        }

        

        public bool Connect(string portName)
        {
            try
            {
                if (!connected)
                {
                    port = new SerialPort();
                    port.PortName = portName;
                    port.BaudRate = 57600;
                    port.DataBits = 8;
                    port.DtrEnable = false;
                    port.StopBits = StopBits.One;
                    port.Handshake = Handshake.None;
                    port.Parity = Parity.None;
                    port.RtsEnable = false;
                    port.Close();
                    port.Open();
                    port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                    connected = true;
                    Thread.Sleep(50);
                }
            }
            catch (System.Exception ex)
            {
                Console.Out.WriteLine(ex);
                return false;
            }
            return true;
        }

        public void Disconnect()
        {
            if(!connected)
                return;

            SoftReset();
            if (port != null)
            {
                if (port.IsOpen)
                {
  //                  port.DataReceived -= new SerialDataReceivedEventHandler(port_DataReceived);
                    //causes hang?
//                    port.Close();
                }
            }
            connected = false;
        }
        public void QueueCommand(OpCode op)
        {
            if (!connected)
                return;
            messageBuffer[messageIndex] = (byte)op;
            messageIndex++;
        }


        public void QueueCommand(OpCode op, byte parameter)
        {
            if (!connected)
                return;
            messageBuffer[messageIndex] = (byte)op;
            messageIndex++;
            messageBuffer[messageIndex] = parameter;
            messageIndex++;
        }
        public void QueueCommand(OpCode op, byte parameter1, byte parameter2)
        {
            if (!connected)
                return;
            messageBuffer[messageIndex] = (byte)op;
            messageIndex++;
            messageBuffer[messageIndex] = parameter1;
            messageIndex++;
            messageBuffer[messageIndex] = parameter2;
            messageIndex++;
        }
        public void QueueCommand(OpCode op, byte parameter1, byte parameter2, byte parameter3)
        {
            if (!connected)
                return;
            messageBuffer[messageIndex] = (byte)op;
            messageIndex++;
            messageBuffer[messageIndex] = parameter1;
            messageIndex++;
            messageBuffer[messageIndex] = parameter2;
            messageIndex++;
            messageBuffer[messageIndex] = parameter3;
            messageIndex++;
        }

        public void QueueCommandParameter(OpCode op, int parameter)
        {
            if (!connected)
                return;
            messageBuffer[messageIndex] = (byte)op;
            messageIndex++;
            messageBuffer[messageIndex] = (byte)(parameter >> 8);
            messageIndex++;
            messageBuffer[messageIndex] = (byte)(parameter & 255);
            messageIndex++;
        }

        public void QueueCommandTwoParameters(OpCode op, int p1, int p2)
        {
            if (!connected)
                return;

            messageBuffer[messageIndex] = (byte)op;
            messageIndex++;
            messageBuffer[messageIndex] = (byte)(p1 >> 8);
            messageIndex++;
            messageBuffer[messageIndex] = (byte)(p1 & 255);
            messageIndex++;
            messageBuffer[messageIndex] = (byte)(p2 >> 8);
            messageIndex++;
            messageBuffer[messageIndex] = (byte)(p2 & 255);
            messageIndex++;
        }

        public void CoverAndDock()
        {
            QueueCommand(OpCode.CoverAndDock);
            SendCommand();
        }
        public void RunDemo(byte demo)
        {
            QueueCommand(OpCode.Demo, demo);
            SendCommand();
        }

        public void QuerySensor(SensorPacket sensorID)
        {
            QueueCommandParameter(OpCode.Sensors, (int)SensorPacket.BatteryCharge);
            SendCommand();
        }

        public void StopStreaming()
        {
            QueueCommandParameter(OpCode.PauseResumeStream, 0);
            SendCommand();
        }

        public void StartSensorStreaming()
        {
            QueueCommand(OpCode.Stream, (byte)1, (byte)SensorPacket.Group0);
            SendCommand();
        }

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //read as much as possible to clear the communication buffer
            int bytes = port.Read(buffer, 0,buffer.Length);
//            for(int i =0; i < bytes; i++)
//                Console.Write((char)buffer[i]);
//            Console.WriteLine("");


            //copy to main buffer
            for (int i = 0; i < bytes; i++)
            {
                recieveBuffer[(recieveBufferWriteIndex++) % recieveBuffer.Length] = buffer[i];
            }

            //if we have enough data parse
            int group0messageSize = 30;
            while (recieveBufferReadIndex < recieveBufferWriteIndex-group0messageSize)
            {
                 if(ParseStreamingGroup0(group0messageSize, recieveBuffer, recieveBufferReadIndex))
                 {
//                     for(int i =recieveBufferReadIndex; i < recieveBufferReadIndex+group0messageSize; i++)
//                         Console.Out.Write((int)recieveBuffer[i % recieveBuffer.Length] + " ");
//                     Console.Out.WriteLine(" message " + sensorState.ChargingState + " " + sensorState.BatteryCharge + " " + sensorState.BatteryCapacity + " " + Environment.TickCount);


                    recieveBufferReadIndex += group0messageSize;

                }
                else
                {
                    recieveBufferReadIndex++;
                }
            }

            //shift out the parsed data
            if (recieveBufferReadIndex > 0)
            {
                int count = recieveBufferWriteIndex - recieveBufferReadIndex;
                for (int i = 0; i < count; i++)
                    recieveBuffer[i] = recieveBuffer[recieveBufferReadIndex++];
                recieveBufferWriteIndex = count;
                recieveBufferReadIndex = 0;
            }
        }


        bool PassesCheckSum(int bytes, byte[] buffer, int offset)
        {
            int sum = 0;
            for(int i = offset; i < offset+bytes; i++)
                sum += buffer[i];
            return ((sum & 0xff)==0);
        }

        bool ParseStreamingGroup1(int bytes, byte[] buffer, int offset)
        {
            if (!PassesCheckSum(bytes, buffer, offset))
            {
                Console.WriteLine("Recieved data: failed checksum");
                return false;
            }
            
            if(buffer[offset] != 19) //streaming header
            {
                Console.WriteLine("Recieved data: not streaming packet " + buffer[offset]);
                return false;
            }

            if (buffer[offset+1] != 11) //streaming header
            {
                Console.WriteLine("Recieved data: wrong message length");
                return false;
            }


            if (buffer[offset+ 2] != (byte)SensorPacket.Group1)
            {
                Console.WriteLine("Recieved data: not yet supported packet type");
                return false;
            }

            int sensorDataIndex = offset + 3;

            prevSensorState = sensorState;

            sensorState.BumpRight = (buffer[sensorDataIndex] & 1) > 0;
            sensorState.BumpLeft = (buffer[sensorDataIndex] & 2) > 0;
            sensorState.WheelDropLeft = (buffer[sensorDataIndex] & 4) > 0;
            sensorState.WheelDropRight = (buffer[sensorDataIndex] & 8) > 0;
            sensorState.WheelDropCaster = (buffer[sensorDataIndex] & 16) > 0;

            if ((prevSensorState.BumpLeft != sensorState.BumpLeft)||
                (prevSensorState.BumpRight != sensorState.BumpRight))
            {
                if(OnBumperChanged!= null)
                    OnBumperChanged(this, new EventArgs());
            }

            if ((prevSensorState.WheelDropLeft != sensorState.WheelDropLeft) ||
                (prevSensorState.WheelDropRight != sensorState.WheelDropRight) ||
                (prevSensorState.WheelDropCaster != sensorState.WheelDropCaster))
            {
                if (OnWheelDropChanged != null)
                    OnWheelDropChanged(this, new EventArgs());
            }


            sensorState.Wall = (buffer[sensorDataIndex+1] > 0);
            sensorState.CliffLeft = (buffer[sensorDataIndex + 2] > 0);
            sensorState.CliffFrontLeft = (buffer[sensorDataIndex + 3] > 0);
            sensorState.CliffFrontRight = (buffer[sensorDataIndex + 4] > 0);
            sensorState.CliffRight = (buffer[sensorDataIndex + 5] > 0);

            if ((prevSensorState.CliffLeft != sensorState.CliffLeft) ||
                (prevSensorState.CliffFrontLeft != sensorState.CliffFrontLeft) ||
                (prevSensorState.CliffFrontRight != sensorState.CliffFrontRight) ||
                (prevSensorState.CliffRight != sensorState.CliffRight))
            {
                if (OnCliffDetectChanged != null)
                    OnCliffDetectChanged(this, new EventArgs());
            }

            
            sensorState.VirtualWall = (buffer[sensorDataIndex + 6] > 0);

            sensorState.OverCurrentLD1 = (buffer[sensorDataIndex + 7] & 1) > 0;
            sensorState.OverCurrentLD0 = (buffer[sensorDataIndex + 7] & 2) > 0;
            sensorState.OverCurrentLD2 = (buffer[sensorDataIndex + 7] & 4) > 0;
            sensorState.OverCurrentLeftWheel = (buffer[sensorDataIndex + 7] & 8) > 0;
            sensorState.OverCurrentRightWheel = (buffer[sensorDataIndex + 7] & 16) > 0;

            if (connected)
            {
                if (OnSensorUpdateRecieved != null)
                {
                    OnSensorUpdateRecieved(this, new EventArgs());
                }
            }

            return true;
        }

        bool ParseStreamingGroup0(int bytes, byte[] buffer, int offset)
        {

            if (buffer[offset] != 19) //streaming header
            {
                //                Console.WriteLine("Recieved data: not streaming packet " + buffer[offset]);
                return false;
            }

            if (buffer[offset + 1] != 27) //streaming header
            {
                //           Console.WriteLine("Recieved data: wrong message length");
                return false;
            }

            if (!PassesCheckSum(bytes, buffer, offset))
            {
                //              Console.WriteLine("Recieved data: failed checksum");
                return false;
            }

            if (buffer[offset + 2] != (byte)SensorPacket.Group0)
            {
                //         Console.WriteLine("Recieved data: not yet supported packet type");
                return false;
            }

//            19 27 0 1 0 0 0 0 0 0 0 0 0 255 0 0 0 0 0 0 64 120 255 158 23 10 57 10 142 139 ;
            int sensorDataIndex = offset + 3;

            prevSensorState = sensorState;

            sensorState.BumpRight = (buffer[sensorDataIndex] & 1) > 0;
            sensorState.BumpLeft = (buffer[sensorDataIndex] & 2) > 0;
            sensorState.WheelDropLeft = (buffer[sensorDataIndex] & 4) > 0;
            sensorState.WheelDropRight = (buffer[sensorDataIndex] & 8) > 0;
            sensorState.WheelDropCaster = (buffer[sensorDataIndex] & 16) > 0;

            if ((prevSensorState.BumpLeft != sensorState.BumpLeft) ||
                (prevSensorState.BumpRight != sensorState.BumpRight))
            {
                if (OnBumperChanged != null)
                    OnBumperChanged(this, new EventArgs());
            }

            if ((prevSensorState.WheelDropLeft != sensorState.WheelDropLeft) ||
                (prevSensorState.WheelDropRight != sensorState.WheelDropRight) ||
                (prevSensorState.WheelDropCaster != sensorState.WheelDropCaster))
            {
                if (OnWheelDropChanged != null)
                    OnWheelDropChanged(this, new EventArgs());
            }


            sensorState.Wall = (buffer[sensorDataIndex + 1] > 0);
            sensorState.CliffLeft = (buffer[sensorDataIndex + 2] > 0);
            sensorState.CliffFrontLeft = (buffer[sensorDataIndex + 3] > 0);
            sensorState.CliffFrontRight = (buffer[sensorDataIndex + 4] > 0);
            sensorState.CliffRight = (buffer[sensorDataIndex + 5] > 0);

            if ((prevSensorState.CliffLeft != sensorState.CliffLeft) ||
                (prevSensorState.CliffFrontLeft != sensorState.CliffFrontLeft) ||
                (prevSensorState.CliffFrontRight != sensorState.CliffFrontRight) ||
                (prevSensorState.CliffRight != sensorState.CliffRight))
            {
                if (OnCliffDetectChanged != null)
                    OnCliffDetectChanged(this, new EventArgs());
            }


            sensorState.VirtualWall = (buffer[sensorDataIndex + 6] > 0);

            sensorState.OverCurrentLD1 = (buffer[sensorDataIndex + 7] & 1) > 0;
            sensorState.OverCurrentLD0 = (buffer[sensorDataIndex + 7] & 2) > 0;
            sensorState.OverCurrentLD2 = (buffer[sensorDataIndex + 7] & 4) > 0;
            sensorState.OverCurrentLeftWheel = (buffer[sensorDataIndex + 7] & 8) > 0;
            sensorState.OverCurrentRightWheel = (buffer[sensorDataIndex + 7] & 16) > 0;

            sensorState.IR = buffer[sensorDataIndex + 10];
            sensorState.ButtonPlay = (buffer[sensorDataIndex + 11] & 1) > 0;
            sensorState.ButtonAdvance = (buffer[sensorDataIndex + 11] & 4) > 0;
            sensorState.Distance = (short)((buffer[sensorDataIndex + 12] << 8) + buffer[sensorDataIndex + 13]);
            sensorState.Angle = (short)((buffer[sensorDataIndex + 14] << 8) + buffer[sensorDataIndex + 15]);
            sensorState.ChargingState = buffer[sensorDataIndex + 16];
            sensorState.Voltage = (buffer[sensorDataIndex + 17] << 8) + buffer[sensorDataIndex + 18];
            sensorState.Current = (short)((buffer[sensorDataIndex + 19] << 8) + buffer[sensorDataIndex + 20]);
            sensorState.BatteryTempurature = buffer[sensorDataIndex + 21];
            if (buffer[sensorDataIndex + 22] == 255)
                sensorState.BatteryCharge = buffer[sensorDataIndex + 23];
            else
                sensorState.BatteryCharge = (buffer[sensorDataIndex + 22] << 8) + buffer[sensorDataIndex + 23];
            sensorState.BatteryCapacity = (buffer[sensorDataIndex + 24] << 8) + buffer[sensorDataIndex + 25];
            if (connected)
            {
                if (OnSensorUpdateRecieved != null)
                {
                    OnSensorUpdateRecieved(this, new EventArgs());
                }

            }

            return true;
        }

        public void StartInPassiveMode()
        {
            QueueCommand(OpCode.Start);
            SendCommand();
        }
        public void StartInSafeMode()
        {
            QueueCommand(OpCode.Start);
            QueueCommand(OpCode.Safe);
            SendCommand();
        }
        public void StartInFullMode()
        {
            QueueCommand(OpCode.Start);
            QueueCommand(OpCode.Full);
            SendCommand();
        }

        public void SoftReset()
        {
            if (drivingThread != null)
            {
                drivingThread.Abort();
                Thread.Sleep(100);
                drivingThread = null;
            }


            QueueCommand(OpCode.SoftReset);
            SendCommand();
            Thread.Sleep(2500);
        }

        public void Drive(int velocity, int angle)
        {
            if (velocity > MaxMotorSpeed) velocity = MaxMotorSpeed;
            if (velocity < -MaxMotorSpeed) velocity = -MaxMotorSpeed;
            QueueCommandTwoParameters(OpCode.Drive, velocity, angle);
            SendCommand();
        }
        public void DriveDirect(int left, int right)
        {
            if (!connected)
                return;
            if (left > MaxMotorSpeed) left = MaxMotorSpeed;
            if (left < -MaxMotorSpeed) left = -MaxMotorSpeed;
            if (right > MaxMotorSpeed) right = MaxMotorSpeed;
            if (right < -MaxMotorSpeed) right = -MaxMotorSpeed;
            targetLeftWheelSpeed = left;
            targetRightWheelSpeed = right;

            if (drivingThread == null)
            {
                //too fast, so create a thread to moderate acceleration
                drivingThread = new Thread(new ThreadStart(DriveThreadCallback));
                drivingThread.Start();
            }

            return;
        }

        void DriveThreadCallback()
        {
            int maxAccelPerInterval = maxAcclerationPerSec * updateIntervalMS / 1000;

            while(connected)
            {
                if (!port.IsOpen)
                    return;

                //Console.Out.WriteLine(lastSentLeftWheelSpeed + "," + lastSentRightWheelSpeed + " " + targetLeftWheelSpeed + "," + targetRightWheelSpeed);
                
                float targetForwardMag = targetLeftWheelSpeed + targetRightWheelSpeed;
                float lastForwardMag = lastSentLeftWheelSpeed + lastSentRightWheelSpeed;
                float deltaFowardMag = Math.Abs(targetForwardMag - lastForwardMag);
                int currLeftWheelSpeed = 0;
                int currRightWheelSpeed = 0;

                if (deltaFowardMag > maxAccelPerInterval)
                {
                    float overAccel = deltaFowardMag / maxAccelPerInterval;
                    int leftWheelAccel = targetLeftWheelSpeed - lastSentLeftWheelSpeed;
                    int rightWheelAccel = targetRightWheelSpeed - lastSentRightWheelSpeed;

                    currLeftWheelSpeed = lastSentLeftWheelSpeed + (int)(leftWheelAccel / overAccel);
                    currRightWheelSpeed = lastSentRightWheelSpeed + (int)(rightWheelAccel / overAccel);
                }
                else
                {
                    currLeftWheelSpeed = targetLeftWheelSpeed;
                    currRightWheelSpeed = targetRightWheelSpeed;
                }

                if ((lastSentLeftWheelSpeed != currLeftWheelSpeed) ||
                    (lastSentRightWheelSpeed != currRightWheelSpeed))
                {
                    QueueCommandTwoParameters(OpCode.DriveDirect, currLeftWheelSpeed, currRightWheelSpeed);
                    SendCommand();
                }

                lastSentLeftWheelSpeed = currLeftWheelSpeed;
                lastSentRightWheelSpeed = currRightWheelSpeed;

                Thread.Sleep(updateIntervalMS);
            }
            //changing slow enough, just relay
            lastSentLeftWheelSpeed = targetLeftWheelSpeed;
            lastSentRightWheelSpeed = targetRightWheelSpeed;
            QueueCommandTwoParameters(OpCode.DriveDirect, lastSentLeftWheelSpeed, lastSentRightWheelSpeed);
            SendCommand();
        }

        public void WaitForDistance(int distance)
        {
            QueueCommandParameter(OpCode.WaitDistance, distance);
            SendCommand();
        }
        public void WaitForAngle(int angle)
        {
            QueueCommandParameter(OpCode.WaitAngle, angle);
            SendCommand();
        }

        public void SendCommand()
        {
            if (port == null)
                return;
            if (!port.IsOpen)
                return;
            try
            {
                port.Write(messageBuffer, 0, messageIndex);
//                for (int i = 0; i < messageIndex; i++)
//                    Console.Out.Write((int)messageBuffer[i] + " ");
//                Console.Out.WriteLine(" sent");
            }
            catch (Exception x)
            {
                Console.Out.WriteLine(x);
            }
            messageIndex = 0;
            Thread.Sleep(5);
        }
    }

}
