//using POSSDKHANDLE = System.Int64;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#if x86
using POSSDKHANDLE = System.Int32;
#else
using POSSDKHANDLE = System.Int64;
#endif

namespace POSSDKSample
{
    public class GenericTool
    {
        public static int LANG_EN = 0;
        public static int LANG_CN = 1;

        public static int language = LANG_EN;
        public static void MMessageBox(string en, string cn)
        {
            if (language.Equals(LANG_CN))
            {
               //MessageBox.Show(cn);
            }
            else
            {
               //MessageBox.Show(en);
            }
        }

        // POSSDK 错误码
        public static int ERR_SUCCESS = 0;
        public static int ERR_FAIL = -1;
        public static int ERR_HANDLE = -2;
        public static int ERR_PARAMATER = -3;
        public static int ERR_FILE = -4;
        public static int ERR_READ = -5;
        public static int ERR_WRITE = -6;
        public static int ERR_NOT_SUPPORT = -7;
        public static int ERR_BITMAP_INVAILD = -8;
        public static int ERR_LOADDLL_FAILURE = -9;
        public static int ERR_FIRNOSUPPORT = -10;
        public static int ERR_TASK_PRINT_FAILED = -11;
        public static int ERR_UNKOWN_ERROR = -127;

        // 枚举类型
        public static int PORT_USBDEVICE_CLASS = 3;
        public static int PORT_DRIVER = 6;

        // 条码
        public static int TYPE_UPC_A = 65;
        public static int TYPE_UPC_E = 66;
        public static int TYPE_JAN13 = 67;
        public static int TYPE_JAN8 = 68;
        public static int TYPE_CODE39 = 69;
        public static int TYPE_ITF = 70;
        public static int TYPE_CODEBAR = 71;
        public static int TYPE_CODE93 = 72;
        public static int TYPE_CODE128 = 73;

        //  

    
        public static string PrinterType = "";
        public static string PrinterModel = "";
        public static POSSDKHANDLE PrinterHandle;

        struct PrinterStatusStruct
        {
            int isOnlineStatus;     //0-offline,1-online
            int isCoverOpened;      //0-false,1-true
            int isPaperEnd;         //0-false,1-true
        }


        public  enum InternalCharSet
        {
            CharSET_USA = 0,
            CharSET_FRANCE,
            CharSET_GERMANY,
            CharSET_UK,
            CharSET_DENARKI,
            CharSET_SWEDEN,
            CharSET_ITALY,
            CharSET_SPAINI,
            CharSET_JAPAN,
            CharSET_NORWAY,
            CharSET_DENMARKII,
            CharSET_SPAINII,
            CharSET_LATINAMERICA,
            CharSET_KOREA
        };

       public enum BarcodeType
        {
            TYPE_UPC_A = 65,
            TYPE_UPC_E,
            TYPE_JAN13,
            TYPE_JAN8,
            TYPE_CODE39,
            TYPE_ITF,
            TYPE_CODEBAR,
            TYPE_CODE93,
            TYPE_CODE128
        };

        public static void MMessageBoxErrorMsg(int ret, string msgEN = "", string msgCN = "")
        {
            if(ret == ERR_SUCCESS)
            {
                return;
            }

            string errorMsgCN;
            string errorMsgEN;

            string errorEN = "Errorcode = " + ret + "\nErrorString = " + GenericTool.getErrorStringEN(ret);
            string errorCN = "错误码 = " + ret + "\n错误描述 = " + GenericTool.getErrorStringCN(ret);

            if (msgEN.Equals(""))
            {
                errorMsgEN = errorEN;
            }
            else
            {
                errorMsgEN = msgEN + "\n" + errorEN;
            }

            if (msgCN.Equals(""))
            {
                errorMsgCN = errorCN;
            }
            else
            {
                errorMsgCN = msgCN + "\n" + errorCN;
            }

            MMessageBox(errorMsgEN, errorMsgCN);
        }

        public static string getErrorStringEN(int errorcode)
        {
            if (errorcode == ERR_SUCCESS)
                return "Succeed";
            else if (errorcode == ERR_FAIL)
                return "Failed";
            else if (errorcode == ERR_HANDLE)
                return "Invalid handle";
            else if (errorcode == ERR_PARAMATER)
                return "Invalid parameter";
            else if (errorcode == ERR_FILE)
                return "Error in parameter file";
            else if (errorcode == ERR_READ)
                return "Read failure";
            else if (errorcode == ERR_WRITE)
                return "Write failure";
            else if (errorcode == ERR_NOT_SUPPORT)
                return "Function not supported";
            else if (errorcode == ERR_BITMAP_INVAILD)
                return "Bitmap error";
            else if (errorcode == ERR_LOADDLL_FAILURE)
                return "Failed to load the dynamic library";
            else if (errorcode == ERR_FIRNOSUPPORT)
                return "Firmware not supported";
            else if (errorcode == ERR_UNKOWN_ERROR)
                return "unknown error";
            else
                return "" + errorcode;
        }

        public static string getErrorStringCN(int errorcode)
        {
            if (errorcode == ERR_SUCCESS)
                return "成功";
            else if (errorcode == ERR_FAIL)
                return "失败";
            else if (errorcode == ERR_HANDLE)
                return "无效句柄";
            else if (errorcode == ERR_PARAMATER)
                return "无效参数";
            else if (errorcode == ERR_FILE)
                return "配置文件错误";
            else if (errorcode == ERR_READ)
                return "读数据失败";
            else if (errorcode == ERR_WRITE)
                return "写数据失败";
            else if (errorcode == ERR_NOT_SUPPORT)
                return "此功能不支持";
            else if (errorcode == ERR_BITMAP_INVAILD)
                return "位图错误";
            else if (errorcode == ERR_LOADDLL_FAILURE)
                return "加载动态库失败";
            else if (errorcode == ERR_FIRNOSUPPORT)
                return "此功能固件不支持";
            else if (errorcode == ERR_UNKOWN_ERROR)
                return "未知错误";
            else
                return "" + errorcode;
        }

        /// 为INI文件中指定的节点取得字符串
        /// </summary>
        /// <param name="lpAppName">欲在其中查找关键字的节点名称</param>
        /// <param name="lpKeyName">欲获取的项名</param>
        /// <param name="lpDefault">指定的项没有找到时返回的默认值</param>
        /// <param name="lpReturnedString">指定一个字串缓冲区，长度至少为nSize</param>
        /// <param name="nSize">指定装载到lpReturnedString缓冲区的最大字符数量</param>
        /// <param name="lpFileName">INI文件完整路径</param>
        /// <returns>复制到lpReturnedString缓冲区的字节数量，其中不包括那些NULL中止字符</returns>
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            int nSize,
            string lpFileName);

        // 静态加载POSSDK.dll Demo程序需要与POSSDK.dll在同一路径下
        // 通讯
        [DllImport("POSSDK.dll")]
        public static extern int EnumDeviceInfo(int nOperationType, StringBuilder szDeviceNameBuf, int nBufLen, out int npNumber);

        [DllImport("POSSDK.dll")]
        public static extern POSSDKHANDLE Init(string szDeviceName);

        [DllImport("POSSDK.dll")]
        public static extern int OpenCOMPort(POSSDKHANDLE hDev, string szName, int nComBaudrate, int nComDataBits, int nComStopBits, int nComParity, int nParam);

        [DllImport("POSSDK.dll")]
        public static extern int OpenLPTPort(POSSDKHANDLE hDev, string szName);

        [DllImport("POSSDK.dll")]
        public static extern int OpenUsbApiPort(POSSDKHANDLE hDev, int nDeviceId);

        [DllImport("POSSDK.dll")]
        public static extern int OpenUsbClassPort(POSSDKHANDLE hDev, string szName, int nIsSplitStatus);

        [DllImport("POSSDK.dll")]
        public static extern int OpenNetPort(POSSDKHANDLE hDev, string szNetIP, int nPort, int nProtocalType, int nBroadcast);

        [DllImport("POSSDK.dll")]
        public static extern int OpenDriverPort(POSSDKHANDLE hDev, string szName);

        [DllImport("POSSDK.dll")]
        public static extern int ClosePort(POSSDKHANDLE hDev);

        [DllImport("POSSDK.dll")]
        public static extern int SendPortData(POSSDKHANDLE hDev, string szData, int nDataLength, out int npReturnLen);

        [DllImport("POSSDK.dll")]
        public static extern int ReadPortData(POSSDKHANDLE hDev, StringBuilder szData, int nDataLength, out int npReturnLen);

        [DllImport("POSSDK.dll")]
        public static extern int SetPortTimeout(POSSDKHANDLE hDev, int nWriteTimeout, int nReadTimeout);


        [DllImport("POSSDK.dll")]
        public static extern int GetPortTimeout(POSSDKHANDLE hDev, out int nWriteTimeout, out int nReadTimeout);

        // 位图
        [DllImport("POSSDK.dll")]
        public static extern int PrintBitmap(POSSDKHANDLE hDev, string szFilePath, int nStartX, int nStartY, int nType);

        [DllImport("POSSDK.dll")]
        public static extern int PrintBitmapByMode(POSSDKHANDLE hDev, string szFilePath, int nStartX, int nStartY, int nMode);

        [DllImport("POSSDK.dll")]
        public static extern int PrintTrueType(POSSDKHANDLE hDev, string szText, int nStartX,
            int nStartY, string szFontName, int nFontHeight, int nFontWidth, int nBold);

        [DllImport("POSSDK.dll")]
        public static extern int DownloadRAMBitmapByFile(POSSDKHANDLE hDev, string szFilePath, int nImageID);

        [DllImport("POSSDK.dll")]
        public static extern int PrintRAMBitmap(POSSDKHANDLE hDev, int nImageID, int nStartX, int nStartY, int nMode);

        [DllImport("POSSDK.dll")]
        public static extern int DownloadFlashBitmapByFile(POSSDKHANDLE hDev, string[] szFilePath, int nImageCount);

        [DllImport("POSSDK.dll")]
        public static extern int PrintFlashBitmap(POSSDKHANDLE hDev, int nImageID, int nStartX, int nStartY, int nMode);

        [DllImport("POSSDK.dll")]
        public static extern int PrintImage(POSSDKHANDLE hDev, string szImage, int nImageDataLen, int nStartX, int nStartY, int nWidth, int nHeight, int nZoom, int nDither, long nReserve);

        [DllImport("POSSDK.dll")]
        public static extern int DownloadImageFile(POSSDKHANDLE hDev, string szImageList, int nType, int nWidth, int nHeight, int nZoom, int nDither, long nReserve);

        [DllImport("POSSDK.dll")]
        public static extern int SetTTFProperties(POSSDKHANDLE hDev, string szFont, string nCodePage, int nFontWidth,int nFontHeight, int nCharSpace, int nLineSpace, int nWidth, int nFontType);

        [DllImport("POSSDK.dll")]
        public static extern int PrintTextByTTF(POSSDKHANDLE hDev, int nStartX, int nStartY, string szContent, int nContentLen);

        // 条码
        [DllImport("POSSDK.dll")]
        public static extern int PrintBarcode(POSSDKHANDLE hDev, string szDataBuffer, int nStartX,
            int nStartY, int nType, int nBasicWidth, int nHeight, int nHriFontType, int nHriFontPosition, int nBytesToPrint);

        [DllImport("POSSDK.dll")]
        public static extern int PrintBarcodeSimple(POSSDKHANDLE hDev, string szDataBuffer, int nStartX,
            int nStartY, int nType, int nWidth, int nHeight, int nHriFontPosition);

        [DllImport("POSSDK.dll")]
        public static extern int BarcodePrintQR(POSSDKHANDLE hDev, string szDataBuffer, int nStartX,
            int nStartY, int nBasicWidth, int nSymbolType, int nLanguageMode, int nErrorCorrect, int nBytesToPrint);

        [DllImport("POSSDK.dll")]
        public static extern int BarcodePrintQREx(POSSDKHANDLE hDev, string szDataBuffer, int nStartX,
            int nStartY, int nBasicWidth, int nErrorCorrect, int nBytesToPrint);

        [DllImport("POSSDK.dll")]
        public static extern int BarcodePrintPDF417(POSSDKHANDLE hDev, string szDataBuffer, int nStartX,
            int nStartY, int nBasicWidth, int nHeight, int nLines, int nColumns, int nScaleH, int nScaleV, int nCorrectGrade, int nBytesToPrint);

        [DllImport("POSSDK.dll")]
        public static extern int BarcodePrintPDF417Simple(POSSDKHANDLE hDev, string szDataBuffer, int nStartX, int nStartY, int nWidth, int nHeight);

        [DllImport("POSSDK.dll")]
        public static extern int BarcodePrintMaxicode(POSSDKHANDLE hDev, string szDataBuffer, int nStartX, int nStartY, int nBytesToPrint);

        [DllImport("POSSDK.dll")]
        public static extern int BarcodePrintGS1DataBar(POSSDKHANDLE hDev, string szDataBuffer, int nStartX,
            int nStartY, int nBarcodeType, int nBasicWidth, int nHeight, int nSegmentNum, int nSeparatorHeight,
            int nBasicHeight, int nHRI, int nAI, int nBytesToPrint);

        // 文本
        [DllImport("POSSDK.dll")]
        public static extern int FeedLine(POSSDKHANDLE hDev);

        [DllImport("POSSDK.dll")]
        public static extern int PrintTextOut(POSSDKHANDLE hDev, string szTextData, int nStartX, int nStartY);

        [DllImport("POSSDK.dll")]
        public static extern int UniversalTextOut(POSSDKHANDLE hDev, string szTextData, int nStartX,
            int nStartY, int nWidthTimes, int nHeightTimes, int nFontType, int nFontStyle);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextLineHight(POSSDKHANDLE hDev, int nLineHeight);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextBold(POSSDKHANDLE hDev, int nBold);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextDoubleWidthAndHeight(POSSDKHANDLE hDev, int nWidthEnable, int nHeightEnable);

        [DllImport("POSSDK.dll")]
        public static extern int SetAlignmentMode(POSSDKHANDLE hDev, int nAlignment);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextCharacterSpace(POSSDKHANDLE hDev, int nLeftSpace, int nRightSpace, int nMode);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextMagnifyTimes(POSSDKHANDLE hDev, int nHorMagnifyTimes, int nVerMagnifyTimes);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextFontType(POSSDKHANDLE hDev, int nFontStyle);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextUpsideDownMode(POSSDKHANDLE hDev, int nMode);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextOppositeColor(POSSDKHANDLE hDev, int nEnable);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextUnderline(POSSDKHANDLE hDev, int nMode);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextColorEnable(POSSDKHANDLE hDev, int nEnable);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextFontColor(POSSDKHANDLE hDev, int nFontColor);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextRotate(POSSDKHANDLE hDev, int nEnable);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextCharsetAndCodepage(POSSDKHANDLE hDev, int nCharSet, int nCodePage);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextUserDefinedCharacterEnable(POSSDKHANDLE hDev, int nEnable);

        [DllImport("POSSDK.dll")]
        public static extern int SetTextDefineUserDefinedCharacter(POSSDKHANDLE hDev, int nStartCode, int nEndCode,
            int nFontType, string szChData);

        // 其他
        [DllImport("POSSDK.dll")]
        public static extern int DownloadFile(POSSDKHANDLE hDev, string szFilePath);

        [DllImport("POSSDK.dll")]
        public static extern int PrintSetMode(POSSDKHANDLE hDev, int nPrintMode);

        [DllImport("POSSDK.dll")]
        public static extern int PageModeSetArea(POSSDKHANDLE hDev, int nStartX, int nStartY, int nWidth, int nHeight, int nDirection);

        [DllImport("POSSDK.dll")]
        public static extern int PageModePrint(POSSDKHANDLE hDev);

        [DllImport("POSSDK.dll")]
        public static extern int PageModeClearBuffer(POSSDKHANDLE hDev);

        [DllImport("POSSDK.dll")]
        public static extern int FeedLineNumber(POSSDKHANDLE hDev, int nLineNum);

        [DllImport("POSSDK.dll")]
        public static extern int CutPaper(POSSDKHANDLE hDev, int nCutMode, int nDistance);

        [DllImport("POSSDK.dll")]
        public static extern int Reset(POSSDKHANDLE hDev);

        [DllImport("POSSDK.dll")]
        public static extern int ApplicationUnit(POSSDKHANDLE hDev, int nUnit);

        [DllImport("POSSDK.dll")]
        public static extern int PrintDensity(POSSDKHANDLE hDev, int nDensity);

        [DllImport("POSSDK.dll")]
        public static extern int MotionUnit(POSSDKHANDLE hDev, int nHorizontalMU, int nVerticalMU);

        [DllImport("POSSDK.dll")]
        public static extern int SelectPaperType(POSSDKHANDLE hDev, int nPaperType);

        [DllImport("POSSDK.dll")]
        public static extern int SelectPaperTypeEEP(POSSDKHANDLE hDev, int nPaperType);

        // 查询
        [DllImport("POSSDK.dll")]
        public static extern int RealTimeQueryStatus(POSSDKHANDLE hDev, IntPtr szStatus, int nDataLength, out ulong nRealDataLength);

        [DllImport("POSSDK.dll")]
        public static extern int NonRealTimeQueryStatus(POSSDKHANDLE hDev, IntPtr szStatus, int nDataLength, out ulong nRealDataLength);

        [DllImport("POSSDK.dll")]
        public static extern int AutoQueryStatus(POSSDKHANDLE hDev, IntPtr szStatus, int nDataLength, int nEnable, out ulong nRealDataLength);

        [DllImport("POSSDK.dll")]
        public static extern int FirmwareVersion(POSSDKHANDLE hDev, StringBuilder szFirmwareVersion, int nDataLength, out ulong nRealDataLength);

        [DllImport("POSSDK.dll")]
        public static extern int SoftwareVersion(POSSDKHANDLE hDev, StringBuilder szSoftwareVersion, int nDataLength, out ulong nRealDataLength);

        [DllImport("POSSDK.dll")]
        public static extern int VendorInformation(POSSDKHANDLE hDev, StringBuilder szVendorInformation, int nDataLength, out ulong nRealDataLength);

        [DllImport("POSSDK.dll")]
        public static extern int PrinterName(POSSDKHANDLE hDev, StringBuilder szPrinterName, int nDataLength, out ulong nRealDataLength);

        [DllImport("POSSDK.dll")]
        public static extern int ResolutionRatio(POSSDKHANDLE hDev, StringBuilder szResolutionRatio, int nDataLength, out ulong nRealDataLength);

        [DllImport("POSSDK.dll")]
        public static extern int KickOutDrawer(POSSDKHANDLE hDev, int nID, int nOnTimes, int nOffTimes);

        [DllImport("POSSDK.dll")]
        public static extern int HardwareSerialNumber(POSSDKHANDLE hDev, StringBuilder szHardwareSerialNumber, int nDataLength,
            out ulong nRealDataLength);

        [DllImport("POSSDK.dll")]
        public static extern int TransactionPrint(POSSDKHANDLE hDev, int nMode);
    }
}
