using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Models
{
    public class SoftwareSiusComFileModel
    {
        public string Name { get; set; }
        public string SiusComponentName { get; set; }
        public string DownloadFileName { get; set; }
        public string Url { get; set; }
        public long Size { get; set; }
        public string Notes { get; set; }
        public string Version { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public bool IsSameFile(SoftwareSiusComFileModel other)
        {
            if (string.IsNullOrWhiteSpace(SiusComponentName))
                return false;

            if (string.IsNullOrWhiteSpace(Version))
                return false;

            if (other == null)
                return false;

            if (string.IsNullOrWhiteSpace(other.SiusComponentName))
                return false;

            if (string.IsNullOrWhiteSpace(other.Version))
                return false;

            if (!Version.Equals(
                other.Version,
                StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        public static SoftwareSiusComFileModel Emtpy()
        {
            return new SoftwareSiusComFileModel();
        }
    }
}
