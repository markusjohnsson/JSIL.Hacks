using JSIL.Meta;
using System;

namespace JSIL.Dom
{
    public class Canvas: Element
    {
        public Canvas(): base("canvas")
        {
        }
    }

    public static class Context2dEx
    {
        public static Context2d Get2DContext(this Canvas canvas)
        {
            return Context2d.GetContext(canvas);
        }
    }

    public class Context2d
    {
        private object _context;

        internal Context2d(object context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this._context = context;
        }

        internal static Context2d GetContext(Canvas canvas)
        {
            return new Context2d(Verbatim.Expression("canvas._element.getContext('2d')"));
        }

        public Style FillStyle
        {
            set
            {
                var str = value.ToString();
                Verbatim.Expression("this._context.fillStyle = str");
            }
        }

        public Style StrokeStyle
        {
            set
            {
                var str = value.ToString();
                Verbatim.Expression("this._context.strokeStyle = str");
            }
        }

        public double LineWidth
        {
            set
            {
                Verbatim.Expression("this._context.lineWidth = value");
            }
        }

        #region Simple shapes
        /// <summary>
        /// Paints the given rectangle onto the canvas, using the current fill style.
        /// </summary>
        [JSReplacement("$this._context.fillRect($x, $y, $width, $height)")]
        public void FillRect(double x, double y, double width, double height)
        {
        }

        /// <summary>
        /// Paints the box that outlines the given rectangle onto the canvas, using the current stroke style.
        /// </summary>
        [JSReplacement("$this._context.strokeRect($x,$y,$w,$h)")]
        public void StrokeRect(double x, double y, double width, double height)
        {
        }

        /// <summary>
        /// Clears all pixels on the canvas in the given rectangle to transparent black.
        /// </summary>
        [JSReplacement("$this._context.clearRect($x,$y,$w,$h)")]
        public void ClearRect(double x, double y, double width, double height)
        {
        }
        #endregion

        #region Context state
        /// <summary>
        /// Pushes the current state onto the stack.
        /// </summary>
        [JSReplacement("$this._context.save()")]
        public void Save()
        {
        }

        /// <summary>
        /// Pops the top state on the stack, restoring the context to that state.
        /// </summary>
        [JSReplacement("$this._context.restore()")]
        public void Restore()
        {
        }
        #endregion

        #region Complex shapes (paths)
        /// <summary>
        /// Resets the current path.
        /// </summary>
        [JSReplacement("$this._context.beginPath()")]
        public void BeginPath()
        {
        }

        /// <summary>
        /// Marks the current subpath as closed, and starts a new subpath with a point the same as the start and end of the newly closed subpath.
        /// </summary>
        [JSReplacement("$this._context.closePath()")]
        public void ClosePath()
        {

        }

        /// <summary>
        /// Strokes the subpaths with the current stroke style.
        /// </summary>
        [JSReplacement("$this._context.stroke()")]
        public void Stroke()
        {
        }

        /// <summary>
        /// Adds an arc with the given control points and radius to the current subpath, connected to the previous point by a straight line.
        /// Throws an INDEX_SIZE_ERR exception if the given radius is negative.
        /// </summary>
        [JSReplacement("$this._context.arcTo($x1, $y1, $x2, $y2, $radius)")]
        public void ArcTo(double x1, double y1, double x2, double y2, double radius)
        {

        }

        /// <summary>
        /// Adds points to the subpath such that the arc described by the circumference of the circle described by the arguments, starting at the given start angle and ending at the given end angle, going in the given direction (defaulting to clockwise), is added to the path, connected to the previous point by a straight line.
        /// Throws an INDEX_SIZE_ERR exception if the given radius is negative.
        /// </summary>
        [JSReplacement("$this._context.arc($x, $y, $radius, $startAngle, $endAngle, $antiClockwise)")]
        public void Arc(double x, double y, double radius, double startAngle, double endAngle, bool antiClockwise = false)
        {

        }

        /// <summary>
        /// Fills the subpaths with the current fill style.
        /// </summary>
        [JSReplacement("$this._context.fill()")]
        public void Fill()
        {
        }

        /// <summary>
        /// Creates a new subpath with the given point.
        /// </summary>
        [JSReplacement("$this._context.moveTo($x, $y)")]
        public void MoveTo(double x, double y)
        {
        }

        /// <summary>
        /// Adds the given point to the current subpath, connected to the previous one by a straight line.
        /// </summary>
        [JSReplacement("$this._context.lineTo($x, $y)")]
        public void LineTo(double x, double y)
        {
        }

        #endregion
    }
}
