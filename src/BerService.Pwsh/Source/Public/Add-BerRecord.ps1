Set-StrictMode -Version Latest

function Add-BerRecord {
    [CmdletBinding(DefaultParameterSetName = 'ByFunctionKey')]
    param(
        [Parameter(Mandatory = $True)]
        [string]
        $berUrl,

        [Parameter(ParameterSetName = 'ByJWT', Mandatory = $True)]
        [string]
        $clientId,

        [Parameter(ParameterSetName = 'ByFunctionKey', Mandatory = $True)]
        [string]
        $functionKey,

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

    Process {

        # Update the BER server
        $recordsUrl = _joinParts $berUrl, "/api/records/"

        $body = @{
            ApplicationName = $appName
            DataType        = $dataType
            Version         = $version
            Value           = $value
        }

        $json = ConvertTo-Json $body -Compress

        if ($clientId) {
            $token = _auth -berUrl $berUrl -clientId $clientId -ErrorAction Stop
            $response = Invoke-RestMethod -Uri $recordsUrl -Method Post -Body $json -ContentType 'application/json' -Headers @{Authorization = "Bearer $token"}
        }
        else {
            $recordsUrl = _joinParts $recordsUrl, "?code=$functionKey"
            $response = Invoke-RestMethod -Uri $recordsUrl -Method Post -Body $json -ContentType 'application/json'
        }

        Write-Output $response
    }
}