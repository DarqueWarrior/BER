Set-StrictMode -Version Latest

function Remove-BerRecord {
   [CmdletBinding(DefaultParameterSetName = 'ByFunctionKey', SupportsShouldProcess = $true, ConfirmImpact = "Medium")]
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

      [switch]
      $force
   )

   Process {
      if ($Force -or $pscmdlet.ShouldProcess("$appName/$dataType/$version", "Remove Record")) {
         $recordsUrl = _joinParts $berUrl, "/api/records/$appName/$dataType/$version"

         if ($clientId) {
            $token = _auth -berUrl $berUrl -clientId $clientId -ErrorAction Stop
            Invoke-RestMethod -Uri $recordsUrl -Method Delete -Headers @{Authorization = "Bearer $token"}
         }
         else {
            $recordsUrl = _joinParts $recordsUrl, "?code=$functionKey"
            Invoke-RestMethod -Uri $recordsUrl -Method Delete
         }
      }
   }
}