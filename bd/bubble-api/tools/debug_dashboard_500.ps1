$ErrorActionPreference = 'Stop'

$base = 'http://127.0.0.1:8000/api/'
$email = ('test_' + [guid]::NewGuid().ToString('N').Substring(0, 8) + '@test.com')
$payload = @{
  name     = 'Test'
  email    = $email
  password = 'test1234'
} | ConvertTo-Json

$reg = Invoke-RestMethod -Uri ($base + 'desktop/register') -Method Post -ContentType 'application/json' -Body $payload -TimeoutSec 15
$token = $reg.token
$headers = @{ Authorization = "Bearer $token" }

try {
  $dash = Invoke-RestMethod -Uri ($base + 'desktop/dashboard?lat=42.2406&lng=-8.7261') -Headers $headers -Method Get -TimeoutSec 15
  Write-Output 'DASH_OK'
  Write-Output ("BUBBLES_COUNT=" + ($dash.bubbles | Measure-Object).Count)
} catch {
  Write-Output 'DASH_ERROR'

  $html = ''
  if ($_.Exception.Response) {
    $resp = $_.Exception.Response
    $sr = New-Object System.IO.StreamReader($resp.GetResponseStream())
    $html = $sr.ReadToEnd()
    $sr.Close()
  } else {
    $html = $_.Exception.Message
  }

  $msgMatch = [regex]::Match($html, 'exception-message[^>]*>\s*([^<]+)\s*<')
  if ($msgMatch.Success) {
    Write-Output ("EX_MSG=" + $msgMatch.Groups[1].Value)
  } else {
    Write-Output 'EX_MSG_NOT_FOUND'
  }

  $clsMatch = [regex]::Match($html, 'exception_class\\\":\\\"([^\\\\]+)')
  if ($clsMatch.Success) {
    Write-Output ("EX_CLASS=" + $clsMatch.Groups[1].Value)
  } else {
    Write-Output 'EX_CLASS_NOT_FOUND'
  }

  if ($html -match 'Malformed UTF-8') { Write-Output 'HIT_MALFORMED_UTF8' }
  if ($html -match 'Undefined') { Write-Output 'HIT_UNDEFINED' }
  if ($html -match 'Trying to access') { Write-Output 'HIT_TRYING_TO_ACCESS' }
  if ($html -match 'Call to') { Write-Output 'HIT_CALL_TO' }
}

