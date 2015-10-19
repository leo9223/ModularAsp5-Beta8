# ModularAsp5-Beta8
#### Modules Plug-in/out Approach in ASP.net5 (MVC6)-Beta8

This technique is originaly used by StuartLeeks [ModularVNext](https://github.com/stuartleeks/ModularVNext) to achieve modular
approach in ASP.net5.I have just ported the project [ModularVNext](https://github.com/stuartleeks/ModularVNext) to latest 
beta8 of ASP.net5.


#### Structure

##### -- ModularAsp5-Beta8 (Main Web App)

This is main web app where all modules will be plugged. There is a **copy** task in **gulp.js** file that copies all the 
modules DLLs in the folder **"ModularAsp5-Beta8\artifacts\bin\ModulesDLLs"** (path from where main web app reads its modules) 
when we build main web app.

##### -- ModuleOne

A pluggable module

##### -- ModuleTwo

A pluggable module



### What has done ?

1. Every module has its controller and views
2. Main web app has its own controller and views


### How to run ?

1. Download the zip file
2. Open **ModularAsp5-Beta8.sln** in Visual Studio 2015
3. Build all the modules first i.e **ModuleOne** and **ModuleTwo**
4. Build main web app i.e **ModularAsp5-Beta8**
5. Now run main web app by clicking on **"IIS Express"** button


### How to test ?

1. Trigger "YourUrl/ModuleOne" or "YourUrl/ModuleTwo" to open respective modules
2. "YourUrl" will point to the **"HomeController"** in main web app

