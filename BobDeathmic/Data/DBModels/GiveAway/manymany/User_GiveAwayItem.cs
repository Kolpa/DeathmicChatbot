﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BobDeathmic;
using BobDeathmic.Models;
using BobDeathmic.Data.DBModels.User;

namespace BobDeathmic.Data.DBModels.GiveAway.manymany
{
    public class User_GiveAwayItem
    {
        [Key]
        public int User_GiveAwayItemID;

        public string UserID { get; set; }
        public ChatUserModel User { get; set; }

        public string GiveAwayItemID { get; set; }
        public GiveAwayItem GiveAwayItem { get; set; }

        public User_GiveAwayItem()
        {

        }
        public User_GiveAwayItem(ChatUserModel user, GiveAwayItem item)
        {
            UserID = user.Id;
            User = user;
            GiveAwayItemID = item.Id;
            GiveAwayItem = item;
        }
    }
}
