using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SortVisualiser
{
    public partial class Form1 : Form
    {
        int[] TheArray;
        Graphics g;
        BackgroundWorker bgw = null;
        bool Paused = false;
        bool colorize = false;


        public Form1()
        {
            InitializeComponent();
            PopulateDropdown();
        }

        private void PopulateDropdown()
        {
            List<string> ClassList = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(ISortEngine).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x.Name).ToList();

            ClassList.Sort();

            foreach (string entry in ClassList)
            {
                comboBox1.Items.Add(entry);
            }

            comboBox1.SelectedIndex = 0;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.vocabulary.com/dictionary/help");
        }

        private void colorizedOnFinishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorizedOnFinishToolStripMenuItem.Checked) { colorize = true; }
            else { colorize = false; }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (TheArray == null) { buttonReset_Click(null, null); }

            bgw = new BackgroundWorker();
            bgw.WorkerSupportsCancellation = true;
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.RunWorkerAsync(argument: comboBox1.SelectedItem);
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (!Paused)
            {
                bgw.CancelAsync();
                Paused = true;
            }
            else
            {
                if (bgw.IsBusy) return;

                int NumEntries = panel1.Width;
                int MaxVal = panel1.Height;
                Paused = false;
                for (int i = 0; i < NumEntries; i++)
                {
                    g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Black), i, 0, 1, MaxVal);
                    g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), i, MaxVal - TheArray[i], 1, MaxVal);
                }

                bgw.RunWorkerAsync(argument: comboBox1.SelectedItem);
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            g = panel1.CreateGraphics();
            int NumEntries = panel1.Width;
            int MaxVal = panel1.Height;

            TheArray = new int[NumEntries];

            g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Black), 0, 0, NumEntries, MaxVal);

            Random random = new Random();

            for (int i = 0; i < NumEntries; i++)
            {
                TheArray[i] = random.Next(0, MaxVal);
            }

            for (int i = 0; i < NumEntries; i++)
            {
                g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), i, MaxVal - TheArray[i], 1, MaxVal);
            }
        }

        #region BackGroundStuff

        public void bgw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            string SortEngineName = (string)e.Argument;
            Type type = Type.GetType("SortVisualiser." + SortEngineName);
            var ctors = type.GetConstructors();

            try
            {
                ISortEngine se = (ISortEngine)ctors[0].Invoke(new object[] { TheArray, g, panel1.Height });

                while (!se.IsSorted() && !(bgw.CancellationPending))
                {
                    se.NextStep();
                }

                if (se.IsSorted() && colorize) { se.Colorize(); }
            }

            catch (Exception ex)
            {
                Console.WriteLine("An exception was just caught, yay!");
                Console.WriteLine("It was found in the bgw_DoWork method!");
                Console.WriteLine("here it is: ");
                Console.WriteLine(ex.Message);
            }
        }



        #endregion

        
    }
}
