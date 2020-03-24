# SpiderX

[DotNetCoreUrl]: https://dotnet.microsoft.com/download
[SpiderXTemplateRepoUrl]: https://github.com/LeaFrock/SpiderX.Template

[![.NET Core](https://img.shields.io/badge/.NET%20Core-%203.1-brightgreen)][DotNetCoreUrl]
![License](https://img.shields.io/badge/License-GPL--3.0-blue)

This is a simple web-crawler development framework based on .Net Core.  
>本项目是一个简单的基于 .Net Core 的爬虫开发框架。

## Disclaimer |免责声明

For everyone, this framework must be used legally under local law. Users have total responsibility for ensuring their own compliance with legal requirements, while authors and contributors won't be held responsible for actions of illegal users.
>对所有人来说，该框架必须在当地法律允许的范围内使用。使用者对确保自己遵守法律要求负有完全责任，而作者和贡献者对违法使用者的行为概不负责。

All of the samples in this project are just for instructions, learning and communication. If you change the source codes, please obey the open-source license.
>本项目中的所有样例仅用于说明、学习和交流。如果你修改了源代码，请遵守开源协议。

## Develop Environment |开发环境

- Target Framework |运行框架: [.Net Core 3.1+](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)

- Recommended IDE |推荐IDE: [Visual Studio 2019 16.4+](https://visualstudio.microsoft.com/zh-hans/downloads/)

- Recommended Language Version |推荐语言版本: [C# 8.0+](https://docs.microsoft.com/zh-cn/dotnet/csharp/whats-new/)

- Recommended ORM |推荐ORM： [EntityFramework Core 3.1+](https://docs.microsoft.com/zh-cn/ef/index#pivot=efcore)

## Structure |项目结构

- Launcher |启动模块

  - SpiderX.Launcher  
    An entry which includes user settings and business references.  
    The user settings are in [appsettings.json](https://github.com/LeaFrock/SpiderX/blob/master/SpiderX.Launcher/AppSettings/appsettings.json) and quite easy to realize. Check the setting file(s) before running your own case.  
    While using Visual Studio in development, you can also run your case(s) by setting init-params under the 'Debug' window instead of editing the 'CaseSettings' in *appsettings.json*. The init-params read like below:

    - One Case: `SpiderX.Bussiness *-MyCaseBll` *//The format is, '[DefaultNameSpace] [NameSpace1]-[CaseName1]-[Param1],[Param2]-[DbConfigName]-[CaseVersion]'. The params and version are optional. If the [NameSpace1] is the same as [DefaultNameSpace], you can use '`*`' instead.*
    - One Case 2: `MyAbbrOfDefaultNameSpace *-MyCaseBll-_-MySqlTest` *//You can use '_' to ignore unnecessary params. In addition, in **hostsettings.json**, you can check the 'BllNameSpaces' element, set the 'Abbr' values ignoring cases and then use the matched abbr for [DefaultNameSpace]. Also you can check the 'DbConfigs' element to select the 'DbConfigName'.*
    - Multi Cases: `SpiderX.Bussiness *-MyFirstCaseBll *-MySecondCaseBll-CaseParam1,CaseParam2-2 SpiderX.ProxyFetcher-MyThirdCaseBll-CaseParam3` *//Cases are divided by 'WHITESPACE'.*
    - Skip Case(s): `SpiderX.Bussiness /*-MyFirstCaseBll *-MySecondCaseBll-CaseParam1-3` *//Cases whose name starts with '/' will be skipped.*

    >包含所有用户设置和业务模块引用的入口。  
    >用户设置在[appsettings.json](https://github.com/LeaFrock/SpiderX/blob/master/SpiderX.Launcher/AppSettings/appsettings.json)里，各项设置非常易懂。在运行你自己的业务前检查下配置文件。  
    >当使用VS开发时，你还可以在“调试”窗口下使用启动参数启动你的业务案例，而不需要编辑设置文件里的“CaseSettings”。启动参数格式如下：
    >>- 单个案例: `SpiderX.Bussiness *-MyCaseBll` *//格式为：‘[默认命名空间] [案例所在命名空间]-[案例名]-[执行参数1],[执行参数2]-[数据源配置名称]-[案例版本]’。执行参数和案例版本如果不需要可以不指定。如果案例的命名空间就是默认命名空间，可以用‘`*`’号代替。*
    >>- 单个案例2: `MyAbbrOfDefaultNameSpace *-MyCaseBll-_-MySqlTest` *可以使用'_'忽略不需要的参数。另外，在**hostsettings.json**文件中，可以检查“BllNameSpaces”元素，设定命名空间的缩写（无视大小写）然后使用相对应的缩写来表示[默认命名空间]。同样地，在“DbConfigs”元素中选择“DbConfigName”。*
    >>- 多个案例: `SpiderX.Bussiness *-MyFirstCaseBll *-MySecondCaseBll-CaseParam1,CaseParam2-2 SpiderX.ProxyFetcher-MyThirdCaseBll-CaseParam3` *//案例由空格隔断。*
    >>- 跳过案例: `SpiderX.Bussiness /*-MyFirstCaseBll *-MySecondCaseBll-CaseParam1-3` *//以'/'开头的案例会被忽略。*

- Business |业务模块

  - SpiderX.BusinessBase  
    Includes a simple BaseType of Business.
    >包含所有业务的基类。

  - SpiderX.Business  
    Includes kinds of business-classes.  
    As a developer, you can also write your own project which includes the reference of 'SpiderX.BusinessBase' and just make your business-classes inherit the 'SpiderX.BusinessBase.BllBase'. Then put your project reference into ‘SpiderX.Launcher’.  
    To keep the code style, the name of a bussiness-class ending with 'Bll' is recommended.
    >包含业务类的集合。  
    >作为开发者，你也可以写自己的项目，项目中只需要引入“SpiderX.BusinessBase”，并且使你的业务类继承“BllBase”类。然后将你的项目引用加入到“SpiderX.Launcher”。  
    >为了保证代码风格一致，业务类命名建议统一以“Bll”结尾。

- Network |通信模块

  - SpiderX.Http  
    A wrapper of HttpClient including other codes related to Http(s).
    >基于HttpClient的封装，并且包含其他跟Http(s)相关的代码。

- Proxy |代理模块

  - SpiderX.ProxyFetcher  
    A special 'SpiderX.Business' in fact which is responsible for free-proxy-fetching.
    >其实是一个特殊的“SpiderX.Business”，负责获取免费代理。

  - SpiderX.Proxy  
    A wrapper of WebProxy including other codes related to Proxy.
    >基于WebProxy的封装，并且包含其他跟Proxy相关的代码。

- Database |数据模块

  - SpiderX.DataClient  
    Includes codes related to DB like SqlServer & MySQL, and based on EF Core currently.
    >包含跟数据库相关的代码，目前基于EF Core.

  - SpiderX.Redis  
    Includes codes related to Redis, and based on StackExchange.Redis currently.
    >包含跟Redis相关的代码，目前基于StackExchange.Redis.

- Common-Use |通用模块

  - SpiderX.Extensions  
    Includes all function extensions.
    >包含所有的扩展方法。

  - SpiderX.Tools  
    Includes kinds of static fields, properties and functions for common usage.
    >包含各种常用的静态字段、属性和方法。

  - SpiderX.Security  
    Includes kinds of functions related to cryptography.
    >包含各种加密解密相关的方法。

  - SpiderX.Puppeteer  
    Includes kinds of functions related to Puppeteer operations, based on PuppeteerSharp.
    >基于PuppeteerSharp封装各种与Puppeteer操作相关的方法。

## Assistant |辅助工具

- CLI & Templates : [SpiderX.Template][SpiderXTemplateRepoUrl]

## Contribution |贡献

Issues and pull requests are welcomed if you have any questions!
>如果您有任何疑问，欢迎提交Issue和PR！
