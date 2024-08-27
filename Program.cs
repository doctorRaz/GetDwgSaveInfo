using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//https://adn-cis.org/forum/index.php?topic=2972.0#msg11337

namespace drz.GetDwgSaveInfo
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                //InitialDirectory = @"D:\",
                Title = "Browse CAD Files",

                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "dwg",
                Filter = "dwg files (*.dwg)|*.dwg",
                FilterIndex = 2,
                RestoreDirectory = true,
                Multiselect = false,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            string[] sdwg = null;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sdwg = openFileDialog1.FileNames;
            }
            else
            {
                Console.WriteLine("return");
                Console.WriteLine("---- Press any key ----");
                Console.ReadKey();
                return;
            }

            //if (args.Length > 0)
            if (sdwg.Length > 0)
            {
                string name = "", build_version = "", registry_version = "", companyname = "";
                string app = GetDwgFileSavedApp(sdwg[0],
                                                ref name,
                                                ref companyname,
                                                ref build_version,
                                                ref registry_version);
                Console.WriteLine("This file was last saved by:\n" +
                    "\tApplication = {0}\n" +
                    "\tName = {1}\n" +
                    "\tCompany Name ={2}\n" +
                    "\tBuild Version = {3}\n" +
                    "\tRegistry Version = {4}\n",
                                  app,
                                  name,
                                  companyname,
                                  build_version,
                                  registry_version);
            }
            else
            {
                Console.WriteLine("DWG-filename not provided");
            }
            Console.WriteLine("---- Press any key ----");
            Console.ReadKey();
        }


        /// <summary>
        /// Получение имени приложения, в котором сохранён dwg-файл.
        /// </summary>
        /// <param name="dwgfilepath">Путь к dwg-файлу.</param>
        /// <param name="name">Имя приложения.</param>
        /// <param name="build_version">Версия exe-файла.</param>
        /// <param name="registry_version">Версия в реестре.</param>
        /// <returns></returns>
        public static string GetDwgFileSavedApp(string dwgfilepath,
                                                ref string name,
                                                ref string companyname,
                                                ref string build_version,
                                                ref string registry_version)
        {
            const string Desc = "<ProductInformation name =";
            try
            {
                string line = System.IO.File.ReadAllText(dwgfilepath, Encoding.Unicode);
                int lineStart = line.IndexOf(Desc);
                int lineEnd = line.IndexOf("\0", lineStart + 1);
                string s = new StringBuilder(line.Substring(lineStart, lineEnd - lineStart))
                  .Replace("\\\"", "\"").ToString();
                using (XmlReader reader = XmlReader.Create(new StringReader(s)))
                {
                    reader.Read();
                    name = reader.GetAttribute("name");
                    companyname = reader.GetAttribute("CompanyName");
                    build_version = reader.GetAttribute("build_version");
                    registry_version = reader.GetAttribute("registry_version");
                    return reader.GetAttribute("install_id_string");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };

        }
    }
}