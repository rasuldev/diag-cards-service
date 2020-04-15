using EaisApi;
using System.Threading.Tasks;

namespace Tests.EaisApi
{
    public class MemoryStorage: IUserStorage
    {
        private ApiUserData _data;

        public MemoryStorage()
        {
        }

        public async Task<ApiUserData> LoadData()
        {
            return _data;
        }

        public async Task SaveData(ApiUserData data)
        {
            _data = data;
        }
    }
}