using Infrastructure.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web_app
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public List<UserEntity> Users { get; set; }
    }
}
