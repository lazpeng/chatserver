namespace ChatServer.Models
{
    public class ServerInfoModel
    {
        public int ApiVersion { get; } = 3;
        public int MajorVersion { get; } = 1;
        public int MinorVersion { get; } = 2;
        public int Revision { get; } = 0;
        public string VersionSuffix { get; } = "beta";
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
