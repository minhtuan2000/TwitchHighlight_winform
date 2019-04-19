using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowFormApp
{
    public partial class Highlighter : Form
    { 
        public Highlighter()
        {
            InitializeComponent();
        }

        private int numHighlights = 15;
        private int lengthHighlights = 2;

        private void RunCommand(string fileName, string argument){
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = Environment.CurrentDirectory + @"\Code";
            startInfo.FileName = fileName;
            startInfo.Arguments = argument;
            process.StartInfo = startInfo;
            process.Start();
        }

        private void SetMessage(string message, Color color)
        {
            label3.Text = message;
            label3.ForeColor = color;
        }

        private void WaitNSeconds(int segundos)
        {
            if (segundos < 1) return;
            DateTime _desired = DateTime.Now.AddSeconds(segundos);
            while (DateTime.Now < _desired)
            {
                Application.DoEvents();
            }
        }

        private void CreateButtons(string link, string id)
        {
            File.Delete(Environment.CurrentDirectory + @"\Code\results" + id + ".txt");

            RunCommand("minh.exe", id + ".txt results" + id + ".txt " + numHighlights.ToString() + " " + lengthHighlights.ToString());

            while (!File.Exists(Environment.CurrentDirectory + @"\Code\results" + id + ".txt"))
            {
                WaitNSeconds(1);
            }

            string[] lines = File.ReadAllLines(Environment.CurrentDirectory + @"\Code\results" + id + ".txt");

            List<Button> buttons = this.Controls.OfType<Button>().ToList();
            foreach (Button btn in buttons)
            {
                this.Controls.Remove(btn);
                btn.Dispose();
            }

            for (int i = 0; i < lines.Length; i++)
            {
                Button x = new Button();
                x.Left = (i % 5) * 35 + 30;
                x.Top = (i / 5) * 35 + 90;
                x.Width = 30;
                x.Height = 30;
                x.Text = (i + 1).ToString();
                x.BackColor = Color.Purple;
                x.ForeColor = Color.White;
                x.Click += (object se, EventArgs ev) => {
                    Button button = (se as Button);
                    int cnt = (button.Top - 90) / 35 * 5 + (button.Left - 30) / 35;
                    Process.Start(link + "?t=" + lines[cnt]);
                };
                this.Controls.Add(x);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string link = textBox1.Text;
            if (link.StartsWith("https://www.twitch.tv/videos/"))
            {
                string id = link.Split('/').Last();

                SetMessage("Loading...", Color.Green);

                if (File.Exists(Environment.CurrentDirectory + @"\Code\" + id + ".txt"))
                {
                    SetMessage("Finished", Color.Black);
                    CreateButtons(link, id);
                }
                else
                {
                    RunCommand("RechatToolnew.exe", "-D " + id);

                    long fileSize = 0;
                    while (textBox1.Text == link)
                    {
                        WaitNSeconds(7);

                        long newFileSize = fileSize;

                        if (File.Exists(Environment.CurrentDirectory + @"\Code\" + id + ".txt"))
                        {
                            newFileSize = new FileInfo(Environment.CurrentDirectory + @"\Code\" + id + ".txt").Length;
                            if (newFileSize == fileSize)
                            {
                                SetMessage("Finished", Color.Black);
                                break;
                            }
                        }

                        fileSize = newFileSize;

                        SetMessage("Still loading, please wait ...", Color.Green);

                        CreateButtons(link, id);
                    }

                
                }
            } else
            {
                SetMessage("Invalid link", Color.Red);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (this.Height == 320)
            {
                this.Height = 400;
            }
            else
            {
                this.Height = 320;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                numHighlights = Convert.ToInt32(textBox2.Text);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
            {
                lengthHighlights = Convert.ToInt32(textBox3.Text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory + @"\Code"))
            {
                if (file.EndsWith(".txt") || file.EndsWith(".json")){
                    File.Delete(file);
                }
            }
        }
    }
}
