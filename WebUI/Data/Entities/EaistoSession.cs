using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Data.Entities
{
    public class EaistoSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Cookies { get; set; }
        public string CaptchaId { get; set; }
    }
}
