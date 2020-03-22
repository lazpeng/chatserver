using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models
{
    public class ServerInfoModel
    {
        public int ApiVersion { get; } = 1;
        public int MajorVersion { get; } = 1;
        public int MinorVersion { get; } = 0;
        public int Revision { get; } = 0;
        public string VersionSuffix { get; } = "";
        public string FullVersion 
        { 
            get
            {
                var ver = $"{MajorVersion}.{MinorVersion}.{Revision}";
                if(! string.IsNullOrEmpty(VersionSuffix))
                {
                    ver += $"-{VersionSuffix}";
                }
                return ver;
            }
        }
    }
}
