<!-- #include "./common/header.md" -->

# Add-BerRecord

## SYNOPSIS

<!-- #include "./Synopsis/Add-BerRecord.txt" -->

## SYNTAX

## DESCRIPTION

<!-- #include "./Synopsis/Add-BerRecord.txt" -->

## EXAMPLES

### Example 1

```powershell
PS C:\> Add-BerRecord -berUrl 'https://ber.test.com' -appName 'test' -version '1.0' -dataType 'api' -value 'testing'
```

Adds a record to the server

## PARAMETERS

<!-- #include "./params/berUrl.txt" -->

<!-- #include "./params/clientId.txt" -->

<!-- #include "./params/functionKey.txt" -->

<!-- #include "./params/appName.txt" -->

<!-- #include "./params/dataType.txt" -->

<!-- #include "./params/version.txt" -->

### -value

The value to add or update to.

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
