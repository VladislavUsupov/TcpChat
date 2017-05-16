using System;
using System.Windows.Forms;

namespace Client
{
    public partial class PublicChatForm : Form
    {
        private readonly PrivateChatForm pChat;
        public readonly LoginForm FormLogin = new LoginForm();

        public PublicChatForm()
        {
            pChat = new PrivateChatForm(this);
            InitializeComponent();
        }       
        

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FormLogin.Client.Received += _client_Received;
            FormLogin.Client.Disconnected += Client_Disconnected;
            Text = "TCP Chat - " + FormLogin.txtIP.Text + " - (Connected as: " + FormLogin.txtNickname.Text + ")";
            FormLogin.ShowDialog();
        }

        private static void Client_Disconnected(ClientSettings cs)
        {
            
        }
        

        public void _client_Received(ClientSettings cs, string received)
        {
            var cmd = received.Split('|');
            switch (cmd[0])
            {
                case "Users":
                    this.Invoke(() =>
                    {
                        userList.Items.Clear();
                        for (int i = 1; i < cmd.Length; i++)
                        {
                            if (cmd[i] != "Connected" | cmd[i] != "RefreshChat")
                            {
                                userList.Items.Add(cmd[i]);
                            }
                        }
                    });
                    break;
                case "Message":
                    this.Invoke(() =>
                    {
                        txtReceive.Text += cmd[1] + "\r\n";
                    });
                    break;
                case "RefreshChat":
                    this.Invoke(() =>
                    {
                        txtReceive.Text = cmd[1];
                    });
                    break;
                case "Chat":
                    this.Invoke(() =>
                    {
                        pChat.Text = pChat.Text.Replace("user", FormLogin.txtNickname.Text);
                        pChat.Show();
                    });
                    break;
                case "pMessage":
                    this.Invoke(() =>
                    {
                        pChat.txtReceive.Text += "Server says: " + cmd[1] + "\r\n";
                    });
                    break;
                case "Disconnect":
                    Application.Exit();
                    break;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtInput.Text != string.Empty)
            {
                FormLogin.Client.Send("Message|" + FormLogin.txtNickname.Text + "|" + txtInput.Text);
                txtReceive.Text += FormLogin.txtNickname.Text + " says: " + txtInput.Text + "\r\n";
                txtInput.Text = string.Empty;
            }
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
            }
        }

        private void txtReceive_TextChanged(object sender, EventArgs e)
        {
            txtReceive.SelectionStart = txtReceive.TextLength;
        }

        private void privateChat_Click(object sender, EventArgs e)
        {
            FormLogin.Client.Send("pChat|" + FormLogin.txtNickname.Text);
        }
    }
}