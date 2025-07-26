using System.Collections.Generic;
using System.IO;

namespace FrameWork.Editor.TableImporter
{
    public class CsvReader
    {
        public static TableData Read(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var data = new TableData
            {
                headers = new List<string>(lines[0].Split(',')),
                types = new List<string>(lines[1].Split(',')),
                rows = new List<List<string>>()
            };

            for (int i = 2; i < lines.Length; i++)
            {
                var values = new List<string>(lines[i].Split(','));
                data.rows.Add(values);
            }
            return data;
        }
    }
}