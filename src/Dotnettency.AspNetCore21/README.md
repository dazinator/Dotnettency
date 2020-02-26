## ASPNET Core

This project exists because it's not possible to create a single project to support the multiple versions
of ASP.NET Core on LTS (2.1, 3.1). This is because there are binary incompatible changes between these two version in
areas that dotnettency cares about like the Options system.

Therefore depending on which version of ASP.NET Core you are using you need to reference the right dotnettency aspnet core package for that version.

