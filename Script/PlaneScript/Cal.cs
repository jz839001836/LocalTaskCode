using System.Collections;
using System.Collections.Generic;
using System;

///<summary>
///
///</summary>
public class Cal
{
    /// <summary>
    /// 求解全弹加速度
    /// </summary>
    /// <param name="fl">推力--时间</param>
    /// <param name="dc">空气动力参数</param>
    /// <param name="t">时间</param>
    /// <param name="a">弹体参数</param>
    /// <param name="y"></param>
    /// <returns></returns>
    private double AccSpeed(double[,] fl, double[,] dc, double t, double[] a, double[] y)
    {
        double fx2, s, wp, wxc, vr, jias, w1, w2, w3;
        double[] aa = new double[2];
        double[] ca = new double[8];

        s = 0.7854 * a[0] * a[0];
        wp = a[10] * Math.Cos(y[1]);
        wxc = a[10] * Math.Sin(y[1]);
        vr = (y[0] - wp) * (y[0] - wp) + wxc * wxc + a[11] * a[11];
        vr = Math.Sqrt(vr);
        GJ(y, aa);
        Cah(a, t, y[0], y[10], fl, dc, ca);
        w2 = 0.5 * Density(y[10]) * s * ca[2] * vr;
        w3 = 0.5 * Density(y[10]) * s * ca[3];
        fx2 = ca[0] * Math.Cos(aa[0]) * Math.Cos(aa[1]);
        fx2 = fx2 - w2 * (y[0] - wp);
        w1 = wxc * wxc + a[11] * a[11] - wxc * (y[0] - wp) * aa[0];
        w1 = w1 + a[11] * (y[0] - wp) * aa[1];
        fx2 = fx2 + w3 * w1;
        fx2 = fx2 - (a[2] + ca[1]) * 9.81 * Math.Sin(y[1]) * Math.Cos(y[2]);
        jias = fx2 / (ca[1] + a[2]);
        return jias;
    }
    /// <summary>
    /// 炮口参数计算函数
    /// </summary>
    /// <param name="fl">推力时间参数</param>
    /// <param name="dc">空气动力参数</param>
    /// <param name="a">弹体参数</param>
    /// <param name="y">计算参数</param>
    /// <param name="y1">存放返回值</param>
    private void PK(double[,] fl, double[,] dc, double[] a, double[] y, double[] y1)
    {
        int i = 0, j = 0;
        double[] d, g, ca;
        d = new double[2];
        g = new double[2];
        ca = new double[8];
        double[,] e = new double[3,2];
        double w1, ab, tt, t, h;
        y1[0] = y[0];
        y1[1] = 0.0;
        w1 = 0.0;
        h = 0.0001;
        t = 0.0;

        //a[25] : 发射架长
        while(y1[1] < a[25])
        {
            for (i = 0; i < 2; i++)
                g[i] = y1[i];
            tt = t;
            for (i = 0; i < 3; i++)
            {
                Cah(a, tt, g[0], w1, fl, dc, ca);

                d[0] = ca[0] - 0.5 * Density(w1) * 0.7854 * a[0] * a[0] * ca[2] * g[0] * g[0];
                d[0] = d[0] / (a[2] + ca[1]) - 9.81 * Math.Sin(y[1]);
                d[1] = g[0];

                for (j = 0; j < 2; j++)
                {
                    tt = t + h / 2.0;
                    e[i, j] = d[j];
                    g[j] = y1[j] + e[i, j] * h / 2.0;
                }
            }
            for (i = 0; i < 2; i++)
            {
                g[i] = y1[i] + e[2, j] * h;
            }
            tt = t + h;
            Cah(a, tt, g[0], w1, fl, dc, ca);
            d[0] = ca[0] - 0.5 * Density(w1) * 0.7854 * a[0] * ca[2] * g[0] * g[0];
            d[0] = d[0] / (a[2] + ca[1]) - 9.81 * Math.Sin(y[1]);
            d[1] = g[0];

            for (i = 0; i < 2; i++)
                e[3, i] = d[i];
            for (i = 0; i < 2; i++)
            {
                ab = e[0, i] + 2.0 * e[1, i] + 2.0 * e[2, i] + e[3, i];
                y1[i] = y1[i] + h * ab / 6.0;
            }
            t = t + h;
        }

        y[0] = y1[0];
        y[8] = a[24];
        y[3] = 2.0 * Math.Tan(a[24]) * y[0] / a[0];
        y[9] = a[25] * Math.Cos(y[1]);
        y[10] = a[25] * Math.Sin(y[1]);
        y1[1] = t;
        Cah(a, y1[1], y1[0], w1, fl, dc, ca);
        y1[3] = 0.5 * Density(w1) * 0.7854 * a[0] * a[0] * ca[2] * y1[0] * y1[0];
        y1[3] = (ca[0] - y1[3]) / (ca[1] + a[2]);
    }
    /// <summary>
    /// 根据高度计算空气密度
    /// </summary>
    /// <param name="y">高度</param>
    /// <returns></returns>
    private double Density(double y)
    {
        double p;
        if (y <= 9300.0 && y >= 0)
        {
            p = 1.0 - 2.1904e-5 * y;
            p = Math.Pow(p, 5.39897);
            p = p * 348.41 / (288.9 - 6.328e-3 * y);
        }
        else if ((y > 9300.0) && (y <= 12000.0))
        {
            p = 7.275e-5 * y - 0.873;
            p = -2.1353 * (0.7177 + Math.Atan(p));
            p = 348.41 * Math.Exp(p);
            p = p / (230.0 - 6.328e-3 * (y - 9300.0) + 1.172e-6 * (y - 9300.0) * (y - 9300.0));
        }
        else
        {
            p = -1.5424e-4 * y;
            p = 1.573 * Math.Exp(p);
        }
        return p;
    }
    /// <summary>
    /// 根据高度计算当地声速
    /// </summary>
    /// <param name="y">高度</param>
    /// <returns></returns>
    private double SoundVelocity(double y)
    {
        double c;
        if (y <= 9300.0)
        {
            c = 402.732 * (288.9 - 6.328e-3 * y);
            c = Math.Sqrt(c);
        }
        else if ((y > 9300.0) && (y <= 12000.0))
        {
            c = 402.372 * (230.0 - 6.328e-3 * (y - 9300.0) + 1.172e-6 * (y - 9300.0) * (y - 9300.0));
            c = Math.Sqrt(c);
        }
        else
            c = 289.54;
        return c;
    }
    /// <summary>
    /// 龙格库塔函数
    /// </summary>
    /// <param name="fl">推力-时间参数</param>
    /// <param name="dc">空气动力参数</param>
    /// <param name="a">弹体系数</param>
    /// <param name="t">时间</param>
    /// <param name="h">计算步长</param>
    /// <param name="y"></param>
    /// <param name="fl2"></param>
    private void RKT(double[,] fl, double[,] dc, double[] a, double t, double h, double[] y, double[] fl2)
    {
        int i, j;
        double[] d, g;
        d = new double[12];
        g = new double[12];
        double[,] e = new double[4, 12];
        double ab, tt;
        for (i = 0; i < 12; i++)
            g[i] = y[i];
        tt = t;
        for (i = 0; i < 3; i++)
        {
            F(a, tt, fl, dc, g, d, fl2);
            for (j = 0; j < 12; j++)
            {
                tt = t + h / 2.0;
                e[i, j] = d[j];
                g[j] = y[j] + e[i, j] * h / 2.0;
            }
        }
        for (i = 0; i < 12; i++)
            g[i] = y[i] + e[2, i] * h;
        tt = t + h;
        F(a, tt, fl, dc, g, d, fl2);
        for (i = 0; i < 12; i++)
            e[3, i] = d[i];
        for (i = 0; i < 12; i++)
        {
            ab = e[0, i] + 2.0 * e[1, i] + 2.0 * e[2, i] + e[3, i];
            y[i] = y[i] + h * ab / 6.0;
        }
    }
    /// <summary>
    /// 插值函数
    /// </summary>
    /// <param name="a">弹体参数</param>
    /// <param name="t">时间</param>
    /// <param name="v"></param>
    /// <param name="y">高度</param>
    /// <param name="fl">推力时间参数</param>
    /// <param name="dc">空气动力参数</param>
    /// <param name="ca">存放返回数据</param>
    private void Cah(double[] a, double t, double v, double y, double[,] fl, double[,] dc, double[] ca)
    {
        int i, j, ij;
        double w1;
        //a[26]发动机工作时间
        if (t < a[26])
        {
            i = 1;
            if (t > fl[0,1])
            {
                do
                {
                    i = i + 1;
                } while (t > fl[0, i]);
            }
            //a[21]:推进剂质量
            ca[1] = a[21] - a[21] * t / a[26];
            /* fl[0, i] : 时间
             * fl[1, i] : 推力
             */
            ca[0] = (fl[1, i] - fl[1, i - 1]) / (fl[0, i] - fl[0, i - 1]);
            ca[0] = fl[1, i - 1] + ca[0] * (t - fl[0, i - 1]);
            if (ca[1] < 0.0)
                ca[1] = 0.0;
        }
        else
        {
            ca[0] = 0.0;
            ca[1] = 0.0;
        }
        w1 = v / SoundVelocity(y);
        i = 1;
        if (w1 < dc[0,0])
        {
            ca[2] = dc[1, 0]; ca[3] = dc[2, 0]; ca[4] = dc[3, 0]; ca[5] = dc[4, 0];
            ca[6] = dc[5, 0]; ca[7] = dc[6, 0];
        }
        else if (w1 > dc[0, 7])
        {
            ca[2] = dc[1, 7]; ca[3] = dc[2, 7]; ca[4] = dc[3, 7]; ca[5] = dc[4, 7];
            ca[6] = dc[5, 7]; ca[7] = dc[6, 7];
        }
        else
        {
            if (w1 >= dc[0, 1])
            {
                for (ij = 0; ij < 8; ij++)
                {
                    i = i + 1;
                    if (w1 <= dc[0, i])
                        break;
                }
            }
            for (j = 1; j < 7; j++)
            {
                ca[j + 1] = (dc[j, i] - dc[j, i - 1]) / (dc[0, i] - dc[0, i - 1]);
                ca[j + 1] = dc[j, i - 1] + ca[j + 1] * (w1 - dc[0, i - 1]);
            }
        }
        if (t < a[26])
        {
            //a[19] : 满载时质心距头部距离  
            //a[20] : 空载时质心距头部距离
            w1 = a[19] + t * (a[20] - a[19]) / a[26];
            for (j = 4; j < 7; j++)
                ca[j] = ca[j] * w1 / a[1];
            ca[1] = ca[1] * 0.75;
        }
        else
        {
            for (j = 4; j < 7; j++)
                ca[j] = ca[j] * a[20] / a[1];
        }
    }
    /// <summary>
    /// 插值函数2
    /// </summary>
    /// <param name="t">时间</param>
    /// <param name="a">弹体参数</param>
    /// <param name="fl">推力--时间</param>
    /// <param name="fl2"></param>
    /// <param name="jj"></param>
    /// <returns></returns>
    private double Cah1(double t, double[] a, double[,] fl, double[] fl2, int jj)
    {
        int i = 0, j, ij;
        double w1;
        double[] ca = new double[4];

        if (t < a[26])
        {
            i = 1;
            if (t > fl[0, 1])
            {
                do
                {
                    i = i + 1;
                } while (t > fl[0, i]);
            }
            ca[0] = (fl2[i] - fl2[i - 1]) / (fl[0, i] - fl[0, i - 1]);
            ca[0] = fl2[i - 1] + ca[0] * (t - fl[0, i - 1]);
        }
        else
        {
            ca[0] = 0.0;
        }
        if (jj == 1)
            return ca[0];
        else
        {
            w1 = 0;
            for (j = 0; j < i - 1; j++)
            {
                w1 += (fl2[j] + fl2[j + 1]) * (fl[0, j + 1] - fl[0, j]) / 2.0;
            }
            w1 += (fl2[i - 1] + ca[0]) * (t - fl[0, i - 1]) / 2.0;

            ca[0] = a[6] + (a[5] - a[6]) * (a[26] - t) / a[26];
            ca[0] = w1 / ca[0];
            return ca[0];
        }
    }
    /// <summary>
    /// 求解微分方程右端函数值函数
    /// </summary>
    /// <param name="a">弹体系数</param>
    /// <param name="t">时间</param>
    /// <param name="fl">推力--时间参数</param>
    /// <param name="dc">空气动力参数</param>
    /// <param name="y"></param>
    /// <param name="d">存放返回参数</param>
    /// <param name="fl2"></param>
    private void F(double[] a, double t, double[,] fl, double[,] dc, double[] y, double[] d, double[] fl2)
    {
        double[] aa, ca;
        aa = new double[2];
        ca = new double[8];
        double fx2, fy2, fz2, m1, m2, m3, s, wp, wxc, fn;
        double vr, ac, b1, b2;
        double w1, w2, w3, w4, w5;
        /**************************
         **  求解诸力和力矩模块  **
         **************************/
        fn = a[12];
        a[12] = fn * 1.0;
        s = 0.7854 * a[0] * a[0];
        wp = a[10] * Math.Cos(y[1]);
        wxc = a[10] * Math.Sin(y[1]);
        vr = (y[0] - wp) * (y[0] - wp) + wxc * wxc + a[11] * a[11];
        vr = Math.Sqrt(vr);
        GJ(y, aa);
        Cah(a, t, y[0], y[10], fl, dc, ca);
        //y[10] : 高度
        w2 = 0.5 * Density(y[10]) * s * ca[2] * vr;
        w3 = 0.5 * Density(y[10]) * s * ca[3];
        fx2 = ca[0] * Math.Cos(aa[0]) * Math.Cos(aa[1]);
        fx2 = fx2 - w2 * (y[0] - wp);
        w1 = wxc * wxc + a[11] * a[11] - wxc * (y[0] - wp) * aa[0];
        w1 = w1 + a[11] * (y[0] - wp) * aa[1];
        fx2 = fx2 + w3 * w1;
        fx2 = fx2 - (a[2] + ca[1]) * 9.81 * Math.Sin(y[1]) * Math.Cos(y[2]);
        fx2 = fx2;
        fy2 = ca[0] * Math.Cos(aa[1]) * Math.Sin(aa[0]) - w2 * wxc;
        fy2 = fy2 + w3 * (vr * vr * aa[0] - (y[0] - wp) * wxc);
        fy2 = fy2 - (a[2] + ca[1]) * 9.81 * Math.Cos(y[1]);
        fy2 = fy2 + 2.0 * (a[2] + ca[1]) * a[9] * y[0] * Math.Cos(a[7]) * Math.Sin(a[8]);
        fy2 = fy2;
        fz2 = ca[0] * Math.Sin(aa[1]) + w2 * a[11];
        fz2 = fz2 + w3 * (vr * vr * aa[1] + (y[0] - wp) * a[11]);
        fz2 = fz2 + (a[2] + ca[1]) * 9.81 * Math.Sin(y[1]) * Math.Sin(y[2]);
        w1 = Math.Sin(a[7]) * Math.Cos(y[1]) - Math.Cos(a[7]) * Math.Cos(a[8]) * Math.Sin(y[1]);
        fz2 = fz2 + 2.0 * (a[2] + ca[1]) * a[9] * y[0] * w1;
        fz2 = fz2;
        w2 = 0.5 * Density(y[10]) * s * a[1] * vr;

        m1 = Cah1(t, a, fl, fl2, 1);
        m1 = m1 + w2 * ca[7] * vr - w2 * y[3] * ca[6] * a[0];
        //a[16] : 推力偏心距 - x方向
        //a[13] : 推力偏心距 - y方向
        m2 = ca[0] * (a[16] * Math.Cos(y[8]) + a[13] * Math.Sin(y[8]));
        m2 = m2 - w2 * a[11] * ca[4] - w2 * ca[4] * (y[0] - wp) * aa[1];
        m2 = m2 - w2 * a[1] * y[4] * ca[5];
        m2 = m2; /*+fn*(a[20]-a[16]); */    /*   NEW   */
        m3 = ca[0] * (a[16] * Math.Sin(y[8]) - a[13] * Math.Cos(y[8]));
        m3 = m3 + w2 * (y[0] - wp) * aa[0] * ca[4] - wxc * ca[4] * w2;
        m3 = m3 - w2 * a[1] * y[5] * ca[5];

        /*******************************
         *   求解微分方程右端值模块   **
         *******************************/
        d[0] = fx2 / (a[2] + ca[1]);
        d[1] = fy2 / ((a[2] + ca[1]) * y[0] * Math.Cos(y[2]));
        d[2] = fz2 / ((a[2] + ca[1]) * y[0]);
        w1 = a[4] + (a[3] - a[4]) * ca[1] / a[21];/*   w1=A  */
        w2 = a[6] + (a[5] - a[6]) * ca[1] / a[21];/*   w2=C  */
        ac = w1 - w2;
        d[8] = y[3] - y[5] * Math.Tan(y[7]);
        b1 = a[22] * Math.Cos(y[8]) - a[23] * Math.Sin(y[8]);
        b2 = a[22] * Math.Sin(y[8]) + a[23] * Math.Cos(y[8]);
        w3 = m2 - w2 * y[3] * y[5] + w1 * y[5] * y[5] * Math.Tan(y[7]);
        w3 = w3 + y[3] * (-a[22] * Math.Sin(y[8]) * d[8] - a[23] * Math.Cos(y[8]) * d[8]) * ac;  /* w3=b  */
        w4 = m3 + w2 * y[3] * y[4] - w1 * y[4] * y[5] * Math.Tan(y[7]);
        w4 = w4 + ac * y[3] * (a[22] * Math.Cos(y[8]) * d[8] - a[23] * Math.Sin(y[8]) * d[8]);  /* w4=c  */
        w5 = m1 + ac * y[3] * (b2 * y[4] - b1 * y[5]);
        w5 = w5 + ac * y[4] * (-a[22] * Math.Sin(y[8]) * d[8] - a[23] * Math.Cos(y[8]) * d[8]);
        w5 = w5 + ac * y[5] * (a[22] * Math.Cos(y[8]) * d[8] - a[23] * Math.Sin(y[8]) * d[8]);   /* w5=a  */

        d[3] = w1 * w5 + b1 * ac * w3 + b2 * w4 * ac;
        d[3] = d[3] / (w1 * w2 - ac * ac * b1 * b1 - ac * ac * b2 * b2);
        d[4] = (w3 + ac * b1 * d[3]) / w1;
        d[5] = (w4 + ac * b2 * d[3]) / w1;
        d[6] = y[5] / Math.Cos(y[7]);
        d[7] = -y[4];
        d[9] = y[0] * Math.Cos(y[2]) * Math.Cos(y[1]);
        d[10] = y[0] * Math.Cos(y[2]) * Math.Sin(y[1]);
        d[11] = y[0] * Math.Sin(y[2]);
    }
    /// <summary>
    /// 求解攻角aa[0],aa[1]函数
    /// </summary>
    /// <param name="y"></param>
    /// <param name="aa"></param>
    private void GJ(double[] y, double[] aa)
    {
        aa[0] = Math.Sin(y[6]) * Math.Cos(y[2]) * Math.Cos(y[7]) - Math.Sin(y[1]);
        aa[0] = aa[0] + Math.Sin(y[1]) * Math.Sin(y[2]) * Math.Sin(y[7]);
        aa[0] = aa[0] / (Math.Cos(y[1]) * Math.Cos(y[2]));
        aa[1] = (Math.Sin(y[7]) - Math.Sin(y[2])) / Math.Cos(y[2]);
        aa[0] = Math.Asin(aa[0]);
        aa[1] = Math.Asin(aa[1]);
    }
    
    private  void Wfmodhs(double wt, double wy, double wfmodn, double wfmodtn, double[] wfmodt, double[] wfmod, double[] wfmodh, 
                          double[] wfmodx, double[] wfmody, double[] wax, bool f1, double[,] ft1, int num1, bool f2, double[,] ft2, int num2)
    {
        int i, j, ij;
        double tt;


    }
}

