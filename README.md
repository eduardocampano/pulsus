Pulsus
======

A simple .NET logging library for modern applications

Some sample action!
-------------------

Using the library to push a trace event...
```csharp
LogManager.EventFactory.Create()
						.Level(LoggingEventLevel.Trace)
						.Text("Sample trace event")
						.AddStrackTrace()
						.Push();
```	
	