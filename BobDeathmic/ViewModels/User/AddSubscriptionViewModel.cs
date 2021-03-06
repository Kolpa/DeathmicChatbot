﻿using BobDeathmic.Data.DBModels.StreamModels;
using BobDeathmic.Data.Enums.Stream;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BobDeathmic.ViewModels.User
{
    public class AddSubscriptionViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Stream")]
        public string StreamNameForSubscription { get; set; }
        public StreamProviderTypes type { get; set; }
        public List<Stream> SubscribableStreams
        {
            get; set;
        }
        public List<StreamSubscription> Subscriptions { get; set; }
    }
}
