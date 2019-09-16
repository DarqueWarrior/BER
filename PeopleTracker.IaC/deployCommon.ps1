[CmdletBinding()]
param(
   [Parameter(Mandatory = $True)]
   [string]
   $servicePrincipal,

   [Parameter(Mandatory = $True)]
   [string]
   $clientSecret,

   [Parameter(Mandatory = $True)]
   [string]
   $tenatId,

   [Parameter(Mandatory = $True)]
   [string]
   $subscription,

   [Parameter(Mandatory = $True)]
   [string]
   $resourceGroupName,

   [Parameter(Mandatory = $False)]
   [string]
   $location = 'eastus2',

   [Parameter(Mandatory = $True)]
   [string]
   $goDaddyKey,

   [Parameter(Mandatory = $True)]
   [string]
   $goDaddySecret,

   [Parameter(Mandatory = $false)]
   [string]
   $subDomain = 'ber',

   [Parameter(Mandatory = $False)]
   [string]
   $startip = '0.0.0.0',

   [Parameter(Mandatory = $False)]
   [string]
   $endip = '0.0.0.0',

   [Parameter(Mandatory = $True)]
   [string]
   $password,

   [Parameter(Mandatory = $False)]
   [string]
   $adminlogin = 'dbrown',

   [Parameter(Mandatory = $False)]
   [string]
   $output = 'jsonc'
)

# Add a random component to all names
# But the "random" part must be the same from run
# to run or new resources will be created. So use
# part of the Project ID
$guild = [GUID]$env:SYSTEM_TEAMPROJECTID
$rand = $guild.Guid.SubString(30)
$sqlServer = "bersqlserver$rand"
$webSiteName = "berwebapp$rand"

#region Install Dependencies

Install-Module -Name Trackyon.GoDaddy -Repository PSGallery -Force -Scope CurrentUser -AllowClobber -SkipPublisherCheck
Install-Module -Name Resolve-DNSNameOverHTTP -Scope CurrentUser -Force  -AllowClobber -SkipPublisherCheck

#endregion

#region Update DNS

# It takes time of the DNS to update so do this first
Write-Output "Mapping $subDomain to $webSiteName.azurewebsites.net with a CNAME"
$secureSecret = $goDaddySecret | ConvertTo-SecureString -AsPlainText -Force
Set-GDDomainRecord -Key $goDaddyKey -Secret $secureSecret -Domain peopletracker.us -Type CNAME -Name $subDomain -IPAddress "$webSiteName.azurewebsites.net" -TTL 600 -Force -ErrorAction Stop

#endregion

#region Login

###############################################################################
# Log in

az login --service-principal `
   --username $servicePrincipal `
   --password $clientSecret `
   --tenant $tenatId `
   --output $output

az account set --subscription $subscription

#endregion

#region Create Resource group

###############################################################################
#  Create Resource group

Write-Output "Creating the Resource Group $resourceGroupName"
az group create --name $resourceGroupName `
   --location $location `
   --output $output

#endregion

#region SQL

###############################################################################
#  Create a sql server
Write-Output "Creating the SQL Server $sqlServer."
az sql server create --name $sqlServer `
   --resource-group  $resourceGroupName `
   --location $location `
   --admin-user $adminlogin `
   --admin-password $password `
   --output $output

Write-Output  "Creating the Database berRecords."
az sql db create --name 'berRecords' `
   --resource-group $resourceGroupName `
   --server $sqlServer `
   --no-wait `
   --output $output   

# We are going to need the FQDN for later so grab it now.
$serverFQDN = $(az sql server show --name $sqlServer `
      --resource-group $resourceGroupName `
      --query 'fullyQualifiedDomainName' `
      --output tsv)

Write-Verbose "Server FQDN = $serverFQDN"

# Set the ServerFQDN variable in the pipeline for use by other tasks.
Write-Output "##vso[task.setvariable variable=ServerFQDN]$serverFQDN"

Write-Output "Configure a firewall rule for the server"
az sql server firewall-rule create --name AllowYourIp `
   --resource-group $resourceGroupName `
   --server $sqlServer `
   --start-ip-address $startip `
   --end-ip-address $endip `
   --output $output   

#endregion

#region Webapp

###############################################################################
# Creating the App Service
Write-Output "Creating the Service Plan resource for $webSiteName."
az appservice plan create --name $webSiteName `
   --resource-group $resourceGroupName `
   --sku S1 `
   --output $output

Write-Output "Creating the Web App resource for $webSiteName."
az webapp create --name $webSiteName `
   --resource-group $resourceGroupName `
   --plan $webSiteName `
   --output $output

Write-Output "Enable managed identities"
$principalId = az webapp identity assign --name $webSiteName `
   --resource-group $resourceGroupName `
   --query 'principalId' `
   --output tsv

# Grant identity access to database
Write-Output "Grant identity access to database"
az sql server ad-admin create --resource-group $resourceGroupName `
   --server-name $sqlServer `
   --display-name $webSiteName `
   --object-id $principalId `
   --output $output

# Set the connection string
az webapp config connection-string set --resource-group $resourceGroupName `
   --name $webSiteName `
   --slot-settings DefaultConnection="Server=tcp:$sqlServer.database.windows.net,1433;Database=berRecords;" `
   --connection-string-type SQLAzure `
   --output $output

#endregion

#region Map DNS

# Wait for the DNS record to resolve
while ($null -eq $(Resolve-DNSNameOverHTTP "$subDomain.peopletracker.us" CNAME).Answer) {
   Write-Output "CNAME for $subDomain.peopletracker.us no resolving retry in 30 seconds."
   Start-Sleep -Seconds 30
}

az webapp config hostname add --webapp-name $webSiteName `
   --resource-group $resourceGroupName `
   --hostname "$subDomain.peopletracker.us"

#endregion