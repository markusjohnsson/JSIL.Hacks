using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloWebGL
{
    public class Matrix4
    {
        public double M11, M12, M13, M14;
        public double M21, M22, M23, M24;
        public double M31, M32, M33, M34;
        public double M41, M42, M43, M44;

        public Matrix4()
        {
            M11 = M22 = M33 = M44 = 1.0d;
        }

        public static Matrix4 Translate(double x, double y, double z)
        {
            return new Matrix4()
            {
                M41 = x,
                M42 = y,
                M43 = z
            };
        }

        public Matrix4 Multiply(Matrix4 matrix)
        {
            return new Matrix4()
            {
                M11 = M11 * matrix.M11 + M12 * matrix.M21 + M13 * matrix.M31 + M14 * matrix.M41,
                M12 = M11 * matrix.M12 + M12 * matrix.M22 + M13 * matrix.M32 + M14 * matrix.M42,
                M13 = M11 * matrix.M13 + M12 * matrix.M23 + M13 * matrix.M33 + M14 * matrix.M43,
                M14 = M11 * matrix.M14 + M12 * matrix.M24 + M13 * matrix.M34 + M14 * matrix.M44,

                M21 = M21 * matrix.M11 + M22 * matrix.M21 + M23 * matrix.M31 + M24 * matrix.M41,
                M22 = M21 * matrix.M12 + M22 * matrix.M22 + M23 * matrix.M32 + M24 * matrix.M42,
                M23 = M21 * matrix.M13 + M22 * matrix.M23 + M23 * matrix.M33 + M24 * matrix.M43,
                M24 = M21 * matrix.M14 + M22 * matrix.M24 + M23 * matrix.M34 + M24 * matrix.M44,

                M31 = M31 * matrix.M11 + M32 * matrix.M21 + M33 * matrix.M31 + M34 * matrix.M41,
                M32 = M31 * matrix.M12 + M32 * matrix.M22 + M33 * matrix.M32 + M34 * matrix.M42,
                M33 = M31 * matrix.M13 + M32 * matrix.M23 + M33 * matrix.M33 + M34 * matrix.M43,
                M34 = M31 * matrix.M14 + M32 * matrix.M24 + M33 * matrix.M34 + M34 * matrix.M44,

                M41 = M41 * matrix.M11 + M42 * matrix.M21 + M43 * matrix.M31 + M44 * matrix.M41,
                M42 = M41 * matrix.M12 + M42 * matrix.M22 + M43 * matrix.M32 + M44 * matrix.M42,
                M43 = M41 * matrix.M13 + M42 * matrix.M23 + M43 * matrix.M33 + M44 * matrix.M43,
                M44 = M41 * matrix.M14 + M42 * matrix.M24 + M43 * matrix.M34 + M44 * matrix.M44
            };
        }


        public static Matrix4 Perspective(double fovy, double aspect, double near, double far)
        {
            var top = near * Math.Tan(fovy * Math.PI / 360.0);
            var right = top * aspect;
            return Matrix4.Frustum(-right, right, -top, top, near, far);
        }

        private static Matrix4 Frustum(
            double left, double right, double bottom, double top, double near, double far)
        {
            var mtx = new Matrix4();
            var rl = (right - left);
            var tb = (top - bottom);
            var fn = (far - near);
            
            mtx.M11 = (near * 2) / rl;
            mtx.M22 = (near * 2) / tb;
            
            mtx.M31 = (right + left) / rl;
            mtx.M32 = (top + bottom) / tb;
            mtx.M33 = -(far + near) / fn;
            mtx.M34 = -1;

            mtx.M43 = -(far * near * 2) / fn;
            mtx.M44 = 0;
            
            return mtx;
        }

        public double[] ToArray()
        {
            return new double[] { 
                M11, M12, M13, M14,
                M21, M22, M23, M24,
                M31, M32, M33, M34,
                M41, M42, M43, M44
            };
        }
    }
}
