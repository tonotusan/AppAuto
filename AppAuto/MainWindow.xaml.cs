using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppAuto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String Anser = "";

            Anser = CommandExec(@"shell input swipe 500 1000 300 800");
//            Anser = CommandExec(@"shell dumpsys activity");

//            int a = 0;
        }

        private String CommandExec(String command)
        {

            string results = "";

            var si = new ProcessStartInfo("adb.exe", command);
            // ウィンドウ表示を完全に消したい場合
            si.CreateNoWindow = true;
            si.RedirectStandardError = true;
            si.RedirectStandardOutput = true;
            si.UseShellExecute = false;
            using (var proc = new Process())
            using (var ctoken = new CancellationTokenSource())
            {
                proc.EnableRaisingEvents = true;
                proc.StartInfo = si;
                // コールバックの設定
                proc.OutputDataReceived += (sender, ev) =>
                {
                    //                    Debug.WriteLine($"stdout={ev.Data}");
                    results += ev.Data + "\n";
                };
                proc.ErrorDataReceived += (sender, ev) =>
                {
                    Console.WriteLine($"stderr={ev.Data}");
                };
                proc.Exited += (sender, ev) =>
                {
                    Console.WriteLine($"exited");
                    // プロセスが終了すると呼ばれる
                    ctoken.Cancel();
                };
                // プロセスの開始
                proc.Start();
                // 非同期出力読出し開始
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
                // 終了まで待つ
                ctoken.Token.WaitHandle.WaitOne();
            }


            return results;
        }

        //OutputDataReceivedイベントハンドラ
        //行が出力されるたびに呼び出される
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
//                        String cmd = @"shell input swipe 500 1000 300 800";
            String cmd = @"shell dumpsys activity";


            var si = new ProcessStartInfo("adb.exe", cmd);
            // ウィンドウ表示を完全に消したい場合
            // si.CreateNoWindow = true;
            si.RedirectStandardError = true;
            si.RedirectStandardOutput = true;
            si.UseShellExecute = false;
            using (var proc = new Process())
            using (var ctoken = new CancellationTokenSource())
            {
                proc.EnableRaisingEvents = true;
                proc.StartInfo = si;
                // コールバックの設定
                proc.OutputDataReceived += (sender, ev) =>
                {
                    Debug.WriteLine($"stdout={ev.Data}");
                };
                proc.ErrorDataReceived += (sender, ev) =>
                {
                    Console.WriteLine($"stderr={ev.Data}");
                };
                proc.Exited += (sender, ev) =>
                {
                    Console.WriteLine($"exited");
                    // プロセスが終了すると呼ばれる
                    ctoken.Cancel();
                };
                // プロセスの開始
                proc.Start();
                // 非同期出力読出し開始
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
                // 終了まで待つ
                ctoken.Token.WaitHandle.WaitOne();
            }

        }
    }
}
