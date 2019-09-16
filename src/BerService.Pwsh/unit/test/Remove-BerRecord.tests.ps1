Set-StrictMode -Version Latest

. "$PSScriptRoot\..\..\dist\Trackyon.Ber.functions.ps1"

Describe 'Remove-BerRecord' {
    Mock _auth { return 'testToken' } -Verifiable
    Mock Invoke-RestMethod {
        # If this test fails uncomment the line below to see how the mock was called.
        # Write-Host $args
        return @{}
    } -Verifiable

    Context 'Execute with no JWT' {
        It 'Should run without error' {
            {
                Remove-BerRecord -Version '1.0' -berUrl 'https://ber.test.com' -functionKey 'key' -dataType 'test' -appName 'unitTests'
            } | Should Not Throw

            Assert-MockCalled _auth -Exactly 0

            Assert-MockCalled Invoke-RestMethod -Exactly 1 -ParameterFilter {
                $Uri -like "https://ber.test.com/api/records/unitTests/test/1.0*" -and
                $Uri -like "*?code=key" -and
                $Method -eq "Delete"
            }
        }
    }

    Context 'Execute with JWT' {
        It 'Should run without error' {
            {
                Remove-BerRecord -Version '1.0' -berUrl 'https://ber.test.com' -clientId test -appName 'unitTests' -dataType 'test'
            } | Should Not Throw

            Assert-MockCalled _auth -Exactly 1

            Assert-MockCalled Invoke-RestMethod -Exactly 1 -ParameterFilter {
                $Uri -like "https://ber.test.com/api/records/unitTests/test/1.0*" -and
                $Method -eq "Delete"
            }
        }
    }
}