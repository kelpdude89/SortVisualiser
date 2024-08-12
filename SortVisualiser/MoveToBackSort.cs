using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SortVisualiser
{
    class MoveToBackSort : ISortEngine
    {
        private int[] TheArray;
        private Graphics g;
        private int MaxVal;

        Brush WhiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
        Brush BlackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        Brush GreenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);


        private int CurrentListPointer = 0;

        public MoveToBackSort(int[] TheArray_In, Graphics g_In, int MaxVal_In)
        {
            TheArray = TheArray_In;
            g = g_In;
            MaxVal = MaxVal_In;
        }


        public void NextStep()
        {
            if (CurrentListPointer >= TheArray.Count() - 1) { CurrentListPointer = 0; }
            
            if (TheArray[CurrentListPointer] > TheArray[CurrentListPointer + 1])
            {
                Rotate(CurrentListPointer);
            }

            CurrentListPointer++;
        }

        private void Rotate(int currentListPointer)
        {
            int temp = TheArray[CurrentListPointer];
            int EndPoint = TheArray.Count() - 1;

            for (int i = CurrentListPointer; i < EndPoint; i++)
            {
                TheArray[i] = TheArray[i + 1];
                DrawBar(i, TheArray[i]);
            }

            TheArray[EndPoint] = temp;

            DrawBar(EndPoint, TheArray[EndPoint]);
        }

        private void DrawBar(int position, int height)
        {
            g.FillRectangle(BlackBrush, position, 0, 1, MaxVal);
            g.FillRectangle(WhiteBrush, position, MaxVal - TheArray[position], 1, MaxVal);
        }

        public bool IsSorted()
        {
            for (int i = 0; i < TheArray.Count() - 1; i++)
            {
                if (TheArray[i] > TheArray[i + 1]) { return false; }
            }

            return true;
        }

        public void ReDraw()
        {
            for (int i = 0; i < (TheArray.Count() - 1); i++)
            {
                g.FillRectangle(WhiteBrush, i, MaxVal - TheArray[i], 1, MaxVal);
            }
        }

        public void Colorize()
        {
            for (int i = 0; i < TheArray.Count(); i++)
            {
                g.FillRectangle(GreenBrush, i, MaxVal - TheArray[i], 1, MaxVal);
                System.Threading.Thread.Sleep(1);
            }
        }
    }
}
