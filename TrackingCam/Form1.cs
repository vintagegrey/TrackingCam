using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;

namespace TrackingCam
{
    public partial class Form1 : Form
    {
        FilterInfoCollection filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        VideoCaptureDevice device;
        static readonly CascadeClassifier haar = new CascadeClassifier(System.Environment.CurrentDirectory + @"\haarcascade_frontalface_alt_tree.xml");
        float faceX;
        float faceY;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (FilterInfo device in filter)
            {
                comboBox1.Items.Add(device.Name);
            }
            comboBox1.SelectedIndex = 0;
            device = new VideoCaptureDevice(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            device = new VideoCaptureDevice(filter[comboBox1.SelectedIndex].MonikerString);
            device.NewFrame += Device_NewFrame;
            device.Start();
        }

        private void Device_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
            Rectangle[] rectangles = haar.DetectMultiScale(grayImage, 1.2, 1);

            foreach (Rectangle rectangle in rectangles)
            {
                faceX = rectangle.X - 320;
                faceY = rectangle.Y - 256;
                System.Diagnostics.Debug.WriteLine($"Face coords: ({faceX} {faceY})");

                using (Graphics gfx = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Purple, 2))
                    {
                        gfx.DrawLine(pen, new Point(320, 0), new Point(320, 512));
                        gfx.DrawLine(pen, new Point(0, 256), new Point(640, 256));
                    }
                    using (Pen pen = new Pen(Color.Red, 3))
                    {
                        gfx.DrawRectangle(pen, rectangle);
                    }
                }
            }
            pictureBox1.Image = bitmap; 

            if (faceX < -227)
            {
                System.Diagnostics.Debug.WriteLine("~~~~~~~MOVE CAMERA LEFT~~~~~~~");
            }
            if (faceX > 130)
            {
                System.Diagnostics.Debug.WriteLine("~~~~~~~MOVE CAMERA RIGHT~~~~~~~");
            }
            if (faceY < -180)
            {
                System.Diagnostics.Debug.WriteLine("~~~~~~~MOVE CAMERA DOWN~~~~~~~");
            }
            if (faceY > 200)
            {
                System.Diagnostics.Debug.WriteLine("~~~~~~~MOVE CAMERA UP~~~~~~~");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (device.IsRunning) device.Stop(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TRACKING CAMERA, WRITTEN 9-25-2022." +
                "\n" +
                "\nThis program is part of the Robotic Arm Project and lesson plan." +
                "\nWithout the arm, this program is not functional." +
                "\n" +
                "\nThis app is built with WinForms and uses EMGU.CV version 4.1.1.3497." +
                "\nCompiled on Visual Studio Community 2022." +
                "\n" +
                "\nCredits:" +
                "\nProgramming: James Henry" +
                "\nDesign & Building: Aiden Reittinger");
        }
    }
}