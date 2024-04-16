using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace Diffusers
{
    public partial class Form1 : Form
    {
        float X1 = 0, X2 = 3, Y1 = 1; //Начальные значения и точность для метода Рунге–Кутты–Мерсона

        /// <summary>
        /// Данная прававя часть уравнения
        /// </summary>
        float Function(float x, float y) { return y - x * x; }
        public Form1()
        {
            InitializeComponent();
            chart.ChartAreas[0].AxisX.Minimum = X1;
            chart.ChartAreas[0].AxisX.Maximum = X2;
            chart.ChartAreas[0].AxisX.Interval = 0.5;
            textBoxH.Text = "0,1";
            textBoxE.Text = "0,00001";

            checkedListBox.CheckOnClick = true;
            checkedListBox.ItemCheck += checkedListBox_SelectedIndexChanged;
        }


        /// <summary>
        /// Точное решение
        /// </summary>
        /// <param name="h">Шаг</param>
        /// <returns>Точки графика</returns>
        float[,] ExactSolution(float h)
        {
            int N = GetPoinsCount(h);
            float x, y;
            float[,] points = new float[N, 2];
            for (int i = 0; i < N; i++)
            {

                x = X1 + i * h;
                y = x * x + 2 * x + 2 - (float)Math.Pow(Math.E, (double)x); //Функция найдена аналитическим способом
                points[i, 0] = x;
                points[i, 1] = y;
            }
            return points;
        }


        /// <summary>
        /// Метод Эйлера
        /// </summary>
        /// <param name="h">Шаг</param>
        /// <returns>Точки графика</returns>
        float[,] Euler(float h)
        {
            int N = GetPoinsCount(h); Console.WriteLine(N);
            float x = X1, y = Y1;
            float[,] points = new float[N, 2];

            //Задаем изначальные значения
            points[0, 0] = x;
            points[0, 1] = y;
            for (int i = 1; i < N; i++)
            {
                x = X1 + i * h;
                y = points[i - 1, 1] + h * Function(points[i - 1, 0], points[i - 1, 1]);
                points[i, 0] = x;
                points[i, 1] = y;
            }
            return points;
        }

        /// <summary>
        /// Метод Рунге–Кутты–Мерсона
        /// </summary>
        /// <param name="H">Шаг с которого начнется вычисление</param>
        /// <param name="e">Точность</param>
        /// <returns>Точки графика</returns>
        float[,] Runge_Kutty_Merson(float H, float e)
        {
            float x = X1, y = Y1, h = H;
            List<List<float>> points;
            List<float> point;
            //Задаем изначальные значения
            point = new List<float>() { x, y };
            points = new List<List<float>>() { point };
            

            float k1, k2, k3, k4, k5;
            while(x < X2)
            {
                //Вычислим шаг 
                do
                {
                    k1 = h * Function(x, y);
                    k2 = h * Function(x + h / 3, y + k1 / 3);
                    k3 = h * Function(x + h / 3, y + k1 / 6 + k2 / 6);
                    k4 = h * Function(x + h / 2, y + k1 / 8 + k3 * 3 / 8);
                    k5 = h * Function(x + h, y + k1 / 2 - k3 * 3 / 2 + 2 * k4);

                    if (Math.Abs((2 * k1 - 9 * k3 + 8 * k4 - k5) / 30) >= e)
                    {
                        h /= 2;
                    }
                    else break;
                }
                while (true);
                x = x + h;
                y = y + k1 / 6 + k4 * 2 / 3 + k5 / 6;

                point = new List<float>() { x, y };
                points.Add(point);
                if (Math.Abs((2 * k1 - 9 * k3 + 8 * k4 - k5) / 30) <= e / 32) h *= 2;
            }

            float[,] pointsArray = new float[points.Count, 2];
            for (int i = 0; i < points.Count; i++)
            {
                pointsArray[i, 0] = points[i][0];
                pointsArray[i, 1] = points[i][1];
            }

            return pointsArray;
           
        }

        /// <summary>
        /// Метод Рунге–Кутты 4ого порядка
        /// </summary>
        /// <param name="h">Шаг</param>
        /// <returns>Точки графика</returns>
        float[,] Runge_Kutty_4(float h)
        {
            int N = GetPoinsCount(h);
            float x = X1, y = Y1;
            float[,] points = new float[N, 2];

            float k1, k2, k3, k4;
            //Задаем изначальные значения
            points[0, 0] = x;
            points[0, 1] = y;
            for (int i = 1; i < N; i++)
            {
                k1 = Function(x, y);
                k2 = Function(x + h / 2, y + h * k1 / 2);
                k3 = Function(x + h / 2, y + h * k2 / 2);
                k4 = Function(x + h, y + h * k3);
                x = x + h;
                y = y + h * (k1 + 2 * k2 + 2 * k3 + k4) / 6;
                points[i, 0] = x;
                points[i, 1] = y;
            }
            return points;

        }


        /// <summary>
        /// Метод Адамса 4ого порядка
        /// </summary>
        /// <param name="h">Шаг</param>
        /// <returns>Точки графика</returns>
        float[,] Adams(float h)
        {
            int N = GetPoinsCount(h), k = 4;
            float[,] points = new float[N, 2], first4Points = Runge_Kutty_4(h, k);
            float[] F = new float[k];
            
            //Создаем массив с изначальными точками полоученными методом того же порядка(4-ого) одношаговым методом
            for (int i = 0; i < k; i++)
            {
                points[i, 0] = first4Points[i, 0];
                points[i, 1] = first4Points[i, 1];
                F[i] = Function(points[i, 0], points[i, 1]);
            }


            float x = points[k-1, 0], y = points[k - 1, 1];
            for (int i = k; i < N; i++)
            {
                //Вычисляем x и y
                x += h;
                y = y + h * (55 * F[k - 1] - 59 * F[k - 2] + 37 * F[k - 3] - 9 * F[k - 4]) / 24;
                points[i, 0] = x;
                points[i, 1] = y;

                //Сдвигаем массив F и вычисляем новое значение
                for (int j = 0; j < k - 1; j++) F[j] = F[j + 1];
                F[k-1] = Function(x, y);
            }
            return points;
        }


        /// <summary>
        /// Метод Рунге–Кутты 4ого порядка
        /// </summary>
        /// <param name="h">Шаг</param>
        /// <param name="N">Кол-во точек для расчёта</param>
        /// <returns>Точки графика</returns>
        float[,] Runge_Kutty_4(float h, int N)
        {
            float x = X1, y = Y1;
            float[,] points = new float[N, 2];

            float k1, k2, k3, k4;
            //Задаем изначальные значения
            points[0, 0] = x;
            points[0, 1] = y;
            for (int i = 1; i < N; i++)
            {
                k1 = Function(x, y);
                k2 = Function(x + h / 2, y + h * k1 / 2);
                k3 = Function(x + h / 2, y + h * k2 / 2);
                k4 = Function(x + h, y + h * k3);
                x = x + h;
                y = y + h * (k1 + 2 * k2 + 2 * k3 + k4) / 6;
                points[i, 0] = x;
                points[i, 1] = y;
            }
            return points;

        }

        /// <summary>
        /// Узнать количество точек 
        /// </summary>
        /// <param name="h">Шаг</param>
        /// <returns>Точки графика</returns>
        int GetPoinsCount(float h)
        {
            return (int)((X2 - X1) / h) + 2;
        }


        /// <summary>
        /// Перерисовка графика по нажатию кнопки
        /// </summary>
        private void button_Click(object sender, EventArgs e)
        {
            float h, eps; //шаг
            float[,] points;
            try { 
                eps = float.Parse(textBoxE.Text);
                h = float.Parse(textBoxH.Text); 
            }
            catch { return; }

            points = ExactSolution(0.0001f);
            DrawFunctionOnPoints(points, "Точное решение                 ", 0);
            points = Euler(h);
            DrawFunctionOnPoints(points, "Метод Эйлера                   ", 1);
            points = Runge_Kutty_Merson(h, eps);
            DrawFunctionOnPoints(points, "Метод Рунге–Кутты–Мерсона      ", 2);
            points = Runge_Kutty_4(h);
            DrawFunctionOnPoints(points, "Метод Рунге–Кутты 4ого порядка ", 3);
            points = Adams(h);
            DrawFunctionOnPoints(points, "Метод Адамса 4ого порядка      ", 4);

        }

        /// <summary>
        /// Отрисовка графика по точкам
        /// </summary>
        /// <param name="points">Точки</param>
        /// <param name="name">Название графика</param>
        /// <param name="seriesNum">Номер серии</param>
        void DrawFunctionOnPoints(float[,] points, string name, int seriesNum)
        {
            chart.Series[seriesNum].Points.Clear();
            chart.Series[seriesNum].Name = name;
            chart.Series[seriesNum].ChartType = SeriesChartType.Line;
            chart.Series[seriesNum].BorderWidth = 2;

            for (int i = 0; i < points.GetLength(0); i++)
            {
                chart.Series[seriesNum].Points.AddXY(points[i, 0], points[i, 1]);
                if (i + 1 < points.GetLength(0) && points[i + 1, 0] >= X2)
                {
                    chart.Series[seriesNum].Points.AddXY(points[i + 1, 0], points[i + 1, 1]);
                    break;
                }
                
            }

        }

        /// <summary>
        /// Скрытие графиков по чек-листу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                chart.Series[i].Enabled = false;
            }

            foreach (int i in checkedListBox.CheckedIndices)
            {

                chart.Series[i].Enabled = true;
            }
        }


    }
}
