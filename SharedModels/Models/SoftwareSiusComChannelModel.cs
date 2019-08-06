using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Models
{
    public class SoftwareSiusComChannelModel
    {
        public string Name { get; set; }
        public List<SoftwareSiusComFileModel> Files { get; set; } = new List<SoftwareSiusComFileModel>();
    }
}
