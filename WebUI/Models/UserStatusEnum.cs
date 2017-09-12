using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Models
{
    public enum UserStatusEnum
    {
        [Display(Name = "Все")]
        All = 0,
        Waiting = 1,
        Accepted = 2,
        Rejected = 3,
        Deleted = 4
    }
}
