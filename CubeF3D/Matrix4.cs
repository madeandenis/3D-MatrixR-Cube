using System;

namespace CubeF3D
{
    public class Matrix4
    {
        public float M11, M12, M13, M14;
        public float M21, M22, M23, M24;
        public float M31, M32, M33, M34;
        public float M41, M42, M43, M44;

        public Matrix4(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44)
        {
            M11 = m11; M12 = m12; M13 = m13; M14 = m14;
            M21 = m21; M22 = m22; M23 = m23; M24 = m24;
            M31 = m31; M32 = m32; M33 = m33; M34 = m34;
            M41 = m41; M42 = m42; M43 = m43; M44 = m44;
        }

        public static Matrix4 Multiply(Matrix4 a, Matrix4 b)
        {
            return new Matrix4(
                a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31 + a.M14 * b.M41,
                a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32 + a.M14 * b.M42,
                a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33 + a.M14 * b.M43,
                a.M11 * b.M14 + a.M12 * b.M24 + a.M13 * b.M34 + a.M14 * b.M44,

                a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31 + a.M24 * b.M41,
                a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32 + a.M24 * b.M42,
                a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33 + a.M24 * b.M43,
                a.M21 * b.M14 + a.M22 * b.M24 + a.M23 * b.M34 + a.M24 * b.M44,

                a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31 + a.M34 * b.M41,
                a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32 + a.M34 * b.M42,
                a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33 + a.M34 * b.M43,
                a.M31 * b.M14 + a.M32 * b.M24 + a.M33 * b.M34 + a.M34 * b.M44,

                a.M41 * b.M11 + a.M42 * b.M21 + a.M43 * b.M31 + a.M44 * b.M41,
                a.M41 * b.M12 + a.M42 * b.M22 + a.M43 * b.M32 + a.M44 * b.M42,
                a.M41 * b.M13 + a.M42 * b.M23 + a.M43 * b.M33 + a.M44 * b.M43,
                a.M41 * b.M14 + a.M42 * b.M24 + a.M43 * b.M34 + a.M44 * b.M44
            );
        }

        public static Point ApplyToPoint(Matrix4 matrix, Point point)
        {
            float x = matrix.M11 * point.X + matrix.M12 * point.Y + matrix.M13 * point.Z + matrix.M14;
            float y = matrix.M21 * point.X + matrix.M22 * point.Y + matrix.M23 * point.Z + matrix.M24;
            float z = matrix.M31 * point.X + matrix.M32 * point.Y + matrix.M33 * point.Z + matrix.M34;

            return new Point(x, y, z);
        }

        public static Matrix4 IdentityMatrix => new Matrix4(
              1, 0, 0, 0,
              0, 1, 0, 0,
              0, 0, 1, 0,
              0, 0, 0, 1
        );


        public static Matrix4 ScalingMatrix(float sx, float sy, float sz)
        {
            return new Matrix4(
                sx, 0, 0, 0,
                0, sy, 0, 0,
                0, 0, sz, 0,
                0, 0, 0, 1
            );
        }

        public static Matrix4 RotationMatrix_X(float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            return new Matrix4(
                1, 0, 0, 0,
                0, cos, -sin, 0,
                0, sin, cos, 0,
                0, 0, 0, 1
            );
        }
        public static Matrix4 RotationMatrix_Y(float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            return new Matrix4(
                cos, 0, sin, 0,
                0, 1, 0, 0,
                -sin, 0, cos, 0,
                0, 0, 0, 1
            );
        }
        public static Matrix4 RotationMatrix_Z(float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            return new Matrix4(
                cos, -sin, 0, 0,
                sin, cos, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }

    }
}
