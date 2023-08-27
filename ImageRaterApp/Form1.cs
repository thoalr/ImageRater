using System.Reflection.Metadata.Ecma335;
using System;

namespace ImageRaterApp
{
    public partial class Form1 : Form
    {
        ImageRaterController? rater;
        // prevents navigation when false
        bool rated = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void OpenDirectory(object sender, EventArgs e)
        {


            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "image files|*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    var filePath = openFileDialog.FileName;

                    string currentDirectory = new FileInfo(filePath).DirectoryName ?? throw new NullReferenceException();
                    rater?.SaveRatings();
                    rater?.Close();
                    rater = new ImageRaterController(currentDirectory);
                    
                    label3.Text = rater?.StringCurrentDirectory();
                    
                    FirstImage();
                }
            }
        }

        void OpenImage()
        {
            if (rater == null) return;
            var picture = rater?.GetCurrentFile();
            Bitmap image = null;
            while(image == null)
            {
                // If we fail to open the file as a Bitmap skip files until it succeeds
                try
                {
                    image = new Bitmap(picture);
                }
                catch {
                    bool end = rater?.NextFile() ?? false;
                    picture = rater?.GetCurrentFile();
                    if (end) { 
                        MessageBox.Show("Reached end of directory wihtout finding valid image file", 
                            "End of directory", 
                            MessageBoxButtons.OK);
                        return;
                    }
                }
                
            }
            label2.Text = rater?.StringCurrentFile();
            pictureBox1.Image = image;
    
        }


        int CurrentSelectedRating()
        {
            if (radioButton1.Checked) return 0;
            else if (radioButton2.Checked) return 1;
            else if (radioButton3.Checked) return 2;
            else if (radioButton4.Checked) return 3;
            else if (radioButton5.Checked) return 4;
            else return -1;
        }

        void FirstImage()
        {
            OpenImage();
            label1.Text = rater?.StringProgress();
            checkBox1.Checked = rater?.IsRated() ?? false;
            radioButton3.Checked = true;
        }

        private void NextImage(object sender, EventArgs e)
        {
            if (!rated) return;
            rater?.SetRating(CurrentSelectedRating());
            if(rater?.NextFile() ?? false)
                MessageBox.Show("Reached end of directory", "End of directory", MessageBoxButtons.OK);
            OpenImage();
            label1.Text = rater?.StringProgress();
            checkBox1.Checked = rater?.IsRated() ?? false;
            radioButton3.Checked = true;
        }

        private void PreviousImage(object sender, EventArgs e)
        {
            if (!rated) return;
            rater?.SetRating(CurrentSelectedRating());
            rater?.PreviousFile();
            OpenImage();
            label1.Text = rater?.StringProgress();
            checkBox1.Checked = rater?.IsRated() ?? false;
            radioButton3.Checked = true;
        }

        void UpdateCheckBox()
        {
            checkBox1.Checked = rater?.IsRated() ?? false;
        }

        private void KeyDowned(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;

            switch(key)
            {
                case Keys.Escape:
                    break;
                case Keys.NumPad0:
                    radioButton1.Checked = true;
                    rated = true;
                    UpdateCheckBox();
                    break;
                case Keys.NumPad1:
                    radioButton2.Checked = true;
                    rated = true;
                    UpdateCheckBox();
                    break;
                case Keys.NumPad2:
                    radioButton3.Checked = true;
                    rated = true;
                    UpdateCheckBox();
                    break;
                case Keys.NumPad3:
                    radioButton4.Checked = true;
                    rated = true;
                    break;
                case Keys.NumPad4:
                    radioButton5.Checked = true;
                    rated = true;
                    UpdateCheckBox();
                    break;
                case Keys.NumPad5:
                    break;
                case Keys.NumPad6:
                    break;
                case Keys.NumPad7:
                    break;
                case Keys.NumPad8:
                    break;
                case Keys.NumPad9:
                    break;
                case Keys.Left:
                    PreviousImage(sender, e);
                    break;
                case Keys.Right:
                    NextImage(sender, e);
                    break;
                case Keys.Enter:
                    NextImage(sender, e);
                    break;
                default:
                    break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            rater?.SaveRatings();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(rater != null)
            {
                rater.SaveRatings();
                rater = null;
                rated = false;
                pictureBox1.Image = null;
                radioButton3.Checked = true;
                checkBox1.Checked = false;
            }
        }
    }
}