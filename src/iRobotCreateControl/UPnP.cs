using System;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace iRobotCreateControl
{
    public class UPnP
    {
        public enum Protocol
        {
            UDP,
            TCP,
        };

        public UPnP()
        {

        }

        public static bool OpenFirewallPort(string serviceName, Protocol p, int port)
        {
            NetworkInterface[] nics =
            NetworkInterface.GetAllNetworkInterfaces();
            try
            {
                //for each nic in computer...
                foreach (NetworkInterface nic in nics)
                {
                    if (nic.OperationalStatus != OperationalStatus.Up)
                         continue;

                    foreach (UnicastIPAddressInformation addrInfo in nic.GetIPProperties().UnicastAddresses)
                    {
                        if (addrInfo.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;
                        string machineIP = addrInfo.Address.ToString();
                        //send msg to each gateway configured on this nic
                        foreach (GatewayIPAddressInformation gwInfo in nic.GetIPProperties().GatewayAddresses)
                        {
                            string gateWayAddr = gwInfo.Address.ToString();
                            Console.Out.WriteLine("trying to add port forwarding {0} machineIP: {1}\tgateway address: {2}",port, machineIP,gateWayAddr);
                            OpenFirewallPort(serviceName,p ,machineIP, gwInfo.Address.ToString(), port);
                        }//each gateay
                    }//each ip
                }//each nic
            }//try
            catch(Exception x)
            {
                Console.Out.WriteLine(x);
                return false;
            }
            return true;
        }//open firewall port

        public static bool CloseFirewallPort(Protocol p, int port)
        {
            NetworkInterface[] nics =
            NetworkInterface.GetAllNetworkInterfaces();
            try
            {
                //for each nic in computer...
                foreach (NetworkInterface nic in nics)
                {
                    if (nic.OperationalStatus != OperationalStatus.Up)
                        continue;

                    //send msg to each gateway configured on this nic
                    foreach (GatewayIPAddressInformation gwInfo in nic.GetIPProperties().GatewayAddresses)
                    {
                        string gateWayAddr = gwInfo.Address.ToString();
                        Console.Out.WriteLine("trying to close port forwarding {0}\tgateway address: {1}", port, gateWayAddr);
                        CloseFirewallPort(p, gwInfo.Address.ToString(), port);
                    }//each gateay
                }//each nic
            }//try
            catch (Exception x)
            {
                Console.Out.WriteLine(x);
                return false;
            }
            return true;
        }//open firewall port



        public static void OpenFirewallPort(string serviceName,Protocol p, string machineIP, string firewallIP, int openPort)
        {
            int gateWayPort = 0;
            string svc = getServicesFromDevice(firewallIP, ref gateWayPort);

            AddPortMapping(svc, "urn:schemas-upnp-org:service:WANIPConnection:1", serviceName, p, machineIP, firewallIP, gateWayPort, openPort);
//            AddPortMapping(svc, "urn:schemas-upnp-org:service:WANPPPConnection:1", serviceName, Protocol.UDP, machineIP, firewallIP, gateWayPort, openPort);
        }

        public static void CloseFirewallPort(Protocol p, string firewallIP, int openPort)
        {
            int gateWayPort = 0;
            string svc = getServicesFromDevice(firewallIP, ref gateWayPort);

            DeletePortMapping(svc, "urn:schemas-upnp-org:service:WANIPConnection:1",p, firewallIP, gateWayPort, openPort);
//            DeletePortMapping(svc, "urn:schemas-upnp-org:service:WANPPPConnection:1", serviceName, Protocol.UDP, machineIP, firewallIP, gateWayPort, openPort);
        }


        private static string getServicesFromDevice(string firewallIP, ref int gatewayPort)
        {
            //To send a broadcast and get responses from all, send to 239.255.255.250
            string queryResponse = "";

            try
            {
            string query = "M-SEARCH * HTTP/1.1\r\n" +
            "Host:" + firewallIP + ":1900\r\n" +
            "ST:upnp:rootdevice\r\n" +
            "Man:\"ssdp:discover\"\r\n" +
            "MX:3\r\n" +
            "\r\n" +
            "\r\n";

            //use sockets instead of UdpClient so we can set a timeout easier
            Socket client = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endPoint = new
            IPEndPoint(IPAddress.Parse(firewallIP), 1900);

            //1.5 second timeout because firewall should be on same segment (fast)
            client.SetSocketOption(SocketOptionLevel.Socket,
            SocketOptionName.ReceiveTimeout, 1500);

            byte[] q = Encoding.ASCII.GetBytes(query);
            client.SendTo(q, q.Length, SocketFlags.None, endPoint);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint senderEP = (EndPoint)sender;

            byte[] data = new byte[1024];
            int recv = client.ReceiveFrom(data, ref senderEP);
            queryResponse = Encoding.ASCII.GetString(data);
            }
            catch { }

            if(queryResponse.Length == 0)
                return "";


            /* QueryResult is somthing like this:
            *
            HTTP/1.1 200 OK
            Cache-Control:max-age=60
            Location:http://10.10.10.1:80/upnp/service/des_ppp.xml
            Server:NT/5.0 UPnP/1.0
            ST:upnp:rootdevice
            EXT:

            USN:uuid:upnp-InternetGatewayDevice-1_0-00095bd945a2::upnp:rootdevice
            */

            string location = "";
            string[] parts = queryResponse.Split(new string[] {
            System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                if (part.ToLower().StartsWith("location"))
                {
                    location = part.Substring(part.IndexOf(':') + 1);
                    break;
                }
            }

            if (location.Length == 0)
                return "";

            //then using the location url, we get more information:
            string gatewayPortStr = location.Substring(location.IndexOf(firewallIP) + firewallIP.Length+1);
            gatewayPortStr = gatewayPortStr.Substring(0, gatewayPortStr.IndexOf('/'));
            gatewayPort = int.Parse(gatewayPortStr);

            System.Net.WebClient webClient = new WebClient();
            try
            {
                string ret = webClient.DownloadString(location);
                return ret;//return services
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                webClient.Dispose();
            }
            return "";
        }



        private static void DeletePortMapping(string services, string serviceType, Protocol p, string firewallIP, int gatewayPort, int portToForward)
        {
            if (services.Length == 0)
                return;
        
            int svcIndex = services.IndexOf(serviceType);
            if (svcIndex == -1)
                return;
            string controlUrl = services.Substring(svcIndex);
            string tag1 = "<controlURL>";
            string tag2 = "</controlURL>";
            controlUrl = controlUrl.Substring(controlUrl.IndexOf(tag1) + tag1.Length);
            controlUrl = controlUrl.Substring(0,controlUrl.IndexOf(tag2));

            string protocolStr = (p == Protocol.UDP)? "UDP" : "TCP";
            string soapBody = "<s:Envelope " + 
                "xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/ \" " +
                "s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/ \">" +
                "<s:Body>" +
                "<u:DeletePortMapping xmlns:u=\"" + serviceType + "\">" +
                "<NewRemoteHost></NewRemoteHost>" +
                "<NewExternalPort>" + portToForward.ToString() +
                "</NewExternalPort>" +
                "<NewProtocol>" + protocolStr + "</NewProtocol>" +
                "</u:DeletePortMapping>" +
                "</s:Body>" +
                "</s:Envelope>";

            byte[] body = System.Text.UTF8Encoding.ASCII.GetBytes(soapBody);

            string url = "http://" + firewallIP + ":" +
            gatewayPort.ToString() + controlUrl;

            Console.Out.WriteLine(url);

            System.Net.WebRequest wr =
            System.Net.WebRequest.Create(url);//+ controlUrl);
            wr.Method = "POST";
            wr.Headers.Add("SOAPAction","\"" + serviceType + "#AddPortMapping\"");
            wr.ContentType = "text/xml;charset=\"utf-8\"";
            wr.ContentLength = body.Length;

            System.IO.Stream stream = wr.GetRequestStream();
            stream.Write(body, 0, body.Length);
            stream.Flush();
            stream.Close();

            WebResponse wres = wr.GetResponse();
            System.IO.StreamReader sr = new
            System.IO.StreamReader(wres.GetResponseStream());
            string ret = sr.ReadToEnd();
            sr.Close();

            Debug.WriteLine("Port Mapping Deleted:" + portToForward.ToString());
        }

        private static bool AddPortMapping(string services, string serviceType, string serviceName, Protocol p, string machineIP, string firewallIP, int gatewayPort, int portToForward)
        {
            if (services.Length == 0)
                return false;

            int svcIndex = services.IndexOf(serviceType);
            if (svcIndex == -1)
                return false;
            string controlUrl = services.Substring(svcIndex);
            string tag1 = "<controlURL>";
            string tag2 = "</controlURL>";
            controlUrl = controlUrl.Substring(controlUrl.IndexOf(tag1) + tag1.Length);
            controlUrl = controlUrl.Substring(0, controlUrl.IndexOf(tag2));

            string protocolStr = (p == Protocol.UDP) ? "UDP" : "TCP";
            string soapBody = "<s:Envelope " +
                "xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/ \" " +
                "s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/ \">" +
                "<s:Body>" +
                "<u:AddPortMapping xmlns:u=\"" + serviceType + "\">" +
                "<NewRemoteHost></NewRemoteHost>" +
                "<NewExternalPort>" + portToForward.ToString() +
                "</NewExternalPort>" +
                "<NewProtocol>" + protocolStr + "</NewProtocol>" +
                "<NewInternalPort>" + portToForward.ToString() +
                "</NewInternalPort>" +
                "<NewInternalClient>" + machineIP +
                "</NewInternalClient>" +
                "<NewEnabled>1</NewEnabled>" +
                "<NewPortMappingDescription>" + serviceName + "</NewPortMappingDescription>" +
                "<NewLeaseDuration>0</NewLeaseDuration>" +
                "</u:AddPortMapping>" +
                "</s:Body>" +
                "</s:Envelope>";

            byte[] body = System.Text.UTF8Encoding.ASCII.GetBytes(soapBody);

            string url = "http://" + firewallIP + ":" +
            gatewayPort.ToString() + controlUrl;

            Console.Out.WriteLine(url);

            System.Net.WebRequest wr =
            System.Net.WebRequest.Create(url);//+ controlUrl);
            wr.Method = "POST";
            wr.Headers.Add("SOAPAction", "\"" + serviceType + "#AddPortMapping\"");
            wr.ContentType = "text/xml;charset=\"utf-8\"";
            wr.ContentLength = body.Length;

            System.IO.Stream stream = wr.GetRequestStream();
            stream.Write(body, 0, body.Length);
            stream.Flush();
            stream.Close();

            WebResponse wres = wr.GetResponse();
            System.IO.StreamReader sr = new
            System.IO.StreamReader(wres.GetResponseStream());
            string ret = sr.ReadToEnd();
            sr.Close();

            Debug.WriteLine("Port Mapping Added:" + portToForward.ToString());
            return true;
        }
    }//upnp class
}