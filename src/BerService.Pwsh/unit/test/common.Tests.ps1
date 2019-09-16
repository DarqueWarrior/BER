Set-StrictMode -Version Latest

. "$PSScriptRoot\..\..\dist\Trackyon.Ber.functions.ps1"

Describe 'Common' {
    Context '_auth_positive' {
        $authPositiveResponse = Get-Content "$PSScriptRoot\sampleFiles\auth_positive_response.json" -Raw | ConvertFrom-Json

        Mock Invoke-RestMethod { return $authPositiveResponse }

        $actual = _auth -berUrl https://ber.somedomain.com -clientId testclientId

        It 'Should return plain text' {
            $actual | Should Be 'myToken'
        }
    }

    Context '_auth_negative' {
        Mock Invoke-RestMethod { throw 'boom' }

        It 'Should return plain text' {
            { _auth -berUrl https://ber.somedomain.com -clientId testclientId } | Should Throw
        }
    }

    Context "_joinParts" {
        It "Should combine without double /" {
            # Make sure if the parts both start and end with a / that we don't get a double
            _joinParts 'https://ber.test.com/', '/api/records/' | Should be 'https://ber.test.com/api/records'
        }

        It "Should combine with single /" {
            # Make sure if the parts both don't start or end with a / that we get a single /
            _joinParts 'https://ber.test.com', 'api/records/' | Should be 'https://ber.test.com/api/records'
        }

        It "Should combine with single /" {
            # Make sure if one part has a / already and the other does not it works
            _joinParts 'https://ber.test.com/', 'api/records/' | Should be 'https://ber.test.com/api/records'
        }

        It "Should combine multiple parts with single /" {
            # Make sure if one part has a / already and the other does not it works
            _joinParts 'https://ber.test.com/', 'api', '/records/' | Should be 'https://ber.test.com/api/records'
        }

        It "Should combine query string" {
            # Make sure if one part has a / already and the other does not it works
            _joinParts 'https://ber.test.com/', 'api', '/records', '?code=functionKey' | Should be 'https://ber.test.com/api/records/?code=functionKey'
        }
    }
}