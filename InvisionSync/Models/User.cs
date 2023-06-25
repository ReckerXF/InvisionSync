using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace InvisionSync.Server.Models
{
    internal class User
    {
        public int forumId { get; set; }
        public string forumName { get; set; }
        public int primaryGroupId { get; set; }
        public List<int> secondaryGroupIds { get; set; }
        public string steamHex { get; set; }
    }
}
