using AudioPhysics;
using AudioPhysics.Provider;
using GameOverlay.Drawing;
using GameOverlay.Windows;
using NAudio.Mixer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LifeParallel
{
    public partial class Form1 : Form
    {
        public static Form1 MW = new Form1();
        public AudioPhysics.Classi.Mixer GlobalMixer = new AudioPhysics.Classi.Mixer();
        private static bool AttivaAggiornamento = false;
        public bool isPlaying = false;
        Oscillatore OCV = new Oscillatore();
        private GraphicsWindow _window;

        private Dictionary<string, GameOverlay.Drawing.SolidBrush> _brushes;
        private Dictionary<string, GameOverlay.Drawing.Font> _fonts;
        private Dictionary<string, GameOverlay.Drawing.Image> _images;

        private GameOverlay.Drawing.Geometry _gridGeometry;
        private GameOverlay.Drawing.Rectangle _gridBounds;
        private Random _random;
        private long _lastRandomSet;
        private List<Action<GameOverlay.Drawing.Graphics, float, float>> _randomFigures;
        public Form1()
        {
            InitializeComponent();
            MW = this;

        }
        private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            if (e.RecreateResources)
            {
                foreach (var pair in _brushes) pair.Value.Dispose();
                foreach (var pair in _images) pair.Value.Dispose();
            }

            _brushes["black"] = gfx.CreateSolidBrush(0, 0, 0);
            _brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
            _brushes["red"] = gfx.CreateSolidBrush(255, 0, 0);
            _brushes["green"] = gfx.CreateSolidBrush(0, 0, 0);
            _brushes["blue"] = gfx.CreateSolidBrush(0, 0, 255);
            _brushes["background"] = gfx.CreateSolidBrush(255, 255, 255);
            _brushes["grid"] = gfx.CreateSolidBrush(0, 0, 0);
            _brushes["random"] = gfx.CreateSolidBrush(0, 0, 0);

            if (e.RecreateResources) return;

            _fonts["arial"] = gfx.CreateFont("Arial", 12);
            _fonts["consolas"] = gfx.CreateFont("Consolas", 14);

            _gridBounds = new GameOverlay.Drawing.Rectangle(0, 0, gfx.Width, gfx.Height);
            _gridGeometry = gfx.CreateGeometry();

            //for (int i = 0; i < field_vert_count; i++)
            //{
            //    var line = new Line(0, i * cell_size, cell_size * field_hor_count, i * cell_size);
            //    _gridGeometry.BeginFigure(line);
            //    _gridGeometry.EndFigure(false);
            //}
            //for (int i = 0; i < field_hor_count; i++)
            //{
            //    var line = new Line(i * cell_size, 0, i * cell_size, cell_size * field_vert_count);
            //    _gridGeometry.BeginFigure(line);
            //    _gridGeometry.EndFigure(false);
            //}

            _gridGeometry.Close();

        }
        private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            foreach (var pair in _brushes) pair.Value.Dispose();
            foreach (var pair in _fonts) pair.Value.Dispose();
            foreach (var pair in _images) pair.Value.Dispose();
        }



        private void DrawRandomFigure(GameOverlay.Drawing.Graphics gfx, float x, float y)
        {
            var action = _randomFigures[_random.Next(0, _randomFigures.Count)];

            action(gfx, x, y);
        }

        private GameOverlay.Drawing.SolidBrush GetRandomColor()
        {
            var brush = _brushes["random"];

            brush.Color = new GameOverlay.Drawing.Color(_random.Next(0, 256), _random.Next(0, 256), _random.Next(0, 256));

            return brush;
        }


        int[,] prev_field = null;
        int[,] current_field = null;
        int field_hor_count = 50;
        int field_vert_count = 50;
        float cell_size = 10;
        Random r = new Random();
        BufferedGraphicsContext CurrentContext = BufferedGraphicsManager.Current;
        BufferedGraphics bg;
        System.Drawing.Graphics graphics;

        BufferedGraphicsContext CurrentContext2 = BufferedGraphicsManager.Current;
        BufferedGraphics bg2;

        BufferedGraphicsContext CurrentContext3 = BufferedGraphicsManager.Current;
        BufferedGraphics bg3;
        System.Drawing.Graphics graphics2;
        System.Drawing.Graphics graphics3;

        bool UpdateField = true;


        private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            if (current_field != null && UpdateField)
            {
                Thread.Sleep(delay);
                var padding = 16;
                var infoText = new StringBuilder()
                    .Append("FPS: ").Append(gfx.FPS.ToString().PadRight(padding))
                    .Append("DeltaTime: ").Append(e.DeltaTime.ToString().PadRight(padding))
                    .ToString();

                gfx.ClearScene(_brushes["background"]);

                //gfx.DrawTextWithBackground(_fonts["consolas"], _brushes["black"], _brushes["background"], 58, 20, infoText);

                gfx.DrawGeometry(_gridGeometry, _brushes["grid"], 1.0f);

                if (_lastRandomSet == 0L || e.FrameTime - _lastRandomSet > 2500)
                {
                    _lastRandomSet = e.FrameTime;
                }

                _random = new Random(unchecked((int)_lastRandomSet));


                {
                    if (checkBox1.Checked)
                    {
                        gfx.DrawRectangle(_brushes["black"], new GameOverlay.Drawing.Rectangle(0, 0, cell_size * field_hor_count, cell_size * field_vert_count), 1);
                        for (int i = 0; i < field_vert_count; i++)
                        {
                            for (int j = 0; j < field_hor_count; j++)
                            {
                                if (current_field[i, j] == 0)
                                {
                                    //graphics.DrawRectangle(Pens.White, i * cell_size, j * cell_size, cell_size, cell_size);
                                }
                                if (current_field[i, j] == 2)
                                {
                                    //if(cell_size > 2)
                                    gfx.FillRectangle(_brushes["black"], i * cell_size, j * cell_size, (i * cell_size) + cell_size, (j * cell_size) + cell_size);

                                    //graphics.FillRectangle(Brushes.Black, i * cell_size, j * cell_size, cell_size, cell_size);
                                    //else
                                    //{
                                    //    graphics.DrawLine(Pens.Black, i * cell_size, j * cell_size, i * cell_size+1, j * cell_size+1);
                                    //}
                                }
                                if (current_field[i, j] == 1)
                                {
                                    //if(cell_size > 2)
                                    gfx.FillRectangle(_brushes["green"], i * cell_size, j * cell_size, (i * cell_size) + cell_size, (j * cell_size) + cell_size);

                                    //graphics.FillRectangle(Brushes.Black, i * cell_size, j * cell_size, cell_size, cell_size);
                                    //else
                                    //{
                                    //    graphics.DrawLine(Pens.Black, i * cell_size, j * cell_size, i * cell_size+1, j * cell_size+1);
                                    //}
                                }
                            }
                        }
                    }
                    if (radioButton1.Checked)
                        update_field_seq();
                    else update_field_parallel2();



                    //for (float row = _gridBounds.Top + 12; row < _gridBounds.Bottom - 120; row += 120)
                    //{
                    //    for (float column = _gridBounds.Left + 12; column < _gridBounds.Right - 120; column += 120)
                    //    {
                    //        DrawRandomFigure(gfx, column, row);
                    //    }
                    //}
                }
            }
        }

        float maxFPS = 75;
        float x_div = 0.5f;
        private void Form1_Load(object sender, EventArgs e)
        {
            AttivaAggiornamento = true;
            OCV.Prepara();
            if(checkBox3.Checked)
                GlobalMixer.Play();
            _brushes = new Dictionary<string, GameOverlay.Drawing.SolidBrush>();
            _fonts = new Dictionary<string, GameOverlay.Drawing.Font>();
            _images = new Dictionary<string, GameOverlay.Drawing.Image>();
            var gfx = new GameOverlay.Drawing.Graphics()
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true
            };
            _window = new GraphicsWindow(this.PointToScreen(panel1.Location).X, this.PointToScreen(panel1.Location).Y, panel1.Width, panel1.Height, gfx)

            //_window = new GraphicsWindow(this.Location.X + panel5.Width, this.Location.Y, panel1.Width, panel1.Height, gfx)
            {
                FPS = 60,
                IsTopmost = false,
                IsVisible = true
            };

            _window.DestroyGraphics += _window_DestroyGraphics;
            _window.DrawGraphics += _window_DrawGraphics;
            _window.SetupGraphics += _window_SetupGraphics;


            bg = CurrentContext.Allocate(panel1.CreateGraphics(), panel1.ClientRectangle);
            bg.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            bg.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics = bg.Graphics;

            bg2 = CurrentContext.Allocate(panel4.CreateGraphics(), panel4.ClientRectangle);
            bg2.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            bg2.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics2 = bg2.Graphics;


            bg3 = CurrentContext.Allocate(panelAlive.CreateGraphics(), panelAlive.ClientRectangle);
            bg3.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            bg3.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics3 = bg3.Graphics;


            cell_size = (float)panel1.Height / field_hor_count;

            current_field = new int[field_vert_count, field_hor_count];
            prev_field = new int[field_vert_count, field_hor_count];
            for (int i = 0; i < field_vert_count; i++)
            {
                for (int j = 0; j < field_hor_count; j++)
                {
                    current_field[i, j] = 0;
                    prev_field[i, j] = current_field[i, j];
                }
            }
            trackBar1.Value = 300;
            field_vert_count = trackBar1.Value;
            field_hor_count = trackBar1.Value;

            current_field = new int[field_vert_count, field_hor_count];
            prev_field = new int[field_vert_count, field_hor_count];
            for (int i = 0; i < field_vert_count; i++)
            {
                for (int j = 0; j < field_hor_count; j++)
                {
                    current_field[i, j] = 0;
                    prev_field[i, j] = current_field[i, j];
                }
            }
            cell_size = Math.Min((float)panel1.Height / field_vert_count, (float)panel1.Width / field_hor_count);
            label2.Text = "Размер поля: " + trackBar1.Value;
            x_div = (float)panel4.Width / 400;
            FillRandom();
            radioButton3_CheckedChanged((object)0, new EventArgs());
        }
        void FillRandom()
        {
            current_field = new int[field_vert_count, field_hor_count];
            prev_field = new int[field_vert_count, field_hor_count];

            for (int i = 0; i < field_vert_count; i++)
            {
                for (int j = 0; j < field_hor_count; j++)
                {
                    int rand = r.Next(0, 100);
                    if (rand >= 70)
                        current_field[i, j] = 1;
                    else
                        current_field[i, j] = 0;
                    prev_field[i, j] = current_field[i, j];
                }
            }
        }
        //==2 - не менять
        // 3 - оживаеь
        // <2 >3 - смерть
        void update_field_seq()
        {
            if (UpdateField)
            {
                for (int i = 0; i < field_vert_count; i++)
                {
                    for (int j = 0; j < field_hor_count; j++)
                    {
                        int alive_nb = get_alive_neighbors(i, j, prev_field);
                        if (alive_nb < 2 || alive_nb > 3)
                        {
                            current_field[i, j] = 0;
                        }
                        else
                        {
                            if (alive_nb == 3)
                                current_field[i, j] = 1;
                            else
                            {
                                if (alive_nb == 2)
                                {
                                    if (prev_field[i, j] != 0)
                                        current_field[i, j] = 2;
                                    else
                                        current_field[i, j] = 0;
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < field_vert_count; i++)
                {
                    for (int j = 0; j < field_hor_count; j++)
                    {
                        if (current_field[i, j] > 0)
                            prev_field[i, j] = current_field[i, j] / current_field[i, j];
                        else
                            prev_field[i, j] = current_field[i, j];
                    }
                }
            }

            else
            {
                for (int i = 0; i < field_vert_count; i++)
                {
                    for (int j = 0; j < field_hor_count; j++)
                    {
                        current_field[i, j] = prev_field[i, j];
                    }
                }
            }

        }

        List<int[,]> updated_parts = new List<int[,]>();

        void update_field_parallel(int task_count = 12)
        {
            updated_parts = new List<int[,]>();
            List<Thread> tasks = new List<Thread>();
            int tail = field_hor_count % task_count;
            int[,] prev_part = new int[0, 0];
            List<int[,]> prev_parts = new List<int[,]>();
            List<int> parts_size = new List<int>();
            timer1.Enabled = false;
            for (var i = 0; i < task_count; i++)
            {
                int part_size = 0;
                if (tail != 0)
                {
                    if (i != task_count - 1)
                    {
                        part_size = field_hor_count / task_count;
                    }
                    else
                    {
                        part_size = field_hor_count / task_count + tail;
                    }
                }
                else
                {
                    part_size = field_hor_count / task_count;
                }
                parts_size.Add(part_size);
                prev_part = new int[0, 0];
                if (i == 0)
                {
                    prev_part = new int[field_vert_count, part_size + 1];
                    int r = 0;
                    int s = 0;
                    for (int k = 0; k < field_vert_count; k++)
                    {
                        s = 0;
                        for (int l = 0; l < part_size + 1; l++)
                        {
                            prev_part[r, s] = prev_field[k, l];
                            s++;

                        }
                        r++;
                    }
                    //заполнить prev_part
                }
                else
                {
                    if (i == task_count - 1)

                    {
                        int r = 0;
                        int s = 0;
                        prev_part = new int[field_vert_count, part_size + 1];
                        for (int k = 0; k < field_vert_count; k++)
                        {
                            s = 0;
                            for (int l = field_hor_count - part_size - 1; l < field_hor_count; l++)
                            {
                                prev_part[r, s] = prev_field[k, l];
                                s++;
                            }
                            r++;
                        }
                        //заполнить prev_part
                    }
                    else
                    {
                        int r = 0;
                        int s = 0;
                        prev_part = new int[field_vert_count, part_size + 2];
                        for (int k = 0; k < field_vert_count; k++)
                        {
                            s = 0;
                            for (int l = i * part_size - 1; l < (i + 1) * part_size + 1; l++)
                            {
                                prev_part[r, s] = prev_field[k, l];
                                s++;
                            }
                            r++;
                        }
                        //заполнить prev_part
                    }
                }
                prev_parts.Add(prev_part);
                tasks.Add(new Thread(new ParameterizedThreadStart(update_part)));
                tasks[i].Start(prev_part as object);
                //object arg = i;
                //tasks.Add(new TaskFactory().StartNew(() =>
                //{
                //}, arg));

                //tasks.Add(Task.Run<int[,]>(() => { int[,] p = update_part(prev_part); return p; }));

            }
            foreach (Thread thread in tasks)
            { thread.Join(); }

            //for (int i = 0; i < tasks.Count; i++)
            //{
            //    updated_parts.Add(tasks[i].Result);
            //}
            int active_part_num = 0;
            int active_part_num_prev = 0;
            int y = 0;
            int[,] active_part = new int[0, 0];

            for (int i = 0; i < field_vert_count; i++)
            {
                y = 0;
                for (int j = 0; j < field_hor_count; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < parts_size.Count; k++)
                    {
                        if (j >= sum && j < sum + parts_size[k])
                        {
                            active_part_num = k;
                            break;
                        }
                        sum += parts_size[k];
                    }
                    //active_part_num = j / task_count;
                    if (active_part_num != active_part_num_prev)
                    {
                        y = 1;
                    }
                    active_part_num_prev = active_part_num;
                    active_part = updated_parts[active_part_num];
                    try
                    {
                        current_field[i, j] = active_part[i, y++];
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.Message); }
                }
            }
            for (int i = 0; i < field_vert_count; i++)
            {
                for (int j = 0; j < field_hor_count; j++)
                {
                    prev_field[i, j] = current_field[i, j];
                }
            }
            timer1.Enabled = true;
            Console.WriteLine();
        }

        void update_part(object param)
        {
            int[,] prev_part = (int[,])param;
            int[,] current_part = new int[prev_part.GetLength(0), prev_part.GetLength(1)];
            if (UpdateField)
            {
                for (int i = 0; i < prev_part.GetLength(0); i++)
                {
                    for (int j = 0; j < prev_part.GetLength(1); j++)
                    {
                        int alive_nb = get_alive_neighbors(i, j, prev_part);
                        if (alive_nb < 2 || alive_nb > 3)
                        {
                            current_part[i, j] = 0;
                        }
                        if (alive_nb == 3)
                            current_part[i, j] = 1;
                        if (alive_nb == 2)
                            current_part[i, j] = prev_part[i, j];
                    }
                }

                //for (int i = 0; i < field_vert_count; i++)
                //{
                //    for (int j = 0; j < field_hor_count; j++)
                //    {
                //        prev_field[i, j] = current_field[i, j];
                //    }
                //}
            }

            else
            {
                for (int i = 0; i < prev_part.GetLength(0); i++)
                {
                    for (int j = 0; j < prev_part.GetLength(1); j++)
                    {
                        current_part[i, j] = prev_part[i, j];
                    }
                }
            }
            updated_parts.Add(current_part);
        }

        private static int get_alive_neighbors(int i, int j, int[,] prev)
        {
            int count = 0;
            int[,] neighbours = new int[8, 2] { { i - 1, j - 1 }, { i - 1, j }, { i - 1, j + 1 }, { i, j + 1 }, { i + 1, j + 1 }, { i + 1, j }, { i + 1, j - 1 }, { i, j - 1 } };


            for (int k = 0; k < neighbours.GetLength(0); k++)
            {
                int i_ = neighbours[k, 0];
                int j_ = neighbours[k, 1];

                if (!(i_ < 0 || i_ > prev.GetLength(0) - 1 || j_ < 0 || j_ > prev.GetLength(1) - 1))
                {
                    if (prev[i_, j_] > 0)
                        count++;
                }
            }
            return count;



        }

        List<float> fps_points = new List<float>();
        List<float> fps_points_av = new List<float>();

        List<float> alive_points = new List<float>();
        List<float> alive_points_av = new List<float>();


        Queue<double> avFPS = new Queue<double>();
        float k = 0.5f;
        float k1 = 0.2f;

        float k3 = 0.2f;

        static float filVal = 0;
        static float alivefilVal = 0;
        static float soundfilVal = 0;
        float expRunningAverage(float newVal)
        {
            filVal += (newVal - filVal) * k;
            return filVal;
        }
        bool randomize_points = true;
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (current_field != null)
            {
                if (manual_input)
                {

                    if (mouse_button == MouseButtons.Left)
                    {
                        int x = (int)(mouse_position.X / cell_size);
                        int y = (int)(mouse_position.Y / cell_size);
                        if (x >= 0 && y >= 0 && x < field_hor_count-1 && y < field_vert_count-1)
                        {
                            int i = (int)(mouse_position.X / cell_size);
                            int j = (int)(mouse_position.Y / cell_size);
                            int val = randomize_points ? r.Next(0, 2) : 1;
                            while (i > 0 && i >= x - brush_radius)
                            {

                                j = y;
                                if ((i - x) * (i - x) + (j - y) * (j - y) < brush_radius * brush_radius)
                                {
                                    val = randomize_points ? r.Next(0, 2) : 1;
                                    prev_field[i, j] = val;
                                    current_field[i, j] = val;
                                }
                                while (j > 0 && j >= y - brush_radius)
                                {
                                    j--;
                                    if ((i - x) * (i - x) + (j - y) * (j - y) < brush_radius * brush_radius)
                                    {
                                        val = randomize_points ? r.Next(0, 2) : 1;
                                        prev_field[i, j] = val;
                                        current_field[i, j] = val;
                                    }
                                }
                                i--;
                                if ((i - x) * (i - x) + (j - y) * (j - y) < brush_radius * brush_radius)
                                {
                                    val = randomize_points ? r.Next(0, 2) : 1;
                                    prev_field[i, j] = val;
                                    current_field[i, j] = val;
                                }
                            }

                            i = (int)(mouse_position.X / cell_size);
                            j = (int)(mouse_position.Y / cell_size);
                            while (i > 0 && i >= x - brush_radius)
                            {

                                j = y;
                                if ((i - x) * (i - x) + (j - y) * (j - y) < brush_radius * brush_radius)
                                {
                                    val = randomize_points ? r.Next(0, 2) : 1;
                                    prev_field[i, j] = val;
                                    current_field[i, j] = val;
                                }
                                while (j < field_vert_count - 1 && j <= y + brush_radius)
                                {
                                    j++;
                                    if ((i - x) * (i - x) + (j - y) * (j - y) < brush_radius * brush_radius)
                                    {
                                        val = randomize_points ? r.Next(0, 2) : 1;
                                        prev_field[i, j] = val;
                                        current_field[i, j] = val;
                                    }
                                }
                                i--;
                                if ((i - x) * (i - x) + (j - y) * (j - y) < brush_radius * brush_radius)
                                {
                                    val = randomize_points ? r.Next(0, 2) : 1;
                                    prev_field[i, j] = val;
                                    current_field[i, j] = val;
                                }
                            }

                            i = (int)(mouse_position.X / cell_size);
                            j = (int)(mouse_position.Y / cell_size);
                            while (i < field_hor_count - 1 && i <= x + brush_radius)
                            {

                                j = y;
                                if ((i - x) * (i - x) + (j - y) * (j - y) < brush_radius * brush_radius)
                                {
                                    val = randomize_points ? r.Next(0, 2) : 1;
                                    prev_field[i, j] = val;
                                    current_field[i, j] = val;
                                }
                                while (j < field_vert_count - 1 && j <= y + brush_radius)
                                {
                                    j++;
                                    if ((i - x) * (i - x) + (j - y) * (j - y) < brush_radius * brush_radius)
                                    {
                                        val = randomize_points ? r.Next(0, 2) : 1;
                                        prev_field[i, j] = val;
                                        current_field[i, j] = val;
                                    }
                                }
                                i++;
                                if ((i - x) * (i - x) + (j - y) * (j - y) < brush_radius * brush_radius)
                                {
                                    val = randomize_points ? r.Next(0, 2) : 1;
                                    prev_field[i, j] = val;
                                    current_field[i, j] = val;
                                }
                            }

                            i = (int)(mouse_position.X / cell_size);
                            j = (int)(mouse_position.Y / cell_size);
                            while (i < field_hor_count - 1 && i <= x + brush_radius)
                            {
                                j = y;
                                if ((i - x) * (i - x) + (j - y) * (j - y) < brush_radius * brush_radius)
                                {
                                    val = randomize_points ? r.Next(0, 2) : 1;
                                    prev_field[i, j] = val;
                                    current_field[i, j] = val;
                                }
                                while (j > 0 && j >= y - brush_radius)
                                {
                                    j--;
                                    if ((i - x) * (i - x) + (j - y) * (j - y) < brush_radius * brush_radius)
                                    {
                                        val = randomize_points ? r.Next(0, 2) : 1;
                                        prev_field[i, j] = val;
                                        current_field[i, j] = val;
                                    }
                                }
                                i++;
                                if ((i - x) * (i - x) + (j - y) * (j - y) < brush_radius * brush_radius)
                                {
                                    val = randomize_points ? r.Next(0, 2) : 1;
                                    prev_field[i, j] = val;
                                    current_field[i, j] = val;
                                }
                            }
                        }
                    }
                    if (mouse_button == MouseButtons.Right)
                    {
                        { prev_field[(int)(mouse_position.X / cell_size), (int)(mouse_position.Y / cell_size)] = 0; current_field[(int)(mouse_position.X / cell_size), (int)(mouse_position.Y / cell_size)] = 0; }
                    }

                }
                graphics2.Clear(System.Drawing.Color.White);
                graphics3.Clear(System.Drawing.Color.White);

                if (radioButton3.Checked)
                {
                    var gfx = _window.Graphics;
                    double fps = gfx.FPS;
                    fps_points.Add((float)fps);
                    if (fps_points.Count > 1000)
                        fps_points.RemoveAt(0);
                }
                if (radioButton4.Checked)
                {
                    double fps = GetFps();
                    fps_points.Add((float)fps);
                    if (fps_points.Count > 1000)
                        fps_points.RemoveAt(0);
                    avFPS.Enqueue(fps);
                    if (avFPS.Count > 200)
                        avFPS.Dequeue();
                    fps = avFPS.Sum() / avFPS.Count;
                }

                float alive = 0;
                float sound = 0;
                for (int i = 0; i < current_field.GetLength(0); i++)
                {
                    for (int j = 0; j < current_field.GetLength(1); j++)
                    {
                        if (current_field[i, j] > 0)
                            alive++;
                        if (current_field[i, j] == 1)
                            sound++;
                    }
                }

                sound /= (float)(current_field.GetLength(0) * current_field.GetLength(1));
                OCV.GS.Frequenza = expRunningAverageSound(sound) * 25000;
                
                //
                //BeepSample.Beep((uint)(alive_ ), 3);
                alive /= (float)(current_field.GetLength(0) * current_field.GetLength(1));
                
                alive *= 100;

                alive_points.Add(alive);
                if (alive_points.Count > 1000)
                {
                    alive_points.RemoveAt(0);
                }


                alive_points_av.Clear();
                for (int i = 0; i < alive_points.Count; i++)
                {
                    alive_points_av.Add(expRunningAverageAlive((int)alive_points[i]));
                }
                List<PointF> points = new List<PointF>();

                points.Add(new PointF(panelAlive.Width - (x_div * (alive_points_av.Count - (alive_points_av.Count - 1) - 1)), panelAlive.Height));
                for (int i = alive_points_av.Count - 1; i >= 60; i--)
                {
                    PointF p1 = new PointF(panelAlive.Width - (x_div * (alive_points_av.Count - i - 1)), panelAlive.Height - panelAlive.Height * (alive_points_av[i] / 50));
                    //PointF p2 = new PointF(panel4.Width - (x_div * (fps_points_av.Count - (i - 1) - 1)), panel4.Height - panel4.Height * (fps_points_av[i - 1] / maxFPS));
                    points.Add(p1);
                    //graphics2.DrawLine(Pens.Red, p1, p2);
                    //graphics2.Draw
                }
                points.Add(new PointF(panelAlive.Width - (x_div * (alive_points_av.Count - 20 - 1)), panelAlive.Height));
                if (points.Count > 1)
                {
                    graphics3.FillClosedCurve(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 248, 189)), points.ToArray(), FillMode.Alternate, 0f);
                    graphics3.DrawCurve(new Pen(System.Drawing.Color.FromArgb(27, 0, 70), 3), points.ToArray(), 0f);
                }



                //filVal = 0;
                fps_points_av.Clear();

                for (int i = 0; i < fps_points.Count; i++)
                {
                    fps_points_av.Add(expRunningAverage((int)fps_points[i]));
                }
                points = new List<PointF>(); points.Add(new PointF(panel4.Width - (x_div * (fps_points_av.Count - (fps_points_av.Count - 1) - 1)), panel4.Height));

                for (int i = fps_points_av.Count - 1; i >= 60; i--)
                {
                    PointF p1 = new PointF(panel4.Width - (x_div * (fps_points_av.Count - i - 1)), panel4.Height - panel4.Height * (fps_points_av[i] / maxFPS));
                    //PointF p2 = new PointF(panel4.Width - (x_div * (fps_points_av.Count - (i - 1) - 1)), panel4.Height - panel4.Height * (fps_points_av[i - 1] / maxFPS));
                    points.Add(p1);
                    //graphics2.DrawLine(Pens.Red, p1, p2);
                    //graphics2.Draw
                }
                points.Add(new PointF(panel4.Width - (x_div * (fps_points_av.Count - 20 - 1)), panel4.Height));
                if (points.Count > 1)
                {
                    graphics2.FillClosedCurve(Brushes.LightBlue, points.ToArray(), FillMode.Alternate, 0f);
                    graphics2.DrawCurve(new Pen(System.Drawing.Color.FromArgb(166, 115, 62), 3), points.ToArray(), 0f);

                }
                if (fps_points_av.Count > 60)
                {
                    //graphics2.FillRectangle(Brushes.White, new RectangleF(panel4.Width - 25-3, 0, 25, 20));
                    //graphics2.DrawString(Math.Floor(fps_points_av[fps_points_av.Count - 1]).ToString(), new System.Drawing.Font("Consolas", 10, FontStyle.Bold), Brushes.Black, new PointF(panel4.Width - 25, 5));
                    graphics2.DrawString("FPS:" + Math.Floor(fps_points_av[fps_points_av.Count - 1]).ToString(), new System.Drawing.Font("Consolas", 10, FontStyle.Bold), Brushes.Black, new PointF(5, 5));


                }
                if (alive_points_av.Count > 60)
                {
                    //graphics3.FillRectangle(Brushes.White, new RectangleF(panelAlive.Width - 30-3, 0, 30,20));
                    //graphics3.DrawString(Math.Floor(alive_points_av[alive_points_av.Count - 1]).ToString() + "%", new System.Drawing.Font("Consolas", 10, FontStyle.Bold), Brushes.Black, new PointF(panelAlive.Width - 30, 5));
                    graphics3.DrawString("Процент живых: " + Math.Floor(alive_points_av[alive_points_av.Count - 1]).ToString() + "%", new System.Drawing.Font("Consolas", 10, FontStyle.Bold), Brushes.Black, new PointF(5, 5));

                }

                bg2.Render();
                bg3.Render();
            }

        }

        private float expRunningAverageSound(float sound)
        {
            soundfilVal += (sound - soundfilVal) * k3;
            return soundfilVal;
        }

        private float expRunningAverageAlive(int v)
        {
            alivefilVal += (v - alivefilVal) * k1;
            return alivefilVal;
        }
        int brush_radius = 5;
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (current_field != null && radioButton4.Checked)
            {
                graphics.Clear(this.BackColor);
                graphics.DrawRectangle(Pens.Black, 0, 0, cell_size * field_hor_count, cell_size * field_vert_count);
                if (checkBox1.Checked)
                {

                    for (int i = 0; i < field_vert_count; i++)
                    {
                        for (int j = 0; j < field_hor_count; j++)
                        {
                            if (current_field[i, j] == 0)
                            {
                                //graphics.DrawRectangle(Pens.White, i * cell_size, j * cell_size, cell_size, cell_size);
                            }
                            if (current_field[i, j] == 2)
                            {
                                //if(cell_size > 2)
                                graphics.FillRectangle(Brushes.Black, i * cell_size, j * cell_size, cell_size, cell_size);
                                //else
                                //{
                                //    graphics.DrawLine(Pens.Black, i * cell_size, j * cell_size, i * cell_size+1, j * cell_size+1);
                                //}
                            }
                            if (current_field[i, j] == 1)
                            {
                                //if(cell_size > 2)
                                graphics.FillRectangle(Brushes.Black, i * cell_size, j * cell_size, cell_size, cell_size);
                                //else
                                //{
                                //    graphics.DrawLine(Pens.Black, i * cell_size, j * cell_size, i * cell_size+1, j * cell_size+1);
                                //}
                            }
                        }
                    }
                }

                //graphics.DrawString(Math.Round(fps, 2).ToString(), new System.Drawing.Font("Arial", 15), Brushes.Red, new System.Drawing.Point(10, 10));
                if (radioButton1.Checked)
                    update_field_seq();
                else update_field_parallel2();



                //update_field_parallel();
                //update_field_parallel2();
                bg.Render();
                OnMapUpdated();
            }
        }
        DateTime _lastCheckTime = DateTime.Now;
        long _frameCount = 0;

        // called whenever a map is updated
        void OnMapUpdated()
        {
            Interlocked.Increment(ref _frameCount);
        }

        // called every once in a while
        double GetFps()
        {
            double secondsElapsed = (DateTime.Now - _lastCheckTime).TotalSeconds;
            long count = Interlocked.Exchange(ref _frameCount, 0);
            double fps = count / secondsElapsed;
            _lastCheckTime = DateTime.Now;
            return (int)fps;
        }
        private void update_field_parallel2()
        {
            if (UpdateField)
            {
                Parallel.For(0, field_vert_count, i =>
               {
                   {
                       for (int j = 0; j < field_hor_count; j++)
                       {
                           int alive_nb = get_alive_neighbors(i, j, prev_field);
                           if (alive_nb < 2 || alive_nb > 3)
                           {
                               current_field[i, j] = 0;
                           }
                           if (alive_nb == 3)
                               current_field[i, j] = 1;
                           else
                           {
                               if (alive_nb == 2)
                               {
                                   if (prev_field[i, j] != 0)
                                       current_field[i, j] = 2;
                                   else
                                       current_field[i, j] = 0;
                               }
                           }
                       }
                   }
               });
                Parallel.For(0, field_vert_count, i =>
                {
                    for (int j = 0; j < field_hor_count; j++)
                    {
                        prev_field[i, j] = current_field[i, j];
                    }
                });
            }

            else
            {
                Parallel.For(0, field_vert_count, i =>
                {
                    for (int j = 0; j < field_hor_count; j++)
                    {
                        current_field[i, j] = prev_field[i, j];
                    }
                });
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateField = true;
            FillRandom(); avFPS.Clear();
            if (_window != null)
            {
                _window.X = this.PointToScreen(panel1.Location).X;
                _window.Y = this.PointToScreen(panel1.Location).Y;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateField = !UpdateField;
            avFPS.Clear();

            if (radioButton3.Checked)
            {
                if (!_window.IsRunning)
                {
                    _window.Create();
                    _window.IsRunning = true;
                    _window.IsPaused = true;
                }
                if (!_window.IsPaused)
                    _window.Pause();
                else
                    _window.Unpause();
            }
            if (_window != null)
            {
                _window.X = this.PointToScreen(panel1.Location).X;
                _window.Y = this.PointToScreen(panel1.Location).Y;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < field_vert_count; i++)
            {
                for (int j = 0; j < field_hor_count; j++)
                {
                    current_field[i, j] = 0;
                    prev_field[i, j] = 0;

                }
            }
            UpdateField = false; avFPS.Clear();
            if (_window != null)
            {
                _window.X = this.PointToScreen(panel1.Location).X;
                _window.Y = this.PointToScreen(panel1.Location).Y;
            }
        }
        bool manual_input = false;
        MouseButtons mouse_button = MouseButtons.None;
        PointF mouse_position = new PointF(-1, -1);
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            //if (!UpdateField)
            {
                manual_input = true;
                mouse_button = e.Button;
                mouse_position = e.Location;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            manual_input = false;
            mouse_button = MouseButtons.None;
            mouse_position = new PointF(-1, -1);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            mouse_button = e.Button;
            mouse_position = e.Location;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            field_vert_count = trackBar1.Value;
            field_hor_count = trackBar1.Value;

            current_field = new int[field_vert_count, field_hor_count];
            prev_field = new int[field_vert_count, field_hor_count];
            for (int i = 0; i < field_vert_count; i++)
            {
                for (int j = 0; j < field_hor_count; j++)
                {
                    current_field[i, j] = 0;
                    prev_field[i, j] = current_field[i, j];
                }
            }
            cell_size = Math.Min((float)panel1.Height / field_vert_count, (float)panel1.Width / field_hor_count) * 0.98f;
            label2.Text = "Размер поля: " + trackBar1.Value;
            FillRandom(); avFPS.Clear();
            //this.Height = (int)(field_vert_count * cell_size + 6);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            //field_hor_count = trackBar2.Value;
            //cell_size = Math.Min(panel1.Height / field_vert_count, panel1.Width / field_hor_count)-2;
            //current_field = new int[field_vert_count, field_hor_count];
            //prev_field = new int[field_vert_count, field_hor_count];
            //for (int i = 0; i < field_vert_count; i++)
            //{
            //    for (int j = 0; j < field_hor_count; j++)
            //    {
            //        current_field[i, j] = 0;
            //        prev_field[i, j] = current_field[i, j];
            //    }
            //}
        }
        int delay = 0;
        private void trackBar2_Scroll_1(object sender, EventArgs e)
        {
            timer1.Interval = 100 / trackBar2.Value;
            delay = 100 / trackBar2.Value;
            if (delay < 2) delay = 0;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            avFPS.Clear();
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            avFPS.Clear();
        }

        private void trackBar1_MouseEnter(object sender, EventArgs e)
        {
            UpdateField = false;
        }

        private void trackBar1_MouseLeave(object sender, EventArgs e)
        {
            UpdateField = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _window.Dispose();
        }

        void Form1_Validated(object sender, EventArgs e)
        {

        }

        private void Form1_Move(object sender, EventArgs e)
        {
            if (_window != null)
            {
                _window.X = this.PointToScreen(panel1.Location).X;
                _window.Y = this.PointToScreen(panel1.Location).Y;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                this.Width = 1190;
                _window.Dispose();
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                //this.Width = 290;
                if (_window.IsInitialized == false)
                {
                    var gfx = new GameOverlay.Drawing.Graphics()
                    {
                        MeasureFPS = true,
                        PerPrimitiveAntiAliasing = true,
                        TextAntiAliasing = true
                    };
                    _window = new GraphicsWindow(this.PointToScreen(panel1.Location).X, this.PointToScreen(panel1.Location).Y, panel1.Width, panel1.Height, gfx)
                    {
                        FPS = 75,
                        IsTopmost = true,
                        IsVisible = true
                    };

                    _window.DestroyGraphics += _window_DestroyGraphics;
                    _window.DrawGraphics += _window_DrawGraphics;
                    _window.SetupGraphics += _window_SetupGraphics;
                }
                if (!_window.IsRunning)
                {
                    _window.Create();
                    _window.IsRunning = true;
                    _window.IsPaused = true;
                }
                if (!_window.IsPaused)
                    _window.Pause();
                else
                    _window.Unpause();
                if (_window != null)
                {
                    _window.X = this.PointToScreen(panel1.Location).X;
                    _window.Y = this.PointToScreen(panel1.Location).Y;
                }
            }
        }

        private void Form1_Leave(object sender, EventArgs e)
        {
            _window.IsTopmost = false;
        }

        private void Form1_Enter(object sender, EventArgs e)
        {
            _window.IsTopmost = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            brush_radius = trackBar3.Value;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            randomize_points = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                GlobalMixer.Play();
            else
                GlobalMixer.Stop();
        }
    }
    class BeepSample
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Beep(uint dwFreq, uint dwDuration);

        
    }
}
