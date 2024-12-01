using UtilityCode.Singleton;

namespace Manager
{
    public class JudgeManager : MonoBehaviourSingleton<JudgeManager>
    {
        public const float Perfect = .06f; //完美判定±60ms
        public const float Good = .10f; //Good判定±100ms
        public const float Bad = .16f; //Bad判定±160ms
    }
}