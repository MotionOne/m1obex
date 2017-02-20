using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

using InTheHand.Net;
using InTheHand.Net.Bluetooth;

namespace m1obex
{
    class Program
    {
        static void Main(string[] args)
        {
            var objex = new m1Obex();
            objex.StartObexListener();

            if (args.Length == 1 && args[0] != null) {
                //Console.WriteLine("args[0].length: {0}", args[0].Length);
                //Console.WriteLine("download_path: {0}", args[0]);
                objex.setDownloadPath(args[0]); // target path
            }

            // Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}", p.Command, p.FileCount, p.FileSize, p.FileName, p.FileIndex, p.CurrentBytes);

            Console.WriteLine("");
            Console.WriteLine("OBEX server starts...");
            Console.Out.Flush();

            //Console.WriteLine("Press any key to exit.");
            //Console.ReadKey();
        }

        // --------------------------------------------------------------------------


        public class m1Obex
        {
            private string download_path;
            private Dispatcher dispatcher;

            private BluetoothRadio radio;
            public BluetoothRadio Radio
            {
                get
                {
                    return radio;
                }
            }

            private ObexListener listener;

            public m1Obex()
            {
                download_path = "c:\\tmp";
            }

            public void setDownloadPath(string path) {
                download_path = path;
            }

            public void StartObexListener()
            {
                radio = InTheHand.Net.Bluetooth.BluetoothRadio.PrimaryRadio;
                if (radio != null)
                {
                    Console.WriteLine("Bluetooth radio on.");
                       
                    radio.Mode = InTheHand.Net.Bluetooth.RadioMode.Discoverable;

                    listener = new ObexListener(ObexTransport.Bluetooth);
                    listener.Start();

                    dispatcher = Dispatcher.CurrentDispatcher;

                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ObexRequestHandler));
                    t.Start();
                }
            }

            private void ObexRequestHandler()
            {
                if (radio == null)
                    return;

                while (listener.IsListening)
                {
                    try
                    {
                        ObexListenerContext_M1 olc = listener.GetContext_M1(download_path, p =>
                        {
                            //Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}", p.Command, p.FileCount, p.FileSize, p.FileName, p.FileIndex, p.CurrentBytes);
                            string filename = "";
                            if (p.FileName != null)
                                filename = p.FileName.Replace("\\", "\\\\");

                            Console.Write("{");
                            Console.Write("  \"command\": \"{0}\",", p.Command);
                            Console.Write("  \"file_count\": {0},", p.FileCount);
                            Console.Write("  \"file_size\": {0},", p.FileSize);
                            Console.Write("  \"file_path\": \"{0}\",", filename);
                            Console.Write("  \"file_index\": {0},", p.FileIndex);
                            Console.Write("  \"file_bytes\": {0}", p.CurrentBytes);
                            Console.Write("}");
                            Console.Write("\n");

                        });
                        //ObexListenerRequest olr = olc.Request;
                        //string filename = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "\\" + DateTime.Now.ToString("yyMMddHHmmss") + " " + Uri.UnescapeDataString(olr.RawUrl.TrimStart(new char[] { '/' }));
                        //olr.WriteFile(filename);
                        //Console.WriteLine("file: " + filename);

                        //dispatcher.Invoke(new Action(delegate()
                        //{
                        //    Console.WriteLine("aaa: ");
                        //}));
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                }
            }

        }
    }
}
