namespace JiraNote
{
    public class ModelJsonNote
    {
#nullable disable
        [Newtonsoft.Json.JsonProperty("jira_url")]
        public string JiraURL { get; set; }
        [Newtonsoft.Json.JsonProperty("jira_url_add")]
        public string JiraURLAdd { get; set; }
        [Newtonsoft.Json.JsonProperty("jira_login")]
        public string JiraLogin { get; set; }
        [Newtonsoft.Json.JsonProperty("jira_pass")]
        public string JiraPass { get; set; }
        [Newtonsoft.Json.JsonProperty("gmail_login")]
        public string GmailLogin { get; set; }
        [Newtonsoft.Json.JsonProperty("gmail_pass")]
        public string GmailPass { get; set; }
        [Newtonsoft.Json.JsonProperty("git_repo_dir")]
        public string GitRepoDir { get; set; }
        [Newtonsoft.Json.JsonProperty("releases_dir")]
        public string ReleasesDir { get; set; }
        [Newtonsoft.Json.JsonProperty("note_file")]
        public string NoteFile { get; set; }

        public DbConnect[] Dbconnect { get; set; }

    }
    public class DbConnect
    {
        [Newtonsoft.Json.JsonProperty("target_branch")]
        public string TargetBranch { get; set; }
        [Newtonsoft.Json.JsonProperty("release_name")]
        public string ReleaseName { get; set; }
    }
}


