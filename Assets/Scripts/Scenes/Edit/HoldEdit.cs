namespace Scenes.Edit
{
    public class HoldEdit : NoteEditItem
    {
        public LengthAdjustment start;
        public LengthAdjustment end;
        protected override void OnStart()
        {
        }

        public override void SetStartAndEndVisibility(bool visibility)
        {
            start.gameObject.SetActive(visibility);
            end.gameObject.SetActive(visibility);
        }
    }
}