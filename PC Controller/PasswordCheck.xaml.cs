using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PC_Controller {
    /// <summary>
    /// PasswordCheck.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordCheck : Window {

        private string requestName;
        private string remotePassword;

        public string RequestName {
            set {
                requestName = value;
                hintText.Content = requestName + " 想要连接，请输入约定好的密码：";
            }
            get {
                return requestName;
            }
        }
        public string RemotePassword {
            set {
                remotePassword = value;
            }
            get {
                return remotePassword;
            }
        }

        public PasswordCheck(string requestName, string remotePassword) {
            InitializeComponent();
            RequestName = requestName;
            RemotePassword = remotePassword;
        }

        private void PasswordBox_Pasting(object sender, DataObjectPastingEventArgs e) {
            e.CancelCommand();
        }

        private void PasswordBox_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) {
                e.Handled = false;
            } else if ((e.Key >= Key.D0 && e.Key <= Key.D9) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift) {
                e.Handled = false;
            } else if (e.Key == Key.Back || e.Key == Key.Return) {
                e.Handled = false;
            } else {
                e.Handled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrEmpty(passwordBox.Text)) {
                if (passwordBox.Text.Equals(RemotePassword)) {
                    this.DialogResult = true;
                    this.Close();
                } else {
                    MessageBox.Show("密码错误！", "错误");
                }
            }
        }

    }
}
