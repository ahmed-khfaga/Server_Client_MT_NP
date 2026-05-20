using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

namespace ClientTaskNp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        TcpClient client;
        NetworkStream ns;
        private void Connectbutton_Click(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient();

                client.Connect("127.0.0.1", 5000);

                ns = client.GetStream();

                textBox2.Text = "Connected to server";

                richTextBox1.AppendText("Connected To Server\n");
            }
            catch (Exception ex)
            {
               textBox2.Text = "Connection Failed: " + ex.Message;
            }

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void chossebutton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;

                richTextBox1.AppendText("File Selected\n");
            }
        }

        private void sendbutton_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = textBox1.Text;

                byte[] fileBytes = File.ReadAllBytes(filePath);

                BinaryWriter bw = new BinaryWriter(ns);

                string fileName = Path.GetFileName(filePath);

                bw.Write(fileName);

                bw.Write(fileBytes.Length);

                bw.Write(fileBytes);

                richTextBox1.AppendText("File Sent To Server\n");



                BinaryReader br = new BinaryReader(ns);

                int compressedSize = br.ReadInt32();

                byte[] compressedBytes =
                    br.ReadBytes(compressedSize);

                string savePath = fileName + ".zip";

                File.WriteAllBytes(savePath, compressedBytes);

                richTextBox1.AppendText("Compressed File Received\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
