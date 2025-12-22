namespace offline_photo_recognition
{
    partial class OfflinePhotosRecognitionForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            picBxImage = new PictureBox();
            btnTestPhoto = new Button();
            btnTtrainPhoto = new Button();
            txtBxRresult = new TextBox();
            label1 = new Label();
            chkBxCpus = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)picBxImage).BeginInit();
            SuspendLayout();
            // 
            // picBxImage
            // 
            picBxImage.Location = new Point(393, -1);
            picBxImage.Name = "picBxImage";
            picBxImage.Size = new Size(640, 480);
            picBxImage.TabIndex = 0;
            picBxImage.TabStop = false;
            // 
            // btnTestPhoto
            // 
            btnTestPhoto.Location = new Point(12, 32);
            btnTestPhoto.Name = "btnTestPhoto";
            btnTestPhoto.Size = new Size(150, 23);
            btnTestPhoto.TabIndex = 1;
            btnTestPhoto.Text = "Test";
            btnTestPhoto.UseVisualStyleBackColor = true;
            btnTestPhoto.Click += btnTestPhoto_Click;
            // 
            // btnTtrainPhoto
            // 
            btnTtrainPhoto.Location = new Point(179, 32);
            btnTtrainPhoto.Name = "btnTtrainPhoto";
            btnTtrainPhoto.Size = new Size(150, 23);
            btnTtrainPhoto.TabIndex = 2;
            btnTtrainPhoto.Text = "Train";
            btnTtrainPhoto.UseVisualStyleBackColor = true;
            btnTtrainPhoto.Click += btnTtrainPhoto_Click;
            // 
            // txtBxRresult
            // 
            txtBxRresult.Location = new Point(42, 527);
            txtBxRresult.Multiline = true;
            txtBxRresult.Name = "txtBxRresult";
            txtBxRresult.Size = new Size(886, 236);
            txtBxRresult.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(42, 500);
            label1.Name = "label1";
            label1.Size = new Size(44, 15);
            label1.TabIndex = 4;
            label1.Text = "Results";
            // 
            // chkBxCpus
            // 
            chkBxCpus.AutoSize = true;
            chkBxCpus.Location = new Point(12, 61);
            chkBxCpus.Name = "chkBxCpus";
            chkBxCpus.Size = new Size(67, 19);
            chkBxCpus.TabIndex = 5;
            chkBxCpus.Text = "Cpus -1";
            chkBxCpus.UseVisualStyleBackColor = true;
            // 
            // OfflinePhotosRecognition
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1045, 786);
            Controls.Add(chkBxCpus);
            Controls.Add(label1);
            Controls.Add(txtBxRresult);
            Controls.Add(btnTtrainPhoto);
            Controls.Add(btnTestPhoto);
            Controls.Add(picBxImage);
            Name = "OfflinePhotosRecognition";
            Text = " Offline Photo Recognition";
            ((System.ComponentModel.ISupportInitialize)picBxImage).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private PictureBox picBxImage;
        private Button btnTestPhoto;
        private Button btnTtrainPhoto;
        private TextBox txtBxRresult;
        private Label label1;
        private CheckBox chkBxCpus;
    }
}