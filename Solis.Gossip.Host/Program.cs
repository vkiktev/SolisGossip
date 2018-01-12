using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Solis.Gossip.Service;
using System;
using System.IO;

namespace Solis.Gossip.Host
{
    class Program
    {
        private static IConfiguration _configuration;
        private static IServiceProvider _serviceProvider;

        public static void Main(string[] args)
        {
            try
            {
                // Create service collection
                ServiceCollection serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);

                // Create service provider
                _serviceProvider = serviceCollection.BuildServiceProvider();

                GossipSettings settings = new GossipSettings();
                ConfigurationBinder.Bind(_configuration.GetSection("gossip"), settings);

                GossipNode gossipNode = new GossipNode(settings);
                gossipNode.NewPeerFound += GossipNode_NewPeerFound;
                gossipNode.PeerDown += GossipNode_PeerDown;
                gossipNode.PeerWakeUp += GossipNode_PeerWakeUp;  
                    
                gossipNode.Init();

                Console.ReadKey();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("The given file is not found!");
            }
            catch (FormatException)
            {
                Console.WriteLine("The given file is not in the correct JSON format!");
            }
            catch (IOException e)
            {
                Console.WriteLine("Could not read the configuration file: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while starting the gossip service: " + e.Message);
            }
        }

        private static void GossipNode_PeerWakeUp(object sender, GossipPeerEventArgs e)
        {
            Console.WriteLine($"Peer {e.Peer} woke-up!");
        }

        private static void GossipNode_PeerDown(object sender, GossipPeerEventArgs e)
        {
            Console.WriteLine($"Peer {e.Peer} down!");
        }

        private static void GossipNode_NewPeerFound(object sender, GossipPeerEventArgs e)
        {
            Console.WriteLine($"New peer {e.Peer} found!");
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add logging
            serviceCollection.AddSingleton(new LoggerFactory().AddNLog());
            serviceCollection.AddLogging();

            // Build configuration

           _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();
            
            // Add access to generic IConfiguration
            serviceCollection.AddSingleton(_configuration);
        }
    }
}
