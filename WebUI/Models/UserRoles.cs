using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Models
{
    public static class UserRoles
    {
        //public const string Social = "Social";
        public const string Local = "Local";
        public const string Admin = "Administrator";
        public const string Spectator = "Spectator";

        public const string All = Admin + "," + Spectator + "," + Local;
        public const string AdminAndSpectator = Admin + "," + Spectator;
        public const string NotSpectator = Admin + "," + Local;
    }
}
