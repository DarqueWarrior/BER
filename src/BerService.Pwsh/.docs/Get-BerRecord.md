<!-- #include "./common/header.md" -->

# Get-BerRecord

## SYNOPSIS

<!-- #include "./synopsis/Get-BerRecord.txt" -->

## SYNTAX

## DESCRIPTION

<!-- #include "./synopsis/Get-BerRecord.txt" -->

## EXAMPLES

### Example 1

```powershell
PS C:\> Get-BerRecord -berUrl https://ber.test.com -clientId test
```

This will return all the records in the server. The clientId is used to
get a JWT token.

### Example 2

```powershell
PS C:\> Get-BerRecord -berUrl https://ber.test.com -functionKey key -appName test -version 1.0 -dataType api
```

This will return a single record with the specified version and dataType for the app. The functionKey must be used when calling an Azure Function.

## PARAMETERS

<!-- #include "./params/berUrl.txt" -->

### -clientId

This is used to get and JWT token.

```yaml
Type: String
Required: True
Parameter Sets: ByRecord, List
```

### -functionKey

This is used as the key for calling an Azure function.

```yaml
Type: String
Required: True
Parameter Sets: ByRecordFunction, ListFunction
```

### -appName

The name of the appliction you want to work with.

```yaml
Type: String
Required: True
Parameter Sets: ByRecord, ByRecordFunction
```

### -dataType

The type of data you want to retrieve.

```yaml
Type: String
Required: True
Parameter Sets: ByRecord, ByRecordFunction
```

### -version

The version of the value to reterieve.

```yaml
Type: String
Required: True
Parameter Sets: ByRecord, ByRecordFunction
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
