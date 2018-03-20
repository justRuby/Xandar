namespace Xandar.Service
{
    public interface IToast
    {
        void ShowLong(string message);
        void ShowShort(string message);
    }
}
