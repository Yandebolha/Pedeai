# Fix corrupted emoji chars in Designer.cs (U+FFFD → correct emoji)
$utf8bom = New-Object System.Text.UTF8Encoding($true)

# --- Form1.Designer.cs ---
$f = "Pedeai\Form1.Designer.cs"
$content = [System.IO.File]::ReadAllText($f, [System.Text.Encoding]::UTF8)
$fffd  = [char]65533
$money = [char]::ConvertFromUtf32(0x1F4B0)  # 💰
$cart  = [char]::ConvertFromUtf32(0x1F6D2)  # 🛒
$content = $content.Replace($fffd + "  Financeiro", $money + "  Financeiro")
$content = $content.Replace('"' + $fffd + $cart + "  Produtos", '"' + $cart + "  Produtos")
[System.IO.File]::WriteAllText($f, $content, $utf8bom)
Write-Host "Designer.cs fixed"

# --- Form1.cs: also convert to UTF-8 BOM so \u escapes are kept clean ---
$f2 = "Pedeai\Form1.cs"
$content2 = [System.IO.File]::ReadAllText($f2, [System.Text.Encoding]::UTF8)
[System.IO.File]::WriteAllText($f2, $content2, $utf8bom)
Write-Host "Form1.cs re-saved as UTF-8 BOM"

# Verify Financeiro line
$verify = [System.IO.File]::ReadAllText($f, [System.Text.Encoding]::UTF8)
$i = $verify.IndexOf("Financeiro")
$sub = $verify.Substring($i-5, 15)
$codes = $sub.ToCharArray() | ForEach-Object { [int]$_ }
Write-Host "Char codes before Financeiro: $codes"
