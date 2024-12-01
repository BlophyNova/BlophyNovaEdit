namespace Form.NoteEdit
{
    public partial class NoteEdit
    {
        public delegate void OnBoxRefreshed(object content);

        public event OnBoxRefreshed onBoxRefreshed = c => { };
    }
}