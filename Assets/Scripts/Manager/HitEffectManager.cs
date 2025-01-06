using Controller;
using System.Collections;
using UnityEngine;
using UtilityCode.ObjectPool;
using UtilityCode.Singleton;

namespace Manager
{
    public class HitEffectManager : MonoBehaviourSingleton<HitEffectManager>
    {
        public ObjectPoolQueue<HitEffectController> hitEffect; //打击对象的对象池

        /// <summary>
        ///     唤醒
        /// </summary>
        protected override void OnAwake()
        {
            hitEffect = new ObjectPoolQueue<HitEffectController>(AssetManager.Instance.hitEffect, 4,
                transform); //初始化，默认为4个，作为自己的儿子
        }

        /// <summary>
        ///     播放打击特效
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="color"></param>
        public void PlayHitEffect(Vector2 position, Quaternion rotation, Color color)
        {
            StartCoroutine(Play(position, rotation, color)); //开始这个协程
        }

        /// <summary>
        ///     play携程
        /// </summary>
        /// <param name="position">打击特效的位置</param>
        /// <param name="rotation">打击特效的旋转</param>
        /// <param name="color">打击特效的颜色</param>
        /// <returns></returns>
        private IEnumerator Play(Vector2 position, Quaternion rotation, Color color)
        {
            HitEffectController hitEffectObject = hitEffect.PrepareObject(); //召唤一个出来
            hitEffectObject.transform.SetPositionAndRotation(position, rotation); //设置位置和旋转
            hitEffectObject.transform.localScale = Vector3.one * ValueManager.Instance.hitEffectScale; //设置缩放
            hitEffectObject.spriteRenderer.color = color; //设置颜色
            yield return new WaitForSeconds(.6f); //打击特效时长是0.5秒，0.6秒是为了兼容误差
            hitEffect.ReturnObject(hitEffectObject); //返回到对象池
        }
    }
}