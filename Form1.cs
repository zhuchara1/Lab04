using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Lab04
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            toolTip1.SetToolTip(button_fileKEYsave, "Зберегти");
            toolTip1.SetToolTip(button_fileKEYopen, "Відкрити");
        }

        private void button_fileIN_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = " txt files(*.txt)| *.txt | All files(*.*) | *.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Шлях до файлу
                    string fileINpath = openFileDialog.FileName;
                    textBox_fileIN.Text = fileINpath;
                }
            }
        }

        private void button_fileOUT_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = " txt files(*.txt)| *.txt | All files(*.*) | *.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Шлях до файлу
                string fileOUTpath = saveFileDialog.FileName;
                textBox_fileOUT.Text = fileOUTpath;
            }
        }

        private void button_fileKEYopen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = " txt files(*.txt)| *.txt | All files(*.*) | *.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Шлях до файлу
                    string fileKEYPath = openFileDialog.FileName;
                    textBox_fileKEY.Text = fileKEYPath;
                }
            }
        }

        private void button_fileKEYsave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = " txt files(*.txt)| *.txt | All files(*.*) | *.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Шлях до файлу
                string fileKEYPath = saveFileDialog.FileName;
                textBox_fileKEY.Text = fileKEYPath;
            }
        }

        private void button_KEYgenerator_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(" Стоворити новий ключ ?", "Увага",
MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;
                string fileINpath = textBox_fileIN.Text;
                if (File.Exists(fileINpath))
                {
                    string fileKEYpath = textBox_fileKEY.Text;
                    string dirKEYpath = Path.GetDirectoryName(fileKEYpath);
                    if (Directory.Exists(dirKEYpath))
                    {
                        //Довжина вхідного файлу у байтах
                        long lengthINfile = new FileInfo(fileINpath).Length;
                        //Створення ключа довжиною, як вхідний файл
                        byte[] arrKEY = new byte[lengthINfile];
                        RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
                        rngCsp.GetBytes(arrKEY);
                        //Збереження згенерованного ключа у файл з ключем
                        using (FileStream fs = File.Create(fileKEYpath))
                        {
                            fs.Write(arrKEY, 0, arrKEY.Length);
                            fs.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Шлях до ключа не вказаний\nабо такий шлях не існує");
                        textBox_fileKEY.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Вхідний файл не існує");
                    button_fileIN.Focus();
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string fileINpath = textBox_fileIN.Text;
            string fileOUTpath = textBox_fileOUT.Text;
            string fileKEYpath = textBox_fileKEY.Text;
            if (File.Exists(fileINpath))
            {
                string dirOUTpath = Path.GetDirectoryName(fileOUTpath);
                if (Directory.Exists(dirOUTpath))
                {
                    if (File.Exists(fileKEYpath))
                    {
                        //Довжина вхідного файлу у байтах
                        long lengthINfile = new FileInfo(fileINpath).Length;
                        //Довжина вхідного файлу у байтах
                        long lengthKEYfile = new FileInfo(fileKEYpath).Length;
                        if (lengthINfile == lengthKEYfile)
                        {
                            Stopwatch stopwatch = new Stopwatch();
                            // Begin timing.
                            stopwatch.Start();
                            byte[] arrCipher = myCipherXOR(fileINpath, fileKEYpath);
                            //шифрування
                            mySaveToFileOUT(arrCipher, fileOUTpath); //збереження у файл
                                                                     // Stop timing.
                            stopwatch.Stop();
                            // Write result.
                            label_time.Text = stopwatch.Elapsed.ToString(@"mm\:ss\.fff");
                            long size = Encoding.UTF8.GetBytes(fileKEYpath).Length;
                            label5.Text = ("Розмір ключа:\n" + size.ToString() + " байт");
                            FileInfo fileWithKey = new FileInfo(fileOUTpath);
                            long size1 = fileWithKey.Length;
                            label7.Text = ("Розмір файла з ключем: " + size1.ToString() + " байт");

                        }
                        else
                        {
                            MessageBox.Show("Розмір файлу ключа не співпадає\nз розміром вхідного файлу");
                            button_fileKEYopen.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Шлях до файлу ключа не вказаний\nабо такий файл не існує");
                        textBox_fileKEY.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Шлях до вихідного файлу не вказаний\nабо такий шлях не існує");
                    textBox_fileOUT.Focus();
                }
            }
            else
            {
                MessageBox.Show(" Вхідний файл не існує");
                button_fileIN.Focus();

            }
            Cursor.Current = Cursors.Default;
        }
        private byte[] myCipherXOR(string fileINpath, string fileKEYpath)
        {
            byte[] arrIN = File.ReadAllBytes(fileINpath);
            byte[] arrKEY = Encoding.UTF8.GetBytes(fileKEYpath);
            int lenIN = arrIN.Length;
            int lenKEY = arrKEY.Length;
            byte[] arrCipher = new byte[lenIN];

            for (int i = 0; i < lenIN; i++)
            {
                //Шифрування XOR
                byte p = arrIN[i];
                byte k = arrKEY[i % lenKEY];
                byte c = (byte)(p ^ k);

                arrCipher[i] = c;
            }
            return arrCipher;
        }
        private void mySaveToFileOUT(byte[] arrCipher, string fileOUTpath)
        {
            //Збереження у файл
            using (FileStream fs = File.Create(fileOUTpath))
            {
                fs.Write(arrCipher, 0, arrCipher.Length);
                fs.Close();
            }
        }
    }

}