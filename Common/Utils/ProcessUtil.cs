using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Management;
using System.Web;
using System.Diagnostics;
using System.IO;

namespace TaskManager.Common.Utils
{
    public class ProcessUtil
    {
        public static bool StartProcess(string FilePath, string Args, int TimeOut, ref string Msg, bool IsLogResult)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = FilePath;
            info.Arguments = Args;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            //info.StandardOutputEncoding = Encoding;
            info.WindowStyle = ProcessWindowStyle.Minimized;
            StreamReader reader = null;

            try
            {
                StringBuilder sb = new StringBuilder();
                Process pro = Process.Start(info);

                if (IsLogResult)
                {
                    reader = pro.StandardOutput;//截取输出流
                    while (!reader.EndOfStream)
                    {
                        sb.AppendLine(reader.ReadLine());
                    }

                }
                pro.WaitForExit(TimeOut);
                Msg = sb.ToString();
                return true;
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
                return false;
            }
        }

    }
}
