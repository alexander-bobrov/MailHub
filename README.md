# MailHub

## Description
Disposable mail service for general QA or Development usage

It allows you to receive e-mails from anywhere and get them through a simple REST API

Notice that sending e-mails is not allowed

## Prerequisites
Since it is based on an SMTP server you have to get the DNS name from some DNS register.

The options are: 
 - Get a free one, e.g.: https://www.cloudns.net/
 - Ask your company you work with for one 

## Setup 
### Azure 
Notice that service won't work within **free** Azure account since port 25 is blocked for this kind of account. More details:
https://docs.microsoft.com/en-us/azure/virtual-network/troubleshoot-outbound-smtp-connectivity

There's arm-template for azure that will configure all the environments for you on Ubuntu Virtual Machine:
 - Go to https://portal.azure.com/
 - Create a resource -> Resource group
 - Use the search box to find **Deploy a custom template** 
 - Choose to build your own template in the editor
 - Find **Load file** button and choose arm-template from this repository
 - Save and fill required field (they're self-explanatory)
 - Hit **Review+Create** button
 - When everything is created go to your resource group, find **DNS zone** resource and fill **A** and **MX** records
 - The previous step also has to be done in the admin panel of DNS register you got DNS from
 - Connect to created **Virtual Machine** and follow **Dedicated Server** instruction

### Dedicated Server
The simplest deployment process could be like that:
 - Install dotnet on your server
 - git clone https://github.com/alexander-bobrov/MailHub.git
 - **dotnet build -c release** within cloned directory
 - Edit appsettings.json according to your needs
 - Now you can run it for test **dotnet MailHub.dll --urls "http://example.site.com:5001"**

To make it work as a service you have some options depending on your environment
 - For Linux based OS you can use **systemd**, follow this instruction: https://dejanstojanovic.net/aspnet/2018/june/setting-up-net-core-servicedaemon-on-linux-os/
 - Or you can use docker for both Linux and Windows, simply build the image with command withing **src** directory **docker build -f MailHub\Dockerfile -t MailHub .**  and run it **docker run MailHub**

