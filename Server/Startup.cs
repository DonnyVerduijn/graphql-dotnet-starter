using GraphQL.Server;
using GraphQL.Server.Ui.GraphiQL;
using GraphQL.Server.Ui.Playground;
using GraphQL.Server.Ui.Voyager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.Orders.Schema;
using WebApi.Orders.Services;
using WebApi.Customers.Schema;
using WebApi.Customers.Services;
using System;
using WebApi;

namespace Server
{
    public class Startup
    {

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<CustomerCreateInputType>();
            services.AddSingleton<CustomerEventType>();
            services.AddSingleton<CustomerMutations>();
            services.AddSingleton<CustomerQueries>();
            services.AddSingleton<CustomerSubscriptions>();
            services.AddSingleton<CustomerType>();
      
            services.AddSingleton<OrderCreateInputType>();
            services.AddSingleton<OrderEventType>();
            services.AddSingleton<OrderMutations>();
            services.AddSingleton<OrderQueries>();
            services.AddSingleton<OrderStatusesEnum>();
            services.AddSingleton<OrderSubscriptions>();
            services.AddSingleton<OrderType>();
            
            services.AddSingleton<ICustomerEventService, CustomerEventService>();
            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton<IOrderEventService, OrderEventService>();
            services.AddSingleton<IOrderService, OrderService>();

            services.AddSingleton<Schema>();

            services.AddGraphQL(options =>
            {   
                options.EnableMetrics = Environment.IsDevelopment();
                options.ExposeExceptions = Environment.IsDevelopment();
                options.UnhandledExceptionDelegate = ctx => { Console.WriteLine(ctx.OriginalException); };
            })
            .AddSystemTextJson(deserializerSettings => { }, serializerSettings => { }) // For .NET Core 3+
            .AddWebSockets()
            .AddDataLoader()
            .AddGraphTypes(typeof(Schema));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
                app
                .UseDeveloperExceptionPage()
                .UseGraphQLPlayground(new GraphQLPlaygroundOptions()
                {
                    Path = "/ui/playground",
                    GraphQLEndPoint = "/graphql"
                })
                .UseGraphiQLServer(new GraphiQLOptions
                {
                    Path = "/ui/graphiql",
                    GraphQLEndPoint = "/graphql"
                })
                .UseGraphQLVoyager(new GraphQLVoyagerOptions()
                {
                    Path = "/ui/voyager",
                    GraphQLEndPoint = "/graphql"
                });

            app
            .UseWebSockets()
            .UseGraphQLWebSockets<Schema>("/graphql")
            .UseGraphQL<Schema>("/graphql");
        }
    }
}
