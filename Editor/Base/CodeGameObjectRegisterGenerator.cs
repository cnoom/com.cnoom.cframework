using UnityEditor;

namespace Editor.Base
{
    public class CodeGameObjectRegisterGenerator
    {
        public const string FileName = "GameObjectRegisterOnStart";

        [MenuItem("cnoom/cframework/脚本生成/自动注册游戏对象脚本(Start)")]
        private static void CreateGameObjectRegisterOnStart()
        {
            var fileName = "GameObjectRegisterOnStart";
            var scriptContent = $@"using CnoomFrameWork.Base.Container;
using CnoomFrameWork.Core;
using CnoomFrameWork.Core.UnityExtensions;
using UnityEngine;

public class {fileName} : MonoBehaviour
{{
    private void Start()
    {{
        ChildContainer childContainer = App.Instance.RootContainer.GetChildContainer(gameObject.scene.name);
        childContainer.Resolve<UnityContainer>().AddGameObject(gameObject);
    }}
}}";
            CodeCreator.CreateScript(fileName, scriptContent);
        }


        [MenuItem("cnoom/cframework/脚本生成/自动注册游戏对象脚本(Trigger)")]
        private static void CreateGameObjectRegisterOnTrigger()
        {
            var fileName = "GameObjectRegisterOnTrigger";
            var scriptContent = $@"using CnoomFrameWork.Base.Events;
using CnoomFrameWork.Core.UnityExtensions;
using UnityEngine;
using UnityEngine.Scripting;

public class {fileName} : MonoBehaviour
{{
    private void Awake()
    {{
        EventManager.Register(this);
    }}

    [EventSubscriber(typeof(UnityContainer)),Preserve]
    private void OnTrigger(UnityContainer unityContainer)
    {{
        if (unityContainer.SceneName != gameObject.scene.name) return;
        unityContainer.AddGameObject(gameObject.name, gameObject);
        EventManager.Unregister(this);
    }}

    private void OnDestroy()
    {{
        EventManager.Unregister(this);
    }}
}}";
            CodeCreator.CreateScript(fileName, scriptContent);
        }
    }
}