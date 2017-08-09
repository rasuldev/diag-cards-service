using EaisApi;

namespace Tests.EaisApi
{
    public class MemoryStorage: IUserStorage
    {
        private ApiUserData _data;

        public MemoryStorage()
        {
        }

        public ApiUserData LoadData()
        {
            return _data;
        }

        public void SaveData(ApiUserData data)
        {
            _data = data;
        }
    }
}