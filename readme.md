A simple .NET logging, parsing, reporting library for modern applications, designed from the ground up for scalability, high availability and flexibility.

!! Why another logging library?
We reviewed and used other logging libraries in the past like log4net, NLog, ELMAH. All of them are just great but we were looking for something to cover some specific needs to simplify our development process and make sure our products and solutions meet defined quality standards. Pulsus is mostly based on NLog which is an awesome library and we tried to maintain some of its concepts so this is not a library you need to learn a lot to use it. 

!! Ok, show me the code
Using the library to push a trace event...

{code:c#}
LogManager.EventFactory.Create()
                        .Level(LoggingEventLevel.Trace)
                        .Text("User accessed private zone")
                        .AddTags("security")
                        .Push();
{code:c#}

Adding custom data to the event

{code:c#}
// creating an anonymous shopping cart object  
var shoppingCart = new {
                        CustomerId = "1234",
                        ItemCount = 1,
                        Total = 105.99
                   };

LogManager.EventFactory.Create()
                        .Level(LoggingEventLevel.Trace)
                        .Text("Shopping Cart Update")
                        .AddData("ShopingCart", shoppingCart)
                        .Push();
{code:c#}

!! Installing using Nuget
If you're using classic ASP.NET, WinForms, WPF, console apps:
{{
PM> Install-Package Pulsus
}}

If you're using ASP.NET MVC:
{{
PM> Install-Package Pulsus.Mvc
}}

!! Configuration
Please check the [Configuration] page.

!! More coming soon
* Javascript client
* Events viewer
* Error handling features for the ASP.NET MVC package

Note: This library was originally hosted at [url:GitHub|https://github.com/eduardocampano/Pulsus] but is now officially an [url:DataArt|http://www.dataart.com/] supported project.

