# BER Service

The BER Service was demoed during [Donovan Brown's](https://twitter.com/DonovanBrown) talk at the [Xamarin Developer Summit 2019](https://www.youtube.com/watch?v=t1cQsenAmNo) in Houston TX.

The purpose of the service to update the back end URL of a mobile application without having to rebuild the mobile application. The BER Service is hosted at a URL that never changes and can be queried at runtime to get the URL of the back end. While the back end moves from environment to environment the BER Service is updated with the new back end URL for the mobile application to start using. The mobile application simply queries the BER Service before making a call to the back end to get the correct URL.

## Repo Contents

This repo contains following:

* BER Service Azure App Service Application with [Azure DevOps](https://docs.microsoft.com/en-us/azure/devops/pipelines) Pipeline
  * Azure App Service, WebApp, BER Service implementation
  * YAML based Azure DevOps CI/CD pipeline
* BER Service Client Library
  * Shared between the Azure Function and Azure WebApp implementations.
* BER Service Data Access Layer using Entity Framework
  * Shared between the Azure Function and Azure WebApp implementations.
* BER Service Azure Function App with [Azure DevOps](https://docs.microsoft.com/en-us/azure/devops/pipelines) Pipeline
  * Azure Function App, BER Service implementation
  * YAML based Azure DevOps CI/CD pipeline
* BER Service [Azure Resource Manager Templates](https://docs.microsoft.com/en-us/azure/templates/)
  * Infrastructure as Code for both WebApp and Azure Function
  * Azure Resource Manager Templates
* BER Service Model Library
  * Library that can be added to Xamarin applications to access a BER Service.
  * It is published on NuGet as [Trackyon.BER.Client](https://www.nuget.org/packages/Trackyon.BER.Client/)
  * There should be no reason to publish your own.
* BER Service PowerShell Module
  * Provided for educational purposes only.
  * The module is published as [Trackyon.Ber](https://www.powershellgallery.com/packages/Trackyon.Ber/0.0.1) in the PowerShell Gallery.
  * There should be no reason to publish your own.
* BER Service Unit Tests

## Prerequisites

* A GitHub account, where you can fork this repository. If you do not have one, you can [create one for free](https://github.com).

* An Azure DevOps organization. If you do not have one, you can [create one for free](../get-started/pipelines-sign-up.md). (An Azure DevOps organization is different from your GitHub organization. Give them the same name if you want alignment between them.)

* An Azure account. If you do not have one, you can [create one for free](https://azure.microsoft.com/free/).

## Deploy the Service as Azure Function

1. Fork this repository into your GitHub account.

1. Sign in to your Azure DevOps organization and navigate to your project.

1. Create an [Azure Resource Manager service connection](https://docs.microsoft.com/en-us/azure/devops/pipelines/library/service-endpoints) named **AzureConnection**.

1. In Azure, create an Azure Resource Group named **berfunc**.

1. In your project, navigate to the **Pipelines** page. Then choose the action to create a new pipeline.

1. Walk through the steps of the wizard by first selecting **GitHub** as the location of your source code.

1. You might be redirected to GitHub to sign in. If so, enter your GitHub credentials.

1. When the list of repositories appears, select your repository where you forked this repository.

1. Azure Pipelines will analyze your repository and recommend templates. Select **Existing Azure Pipelines YAML file**.

1. Under path select **azure-pipelines-function.yml**, then select **Continue**.

1. Select **Variables**, then select **New variable**, and then name the variable **administratorLoginPassword**. Next enter a strong password and check the **Keep this value secret** check box, and then choose **OK**, then **Save**.

1. Select **Run**. Once the pipeline is complete you will have a BER Service deployed into an Azure Function.

1. From the function app get the [URL and default key](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook#authorization-keys). 

## Using the Service

### From Mobile Application

To use the service in your mobile application you will need to add a reference to the BerService.Client library. You can build your own copy or download a version of [Trackyon.BER.Client](https://www.nuget.org/packages/Trackyon.BER.Client/) from NuGet.

When calling the function you are going to use the function key to authenticate.

```csharp
var ber = new Trackyon.BER.Client.BerFunctionClient(functionURL, functionKey, version, appName);
this.baseAddress = ber.Get("back-end");
```

### From DevOps Pipeline

To use the service in your DevOps pipeline you can call the REST API directly or use the cross-platform PowerShell Module [Trackyon.Ber](https://www.powershellgallery.com/packages/Trackyon.Ber/0.0.1).

```PowerShell
Install-Module Trackyon.Ber -Scope CurrentUser -Force -Verbose

Add-BerRecord -functionKey $(functionKey) -berUrl $(functionUrl) -appName $(appName) `
	-dataType 'back-end' -version $version `
	-value 'http://$(webSiteName).azurewebsites.net' -verbose
```

## Deploy the Service as App Service

1. Fork this repository into your GitHub account.

1. Sign in to your Azure DevOps organization and navigate to your project.

1. Create an [Azure Resource Manager service connection](https://docs.microsoft.com/en-us/azure/devops/pipelines/library/service-endpoints) named **AzureConnection**.

1. In Azure, create an Azure Resource Group named **berblog**.

1. In your project, navigate to the **Pipelines** page. Then choose the action to create a new pipeline.

1. Walk through the steps of the wizard by first selecting **GitHub** as the location of your source code.

1. You might be redirected to GitHub to sign in. If so, enter your GitHub credentials.

1. When the list of repositories appears, select your repository where you forked this repository.

1. Azure Pipelines will analyze your repository and recommend templates. Select **Existing Azure Pipelines YAML file**.

1. Under path select **azure-pipelines-website.yml**, then select **Continue**.

1. Select **Variables**, then select **New variable**, and then name the variable **administratorLoginPassword**. Next enter a strong password and check the **Keep this value secret** check box, and then choose **OK**, then **Save**.

1. Select **Run**. Once the pipeline is complete you will have a BER Service deployed into an Azure Function.

1. From the App Service get the URL. 

## Using the Service

### From Mobile Application

To use the service in your mobile application you will need to add a reference to the BerService.Client library. You can build your own copy or download a version of [Trackyon.BER.Client](https://www.nuget.org/packages/Trackyon.BER.Client/) from NuGet.

When calling the web application you are going to use clientId to request a JWT to authenticate. The clientId must match either the Web or Mobile properties of the TokenData object. Those values are read from the application configuration. You can update those values in the **azure-pipelines-website.yml** file.

```csharp
var ber = new Trackyon.BER.Client.BerClient(appServiceURL, clientId, version, appName);
this.baseAddress = ber.Get("back-end");
```

### From DevOps Pipeline

To use the service in your DevOps pipeline you can call the REST API directly or use the cross-platform PowerShell Module [Trackyon.Ber](https://www.powershellgallery.com/packages/Trackyon.Ber/0.0.1).

```PowerShell
Install-Module Trackyon.Ber -Scope CurrentUser -Force -Verbose

Add-BerRecord -clientId $(appName) -berUrl $(berUrl) -appName $(appName) `
	-dataType 'back-end' -version $version `
	-value 'http://$(webSiteName).azurewebsites.net' -verbose
```
