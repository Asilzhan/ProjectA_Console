using System;

namespace ProjectA_ConsoleCore.Models
{
    public class ConsoleRewriteEventArgs
    {
        public ConsoleColor Color { get; set; }
        public string Text { get; set; }
        public int CurrentX { get; set; }
        public int CurrentY { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public ConsoleRewriteEventArgs(string text, int y, int x, int currentX, int currentY, ConsoleColor color = ConsoleColor.White)
        {
            Color = color;
            Text = text;
            X = x;
            Y = y;
            CurrentX = currentX;
            CurrentY = currentY;
        }
    }
}