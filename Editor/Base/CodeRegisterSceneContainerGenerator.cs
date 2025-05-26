using UnityEditor;
using System.IO;
using Editor.Base;
using UnityEngine;

namespace FrameWork.Editor.Base
{
    public static class CodeRegisterSceneContainerGenerator
    {
        public const string FileName = "RegisterSceneContainer";
        
        [MenuItem("FrameWork/脚本生成/生成场景容器脚本")]
        private static void CreateSceneContainer()
        {
            // 如果文件已存在，则提示用户
            if(File.Exists(FrameWorkConfig.ScriptPath))
            {
                Debug.LogWarning($"脚本 {FileName}.cs 已存在！将覆盖生成!");
            }

            // 脚本内容
            string scriptContent = $@"using CnoomFrameWork.Core;
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

            CodeCreator.CreateScript(FileName, scriptContent);
        }
    }
}