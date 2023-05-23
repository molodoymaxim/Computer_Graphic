using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KG2
{
    class Parall_
    {
        public double X, Y, Z;
        public double W;
        public Parall_ Vcam;

        public Parall_(double aX, double aY, double aZ, double aW = 1)
        {
            X = aX;
            Y = aY;
            Z = aZ;
            W = aW;
            Vcam = null;
        }

        public Parall_(String str)
        {
            string[] coords = str.Split(' ');
            X = Double.Parse(coords[0]);
            Y = Double.Parse(coords[1]);
            Z = Double.Parse(coords[2]);
            W = 1;
            Vcam = null;
        }

        public Parall_(Point pnt)
        {
            X = pnt.X;
            Y = pnt.Y;
            Z = 0;
            W = 1;
            Vcam = null;
        }

        public double getX()
        {
            return X / W;
        }

        public double getY()
        {
            return Y / W;
        }

        public double getZ()
        {
            return Z / W;
        }

        public double getW()
        {
            return W;
        }

        public double length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        public void Normalize()
        {
            double len = length;
            X = X / len;
            Y = Y / len;
            Z = Z / len;
        }

        // Векторное произведение на вектор src
        public Parall_ getVectorMult(Parall_ src)
        {
            double ax, ay, az;
            ax = getY() * src.getZ() - getZ() * src.getY();
            ay = getZ() * src.getX() - getX() * src.getZ();
            az = getX() * src.getY() - getY() * src.getX();
            return new Parall_(ax, ay, az);
        }

        public Parall_ applyMatrix(Matrix matr)
        {
            Parall_ result = new Parall_(0,0,0,0);
            double[] input = { X, Y, Z, W };
            for (int i = 0; i < 4; i++)
            {
                result.X += input[i] * matr.fields[i, 0];
                result.Y += input[i] * matr.fields[i, 1];
                result.Z += input[i] * matr.fields[i, 2];
                result.W += input[i] * matr.fields[i, 3];
            }
            return result;
        }

        // Находит угол между двумя веторами
        public double findAngle(Parall_ src)
        {
            if (length == 0 || src.length == 0)
                return 0;
            double cos=(getX()*src.getX()+getY()*src.getY()+getZ()*src.getZ())/(length*src.length);
            return Math.Acos(cos);
        }

        // Считаем координаты точки относительно камеры
        public void rotateForCam(AngleRotate cam)
        {
            Vcam = applyMatrix(cam.toRotate);
        }

        public Parall_ minus(Parall_ src)
        {
            return new Parall_(getX() - src.getX(), getY() - src.getY(), getZ() - src.getZ());
        }

        public Parall_ plus(Parall_ src)
        {
            return new Parall_(getX() + src.getX(), getY() + src.getY(), getZ() + src.getZ());
        }

        public Parall_ scale(double mnozh)
        {
            return new Parall_(getX() * mnozh, getY() * mnozh, getZ() * mnozh);
        }
    }
}
