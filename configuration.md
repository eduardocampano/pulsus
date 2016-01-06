There are two ways to configure Pulsus, using a XML configuration file or using code.

! Using Pulsus.config configuration file

Placing a Pulsus.config file in your application root directory right next to your app.config or web.config file will do the trick.

{code:xml}
<pulsus logKey="ecommerce" includeHttpContext="true">
</pulsus>
{code:xml}

! Using code

{code:c#}
LogManager.Configuration.LogKey = "ecommerce";
LogManager.Configuration.IncludeHttpContext = true;
{code:c#}

! Configuration settings and default values

!! Enabled
This is used to quickly enable or disable logging. The default value is true.

!! LogKey
This is used to indicate the application that is generating the event. The default value will be auto-generated based on the applications' assembly name or the site name for web applications.

!! Tags
Allows to define default tags for all the events. The default value is null.

!! IncludeHttpContext
If set to true, Pulsus will try to add the HttpContext information to the event when it's created. The default value is false.

If you don't want to include HttpContext information on all the events, it can be manually added to the event using the event builder, like this:
{code:c#}
LogManager.EventFactory.Create()
                                     .Level(LoggingEventLevel.Error) 
                                     .Text("The provided information is not valid")
                                     .AddHttpContext()
                                     .Push();
{code:c#}

!! IncludeStackTrace
If set to true, the Stack Trace information will be added to the event. The default value is false.

!! IgnoreNotFound
If set to true, the ErrorLoggingModule will ignore 404 HTTP exceptions. This is an easy way to ignore HTTP exceptions associated with not found pages. ASP.NET (and MVC) will trow an exception when a page or matching route is not found. If you want to ignore these exceptions for all targets  

!! Async
If set to true all targets will be wrapped with an AsyncWrapperTarget. This is a shortcut to defining the AsyncWrapperTarget in the XML configuration. It can be used at the target element level to specify the AsyncWrapperTarget must only be enabled for a particular target. The default value is false.

!! ThrowExceptions
If set to true, exceptions generated from Pulsus code (or targets) will be rethrown after catched. This is useful for debugging and some application scenarios. The default value is false.

!! Debug
If set to true Pulsus will write debugging information to a local log file. This is useful to find out configuration errors. The default value is false.

!! DebugFile
Allows you to specify the file path (o relative path) to the pulsus debug file. The default value for a web application is ~/App_Data/pulsus_log.txt and pulsus_log.txt in the application domain directory for the rest. 

!! DebugVerbose
If set to true, detailed information will be written to the pulsus debug file. The default value is false.

! Filters and Ignores
A particular target can define filters and ignore conditions. This feature allows to define "rules" a particular event needs to match in order to be pushed to the target. 

* MinLevel
* MaxLevel
* LogKeyContains
* LogKeyStartsWith
* TextContains
* TextStartsWith
* TagsContains
* MinValue
* MaxValue

The example above shows a configuration sample with an EmailTarget defining both level filters and an ignore. In this case the event will be pushed to the email target only if it has a Warning or above level but will be ignored if it's tagged with the "notemail" tag.

{code:xml}
<pulsus>
    <targets>
        <target name="email" type="EmailTarget" minLevel="Warning">
            <ignores>
                <ignore tagsContains="notemail" />
            </ignores>
        </target>
    </targets>
</pulsus>
{code:xml} 