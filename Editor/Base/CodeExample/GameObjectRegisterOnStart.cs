using CnoomFrameWork.Core;
using CnoomFrameWork.Core.UnityExtensions;
using UnityEngine;

public class GameObjectRegisterOnStart : MonoBehaviour
{
    private void Start()
    {
        var childContainer = App.Instance.RootContainer.GetChildContainer(gameObject.scene.name);
        childContainer.Resolve<UnityContainer>().AddGameObject(gameObject);
    }
}