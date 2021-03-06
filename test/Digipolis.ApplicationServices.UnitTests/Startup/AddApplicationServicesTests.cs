﻿using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Digipolis.ApplicationServices.UnitTests.Startup
{
    public class AddApplicationServicesTests
    {
        [Fact]
        void OptionsAreRegisteredAsSingleton()
        {
            var services = new ServiceCollection();
            services.AddApplicationServices(options => options.ApplicationId = "123456789");

            var registrations = services.Where(sd => sd.ServiceType == typeof(IConfigureOptions<ApplicationServicesOptions>))
                                        .ToArray();

            Assert.Equal(1, registrations.Count());
            Assert.Equal(ServiceLifetime.Singleton, registrations[0].Lifetime);
        }

        [Fact]
        void ApplicationContextIsRegisteredAsSingletonInstance()
        {
            var services = new ServiceCollection();
            services.AddApplicationServices(options => options.ApplicationId = "123456789");

            var registrations = services.Where(sd => sd.ServiceType == typeof(IApplicationContext) &&
                                               sd.ImplementationInstance?.GetType() == typeof(ApplicationContext))
                                        .ToArray();

            Assert.Equal(1, registrations.Count());
            Assert.Equal(ServiceLifetime.Singleton, registrations[0].Lifetime);
        }

        [Fact]
        void ApplicationIdNullRaisesArgumentNullException()
        {
            var services = new ServiceCollection();
            var ex = Assert.Throws<ArgumentNullException>(() => services.AddApplicationServices(options => options.ApplicationId = null));
            Assert.Equal("ApplicationId", ex.ParamName);
        }

        [Fact]
        void ApplicationIdEmptyRaisesArgumentException()
        {
            var services = new ServiceCollection();
            var ex = Assert.Throws<ArgumentException>(() => services.AddApplicationServices(options => options.ApplicationId = ""));
            Assert.Equal("ApplicationId", ex.ParamName);
        }

        [Fact]
        void ApplicationIdWhiteSpaceRaisesArgumentException()
        {
            var services = new ServiceCollection();
            var ex = Assert.Throws<ArgumentException>(() => services.AddApplicationServices(options => options.ApplicationId = "   "));
            Assert.Equal("ApplicationId", ex.ParamName);
        }

        [Fact]
        void ApplicationIdAndNameAreSetInApplicationContext()
        {
            var services = new ServiceCollection();
            services.AddApplicationServices(options => {
                options.ApplicationId = "123456";
                options.ApplicationName = "MyApplication";
            });

            var appContext = services.BuildServiceProvider().GetRequiredService<IApplicationContext>();

            Assert.Equal("123456", appContext.ApplicationId);
            Assert.Equal("MyApplication", appContext.ApplicationName);
        }

        [Fact]
        void ApplicationNameNullTakesProjectName()
        {
            var services = new ServiceCollection();
            services.AddApplicationServices(options => {
                options.ApplicationId = "123";
                options.ApplicationName = null;
            });


            var appContext = services.BuildServiceProvider().GetRequiredService<IApplicationContext>();

            Assert.Equal(PlatformServices.Default.Application.ApplicationName, appContext.ApplicationName);
        }

        [Fact]
        void ApplicationNameEmptyTakesProjectName()
        {
            var services = new ServiceCollection();
            services.AddApplicationServices(options => {
                options.ApplicationId = "123";
                options.ApplicationName = "";
            });

            var appContext = services.BuildServiceProvider().GetRequiredService<IApplicationContext>();

            Assert.Equal(PlatformServices.Default.Application.ApplicationName, appContext.ApplicationName);
        }

        [Fact]
        void ApplicationNameWhiteSpaceTakesProjectName()
        {
            var services = new ServiceCollection();
            services.AddApplicationServices(options => {
                options.ApplicationId = "123";
                options.ApplicationName = "  ";
            });

            var appContext = services.BuildServiceProvider().GetRequiredService<IApplicationContext>();

            Assert.Equal(PlatformServices.Default.Application.ApplicationName, appContext.ApplicationName);
        }
    }
}
