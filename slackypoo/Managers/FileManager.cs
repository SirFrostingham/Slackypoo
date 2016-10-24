
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace slackypoo.Managers
{
    public class FileManager
    {

        public const string DataFilename = @"_{0}_data.json";

        public void DeletePreviousDataFile()
        {
            var file = string.Format(DataFilename, "CurrentEmailIds");

            //delete previous file
            File.Delete(file);
        }

        public static void SaveDataToDisk()
        {
            if (Globals.Current.CurrentEmailIds == null) return;

            FileStream fs;

            var file = string.Format(DataFilename, "CurrentEmailIds");

            //delete previous file
            File.Delete(file);

            using (fs = File.Open(file, FileMode.OpenOrCreate))
            {
                WriteDataStreamJson(Globals.Current.CurrentEmailIds, fs);
            }
        }

        private static void WriteDataStreamJson(List<string> data, FileStream fs)
        {
            //Optional: Async write, but would need to test it
            //App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            //{
            //    using (var sw = new StreamWriter(fs))
            //    using (JsonWriter jw = new JsonTextWriter(sw))
            //    {
            //        jw.Formatting = (Newtonsoft.Json.Formatting)Formatting.Indented;
            //        var serializer = new JsonSerializer();
            //        serializer.Serialize(jw, data.ToList());
            //        //jw.WriteRaw(result);
            //    }
            //});

            using (var sw = new StreamWriter(fs))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = (Newtonsoft.Json.Formatting)Formatting.Indented;
                var serializer = new JsonSerializer();
                serializer.Serialize(jw, data.ToList());
            }
        }

        public static void GetData()
        {
            var file = string.Format(DataFilename, "CurrentEmailIds");

            if (File.Exists(file))
            {
                Globals.Current.CurrentEmailIds = ReadDataStreamJson(file);
            }

            if (Globals.Current.CurrentEmailIds == null)
                Globals.Current.CurrentEmailIds = new List<string>();
        }

        private static List<string> ReadDataStreamJson(string fs)
        {
            var data = new List<string>();
            using (var r = new StreamReader(fs))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<string>>(json);
            }
            return data;
        }
    }
}
