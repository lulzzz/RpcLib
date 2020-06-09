﻿using DemoShared.Model;
using RpcLib;
using System.Threading.Tasks;

namespace DemoShared.Rpc {

    /// <summary>
    /// Demo interface for a <see cref="IRpcServer"/>.
    /// This interface defines all methods which can be called
    /// on the server side from RPC calls by the client.
    /// </summary>
    public interface IDemoRpcServer : IRpcServer {

        /// <summary>
        /// Says "hello" to the given name on the console of the server.
        /// </summary>
        Task SayHello(Greeting greeting);

        /// <summary>
        /// Modifies the given data on the server and returns it.
        /// Strings are suffixed by "-ServerWasHere", numbers are multipied by 2.
        /// </summary>
        Task<SampleData> ProcessData(SampleData baseData);

    }

}
