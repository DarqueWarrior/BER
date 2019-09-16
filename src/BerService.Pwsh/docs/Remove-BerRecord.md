


# Remove-BerRecord

## SYNOPSIS
Removes a record from the server.


## SYNTAX

## DESCRIPTION
Removes a record from the server.


## EXAMPLES

### Example 1

```powershell
PS C:\> Remove-BerRecord -berUrl https://ber.test.com -appName test -version 1.0 -dataType api -Force
```

Removes a record from the server

## PARAMETERS

### -berUrl

The base URL to your BER server. Do not include api/record. That will be added by the functions. Just provide https://domain

```yaml
Type: String
Required: True
```

### -clientId

This is used to get a JWT token. The BER server when deployed to Docker or App Service as a Web App requires authentication using JWT. The clientId is used to make sure the client is allowed to connect to the server.

```yaml
Type: String
Required: True
Parameter Sets: ByJWT
```

### -functionKey

This is used as the key for calling an Azure function.

```yaml
Type: String
Required: True
Parameter Sets: ByFunctionKey
```

### -appName

The name of the appliction you want to work with.

```yaml
Type: String
Required: True
```

### -dataType

The type of data you want to retrieve.

```yaml
Type: String
Required: True
```

### -version

The version of the value to reterieve, add or update.

```yaml
Type: String
Required: True
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS

