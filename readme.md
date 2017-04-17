# TravelIC
[![Build Status](https://travis-ci.org/Anduin2017/TravelIC.svg?branch=master)](https://travis-ci.org/Anduin2017/TravelIC)

TravelIC is a simple WeChat application which allows your WeChat followers sell and buy their own things.

TravelIC is based on dot net core.

## Requirements

Requirements about how to run
* [Windows Server](http://www.microsoft.com/en-us/cloud-platform/windows-server) or [Ubuntu Server](https://www.ubuntu.com/server)
* [dot net Core 1.1.0 or later](https://github.com/dotnet/core/tree/master/release-notes)
* [SQL Server or SQL Server LocalDB](http://www.microsoft.com/en-us/sql-server/sql-server-2016)
* [bower](https://bower.io)
* [nodejs and npm](https://nodejs.org)
* [git](https://git-scm.com)

**bower depends on nodejs, npm and git!**

Requirements about how to develope
* [Windows 10](http://www.microsoft.com/en-US/windows/) or [Ubuntu desktop](https://www.ubuntu.com/desktop)
* [dot net Core SDK 1.0.3 or later](https://github.com/dotnet/core/tree/master/release-notes)
* [SQL Server or SQL Server LocalDB](http://www.microsoft.com/en-us/sql-server/sql-server-2016)
* [bower](https://bower.io)
* [nodejs and npm](https://nodejs.org)
* [git](https://git-scm.com)
* [VS Code](https://code.visualstudio.com) (Strongly suggest)

## How to run locally

1. Change the database connection string to your own database.  
    The connection string of this app is located at `./TravelInCloud/appsettings.json`  
    About How to write connection string please view [here](https://www.connectionstrings.com/)
2. Excute `bower install` to download all front-end packages
3. Excute `dotnet restore` to restore all dotnet requirements
4. Excute `dotnet ef database update` to update your database
5. Excute `dotnet run` to run the app
6. Use your browser to view [http://localhost:5000](http://localhost:5000)

## How to publish to your server

1. Prepare a Linux or Windows Server
2. Excute `dotnet publish`
3. Copy `./TravelInCloud/bin/Debug/netcoreapp1.1/publish` path to your server
4. Excute `dotnet TravelInCloud.dll` at `./TravelInCloud/bin/Debug/netcoreapp1.1/publish` path on your server

**When you have successfully published the app to your server, it is not running properly! You need the following setps.**

## How to run in WeChat

1. Create a WeChat MP Account at [https://mp.weixin.qq.com](https://mp.weixin.qq.com)
2. Start developer mode in your account
3. Set proper app domain
4. Set proper oauth domain
5. Set proper js domain
6. Set proper appid and appsecret
7. Validate your wechat app at `ServerRoot\api\WeChatVerify`
8. Set your menu at `ServerRoot\api\ApplyMenu`
9. Configure your WeChat pay
10. Chnfigure your WeChat OAuth authorization

For more information about how to run it in your WeChat account, please view WeChat document at [https://mp.weixin.qq.com/wiki](https://mp.weixin.qq.com/wiki)

## How to contribute

Directly fork this repo and edit it. When you have successfully fixed an issue or have successfully made some useful change, please create a pull request.

If you are faceing any problem, please disscuss it at Issues.