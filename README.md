Pulsus
======

A simple .NET logging library for modern applications

Ok, but some code first
-----------------------

Using the library to push a trace event...
```csharp
LogManager.EventFactory.Create()
						.Level(LoggingEventLevel.Trace)
						.Text("Sample trace event")
						.AddStrackTrace()
						.Push();
```	

Adding custom data to the event
```csharp
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
```	


Installing via Nuget
--------------------

If you're using classic ASP.NET, WinForms, WPF, console apps:
```
PM> Install-Package Pulsus
```	

If you're using ASP.NET MVC:
```
PM> Install-Package Pulsus.Mvc
```	

Configuration
-------------

You can change the Pulsus settings using your application's config file or by code.

Using your app.config or your web.config file:
```xml
<configuration>
  <configSections>
    <section name="pulsus" type="Pulsus.Configuration.PulsusSection, Pulsus" />
  </configSections>

  <pulsus async="false" includeHttpContext="true" includeStackTrace="true">
  </pulsus>
</configuration>
```	

Using code:
```csharp
// setting the event dispatcher to push the events synchronously (async is by default)
LogManager.Settings.Async = false;
// include the HttpContext information with the event
LogManager.Settings.IncludeHttpContext = true;
// include the StackTrace information with the event
LogManager.Settings.IncludeStackTrace = true;
```	

More comming soon
-----------------

- Error handling for the ASP.NET MVC integration package
- Pulsus Server for viewing events and stats
- More targets...