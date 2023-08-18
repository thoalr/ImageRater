namespace ImageRaterApp
{
    public partial class Form1 : Form
    {
        string currentDirectory;
        List<string> files;
        int currentFileIndex = 0;
        int numberOfFiles = 0;
        StreamWriter? outputFile;


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

                    currentDirectory = new FileInfo(filePath).DirectoryName ?? throw new NullReferenceException();
                    label3.Text = $"Current Directory: {currentDirectory}";
                    files = Directory.EnumerateFiles(currentDirectory).ToList();
                    currentFileIndex = 0;
                    numberOfFiles = files.Count();
                    outputFile = new StreamWriter(File.Open(Path.Combine(currentDirectory, "image_rating.csv"), FileMode.Append));
                    FirstImage();
                }
            }
        }

        void OpenNextImage()
        {
            var picture = files[currentFileIndex++];
            Bitmap image = null;
            while(image == null && currentFileIndex < numberOfFiles)
            {
                // If we fail to open the file as a Bitmap skip files until it succeeds
                try
                {
                    image = new Bitmap(picture);
                }
                catch {
                    picture = files[currentFileIndex++];
                }
                
            }
            label2.Text = $"Current: {picture}";
            pictureBox1.Image = new Bitmap(image);

            MessageBox.Show("Reached end of directory", "End of directory", MessageBoxButtons.OK);

            
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
            OpenNextImage();
            label1.Text = $"Progress: [{currentFileIndex}/{numberOfFiles}] {100 * currentFileIndex / numberOfFiles}%";
            radioButton3.Checked = true;
        }

        private void NextImage(object sender, EventArgs e)
        {
            if (currentFileIndex < numberOfFiles)
            {
                outputFile?.WriteLine($"{files[currentFileIndex]},{CurrentSelectedRating()}");
            }
            OpenNextImage();
            label1.Text = $"Progress: [{currentFileIndex}/{numberOfFiles}] {100*currentFileIndex/numberOfFiles}%";
            radioButton3.Checked = true;
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
                    break;
                case Keys.NumPad1:
                    radioButton2.Checked = true;
                    break;
                case Keys.NumPad2:
                    radioButton3.Checked = true;
                    break;
                case Keys.NumPad3:
                    radioButton4.Checked = true;
                 
                    break;
                case Keys.NumPad4:
                    radioButton5.Checked = true;
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
                case Keys.Enter:
                    NextImage(sender, e);
                    break;
                default:
                    break;
            }
        }

    }
}