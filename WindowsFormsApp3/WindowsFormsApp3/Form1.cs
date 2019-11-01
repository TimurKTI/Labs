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
			public ChangeCoords(DlTp dl, int dx, int dy, int w, int h)
			{
				dl1 = dl;
				this.dx = dx;
				this.dy = dy;
				this.w = w;
				this.h = h;
			}
			static object obj = new object();
			public int X, Y, dx, dy, w, h;
			public bool live = true;
			private DlTp dl1;
			public void FnThr()
			{
				//lock (obj)
				//{
				while (live)
				{
					//здесь отражемся от границ области
					if (X < 0 || X > 200) dx = -dx;
					if (Y < 0 || Y > 200) dy = -dy;
					//здесь пересчитываем координаты
					X += dx;
					Y += dy;
					Thread.Sleep(100); //спим

					dl1(); //вызываем с помощью делегата Invalidate()
				}
				if(!live)
				{
					w = h = 0; //схлопываем шарик
					dl1(); //вызываем с помощью делегата Invalidate()
				}
				
					  //}
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
			public ChangeCoords ObjChangeCoords;
			//функция рисования шарика
			public void DrawBall(Graphics dc)
			{
				dc.DrawEllipse(Pens.Magenta, ObjChangeCoords.X, ObjChangeCoords.Y, ObjChangeCoords.w, ObjChangeCoords.h);
			}
			//конструктор класса
			public Ball(int xn, int yn, int wn, int hn, int dxn, int dyn)
			{

				X = xn; Y = yn; w = wn; h = hn; dx = dxn; dy = dyn;//инициализируем

			}
			public void Substring()
			{
				ObjChangeCoords = new ChangeCoords(dl, dx, dy, w, h);
				thr = new Thread(ObjChangeCoords.FnThr); //создаем потоковый объект
				ObjChangeCoords.live = true;    //устанавливаем признак жизни
				thr.Start();    //запускаем поток
			}
		}

		public Form1()
		{
			InitializeComponent();
			for (int j = 0; j < bl.Length; j++)
			{
				//создаем потоковые объекты
				bl[j] = new Ball(j, j * 10, 10, 10, j + 1, j ^ (j - 5));
				//подписываемся на событие
				bl[j].dl += new DlTp(Invalidate);
				bl[j].Substring();
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
				bl[j].ObjChangeCoords.live = false;// Уничтожаем потоки
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
