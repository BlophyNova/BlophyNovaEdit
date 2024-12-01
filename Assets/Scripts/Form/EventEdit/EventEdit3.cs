namespace Form.EventEdit
{
    public partial class EventEdit
    {
        public delegate void OnBoxRefreshed(object content);

        public event OnBoxRefreshed onBoxRefreshed = c => { };
    }
}