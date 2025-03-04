using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using FO.CLS.UTIL;
using Application = System.Windows.Forms.Application;
using System.Security.Policy;
using System.IO;

namespace main
{
    public class clsHKLineScanCamera
    {
        public delegate void APPENDIMAGE(string path);

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        private MvLineScanCamera mvlc = new MvLineScanCamera();

        MvLineScanCamera.MV_CC_DEVICE_INFO_LIST stDevList = new MvLineScanCamera.MV_CC_DEVICE_INFO_LIST();

        // -----------------------------------------------
        // 이미지 저장

        private IntPtr m_ConvertDstBuf = IntPtr.Zero;
        private UInt32 m_nConvertDstBufLen = 0;
        private Bitmap scanTemp = null;
        public Bitmap scanImage = new Bitmap(10, 10);

        public string savePath = string.Empty;

        private bool bSaveImage = false;

        // -----------------------------------------------

        CancellationTokenSource cancellationTokenSource;

        Thread hReceiveImageThreadHandle = null;

        IntPtr m_BufForDriver = IntPtr.Zero;

        // -----------------------------------------------

        public float exposureTime = 0;

        public List<string> cameraInfoList = new List<string>();
        public List<string> cameraIpList = new List<string>();

        public string lastErrorMsg = string.Empty;

        public long imageWidth;

        public long imageHeight;

        public uint frameNo = 0;

        private PictureBox pictureBox = null;

        public bool connectedCamera = false;

        public float mmPerImage = 90;

        public float scanLength = 0;

        // -----------------------------------------------

        private APPENDIMAGE appendImage = null;

        public void setAppendImage(APPENDIMAGE _appendImage)
        {
            appendImage = _appendImage;
        }

        public List<string> findCamera()
        {
            MvLineScanCamera.MV_CC_DEVICE_INFO stDevInfo;

            int nRet = MvLineScanCamera.MV_CC_EnumDevices_NET(MvLineScanCamera.MV_GIGE_DEVICE
                                                            //| MvLineScanCamera.MV_USB_DEVICE
                                                            //| MvLineScanCamera.MV_GENTL_GIGE_DEVICE
                                                            //| MvLineScanCamera.MV_GENTL_CAMERALINK_DEVICE
                                                            //| MvLineScanCamera.MV_GENTL_CXP_DEVICE
                                                            //| MvLineScanCamera.MV_GENTL_XOF_DEVICE
                                                            , ref stDevList);
            if (0 != nRet)
            {
                lastErrorMsg = "can't find camera";
            }

            for (Int32 i = 0; i < stDevList.nDeviceNum; i++)
            {
                stDevInfo = (MvLineScanCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[i], typeof(MvLineScanCamera.MV_CC_DEVICE_INFO));

                if (MvLineScanCamera.MV_GIGE_DEVICE == stDevInfo.nTLayerType)
                {
                    MvLineScanCamera.MV_GIGE_DEVICE_INFO_EX stGigEDeviceInfo = (MvLineScanCamera.MV_GIGE_DEVICE_INFO_EX)MvLineScanCamera.ByteToStruct(stDevInfo.SpecialInfo.stGigEInfo, typeof(MvLineScanCamera.MV_GIGE_DEVICE_INFO_EX));

                    uint nIp1 = ((stGigEDeviceInfo.nCurrentIp & 0xff000000) >> 24);
                    uint nIp2 = ((stGigEDeviceInfo.nCurrentIp & 0x00ff0000) >> 16);
                    uint nIp3 = ((stGigEDeviceInfo.nCurrentIp & 0x0000ff00) >> 8);
                    uint nIp4 = (stGigEDeviceInfo.nCurrentIp & 0x000000ff);

                    string ip = nIp1 + "." + nIp2 + "." + nIp3 + "." + nIp4;

                    string t = "[" + i.ToString() + "] [" + ip + "] " + stGigEDeviceInfo.chModelName;

                    cameraIpList.Add(ip);
                    cameraInfoList.Add(t);
                }
                //else if (MvLineScanCamera.MV_USB_DEVICE == stDevInfo.nTLayerType)
                //{
                //    MvLineScanCamera.MV_USB3_DEVICE_INFO_EX stUsb3DeviceInfo = (MvLineScanCamera.MV_USB3_DEVICE_INFO_EX)MvLineScanCamera.ByteToStruct(stDevInfo.SpecialInfo.stUsb3VInfo, typeof(MvLineScanCamera.MV_USB3_DEVICE_INFO_EX));
                //    Console.WriteLine("[device " + i.ToString() + "]:");
                //    Console.WriteLine("SerialNumber:" + stUsb3DeviceInfo.chSerialNumber);
                //    Console.WriteLine("ModelName:" + stUsb3DeviceInfo.chModelName + "\n");
                //}
            }

            return cameraInfoList;
        }

        public bool connectCamera(string ip, bool triggerMode, int triggerSource)
        {
            int cameraIndex = -1;

            for (int i = 0; i < cameraIpList.Count; i++)
            {
                if (cameraIpList[i].IndexOf(ip) != -1)
                {
                    cameraIndex = i;
                    break;
                }
            }

            return connectCamera(cameraIndex, triggerMode, triggerSource);
        }

        public bool connectCamera(int cameraIndex, bool triggerMode, int triggerSource)
        {
            if (connectedCamera == false)
            {
                if (stDevList.nDeviceNum == 0 || cameraIndex > stDevList.nDeviceNum - 1 || cameraIndex < 0)
                {
                    lastErrorMsg = "camera no error";
                    return false;
                }

                MvLineScanCamera.MV_CC_DEVICE_INFO stDevInfo = (MvLineScanCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[cameraIndex], typeof(MvLineScanCamera.MV_CC_DEVICE_INFO));

                int nRet = mvlc.MV_CC_CreateDevice_NET(ref stDevInfo);
                if (MvLineScanCamera.MV_OK != nRet)
                {
                    lastErrorMsg = "Create device failed";
                    return false;
                }

                nRet = mvlc.MV_CC_OpenDevice_NET();
                if (MvLineScanCamera.MV_OK != nRet)
                {
                    mvlc.MV_CC_DestroyDevice_NET();

                    lastErrorMsg = "Open device failed";
                    return false;
                }

                if (stDevInfo.nTLayerType == MvLineScanCamera.MV_GIGE_DEVICE)
                {
                    int nPacketSize = mvlc.MV_CC_GetOptimalPacketSize_NET();
                    if (nPacketSize > 0)
                    {
                        nRet = mvlc.MV_CC_SetIntValueEx_NET("GevSCPSPacketSize", nPacketSize);
                        if (nRet != MvLineScanCamera.MV_OK)
                        {
                            lastErrorMsg = "Warning: Set Packet Size failed";
                        }
                    }
                    else
                    {
                        lastErrorMsg = "Warning: Get Packet Size failed";
                    }
                }

                if (triggerMode)
                {
                    if (MvLineScanCamera.MV_OK != mvlc.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MvLineScanCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON))
                    {
                        lastErrorMsg = "Set TriggerMode failed MV_TRIGGER_MODE_ON";
                        return false;
                    }

                    uint s = 0;

                    if (triggerSource == 0) s = (uint)MvLineScanCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0;
                    if (triggerSource == 1) s = (uint)MvLineScanCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE1;
                    if (triggerSource == 2) s = (uint)MvLineScanCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE2;
                    if (triggerSource == 3) s = (uint)MvLineScanCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE3;
                    if (triggerSource == 4) s = (uint)MvLineScanCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_FrequencyConverter;


                    if (MvLineScanCamera.MV_OK != mvlc.MV_CC_SetEnumValue_NET("TriggerSource", s))
                    {
                        lastErrorMsg = "Set TriggerSource failed MV_TRIGGER_SOURCE_FrequencyConverter";
                        return false;
                    }
                }
                else
                {
                    if (MvLineScanCamera.MV_OK != mvlc.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MvLineScanCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF))
                    {
                        lastErrorMsg = "Set TriggerMode failed MV_TRIGGER_MODE_OFF";

                        return false;
                    }
                }

                connectedCamera = true;
            }

            return connectedCamera;
        }

        public void disconnectCamera()
        {
            grabStop();

            if (m_BufForDriver != IntPtr.Zero)
            {
                Marshal.Release(m_BufForDriver);

                m_BufForDriver = IntPtr.Zero;
            }

            if (mvlc != null)
            {
                mvlc.MV_CC_CloseDevice_NET();
                mvlc.MV_CC_DestroyDevice_NET();
            }

            connectedCamera = false;
        }

        public bool getParameter()
        {
            // after connectCamera 

            MvLineScanCamera.MVCC_FLOATVALUE stParam = new MvLineScanCamera.MVCC_FLOATVALUE();

            int nRet = mvlc.MV_CC_GetFloatValue_NET("ExposureTime", ref stParam);
            if (MvLineScanCamera.MV_OK != nRet)
            {
                lastErrorMsg = "Get ExposureTime failed";

                return false;
            }

            exposureTime = stParam.fCurValue;

            return true;
        }

        public bool makeScanImageBuffer()
        {
            MvLineScanCamera.MVCC_INTVALUE_EX stWidth = new MvLineScanCamera.MVCC_INTVALUE_EX();
            int nRet = mvlc.MV_CC_GetIntValueEx_NET("Width", ref stWidth);

            if (MvLineScanCamera.MV_OK != nRet)
            {
                lastErrorMsg = "Get Width Info Fail!";
                return false;
            }


            MvLineScanCamera.MVCC_INTVALUE_EX stHeight = new MvLineScanCamera.MVCC_INTVALUE_EX();
            nRet = mvlc.MV_CC_GetIntValueEx_NET("Height", ref stHeight);
            if (MvLineScanCamera.MV_OK != nRet)
            {
                lastErrorMsg = "Get Height Info Fail!";
                return false;
            }

            MvLineScanCamera.MVCC_ENUMVALUE stPixelFormat = new MvLineScanCamera.MVCC_ENUMVALUE();
            nRet = mvlc.MV_CC_GetEnumValue_NET("PixelFormat", ref stPixelFormat);
            if (MvLineScanCamera.MV_OK != nRet)
            {
                lastErrorMsg = "Get Pixel Format Fail!";
                return false;
            }

            if ((Int32)MvLineScanCamera.MvGvspPixelType.PixelType_Gvsp_Undefined == (Int32)stPixelFormat.nCurValue)
            {
                lastErrorMsg = "Unknown Pixel Format!";

                return false;
            }

            if (IntPtr.Zero != m_ConvertDstBuf)
            {
                Marshal.FreeHGlobal(m_ConvertDstBuf);
                m_ConvertDstBuf = IntPtr.Zero;
            }

            m_nConvertDstBufLen = (UInt32)(3 * stWidth.nCurValue * stHeight.nCurValue);
            m_ConvertDstBuf = Marshal.AllocHGlobal((Int32)m_nConvertDstBufLen);
            if (IntPtr.Zero == m_ConvertDstBuf)
            {
                lastErrorMsg = "Malloc Memory Fail!";

                return false;
            }

            imageWidth = stWidth.nCurValue;
            imageHeight = stHeight.nCurValue;

            scanTemp = new Bitmap((Int32)stWidth.nCurValue, (Int32)stHeight.nCurValue, PixelFormat.Format24bppRgb);

            return true;
        }

        public bool grabStart()
        {
            if (hReceiveImageThreadHandle != null)
            {
                lastErrorMsg = "aleady start";

                return false;
            }

            int nRet = mvlc.MV_CC_StartGrabbing_NET();
            if (MvLineScanCamera.MV_OK != nRet)
            {
                lastErrorMsg = "Start grabbing failed";
                return false;
            }

            cancellationTokenSource = new CancellationTokenSource();
            hReceiveImageThreadHandle = new Thread(() => ReceiveImageWorkThread(cancellationTokenSource.Token));
            hReceiveImageThreadHandle.IsBackground = true;
            hReceiveImageThreadHandle.Start();

            return true;
        }

        public bool grabStop()
        {
            bool stopSuccess = true;

            if (hReceiveImageThreadHandle != null)
            {
                cancellationTokenSource.Cancel();

                //hReceiveImageThreadHandle.Join();

                DateTime max = DateTime.Now.AddSeconds(5);

                while (hReceiveImageThreadHandle.IsAlive)
                {
                    Thread.Sleep(1);
                    Application.DoEvents();

                    if (max < DateTime.Now)
                    {
                        stopSuccess = false;
                        break;
                    }
                }
            }

            hReceiveImageThreadHandle = null;

            return stopSuccess;
        }

        public bool isRun()
        {
            if (hReceiveImageThreadHandle != null)
                return true;

            return false;
        }


        public void setSavePath(string path)
        {
            savePath = path;

            if (!Directory.Exists(path)) 
            { 
                Directory.CreateDirectory(path); 
            }
        }

        public void saveImageStart()
        {
            bSaveImage = true;
        }

        public void saveImageStop()
        {
            bSaveImage = true;
        }

        public void setPictureBox(PictureBox pb)
        {
            pictureBox = pb;
        }

        public void ReceiveImageWorkThread(CancellationToken token)
        {
            int nRet = MvLineScanCamera.MV_OK;

            MvLineScanCamera.MV_FRAME_OUT stImageOut = new MvLineScanCamera.MV_FRAME_OUT();

            MvLineScanCamera.MV_SAVE_IMG_TO_FILE_PARAM stSaveFileParam = new MvLineScanCamera.MV_SAVE_IMG_TO_FILE_PARAM();
            MvLineScanCamera.MV_PIXEL_CONVERT_PARAM stConvertInfo = new MvLineScanCamera.MV_PIXEL_CONVERT_PARAM();


            while (!token.IsCancellationRequested)
            {
                try
                {
                    nRet = mvlc.MV_CC_GetImageBuffer_NET(ref stImageOut, 1000);

                    if (nRet == MvLineScanCamera.MV_OK)
                    {
                        do
                        {
                            MvLineScanCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = stImageOut.stFrameInfo;

                            frameNo = stFrameInfo.nFrameNum;

                            scanLength = frameNo * mmPerImage;

                            // ---------------------------------------------------------------------------

                            stConvertInfo.nWidth = stFrameInfo.nWidth;
                            stConvertInfo.nHeight = stFrameInfo.nHeight;
                            stConvertInfo.enSrcPixelType = stFrameInfo.enPixelType;
                            stConvertInfo.pSrcData = stImageOut.pBufAddr;
                            stConvertInfo.nSrcDataLen = stFrameInfo.nFrameLen;
                            stConvertInfo.pDstBuffer = m_ConvertDstBuf;
                            stConvertInfo.nDstBufferSize = m_nConvertDstBufLen;

                            stConvertInfo.enDstPixelType = MvLineScanCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed;
                            if (MvLineScanCamera.MV_OK != mvlc.MV_CC_ConvertPixelType_NET(ref stConvertInfo))
                            {
                                lastErrorMsg = "convert Fail!";
                                break;
                            }

                            BitmapData bitmapData = scanTemp.LockBits(new Rectangle(0, 0, stConvertInfo.nWidth, stConvertInfo.nHeight), ImageLockMode.ReadWrite, scanTemp.PixelFormat);
                            CopyMemory(bitmapData.Scan0, stConvertInfo.pDstBuffer, (UInt32)(bitmapData.Stride * scanTemp.Height));
                            scanTemp.UnlockBits(bitmapData);

                            scanImage = (Bitmap)scanTemp.Clone();

                            // ---------------------------------------------------------------------------

                            if (pictureBox != null)
                            {
                                pictureBox.Invoke(new Action(() =>
                                {
                                    Graphics g = pictureBox.CreateGraphics();

                                    g.DrawImage(scanImage, 0, 0, pictureBox.Width, pictureBox.Height);

                                }));
                            }

                            // ---------------------------------------------------------------------------

                            if (bSaveImage)
                            {
                                string path = savePath + "\\" + stFrameInfo.nFrameNum.ToString("0000000000") + ".bmp";
                                stSaveFileParam.enImageType = MvLineScanCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Bmp;
                                stSaveFileParam.enPixelType = stFrameInfo.enPixelType;
                                stSaveFileParam.pData = stImageOut.pBufAddr;
                                stSaveFileParam.nDataLen = stFrameInfo.nFrameLen;
                                stSaveFileParam.nHeight = stFrameInfo.nHeight;
                                stSaveFileParam.nWidth = stFrameInfo.nWidth;
                                stSaveFileParam.iMethodValue = 2;
                                stSaveFileParam.pImagePath = path;

                                if (MvLineScanCamera.MV_OK != mvlc.MV_CC_SaveImageToFile_NET(ref stSaveFileParam))
                                {
                                    lastErrorMsg = "Save Image Fail!";
                                    break;
                                }


                                if (appendImage != null)
                                {
                                    appendImage(path);
                                }
                            }

                            // ---------------------------------------------------------------------------

                            lastErrorMsg = "OK";

                        } while (false);

                        mvlc.MV_CC_FreeImageBuffer_NET(ref stImageOut);

                    }
                    else
                    {
                        lastErrorMsg = "이미지 가져오기 실패 : 트리거 모드 확인";
                    }
                }
                catch (Exception e)
                {
                    lastErrorMsg = "except : " + e.Message;
                }
                finally
                {
                    Thread.Sleep(1);
                }
            }

            try
            {
                if (MvLineScanCamera.MV_OK != mvlc.MV_CC_StopGrabbing_NET())
                {
                    lastErrorMsg = "Stop grabbing failed";
                }
                else
                {
                    lastErrorMsg = "Stop";
                }
            }
            catch (Exception e)
            {
                lastErrorMsg = "Error Stopping : " + e.Message;
            }
        }
    }
}
