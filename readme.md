# TravelIC
TravelIC is a simple WeChat application which allows your WeChat followers sell and buy their own things.

TravelIC is based on dot net core.

## Requirements

* Windows Server or Ubuntu
* dot net Core 1.1.0 or later
* dot net Core SDK 1.0.1 or later
* SQL Server or SQL Server LocalDB
* bower

**bower depends on nodejs, npm and git!**

## How to run locally

1. Change the database connection string to your own database.
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

For more information about how to run it in your WeChat account, please view WeChat Document at [https://mp.weixin.qq.com/wiki](https://mp.weixin.qq.com/wiki)
