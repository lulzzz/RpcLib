using BankClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RpcLib.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RpcLibTest {

    /// <summary>
    /// Test of the different retry strategies, see <see cref="RpcRetryStrategy"/>.
    /// Here, in "RetryOnClientTest", the bank is the server and the customer is the client behind the firewall.
    /// To test the other way, see the "RetryOnServerTest" demo.
    /// 
    /// See the comments within this test what we do and what we expect.
    /// </summary>
    [TestClass]
    public class RetryOnClientTest {

        [TestInitialize]
        public void Cleanup() {
            // Remove all *.banklog files
            foreach (var file in thisDir.GetFiles("*.banklog"))
                file.Delete();
        }

        [TestMethod]
        public void TestRetry() {

            // Start client. It uses a default timeout of 1 second.
            // Each second, it sends an increasing amount (1 ct, 2ct, 3ct, ...)
            // to the bank, which is still offline. This is done for 10 seconds.
            client = LaunchClient(number: 0);

            // After about 25 seconds, the server starts. It uses a default timeout of 1 second.
            _ = Task.Run(async () => {
                await Task.Delay(25000);
                server = LaunchServer();
            });

            // After about 45 seconds, the client should have been closed and we close the server, too.
            Thread.Sleep(45000);
            client.Kill(); // Should be closed already
            server.Kill();

            // Check the log files
            // TODO: evaluate
        }

        private Process LaunchServer() =>
            Launch(Path.Combine(baseDir.FullName, "BankServer/bin/Debug/netcoreapp3.1/BankServer.exe"));

        private Process LaunchClient(int number) =>
            Launch(Path.Combine(baseDir.FullName, "BankClient/bin/Debug/netcoreapp3.1/BankClient.exe"), $"{number}");

        private Process Launch(string path, string arguments = "") {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = path;
            psi.Arguments = arguments;
            psi.UseShellExecute = true;
            psi.CreateNoWindow = false;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            return Process.Start(psi);
        }

        private Process server = null;
        private Process client = null;

        private DirectoryInfo thisDir = new DirectoryInfo(".");
        private DirectoryInfo baseDir = new DirectoryInfo(".").Parent.Parent.Parent.Parent;

    }
}
