using Controller;
using Data.ChartData;
using UnityEngine;
using UnityEngine.UI;
using UtilityCode.Singleton;

namespace Manager
{
    public class AssetManager : MonoBehaviourSingleton<AssetManager>
    {
        [Header("铺面数据")] public ChartData chartData;

        [Header("音乐播放")] public AudioSource musicPlayer;
        //public Koreography MusicPlayer=>musicPlayer.GetKoreographyAtIndex(0);

        [Header("方框以及他们的爹爹~")] public Transform box;

        public BoxController boxController;

        [Header("文字的预制件")] public TextController text;

        [Header("音符萌~")] public NoteController[] noteControllers;

        [Header("打击特效的预制件")] public HitEffectController hitEffect;

        [Header("方框波纹特效预制件")] public RippleController ripple;

        [Header("打击音效预制件")] public HitSoundController hitSoundController;

        [Header("背景")] public Image background;
    }
}