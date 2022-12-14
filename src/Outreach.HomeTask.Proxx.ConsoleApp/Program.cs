using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Outreach.HomeTask.Proxx.ConsoleApp;
using Outreach.HomeTask.Proxx.Domain.Interfaces;
using Outreach.HomeTask.Proxx.Domain.Services;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices(services => services
        .AddSingleton<IProxxGameFactory, ProxxGameFactory>()
        .AddSingleton<PlayGameInConsoleService>())
    .Build();

PlayGameInConsoleService playGameInConsoleService = host.Services.GetRequiredService<PlayGameInConsoleService>();
playGameInConsoleService.PlayGame();
