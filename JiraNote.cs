using LibGit2Sharp;
using System.Text.RegularExpressions;

namespace JiraNote
{
#nullable disable
    public class JiraNote
    {
        // ---=== Method MAIN ===--- //
        static async Task Main(string[] args)
        {
            string repoPath = JiraApi.parJSON.GitRepoDir;
            string tagOld = args[0];        //"BugFix_2.0.111.1.24"; -> tagNew = "BugFix_2.0.111.1.36";
            string tagNew = args[1];        //"bugfix";
            string releaseNo = args[2];     // "BugFix_2.0.111.1.25"
            string targetBranch = args[3];  // "bugfix";

            string patternBranch = $"^Merge.* into '{targetBranch}'";
            string patternCommit = "COBU([A-Z]|[a-z])*(-|_)\\d{1,}";
            string releaseName = null;
            
            foreach (var item in JiraApi.parJSON.Dbconnect)
                if (item.TargetBranch == targetBranch)
                   releaseName = item.ReleaseName;
            
            string note = $"{JiraApi.parJSON.ReleasesDir}{Path.DirectorySeparatorChar}{releaseName}_{releaseNo}{Path.DirectorySeparatorChar}release_note.txt";

            //////////////////////////////////////////////////////////////////////////////////////

            Repository repo = new(repoPath);
            Tag tagToTag = repo.Tags[tagNew];
            Tag tagFromTag = repo.Tags[tagOld];

            var filter = new CommitFilter();

            if (targetBranch == tagNew)
            {
                filter.IncludeReachableFrom = repo.Head.Tip.Sha;
            }
            else
            {
                filter.IncludeReachableFrom = tagToTag.Target.Sha;
            }
            filter.ExcludeReachableFrom = tagFromTag.Target.Sha;
            var commitsOut = repo.Commits.QueryBy(filter).ToList();
            
            List<string> valueList = new();

            foreach (var commit in commitsOut)
            {
                foreach (var valueChar in Regex.Matches(commit.MessageShort, patternBranch, RegexOptions.Multiline))
                {
                    string valueString = valueChar.ToString();
                    valueString = Regex.Match(valueString, patternCommit).Value;

                    if (valueString != "")
                        valueList.Add($"{JiraApi.parJSON.JiraURL}/{JiraApi.parJSON.JiraURLAdd}/{valueString}");
                }
            }
            valueList = valueList.Distinct().ToList();
            await JiraApi.ProcessRepositories(note, valueList);
        }
    }
}
