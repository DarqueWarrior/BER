
# Makes sure we don't have to many / in the paths.
function _joinParts {
    param
    (
        [string[]]
        $parts = $null,

        [string]
        $separator = '/'
    )

    ($parts | Where-Object { $_ } | ForEach-Object { ([string]$_).trim($separator) } | Where-Object { $_ } ) -join $separator
}

# Get the JWT token when hosting in a web site. This is not used when calling an Azure Function.
function _auth {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $True)]
        [string]
        $berUrl,

        [Parameter(Mandatory = $True)]
        [string]
        $clientId
    )

    # Get the JWT token
    $authUrl = _joinParts $berUrl, "/api/auth/token/"

    $body = @{
        UserAgent = $clientId
    }

    $json = ConvertTo-Json $body -Compress

    try {
        $response = Invoke-RestMethod -Uri $authUrl -Method Post -Body $json -ContentType 'application/json'
    }
    catch  [Exception] {
        throw "Error attempting to authenticate $($_.Exception.Message)"
    }


    Write-Output $response.token
}