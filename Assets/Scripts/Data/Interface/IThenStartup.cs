public interface IThenStartup
{
    public void Init(bool isServer);
    public delegate void OnDeviceCountChanged(int deviceCount);
    public event OnDeviceCountChanged onDeviceCountChanged;
}