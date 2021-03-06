using System.Threading.Tasks;
using System;
using DemoShared.Model;
using DemoShared.Rpc;
using RpcLib.Model;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using DemoServer.Services;
using RpcLib;
using RpcLib.Utils;

/// <summary>
/// Implementation of the demo RPC server functions.
/// </summary>
public class DemoServerRpc : RpcFunctions, IDemoServerRpc {

    public async Task SayHelloToServer(Greeting greeting) {
        Console.WriteLine(Context.ClientID + " says: Hello " + greeting.Name + "!");
        if (greeting.MoreData is SampleData moreData && moreData.Text.Length > 0)
            Console.WriteLine("More information for you: " + moreData.Text);
        // Test service injection
        using (var services = Context.ServiceScopeFactory!.CreateScope()) {
            var demoService = services.ServiceProvider.GetService<DemoService>();
            Console.WriteLine("And the demo ASP.NET Core service says: " +
                demoService.CallService("How are you"));
        }
    }

    public async Task<SampleData> ProcessDataOnServer(SampleData baseData) {
        return new SampleData {
            Text = baseData.Text + "-ServerWasHere",
            Number = baseData.Number * 2,
            List = baseData.List.Select(it => it + "-ServerWasHere").ToList()
        };
    }

    // Mapping of RpcCommand to real method calls (boilerplate code; we could auto-generate this method later)
    public override Task<string?>? Execute(RpcCommand command) => command.MethodName switch {
        "SayHelloToServer" => SayHelloToServer(command.GetParam<Greeting>(0)).ToJson(),
        "ProcessDataOnServer" => ProcessDataOnServer(command.GetParam<SampleData>(0)).ToJson(),
        _ => null
    };
    
}