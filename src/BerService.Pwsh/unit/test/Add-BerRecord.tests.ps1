Set-StrictMode -Version Latest

. "$PSScriptRoot\..\..\dist\Trackyon.Ber.functions.ps1"

Describe 'Add-BerRecord' {
    Mock _auth { return 'testToken' } -Verifiable
    Mock Invoke-RestMethod {
        # If this test fails uncomment the line below to see how the mock was called.
        # Write-Host $args
        return @{}
    } -Verifiable

    Context 'Execute with JWT' {
        It 'Should run without error' {
            {
                Add-BerRecord -Version '1.0' -berUrl 'https://ber.test.com' -clientId test -appName 'unitTests' -dataType 'test' -value 'pass'
            } | Should Not Throw

            Assert-MockCalled _auth -Exactly 1

            Assert-MockCalled Invoke-RestMethod -Exactly 1 -ParameterFilter {
                $Uri -like "https://ber.test.com/api/records*" -and
                $Body -like "*`"Value`":`"pass`"*" -and
                $Body -like "*`"ApplicationName`":`"unitTests`"*" -and
                $Body -like "*`"DataType`":`"test`"*" -and
                $Body -like "*`"Version`":`"1.0`"*" -and
                $ContentType -eq "application/json" -and
                $Method -eq "Post"
            }
        }
    }

    Context 'Execute with no JWT' {
        It 'Should run without error' {
            {
                Add-BerRecord -Version '1.0' -berUrl 'https://ber.test.com' -functionKey 'key' -appName 'unitTests' -dataType 'test' -value 'pass'
            } | Should Not Throw

            Assert-MockCalled _auth -Exactly 0

            Assert-MockCalled Invoke-RestMethod -Exactly 1 -ParameterFilter {
                $Uri -like "https://ber.test.com/api/records*" -and
                $Uri -like "*?code=key" -and
                $Body -like "*`"Value`":`"pass`"*" -and
                $Body -like "*`"ApplicationName`":`"unitTests`"*" -and
                $Body -like "*`"DataType`":`"test`"*" -and
                $Body -like "*`"Version`":`"1.0`"*" -and
                $ContentType -eq "application/json" -and
                $Method -eq "Post"
            }
        }
    }
}