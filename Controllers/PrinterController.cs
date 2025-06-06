using Microsoft.AspNetCore.Mvc;
using POSSDKSample;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Runtime.InteropServices;


#if x86
using POSSDKHANDLE = System.Int32;
#else
using POSSDKHANDLE = System.Int64;
#endif
namespace API_ReceiptSNBC.Controllers
{
    public class PrinterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public static void SaveAs1BitBitmap(string text, string outputPath)
        {
            // สร้างภาพจากข้อความ
            using (Bitmap original = new Bitmap(202, 46))
            using (Graphics g = Graphics.FromImage(original))
            {
                g.Clear(Color.White);
                using (Font font = new Font("Tahoma", 20))
                {
                    g.DrawString(text, font, Brushes.Black, new PointF(10, 7));
                }

                // แปลงเป็น 1-bit BMP
                using (Bitmap bwBmp = ConvertTo1Bpp(original))
                {
                    bwBmp.Save(outputPath, ImageFormat.Bmp);
                }
            }
        }

        private static Bitmap ConvertTo1Bpp(Bitmap input)
        {
            int width = input.Width;
            int height = input.Height;

#pragma warning disable CA1416 // Validate platform compatibility
            Bitmap output = new Bitmap(width, height, PixelFormat.Format1bppIndexed);
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
            BitmapData data = output.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
#pragma warning restore CA1416 // Validate platform compatibility

            for (int y = 0; y < height; y++)
            {
                byte[] scan = new byte[(width + 7) / 8];
                for (int x = 0; x < width; x++)
                {
                    Color c = input.GetPixel(x, y);
                    int brightness = (c.R + c.G + c.B) / 3;

                    // ปกติ: ถ้ามืด (ดำ) → บิตเป็น 1
                    // แต่เราจะ Invert: ถ้ามืด → บิตเป็น 0 (กลับสี)
                    if (brightness >= 128)
                    {
                        // พื้นหลังสว่าง → set bit = 1 → กลายเป็น "ขาว"
                        scan[x / 8] |= (byte)(0x80 >> (x % 8));
                    }
                }
                Marshal.Copy(scan, 0, data.Scan0 + y * data.Stride, scan.Length);
            }

            output.UnlockBits(data);
            return output;
        }
        public static void PrintThaiLinesIndividually(string[] lines)
        {
            if (lines == null || lines.Length == 0)
                throw new ArgumentException("No lines to print.");

            string imgDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
            Directory.CreateDirectory(imgDir);

            string bmpPath = Path.Combine(imgDir, "ThaiLine.bmp");

            // Printer setup
            GenericTool.SelectPaperType(GenericTool.PrinterHandle, 0);
            GenericTool.PrintSetMode(GenericTool.PrinterHandle, 0);
            GenericTool.ApplicationUnit(GenericTool.PrinterHandle, 1);
            GenericTool.SetTextFontType(GenericTool.PrinterHandle, 0);
            GenericTool.SetAlignmentMode(GenericTool.PrinterHandle, 0);

            int nStartY = -1;

            foreach (var line in lines)
            {
                SaveAs1BitBitmap(line, bmpPath);

                int ret = GenericTool.DownloadRAMBitmapByFile(GenericTool.PrinterHandle, bmpPath, 0);
                if (ret != GenericTool.ERR_SUCCESS)
                    throw new Exception($"Failed to load BMP for line: {line}");

                ret = GenericTool.PrintRAMBitmap(GenericTool.PrinterHandle, 0, 5, nStartY, 0);
                if (ret != GenericTool.ERR_SUCCESS)
                    throw new Exception($"Failed to print line: {line}");

                GenericTool.FeedLine(GenericTool.PrinterHandle);
            }

            // Finalize receipt
            GenericTool.CutPaper(GenericTool.PrinterHandle, 1, 80);
            GenericTool.Reset(GenericTool.PrinterHandle);
        }


        public enum PortType
        {
            Serial = 0,
            LPT = 1,
            Network = 2,
            Driver = 3,
            USBAPI = 4,
            USBClass = 5
        }
        public class ConnectionParams
        {
            public PortType Type { get; set; }
            public string ComPort { get; set; }
            public int BaudRate { get; set; }
            public int Handshake { get; set; }
            public string LptPort { get; set; }
            public string IpAddress { get; set; }
            public int PortNumber { get; set; }
            public string DriverName { get; set; }
            public int UsbId { get; set; }
            public string UsbPrinterName { get; set; }
            public int WidthDPI { get; set; }
            public int HeightDPI { get; set; }
            public string PrinterType { get; set; }
            public string[] ThaiLines { get; set; }
        }

        public static bool AlreadyConnected { get; private set; } = false;
        public static (bool Success, string Message) Connect(ConnectionParams param)
        {
            if (AlreadyConnected)
            {
                int closeRet = GenericTool.ClosePort(GenericTool.PrinterHandle);
                if (closeRet != GenericTool.ERR_SUCCESS)
                    return (false, $"Close failed: {closeRet}");
                AlreadyConnected = false;
            }

            POSSDKHANDLE handle = GenericTool.Init(param.PrinterType);
            if (handle < 0)
                return (false, $"Init failed. Code: {handle}");

            GenericTool.PrinterHandle = handle;

            int retCode;
            switch (param.Type)
            {
                case PortType.Serial:
                    retCode = GenericTool.OpenCOMPort(handle, param.ComPort, param.BaudRate, 8, 0, 0, param.Handshake);
                    break;
                case PortType.LPT:
                    retCode = GenericTool.OpenLPTPort(handle, param.LptPort);
                    break;
                case PortType.Network:
                    if (!IPAddress.TryParse(param.IpAddress, out _))
                        return (false, "Invalid IP.");
                    retCode = GenericTool.OpenNetPort(handle, param.IpAddress, param.PortNumber, 0, 0);
                    break;
                case PortType.Driver:
                    retCode = GenericTool.OpenDriverPort(handle, param.DriverName);
                    break;
                case PortType.USBAPI:
                    retCode = GenericTool.OpenUsbApiPort(handle, param.UsbId);
                    break;
                case PortType.USBClass:
                    retCode = GenericTool.OpenUsbClassPort(handle, param.UsbPrinterName, 0);
                    break;
                default:
                    return (false, "Unknown port type.");
            }

            if (retCode != GenericTool.ERR_SUCCESS)
                return (false, $"Connection failed: {retCode}");

            GenericTool.MotionUnit(handle, param.WidthDPI, param.HeightDPI);
            AlreadyConnected = true;

            try
            {
                if (param.ThaiLines != null)
                {
                    PrintThaiLinesIndividually(param.ThaiLines);
                }
                GenericTool.ClosePort(handle);
                AlreadyConnected = false;

                return (true, "Printed successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Exception: {ex.Message}");
            }
        }

        public static (bool Success, string Message) Disconnect()
        {
            if (!AlreadyConnected)
                return (false, "Printer not connected.");

            int ret = GenericTool.ClosePort(GenericTool.PrinterHandle);
            if (ret != GenericTool.ERR_SUCCESS)
                return (false, $"Disconnect failed: {ret}");

            AlreadyConnected = false;
            return (true, "Disconnected successfully.");
        }
        [HttpPost("connect")]
        public IActionResult ConnectPrinter([FromBody] ConnectionParams param)
        {
            var result = Connect(param);
            return Ok(new
            {
                success = result.Success,
                message = result.Message
            });
        }

    }
}
