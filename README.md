# SpiderX

This is a simple web-crawler development framework based on .Net Core.
>本项目是一个简单的基于 .Net Core 的爬虫开发框架。

## Develop Environment |开发环境

- Target Framework |运行框架: [.Net Core 2.2+](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)

- Recommended IDE |推荐IDE: [Visual Studio 2017 15.9+](https://visualstudio.microsoft.com/zh-hans/downloads/)

- Recommended Language Version |推荐语言版本: [C# 7.0+](https://docs.microsoft.com/zh-cn/dotnet/csharp/whats-new/)

- Recommended ORM |推荐ORM： [EntityFramework Core 2.2+](https://docs.microsoft.com/zh-cn/ef/index#pivot=efcore)

## Structure |项目结构

- Launcher |启动模块

  - SpiderX.Launcher  
    An entry which includes user settings and business references.  
    The user settings are in [appsettings.json](https://github.com/LeaFrock/SpiderX/blob/master/SpiderX.Launcher/AppSettings/appsettings.json) and quite easy to realize. Check the setting file(s) before running your own case.  
    While using Visual Studio in development, you can also run your case(s) by setting init-params under the 'Debug' window instead of editing the 'CaseSettings' in *appsettings.json*. The init-params read like below:

    - One Case: `MyCaseBll`
    - Multi Cases: `MyFirstCaseBll;MySecondCaseBll CaseParam1 CaseParam2;MyThirdCaseBll CaseParam3` *//Cases are divided by ';'.*
    - Skip Case(s): `-MyFirstCaseBll;/MySecondCaseBll;MyThirdCaseBll` *//Cases whose name starts with '-' or '/' will be skipped.*

    >包含所有用户设置和业务模块引用的入口。  
    >用户设置在[appsettings.json](https://github.com/LeaFrock/SpiderX/blob/master/SpiderX.Launcher/AppSettings/appsettings.json)里，各项设置非常易懂。在运行你自己的业务前检查下配置文件。  
    >当使用VS开发时，你还可以在“调试”窗口下使用启动参数启动你的业务案例，而不需要编辑设置文件里的“CaseSettings”。启动参数格式如下：
    >>- 单个案例: `MyCaseBll`
    >>- 多个案例: `MyFirstCaseBll;MySecondCaseBll CaseParam1 CaseParam2;MyThirdCaseBll CaseParam3` *//案例由英文分号隔断.*
    >>- 跳过案例: `-MyFirstCaseBll;/MySecondCaseBll;MyThirdCaseBll` *//以'-'或'/'开头的案例会被忽略.*

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
    A special 'SpiderX.Business' in fact which is responsible for proxy-fetching.
    >其实是一个特殊的“SpiderX.Business”，负责获取代理。

  - SpiderX.Proxy  
    A wrapper of WebProxy including other codes related to Proxy.
    >基于WebProxy的封装，并且包含其他跟Proxy相关的代码。

- Database |数据模块

  - SpiderX.DataClient  
    Includes codes related to DB like SqlServer & MySQL, and based on EF Core currently.
    >包含跟数据库相关的代码，目前基于EF Core.

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

## Contribution |贡献

Issues and pull requests are welcomed if you have any questions!
>如果您有任何疑问，欢迎提交Issue和PR！
