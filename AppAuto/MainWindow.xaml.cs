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

        Thread thread;

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
                    if (ev.Data != null)
                    {
                        //                        results += ev.Data + "\n";
                        if (ev.Data.IndexOf("Run #") > -1)
                        {
                            results += ev.Data + "\n";
                            if (ev.Data.IndexOf(".smartnews.") > -1)
                            {
                                //                                results += ev.Data + "\n";
                            }
                        }
                    }
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
        private void Button_Connection_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.Text = "";

            TextBox1.Text = CommandExec(@"connect 127.0.0.1:21513");

        }

        private async void Button_Loop_Click(object sender, RoutedEventArgs e)// loop
        {

            String Anser = "";

            int wt = int.Parse(ComboBox2.Text);

            Anser = CommandExec(@"shell dumpsys activity"); // 確認

            int app_num = CountChar(Anser, "Run #");

            int cnt = int.Parse(ComboBox1.Text);

            for(int i = 1; i < cnt; i++)
            {
                Label1.Content = i.ToString();

                Anser = CommandExec(@"shell input touchscreen swipe 100 600 100 600 1000");// 長押し
                await Task.Delay(wt);
                Anser = CommandExec(@"shell dumpsys activity"); // 確認
                TextBox1.Text = Anser;

                //                await Task.Delay(500);
                if (CountChar(Anser, "Run #") == app_num && CountChar(Anser, ".smartnews.") == 1)
                {
                    await Task.Delay(wt);
                    Anser = CommandExec(@"shell input touchscreen tap 200 1285"); // urlクリック
                }
                else
                {
                    await Task.Delay(wt);
                    Anser = CommandExec(@"shell input keyevent KEYCODE_BACK "); // 戻る
                }
                await Task.Delay(wt);
                Anser = CommandExec(@"shell input swipe 500 1000 300 800"); // スクロールダウン

                await Task.Delay(wt);

            }


        }

        private void Button_Loop_2_Click(object sender, RoutedEventArgs e)
        {
            thread = new Thread(new ThreadStart(ThreadProc));

            int cnt = int.Parse(ComboBox1.Text);

            for (int i = 1; i < cnt; i++)
            {
                thread.Start();
            }
        }

        private async void ThreadProc()
        {
            String Anser = "";

            int wt = int.Parse(ComboBox2.Text);

            Anser = CommandExec(@"shell input touchscreen swipe 100 600 100 600 1000");// 長押し
            await Task.Delay(wt);
            Anser = CommandExec(@"shell dumpsys activity"); // 確認
            TextBox1.Text = Anser;

            //                await Task.Delay(500);
            int app_num = CountChar(Anser, "Run #");
            if (CountChar(Anser, "Run #") == app_num && CountChar(Anser, ".smartnews.") == 1)
            {
                await Task.Delay(wt);
                Anser = CommandExec(@"shell input touchscreen tap 200 1285"); // urlクリック
            }
            else
            {
                await Task.Delay(wt);
                Anser = CommandExec(@"shell input keyevent KEYCODE_BACK "); // 戻る
            }
            await Task.Delay(wt);
            Anser = CommandExec(@"shell input swipe 500 1000 300 800"); // スクロールダウン

            await Task.Delay(wt);
        }

        // 文字の出現回数をカウント
        public static int CountChar(string s, string c)
        {
            return (s.Length - s.Replace(c.ToString(), "").Length) / c.Length;
        }

        private void Button_Dump_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.Text = "";

            TextBox1.Text = CommandExec(@"shell dumpsys activity");
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            thread.Abort();
            thread.Join();
        }

    }
}
