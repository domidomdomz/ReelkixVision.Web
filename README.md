# Reelkix Vision Web API

Web API service of Reelkix Vision using ASP.Net Web API (C#, .NET 8)

This exposes an endpoint that receives an image file from the UI client and processes the uploaded image for compression, if necessary, and uploads it to AWS S3 for CDN storage.
Implemented feature flag as well for Upload logs if we want to keep track of the uploaded images history.
