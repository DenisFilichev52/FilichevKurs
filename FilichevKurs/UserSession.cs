using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilichevKurs
{
    public static class UserSession
    {
        public static int CurrentUserId { get; set; }
        public static string CurrentUserName { get; set; }
        public static string CurrentUserRole { get; set; }
        public static bool IsLoggedIn { get; set; }

        public static void Clear()
        {
            CurrentUserId = 0;
            CurrentUserName = string.Empty;
            CurrentUserRole = string.Empty;
            IsLoggedIn = false;
        }
    }
}