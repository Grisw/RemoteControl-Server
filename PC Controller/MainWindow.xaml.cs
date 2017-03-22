using System;
using System.Windows;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace PC_Controller {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        
        private TcpClient client;
        private TcpListener listener;
        private static double resolution = 0.3;

        public MainWindow() {
            InitializeComponent();
        }

        private void Listen() {
            listener = new TcpListener(IPAddress.Any, 4869);
            listener.Start();
            bool isConnected = false;
            while (!isConnected) {
                TcpClient client = null;
                StreamReader reader = null;
                StreamWriter writer = null;
                try {
                    client = listener.AcceptTcpClient();
                    this.client = client;
                    reader = new StreamReader(client.GetStream());
                    writer = new StreamWriter(client.GetStream());
                    string password = reader.ReadLine();
                    if (!string.IsNullOrEmpty(password)) {
                        Dispatcher.Invoke(new Action(() => {
                            PasswordCheck passwordCheck = new PasswordCheck(client.Client.RemoteEndPoint.ToString().Split(':')[0], password);
                            passwordCheck.Owner = this;
                            if (passwordCheck.ShowDialog() == true) {
                                writer.WriteLine("accept");
                                writer.Flush();
                                label.Content = client.Client.RemoteEndPoint.ToString().Split(':')[0] + " 正在控制...";
                                new Thread(new ThreadStart(() => {
                                    try {
                                        Control(reader);
                                    } finally {
                                        if (reader != null)
                                            reader.Close();
                                        if (writer != null)
                                            writer.Close();
                                        if (client != null)
                                            client.Close();
                                    }
                                })).Start();
                                new Thread(new ThreadStart(() => {
                                    try {
                                        SendImage(writer);
                                    } catch { } finally {
                                        if (reader != null)
                                            reader.Close();
                                        if (writer != null)
                                            writer.Close();
                                        if (client != null)
                                            client.Close();
                                    }
                                })).Start();
                                if (listener != null)
                                    listener.Stop();
                                isConnected = true;
                            } else {
                                writer.WriteLine("deny");
                                writer.Flush();
                            }
                        }));
                    }
                } catch {
                    if (reader != null)
                        reader.Close();
                    if (writer != null)
                        writer.Close();
                    if (client != null)
                        client.Close();
                }
            }
        }

        private void Control(StreamReader reader) {
            try {
                while (true) {
                    string cmd = reader.ReadLine();
                    string[] param = cmd.Split(new char[] { ',' });
                    switch (param[0]) {
                        case CursorCommand.MOVE:
                            try {
                                int x = int.Parse(param[1]);
                                int y = int.Parse(param[2]);
                                CursorController.POINT point = new CursorController.POINT();
                                CursorController.GetCursorPos(out point);
                                CursorController.SetCursorPos(point.X + x, point.Y + y);
                            } catch (FormatException) { }
                            break;
                        case CursorCommand.LEFT_CLICK:
                            CursorController.mouse_event(CursorController.MouseEventFlag.LeftDown | CursorController.MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                            break;
                        case CursorCommand.RIGHT_CLICK:
                            CursorController.mouse_event(CursorController.MouseEventFlag.RightDown | CursorController.MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
                            break;
                        case CursorCommand.WHEEL_UP:
                            CursorController.mouse_event(CursorController.MouseEventFlag.Wheel, 0, 0, 50, UIntPtr.Zero);
                            break;
                        case CursorCommand.WHEEL_DOWN:
                            CursorController.mouse_event(CursorController.MouseEventFlag.Wheel, 0, 0, -50, UIntPtr.Zero);
                            break;
                        case "string":
                            string s = param[1];
                            break;
                        default:
                            break;
                    }
                }
            } catch (Exception e){
                Dispatcher.Invoke(new Action(() => {
                    System.Windows.MessageBox.Show(this, "已断开连接。" + e.Message, "提示");
                    if (client != null)
                        client.Close();
                    if (listener != null)
                        listener.Stop();
                    label.Content = "未启动";
                    start.IsEnabled = true;
                    stop.IsEnabled = false;
                }));
            }
        }
        
        private void SendImage(StreamWriter writer) {
            while (true) {
                byte[] data = ImageCore.Compress(ImageCore.Capture(resolution));
                writer.WriteLine(Convert.ToBase64String(data));
            }
        }
        
        private static class CursorCommand {
            public const string MOVE = "move";
            public const string LEFT_CLICK = "lclick";
            public const string RIGHT_CLICK = "rclick";
            public const string WHEEL_UP = "wheelup";
            public const string WHEEL_DOWN = "wheeldown";
        }

        private void Window_Closed(object sender, EventArgs e) {
            Environment.Exit(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            new Thread(new ThreadStart(Listen)).Start();
            label.Content = "正在等待连接...";
            start.IsEnabled = false;
            stop.IsEnabled = true;
        }

        private void stop_Click(object sender, RoutedEventArgs e) { 
            if (client != null)
                client.Close();
            if (listener != null)
                listener.Stop();
            label.Content = "未启动";
            start.IsEnabled = true;
            stop.IsEnabled = false;
        }
        
        private void RadioButton_Checked(object sender, RoutedEventArgs e) {
            resolution = 0.3;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e) {
            resolution = 0.5;
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e) {
            resolution = 0.8;
        }

        private void RadioButton_Checked_3(object sender, RoutedEventArgs e) {
            resolution = 1.0;
        }

    }

}
