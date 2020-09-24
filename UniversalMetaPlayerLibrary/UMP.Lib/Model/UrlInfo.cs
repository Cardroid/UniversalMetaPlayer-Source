namespace UMP.Lib.Model
{
    public class UrlInfo
    {
        public UrlInfo(string url, bool success, string protocol, string domain, int? port)
        {
            if (string.IsNullOrWhiteSpace(url))
                this.Success = false;
            else
                this.Success = success;

            this.Url = url;
            this.Protocol = protocol;
            this.Domain = domain;
            this.Port = port;
        }

        public UrlInfo(string url, bool success = false)
        {
            this.Success = success;
            this.Url = url;
            this.Protocol = string.Empty;
            this.Domain = string.Empty;
            this.Port = null;
        }

        /// <summary>
        /// 성공여부 (Url 형식이 아니면 false)
        /// </summary>
        public bool Success { get; }
        /// <summary>
        /// 검사한 Url
        /// </summary>
        public string Url { get; }
        /// <summary>
        /// Url의 프로토콜
        /// </summary>
        public string Protocol { get; }
        /// <summary>
        /// Url의 도메인
        /// </summary>
        public string Domain { get; }
        /// <summary>
        /// Url의 포트번호
        /// </summary>
        public int? Port { get; }
    }
}
