namespace Data.Interface
{
    public interface IThenStartup
    {
        public delegate void OnDeviceCountChanged(int deviceCount);

        public void ServerInit()
        {
        }

        public void ClientInit(string ipAddress)
        {
        }

        public event OnDeviceCountChanged onDeviceCountChanged;
    }
}