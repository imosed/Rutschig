# Rutschig
A URL shortener written in C# using ASP.Net Core. It features PINs, expirations, and max hits to protect the endpoint. You can see it in action at https://www.rutschig.us.

## Config
You can customize your instance of this URL shortener by changing appconfig.json. Values are...
* appName: customize the name of your instance; changes the top bar and copyright strings
* shortenedLength: controls the number of characters generated for the shortened string
* maxPinLength: controls the number of digits the user can enter for the PIN
* maxExpirationDelta: controls how many days in the future the expiration date can be; 0 is no restriction
