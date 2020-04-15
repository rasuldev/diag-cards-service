using EaisApi;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WebUI.Services
{
    public class SessionStorage: IUserStorage
    {
        private const string UserDataKey = "UserData";
        private readonly ISession _session;

        public SessionStorage(ISession session)
        {
            _session = session;
        }

        public async Task<ApiUserData> LoadData()
        {
            var data = _session.GetString(UserDataKey);
            return data == null ? null : JsonConvert.DeserializeObject<ApiUserData>(data);
        }

        public async Task SaveData(ApiUserData data)
        {
            _session.SetString(UserDataKey, JsonConvert.SerializeObject(data));
        }
    }
}