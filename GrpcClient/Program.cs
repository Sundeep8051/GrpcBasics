using Basics;
using Grpc.Core;
using Grpc.Net.Client;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("gRPC client");

using var channel = GrpcChannel.ForAddress("https://localhost:7268");

var rpcClient = new RpcServiceTypes.RpcServiceTypesClient(channel);

// UseUnary(rpcClient);

//await UseClientStreaming(rpcClient);

//await UseServerStreaming(rpcClient);

await UseBidirectionalStreaming(rpcClient);

Console.ReadKey();

#region call to service methods

static void UseUnary(RpcServiceTypes.RpcServiceTypesClient client){
    //Unary type
    var result = client.Unary(new Request{ Content = "Hello World!"});
    Console.WriteLine(result.Message);
}

static async Task UseClientStreaming(RpcServiceTypes.RpcServiceTypesClient client){
    //Client Streaming type
    var message = "This message is from the grpc client as a client streaming type";
    using var clientCall = client.ClientStreaming();

    foreach(var c in message){
        await clientCall.RequestStream.WriteAsync(new Request{ Content = c.ToString()});
    }

    await clientCall.RequestStream.CompleteAsync();

    var response = await clientCall;
    Console.WriteLine(response.Message);
}

static async Task UseServerStreaming(RpcServiceTypes.RpcServiceTypesClient client){
    //Server streaming type
    var message = "This message is from the the grpc server as a server streaming type";

    using var serverCall = client.ServerStreaming(new Request { Content = message});

    await foreach(var response in serverCall.ResponseStream.ReadAllAsync()){
        await Task.Delay(500);
        Console.WriteLine(response.Message);
    }
}

static async Task UseBidirectionalStreaming(RpcServiceTypes.RpcServiceTypesClient client){
    //Bidirectional streaming type
    var message = "This message is from the the grpc server as a bidirectional streaming type";   

    using var call = client.BiDirectionalStreaming();

    foreach(var c in message){
        await Task.Delay(100);
        await call.RequestStream.WriteAsync(new Request{Content = c.ToString()});
    }
    await call.RequestStream.CompleteAsync();

    await foreach(var response in call.ResponseStream.ReadAllAsync()){
        await Task.Delay(500);
        Console.WriteLine(response.Message);
    }
}
    

#endregion