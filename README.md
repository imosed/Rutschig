# Rutschig
A URL shortener written in C# using ASP.Net Core. It features PINs and expirations to protect the endpoint.

## Config
You can customize your instance of this URL shortener by changing appconfig.json. Values are...
* appName: customize the name of your instance; changes the top bar and copyright strings
* appDomain: strictly used for sitemap.xml; change this to the domain on which your instance is hosted
* shortenedLength: controls the number of characters generated for the shortened string
* maxPinLength: controls the number of digits the user can enter for the PIN
* maxExpirationDelta: controls how many days in the future the expiration date can be; 0 is no restriction
