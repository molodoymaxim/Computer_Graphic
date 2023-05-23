using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KG2
{
    class Scene
    {
        public Color brush = Color.RoyalBlue;
        public Bitmap pic;
        public int height, width;
        public AngleRotate cam;
        double del; 
        public List<Parall_> verts;
        public List<Triangle> polys;
        public Parall_ lightPoint;
        double[,] Zbuffer;

        public Scene(int aheight, int awidth)
        {
            height = aheight;
            width = awidth;
            pic = new Bitmap(width, height);
            verts = new List<Parall_>();
            polys = new List<Triangle>();
        }
        public void addShar(double height, double baseRadius, int sideCount)
        {
            double angleStep = 360.0 / sideCount;

            // Создание вершин основания пирамиды
            List<Parall_> baseVerts = new List<Parall_>();
            for (int i = 0; i < sideCount; i++)
            {
                double angleRad = i * angleStep * Math.PI / 180.0;
                double x = baseRadius * Math.Cos(angleRad);
                double y = baseRadius * Math.Sin(angleRad);
                baseVerts.Add(new Parall_(x, y, 0));
            }

            // Создание вершины верхушки пирамиды
            Parall_ topVert = new Parall_(0, 0, height);

            // Создание боковых граней пирамиды
            for (int i = 0; i < sideCount; i++)
            {
                int nextIndex = (i + 1) % sideCount;
                polys.Add(new Triangle(baseVerts[i], baseVerts[nextIndex], topVert, brush));
            }

            // Добавление вершин и основания пирамиды в общий список вершин
            verts.AddRange(baseVerts);
            verts.Add(topVert);
        }

        public void addAngleRotate(AngleRotate acam)
        {
            cam = acam;
            del = 1 / Math.Tan(cam.angle / 2);
        }

        public Point convertToScreenPoint(Parall_ v)
        {
            Point result = new Point();
            result.X = (int)Math.Round(v.Vcam.getX() / v.Vcam.getZ() * del * (double)width / 2 + (double)width / 2); //преобразовываем в экранные координаты
            result.Y = (int)Math.Round(v.Vcam.getY() / v.Vcam.getZ() * del * (double)width / 2 + (double)height / 2);
            return result;
        }

        public bool CheckPoint(Point pnt, List<Point> pnts)
        {
            Parall_ v0, v1, v2;
            v0 = new Parall_(pnt).minus(new Parall_(pnts[0]));
            v1 = new Parall_(pnts[1]).minus(new Parall_(pnts[0]));
            v2 = new Parall_(pnts[2]).minus(new Parall_(pnts[0]));
            if (Math.Abs(v2.findAngle(v1) - v2.findAngle(v0) - v1.findAngle(v0)) > 1e-3)
                return false;

            v0 = new Parall_(pnt).minus(new Parall_(pnts[1]));
            v1 = new Parall_(pnts[0]).minus(new Parall_(pnts[1]));
            v2 = new Parall_(pnts[2]).minus(new Parall_(pnts[1]));
            if (Math.Abs(v2.findAngle(v1) - v2.findAngle(v0) - v1.findAngle(v0)) > 1e-3)
                return false;

            return true;
        }

        // Находит точку на грани, которая смотрит в заданную точку на экране
        public Parall_ findVertex(Point pnt, Triangle poly)
        {
            Parall_ onScreen = new Parall_((pnt.X - (double)width / 2) / (del * (double)width / 2), (pnt.Y - (double)height / 2) / (del * (double)width / 2), 1);
            Parall_ norm = poly.normalCam;
            double A = norm.X,
                B = norm.Y,
                C = norm.Z,
                D = -(A * poly.v0.Vcam.getX() + B * poly.v0.Vcam.getY() + C * poly.v0.Vcam.getZ());
            double t = -D / (A * onScreen.getX() + B * onScreen.getY() + C * onScreen.getZ());
            return onScreen.scale(t);
        }

        public Color colorWithLight(Parall_ t, Triangle poly)
        {
            double R = 20, G = 20, B = 20;

            Parall_ norm = poly.normalCam;
            Parall_ ray = lightPoint.Vcam.minus(t); //от точки до источника освещения
            if (ray.findAngle(norm) < Math.PI / 2)
            {
                double cos = Math.Pow(Math.Cos(ray.findAngle(norm)), 0.5);
                R = cos * poly.color.R;
                G = cos * poly.color.G;
                B = cos * poly.color.B;
            }
            return Color.FromArgb(Math.Min((int)R, 255), Math.Min((int)G, 255), Math.Min((int)B, 255));
        }

        // рисует полигон на экране
        public void DrawPolygon(Triangle poly)
        {
            List<Point> pnts = new List<Point>();
            pnts.Add(convertToScreenPoint(poly.v0));
            pnts.Add(convertToScreenPoint(poly.v1));
            pnts.Add(convertToScreenPoint(poly.v2));

            int minX, minY, maxX, maxY;
            minX = maxX = pnts[0].X;
            minY = maxY = pnts[0].Y;
            foreach (Point pnt in pnts)
            {
                minX = Math.Min(minX, pnt.X);
                maxX = Math.Max(maxX, pnt.X);
                minY = Math.Min(minY, pnt.Y);
                maxY = Math.Max(maxY, pnt.Y);
            }
            minX = Math.Max(minX, 0);
            minY = Math.Max(minY, 0);
            maxX = Math.Min(maxX, width - 1);
            maxY = Math.Min(maxY, height - 1);
            for (int X = minX; X <= maxX; X++)
                for (int Y = minY; Y <= maxY; Y++)
                {
                    Point curp = new Point(X, Y);
                    if (CheckPoint(curp, pnts))
                    {
                        Parall_ curV = findVertex(curp, poly);
                        if (curV.getZ() < Zbuffer[X, Y])
                        {
                            Zbuffer[X, Y] = curV.getZ();
                            pic.SetPixel(X, Y, colorWithLight(curV, poly));
                        }
                    }
                }
        }

        public void Render()
        {
            if (cam != null)
            {
                //вычисление проекционных координат
                foreach (Parall_ vr in verts)
                    vr.rotateForCam(cam);
                lightPoint.rotateForCam(cam);
                Graphics gr = Graphics.FromImage(pic);
                gr.Clear(Color.White);
                Zbuffer = new double[width, height];
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        Zbuffer[i, j] = 10000;  //устанавливаем линию горизонта

                foreach (Triangle pl in polys)
                    if (pl.v0.minus(cam.pos).findAngle(pl.normal) > Math.PI / 2)  //если плоскость смотрит в сторону камеры
                        if (pl.v0.Vcam.getZ() > 0.2 && pl.v1.Vcam.getZ() > 0.2 && pl.v2.Vcam.getZ() > 0.2) //и если она не лежит за ней
                            DrawPolygon(pl);
            }
        }
    }
}
