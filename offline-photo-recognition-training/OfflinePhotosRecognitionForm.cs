using FaceRecognitionDotNet;
using System.Text.RegularExpressions;

namespace offline_photo_recognition
{
    public partial class OfflinePhotosRecognitionForm : Form
    {
        private FaceRecognition _FaceRecognition;
        private Model model;

        public OfflinePhotosRecognitionForm()
        {
            InitializeComponent();
            var directory = Path.GetFullPath("Hog");

            _FaceRecognition = FaceRecognition.Create(directory);
        }

        private void btnTestPhoto_Click(object sender, EventArgs e)
        {
            txtBxRresult.Text = string.Empty;
            txtBxRresult.Refresh();
            picBxImage.Image = null;
            picBxImage.Refresh();
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var imageFile = fileDialog.FileName;
                byte[] fileContent = File.ReadAllBytes(imageFile);
                using (MemoryStream ms = new MemoryStream(fileContent))
                {
                    picBxImage.Image = System.Drawing.Image.FromStream(ms);
                }
                TestImage(imageFile, model);
                Application.DoEvents();
                Thread.Sleep(1000);
            }
        }

        private void btnTtrainPhoto_Click(object sender, EventArgs e)
        {
            trainOfflineCollection();
        }

        private void trainOfflineCollection()
        {
            var strModel = "Hog";
            int cpus = chkBxCpus.Checked == true ? -1 : 1;

            picBxImage.Image = null;
            picBxImage.Refresh();
            txtBxRresult.Text = string.Empty;
            txtBxRresult.Refresh();

            Enum.TryParse<Model>(strModel, true, out model);

            var imageToCheck = "IndexedPatientPhotos";

            txtBxRresult.Text = "";

            if (Directory.Exists(imageToCheck))
                switch (cpus)
                {
                    case 1:
                        foreach (var imageFile in ImageFilesInFolder(imageToCheck))
                        {
                            byte[] fileContent = File.ReadAllBytes(imageFile);
                            using ( MemoryStream ms = new MemoryStream(fileContent))
                            {
                                picBxImage.Image = System.Drawing.Image.FromStream(ms);
                            }
                            TestImage(imageFile, model);
                            Application.DoEvents();
                            Thread.Sleep(1000);
                        }
                        break;
                    default:
                        ProcessImagesInProcessPool(ImageFilesInFolder(imageToCheck), cpus, model);
                        break;
                }
            else
                TestImage(imageToCheck, model);

        }
        private IEnumerable<string> ImageFilesInFolder(string folder)
        {
            return Directory.GetFiles(folder)
                            .Where(s => Regex.IsMatch(Path.GetExtension(s), "(jpg|jpeg|png)$", RegexOptions.Compiled));
        }

        private void PrintResult(string filename, Location location)
        {
            txtBxRresult.Text += $"{filename},{location.Top},{location.Left},{location.Bottom},{location.Right}\r\n";
        }

        private void ProcessImagesInProcessPool(IEnumerable<string> imagesToCheck, int numberOfCpus, Model model)
        {
            if (numberOfCpus == -1)
                numberOfCpus = Environment.ProcessorCount > 1 ? 1 : 0;

            var files = imagesToCheck.ToArray();
            var functionParameters = files.Select(s => new Tuple<string, Model>(s, model)).ToArray();

            var total = functionParameters.Length;
            var option = new ParallelOptions
            {
                MaxDegreeOfParallelism = numberOfCpus
            };

            Parallel.For(0, total, option, i =>
            {
                try
                {
                    var t = functionParameters[i];
                    TestImage(t.Item1, numberOfCpus, t.Item2);
                }
                catch (Exception ex) 
                {
                    var e = ex.Message;
                }
            });
        }

        private void TestImage(string imageToCheck, Model model)
        {
            try
            {
                using (var unknownImage = FaceRecognition.LoadImageFile(imageToCheck))
                {
                    var faceLocations = _FaceRecognition.FaceLocations(unknownImage, 0, model).ToArray();

                    if (faceLocations != null && faceLocations.Length > 0)
                    {
                        foreach (var faceLocation in faceLocations)
                            PrintResult(imageToCheck, faceLocation);
                    }
                    else
                    {
                        var faceLocation = new FaceRecognitionDotNet.Location(0,0,0,0);
                        PrintResult(imageToCheck, faceLocation);
                    }

                }
            }
            catch (Exception ex)
            {
                var e = ex.Message;
            }
        }

        private void TestImage(string imageToCheck, int numberOfCpus, Model model)
        {
            try
            {
                using (var unknownImage = FaceRecognition.LoadImageFile(imageToCheck))
                {
                    var faceLocations = _FaceRecognition.FaceLocations(unknownImage, numberOfCpus, model).ToArray();

                    foreach (var faceLocation in faceLocations)
                        PrintResult(imageToCheck, faceLocation);
                }
            }
            catch (Exception ex)
            {
                var e = ex.Message;
            }
        }
    }
}