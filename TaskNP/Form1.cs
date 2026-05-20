using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace TaskNP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        TcpListener server;
        bool isRunning = false;
        private void Startbuttom_Click(object sender, EventArgs e)
        {
            server = new TcpListener(IPAddress.Any, 5000);

            server.Start();

            isRunning = true;

            richTextBox1.AppendText("Server Started...\n");
            textBox1.AppendText("Listening on port 5000...\n");

            Thread t = new Thread(StartServer);

            t.IsBackground = true;

            t.Start();

        }

        void StartServer()
        {
            while (isRunning)
            {
                TcpClient client = server.AcceptTcpClient();

                richTextBox1.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText("Client Connected\n");
                }));

                Thread clientThread = new Thread(() =>
                {
                    HandleClient(client);
                });

                clientThread.IsBackground = true;

                clientThread.Start();
            }
        }
        void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream ns = client.GetStream();

                BinaryReader br = new BinaryReader(ns);

                BinaryWriter bw = new BinaryWriter(ns);

                // Receive File Name
                string fileName = br.ReadString();

                // Receive File Size
                int fileSize = br.ReadInt32();
                // Receive File Bytes
                byte[] fileBytes = br.ReadBytes(fileSize);

                // Save Original File
                string originalPath = "Received_" + fileName;
                File.WriteAllBytes(originalPath, fileBytes);

                richTextBox1.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText("File Received\n");
                }));

                // =========================
                // COMPRESS FILE
                // =========================

                string compressedPath = originalPath + ".gz";

                string zipPath = originalPath + ".zip";

                // Delete old zip if exists
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                // Create ZIP
                using (ZipArchive archive =
                       ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(originalPath,
                        Path.GetFileName(originalPath));
                }

                richTextBox1.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText("Compression Done\n");
                }));

                // Read Compressed File
                byte[] compressedBytes = File.ReadAllBytes(zipPath);

                // Send Compressed File Size First
                bw.Write(compressedBytes.Length);

                // Send Compressed File
                bw.Write(compressedBytes);

                richTextBox1.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText("Compressed File Sent\n");
                }));
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(ex.Message + "\n");
                }));
            }
        }
    }
}
