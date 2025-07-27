using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor.TableImporter
{
    public class TableImporterWindow : EditorWindow
    {
        private static string _tablePath = "Assets/Tables";
        private static string _classOutputPath = "Assets/Scripts/Generated/Tables";
        private static string _jsonOutputPath = "Assets/StreamingAssets/Tables";

        [MenuItem("FrameWork/表格导入工具")]
        static void OpenWindow() => GetWindow<TableImporterWindow>("表格导入工具");

        void OnGUI()
        {
            GUILayout.Label("表格导入工具", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            _tablePath = EditorGUILayout.TextField("表格路径", _tablePath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("选择表格路径"))
            {
                OnClickTableButton();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _classOutputPath = EditorGUILayout.TextField("类输出目录", _classOutputPath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("选择类输出目录"))
            {
                OnClickClassButton();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _jsonOutputPath = EditorGUILayout.TextField("JSON输出目录", _jsonOutputPath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("选择JSON输出目录"))
            {
                OnClickJsonButton();
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("开始导入"))
            {
                ImportTable();
            }
        }

        void OnClickTableButton()
        {
            string[] filters = new string[] { "Excel Files", "xlsx,csv", "All Files", "*" };
            _tablePath = EditorUtility.OpenFilePanelWithFilters("选择文件", Application.dataPath, filters);
        }

        void OnClickClassButton()
        {
            _classOutputPath = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, _classOutputPath);
        }

        void OnClickJsonButton()
        {
            _jsonOutputPath = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, _jsonOutputPath);
        }

        void ImportTable()
        {
            //判断文件类型
            if (Path.GetExtension(_tablePath) == ".csv")
            {
                ProcessTable(CsvReader.Read);
            }
            else if (Path.GetExtension(_tablePath) == ".xlsx")
            {
                ProcessTable(ExcelReader.Read);
            }

            AssetDatabase.Refresh();
            Debug.Log("表格导入完成");
        }

        void ProcessTable(System.Func<string, TableData> reader)
        {
            var table = reader(_tablePath);
            var className = Path.GetFileNameWithoutExtension(_tablePath);
            TableToClassGenerator.Generate(className, table.headers, table.types, _classOutputPath);
            TableToJsonExporter.Export(className, table.headers, table.types, table.rows, _jsonOutputPath);
        }
    }
}