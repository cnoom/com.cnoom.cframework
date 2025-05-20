using System.IO;
using FrameWork.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor.Base
{
    public class CodeGameObjectRegisterGenerator
    {
        public const string FileName = "GameObjectRegister";

        [MenuItem("FrameWork/脚本生成/自动注册游戏对象脚本")]
        private static void CreateSceneContainer()
        {


            // 如果文件已存在，则提示用户
            if(File.Exists(FrameWorkConfig.ScriptPath))
            {
                Debug.LogWarning($"脚本 {FileName}.cs 已存在！将覆盖生成!");
            }

            // 脚本内容
            string scriptContent = $@"
using System;
using CnoomFrameWork.Core;
using UnityEngine;

namespace Editor.Base
{{
    /// <summary>
    /// 自动生成的场景容器注册脚本
    /// </summary>
    public class {FileName} : MonoBehaviour
    {{
        private App app;

        private void Awake()
        {{
            app = App.Instance;
            app.RootContainer.CreateChildContainer(gameObject.scene.name);
        }}

        private void OnDestroy()
        {{
            app.RootContainer.RemoveChildContainer(gameObject.scene.name);
        }}
    }}
}}
";

            string filePath = Path.Combine(FrameWorkConfig.ScriptPath, $"{FileName}.cs");
            // 写入文件
            File.WriteAllText(filePath, scriptContent);

            // 刷新 AssetDatabase 以使 Unity 识别新文件
            AssetDatabase.Refresh();

            // 提示用户生成成功
            EditorUtility.DisplayDialog("生成成功", $"脚本 {FileName}.cs 已生成！", "确定");
        }
    }
}