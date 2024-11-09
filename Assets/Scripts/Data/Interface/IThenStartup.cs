public interface IThenStartup
{
    public void ServerInit() { }
    public void ClientInit(string ipAddress) { }
    public delegate void OnDeviceCountChanged(int deviceCount);
    public event OnDeviceCountChanged onDeviceCountChanged;
}