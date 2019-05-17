using System;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;


namespace ClientApp
{
    public partial class Form1 : Form
    {
        private const int portNum = 4545;

        delegate void SetTextCallback(string text);

        TcpClient client;
        NetworkStream ns;
        Thread t = null;

        private const string hostAddr = "127.0.0.1";

        public Form1()
        {
            InitializeComponent();

            client = new TcpClient(hostAddr, portNum);
            ns = client.GetStream();
           
            t = new Thread(DoWork);
            t.Start();
        }

        public void DoWork()
        {
            try
            {
                byte[] bytes = new byte[1024];

                while (true)
                {
                    int bytesRead = ns.Read(bytes, 0, bytes.Length);
                    this.SetRandomTips(Encoding.ASCII.GetString(bytes, 0, bytesRead));
                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetRandomTips(string text)
        {
            try
            {
                if (this.listBox1.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetRandomTips);
                    this.Invoke(d, new object[] { text });
                }
                else
                {
                    string []list = text.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

                    int count = list.Length;

                    Random rand = new Random();
                    int r_count = rand.Next(1, count + 1);

                    string[] r_tips_list = new string[r_count];

                    for (int i = 0; i < r_count; i++)
                    {                        
                        r_tips_list[i] = list[i];                        
                    }
                    
                    this.listBox1.Items.Clear();

                    this.listBox1.Items.AddRange(r_tips_list);
                    this.listBox1.Visible = true;                                       
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string s = textBox1.Text;
                string[] s2 = s.Split('\n');
                s = s2[s2.Length - 1];
                s2 = s.Split(' ');
                s = s2[s2.Length - 1];

                byte[] byteText = Encoding.ASCII.GetBytes(s);
                ns.Write(byteText, 0, byteText.Length);

                if ((!textBox1.Text.EndsWith(" ")) && (textBox1.Text.Length != 0) && (textBox1.Text.StartsWith("\n")))
                {
                    listBox1.Visible = true;                    
                }
                else
                {
                    listBox1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
      
        private void ListBox1_SelectedValueChanged_1(object sender, EventArgs e)
        {
            try
            {
                if (!textBox1.Text.EndsWith(" ") && textBox1.Text.Length != 0)
                {
                    var selected = listBox1.SelectedItem.ToString();
                    var index = textBox1.Text.LastIndexOf(" ");
                    var index_n = textBox1.Text.LastIndexOf("\n");
                    
                    if (index > index_n)                    
                        textBox1.Text = textBox1.Text.Substring(0, index + 1);                    
                    else
                        textBox1.Text = textBox1.Text.Substring(0, index_n + 1);
                    textBox1.Text += selected;
                }
                else
                {
                    textBox1.Text += listBox1.SelectedItem;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}