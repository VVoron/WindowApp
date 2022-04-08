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

        List<Process> processes = new List<Process>();
        public Form1()
        {
            InitializeComponent();
        }

        private void RefreshProcessesList()
        {
            listView1.Items.Clear();
            processes.Clear();

            var allProcess = dll.GetProcesses();
            double memory = 0;
            int WindowCount = 0;
            foreach (Process proc in allProcess)
            {
                Console.WriteLine(proc.ToString());
                if (proc.MainWindowTitle.ToString() != "")
                {
                    memory = 0;
                    WindowCount++;
                    PerformanceCounter pc = new PerformanceCounter();
                    pc.CategoryName = "Process";
                    pc.CounterName = "Working Set - Private";
                    pc.InstanceName = proc.ProcessName;

                    memory = (double)pc.NextValue() / (1024 * 1024);

                    processes.Add(proc);
                    string[] row = new string[] { proc.ProcessName.ToString(), proc.MainWindowTitle.ToString(), Math.Round(memory, 1).ToString(), proc.UserProcessorTime.ToString(),
                        proc.StartTime.ToString(), proc.WorkingSet.ToString(), proc.TotalProcessorTime.ToString(), proc.BasePriority.ToString() };
                    listView1.Items.Add(new ListViewItem(row));
                    pc.Close();
                    pc.Dispose();
                }
            }
            Text = "Запущено окон: " + WindowCount.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshProcessesList();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
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
                    dll.KillProcess(processToKill);
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
                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }

        private void завершитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    dll.KillProcess(processToKill);
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
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToMinOrMax = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    dll.MaximizOrMinimiz(processToMinOrMax);
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
                    dll.Rename(processRename);
                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
