using System;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.FreeGlut;
using System.Drawing;

namespace Lab3_baaa
{
    public partial class Form1 : Form
    {
        private double _Radius=5;
        private double _Height=5;
        private int _Slices=5;
        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // инициализация Glut
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            // очитка окна
            Gl.glClearColor(0, 0, 0, 1); // черный фон
            // установка порта вывода в соотвествии с размерами элемента 
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);
            // настройка параллельной проекции
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            const double W = 7;
            double H = W * AnT.Height / AnT.Width;
            Gl.glOrtho(-W, W, -H, H, -200, 200);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            // настройка параметров OpenGL для визуализации
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glEnable(Gl.GL_LIGHTING);

            Gl.glEnable(Gl.GL_LIGHT0);
        }
        private void Render(double x, double y)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glLoadIdentity();
            Gl.glColor3f(1, 1, 1); // цвет фигуры - белый

            Gl.glPushMatrix();
            Gl.glTranslated(0, -1, -6);
            Gl.glRotated((x - AnT.Height) * 180 / AnT.Height, 0, 1, 0);
            Gl.glRotated((y - AnT.Width) * 180 / AnT.Width, 1, 0, 0);
            Glut.glutSolidCone(_Radius, _Height, _Slices, 1);
            Gl.glPopMatrix();
            Gl.glFlush();
            AnT.Invalidate();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Отключаем все предыдущие источники света
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    Gl.glDisable(Gl.GL_LIGHT0 + i);
                }

                // Получаем координаты одноточечного источника из первой строки таблицы
                float posX = (float)Convert.ToDecimal(dataGridView1.Rows[0].Cells[0].Value);
                float posY = (float)Convert.ToDecimal(dataGridView1.Rows[0].Cells[1].Value);
                float posZ = (float)Convert.ToDecimal(dataGridView1.Rows[0].Cells[2].Value);

                // Включаем освещение для одноточечного источника
                Gl.glEnable(Gl.GL_LIGHT0);
                float[] pos = { posX, posY, posZ, 1.0f };
                Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, pos);

                // Устанавливаем цвет одноточечного источника из четвертой ячейки таблицы
                Color color = (Color)dataGridView1.Rows[0].Cells[3].Value;
                float[] lightColor = { color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, 1.0f };
                Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, lightColor);
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка входных данных!");
            }
        }

        private void AnT_MouseMove(object sender, MouseEventArgs e)
        {
            Render(e.X, e.Y);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3) if (DialogResult.OK == colorDialog1.ShowDialog()) dataGridView1.Rows[e.RowIndex].Cells[3].Value = colorDialog1.Color;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _Radius = Convert.ToDouble(textBox1.Text);
            }
            catch (Exception)
            {
                _Radius = 5;
            }
            try
            {
                _Height = Convert.ToDouble(textBox2.Text);
            }
            catch (Exception)
            {
                _Height = 5;
            }
            try
            {
                _Slices = Convert.ToInt32(textBox3.Text);
            }
            catch (Exception)
            {
                _Slices = 5;
            }
        }
    }
}
