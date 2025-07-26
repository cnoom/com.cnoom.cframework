using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor.TableImporter
{
    public class TableImporterWindow : EditorWindow
    {
        private string tablePath = "Assets/Tables";
        private string classOutputPath = "Assets/Scripts/Generated/Tables";
        private string jsonOutputPath = "Assets/StreamingAssets/Tables";

        [MenuItem("FrameWork/表格导入工具")]
        static void OpenWindow() => GetWindow<TableImporterWindow>("表格导入工具");

        void OnGUI()
        {
            GUILayout.Label("表格导入工具", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            tablePath = EditorGUILayout.TextField("表格目录路径", tablePath);
            if (GUILayout.Button("选择表格目录"))
            {
                OnClickTableButton();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            classOutputPath = EditorGUILayout.TextField("类输出路径", classOutputPath);
            if (GUILayout.Button("选择类输出路径"))
            {
                OnClickClassButton();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            jsonOutputPath = EditorGUILayout.TextField("JSON输出路径", jsonOutputPath);
            if (GUILayout.Button("选择JSON输出路径"))
            {
                OnClickJsonButton();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("开始导入"))
            {
                ImportTables();
            }
        }

        void OnClickTableButton()
        {
            tablePath = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, tablePath);
        }

        void OnClickClassButton()
        {
            classOutputPath = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, classOutputPath);
        }

        void OnClickJsonButton()
        {
            jsonOutputPath = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, jsonOutputPath);
        }

        void ImportTables()
        {
            ProcessTables("*.csv", CsvReader.Read);
            ProcessTables("*.xlsx", ExcelReader.Read);

            AssetDatabase.Refresh();
            Debug.Log("表格导入完成");
        }

        void ProcessTables(string pattern, System.Func<string, TableData> reader)
        {
            var files = Directory.GetFiles(tablePath, pattern, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var table = reader(file);
                var className = Path.GetFileNameWithoutExtension(file);
                TableToClassGenerator.Generate(className, table.headers, table.types, classOutputPath);
                TableToJsonExporter.Export(className, table.headers, table.types, table.rows, jsonOutputPath);
            }
        }
    }
}