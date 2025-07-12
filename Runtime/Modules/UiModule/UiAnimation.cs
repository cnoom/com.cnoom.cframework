using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Modules.UiModule;
using UnityEngine;

namespace Modules.UiModule
{
    public class UiAnimation : MonoBehaviour
    {
        public bool hasOpenAnimation = true;
        public bool hasCloseAnimation = true;
        
        private Coroutine _currentAnimation;
        protected List<CanvasGroup> Animations;
        
        public virtual void InitAnimationPart()
        {
            Animations = new List<CanvasGroup>();
            // 为需要动画的子对象初始化CanvasGroup（跳过标记忽略的）
            foreach (Transform child in transform)
            {
                if(child == transform) continue;
                if(!child.TryGetComponent<IgnorePanelAnimation>(out _))
                {
                    if(!child.TryGetComponent(out CanvasGroup cg))
                    {
                        cg = child.gameObject.AddComponent<CanvasGroup>();
                    }
                    Animations.Add(cg);
                }
            }
        }
        /// <summary>
        /// 打开ui动画
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator PlayEnterAnimation()
        {
            yield return StartCoroutine(CombineAnimations(
                FadeAnimation(0, 1, 0.2f),
                ScaleAnimation(Vector3.one * 0.8f, Vector3.one, 0.2f)
            ));
        }
        
        /// <summary>
        /// 关闭ui动画
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator PlayExitAnimation()
        {
            // 默认动画：渐隐+缩小
            yield return StartCoroutine(CombineAnimations(
                FadeAnimation(1, 0, 0.15f),
                ScaleAnimation(Vector3.one, Vector3.one * 0.8f, 0.15f)
            ));
        }
        
        protected IEnumerator FadeAnimation(float from, float to, float duration)
        {
            foreach (CanvasGroup cg in Animations) cg.alpha = from;

            float timer = 0;
            while (timer < duration)
            {
                foreach (CanvasGroup cg in Animations)
                {
                    cg.alpha = Mathf.Lerp(from, to, timer / duration);
                }

                timer += Time.deltaTime;
                yield return null;
            }

            foreach (CanvasGroup cg in Animations) cg.alpha = to;
        }

        protected IEnumerator ScaleAnimation(Vector3 from, Vector3 to, float duration)
        {
            // 获取所有需要动画的子对象（排除标记忽略的）
            IEnumerable<Transform> targets = Animations.Select(cg => cg.transform);

            IEnumerable<Transform> transforms = targets as Transform[] ?? targets.ToArray();
            foreach (Transform t in transforms) t.localScale = from;

            float timer = 0;
            while (timer < duration)
            {
                foreach (Transform t in transforms)
                {
                    t.localScale = Vector3.Lerp(from, to, timer / duration);
                }

                timer += Time.deltaTime;
                yield return null;
            }

            foreach (Transform t in transforms) t.localScale = to;
        }

        // 组合动画协程
        protected IEnumerator CombineAnimations(params IEnumerator[] animations)
        {
            _currentAnimation = StartCoroutine(RunCombinedAnimations(animations));
            yield return _currentAnimation;
        }

        private IEnumerator RunCombinedAnimations(IEnumerator[] animations)
        {
            List<Coroutine> running = new List<Coroutine>();
            foreach (IEnumerator anim in animations)
            {
                running.Add(StartCoroutine(anim));
            }

            foreach (Coroutine coroutine in running)
            {
                yield return coroutine;
            }
        }
    }
}