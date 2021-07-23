using System;
using System.Collections.Generic;

namespace HafezWPFUI.Models
{
    public class UserDisplayModel
    {
        private char _statue;
        private char _userType;

        public int    Id       { get; set; }
        public string UserId   { get; set; }
        public string Password { get; set; }
        public string Name     { get; set; }

        public char Statue
        {
            get => _statue;
            set
            {
                _statue = value;

                StatusColor = StatueColDict[Convert.ToChar(value)];

                StatusName = value switch
                {
                    'E' => "فعال",
                    'D' => "غیرفعال",
                    _   => StatusName
                };
            }
        }

        public char UserType
        {
            get => _userType;
            set
            {
                _userType     = value;
                UserTypeColor = UserTypeColDict[Convert.ToChar(value)];

                UserTypeName = value switch
                {
                    'S' => "",
                    'A' => "ادمین",
                    'U' => "کاربر عادی",
                    _   => UserTypeName
                };
            }
        }

        public string StatusColor   { get; private set; }
        public string StatusName    { get; private set; }
        public string UserTypeColor { get; private set; }
        public string UserTypeName  { get; private set; }
        public string Command       { get; set; }

        public string ReceiveTime { get; set; } = DateTime.Now.ToLongTimeString();
        //public bool IsUserValid { get; private set; }


        private Dictionary<char, string> StatueColDict { get; } = new()
        {
            /*{'E', "#006400"},
            {'D', "#FF0000"}*/
            {'E', "LimeGreen"}, {'D', "Red"}
        };

        private Dictionary<char, string> UserTypeColDict { get; } = new()
        {
            /*{'S', "#000000"},
            {'A', "#000000"},
            {'U', "#FFFF00"}*/
            {'S', "Black"}, {'A', "Black"}, {'U', "Yellow"}
        };
    }
}