using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL.Dom;

namespace CanvasAnimation
{
    public class Program
    {
        public static void Main()
        {
            var w = 800;
            var h = 600;

            // create a new Canvas
            var canvas = new Canvas();
            canvas.Width = w;
            canvas.Height = h;

            // add canvas to the div with id = "target" (defined in MainPage.html)
            var target = Element.GetById("target");
            target.AppendChild(canvas);

            // get the 2D context for the canvas, which contains all the draw methods
            var context = canvas.Get2DContext();

            var angleOffset = 0d;

            var timer = new IntervalDispatcher(TimeSpan.FromMilliseconds(30));
            timer.Tick += (s, e) =>
            {
                // draw background rect
                context.FillStyle = new RgbStyle(0xcc, 0x66, 0x99);
                context.FillRect(0, 0, w, h);
                context.LineWidth = 2;

                // draw flowers
                context.StrokeStyle = new RgbStyle(0x33, 0x33, 0x33);

                context.FillStyle = new RgbStyle(0xff, 0xff, 0);
                DrawFlower(context, Math.Min(w, h) * 0.4, 25d, 0.5 * w, 0.5 * h, angleOffset);

                context.FillStyle = new RgbStyle(0xff, 0x0, 0);
                DrawFlower(context, Math.Min(w, h) * 0.1, 12d, 0.5 * w, 0.5 * h, angleOffset);

                angleOffset += 0.02;
            };

            timer.Start();
            bool started = true;

            var startButton = new Element("button") { TextContent = "Start" };
            var stopButton = new Element("button") { TextContent = "Stop" };

            startButton.Click += (s, e) => {
                timer.Start();
                started = true;
                startButton.Enabled = !started;
                stopButton.Enabled = started;
            };

            stopButton.Click += (s, e) => {
                timer.Stop();
                started = false;
                startButton.Enabled = !started;
                stopButton.Enabled = started;
            };

            startButton.Enabled = !started;
            stopButton.Enabled = started;

            target.AppendChild(startButton);
            target.AppendChild(stopButton);
        }

        private static void DrawFlower(Context2d context, double leafLength, double leafs, double x, double y, double angleOffset)
        {
            context.BeginPath();

            var leafAngle = (2d * Math.PI / leafs);
            for (var i = 0d; i < leafs; i++)
            {
                var startAngle = i * leafAngle + angleOffset;
                var circlePointAngle = (i + 0.5) * leafAngle + angleOffset;
                var arcStartAngle = circlePointAngle - 0.5 * Math.PI;
                var acrEndAngle = arcStartAngle + Math.PI;

                context.MoveTo(x, y);
                context.LineTo(
                    x + leafLength * Math.Cos(startAngle),
                    y + leafLength * Math.Sin(startAngle));
                context.Arc(
                    x + leafLength * Math.Cos(circlePointAngle),
                    y + leafLength * Math.Sin(circlePointAngle),
                    0.5 * leafLength * leafAngle,
                    arcStartAngle,
                    acrEndAngle);

                context.ClosePath();
            }
            context.Fill();
            context.Stroke();
        }
    }
}
