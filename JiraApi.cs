using System.Text;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Newtonsoft.Json;
using MorelJsonJira;

namespace JiraNote {
#nullable disable
    public class JiraApi : IDisposable
    {
        private readonly Repository repo = new();       //Object repo;
        private readonly string repoUser = "pavlo.kuznetsov"; //Variable repoUser;
        private readonly string repoPass = "+Zse4M55(";       //Variable repoPass;
        internal static ModelJsonNote parJSON = MakeParamsFromJSON();

        // ---=== Method ===--- //
        internal static ModelJsonNote MakeParamsFromJSON()
        {
            // Read JSON and Make Params from JSON
            var path = Path.Combine(Environment.CurrentDirectory, "params.json");
            var json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<ModelJsonNote>(json);
        }

        // ---=== Method ===--- //
        // User information to create a merge commit        

        private readonly Signature signature = new(new Identity("MERGE_USER_NAME", "MERGE_USER_EMAIL"), DateTimeOffset.Now);
        // ---=== Method ===--- //
        public Branch CurrentBranch { private set; get; }
        
        // ---=== Method ===--- //
        public void CheckoutBranch(string branch)
        {
            CurrentBranch = Commands.Checkout(repo, repo.Branches[branch]);
        }

        // ---=== Method ===--- //
        public void PullBranch()
        {
            // Credential information to fetch
            PullOptions options = new()
            {
                FetchOptions = new FetchOptions
                {
                    CredentialsProvider = new CredentialsHandler(
                (url, usernameFromUrl, types) =>
                    new UsernamePasswordCredentials()
                    {
                        Username = repoUser,
                        Password = repoPass
                    })
                }
            };
            // Pull
            try
            {
                Commands.Pull(repo, signature, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server is not available\n\n");
                Console.WriteLine(ex.ToString());
            }
        }
        /*~*/


        private static readonly HttpClient client = new();
        internal static ModelJsonJira valJsonJira;

        // ---=== Method ===--- //
        public static async Task ProcessRepositories(string noteFile, List<string> lstIn)
        {

            string jiraAuth = JiraApi.parJSON.JiraURL + "/login.jsp"; 

            Dictionary<string, string> values = new()
            {
                { "os_username", parJSON.JiraLogin },
                { "os_password", parJSON.JiraPass }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(jiraAuth, content);
            await response.Content.ReadAsStringAsync();
            /////////////////////////////////////////////////////////////////////////////
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            string noteWrite = "|".PadRight(304, '-') + "|\n" + "|         Заявка          |       Заявка в ОТ       |" + " ".PadLeft(37) + "Контакт"
                             + " ".PadLeft(36) + "|" + " ".PadLeft(83) + "Опис" + " ".PadLeft(83) + "|\n" + "|".PadRight(304, '-') + "|\n";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Directory.CreateDirectory(noteFile[..noteFile.LastIndexOf(Path.DirectorySeparatorChar)]);
            File.WriteAllText(noteFile, noteWrite, Encoding.GetEncoding("windows-1251"));

            foreach (var item in lstIn)
            {
                var stringTask = client.GetStringAsync(item);
                var msg = await stringTask;

                valJsonJira = JsonConvert.DeserializeObject<ModelJsonJira>(msg);

                noteWrite = "|" + MakeColumn(valJsonJira.Key, 25) + "|"                           //Заявка
                                      + MakeColumn(valJsonJira.Fields.CustomField11111, 25) + "|" //Заявка в ОТ
                                      + MakeColumn(valJsonJira.Fields.CustomField11112, 80) + "|" //Контакт
                                      + MakeColumn(valJsonJira.Fields.Summary, 170) + "|\n";      //Опис
                File.AppendAllText(noteFile, noteWrite, Encoding.GetEncoding("windows-1251"));
            }
            noteWrite = "-".PadRight(304, '-') + "-";
            File.AppendAllText(noteFile, noteWrite, Encoding.GetEncoding("windows-1251"));
        }

        // ---=== Method ===--- //
        // Add
        public void AddFileToRepo(string fileToAdd)
            {
                repo.Index.Add(fileToAdd);
                repo.Index.Write();
            }

            // Commit
            public void CommitRepoChanges(string message)
            {
                repo.Commit(message, signature, signature);
            }

        // ---=== Method ===--- //
        // Erase Object Repository
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // called via myClass.Dispose(). 
                    // OK to use any private object references
                }
                // Release unmanaged resources.
                // Set large fields to null.                
                disposed = true;
            }
        }

        public void Dispose() // Implement IDisposable
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JiraApi() // the finalizer
        {
            Dispose(false);
        }


        /*

        public void Dispose()
            {
                repo.Dispose();
            }
        */


        // ---=== Method ===--- //
        internal static string MakeColumn(string valJson, int width)
        {
            if (valJson == null) valJson = "";
            if (valJson.Length > width) 
                valJson = valJson[..width];
            return valJson.PadRight(width);
        }
    }
}

