namespace Lab1_14
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonChooseImg_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "����� �����������|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // �������� �����������
                    Bitmap image = new Bitmap(openFileDialog.FileName);

                    // ����������� ����������� � PictureBox
                    pictureBoxIn.Image = image;
                    Bitmap smoothed = ImageSmoothing.ApplySmoothing(image);

                    // ���������� ����������
                    //smoothed.Save(image);
                    pictureBoxOut.Image = smoothed;
                }
            }
        }
    }
}