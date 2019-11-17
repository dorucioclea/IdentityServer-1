# Parnassian Studios Identity Server Source
**(WORK IN PROGRESS! This application is not in a working state at the moment.)**

The source code for the Parnassian Studios identity server app.
IdentityServer4 is an OpenID Connect and OAuth 2.0 framework for ASP.NET Core.
Using this framework, we allow all of our apps to authenticate to one central service, this app, to share a login with all apps in the suite.

## License
Parnassian Studios is a strong believer in the open-source paradigm, and so this web app (and the others below) are made freely available under the MIT License.
Feel free to fork, make changes, use, sell, whatever- privately or commercially. We would appreciate attribution, but it isn't required.
If you make any changes or additions that you believe would be useful to the wider community, feel free to make a pull/merge request.

## How We Use These
This is one app in a suite which are run on the Parnassian Studios server.
We run each app as containers in Docker so that they can be scaled up and down independently from one-another based on the resources each needs.
We use a single Microsoft SQL Server 2017 instance shared across all apps, plus one shared Redis cache and individual Redis caches for each app.
A single data volume is shared between all apps- this is univerally bound to /data/shared/ in each container.
Each app also has its own volume, shared with no one else, so that it can persist some of its own data- this is bound universally to /data/ in each container.

Secrets and environment variables are used in order to override the values for configuration keys in appsettings.json.
All environmental variables used in the application will be represented there, in order to demonstrate all settings that can be easily overridden.
If you add additional configuration keys, please add default values to appsettings.json as well.
You can alternatively use appsettings.development/staging/production.json as overrides, but be sure to add to .gitignore so the values aren't exposed in your fork.

## All Parnassian Studios Web Apps
* [Web Portal](https://github.com/ParnassianStudios/Website)
* [Identity Server](https://github.com/ParnassianStudios/IdentityServer)
* [Anime Database](https://github.com/ParnassianStudios/AnimeDB)
* [Visual Novel Reader](https://github.com/ParnassianStudios/VNR)

## Technologies Used
* [ASP.NET Core (C#)](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-3.0)
* [Docker](https://docs.docker.com/)
* [Bootstrap](https://getbootstrap.com/docs/4.1/getting-started/introduction/)
* [Font-Awesome](https://fontawesome.com/how-to-use/on-the-web/referencing-icons/basic-use)
* Javascript ([jQuery](https://api.jquery.com/), [Validation](https://jqueryvalidation.org/reference/))
* [Visual Studio 2019](https://visualstudio.microsoft.com/vs/)