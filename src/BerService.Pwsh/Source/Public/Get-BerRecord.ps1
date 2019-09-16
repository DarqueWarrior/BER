Set-StrictMode -Version Latest

function Get-BerRecord {
    [CmdletBinding(DefaultParameterSetName = 'ListFunction')]
    param(
        [Parameter(Mandatory = $True)]
        [string]
        $berUrl,

        [Parameter(ParameterSetName = 'List', Mandatory = $True)]
        [Parameter(ParameterSetName = 'ByRecord', Mandatory = $True)]
        [string]
        $clientId,

        [Parameter(ParameterSetName = 'ListFunction', Mandatory = $True)]
        [Parameter(ParameterSetName = 'ByRecordFunction', Mandatory = $True)]
        [string]
        $functionKey,

        [Parameter(ParameterSetName = 'ByRecord', Mandatory = $True)]
        [Parameter(ParameterSetName = 'ByRecordFunction', Mandatory = $True)]
        [string]
        $appName,

        [Parameter(ParameterSetName = 'ByRecord', Mandatory = $True)]
        [Parameter(ParameterSetName = 'ByRecordFunction', Mandatory = $True)]
        [string]
        $dataType,

        [Parameter(ParameterSetName = 'ByRecord', Mandatory = $True)]
        [Parameter(ParameterSetName = 'ByRecordFunction', Mandatory = $True)]
        [string]
        $version
    )

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'ByRecord' -or
            $PSCmdlet.ParameterSetName -eq 'ByRecordFunction') {
            $recordsUrl = _joinParts $berUrl, "api/records/$appName/$dataType/$version"
        }
        else {
            $recordsUrl = _joinParts $berUrl, "api/records"
        }

        if ($clientId) {
            $token = _auth -berUrl $berUrl -clientId $clientId -ErrorAction Stop
            $response = Invoke-RestMethod -Uri $recordsUrl -Method Get -Headers @{Authorization = "Bearer $token"}
        }
        else {
            $recordsUrl = _joinParts $recordsUrl, "?code=$functionKey"
            $response = Invoke-RestMethod -Uri $recordsUrl -Method Get
        }

        Write-Output $response
    }
}