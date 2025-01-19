using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace AiTrainer.Web.Api.SignalR.Extensions;

internal static class HubExtensions
{
    public static T? GetMetadata<T>(this HubInvocationContext invocationContext) where T : Attribute
     => invocationContext.HubMethod.GetCustomAttribute<T>() ?? invocationContext.Hub.GetType().GetCustomAttribute<T>();
    
}