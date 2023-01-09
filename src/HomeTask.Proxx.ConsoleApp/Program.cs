using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HomeTask.Proxx.ConsoleApp;
using HomeTask.Proxx.Domain.Interfaces;
using HomeTask.Proxx.Domain.Services;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices(services => services
        .AddSingleton<IProxxGameFactory, ProxxGameFactory>()
        .AddSingleton<PlayGameInConsoleService>())
    .Build();

PlayGameInConsoleService playGameInConsoleService = host.Services.GetRequiredService<PlayGameInConsoleService>();
playGameInConsoleService.PlayGame();
