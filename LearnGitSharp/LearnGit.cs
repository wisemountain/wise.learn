using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace gitsharp
{
    class Program
    {
        private static CloneOptions co; 

        static void Main(string[] args)
        {
            co = new CloneOptions();
            co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = "npangunion@gmail.com", Password = "" };

            Clone();
            Pull();
            AddFile();
            Push();
        }

        static void Clone()
        {

            try
            {
                // URL, 로컬 폴더명, 옵션 
                string m = Repository.Clone("https://github.com/npangunion/wise.scratchpad.git", "Pad", co);
                Console.WriteLine(m);
            }
            catch( Exception e)
            {

            }
        }

        static void Pull()
        {
            var repo = new Repository("Pad");

            // Credential information to fetch
            PullOptions options = new PullOptions();
            options.FetchOptions = new FetchOptions();
            options.FetchOptions.CredentialsProvider = new CredentialsHandler(
                (url, usernameFromUrl, types) =>
                    new UsernamePasswordCredentials()
                    {
                        Username = "npangunion@gmail.com",
                        Password = "" 
                    });

            // User information to create a merge commit
            var signature = new LibGit2Sharp.Signature(
                new Identity("npangunion", "npangunion@gmail.com"), DateTimeOffset.Now);

            // Pull
            var mergeResult = Commands.Pull(repo, signature, options);

            Console.WriteLine(mergeResult.Status);

            if ( mergeResult.Commit != null )
            {
                Console.WriteLine(mergeResult.Commit.Message);
            }
        }

        static void AddFile()
        {
            var file = CreateChangeFile();
            using (var repo = new Repository("Pad"))
            {
                // Stage the file
                repo.Index.Add(file);
                repo.Index.Write();

                // Write content to file system
                var content = "Commit this!";

                // Create the committer's signature and commit
                Signature author = new Signature("Agent", "@agent.com", DateTime.Now);
                Signature committer = author;

                // Commit to the repository
                Commit commit = repo.Commit("Here's a commit i made!", author, committer);
            }
        }

        static void Push()
        {
            using (var repo = new Repository("Pad"))
            {
                Remote remote = repo.Network.Remotes["origin"];

                var options = new PushOptions();
                options.CredentialsProvider = (url, usernameFromUrl, types) =>
                    new UsernamePasswordCredentials()
                    {
                        Username = "npangunion@gmail.com",
                        Password = "" 
                    };

                repo.Network.Push(remote, @"refs/heads/master", options);
            }
        }

        static string CreateChangeFile()
        {
            var filename = $"file_{DateTime.Now.ToString("yyyy_MMdd_HHmm_ss")}.txt";
            var fs = System.IO.File.Create($"Pad\\{filename}");
            fs.Close();

            return filename;
        }
    }
}
