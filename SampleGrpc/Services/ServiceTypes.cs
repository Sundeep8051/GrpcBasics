using Basics;
using Grpc.Core;

namespace SampleGrpc.Services
{
    public class GrpcServiceTypes(ILogger<GrpcServiceTypes> logger) : RpcServiceTypes.RpcServiceTypesBase
    {
        public override async Task<Response> Unary(Request request, ServerCallContext context)
        {
            var res = new Response(){
                Message = $"{request.Content } received from GRPC server"
            };

            logger.LogInformation(res.ToString());

            return await Task.FromResult(res);
        }

        public override async Task<Response> ClientStreaming(IAsyncStreamReader<Request> requestStream, ServerCallContext context)
        {
            var response = new Response();
            
            while(await requestStream.MoveNext()){
                var reqPayload = requestStream.Current;
                response.Message += reqPayload.Content.ToString() + ", ";
            };

            return response;
        }

        public override async Task ServerStreaming(Request request, IServerStreamWriter<Response> responseStream, ServerCallContext context)
        {
            foreach(var c in request.Content){
                var response = new Response(){
                    Message = $"The current word is {c}"
                };
                await responseStream.WriteAsync(response);       
            }
        }

        public override async Task BiDirectionalStreaming(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context)
        {
            var response = new Response();

            while (await requestStream.MoveNext())
            {
                response.Message = $"The current word is {requestStream.Current}";
                await responseStream.WriteAsync(response);
            }
        }
    }
}