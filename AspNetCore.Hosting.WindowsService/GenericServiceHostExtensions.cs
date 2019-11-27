using System.ServiceProcess;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.Hosting.WindowsService
{
    public static class GenericServiceHostExtensions
    {
        public static void RunAsWinService(this IHost host)
        {
            var hostService = new GenericServiceHost(host);
            ServiceBase.Run(hostService);
        }
    }
}