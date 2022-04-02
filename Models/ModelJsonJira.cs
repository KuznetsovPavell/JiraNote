namespace MorelJsonJira
{
    public class ModelJsonJira
    {
#nullable disable
        [Newtonsoft.Json.JsonProperty("key")]
        public string Key { get; set; }
        [Newtonsoft.Json.JsonProperty("fields")]
        public CFields Fields { get; set; }
        public class CFields
        {
            [Newtonsoft.Json.JsonProperty("customfield_11111")]
            public string CustomField11111 { get; set; }
            [Newtonsoft.Json.JsonProperty("customfield_11112")]
            public string CustomField11112 { get; set; }
            [Newtonsoft.Json.JsonProperty("summary")]
            public string Summary { get; set; }
        }
    }
}
