using CnoomFrameWork.Base.Events;
using CnoomFrameWork.Core.UnityExtensions;
using UnityEngine;
using UnityEngine.Scripting;

public class GameObjectRegisterOnTrigger : MonoBehaviour
{
    private void Awake()
    {
        EventManager.Register(this);
    }

    private void OnDestroy()
    {
        EventManager.Unregister(this);
    }

    [EventSubscriber(typeof(UnityContainer))]
    [Preserve]
    private void OnTrigger(UnityContainer unityContainer)
    {
        if (unityContainer.SceneName != gameObject.scene.name) return;
        unityContainer.AddGameObject(gameObject.name, gameObject);
        EventManager.Unregister(this);
    }
}