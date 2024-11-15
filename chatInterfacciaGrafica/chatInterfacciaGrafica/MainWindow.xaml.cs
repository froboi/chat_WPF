using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Net;
using System.Net.Sockets;

namespace chatInterfacciaGrafica
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string ultimoMess = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private string StartCLient
            (string messaggio, string indirizzoIP)
        {
            string risposta = "";
            byte[] bytes = new byte[2048];
            IPAddress ipAddress = IPAddress.Parse(indirizzoIP);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            try
            {
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    sender.Connect(remoteEP);
                    byte[] msg = Encoding.UTF8.GetBytes(messaggio);
                    int bytesSent = sender.Send(msg);
                    int bytesRec = sender.Receive(bytes);
                    risposta = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    return risposta;
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return risposta;
        }

        private void chatJoin_Click(object sender, RoutedEventArgs e)
        {
            string messaggio = TxtNickname.Text + "è entrato nella chat";
            string indirizzoIP = TxtIndirizzoIP.Text;
            string risposta = StartCLient(messaggio, indirizzoIP);
        }

        private void chatSend_Click(object sender, RoutedEventArgs e)
        {
            

        }
        






    }
}