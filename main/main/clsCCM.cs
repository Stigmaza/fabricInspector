using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMCOMPARE
{
    class clsCCM
    {
        public static double[,] GetLightXYZ(string stdLight)
        {
            double[,] numArray = new double[3, 31];
            string[] strArray1 = new string[31];
            switch (stdLight)
            {
                case "D65-2":
                    strArray1 = XYZfactor31_0();
                    break;
                case "D65-10":
                    strArray1 = XYZfactor31_1();
                    break;
                case "TL84-2":
                    strArray1 = XYZfactor31_2();
                    break;
                case "TL84-10":
                    strArray1 = XYZfactor31_3();
                    break;
                case "A-2":
                    strArray1 = XYZfactor31_4();
                    break;
                case "A-10":
                    strArray1 = XYZfactor31_5();
                    break;
                case "CWF-2":
                    strArray1 = XYZfactor31_6();
                    break;
                case "CWF-10":
                    strArray1 = XYZfactor31_7();
                    break;
                case "D50-2":
                    strArray1 = XYZfactor31_8();
                    break;
                case "D50-10":
                    strArray1 = XYZfactor31_9();
                    break;
                case "D55-2":
                    strArray1 = XYZfactor31_10();
                    break;
                case "D55-10":
                    strArray1 = XYZfactor31_11();
                    break;
                case "D75-2":
                    strArray1 = XYZfactor31_12();
                    break;
                case "D75-10":
                    strArray1 = XYZfactor31_13();
                    break;
                case "C-2":
                    strArray1 = XYZfactor31_14();
                    break;
                case "C-10":
                    strArray1 = XYZfactor31_15();
                    break;
                case "T83-2":
                    strArray1 = XYZfactor31_16();
                    break;
                case "T83-10":
                    strArray1 = XYZfactor31_17();
                    break;
                case "F7-2":
                    strArray1 = XYZfactor31_18();
                    break;
                case "F7-10":
                    strArray1 = XYZfactor31_19();
                    break;
                case "F8-2":
                    strArray1 = XYZfactor31_20();
                    break;
                case "F8-10":
                    strArray1 = XYZfactor31_21();
                    break;
                case "F10-2":
                    strArray1 = XYZfactor31_22();
                    break;
                case "F10-10":
                    strArray1 = XYZfactor31_23();
                    break;
                case "UL3000-2":
                    strArray1 = XYZfactor31_24();
                    break;
                case "UL3000-10":
                    strArray1 = XYZfactor31_25();
                    break;
                case "NBF11-2":
                    strArray1 = XYZfactor31_26();
                    break;
                case "NBF11-10":
                    strArray1 = XYZfactor31_27();
                    break;
                case "SPL D65-2":
                    strArray1 = XYZfactor31_28();
                    break;
                case "SPL D65-10":
                    strArray1 = XYZfactor31_29();
                    break;
                case "SPL D75-2":
                    strArray1 = XYZfactor31_30();
                    break;
                case "SPL D75-10":
                    strArray1 = XYZfactor31_31();
                    break;
                case "UL3500-2":
                    strArray1 = XYZfactor31_32();
                    break;
                case "UL3500-10":
                    strArray1 = XYZfactor31_33();
                    break;
            }
            for (int index = 0; index <= 30; ++index)
            {
                string[] strArray2 = strArray1[index].Split(' ');
                numArray[0, index] = Convert.ToDouble(strArray2[0].Trim());
                numArray[1, index] = Convert.ToDouble(strArray2[1].Trim());
                numArray[2, index] = Convert.ToDouble(strArray2[2].Trim());
            }
            return numArray;
        }

        public static double[] GetLightTXYZ(double[,] XYZ)
        {
            double[] numArray = new double[3];
            for (int index1 = 0; index1 <= 2; ++index1)
            {
                for (int index2 = 0; index2 <= 30; ++index2)
                    numArray[index1] += XYZ[index1, index2];
            }
            return numArray;
        }

        public static double[] StrREFLtoDouble(string strREFL)
        {
            string[] strArray = strREFL.Split(',');
            double[] numArray = new double[strArray.Length];
            for (int index = 0; index < strArray.Length; ++index)
            {
                double result = 0.0;
                double.TryParse(strArray[index], out result);
                numArray[index] = result;
            }
            return numArray;
        }

        public static double[] REFLtoKS(double[] REFL)
        {
            double[] numArray = new double[31];
            for (int index = 0; index <= 30; ++index)
            {
                double num1 = REFL[index] / 100.0;
                if (num1 > 1.0)
                {
                    double num2 = 2.0 - num1;
                    numArray[index] = -((1.0 - num2) * (1.0 - num2) / (2.0 * num2));
                }
                else
                    numArray[index] = (1.0 - num1) * (1.0 - num1) / (2.0 * num1);
            }
            return numArray;
        }

        public static double[] KStoREFL(double[] KS)
        {
            double[] numArray = new double[31];
            for (int index = 0; index <= 30; ++index)
            {
                double num1;
                if (KS[index] < 0.0)
                {
                    double num2 = -KS[index];
                    double num3 = Math.Sqrt(num2 * num2 + 2.0 * num2);
                    num1 = 2.0 - (1.0 + num2 - num3);
                }
                else
                {
                    double num2 = Math.Sqrt(KS[index] * KS[index] + 2.0 * KS[index]);
                    num1 = 1.0 + KS[index] - num2;
                }
                if (num1 < 0.0)
                    num1 = 0.0;
                numArray[index] = num1 * 100.0;
            }
            return numArray;
        }

        public static double[] LABtoXYZ(string stdLight, double[] LAB)
        {
            double[] numArray1 = new double[3];
            double[,] numArray2 = new double[3, 31];
            double[] numArray3 = new double[3];
            double[] numArray4 = new double[3];
            double[] numArray5 = new double[3];
            double[] lightTxyz = GetLightTXYZ(GetLightXYZ(stdLight));
            numArray5[1] = (LAB[0] + 16.0) / 116.0;
            numArray5[0] = LAB[1] / 500.0 + numArray5[1];
            numArray5[2] = numArray5[1] - LAB[2] / 200.0;
            for (int index = 0; index <= 2; ++index)
                numArray4[index] = numArray5[index] >= 0.206823 ? Math.Pow(numArray5[index], 3.0) : (numArray5[index] - 4.0 / 29.0) / 7.787;
            for (int index = 0; index <= 2; ++index)
                numArray3[index] = numArray4[index] * lightTxyz[index];
            return numArray3;
        }

        public static double[] XYZtoLAB(string stdLight, double[] XYZ)
        {
            double[] numArray1 = new double[3];
            double[,] numArray2 = new double[3, 31];
            double[] numArray3 = new double[3];
            double[] numArray4 = new double[3];
            double[] numArray5 = new double[3];
            double[] lightTxyz = GetLightTXYZ(GetLightXYZ(stdLight));
            for (int index = 0; index <= 2; ++index)
                numArray4[index] = XYZ[index] / lightTxyz[index];
            for (int index = 0; index <= 2; ++index)
                numArray5[index] = numArray4[index] > 0.008856 ? Math.Pow(numArray4[index], 1.0 / 3.0) : 7.787 * numArray4[index] + 4.0 / 29.0;
            numArray3[0] = 116.0 * numArray5[1] - 16.0;
            numArray3[1] = (numArray5[0] - numArray5[1]) * 500.0;
            numArray3[2] = (numArray5[1] - numArray5[2]) * 200.0;
            return numArray3;
        }

        public static double[] LABtoLCH(double[] LAB)
        {
            double[] numArray = new double[3]
            {
        LAB[0],
        Math.Sqrt(LAB[1] * LAB[1] + LAB[2] * LAB[2]),
        0.0
            };
            double num1 = 57.2958279087978;
            if (LAB[1] == 0.0)
                numArray[2] = Math.Sign(LAB[2]) != 1 ? 270.0 : 90.0;
            else if (LAB[2] == 0.0)
            {
                numArray[2] = Math.Sign(LAB[1]) != 1 ? 180.0 : 0.0;
            }
            else
            {
                double num2 = Math.Atan(Math.Abs(LAB[2] / LAB[1])) * num1;
                if (Math.Sign(LAB[1]) == 1 && Math.Sign(LAB[2]) == 1)
                    numArray[2] = num2;
                if (Math.Sign(LAB[1]) == -1 && Math.Sign(LAB[2]) == 1)
                    numArray[2] = 180.0 - num2;
                if (Math.Sign(LAB[1]) == -1 && Math.Sign(LAB[2]) == -1)
                    numArray[2] = num2 + 180.0;
                if (Math.Sign(LAB[1]) == 1 && Math.Sign(LAB[2]) == -1)
                    numArray[2] = 360.0 - num2;
            }
            return numArray;
        }

        public static double[] LABtoRGB(double[] LAB)
        {
            double[] numArray1 = new double[3];
            double[,] numArray2 = new double[3, 31];
            double[] numArray3 = new double[3];
            double[] numArray4 = new double[3];
            double[] numArray5 = new double[3];
            double[] lightTxyz = GetLightTXYZ(GetLightXYZ("D65-2"));
            numArray5[1] = (LAB[0] + 16.0) / 116.0;
            numArray5[0] = LAB[1] / 500.0 + numArray5[1];
            numArray5[2] = numArray5[1] - LAB[2] / 200.0;
            for (int index = 0; index <= 2; ++index)
                numArray4[index] = numArray5[index] >= 0.206823 ? Math.Pow(numArray5[index], 3.0) : (numArray5[index] - 4.0 / 29.0) / 7.787;
            double[] numArray6 = new double[3];
            for (int index = 0; index <= 2; ++index)
                numArray6[index] = numArray4[index] * lightTxyz[index];
            double num1 = numArray6[0] / 100.0;
            double num2 = numArray6[1] / 100.0;
            double num3 = numArray6[2] / 100.0;
            double[] numArray7 = new double[3]
            {
        3.2406 * num1 + -1.5372 * num2 + -0.4986 * num3,
        -0.9689 * num1 + 1.8758 * num2 + 0.0415 * num3,
        0.0557 * num1 + -0.204 * num2 + 1.057 * num3
            };
            for (int index = 0; index <= 2; ++index)
            {
                if (numArray7[index] < 0.0)
                    numArray7[index] = 0.0;
                if (numArray7[index] > 1.0)
                    numArray7[index] = 1.0;
            }
            double[] numArray8 = new double[3];
            for (int index = 0; index <= 2; ++index)
                numArray8[index] = numArray7[index] <= 0.0031308 ? 12.92 * numArray7[index] : 1.055 * Math.Pow(numArray7[index], 5.0 / 12.0) - 0.055;
            double[] numArray9 = new double[3];
            for (int index = 0; index <= 2; ++index)
                numArray9[index] = (double)byte.MaxValue * numArray8[index];
            return numArray9;
        }

        public static double[] KStoXYZ(string stdLight, double[] KS)
        {
            double[] REFL = KStoREFL(KS);
            double[] numArray = new double[3];
            return REFLtoXYZ(stdLight, REFL);
        }

        public static double[] REFLtoXYZ(string stdLight, double[] REFL)
        {
            double[,] numArray1 = new double[3, 31];
            double[,] lightXyz = GetLightXYZ(stdLight);
            double[] numArray2 = new double[3];
            for (int index1 = 0; index1 <= 2; ++index1)
            {
                numArray2[index1] = 0.0;
                for (int index2 = 0; index2 <= 30; ++index2)
                    numArray2[index1] += REFL[index2] / 100.0 * lightXyz[index1, index2];
            }
            return numArray2;
        }

        // REFLtoXYZLABCHRGB : 오리지널 샘플의 반사율로 XYZ, LAB, LCH, RGB 를 계산하는 함수
        public static double[] REFLtoXYZLABCHRGB(string stdLight, double[] REFL)
        {
            // 변환순서대로 진행해야 함.
            double[] XYZ = REFLtoXYZ(stdLight, REFL); // 반사율에서 XYZ 산출
            double[] LAB = XYZtoLAB(stdLight, XYZ); // XYZ에서 LAB 산출
            double[] numArray1 = LABtoLCH(LAB); // LAB 에서 LCH 산출
            double[] numArray2 = LABtoRGB(LAB); // LAB 에서 RGB 산출

            // XYZLABCHRGB 값 리턴
            return new double[12]
          {
        XYZ[0],
        XYZ[1],
        XYZ[2],
        LAB[0],
        LAB[1],
        LAB[2],
        numArray1[0],
        numArray1[1],
        numArray1[2],
        numArray2[0],
        numArray2[1],
        numArray2[2]
          };
        }

        public static double[] REFLtoRGB(string stdLight, double[] REFL)
        {
            // 변환순서대로 진행해야 함.
            double[] XYZ = REFLtoXYZ(stdLight, REFL); // 반사율에서 XYZ 산출
            double[] LAB = XYZtoLAB(stdLight, XYZ); // XYZ에서 LAB 산출
            double[] numArray2 = LABtoRGB(LAB); // LAB 에서 RGB 산출

            // XYZLABCHRGB 값 리턴
            return new double[] { numArray2[0], numArray2[1], numArray2[2] };
        }

        public static double[] REFLtoLAB(string stdLight, double[] REFL)
        {
            // 변환순서대로 진행해야 함.
            double[] XYZ = REFLtoXYZ(stdLight, REFL); // 반사율에서 XYZ 산출
            double[] LAB = XYZtoLAB(stdLight, XYZ); // XYZ에서 LAB 산출

            // XYZLABCHRGB 값 리턴
            return new double[] { LAB[0], LAB[1], LAB[2], };
        }


        public static double[] XYZLABCHRGBtoDXYZLABCHE(string eq, double[] T, double[] BT)
        {
            // 색차계산 : Delta 값 = 비교샘플 - 오리지널
            double[] numArray1 = new double[9]
            {
        BT[0] - T[0],
        BT[1] - T[1],
        BT[2] - T[2],
        BT[3] - T[3],
        BT[4] - T[4],
        BT[5] - T[5],
        BT[6] - T[6],
        0.0,
        0.0
            };
            double[] numArray2 = new double[2];
            double[] numArray3 = new double[2];
            double[] numArray4 = new double[2];
            double[] numArray5 = new double[2];
            double[] numArray6 = new double[2];
            numArray2[0] = T[3]; // 오리지널 L값
            numArray3[0] = T[4]; // 오리지널 a값
            numArray4[0] = T[5];// 오리지널 b값
            numArray5[0] = T[6];// 오리지널 C값
            numArray6[0] = T[7];// 오리지널 H값
            numArray2[1] = BT[3]; // 비교샘플 L값
            numArray3[1] = BT[4]; // 비교샘플 a값
            numArray4[1] = BT[5]; // 비교샘플 b값
            numArray5[1] = BT[6]; // 비교샘플 C값
            numArray6[1] = BT[7]; // 비교샘플 H값
            double num1 = numArray1[3]; // L
            double num2 = numArray1[4]; // a
            double num3 = numArray1[5]; // b
            double num4 = numArray1[6]; // H
            double num5 = Math.Sqrt(num1 * num1 + num2 * num2 + num3 * num3); // 종합색차 dE
            numArray1[8] = num5;
            double num6 = numArray6[1] - numArray6[0]; // dh : 각도
            if (num6 > 180.0)
                num6 -= 360.0;
            else if (num6 < -180.0)
                num6 += 360.0;

            double num7 = num6 >= 0.0 ? Math.Sqrt(Math.Abs(num2 * num2 + num3 * num3 - num4 * num4)) : -Math.Sqrt(Math.Abs(num2 * num2 + num3 * num3 - num4 * num4));
            numArray1[7] = num7; // dC
            double num8 = 2.0; // 색차식 상수값 CMC(2:1)
            double num9 = 1.0;
            double num10 = 0.0174532777777778; // 3.14 / 180 
            switch (eq.ToUpper())
            {
                case "CMC(2:1)":
                    double num11 = T[3] >= 16.0 ? 0.040975 * T[3] / (1.0 + 0.01765 * T[3]) : 0.511;
                    double num12 = 0.0638 * numArray5[0] / (1.0 + 0.0131 * numArray5[0]) + 0.638;
                    double num13 = Math.Sqrt(Math.Pow(numArray5[0], 4.0) / (Math.Pow(numArray5[0], 4.0) + 1900.0));
                    double num14 = numArray6[0] >= 164.0 && numArray6[0] <= 345.0 ? 0.56 + Math.Abs(0.2 * Math.Cos((numArray6[0] + 168.0) * num10)) : 0.36 + Math.Abs(0.4 * Math.Cos((numArray6[0] + 35.0) * num10));
                    double num15 = num12 * (num13 * num14 + 1.0 - num13);
                    double num16 = num1 / (num8 * num11);
                    double num17 = num4 / (num9 * num12);
                    double num18 = num7 / num15;
                    double num19 = Math.Sqrt(num16 * num16 + num17 * num17 + num18 * num18);
                    numArray1[8] = num19;
                    break;
                case "CIE2000(2:1)":
                    double num20 = numArray2[0];
                    double num21 = numArray2[1];
                    double x = (numArray5[0] + numArray5[1]) / 2.0;
                    double num22 = Math.Pow(x, 7.0);
                    double num23 = 0.5 * (1.0 - Math.Sqrt(num22 / (num22 + Math.Pow(25.0, 7.0))));
                    double num24 = (1.0 + num23) * numArray3[0];
                    double num25 = (1.0 + num23) * numArray3[1];
                    double num26 = numArray4[0];
                    double num27 = numArray4[1];
                    double num28 = Math.Sqrt(num24 * num24 + num26 * num26);
                    double num29 = Math.Sqrt(num25 * num25 + num27 * num27);
                    double num30 = numArray6[0];
                    double num31 = numArray6[1];
                    double num32 = numArray1[3];
                    double num33 = numArray1[6];
                    double num34 = num31 - num30;
                    double num35 = 2.0 * Math.Sqrt(num29 * num28) * Math.Sin(num34 / 2.0 * num10);
                    double num36 = (num20 + num21) / 2.0;
                    double num37 = (num30 + num31) / 2.0;
                    double num38 = 2.0 * Math.Sqrt(num22 / (num22 + Math.Pow(25.0, 7.0)));
                    double num39 = -Math.Sin(2.0 * (30.0 * Math.Exp(-Math.Pow((num37 - 275.0) / 25.0, 2.0))) * num10) * num38;
                    double num40 = 1.0 + -0.17 * Math.Cos((num37 - 30.0) * num10) + 0.24 * Math.Cos(2.0 * num37 * num10) + 0.32 * Math.Cos((3.0 * num37 + 6.0) * num10) + -0.2 * Math.Cos((4.0 * num37 - 63.0) * num10);
                    double num41 = (num36 - 50.0) * (num36 - 50.0);
                    double num42 = 1.0 + 0.015 * num41 / Math.Sqrt(20.0 + num41);
                    double num43 = 1.0 + 0.045 * x;
                    double num44 = 1.0 + 0.015 * x * num40;
                    double num45 = num32 / (num8 * num42);
                    double num46 = num33 / num43;
                    double num47 = num35 / num44;
                    double num48 = Math.Sqrt(num45 * num45 + num46 * num46 + num47 * num47 + num39 * num46 * num47);
                    numArray1[8] = num48;
                    break;
            }
            return numArray1;
        }

        public static string[] XYZfactor31_0() => new string[31]
        {
            "0.188 \t0.009 \t1.001",
            "0.215 \t0.000 \t0.765",
            "1.388 \t0.068 \t6.941",
            "2.218 \t0.065 \t11.171",
            "3.564 \t0.199 \t16.853",
            "3.675 \t0.440 \t19.819",
            "3.145 \t0.848 \t18.588",
            "2.207 \t0.572 \t14.132",
            "1.077 \t1.913 \t8.660",
            "0.246 \t1.989 \t4.864",
            "0.124 \t3.336 \t3.025",
            "0.054 \t5.180 \t1.407",
            "0.639 \t7.060 \t0.882",
            "1.692 \t8.670 \t0.418",
            "2.852 \t9.546 \t0.152",
            "4.289 \t9.702 \t0.108",
            "5.581 \t9.411 \t0.042",
            "7.033 \t8.777 \t0.037",
            "8.176 \t7.734 \t0.000",
            "8.736 \t6.491 \t0.007",
            "9.045 \t5.328 \t0.011",
            "8.481 \t4.272 \t0.001",
            "6.955 \t3.134 \t0.000",
            "5.316 \t2.129 \t0.000",
            "3.460 \t1.367 \t0.000",
            "1.968 \t0.795 \t0.000",
            "1.375 \t0.497 \t0.000",
            "0.872 \t0.238 \t0.000",
            "0.164 \t0.136 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.313 \t0.093 \t0.000"
        };

        public static string[] XYZfactor31_1() => new string[31]
        {
            "0.181 \t0.014 \t0.690",
            "0.573 \t0.073 \t2.976",
            "1.735 \t0.167 \t7.816",
            "2.333 \t0.253 \t11.910",
            "3.510 \t0.667 \t17.651",
            "3.658 \t0.743 \t19.857",
            "3.062 \t1.447 \t17.672",
            "1.942 \t1.675 \t13.001",
            "0.811 \t2.702 \t7.773",
            "0.125 \t3.054 \t3.821",
            "0.087 \t4.429 \t2.131",
            "0.299 \t5.583 \t1.024",
            "1.093 \t6.901 \t0.540",
            "2.170 \t8.077 \t0.287",
            "3.386 \t8.652 \t0.117",
            "4.757 \t8.872 \t0.035",
            "6.038 \t8.559 \t0.002",
            "7.336 \t7.983 \t0.000",
            "8.278 \t7.069 \t0.000",
            "8.631 \t6.033 \t0.000",
            "8.684 \t5.055 \t0.000",
            "7.930 \t4.095 \t0.000",
            "6.401 \t2.960 \t0.000",
            "4.756 \t2.071 \t0.000",
            "3.078 \t1.306 \t0.000",
            "1.741 \t0.692 \t0.000",
            "1.137 \t0.442 \t0.000",
            "0.656 \t0.256 \t0.000",
            "0.203 \t0.084 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.219 \t0.085 \t0.000"
        };

        public static string[] XYZfactor31_2() => new string[31]
        {
            "0.081 \t0.001 \t0.326",
            "0.029 \t0.000 \t0.235",
            "0.219 \t0.008 \t0.907",
            "2.729 \t0.109 \t13.476",
            "3.651 \t0.243 \t18.335",
            "1.441 \t0.216 \t6.915",
            "0.971 \t0.088 \t6.898",
            "0.826 \t0.327 \t4.307",
            "0.379 \t0.977 \t3.773",
            "0.294 \t1.370 \t4.066",
            "0.000 \t1.359 \t1.064",
            "0.000 \t0.526 \t0.089",
            "0.000 \t0.018 \t0.046",
            "0.000 \t0.477 \t0.034",
            "8.836 \t27.514 \t0.517",
            "10.911 \t25.889 \t0.269",
            "0.154 \t0.696 \t0.000",
            "1.851 \t1.875 \t0.002",
            "6.993 \t6.566 \t0.009",
            "8.430 \t6.419 \t0.011",
            "7.064 \t4.139 \t0.010",
            "26.373 \t13.055 \t0.008",
            "12.925 \t6.095 \t0.000",
            "3.507 \t1.065 \t0.000",
            "1.049 \t0.702 \t0.000",
            "0.436 \t0.083 \t0.000",
            "0.359 \t0.098 \t0.000",
            "0.105 \t0.052 \t0.000",
            "0.013 \t0.003 \t0.000",
            "0.000 \t0.018 \t0.000",
            "0.067 \t0.014 \t0.000"
        };

        public static string[] XYZfactor31_3() => new string[31]
        {
            "0.111 \t0.020 \t0.342",
            "0.139 \t0.000 \t1.212",
            "0.301 \t0.046 \t0.059",
            "2.858 \t0.378 \t15.600",
            "3.769 \t0.512 \t18.829",
            "1.495 \t0.513 \t7.332",
            "0.991 \t0.312 \t7.153",
            "0.789 \t0.508 \t3.953",
            "0.237 \t1.614 \t3.351",
            "0.206 \t2.308 \t3.642",
            "0.000 \t1.787 \t0.661",
            "0.000 \t0.515 \t0.133",
            "0.000 \t0.159 \t0.023",
            "0.000 \t0.355 \t0.000",
            "10.905 \t26.323 \t0.343",
            "12.510 \t24.545 \t0.117",
            "0.447 \t0.614 \t0.000",
            "1.804 \t1.872 \t0.000",
            "7.446 \t6.171 \t0.000",
            "8.720 \t6.258 \t0.000",
            "7.049 \t4.125 \t0.000",
            "25.676 \t12.943 \t0.000",
            "12.406 \t6.108 \t0.000",
            "3.296 \t1.076 \t0.000",
            "0.993 \t0.680 \t0.000",
            "0.381 \t0.100 \t0.000",
            "0.323 \t0.086 \t0.000",
            "0.092 \t0.030 \t0.000",
            "0.000 \t0.021 \t0.000",
            "0.004 \t0.000 \t0.000",
            "0.056 \t0.020 \t0.000"
        };

        public static string[] XYZfactor31_4() => new string[31]
        {
            "0.030 \t0.000 \t0.189",
            "0.094 \t0.000 \t0.115",
            "0.074 \t0.000 \t1.601",
            "0.801 \t0.000 \t2.971",
            "1.056 \t0.147 \t4.606",
            "0.889 \t0.063 \t5.500",
            "1.082 \t0.225 \t5.868",
            "0.666 \t0.319 \t5.146",
            "0.564 \t0.676 \t3.567",
            "0.089 \t1.017 \t2.352",
            "0.068 \t1.792 \t1.564",
            "0.001 \t3.109 \t0.892",
            "0.452 \t4.722 \t0.593",
            "1.190 \t6.348 \t0.279",
            "2.373 \t7.589 \t0.150",
            "3.663 \t8.589 \t0.098",
            "5.558 \t9.195 \t0.015",
            "7.571 \t9.464 \t0.043",
            "9.684 \t9.241 \t0.000",
            "11.607 \t8.490 \t0.024",
            "12.677 \t7.618 \t0.000",
            "12.820 \t6.334 \t0.012",
            "10.935 \t4.970 \t0.001",
            "9.498 \t3.883 \t0.000",
            "6.719 \t2.567 \t0.000",
            "3.396 \t1.362 \t0.000",
            "3.070 \t1.103 \t0.000",
            "2.233 \t0.819 \t0.000",
            "0.075 \t0.021 \t0.000",
            "0.001 \t0.001 \t0.000",
            "0.913 \t0.334 \t0.000"
        };

        public static string[] XYZfactor31_5() => new string[31]
        {
            "0.014 \t0.000 \t0.116",
            "0.134 \t0.024 \t0.617",
            "0.445 \t0.000 \t1.786",
            "0.446 \t0.121 \t3.349",
            "1.314 \t0.152 \t5.043",
            "0.902 \t0.284 \t5.741",
            "1.008 \t0.400 \t5.786",
            "0.671 \t0.661 \t4.954",
            "0.486 \t1.150 \t3.327",
            "0.000 \t1.580 \t1.923",
            "0.048 \t2.418 \t1.168",
            "0.180 \t3.553 \t0.675",
            "0.761 \t4.793 \t0.362",
            "1.632 \t6.142 \t0.219",
            "2.880 \t7.239 \t0.104",
            "4.314 \t8.114 \t0.028",
            "6.211 \t8.755 \t0.002",
            "8.235 \t8.987 \t0.000",
            "10.252 \t8.771 \t0.000",
            "11.924 \t8.270 \t0.000",
            "12.737 \t7.517 \t0.000",
            "12.463 \t6.301 \t0.000",
            "10.481 \t4.966 \t0.000",
            "8.945 \t3.882 \t0.000",
            "6.171 \t2.510 \t0.000",
            "3.090 \t1.323 \t0.000",
            "2.696 \t1.025 \t0.000",
            "1.905 \t0.750 \t0.000",
            "0.047 \t0.009 \t0.000",
            "0.056 \t0.037 \t0.000",
            "0.696 \t0.265 \t0.000"
        };

        public static string[] XYZfactor31_6() => new string[31]
        {
            "0.099 \t0.005 \t0.397",
            "0.128 \t0.000 \t0.780",
            "0.190 \t0.000 \t0.739",
            "2.960 \t0.144 \t14.651",
            "3.703 \t0.218 \t18.230",
            "1.615 \t0.199 \t8.874",
            "1.177 \t0.258 \t6.649",
            "1.166 \t0.448 \t7.362",
            "0.462 \t0.807 \t4.419",
            "0.112 \t1.020 \t1.803",
            "0.095 \t1.555 \t1.927",
            "0.000 \t2.579 \t0.437",
            "0.328 \t3.324 \t0.522",
            "0.796 \t4.447 \t0.178",
            "2.919 \t9.300 \t0.180",
            "5.324 \t12.176 \t0.151",
            "6.406 \t11.097 \t0.000",
            "10.121 \t12.229 \t0.038",
            "13.467 \t12.954 \t0.035",
            "13.123 \t9.752 \t0.000",
            "11.943 \t6.925 \t0.021",
            "9.516 \t4.939 \t0.000",
            "6.309 \t2.774 \t0.000",
            "3.784 \t1.451 \t0.000",
            "1.913 \t0.864 \t0.000",
            "0.861 \t0.314 \t0.000",
            "0.469 \t0.139 \t0.000",
            "0.081 \t0.026 \t0.000",
            "0.078 \t0.051 \t0.000",
            "0.032 \t0.005 \t0.000",
            "0.009 \t0.000 \t0.000"
        };

        public static string[] XYZfactor31_7() => new string[31]
        {
            "0.107 \t0.006 \t0.481",
            "0.319 \t0.000 \t1.638",
            "0.392 \t0.216 \t1.067",
            "2.964 \t0.320 \t15.624",
            "3.792 \t0.494 \t19.365",
            "1.862 \t0.412 \t9.211",
            "1.035 \t0.494 \t6.665",
            "1.030 \t0.946 \t7.065",
            "0.509 \t1.443 \t3.949",
            "0.000 \t1.551 \t1.548",
            "0.002 \t2.034 \t1.488",
            "0.209 \t3.014 \t0.244",
            "0.537 \t3.291 \t0.404",
            "1.054 \t4.272 \t0.086",
            "3.638 \t8.989 \t0.167",
            "6.080 \t11.391 \t0.022",
            "7.272 \t10.479 \t0.000",
            "10.953 \t11.700 \t0.000",
            "14.126 \t12.197 \t0.001",
            "13.571 \t9.446 \t0.000",
            "11.845 \t6.854 \t0.000",
            "9.319 \t4.847 \t0.000",
            "5.990 \t2.859 \t0.000",
            "3.547 \t1.384 \t0.000",
            "1.802 \t0.902 \t0.000",
            "0.731 \t0.343 \t0.000",
            "0.395 \t0.000 \t0.000",
            "0.117 \t0.007 \t0.000",
            "0.057 \t0.111 \t0.000",
            "0.001 \t0.000 \t0.000",
            "0.023 \t0.000 \t0.000"
        };

        public static string[] XYZfactor31_8() => new string[31]
        {
            "0.114 \t0.011 \t0.535",
            "0.150 \t0.000 \t0.662",
            "0.801 \t0.003 \t4.186",
            "1.634 \t0.092 \t7.708",
            "2.450 \t0.164 \t12.137",
            "2.813 \t0.302 \t14.810",
            "2.467 \t0.525 \t14.326",
            "1.681 \t0.797 \t11.349",
            "0.925 \t1.216 \t7.220",
            "0.240 \t1.868 \t4.084",
            "0.069 \t2.946 \t2.618",
            "0.058 \t4.599 \t1.329",
            "0.609 \t6.627 \t0.790",
            "1.583 \t8.278 \t0.402",
            "2.804 \t9.224 \t0.155",
            "4.224 \t9.619 \t0.124",
            "5.608 \t9.481 \t0.011",
            "7.200 \t8.936 \t0.048",
            "8.485 \t8.049 \t0.000",
            "9.259 \t6.890 \t0.006",
            "9.877 \t5.798 \t0.015",
            "9.452 \t4.770 \t0.001",
            "7.908 \t3.558 \t0.002",
            "6.131 \t2.483 \t0.000",
            "4.100 \t1.599 \t0.000",
            "2.413 \t0.958 \t0.000",
            "1.660 \t0.596 \t0.000",
            "1.092 \t0.353 \t0.000",
            "0.212 \t0.122 \t0.000",
            "0.018 \t0.000 \t0.000",
            "0.385 \t0.134 \t0.000"
        };

        public static string[] XYZfactor31_9() => new string[31]
        {
            "0.083 \t0.015 \t0.405",
            "0.448 \t0.027 \t1.912",
            "0.980 \t0.130 \t5.064",
            "1.683 \t0.188 \t8.155",
            "2.606 \t0.417 \t12.804",
            "2.731 \t0.678 \t15.135",
            "2.372 \t0.991 \t13.854",
            "1.594 \t1.517 \t10.514",
            "0.708 \t2.093 \t6.514",
            "0.079 \t2.779 \t3.327",
            "0.073 \t3.850 \t1.845",
            "0.297 \t5.151 \t0.965",
            "1.005 \t6.499 \t0.502",
            "2.120 \t7.800 \t0.270",
            "3.315 \t8.531 \t0.131",
            "4.779 \t8.856 \t0.029",
            "6.149 \t8.762 \t0.000",
            "7.584 \t8.227 \t0.000",
            "8.714 \t7.451 \t0.000",
            "9.279 \t6.491 \t0.000",
            "9.594 \t5.580 \t0.000",
            "8.973 \t4.615 \t0.000",
            "7.337 \t3.426 \t0.000",
            "5.589 \t2.437 \t0.000",
            "3.717 \t1.526 \t0.000",
            "2.106 \t0.861 \t0.000",
            "1.393 \t0.572 \t0.000",
            "0.909 \t0.329 \t0.000",
            "0.208 \t0.069 \t0.000",
            "0.000 \t0.028 \t0.000",
            "0.295 \t0.104 \t0.000"
        };

        public static string[] XYZfactor31_10() => new string[31]
        {
            "0.193 \t0.006 \t0.693",
            "0.000 \t0.000 \t0.702",
            "1.317 \t0.035 \t5.173",
            "1.618 \t0.105 \t8.890",
            "2.816 \t0.101 \t13.863",
            "3.231 \t0.384 \t16.693",
            "2.724 \t0.723 \t15.880",
            "1.845 \t0.604 \t12.377",
            "0.969 \t1.540 \t7.782",
            "0.252 \t1.898 \t4.380",
            "0.101 \t3.085 \t2.753",
            "0.059 \t4.842 \t1.365",
            "0.597 \t6.782 \t0.834",
            "1.648 \t8.436 \t0.391",
            "2.828 \t9.380 \t0.181",
            "4.230 \t9.658 \t0.090",
            "5.636 \t9.460 \t0.043",
            "7.145 \t8.905 \t0.027",
            "8.352 \t7.933 \t0.014",
            "9.105 \t6.737 \t0.003",
            "9.483 \t5.601 \t0.009",
            "9.064 \t4.575 \t0.005",
            "7.597 \t3.385 \t0.000",
            "5.691 \t2.321 \t0.000",
            "3.794 \t1.503 \t0.000",
            "2.361 \t0.902 \t0.000",
            "1.548 \t0.555 \t0.000",
            "0.844 \t0.291 \t0.000",
            "0.199 \t0.134 \t0.000",
            "0.165 \t0.000 \t0.000",
            "0.270 \t0.117 \t0.000"
        };

        public static string[] XYZfactor31_11() => new string[31]
        {
            "0.113 \t0.000 \t0.509",
            "0.491 \t0.075 \t2.293",
            "1.280 \t0.130 \t6.021",
            "1.854 \t0.209 \t9.522",
            "2.970 \t0.489 \t14.525",
            "3.094 \t0.693 \t16.906",
            "2.590 \t1.258 \t15.325",
            "1.729 \t1.439 \t11.455",
            "0.798 \t2.424 \t6.925",
            "0.045 \t2.856 \t3.546",
            "0.081 \t4.049 \t1.989",
            "0.334 \t5.350 \t0.928",
            "1.006 \t6.645 \t0.566",
            "2.152 \t7.916 \t0.254",
            "3.358 \t8.611 \t0.127",
            "4.769 \t8.860 \t0.034",
            "6.112 \t8.706 \t0.000",
            "7.520 \t8.153 \t0.000",
            "8.554 \t7.305 \t0.000",
            "9.012 \t6.328 \t0.000",
            "9.252 \t5.361 \t0.000",
            "8.531 \t4.392 \t0.000",
            "6.950 \t3.261 \t0.000",
            "5.253 \t2.269 \t0.000",
            "3.453 \t1.428 \t0.000",
            "1.941 \t0.798 \t0.000",
            "1.281 \t0.512 \t0.000",
            "0.816 \t0.304 \t0.000",
            "0.195 \t0.079 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.265 \t0.105 \t0.000"
        };

        public static string[] XYZfactor31_12() => new string[31]
        {
            "0.301 \t0.010 \t1.289",
            "0.073 \t0.004 \t0.740",
            "1.922 \t0.039 \t8.707",
            "2.510 \t0.167 \t12.689",
            "3.954 \t0.164 \t19.586",
            "4.238 \t0.468 \t22.275",
            "3.605 \t0.977 \t20.859",
            "2.345 \t0.715 \t15.547",
            "1.117 \t1.964 \t9.270",
            "0.335 \t2.151 \t5.311",
            "0.144 \t3.589 \t3.298",
            "0.000 \t5.376 \t1.258",
            "0.693 \t7.266 \t1.125",
            "1.724 \t8.860 \t0.267",
            "2.859 \t9.613 \t0.246",
            "4.307 \t9.730 \t0.051",
            "5.542 \t9.341 \t0.069",
            "6.920 \t8.632 \t0.035",
            "7.995 \t7.575 \t0.000",
            "8.501 \t6.295 \t0.005",
            "8.669 \t5.126 \t0.011",
            "8.108 \t4.090 \t0.000",
            "6.621 \t2.952 \t0.000",
            "4.955 \t2.006 \t0.000",
            "3.185 \t1.278 \t0.000",
            "1.880 \t0.718 \t0.000",
            "1.283 \t0.462 \t0.000",
            "0.714 \t0.227 \t0.000",
            "0.167 \t0.116 \t0.000",
            "0.060 \t0.000 \t0.000",
            "0.243 \t0.088 \t0.000"
        };

        public static string[] XYZfactor31_13() => new string[31]
        {
            "0.222 \t0.040 \t0.885",
            "0.688 \t0.040 \t3.474",
            "2.100 \t0.212 \t9.507",
            "2.666 \t0.392 \t13.720",
            "4.039 \t0.604 \t20.198",
            "4.071 \t0.922 \t22.236",
            "3.452 \t1.647 \t19.701",
            "2.095 \t1.742 \t14.172",
            "0.839 \t2.941 \t8.309",
            "0.177 \t3.283 \t4.174",
            "0.089 \t4.648 \t2.235",
            "0.287 \t5.781 \t0.975",
            "1.144 \t7.089 \t0.662",
            "2.186 \t8.154 \t0.211",
            "3.389 \t8.681 \t0.151",
            "4.742 \t8.844 \t0.028",
            "5.952 \t8.406 \t0.001",
            "7.174 \t7.844 \t0.000",
            "8.042 \t6.843 \t0.000",
            "8.324 \t5.834 \t0.000",
            "8.310 \t4.830 \t0.000",
            "7.516 \t3.877 \t0.000",
            "6.003 \t2.789 \t0.000",
            "4.473 \t1.942 \t0.000",
            "2.803 \t1.173 \t0.000",
            "1.605 \t0.656 \t0.000",
            "1.084 \t0.445 \t0.000",
            "0.552 \t0.154 \t0.000",
            "0.182 \t0.105 \t0.000",
            "0.024 \t0.036 \t0.000",
            "0.187 \t0.047 \t0.000"
        };

        public static string[] XYZfactor31_14() => new string[31]
        {
            "0.151 \t0.000 \t0.780",
            "0.199 \t0.019 \t0.613",
            "1.372 \t0.000 \t7.550",
            "2.967 \t0.182 \t13.620",
            "3.946 \t0.241 \t19.718",
            "3.956 \t0.378 \t21.192",
            "3.341 \t0.711 \t19.126",
            "2.285 \t1.366 \t15.020",
            "1.118 \t0.954 \t9.483",
            "0.334 \t3.052 \t5.151",
            "0.125 \t2.994 \t3.136",
            "0.000 \t4.967 \t1.267",
            "0.635 \t6.476 \t0.882",
            "1.497 \t7.862 \t0.298",
            "2.801 \t9.221 \t0.218",
            "4.271 \t9.774 \t0.093",
            "5.877 \t9.856 \t0.009",
            "7.342 \t9.150 \t0.058",
            "8.392 \t7.985 \t0.000",
            "8.987 \t6.633 \t0.005",
            "8.970 \t5.322 \t0.013",
            "8.341 \t4.162 \t0.000",
            "6.949 \t3.162 \t0.000",
            "5.488 \t2.215 \t0.000",
            "3.697 \t1.417 \t0.000",
            "2.106 \t0.837 \t0.000",
            "1.517 \t0.563 \t0.000",
            "0.850 \t0.322 \t0.000",
            "0.232 \t0.016 \t0.000",
            "0.030 \t0.075 \t0.000",
            "0.296 \t0.090 \t0.000"
        };

        public static string[] XYZfactor31_15() => new string[31]
        {
            "0.099 \t0.000 \t0.531",
            "0.631 \t0.091 \t2.558",
            "1.614 \t0.100 \t8.421",
            ";3.078 \t0.431 \t14.624",
            "3.996 \t0.677 \t20.379",
            "3.873 \t0.882 \t21.063",
            "3.163 \t1.280 \t18.304",
            "2.059 \t2.267 \t13.790",
            "0.890 \t2.118 \t8.302",
            "0.117 \t4.123 \t4.110",
            "0.081 \t3.958 \t2.249",
            "0.299 \t5.482 \t0.871",
            "0.991 \t6.236 \t0.564",
            "1.946 \t7.279 \t0.216",
            "3.302 \t8.408 \t0.131",
            "4.724 \t8.835 \t0.032",
            "6.342 \t8.965 \t0.000",
            "7.632 \t8.290 \t0.000",
            "8.454 \t7.268 \t0.000",
            "8.858 \t6.150 \t0.000",
            "8.594 \t5.036 \t0.000",
            "7.763 \t3.963 \t0.000",
            "6.350 \t3.003 \t0.000",
            "4.948 \t2.114 \t0.000",
            "3.267 \t1.333 \t0.000",
            "1.765 \t0.776 \t0.000",
            "1.333 \t0.532 \t0.000",
            "0.702 \t0.197 \t0.000",
            "0.137 \t0.118 \t0.000",
            "0.052 \t0.000 \t0.000",
            "0.225 \t0.088 \t0.000"
        };

        public static string[] XYZfactor31_16() => new string[31]
        {
            "0.095 \t0.006 \t0.322",
            "0.011 \t0.000 \t0.253",
            "0.000 \t0.028 \t0.000",
            "2.281 \t0.028 \t11.135",
            "2.913 \t0.233 \t14.565",
            "0.409 \t0.000 \t1.008",
            "0.037 \t0.091 \t2.880",
            "0.607 \t0.156 \t1.564",
            "0.154 \t0.412 \t2.307",
            "0.216 \t1.373 \t3.246",
            "0.000 \t0.854 \t0.667",
            "0.000 \t0.237 \t0.107",
            "0.000 \t0.101 \t0.006",
            "0.000 \t0.000 \t0.027",
            "7.811 \t24.809 \t0.452",
            "9.356 \t22.019 \t0.242",
            "0.860 \t1.776 \t0.000",
            "1.462 \t1.513 \t0.000",
            "8.283 \t7.895 \t0.020",
            "8.728 \t6.296 \t0.000",
            "9.405 \t6.040 \t0.017",
            "33.750 \t16.173 \t0.011",
            "15.797 \t7.784 \t0.000",
            "3.376 \t1.063 \t0.000",
            "1.820 \t0.603 \t0.000",
            "0.542 \t0.389 \t0.000",
            "0.085 \t0.030 \t0.000",
            "0.329 \t0.050 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.058 \t0.040 \t0.000"
        };

        public static string[] XYZfactor31_17() => new string[31]
        {
            "0.113 \t0.027 \t0.453",
            "0.146 \t0.000 \t0.719",
            "0.001 \t0.033 \t0.000",
            "2.365 \t0.203 \t11.988",
            "3.056 \t0.597 \t15.375",
            "0.465 \t0.000 \t1.208",
            "0.000 \t0.173 \t2.964",
            "0.599 \t0.242 \t1.412",
            "0.099 \t0.834 \t2.100",
            "0.119 \t2.052 \t2.889",
            "0.000 \t1.252 \t0.419",
            "0.000 \t0.197 \t0.114",
            "0.000 \t0.168 \t0.002",
            "0.000 \t0.000 \t0.000",
            "9.680 \t23.865 \t0.313",
            "10.935 \t21.169 \t0.097",
            "1.129 \t1.630 \t0.000",
            "1.451 \t1.501 \t0.000",
            "8.883 \t7.582 \t0.000",
            "9.141 \t6.152 \t0.000",
            "9.443 \t6.064 \t0.000",
            "33.243 \t16.243 \t0.000",
            "15.297 \t7.837 \t0.000",
            "3.134 \t1.087 \t0.000",
            "1.794 \t0.604 \t0.000",
            "0.495 \t0.402 \t0.000",
            "0.060 \t0.015 \t0.000",
            "0.277 \t0.036 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.049 \t0.036 \t0.000"
        };

        public static string[] XYZfactor31_18() => new string[31]
        {
            "0.081 \t0.006 \t0.284",
            "0.043 \t0.000 \t0.448",
            "0.298 \t0.036 \t0.971",
            "3.292 \t0.033 \t17.105",
            "4.603 \t0.461 \t21.531",
            "2.267 \t0.083 \t12.704",
            "1.512 \t0.462 \t9.433",
            "1.282 \t0.382 \t7.213",
            "0.434 \t1.014 \t4.563",
            "0.343 \t1.854 \t4.420",
            "0.000 \t1.376 \t1.411",
            "0.000 \t0.562 \t0.000",
            "0.000 \t0.235 \t0.095",
            "0.000 \t0.000 \t0.011",
            "9.282 \t29.658 \t0.560",
            "11.436 \t26.268 \t0.271",
            "0.184 \t1.556 \t0.005",
            "1.395 \t0.647 \t0.001",
            "7.127 \t7.307 \t0.004",
            "7.757 \t5.257 \t0.011",
            "6.471 \t4.467 \t0.012",
            "22.567 \t10.593 \t0.004",
            "12.075 \t5.935 \t0.000",
            "2.983 \t1.104 \t0.000",
            "1.150 \t0.265 \t0.000",
            "0.373 \t0.277 \t0.000",
            "0.259 \t0.130 \t0.000",
            "0.163 \t0.000 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.058 \t0.033 \t0.000"
        };

        public static string[] XYZfactor31_19() => new string[31]
        {
            "0.101 \t0.009 \t0.324",
            "0.203 \t0.025 \t1.342",
            "0.349 \t0.067 \t0.667",
            "3.461 \t0.323 \t18.746",
            "4.754 \t0.888 \t22.716",
            "2.286 \t0.472 \t12.780",
            "1.539 \t0.738 \t9.975",
            "1.255 \t0.781 \t6.630",
            "0.193 \t1.787 \t3.805",
            "0.275 \t2.803 \t4.222",
            "0.000 \t1.881 \t0.727",
            "0.000 \t0.615 \t0.135",
            "0.000 \t0.273 \t0.032",
            "0.000 \t0.000 \t0.000",
            "11.359 \t28.052 \t0.355",
            "13.016 \t24.795 \t0.123",
            "0.488 \t1.357 \t0.000",
            "1.263 \t0.725 \t0.000",
            "7.608 \t6.788 \t0.000",
            "7.842 \t5.188 \t0.000",
            "6.541 \t4.259 \t0.000",
            "21.692 \t10.547 \t0.000",
            "11.535 \t5.873 \t0.000",
            "2.794 \t1.026 \t0.000",
            "1.065 \t0.305 \t0.000",
            "0.331 \t0.322 \t0.000",
            "0.206 \t0.066 \t0.000",
            "0.154 \t0.003 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.042 \t0.033 \t0.000"
        };

        public static string[] XYZfactor31_20() => new string[31]
        {
            "0.033 \t0.000 \t0.048",
            "0.000 \t0.000 \t0.238",
            "0.015 \t0.000 \t0.375",
            "0.202 \t0.000 \t1.849",
            "1.138 \t0.038 \t2.481",
            "0.186 \t0.145 \t2.639",
            "0.561 \t0.022 \t3.875",
            "0.418 \t0.231 \t3.167",
            "0.495 \t0.505 \t1.931",
            "0.000 \t0.575 \t2.093",
            "0.000 \t1.444 \t0.907",
            "0.050 \t2.261 \t0.689",
            "0.366 \t3.854 \t0.557",
            "0.911 \t5.304 \t0.141",
            "2.296 \t6.804 \t0.216",
            "3.272 \t7.889 \t0.014",
            "5.074 \t8.702 \t0.078",
            "8.052 \t9.380 \t0.001",
            "8.961 \t9.341 \t0.024",
            "13.466 \t9.176 \t0.014",
            "12.918 \t8.297 \t0.011",
            "15.915 \t7.485 \t0.003",
            "12.557 \t5.894 \t0.001",
            "11.281 \t4.692 \t0.000",
            "8.559 \t3.172 \t0.000",
            "4.796 \t1.775 \t0.002",
            "3.432 \t1.422 \t0.001",
            "2.889 \t1.081 \t0.000",
            "0.458 \t0.020 \t0.000",
            "0.000 \t0.030 \t0.000",
            "1.181 \t0.459 \t0.001"
        };

        public static string[] XYZfactor31_21() => new string[31]
        {
            "0.015 \t0.000 \t0.065",
            "0.077 \t0.000 \t0.272",
            "0.088 \t0.000 \t0.930",
            "0.210 \t0.000 \t1.663",
            "1.093 \t0.207 \t2.698",
            "0.206 \t0.138 \t3.164",
            "0.634 \t0.136 \t3.551",
            "0.415 \t0.467 \t3.057",
            "0.346 \t0.877 \t2.078",
            "0.000 \t0.903 \t1.560",
            "0.000 \t1.921 \t0.674",
            "0.145 \t2.659 \t0.626",
            "0.683 \t3.971 \t0.240",
            "1.264 \t5.111 \t0.214",
            "2.747 \t6.596 \t0.076",
            "3.923 \t7.589 \t0.036",
            "5.786 \t8.275 \t0.000",
            "8.751 \t8.989 \t0.000",
            "9.667 \t9.086 \t0.000",
            "13.947 \t8.859 \t0.000",
            "13.075 \t8.387 \t0.000",
            "15.684 \t7.503 \t0.000",
            "12.121 \t5.912 \t0.000",
            "10.753 \t4.707 \t0.000",
            "8.006 \t3.233 \t0.000",
            "4.287 \t1.690 \t0.000",
            "3.030 \t1.278 \t0.000",
            "2.618 \t1.064 \t0.000",
            "0.325 \t0.055 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.949 \t0.387 \t0.000"
        };

        public static string[] XYZfactor31_22() => new string[31]
        {
            "0.114 \t0.005 \t0.613",
            "0.286 \t0.009 \t1.114",
            "0.388 \t0.002 \t2.336",
            "4.128 \t0.199 \t19.949",
            "5.221 \t0.297 \t25.806",
            "2.871 \t0.339 \t15.416",
            "2.251 \t0.535 \t13.191",
            "2.035 \t0.835 \t12.988",
            "0.902 \t1.289 \t7.713",
            "0.223 \t2.035 \t3.938",
            "0.115 \t2.928 \t3.061",
            "0.043 \t4.512 \t0.982",
            "0.578 \t6.209 \t0.968",
            "1.243 \t6.859 \t0.173",
            "3.669 \t11.441 \t0.315",
            "5.497 \t12.969 \t0.102",
            "4.943 \t8.362 \t0.014",
            "7.155 \t8.699 \t0.034",
            "9.568 \t9.238 \t0.008",
            "8.960 \t6.567 \t0.013",
            "8.733 \t5.170 \t0.010",
            "8.064 \t4.083 \t0.000",
            "6.366 \t2.817 \t0.001",
            "4.778 \t1.939 \t0.000",
            "3.198 \t1.352 \t0.000",
            "1.777 \t0.575 \t0.000",
            "1.146 \t0.416 \t0.000",
            "0.557 \t0.270 \t0.000",
            "0.083 \t0.000 \t0.000",
            "0.056 \t0.016 \t0.000",
            "0.094 \t0.036 \t0.000"
        };

        public static string[] XYZfactor31_23() => new string[31]
        {
            "0.130 \t0.030 \t0.624",
            "0.570 \t0.000 \t2.521",
            "0.577 \t0.165 \t2.808",
            "4.194 \t0.480 \t20.814",
            "5.249 \t0.738 \t26.701",
            "2.855 \t0.805 \t15.488",
            "2.202 \t0.965 \t12.767",
            "1.843 \t1.531 \t12.088",
            "0.664 \t2.342 \t6.748",
            "0.112 \t2.920 \t3.221",
            "0.077 \t3.749 \t2.292",
            "0.280 \t5.197 \t0.562",
            "0.963 \t5.906 \t0.749",
            "1.656 \t6.474 \t0.039",
            "4.302 \t10.654 \t0.259",
            "6.222 \t11.624 \t0.000",
            "5.354 \t7.960 \t0.002",
            "7.553 \t7.814 \t0.002",
            "9.728 \t8.517 \t0.002",
            "8.934 \t6.248 \t0.000",
            "8.484 \t4.816 \t0.000",
            "7.572 \t4.021 \t0.000",
            "5.893 \t2.759 \t0.000",
            "4.369 \t1.709 \t0.000",
            "2.833 \t1.346 \t0.000",
            "1.555 \t0.606 \t0.000",
            "0.983 \t0.439 \t0.000",
            "0.432 \t0.055 \t0.000",
            "0.103 \t0.065 \t0.000",
            "0.021 \t0.067 \t0.000",
            "0.081 \t0.000 \t0.000"
        };

        public static string[] XYZfactor31_24() => new string[31]
        {
            "0.092 \t0.020 \t0.286",
            "0.000 \t0.000 \t0.124",
            "0.062 \t0.000 \t0.000",
            "1.746 \t0.039 \t9.676",
            "2.973 \t0.224 \t13.227",
            "0.631 \t0.000 \t4.194",
            "0.000 \t0.075 \t0.912",
            "0.627 \t0.222 \t2.249",
            "0.118 \t0.357 \t2.380",
            "0.234 \t1.406 \t2.930",
            "0.000 \t1.098 \t1.152",
            "0.000 \t0.210 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.000 \t0.000 \t0.001",
            "6.832 \t21.577 \t0.453",
            "10.316 \t24.433 \t0.214",
            "1.274 \t2.151 \t0.000",
            "0.162 \t0.193 \t0.018",
            "9.417 \t8.669 \t0.001",
            "7.723 \t5.858 \t0.009",
            "10.027 \t6.159 \t0.010",
            "32.057 \t15.518 \t0.013",
            "20.151 \t9.545 \t0.000",
            "3.877 \t1.396 \t0.000",
            "0.576 \t0.073 \t0.000",
            "1.093 \t0.635 \t0.000",
            "0.521 \t0.098 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.113 \t0.047 \t0.000"
        };

        public static string[] XYZfactor31_25() => new string[31]
        {
            "0.137 \t0.025 \t0.368",
            "0.022 \t0.000 \t0.544",
            "0.153 \t0.000 \t0.000",
            "1.837 \t0.246 \t10.183",
            "3.053 \t0.446 \t14.705",
            "0.702 \t0.201 \t3.882",
            "0.000 \t0.087 \t1.024",
            "0.597 \t0.180 \t2.463",
            "0.050 \t0.888 \t1.765",
            "0.142 \t2.237 \t2.800",
            "0.000 \t1.335 \t0.796",
            "0.000 \t0.295 \t0.000",
            "0.000 \t0.000 \t0.008",
            "0.000 \t0.000 \t0.000",
            "8.459 \t20.865 \t0.289",
            "12.029 \t23.165 \t0.091",
            "1.592 \t2.296 \t0.000",
            "0.112 \t0.014 \t0.000",
            "9.918 \t8.501 \t0.000",
            "8.260 \t5.468 \t0.000",
            "10.010 \t6.548 \t0.000",
            "31.452 \t15.242 \t0.000",
            "19.556 \t9.582 \t0.000",
            "3.656 \t1.714 \t0.000",
            "0.541 \t0.000 \t0.000",
            "1.037 \t0.389 \t0.000",
            "0.411 \t0.230 \t0.000",
            "0.027 \t0.022 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.089 \t0.026 \t0.000"
        };

        public static string[] XYZfactor31_26() => new string[31]
        {
            "0.033 \t0.002 \t0.118",
            "0.122 \t0.000 \t0.734",
            "0.076 \t0.000 \t0.000",
            "2.626 \t0.111 \t13.202",
            "3.853 \t0.260 \t19.425",
            "1.767 \t0.218 \t8.198",
            "1.014 \t0.124 \t7.411",
            "0.809 \t0.400 \t4.705",
            "0.559 \t0.978 \t4.210",
            "0.250 \t1.473 \t4.263",
            "0.000 \t1.487 \t1.102",
            "0.000 \t0.412 \t0.115",
            "0.000 \t0.012 \t0.015",
            "0.000 \t0.612 \t0.000",
            "8.576 \t26.408 \t0.595",
            "11.068 \t26.563 \t0.215",
            "0.547 \t1.059 \t0.000",
            "1.252 \t1.294 \t0.000",
            "6.736 \t6.326 \t0.000",
            "8.558 \t6.448 \t0.033",
            "6.396 \t3.777 \t0.003",
            "26.862 \t13.318 \t0.000",
            "13.987 \t6.515 \t0.000",
            "3.920 \t1.230 \t0.000",
            "1.028 \t0.778 \t0.000",
            "0.310 \t0.000 \t0.000",
            "0.394 \t0.056 \t0.000",
            "0.157 \t0.110 \t0.000",
            "0.000 \t0.018 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.060 \t0.012 \t0.007"
        };

        public static string[] XYZfactor31_27() => new string[31]
        {
            "0.052 \t0.000 \t0.184",
            "0.258 \t0.018 \t1.325",
            "0.076 \t0.024 \t0.000",
            "2.778 \t0.351 \t14.156",
            "4.020 \t0.607 \t20.595",
            "1.786 \t0.522 \t8.835",
            "1.025 \t0.303 \t7.032",
            "0.801 \t0.682 \t4.657",
            "0.364 \t1.681 \t3.919",
            "0.171 \t2.384 \t3.470",
            "0.000 \t1.965 \t0.886",
            "0.000 \t0.424 \t0.073",
            "0.000 \t0.111 \t0.000",
            "0.000 \t0.504 \t0.000",
            "10.552 \t25.203 \t0.411",
            "12.700 \t25.166 \t0.059",
            "0.809 \t0.889 \t0.000",
            "1.235 \t1.354 \t0.000",
            "7.100 \t5.935 \t0.000",
            "8.873 \t6.227 \t0.001",
            "6.328 \t3.834 \t0.000",
            "26.123 \t13.103 \t0.000",
            "13.372 \t6.539 \t0.000",
            "3.679 \t1.271 \t0.000",
            "0.949 \t0.697 \t0.000",
            "0.245 \t0.010 \t0.000",
            "0.462 \t0.084 \t0.000",
            "0.035 \t0.096 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.025 \t0.000 \t0.000",
            "0.046 \t0.015 \t0.003"
        };

        public static string[] XYZfactor31_28() => new string[31]
        {
            "0.195 \t0.000 \t0.902",
            "0.141 \t0.012 \t0.712",
            "1.543 \t0.012 \t7.417",
            "2.390 \t0.154 \t12.023",
            "3.565 \t0.191 \t16.968",
            "3.427 \t0.352 \t18.535",
            "2.887 \t0.821 \t17.021",
            "2.075 \t0.561 \t13.239",
            "1.055 \t1.810 \t8.429",
            "0.222 \t1.943 \t4.787",
            "0.103 \t3.334 \t2.933",
            "0.103 \t5.069 \t1.375",
            "0.575 \t6.719 \t0.878",
            "1.579 \t8.182 \t0.338",
            "2.936 \t9.661 \t0.223",
            "4.757 \t10.838 \t0.121",
            "6.237 \t10.517 \t0.000",
            "7.255 \t9.009 \t0.034",
            "7.682 \t7.319 \t0.023",
            "8.264 \t6.121 \t0.000",
            "8.937 \t5.261 \t0.012",
            "8.498 \t4.284 \t0.000",
            "6.834 \t3.076 \t0.000",
            "4.965 \t2.002 \t0.000",
            "3.004 \t1.185 \t0.000",
            "1.632 \t0.630 \t0.000",
            "1.238 \t0.461 \t0.000",
            "0.825 \t0.273 \t0.000",
            "0.135 \t0.074 \t0.000",
            "0.000 \t0.000 \t0.000",
            "0.367 \t0.128 \t0.001"
        };

        public static string[] XYZfactor31_29() => new string[31]
        {
            "0.164 \t0.013 \t0.677",
            "0.566 \t0.064 \t2.761",
            "1.771 \t0.192 \t8.148",
            "2.573 \t0.335 \t13.216",
            "3.595 \t0.491 \t17.747",
            "3.364 \t0.826 \t18.341",
            "2.803 \t1.425 \t16.454",
            "1.844 \t1.337 \t12.225",
            "0.811 \t2.778 \t7.436",
            "0.092 \t3.018 \t3.817",
            "0.085 \t4.299 \t2.137",
            "0.315 \t5.567 \t0.933",
            "1.027 \t6.565 \t0.595",
            "2.047 \t7.618 \t0.213",
            "3.462 \t8.845 \t0.153",
            "5.306 \t9.865 \t0.026",
            "6.779 \t9.623 \t0.002",
            "7.555 \t8.234 \t0.000",
            "7.823 \t6.671 \t0.000",
            "8.197 \t5.731 \t0.000",
            "8.575 \t5.010 \t0.000",
            "7.997 \t4.086 \t0.000",
            "6.284 \t2.951 \t0.000",
            "4.470 \t1.913 \t0.000",
            "2.691 \t1.131 \t0.000",
            "1.420 \t0.609 \t0.000",
            "1.037 \t0.398 \t0.000",
            "0.667 \t0.194 \t0.000",
            "0.113 \t0.118 \t0.000",
            "0.036 \t0.000 \t0.000",
            "0.253 \t0.091 \t0.000"
        };

        public static string[] XYZfactor31_30() => new string[31]
        {
            "0.222 \t0.020 \t1.047",
            "0.188 \t0.000 \t0.892",
            "1.772 \t0.000 \t8.553",
            "2.774 \t0.205 \t13.858",
            "4.054 \t0.229 \t19.507",
            "3.844 \t0.312 \t20.666",
            "3.201 \t0.971 \t18.835",
            "2.276 \t0.692 \t14.552",
            "1.133 \t1.827 \t9.163",
            "0.249 \t2.118 \t5.115",
            "0.114 \t3.568 \t3.064",
            "0.092 \t5.232 \t1.437",
            "0.600 \t6.901 \t0.941",
            "1.601 \t8.361 \t0.297",
            "3.023 \t9.876 \t0.249",
            "4.884 \t11.126 \t0.123",
            "6.326 \t10.728 \t0.001",
            "7.203 \t8.894 \t0.024",
            "7.361 \t7.041 \t0.032",
            "7.900 \t5.846 \t0.000",
            "8.439 \t4.950 \t0.003",
            "7.914 \t4.015 \t0.005",
            "6.346 \t2.841 \t0.000",
            "4.524 \t1.805 \t0.000",
            "2.669 \t1.067 \t0.000",
            "1.444 \t0.593 \t0.000",
            "1.111 \t0.394 \t0.000",
            "0.678 \t0.169 \t0.000",
            "0.120 \t0.114 \t0.000",
            "0.000 \t0.013 \t0.001",
            "0.320 \t0.089 \t0.000"
        };

        public static string[] XYZfactor31_31() => new string[31]
        {
            "0.216 \t0.031 \t0.816",
            "0.550 \t0.044 \t3.131",
            "2.287 \t0.243 \t9.632",
            "2.808 \t0.359 \t14.956",
            "3.952 \t0.644 \t20.183",
            "3.887 \t0.818 \t20.478",
            "3.087 \t1.545 \t18.019",
            "1.966 \t1.633 \t13.440",
            "0.887 \t2.810 \t7.987",
            "0.116 \t3.275 \t4.022",
            "0.086 \t4.553 \t2.276",
            "0.310 \t5.684 \t0.949",
            "1.050 \t6.769 \t0.631",
            "2.089 \t7.679 \t0.191",
            "3.505 \t9.010 \t0.162",
            "5.461 \t10.110 \t0.031",
            "6.811 \t9.689 \t0.000",
            "7.437 \t8.113 \t0.000",
            "7.503 \t6.402 \t0.000",
            "7.754 \t5.409 \t0.000",
            "8.058 \t4.697 \t0.000",
            "7.421 \t3.829 \t0.000",
            "5.783 \t2.670 \t0.000",
            "4.041 \t1.762 \t0.000",
            "2.386 \t1.029 \t0.000",
            "1.274 \t0.490 \t0.000",
            "0.889 \t0.325 \t0.000",
            "0.584 \t0.249 \t0.000",
            "0.000 \t0.030 \t0.000",
            "0.157 \t0.026 \t0.000",
            "0.169 \t0.073 \t0.000"
        };

        public static string[] XYZfactor31_32() => new string[31]
        {
            "0.046    0.001    0.21",
            "0.069    0.002    0.329",
            "0.013    -0.004   0.037",
            "1.992    0.099    9.826",
            "3.127    0.182    15.561",
            "0.802    0.102    4.292",
            "0.787    0.162    4.513",
            "0.437    0.165    2.813",
            "0.291    0.558    2.689",
            "0.332    2.051    4.649",
            "0.027    1.171    1.146",
            "0.004    0.395    0.067",
            "0.027    0.433    0.051",
            "-0.403   -0.642   0.022",
            "8.239    24.003   0.396",
            "9.980    25.105   0.293",
            "0.852    1.031    -0.010",
            "0.861    1.100    0.002",
            "8.226    7.487    0.014",
            "11.855   8.963    0.014",
            "4.757    2.967    0.004",
            "30.578   14.976   0.009",
            "16.121   7.445    0.004",
            "3.957    1.574    0.000",
            "0.768    0.297    0.000",
            "0.546    0.205    0.000",
            "0.249    0.092    0.000",
            "0.111    0.041    0.000",
            "0.044    0.016    0.000",
            "0.027    0.010    0.000",
            "0.009    0.003    0.000"
        };

        public static string[] XYZfactor31_33() => new string[31]
        {
            "0.079    0.008    0.356",
            "0.130    0.013    0.595",
            "0.073    0.001    0.300",
            "2.084    0.284    10.446",
            "3.254    0.486    16.517",
            "0.841    0.222    4.588",
            "0.782    0.329    4.514",
            "0.415    0.329    2.717",
            "0.220    0.962    2.415",
            "0.177    3.182    3.962",
            "0.011    1.622    0.895",
            "0.029    0.439    0.035",
            "0.057    0.446    0.037",
            "-0.451   -0.600   0.018",
            "9.951    22.935   0.237",
            "11.772   23.889   0.153",
            "0.937    0.978    -0.011",
            "0.948    1.045    0.000",
            "8.648    7.151    0.000",
            "12.293   8.722    0.000",
            "4.841    2.946    0.000",
            "29.743   14.942   0.000",
            "15.505   7.423    0.000",
            "3.766    1.601    0.000",
            "0.710    0.294    0.000",
            "0.488    0.194    0.000",
            "0.220    0.087    0.000",
            "0.098    0.038    0.000",
            "0.037    0.014    0.000",
            "0.022    0.009    0.000",
            "0.008    0.003    0.000"
        };
    }
}