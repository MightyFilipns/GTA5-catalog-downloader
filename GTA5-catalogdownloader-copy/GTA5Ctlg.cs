using Ionic.Zlib;
using System.Net;
using System.Text;

namespace GTA5_catalogdownloader_copy
{
    internal class GTA5Ctlg
    {
        const string getcatalogurl = "https://prod.p01ams.pod.rockstargames.com/gta5/11/GamePlayServices/GameTransactions.asmx/GetCatalog"; // not implemented requreds a RSACCESS token passed in the Authorization header
        const string catalogprefix = "https://prod.cloud.rockstargames.com/titles/gta5/pcros/gamecatalog/";
        static string catalog_name = "WkYi7fg8KlWWys5_1FFRHw"; // -- changes atleast weekly or mroe frequently 
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                catalog_name = args[0];
            }
            using (var client = new HttpClient())
            {
                Task<Stream> filedown = client.GetStreamAsync(catalogprefix + catalog_name + ".zip");
                int dots = 1;
                Console.WriteLine("Downloading");
                while (!filedown.IsCompleted)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);


                    Console.WriteLine("Downloading " + Encoding.UTF8.GetString(Enumerable.Repeat('.', dots).Select(a => (byte)a).ToArray()) + "    ");
                    Thread.Sleep(500);
                    dots = dots == 3 ? 1 : dots + 1;
                }
                Console.WriteLine("Decompressing ....");
                byte[] compresseddata = ReadToEndBytes(filedown.Result);
                MemoryStream ms = new MemoryStream(compresseddata);
                DeflateStream df1 = new(ms, CompressionMode.Decompress, CompressionLevel.Level9);
                byte[] decompressed = ReadToEndBytes(df1);
                string towrite = Environment.CurrentDirectory + "\\" + catalog_name + ".json";
                File.WriteAllBytes(towrite, decompressed);
                Console.WriteLine($"Written to {towrite}");
            }
        }
        public static byte[] ReadToEndBytes(Stream str)
        {
            var str2 = new StreamReader(str);
            List<byte> bytes = new List<byte>();
            int bt = -1;
            do
            {
                bt = str.ReadByte();
                if (bt != -1)
                {
                    bytes.Add((byte)bt);
                }
            } while (bt != -1);
            return bytes.ToArray();
        }
    }
}
