using FaceRecognitionDotNet;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Image = FaceRecognitionDotNet.Image;

namespace offline_photo_recognition
{
    public partial class OfflinePhotosRecognitionForm : Form
    {
        private FaceRecognition _FaceRecognition;


        private Model model;
        string modelDir = "";
        private static readonly string[] EmotionLabels = new[]
{
        "Neutral", "Happiness", "Surprise", "Sadness",
        "Anger", "Disgust", "Fear", "Contempt"
    };

        public OfflinePhotosRecognitionForm()
        {
            InitializeComponent();
            var directory = Path.GetFullPath("Hog");
            modelDir = directory;
            _FaceRecognition = FaceRecognition.Create(modelDir);
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
                            using (MemoryStream ms = new MemoryStream(fileContent))
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

        private void PrintResult(string filename, Location location, string faceEmotion = "")
        {
            txtBxRresult.Text += $"{filename},{location.Top},{location.Left},{location.Bottom},{location.Right}" + faceEmotion + Environment.NewLine;
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
                        {
                            PrintResult(imageToCheck, faceLocation);
                        }
                    }
                    else
                    {
                        var faceLocation = new FaceRecognitionDotNet.Location(0, 0, 0, 0);
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

        private void btnPredictEmotion_Click(object sender, EventArgs e)
        {
            txtBxRresult.Text = string.Empty;
            txtBxRresult.Refresh();
            picBxImage.Image = null;
            picBxImage.Refresh();
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var fr = FaceRecognition.Create(modelDir);
                var imageToCheck = fileDialog.FileName;
                using (var unknownImage = FaceRecognition.LoadImageFile(imageToCheck))
                {
                    var faceLocations = fr.FaceLocations(unknownImage);

                    var croppedFaces = FaceRecognition.CropFaces(unknownImage, faceLocations).ToArray();
                    foreach (var croppedFace in croppedFaces) {
                        txtBxRresult.Text += $"{imageToCheck}" + Environment.NewLine;
                    }
                }
            }
        }

        private void btnComparePhoto_Click(object sender, EventArgs e)
        {
            txtBxRresult.Text = string.Empty;
            txtBxRresult.Refresh();
            picBxImage.Image = null;
            picBxImage.Refresh();
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var imageFile = fileDialog.FileName;
                var rtnValue = photoCompare(imageFile);
                Thread.Sleep(1000);
            }
        }

        private string photoCompare(string imageToCheck)
        {
            txtBxRresult.Text = "";
            txtBxRresult.Refresh();

            using (var unknownImage = FaceRecognition.LoadImageFile(imageToCheck))
            {
                var faceLocations = _FaceRecognition.FaceLocations(unknownImage);

                var croppedFaces = FaceRecognition.CropFaces(unknownImage, faceLocations);
                if (croppedFaces.Count() > 1)
                {
                    foreach (var croppedFace in croppedFaces)
                    {
                        imageToCompare(croppedFace, faceLocations);
                    }
                }
                else
                {
                    imageToCompare(unknownImage, faceLocations);

                }
            }
            return "";
        }

        private void imageToCompare(Image croppedFace, IEnumerable<Location> faceLocations)
        {

            var filesToCheck = "IndexedPatientPhotos";
            var isMatch = false;
            picBxImage.Image = croppedFace.ToBitmap();
            picBxImage.Refresh();
            var foundMatchFace = "";
            foreach (var imageFile in ImageFilesInFolder(filesToCheck))
            {
                try
                {
                    var unknownImage = FaceRecognition.LoadImageFile(imageFile);
                    var photoLocations = _FaceRecognition.FaceLocations(unknownImage);

                    var checkFace = FaceRecognition.CropFaces(unknownImage, photoLocations).FirstOrDefault();
                    picBxImage.Image = unknownImage.ToBitmap();
                    picBxImage.Refresh();

                    var encodings1 = _FaceRecognition.FaceEncodings(unknownImage, photoLocations);
                    var encodings2 = _FaceRecognition.FaceEncodings(croppedFace, faceLocations).FirstOrDefault();

                    isMatch = FaceRecognition.CompareFaces(
                    new[] { encodings1.FirstOrDefault() },
                    encodings2,
                    0.5
                ).FirstOrDefault();
                    if (isMatch)
                    {
                        foundMatchFace = imageFile;
                        break;
                    }
                }
                catch { }
            }

            if (isMatch)
            {
                txtBxRresult.Text = "Match found in "+ foundMatchFace + Environment.NewLine;
            }
            else
            {
                txtBxRresult.Text = "No Match Found" + Environment.NewLine;
            }
        }
    }
}
