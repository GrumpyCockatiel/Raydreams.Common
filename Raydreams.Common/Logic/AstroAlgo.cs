using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Raydreams.Common.AstroAlgo
{
    /// <summary></summary>
    public static class AstroAlgo
    {
        /// <summary>Degrees to Radians conversion factor.</summary>
        public static readonly double Deg2Radian = Math.PI / 180.0;
        public static readonly double Radian2Deg = 180.0 / Math.PI;

        public enum MoonPhase : byte
        {
            NewMoon = 0,
            FirstQuarter = 1,
            FullMoon = 2,
            LastQuarter = 3
        }

        public enum ES : byte
        {
            VernalEquinox = 0,
            SummerSolstice = 1,
            AutumnalEquinox = 2,
            WinterSolstice = 3
        }

        /// <summary>Get the date the equinox or solstice occurs.</summary>
        /// <param name="season">Equinox or solstice to calculate for.</param>
        /// <returns>Date and time event occurs.</returns>
        public static DateTimeOffset GetEquinoxSolstice(ES season)
        {
            return Julian2Date(EquinoxSolstice(DateTime.Now.Year, season));
        }

        /// <summary>Returns a list of dates on which in the input moon phase occurs within the current object's year.  Basically, this method finds all the roots within a given range for a periodic function.</summary>
        /// <param name="phase">Phase to calcualte for. MoonPhases enumeration.</param>
        /// <returns>List of dates phase occurs on.</returns>
        public static List<DateTimeOffset> GetMoonPhases(MoonPhase phase)
        {
            // create a new list
            List<DateTimeOffset> phaseDates = new List<DateTimeOffset>();

            double year = (double)DateTime.Now.Year;

            for (year -= 0.05; year < DateTime.Now.Year + 1; year += 0.05)
            {
                DateTimeOffset d = Julian2Date(MoonPhaseDate(year, phase));

                // if the list already contains the calculated date, do not include it again.
                // or if the date is not within the current object's year.
                if (!phaseDates.Contains(d) && d.Year == DateTime.Now.Year)
                    phaseDates.Add(d);
            }

            return phaseDates;
        }

        /// <summary>Calculate lunar illumination in a year.  Starts at midnight the first day of the year.</summary>
        /// <param name="inc">Whole or fractional days between each calculation.</param>
        public static double[] GetLunations(double inc)
        {
            // increment size must be greater than 0
            inc = (inc > 0) ? inc : 1;

            // create an array the size of number of days in the year
            DateTime day1 = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);

            // create a double array the size of the number of days in the current object's year
            // divided by the increment size
            double[] lunations = new double[(int)(((DateTime.IsLeapYear(day1.Year)) ? 366 : 365) / inc)];

            // calcuate each of the illumination values for each increment
            for (int i = 0; i < lunations.Length; ++i)
            {
                double jd = Date2Julian(day1);
                lunations[i] = SimpleIllumination(jd);
                day1 = day1.AddDays(inc);
            }

            return lunations;
        }

        /// <summary>Calculates Ash Wednesday as the Wednesday 40 days before Ecclesiastical Easter (not counting the six Sundays.</summary>
        /// <param name="e">The day Easter occurs for the year to calculate Ash Wednesday.</param>
        /// <returns>Returns the date of Ash Wednesday.</returns>
        public static DateTimeOffset AshWednesday(DateTimeOffset e)
        {
            return e.Subtract(new TimeSpan(46, 0, 0, 0));
        }

        /// <summary>Calculates the Ecclesiastical Day of Easter in the Greogrian Calendar.  Does not work for Julian Calendar, thus only when y > 1582.  Does not coincide with astronomical Easter.</summary>
        /// <reference>pg 67.</reference>
        /// <remarks>Checks out for all test cases on pg. 68</remarks>
        /// <param name="y">Year in the Gregorian calendar to calculate the day of Easter.</param>
        /// <returns>Returns the date of Easter.</returns>
        public static DateTimeOffset Easter(int y)
        {
            if (y < 1583)
                throw new System.Exception("Input year must be in the Gregorian Calendar (greater than 1582).");

            int a = y % 19;

            int c;

            int b = Math.DivRem(y, 100, out c);

            int e;

            int d = Math.DivRem(b, 4, out e);

            int f = (b + 8) / 25;

            int g = (b - f + 1) / 3;

            int h = (19 * a + b - d - g + 15) % 30;

            int k;

            int i = Math.DivRem(c, 4, out k);

            int L = (32 + 2 * e + 2 * i - h - k) % 7;

            int m = (a + 11 * h + 22 * L) / 451;

            int p;

            int n = Math.DivRem(h + L - 7 * m + 114, 31, out p);

            return new DateTimeOffset(y, n, p + 1, 0, 0, 0, 0, new GregorianCalendar(), new TimeSpan());
        }

        /// <summary>Calculates the Julian Day the input phases occurs closets to the input fractional year.</summary>
        /// <param name="year">Year and day as a fraction in which to calculate the phase occurance.</param>
        /// <param name="phase">Phase to calculate.</param>
        /// <returns>Return a date and time as a fractional Julian Day.</returns>
        public static double MoonPhaseDate(double year, MoonPhase phase)
        {
            double k = 0;
            double t = 0;               /* time in Julian centuries */
            double m = 0;               /* Sun's mean anomaly */
            double mprime = 0;          /* Moon's mean anomaly */
            double f = 0;               /* Moon's argument of latitude */
            double omega = 0;           /* Longitude of the ascending node of the lunar orbit */
            double w = 0;               /* quarter phase corrections */
            double[] a;                 /* planatary arguments */
            double atotal = 0;          /* sum of planatary arguments */
            double corrections = 0; /* sum of corrections */
            double e = 0;               /* eccentricity of Earth's orbit */

            // init planatary arguments array
            a = new double[14];

            k = Math.Floor((year - 2000.0) * 12.3685) + ((double)phase * 0.25);

            t = (k / 1236.85);

            e = 1.0 - t * (0.002516 - (0.0000074 * t)); // pg 308

            m = Deg2Radian * (2.5534 + (29.10535669 * k) - t * t * (0.0000218 - (0.00000011 * t)));

            mprime = Deg2Radian * (201.5643 + (385.81693528 * k) + t * t * (0.0107438 + (0.00001239 * t) - (0.000000058 * t * t)));

            f = Deg2Radian * (160.7108 + (390.67050274 * k) + t * t * (0.0016341 + (0.00000227 * t) - (0.000000011 * t * t)));

            omega = Deg2Radian * (124.7746 - (1.56375580 * k) + t * t * (0.0020691 + (0.00000215 * t)));

            a[0] = Deg2Radian * (299.77 + (0.107408 * k) - (0.009173 * t * t));
            a[1] = Deg2Radian * (251.88 + (0.016321 * k));
            a[2] = Deg2Radian * (251.83 + (26.651886 * k));
            a[3] = Deg2Radian * (349.42 + (36.412478 * k));
            a[4] = Deg2Radian * (84.66 + (18.206239 * k));
            a[5] = Deg2Radian * (141.74 + (53.303771 * k));
            a[6] = Deg2Radian * (207.14 + (2.453732 * k));
            a[7] = Deg2Radian * (154.84 + (7.306860 * k));
            a[8] = Deg2Radian * (34.52 + (27.261239 * k));
            a[9] = Deg2Radian * (207.19 + (0.121824 * k));
            a[10] = Deg2Radian * (291.34 + (1.844379 * k));
            a[11] = Deg2Radian * (161.72 + (24.198154 * k));
            a[12] = Deg2Radian * (239.56 + (25.513099 * k));
            a[13] = Deg2Radian * (331.55 + (3.592518 * k));

            atotal = .000001 * ((325 * Math.Sin(a[0]))
                    + (165 * Math.Sin(a[1]))
                    + (164 * Math.Sin(a[2]))
                    + (126 * Math.Sin(a[3]))
                    + (110 * Math.Sin(a[4]))
                    + (62 * Math.Sin(a[5]))
                    + (60 * Math.Sin(a[6]))
                    + (56 * Math.Sin(a[7]))
                    + (47 * Math.Sin(a[8]))
                    + (42 * Math.Sin(a[9]))
                    + (40 * Math.Sin(a[10]))
                    + (37 * Math.Sin(a[11]))
                    + (35 * Math.Sin(a[12]))
                    + (23 * Math.Sin(a[13])));

            switch (phase)
            {
                case (MoonPhase.NewMoon):
                    {
                        corrections = -(0.40720 * Math.Sin(mprime))
                                + (0.17241 * e * Math.Sin(m))
                                + (0.01608 * Math.Sin(2 * mprime))
                                + (0.01039 * Math.Sin(2 * f))
                                + (0.00739 * e * Math.Sin(mprime - m))
                                - (0.00514 * e * Math.Sin(mprime + m))
                                + (0.00208 * e * e * Math.Sin(2 * m))
                                - (0.00111 * Math.Sin(mprime - 2 * f))
                                - (0.00057 * Math.Sin(mprime + 2 * f))
                                + (0.00056 * e * Math.Sin(2 * mprime + m))
                                - (0.00042 * Math.Sin(3 * mprime))
                                + (0.00042 * e * Math.Sin(m + 2 * f))
                                + (0.00038 * e * Math.Sin(m - 2 * f))
                                - (0.00024 * e * Math.Sin(2 * mprime - m))
                                - (0.00017 * Math.Sin(omega))
                                - (0.00007 * Math.Sin(mprime + 2 * m))
                                + (0.00004 * Math.Sin(2 * mprime - 2 * f))
                                + (0.00004 * Math.Sin(3 * m))
                                + (0.00003 * Math.Sin(mprime + m - 2 * f))
                                + (0.00003 * Math.Sin(2 * mprime + 2 * f))
                                - (0.00003 * Math.Sin(mprime + m + 2 * f))
                                + (0.00003 * Math.Sin(mprime - m + 2 * f))
                                - (0.00002 * Math.Sin(mprime - m - 2 * f))
                                - (0.00002 * Math.Sin(3 * mprime + m))
                                + (0.00002 * Math.Sin(4 * mprime));
                        break;
                    }

                case (MoonPhase.FullMoon):
                    {
                        corrections = -(0.40614 * Math.Sin(mprime))
                        + (0.17302 * e * Math.Sin(m))
                        + (0.01614 * Math.Sin(2 * mprime))
                        + (0.01043 * Math.Sin(2 * f))
                        + (0.00734 * e * Math.Sin(mprime - m))
                        - (0.00515 * e * Math.Sin(mprime + m))
                        + (0.00209 * e * e * Math.Sin(2 * m))
                        - (0.00111 * Math.Sin(mprime - 2 * f))
                        - (0.00057 * Math.Sin(mprime + 2 * f))
                        + (0.00056 * e * Math.Sin(2 * mprime + m))
                        - (0.00042 * Math.Sin(3 * mprime))
                        + (0.00042 * e * Math.Sin(m + 2 * f))
                        + (0.00038 * e * Math.Sin(m - 2 * f))
                        - (0.00024 * e * Math.Sin(2 * mprime - m))
                        - (0.00017 * Math.Sin(omega))
                        - (0.00007 * Math.Sin(mprime + 2 * m))
                        + (0.00004 * Math.Sin(2 * mprime - 2 * f))
                        + (0.00004 * Math.Sin(3 * m))
                        + (0.00003 * Math.Sin(mprime + m - 2 * f))
                        + (0.00003 * Math.Sin(2 * mprime + 2 * f))
                        - (0.00003 * Math.Sin(mprime + m + 2 * f))
                        + (0.00003 * Math.Sin(mprime - m + 2 * f))
                        - (0.00002 * Math.Sin(mprime - m - 2 * f))
                        - (0.00002 * Math.Sin(3 * mprime + m))
                        + (0.00002 * Math.Sin(4 * mprime));
                        break;
                    }

                case (MoonPhase.FirstQuarter):
                case (MoonPhase.LastQuarter):
                    {
                        corrections = -(0.62801 * Math.Sin(mprime))
                        + (0.17172 * e * Math.Sin(m))
                        - (0.01183 * e * Math.Sin(mprime + m))
                        + (0.00862 * Math.Sin(2 * mprime))
                        + (0.00804 * Math.Sin(2 * f))
                        + (0.00454 * e * Math.Sin(mprime - m))
                        + (0.00204 * e * e * Math.Sin(2 * m))
                        - (0.00180 * Math.Sin(mprime - 2 * f))
                        - (0.00070 * Math.Sin(mprime + 2 * f))
                        - (0.00040 * Math.Sin(3 * mprime))
                        - (0.00034 * e * Math.Sin(2 * mprime - m))
                        + (0.00032 * e * Math.Sin(m + 2 * f))
                        + (0.00032 * e * Math.Sin(m - 2 * f))
                        - (0.00028 * e * e * Math.Sin(mprime + 2 * m))
                        + (0.00027 * e * Math.Sin(2 * mprime + m))
                        - (0.00017 * Math.Sin(omega))
                        - (0.00005 * Math.Sin(mprime - m - 2 * f))
                        + (0.00004 * Math.Sin(2 * mprime + 2 * f))
                        - (0.00004 * Math.Sin(mprime + m + 2 * f))
                        + (0.00004 * Math.Sin(mprime - 2 * m))
                        + (0.00003 * Math.Sin(mprime + m - 2 * f))
                        + (0.00003 * Math.Sin(3 * m))
                        + (0.00002 * Math.Sin(2 * mprime - 2 * f))
                        + (0.00002 * Math.Sin(mprime - m + 2 * f))
                        - (0.00002 * Math.Sin(3 * mprime + m));

                        w = .00306 - .00038 * e * Math.Cos(m) + .00026 * Math.Cos(mprime) - .00002 * Math.Cos(mprime - m) + .00002 * Math.Cos(mprime + m) + .00002 * Math.Cos(2 * f);

                        if (phase == MoonPhase.LastQuarter)
                            w = -w;
                        break;
                    }

                default:
                    {
                        ; // throw some exception here
                        break;
                    }
            } // end switch

            return (2451550.09765 + (29.530588853 * k) + (0.0001337 * Math.Pow(t, 2)) - (0.000000150 * Math.Pow(t, 3)) + (0.00000000073 * Math.Pow(t, 4)) + corrections + atotal + w);

        } // MoonPhase

        /// <summary>Converts a fractional Julian Day to a .NET DateTime.</summary>
        /// <param name="JD">Fractional Julian Day to convert.</param>
        /// <returns>Date and Time in .NET DateTime format.</returns>
        public static DateTimeOffset Julian2Date(double JD)
        {
            double A, B, C, D, F, J, Z;
            int E, month, year;
            double alpha;
            double day;

            J = JD + 0.5;

            Z = Math.Floor(J);

            F = J - Z;

            if (Z >= 2299161)
            {
                alpha = Math.Floor((Z - 1867216.25) / 36524.25);
                A = Z + 1 + alpha - Math.Floor(alpha / 4);
            }
            else
                A = Z;

            B = A + 1524;

            C = Math.Floor((B - 122.1) / 365.25);

            D = Math.Floor(365.25 * C);

            E = Convert.ToInt32(Math.Floor( (B - D) / 30.6001 ));

            day = B - D - Math.Floor(30.6001 * E) + F;

            if (E < 14)
                month = E - 1;
            else if (E == 14 || E == 15)
                month = E - 13;
            else
                throw new System.Exception("Illegal month calculated.");

            if (month > 2)
                year = (int)(C - 4716.0);
            else if (month == 1 || month == 2)
                year = (int)(C - 4715.0);
            else
                throw new System.Exception("Illegal year calculated.");

            TimeSpan span = TimeSpan.FromDays(day);

            return new DateTimeOffset(year, month, (int)day, span.Hours, span.Minutes,
                span.Seconds, span.Milliseconds, new GregorianCalendar(), new TimeSpan() );
        }

        /// <summary>Calculates the Julian Day from a DateTime object.</summary>
        /// <param name="d">DateTime object from which to calculate a Julian Day.</param>
        /// <returns>A fractional Julian Day value.</returns>
        /// <remarks>Tested against pg 61 input data.</remarks>
        public static double Date2Julian(DateTimeOffset day)
        {
            // always convert to UTC
            DateTime d = day.UtcDateTime;

            double A, B;
            int theMonth = d.Month;
            int theYear = d.Year;

            if (d.Month <= 2)
            { --theYear; theMonth += 12; }

            A = Math.Floor(theYear / 100.0);

            if (d.Year < 1582)
                B = 0;
            else if (d.Year > 1582)
                B = 2 - A + Math.Floor(A / 4);
            else
            {
                if (d.Month < 10)
                    B = 0;
                else if (d.Month > 10)
                    B = 2 - A + Math.Floor(A / 4);
                else
                {
                    if (d.Day < 5)
                        B = 0;
                    else if (d.Day >= 15)
                        B = 2 - A + Math.Floor(A / 4);
                    else
                        throw new System.Exception("Input day falls between 10/5/1582 and 10/14/1582, which does not exist in the Gregorian Calendar.");
                } // end middle else
            } // end outer else

            double jd = (Math.Floor(365.25 * (theYear + 4716)) + Math.Floor(30.6001 * (theMonth + 1)) + d.Day + B - 1524.5);

            // add fractional parts of the day to the Julian Day
            TimeSpan span = TimeSpan.FromHours(d.Hour) + TimeSpan.FromMinutes(d.Minute) + TimeSpan.FromSeconds(d.Second) + TimeSpan.FromMilliseconds(d.Millisecond);

            return jd + span.TotalDays;
        }

        /// <summary>Calculates time of Equinox and Solstice.
        /// </summary>
        /// <param name="year">Year to calculate for.</param>
        /// <param name="inES">Event to calcualte.</param>
        /// <returns>Date and time event occurs as a fractional Julian Day.</returns>
        public static double EquinoxSolstice(double year, ES inES)
        {
            double y;
            double jden;        /* Julian Ephemeris Day */
            double T;           /* Julian Centuries */
            double W;
            double lambda;
            double S;           /* sum of periodic terms */

            if (year >= 1000)
            {
                y = (Math.Floor(year) - 2000) / 1000;

                if (inES == ES.VernalEquinox) /* march equinox */
                    jden = 2451623.80984 + 365242.37404 * y + 0.05169 * (y * y) - 0.00411 * (y * y * y) - 0.00057 * (y * y * y * y);
                else if (inES == ES.SummerSolstice) /* june solstice */
                    jden = 2451716.56767 + 365241.62603 * y + 0.00325 * (y * y) - 0.00888 * (y * y * y) - 0.00030 * (y * y * y * y);
                else if (inES == ES.AutumnalEquinox) /* september equinox */
                    jden = 2451810.21715 + 365242.01767 * y + 0.11575 * (y * y) - 0.00337 * (y * y * y) - 0.00078 * (y * y * y * y);
                else if (inES == ES.WinterSolstice) /* december solstice */
                    jden = 2451900.05952 + 365242.74049 * y + 0.06223 * (y * y) - 0.00823 * (y * y * y) - 0.00032 * (y * y * y * y);
                else
                    return -1; // throw exception
            }
            else
            {
                y = Math.Floor(year) / 1000;

                if (inES == ES.VernalEquinox) /* march equinox */
                    jden = 1721139.29189 + 365242.13740 * y + 0.06134 * (y * y) - 0.00111 * (y * y * y) - 0.00071 * (y * y * y * y);
                else if (inES == ES.SummerSolstice) /* june solstice */
                    jden = 1721233.25401 + 365241.72562 * y + 0.05323 * (y * y) - 0.00907 * (y * y * y) - 0.00025 * (y * y * y * y);
                else if (inES == ES.AutumnalEquinox) /* september equinox */
                    jden = 1721325.70455 + 365242.49558 * y + 0.11677 * (y * y) - 0.00297 * (y * y * y) - 0.00074 * (y * y * y * y);
                else if (inES == ES.WinterSolstice) /* december solstice */
                    jden = 1721414.39987 + 365242.88257 * y + 0.00769 * (y * y) - 0.00933 * (y * y * y) - 0.00006 * (y * y * y * y);
                else
                    return -1; // throw exception
            }

            T = (jden - 2451545.0) / 36525;

            W = 35999.373 * T - 2.47;

            lambda = 1 + 0.0334 * Math.Cos(W * Deg2Radian) + 0.0007 * Math.Cos(2 * W * Deg2Radian);

            S = 485 * Math.Cos(Deg2Radian * 324.96 + Deg2Radian * (1934.136 * T))
                + 203 * Math.Cos(Deg2Radian * 337.23 + Deg2Radian * (32964.467 * T))
                + 199 * Math.Cos(Deg2Radian * 342.08 + Deg2Radian * (20.186 * T))
                + 182 * Math.Cos(Deg2Radian * 27.85 + Deg2Radian * (445267.112 * T))
                + 156 * Math.Cos(Deg2Radian * 73.14 + Deg2Radian * (45036.886 * T))
                + 136 * Math.Cos(Deg2Radian * 171.52 + Deg2Radian * (22518.443 * T))
                + 77 * Math.Cos(Deg2Radian * 222.54 + Deg2Radian * (65928.934 * T))
                + 74 * Math.Cos(Deg2Radian * 296.72 + Deg2Radian * (3034.906 * T))
                + 70 * Math.Cos(Deg2Radian * 243.58 + Deg2Radian * (9037.513 * T))
                + 58 * Math.Cos(Deg2Radian * 119.81 + Deg2Radian * (33718.147 * T))
                + 52 * Math.Cos(Deg2Radian * 297.17 + Deg2Radian * (150.678 * T))
                + 50 * Math.Cos(Deg2Radian * 21.02 + Deg2Radian * (2281.226 * T))
                + 45 * Math.Cos(Deg2Radian * 247.54 + Deg2Radian * (29929.562 * T))
                + 44 * Math.Cos(Deg2Radian * 325.15 + Deg2Radian * (31555.956 * T))
                + 29 * Math.Cos(Deg2Radian * 60.93 + Deg2Radian * (4443.417 * T))
                + 28 * Math.Cos(Deg2Radian * 155.12 + Deg2Radian * (67555.328 * T))
                + 17 * Math.Cos(Deg2Radian * 288.79 + Deg2Radian * (4562.452 * T))
                + 16 * Math.Cos(Deg2Radian * 198.04 + Deg2Radian * (62894.029 * T))
                + 14 * Math.Cos(Deg2Radian * 199.76 + Deg2Radian * (31436.921 * T))
                + 12 * Math.Cos(Deg2Radian * 95.39 + Deg2Radian * (14577.848 * T))
                + 12 * Math.Cos(Deg2Radian * 287.11 + Deg2Radian * (31931.756 * T))
                + 12 * Math.Cos(Deg2Radian * 320.81 + Deg2Radian * (34777.259 * T))
                + 9 * Math.Cos(Deg2Radian * 227.73 + Deg2Radian * (1222.114 * T))
                + 8 * Math.Cos(Deg2Radian * 15.45 + Deg2Radian * (16859.074 * T));

            return (jden + (0.00001 * S / lambda));

        }

        /// <summary></summary>
        /// <param name="jd"></param>
        /// <returns></returns>
        public static double SimpleIllumination(double jd)
        {
            double k;       /* illuminated fraction of moon's disc */
            double T;       /* Julian Centuries */
            double i;       /* phase angle of the moon */
            double D;       /* mean elogation of the moon */
            double M;       /* sun's mean anomaly */
            double Mprime;  /* moon's mean anomaly */

            // pg. 131
            T = (jd - 2451545.0) / 36525.0;

            D = 297.8502042 + (445267.1115168 * T)
                - (0.0016300 * T * T)
                + ((T * T * T) / 545868)
                - ((T * T * T * T) / 113065000);

            M = 357.5291092 + (35999.0502909 * T)
                - (0.0001536 * T * T)
                + ((T * T * T) / 24490000);

            Mprime = 134.9634114 + (477198.8676313 * T)
                + (0.0089970 * T * T)
                + ((T * T * T) / 69699)
                - ((T * T * T * T) / 14712000);

            D = Math.IEEERemainder(D, 360.0);
            M = Math.IEEERemainder(M, 360.0);
            Mprime = Math.IEEERemainder(Mprime, 360.0);

            M = Deg2Radian * M;
            Mprime = Deg2Radian * Mprime;

            i = 180 - D - (6.289 * Math.Sin(Mprime))
                + (2.100 * Math.Sin(M))
                - (1.274 * Math.Sin(2 * Deg2Radian * D - Mprime))
                - (0.658 * Math.Sin(2 * Deg2Radian * D))
                - (0.214 * Math.Sin(2 * Mprime))
                - (0.110 * Math.Sin(Deg2Radian * D));

            k = (1 + Math.Cos(i * Deg2Radian)) / 2;

            return k;
        }

        /// <summary>Computes the apparent coordinates of the sun</summary>
        /// <param name="JD">Julian Day for day/time to calculate at TD.</param>
        /// <param name="alpha">Apparent right ascention in degrees.</param>
        /// <param name="delta">Apparent declination in degrees.</param>
        /// <remark>Tested against example 24.a, pg. 153 on 2006-Jan-05.</remark>
        public static (double alpha, double delta) ApparentSolarCoordinates(double JD)
        {
            double T;       /* Julian Centuries */
            double L0;      /* geometric mean longitude of the sun */
            double M;       /* mean anomoly */
            double e;       /* eccentricity of Earth's orbit */
            double C;       /* Sun's equation of center */
            double Long;    /* true longitude of the sun */
            double v;       /* true anomaly of the sun */
            double R;       /* Radius in AU */
            double omega;   /* nutation */
            double lamda;   /* apparent longitude of the sun */
            double ep0; /* mean obliquity of the ecliptic */

            // Get Julian Centuries
            T = JulianCenturies(JD);

            // calculate the geometric mean longitude of the sun
            L0 = 280.46645 + 36000.76983 * T + 0.0003032 * T * T;

            // calculate the mean anomaly of the sun
            M = 357.52910 + 35999.05030 * T - 0.0001559 * T * T - 0.00000048 * T * T * T;

            // calculate the eccentricity of the Earth's Orbit
            e = 0.016708617 - 0.000042037 * T - 0.0000001236 * T * T;

            // calculate the sun's equation of center
            C = (1.914600 - 0.004817 * T - 0.000014 * T * T) * Math.Sin(M * Deg2Radian)
                + (0.019993 - 0.000101 * T) * Math.Sin(2 * M * Deg2Radian)
                + 0.000290 * Math.Sin(3 * M * Deg2Radian);

            // calculate the sun's true longitude
            Long = L0 + C;

            // calculate the true anomaly
            v = M + C;

            // calculate the sun's radius vector, distance of the earth in AUs
            R = (1.000001018 * (1 - e * e)) / (1 + e * Math.Cos(v * Deg2Radian));

            omega = 125.04 - 1934.136 * T;

            // calculate the apparent longitude of the sun
            lamda = Revolution(Long - 0.00569 - 0.00478 * Math.Sin(omega * Deg2Radian));

            // calculate the mean obliquity of the ecliptic
            ep0 = ((23 * 60) + 26) * 60 + 21.448 - 46.8150 * T - 0.00059 * T * T + 0.001813 * T * T * T;
            ep0 /= 3600;

            // correct mean obliquity of the ecliptic
            ep0 = ep0 + 0.00256 * Math.Cos(omega * Deg2Radian);

            // calculate right ascension and declination
            double alpha = Revolution(Math.Atan2(Math.Cos(ep0 * Deg2Radian) * Math.Sin(lamda * Deg2Radian), Math.Cos(lamda * Deg2Radian)) * Radian2Deg);

            double delta = Math.Asin(Math.Sin(ep0 * Deg2Radian) * Math.Sin(lamda * Deg2Radian)) * Radian2Deg;

            return (alpha, delta);
        }

        /// <summary>Computes the azimuth and altitude of a body knowing its apparent right ascension and declination.</summary>
        /// <param name="jd">Julian Day for day/time to calculate at UT</param>
        /// <param name="alpha">apparent right ascention in degrees</param>
        /// <param name="delta">apparent declination in degrees</param>
        /// <param name="L">longitude in degrees</param>
        /// <param name="phi">latitude in degrees</param>
        /// <param name="A">azimuth in degrees west of south</param>
        /// <param name="h">altitude in degrees</param>
        public static (double, double) AzimuthAltitude(double jd, double alpha, double delta, double L, double phi)
        {
            double theta0;  // apparent sidereal time at Greenwich
            double H;       // local hour angle

            theta0 = AppSiderealTime(jd);

            /* calculate local hour angle */
            /* normalize the hour angle to +0 to +360 degrees */
            H = Revolution(theta0 - L - alpha);

            /* calculate azimuth */
            double A = Math.Atan2(Math.Sin(H * Deg2Radian), (Math.Cos(H * Deg2Radian) * Math.Sin(phi * Deg2Radian) - Math.Tan(delta * Deg2Radian) * Math.Cos(phi * Deg2Radian))) * Radian2Deg;

            /* calculate altitude */
            double h = Math.Asin(Math.Sin(phi * Deg2Radian) * Math.Sin(delta * Deg2Radian) + Math.Cos(phi * Deg2Radian) * Math.Cos(delta * Deg2Radian) * Math.Cos(H * Deg2Radian)) * Radian2Deg;

            return (A, h);
        }

        /// <summary>Computes the mean sidereal time, the Greenwich hour angle of the mean vernal point (the intersection of the ecliptic of the date with the mean equator of the date), for any instant UT NOT just 0 hour UT.</summary>
        /// <param name="jd">Julian Day at any instant UT</param>
        /// <returns>mean sidereal time at the meridian of Greenwich (double) in degrees.</returns>
        /// <remarks>Tested with Example 11.b pg 85.</remarks>
        public static double MeanSiderealTime(double jd)
        {
            // calculate julian centuries for the given input
            double T = JulianCenturies(jd);

            // calculate mst
            double mst = 280.46061837 + 360.98564736629 * (jd - 2451545.0) + 0.000387933 * T * T - (T * T * T) / 38710000.0;

            mst = Revolution(mst);

            return mst;
        }

        /// <summary>Computes the apparent sidereal time at Greenwich, or the Greenwich hour angle of the true vernal equinox.</summary>
        /// <param name="jd">Julian Day at any instant UT</param>
        /// <returns>Apparent sidereal time at the meridian of Greenwich (double) in degrees.</returns>
        public static double AppSiderealTime(double jd)
        {
            // convert day to Julian Centuries
            double T = JulianCenturies(jd);

            // calculate nutation and obliquity values
            (double DeltaPsi, double DeltaEpsilon) nut = Nutation(T);
            (double Epsilon, double EpsilonNull) ob = Obliquity(T);

            // determine the mean sidereal time for the given day
            double mst = MeanSiderealTime(jd);

            // determine the apparent sidereal time
            return mst + nut.DeltaPsi / 15.0 * Math.Cos(ob.Epsilon * Deg2Radian) / 240.0;
        }

        /// <summary>Computes the mean and true obliquity of the ecliptic</summary>
        /// <param name="T">Julian Centuries</param>
        /// <param name="epsilon">True obliquity of the ecliptic in degrees.</param>
        /// <param name="epsilonNull">Mean obliquity of the ecliptic in arc seconds.</param>
        public static (double Epsilon, double EpsilonNull) Obliquity(double T)
        {
            // calculate mean obliquity of the ecliptic in arc seconds, 21.2
            double epsilonNull = ((23 * 60) + 26) * 60 + 21.448 - 46.8150 * T - 0.00059 * T * T + 0.001813 * T * T * T;

            // convert back to degrees
            epsilonNull /= 3600;

            // calculate nutation of obliquity
            (double DeltaPsi, double DeltaEpsilon) nut = Nutation(T);

            // calculate true obliquity of the ecliptic
            double epsilon = epsilonNull + nut.DeltaEpsilon / 3600;

            return (epsilon, epsilonNull);
        }

        /// <summary>Computes the nutation of longitude and nutation of obliquity of the ecliptic</summary>
        /// <param name="T">Julian Centuries</param>
        /// <param name="deltaPsi">Nutation of longitude in arc seconds.</param>
        /// <param name="deltaEpsilon">Nutation of obliquity in arc seconds.</param>
        public static (double DeltaPsi, double DeltaEpsilon) Nutation(double T)
        {
            double D;       /* longitude of the ascending node of the moon's mean orbit */
            double m;       /* mean anomaly of the Sun (Earth) */
            double mprime;  /* mean anomaly of the moon */
            double F;       /* moon's argument of latitude */
            double omega;	/* longitude of the ascending node of the moon's mean orbit */
            double L = 0;       // Mean longitude of the sun
            double Lprime = 0;  // Mean longitude of the moon

            // calculate the five local variables described on pg. 132
            D = 297.85036 + 445267.111480 * T - 0.0019142 * T * T + (T * T * T) / 189474;

            m = 357.52772 + 35999.050340 * T - 0.0001603 * T * T - (T * T * T) / 300000;

            mprime = 134.96298 + 477198.867398 * T + 0.0086972 * T * T + (T * T * T) / 56250;

            F = 93.27191 + 483202.017538 * T - 0.0036825 * T * T + (T * T * T) / 327270;

            omega = 125.04452 - 1934.136261 * T + 0.0020708 * T * T + (T * T * T) / 450000;

            // calculate mean longitudes
            L = 280.4665 + 36000.7698 * T;
            Lprime = 218.3165 + 481267.8813 * T;

            // only need omega for simplified calculation
            omega = 125.04452 - 1934.136261 * T;

            // calculate nutation of longitude in arc seconds
            double deltaPsi = -17.20 * Math.Sin(omega * Deg2Radian) - 1.32 * Math.Sin(2 * L * Deg2Radian)
                - 0.23 * Math.Sin(2 * Lprime * Deg2Radian) + 0.21 * Math.Sin(2 * omega * Deg2Radian);

            double deltaEpsilon = 9.20 * Math.Cos(omega * Deg2Radian) + 0.57 * Math.Cos(2 * L * Deg2Radian)
                + 0.10 * Math.Cos(2 * Lprime * Deg2Radian) - 0.09 * Math.Cos(2 * omega * Deg2Radian);

            return (deltaPsi, deltaEpsilon);

            // coefficients are in units of 0".0001 so convert
            //deltaPsi *= 0.0001;			
            //deltaEpsilon *= 0.0001;
        }

        /// <summary>Computes the rising, setting and transit time of a body</summary>
        /// <param name="L">longitude in degrees</param>
        /// <param name="phi">latitude in degrees</param>
        /// <param name="h0">standard altitude of the body at time of rising and setting
        /// sun = -0.8333
        /// stars & planets = -0.5667
        /// moon (mean value ONLY) = +0.125
        /// </param>
        /// <param name="JD">Julian Day for day/time to calculate at 0 hour UT</param>
        /// <param name="A">apparent right ascention in degrees at JD-1, JD and JD+1 respectively at 0 hour Dynamical Time</param>
        /// <param name="D">apparent declination in degrees at JD-1, JD and JD+1 respectively at 0 hour Dynamical Time</param>
        /// <returns>Triple of transit, rising, and setting times.</returns>
        public static double[] RiseTranSet(double L, double phi, double h0, double JD, double[] A, double[] D)
        {
            double theta0 = 177.74208;      // apparent sidereal time
            double H0;                      // approximate time
            double[] theta = new double[3]; // array of sidereal times, transit, rising, setting
            double[] n = new double[3];     // interpolating factor, transit, rising, setting
            double[] alpha = new double[3]; // right ascention correction factor
            double[] gamma = new double[3]; // declination correction factor
            double[] H = new double[3];     // local hour angle
            double[] h = new double[3];     // altitude
            double deltaT = 56;
            double[] m = new double[3];     // transit, rising, setting time respectively of object

            // get apparent sidereal time at greenwich at 0 hour Universal Time on JD from an almanac or calculation
            // use a test value theta0 = 177.74208 from pg 99
            theta0 = AppSiderealTime(JD);

            // Make sure the body is not above or below the horizon all day
            if (Math.Abs(-Math.Sin(phi * Deg2Radian) * Math.Sin(D[1] * Deg2Radian) / Math.Cos(phi * Deg2Radian) * Math.Cos(D[1] * Deg2Radian)) > 1)
                throw new System.Exception("The body is above or below the horizon all day and thus has no rising or setting time.");

            // Calculate approximate times, 14.1
            H0 = Math.Acos((Math.Sin(h0 * Deg2Radian) - Math.Sin(phi * Deg2Radian) * Math.Sin(D[1] * Deg2Radian)) / (Math.Cos(phi * Deg2Radian) * Math.Cos(D[1] * Deg2Radian))) * Radian2Deg;

            // calculate transit time
            m[0] = Normalize0To1((A[1] + L - theta0) / 360.0);

            // calculate rise and set time
            m[1] = Normalize0To1(m[0] - H0 / 360.0);
            m[2] = Normalize0To1(m[0] + H0 / 360.0);

            // for each m value do
            for (short i = 0; i < 3; ++i)
            {
                // compute the sidereal time
                theta[i] = theta0 + 360.985647 * m[i];
                theta[i] = Revolution(theta[i]);

                // compute interpolating factor
                n[i] = m[i] + (deltaT / 86400.0);

                // interpolate alpha and gamma from input, 3.3
                alpha[i] = A[1] + (n[i] / 2.0) * (-A[0] + A[2] + n[i] * (A[2] - A[1] - A[1] + A[0]));
                gamma[i] = D[1] + (n[i] / 2.0) * (-D[0] + D[2] + n[i] * (D[2] - D[1] - D[1] + D[0]));

                // calculate local hour angle
                H[i] = Revolution180(theta[i] - L - alpha[i]);

                // calculate altitude, 12.6
                h[i] = Math.Asin(Math.Sin(phi * Deg2Radian) * Math.Sin(gamma[i] * Deg2Radian) + Math.Cos(phi * Deg2Radian) * Math.Cos(gamma[i] * Deg2Radian) * Math.Cos(H[i] * Deg2Radian)) * Radian2Deg;
            }

            // make corrections
            m[0] = m[0] + (-H[0] / 360.0);
            m[1] = m[1] + (h[1] - h0) / (360 * Math.Cos(gamma[1]) * Math.Cos(phi) * Math.Sin(H[1]));
            m[2] = m[2] + (h[2] - h0) / (360 * Math.Cos(gamma[2]) * Math.Cos(phi) * Math.Sin(H[2]));

            return m;
        }

        /// <summary>Calculate Julian Centuries.</summary>
        /// <param name="JDe">Julian Day Ephemeris</param>
        /// <returns>Julian Centuries (double) from the Epoch J2000.0 (JDe 2451545.0)</returns>
        public static double JulianCenturies(double JDe)
        {
            return (JDe - 2451545.0) / 36525.0;
        }

        /// <summary>Returns a TimeSpan from an input number of fractional days.  Same as TimeSpan.FromDays()</summary>
        /// <param name="x">Fractional days.</param>
        public static TimeSpan Fraction2Time(double x)
        {
            // obtain the fractional part of the input
            double i = x - Math.Floor(x);

            // calculate hours, minutes and seconds
            int hour = (int)(i * 24);
            int minute = (int)((i * 24 - hour) * 60);
            int second = (int)(((i * 24 - hour) * 60 - minute) * 60);
            int millisecond = (int)(((i * 24 - hour) * 60 - minute) * 60 - second) * 1000;

            // create TimeSpan
            return new TimeSpan((int)Math.Floor(x), hour, minute, second, millisecond);
        }

        /// <summary>Normalizes the input angle to an equivalent positive angle between 0 and 360.</summary>
        /// <param name="theta">Input angle in degrees.</param>
        /// <returns>Normalized angle in degrees.</returns>
        /// <example>An input of -1677831.2621266 would return 128.737873400096</example>
        public static double Revolution(double theta)
        {
            //if ( theta >= 0 )
            //    return Math.IEEERemainder(theta, 360.0);
            //else
            //    return 360.0 + Math.IEEERemainder(theta, 360.0);

            while (theta < 0.0 || theta > 360.0)
            {
                if (theta > 360.0)
                    theta -= 360.0;
                else if (theta < 0.0)
                    theta += 360.0;
                else break;
            }

            return theta;
        }

        /// <summary>Normalizes the input angle to an equivalent angle between +180.0 and -180.0</summary>
        /// <param name="theta">input angle in degrees</param>
        /// <returns>normalized angle (double) in degrees</returns>
        public static double Revolution180(double theta)
        {
            while (theta < -180.0 || theta > 180.0)
            {
                //theta = Math.IEEERemainder(theta, 360.0);

                if (theta > 180.0)
                    theta -= 180.0;
                else if (theta < -180.0)
                    theta += 180.0;
                else break;
            }

            return theta;
        }

        /// <summary>Normalizes the input value to a value between 0 and 1.</summary>
        /// <param name="x">value to normalize</param>
        /// <returns>value between 0 and 1 (double)</returns>
        /// <remarks>
        ///	f(x) =	x + 1,	x < 0
        ///			x,		0 <= x <= 1
        ///			x - 1,	x > 1
        ///			
        ///	Examples:
        ///	-0.65 -> 0.35
        ///	-3.45 -> 0.55
        ///	+1.25 -> 0.25
        /// </remarks>
        public static double Normalize0To1(double x)
        {
            while (x < 0 || x > 1)
            {
                if (x > 1)
                    --x;
                else
                    ++x;
            }

            return x;
        }

        /// <summary>Returns the JD value at 0 hour (midnight) of the given input Julian Day.</summary>
        /// <param name="jd">Julian day.</param>
        /// <returns>Julian day at midnight of the input Julian Day.</returns>
        public static double ZeroHourJulian(double jd)
        {
            return Math.Floor(jd - 0.5) + 0.5;
        }

        /// <summary>Returns what day of the week the input Julian Day is.</summary>
        /// <param name="jd">Julian day.</param>
        /// <returns>day of the week: 0 = Sunday...6 = Saturday</returns>
        public static int DayOfWeek(double jd)
        {
            return (int)(ZeroHourJulian(jd) + 1.5) % 7;
        }

    }

}