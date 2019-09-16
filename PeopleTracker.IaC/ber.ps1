##############################################################################
#.SYNOPSIS
# Can be used in your CI/CD pipeline to send updates to your BER Server.
#
#.DESCRIPTION
# Back End Resolver (BER) Servers store information about the back end that
# supports mobile applications. The mobile app can query the BER server passing
# in the application name, version and data it needs and the information will
# be returned.  You hard code the BER server URL not the back end URL. That way
# as your back end moves from Dev => QA => Prod your mobile app does not have to
# rebuilt. It simply queries the BER server and the back end URL will be returned.
#
# The data is stored with a composite key made up of the following fields:
# - appName
# - dataType
# - version
#
#.PARAMETER berUrl
# This is the base URL of your BER server installation. Do not include api/. For
# example for my peopleTracker demo the base URL is http://ber.peopletracker.us
#
#.PARAMETER appName
# The name of the application requesting the data. This makes up part of the
# composite key.
#
#.PARAMETER dataType
# A string that represents the data type to be retrieved. This string can be
# any value. Just make sure and use the same value when setting and retrieving 
# the value. This makes up part of the composite key.
#
#.PARAMETER version
# The version of the application. This makes up part of the composite key.
#
#.PARAMETER value
# The value to store for the given appName, dataType and Version
# any value. Just make sure and use the same value when setting and retrieving 
# the value.
#
#.EXAMPLE
# PS C:\> .\ber.ps1 -berUrl http://ber.domain.com -appName MyApp -dataType back-end -version 1.0.0 -value testing
# applicationName : MyApp
# version         : 1.0.0
# dataType        : back-end
# value           : testing
# dateCreated     : 2019-04-19T01:41:56.1555411Z
# dateModified    :
##############################################################################
[CmdletBinding()]
param(
   [Parameter(Mandatory = $True)]
   [string]
   $berUrl,

   [Parameter(Mandatory = $True)]
   [string]
   $appName,

   [Parameter(Mandatory = $True)]
   [string]
   $dataType,

   [Parameter(Mandatory = $True)]
   [string]
   $version,

   [Parameter(Mandatory = $True)]
   [string]
   $value
)

# Get the JWT token
$authUrl = "$berUrl/api/auth/token/"

$body = @{
   UserAgent = $appName
}

$json = ConvertTo-Json $body -Compress

$response = Invoke-RestMethod -Uri $authUrl -Method Post -Body $json -ContentType 'application/json'

$token = $response.token

# Update the BER server
$recordsUrl = "$berUrl/api/records/"

$body = @{
   ApplicationName = $appName
   DataType        = $dataType
   Version         = $version
   Value           = $value
}

$json = ConvertTo-Json $body -Compress

$response = Invoke-RestMethod -Uri $recordsUrl -Method Post -Body $json -ContentType 'application/json' -Headers @{Authorization = "Bearer $token"}

Write-Output $response