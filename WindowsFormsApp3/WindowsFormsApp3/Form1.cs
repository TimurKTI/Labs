using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public delegate void DlTp();// Объявление типа (делегат) и 
        //создание пока что пустой ссылки для организации в последующем
        // с помощью ее вызова функции Invalidate()для главного потока
        public class ChangeCoords
        {
            private int X, Y, dx, dy, w, h;
            private bool live = true;
            //private DlTp dl = new DlTp();
            public void FnThr(DlTp dl1)
            {
                while (live)
                {
                    //здесь отражемся от границ области
                    if (X < 0 || Y > 200) dx = -dx;
                    if (Y < 0 || Y > 200) dy = -dy;
                    //здесь пересчитываем координаты
                    X += dx;
                    X += dy;
                    Thread.Sleep(100); //спим
                    dl1(); //вызываем с помощью делегата Invalidate()
                }
                w = h = 0; //схлопываем шарик
                dl1(); //вызываем с помощью делегата Invalidate()
            }
        }

        Ball[] bl = new Ball[10];//массив пустых ссылок типа Ball

        public class Ball
        {
            public int X; // координаты
            public int Y;
            public int dx, dy; //приращение координат-определяет скорость
            public int w, h;   //ширина высота шарика
            public bool live { get; set; }   // признак жизни 
            public DlTp dl;
            public Thread thr;  //Создание ссылки на потоковый объект
                                // потоковая функция
            public ChangeCoords ObjChangeCoords = new ChangeCoords();
            //функция рисования шарика
            public void DrawBall(Graphics dc)
            {
                dc.DrawEllipse(Pens.Magenta, X, Y, w, h);
            }
            //конструктор класса
            public Ball(int xn, int yn, int wn, int hn, int dxn, int dyn)
            {
                X = xn; Y = yn; w = wn; h = hn; dx = dxn; dy = dyn;//инициализируем
                thr = new Thread(new ThreadStart(ObjChangeCoords.FnThr)); //создаем потоковый объект
                live = true;    //устанавливаем признак жизни
                thr.Start();    //запускаем поток
            }
        }
        
        public Form1()
        {
            InitializeComponent();
            for (int j = 0; j < bl.Length; j++)
            {
                //создаем потоковые объекты
                bl[j] = new Ball(j, j * 10, 10, 10, j + 1, j + 1);
                //подписываемся на событие
                bl[j].dl += new DlTp(Invalidate);
            }
            
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            for (int j = 0; j < bl.Length; j++)
            {
                bl[j].DrawBall(e.Graphics);//рисуем
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            for (int j = 0; j < bl.Length; j++)
            {
                bl[j].live = false;// Уничтожаем потоки
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int j = 0; j < bl.Length; j++)
            {
                bl[j].live = false;//уничтожаем потоки
            }
        }
    }
}
