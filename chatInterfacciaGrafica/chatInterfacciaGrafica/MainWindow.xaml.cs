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
using System.Windows.Threading;
using System.Xml.Linq;
using System.IO;

namespace chatInterfacciaGrafica
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string ultimoMess = "";

        string ip;
        string nikname;
        string messaggio;
        DispatcherTimer Timer;


        public MainWindow()
        {
            InitializeComponent();

            Timer = new DispatcherTimer(); // Inizializza il timer qui
            Timer.Interval = TimeSpan.FromSeconds(1); // Imposta l'intervallo a 1 secondo
            Timer.Tick += timer_Tick; // Assegna l'evento Tick
        }

        public void timer_Tick(object? sender, EventArgs e)
        {
            RichiedoMessaggiAlserver();
            lstMess.Items.Add("Secondo passato");
        }


        private void StartClient()
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[2048];
            ip = TxtIndirizzoIP.Text;
            nikname = TxtNickname.Text;

            try
            {
                // Connect to a remote device.
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
                Socket sender = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

                messaggio = TxtMessaggio.Text;
                sender.Connect(remoteEP);
                sender.RemoteEndPoint.ToString();

                if (messaggio == ultimoMess)
                {
                    MessageBox.Show("Non puoi inviare lo stesso messaggio due volte di seguito");
                }
                else
                {
                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.UTF8.GetBytes(nikname + ": " + messaggio + "<EOF>");
                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);
                    lstMess.Items.Add(nikname + " ha inviato: " + messaggio);


                }
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                MessageBox.Show("Errore! l'ip non risulta essere un server autorizzato! " + ex.Message + "al socket " + ex.SocketErrorCode);
            }
            catch (System.FormatException)

            {
                MessageBox.Show("Errore! Non risulta essere un ip valido! ");

            }

            // Invio il messaggio
            LeggoIMessaggi(nikname, ip, messaggio);

            ultimoMess = messaggio;


        }

        private void chatJoin_Click(object sender, RoutedEventArgs e)
        {


            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(30);
            Timer.Tick += timer_Tick;
            Timer.Start();


            try
            {
                ip = TxtIndirizzoIP.Text;
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
                Socket test = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
                lstMess.Items.Add("L'utente " + TxtNickname.Text + " si è connesso al server " + TxtIndirizzoIP.Text).ToString();
                //CaricoMessaggiVecchi(txtIp.Text);



            }
            catch (System.Net.Sockets.SocketException ex)
            {
                MessageBox.Show("Errore! l'ip non risulta essere un server autorizzato! " + ex.Message + "al socket " + ex.SocketErrorCode);
            }
            catch (System.FormatException)

            {
                MessageBox.Show("Errore! Non risulta essere un ip valido! ");
            }
        }

        private void chatSend_Click(object sender, RoutedEventArgs e)
        {
            StartClient();
            RichiedoMessaggiAlserver();
            TxtMessaggio.Text = "";
        }
        private void LeggoIMessaggi(string nome, string server, string messaggio)
        {
            try
            {
                DateTime data = DateTime.Now;

                string ora = data.ToString();

                using (StreamWriter sw = File.AppendText(server + ".txt"))
                {
                    sw.WriteLine("L'utente " + nome + " ha detto " + messaggio + " alle ore " + ora);
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                string filePath = server + ".txt";
                File.Create(filePath).Dispose();
            }
        }

        private void RichiedoMessaggiAlserver()
        {
            byte[] bytes = new byte[2048];

            string richiesta = "<RDMSG><EOF>";
            // Connect to a remote device.
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
            Socket sender = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

            sender.Connect(remoteEP);
            sender.RemoteEndPoint.ToString();



            // Encode the data string into a byte array.
            byte[] msg = Encoding.UTF8.GetBytes(richiesta);
            // Send the data through the socket.
            int bytesSent = sender.Send(msg);
            // Receive the response from the remote device.
            int bytesRec = sender.Receive(bytes);

            string risposta = Encoding.UTF8.GetString(bytes, 0, bytesRec);


            //Mi salvo l'ultima risposta presente 
            string? ultimaRisposta = lstMess.Items[lstMess.Items.Count - 1].ToString();

            if (risposta != ultimaRisposta)
            {
                lstMess.Items.Add(risposta);
                ultimaRisposta = risposta;
            }
        }
    }
}