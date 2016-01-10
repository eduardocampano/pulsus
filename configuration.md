#Pulsus Configuration

There are two ways to configure Pulsus, using a XML configuration file or using code.

##Using Pulsus.config configuration file

Placing a Pulsus.config file in your application root directory right next to your app.config or web.config file will do the trick.

```xml
<pulsus logKey="ecommerce" includeHttpContext="true">
</pulsus>
```

##Using code

```csharp
LogManager.Configuration.LogKey = "ecommerce";
LogManager.Configuration.IncludeHttpContext = true;
```

#Configuration settings and default values

| Setting  | Description      |  Default Value |
|----------|---------------|-------|
| Enabled  | Boolean used to quickly enable or disable logging | true |
| LogKey | A string indicates the application generating the event. The recommendation is to use a dash separated set of values to differentiate the application and environment, i.e. `ecommerce-staging`, `ecommerce-production`  | The default value will be auto-generated based on the applications' assembly name or the site name for web applications. |
| Tags | A string indicating space separated set of tags which will be added to all events | null |
| IncludeHttpContext | When set to true Pulsus will try to include HttpContext information to all events if available | false |
| IncludeStackTrace | When set to true, the Stack Trace information will be added to all the events | false |
| IgnoreNotFound | When set to true, the ErrorLoggingModule will ignore 404 HTTP exceptions. This is an easy way to ignore HTTP exceptions associated with not found pages. ASP.NET MVC will trow an exception when a page or matching route is not found. If you want to ignore these exceptions for all targets | false |
| Async | When set to true all targets will be wrapped with an AsyncWrapperTarget. This is a shortcut to defining the AsyncWrapperTarget in the XML configuration. It can be used at the target element level to specify the AsyncWrapperTarget must only be enabled for a particular target. | false |
| ThrowExceptions | When set to true, exceptions generated from Pulsus code (or custom targets) will be rethrown after catched. This is useful for debugging Pulsus configuration and troubleshooting target issues. | false |
| Debug | When set to true Pulsus will write debugging information to a local log file. This is useful to find out configuration errors. | false |
| DebugFile | Indicates the file path (o relative path) to the pulsus debug file. | For a web application is `~/App_Data/pulsus_log.txt` and `pulsus_log.txt` in the application domain directory for the rest. |
| DebugVerbose | When set to true, detailed information will be written to the pulsus debug file. | false |


# Filters and Ignores
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

```xml
<pulsus>
    <targets>
        <target name="email" type="EmailTarget" minLevel="Warning">
            <ignores>
                <ignore tagsContains="notemail" />
            </ignores>
        </target>
    </targets>
</pulsus>
```
