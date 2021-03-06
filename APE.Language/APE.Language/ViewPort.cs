﻿//
//Copyright 2016 David Beales
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using APE.Communication;
using APE.Dock;
using NM = APE.Native.NativeMethods;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using APE.Language;
using System.IO;
using System.Reflection;

namespace APE
{
    /// <summary>
    /// Class used to display a small window which shows the last few lines logged, along with some other
    /// information and buttons to break, abort and display an about box
    /// </summary>
    internal partial class ViewPort : AppBar
    {
        private int m_Lines;
        private object m_lock = new object();
        internal IntPtr Foreground = IntPtr.Zero;
        internal bool Acknowledge = false;
        internal bool Processing = false;
        private uint m_DoubleClickTimer = 0;

        private delegate void AppendToLogCallback(string text, LogItemType type);

        internal ViewPort()
        {
            InitializeComponent();
            rtbLogViewer.BackColor = Color.White;
            rtbLogViewer.WordWrap = true;
            ctxtMenuViewPort.Opened += new EventHandler(ctxtMenuViewPort_Opened);
            this.Icon = Language.Properties.Resources.APE256x256;
            if (GUI.IsElevatedAdmin)
            {
                elevatedAdminPictureBox.Image = Language.Properties.Resources.Tick16x16;
            }
            else
            {
                elevatedAdminPictureBox.Image = Language.Properties.Resources.Cross16x16;
            }
            if (GUI.FormAnimationDisabled)
            {
                formAnimationDisabledPictureBox.Image = Language.Properties.Resources.Tick16x16;
            }
            else
            {
                formAnimationDisabledPictureBox.Image = Language.Properties.Resources.Cross16x16;
            }
            this.ControlBox = false;
            this.Shown += new EventHandler(frmViewPort_Shown);
        }

        //TODO move this to capture? have in out image instead of width/height?
        //private static Bitmap ResizeImage(Image image, int width, int height)
        //{
        //    Rectangle destRect = new Rectangle(0, 0, width, height);
        //    Bitmap destImage = new Bitmap(width, height);

        //    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        //    using (Graphics imageGraphics = Graphics.FromImage(destImage))
        //    {
        //        imageGraphics.CompositingMode = CompositingMode.SourceCopy;
        //        imageGraphics.CompositingQuality = CompositingQuality.HighQuality;
        //        imageGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //        imageGraphics.SmoothingMode = SmoothingMode.HighQuality;
        //        imageGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        //        using (ImageAttributes wrapMode = new ImageAttributes())
        //        {
        //            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        //            imageGraphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        //        }
        //    }

        //    return destImage;
        //}

        private void frmViewPort_Shown(Object sender, EventArgs e)
        {
            base.AppbarNew(AppBarEdges.Top);

            if (!NM.RegisterHotKey(this.Handle, 1, 0, NM.VK_PAUSE))
            {
                throw new Exception("Failed to register hotkey");
            }

            ProcessStartInfo AppStartup = new ProcessStartInfo();
            AppStartup.FileName = "APE.Watcher.exe";
            AppStartup.Arguments = Process.GetCurrentProcess().Id.ToString() + " " + ((uint)SystemInformation.DoubleClickTime).ToString();
            AppStartup.WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Process.Start(AppStartup);

            m_DoubleClickTimer = (uint)SystemInformation.DoubleClickTime;
        }

        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == NM.WM_HOTKEY)
            {
                if (!Processing)
                {
                    Processing = true;
                    if (msg.WParam.ToInt32() == 1)
                    {   
                        if (Foreground != IntPtr.Zero)
                        {
                            if (!NM.SetForegroundWindow(Foreground))
                            {
                                //Foreground = IntPtr.Zero;
                                throw new Exception("SetForegroundWindow failed");
                            }
                        }
                        else
                        {
                            Break();
                        }
                    }
                    Processing = false;
                }
            }

            base.WndProc(ref msg);
        }

        private void Break()
        {
            // Restore the double click time if need be
            if (SystemInformation.DoubleClickTime == 1)
            {
                NM.SetDoubleClickTime(m_DoubleClickTimer);
            }

            Input.Reset();

            if (Debugger.IsAttached)
            {
                Process CurrentProcess = Process.GetCurrentProcess();
                Process VisualStudio = GetParentProcess(CurrentProcess);
                if (VisualStudio.ProcessName.ToLower() == "msvsmon")
                {
                    VisualStudio = GetParentProcess(VisualStudio);
                }

                if (VisualStudio.ProcessName.ToLower() == "devenv" && VisualStudio.MainWindowHandle != IntPtr.Zero)
                {
                    if (!NM.SetForegroundWindow(VisualStudio.MainWindowHandle))
                    {
                        throw new Exception("SetForegroundWindow VisualStudio failed");
                    }

                    NM.keybd_event(NM.VK_CONTROL, 0x1d, NM.KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    NM.keybd_event(NM.VK_MENU, 0x38, NM.KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    NM.keybd_event(NM.VK_CANCEL, 0x46, NM.KEYEVENTF_KEYDOWN | NM.KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
                    NM.keybd_event(NM.VK_CANCEL, 0x46, NM.KEYEVENTF_KEYUP | NM.KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
                    NM.keybd_event(NM.VK_MENU, 0x38, NM.KEYEVENTF_KEYUP, UIntPtr.Zero);
                    NM.keybd_event(NM.VK_CONTROL, 0x1d, NM.KEYEVENTF_KEYUP, UIntPtr.Zero);
                }
                else
                {
                    Debugger.Launch();
                }
            }
            else
            {
                Debugger.Launch();
            }
        }

        private void frmViewPort_FormClosing(Object sender, FormClosingEventArgs e)
        {
            NM.UnregisterHotKey(this.Handle, 1);
            Exit();
        }

        private void btnBreak_Click(object sender, EventArgs e)
        {
            Break();
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void Exit()
        {
            Environment.Exit(0);
        }

        internal void AppendToLog(string text, LogItemType type)
        {
            if (InvokeRequired)
            {
                AppendToLogCallback cbAppendToLog = new AppendToLogCallback(AppendToLog);
                Invoke(cbAppendToLog, new object[] { text, type });
            }
            else
            {
                Color colText;
                // TODO let the user configure the colours
                switch (type)
                {
                    case LogItemType.Action:
                        colText = Color.Black;
                        break;
                    case LogItemType.Start:
                        colText = Color.Turquoise;
                        break;
                    case LogItemType.Finish:
                        colText = Color.Brown;
                        break;
                    case LogItemType.Pass:
                        colText = Color.Green;
                        break;
                    case LogItemType.Fail:
                        colText = Color.Red;
                        break;
                    case LogItemType.Disabled:
                        colText = Color.LightGray;
                        break;
                    case LogItemType.NA:
                        colText = Color.Purple;
                        break;
                    case LogItemType.Debug:
                        colText = Color.SlateGray;
                        break;
                    case LogItemType.Information:
                        colText = Color.Navy;
                        break;
                    case LogItemType.Warning:
                        colText = Color.LightPink;
                        break;
                    case LogItemType.Error:
                        colText = Color.DeepPink;
                        break;
                    default:
                        throw new Exception("Implement support for LogItemType." + type.ToString());
                }

                string TextToAdd;

                m_Lines += 1;
                    
                if (m_Lines == 1)
                {
                    TextToAdd = text;
                }
                else
                {
                    TextToAdd = "\n" + text;
                }

                // Append the line and set its colour to be correct
                int TextLengthBeforeAppend = rtbLogViewer.TextLength;
                rtbLogViewer.AppendText(TextToAdd);
                rtbLogViewer.SelectionStart = TextLengthBeforeAppend;
                rtbLogViewer.SelectionLength = TextToAdd.Length;
                rtbLogViewer.SelectionColor = colText;
                rtbLogViewer.ScrollToCaret();
                
                // If we have more than 40 lines
                if (m_Lines > 40)
                {
                    // Remove the first line currently in the rich text box
                    rtbLogViewer.SelectionStart = 0;
                    rtbLogViewer.SelectionLength = rtbLogViewer.Text.IndexOf('\n') + 1;
                    rtbLogViewer.ReadOnly = false;
                    rtbLogViewer.SelectedText = "";
                    rtbLogViewer.ReadOnly = true;
                    m_Lines -= 1;
                    rtbLogViewer.SelectionStart = rtbLogViewer.TextLength;
                }
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbLogViewer.Copy();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbLogViewer.ReadOnly = false;
            int before = rtbLogViewer.Lines.GetLength(0);
            if (rtbLogViewer.SelectionStart > 0)
            {
                rtbLogViewer.SelectionStart = rtbLogViewer.SelectionStart - 1;
                if (!rtbLogViewer.SelectedText.StartsWith("\n"))
                {
                    rtbLogViewer.SelectionStart = rtbLogViewer.SelectionStart + 1;
                }
            }
            rtbLogViewer.SelectedText = "";
            int after = rtbLogViewer.Lines.GetLength(0);
            rtbLogViewer.ReadOnly = true;
            m_Lines -= before - after;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbLogViewer.SelectAll();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbLogViewer.SelectAll();
            rtbLogViewer.ReadOnly = false;
            rtbLogViewer.SelectedText = "";
            rtbLogViewer.ReadOnly = true;
            m_Lines = 0;
        }

        private void ctxtMenuViewPort_Opened(object sender, EventArgs e)
        {
            if (Cursor.Position.Y + ctxtMenuViewPort.Height > SystemInformation.PrimaryMonitorSize.Height)
            {
                NM.SetWindowPos(ctxtMenuViewPort.Handle, IntPtr.Zero, Cursor.Position.X, Cursor.Position.Y - ctxtMenuViewPort.Height, 0, 0, NM.SWP_NOSIZE | NM.SWP_NOZORDER);
            }
            else
            {
                NM.SetWindowPos(ctxtMenuViewPort.Handle, IntPtr.Zero, Cursor.Position.X, Cursor.Position.Y, 0, 0, 5);
            }
        }

        internal static Process GetParentProcess(Process process)
        {
            int pid = process.Id;
            Process parentProc = null;
            IntPtr handleToSnapshot = IntPtr.Zero;

            try
            {
                NM.ProcessEntry32 procEntry = new NM.ProcessEntry32();
                procEntry.dwSize = (uint)Marshal.SizeOf(typeof(NM.ProcessEntry32));
                handleToSnapshot = NM.CreateToolhelp32Snapshot(NM.ToolHelpCreateSnapshotFlags.Process, 0);
                if (NM.Process32First(handleToSnapshot, ref procEntry))
                {
                    do
                    {
                        if (pid == procEntry.th32ProcessID)
                        {
                            parentProc = Process.GetProcessById((int)procEntry.th32ParentProcessID);
                            break;
                        }
                    }
                    while (NM.Process32Next(handleToSnapshot, ref procEntry));
                }
                else
                {
                    throw new Exception("Failed with win32 error code " + Marshal.GetLastWin32Error().ToString());
                }
            }
            finally
            {
                NM.CloseHandle(handleToSnapshot);
            }
            return parentProc;
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            //TODO
        }
    }
}
