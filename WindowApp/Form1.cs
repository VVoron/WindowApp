using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;

namespace WindowApp
{
    public partial class Form1 : Form
    {
        WinAppDLL.WinAppDLL dll = new WinAppDLL.WinAppDLL();

        private List<Process> processes = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void RefreshProcessesList()
        {
            listView1.Items.Clear();

            double memory = 0;
            int WindowCount = 0;
            foreach (Process proc in processes)
            {
                Console.WriteLine(proc.ToString());
                if (proc.MainWindowHandle != IntPtr.Zero)
                {
                    memory = 0;
                    WindowCount++;
                    PerformanceCounter pc = new PerformanceCounter();
                    pc.CategoryName = "Process";
                    pc.CounterName = "Working Set - Private";
                    pc.InstanceName = proc.ProcessName;

                    memory = (double)pc.NextValue() / (1024 * 1024);

                    string[] row = new string[] { proc.ProcessName.ToString(), Math.Round(memory, 1).ToString(), proc.UserProcessorTime.ToString(), proc.StartTime.ToString(), proc.WorkingSet.ToString(), proc.TotalProcessorTime.ToString(), proc.BasePriority.ToString() };
                    listView1.Items.Add(new ListViewItem(row));
                    pc.Close();
                    pc.Dispose();
                }
            }
            Text = "Запущено процессов: " + WindowCount.ToString();
        }

        private void KillProcess(Process process)
        {
            process.Kill();
            process.WaitForExit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            processes = dll.GetProcesses();
            RefreshProcessesList();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            processes = dll.GetProcesses();
            RefreshProcessesList();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == 
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    KillProcess(processToKill);
                    processes = dll.GetProcesses();
                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    dll.KillProcessAndChildren(dll.GetParentProcessId(processToKill));
                    processes = dll.GetProcesses();
                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }

        private void завершитьДеревоПроцессовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    dll.KillProcessAndChildren(dll.GetParentProcessId(processToKill));
                    processes = dll.GetProcesses();
                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }






        //Для свертывания/развертывания окна
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref Windowplacement lpwndpl);
        public struct Windowplacement
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToMinOrMax = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    Windowplacement placement = new Windowplacement();
                    GetWindowPlacement(processToMinOrMax.MainWindowHandle, ref placement);

                    if (placement.showCmd == 2)
                    {
                        WinAppDLL.WinAppDLL.WinAPI.ShowWindow(processToMinOrMax.MainWindowHandle, WinAppDLL.WinAppDLL.WinAPI.Consts.SHOWWINDOW.SW_SHOWMAXIMIZED);
                    }
                    else
                    {
                        WinAppDLL.WinAppDLL.WinAPI.ShowWindow(processToMinOrMax.MainWindowHandle, WinAppDLL.WinAppDLL.WinAPI.Consts.SHOWWINDOW.SW_SHOWMINIMIZED);
                    }
                    WinAppDLL.WinAppDLL.WinAPI.SetForegroundWindow(processToMinOrMax.MainWindowHandle);
                }
            }
            catch (Exception) { }
        }



        //Для переименования окна
        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string text);
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processRename = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    string rename = Interaction.InputBox("Введите новое название: ", "Переименование", processRename.ProcessName);
                    SetWindowText(processRename.MainWindowHandle, rename);  
                }
            }
            catch (Exception) { }
        }
    }
}
