$utf8bom = New-Object System.Text.UTF8Encoding($true)
$f = "Pedeai\Form1.cs"
$c = [System.IO.File]::ReadAllText($f, [System.Text.Encoding]::UTF8)
$fd = [char]65533

$pairs = @(
    @{ Old = $fd+"ltimos Pedidos";             New = "Ultimos Pedidos" },
    @{ Old = "Tabela de "+$fd+"ltimos pedidos"; New = "Tabela de Ultimos pedidos" },
    @{ Old = $fd+"ltimos pedidos";             New = "ultimos pedidos" },
    @{ Old = "Situa"+$fd+$fd+"o:";             New = "Situacao:" },
    @{ Old = "A"+$fd+$fd+"es";                 New = "Acoes" },
    @{ Old = "NAVEGA"+$fd+$fd+"O";             New = "NAVEGACAO" },
    @{ Old = "vis"+$fd+"vel";                  New = "visivel" },
    @{ Old = "Itens "+$fd+" Pedido";           New = "Itens - Pedido" },
    @{ Old = "Cancelar: pede confirma"+$fd+$fd+"o"; New = "Cancelar: pede confirmacao" },
    @{ Old = "Confirma"+$fd+$fd+"o";           New = "Confirmacao" },
    @{ Old = "se "+$fd+" finaliza"+$fd+$fd+"o e pede dados de pagamento"; New = "se e finalizacao e pede dados de pagamento" },
    @{ Old = "Cart"+$fd+"o ou Pix";            New = "Cartao ou Pix" },
    @{ Old = "Finalizar Pedido "+$fd+" Pagamento"; New = "Finalizar Pedido - Pagamento" },
    @{ Old = "C"+$fd+"digo Pix:";              New = "Codigo Pix:" },
    @{ Old = "C"+$fd+"d. Transa"+$fd+$fd+"o:"; New = "Cod. Transacao:" },
    @{ Old = "At"+$fd+":";                     New = "Ate:" },
    @{ Old = "Rodap"+$fd+" resumo";            New = "Rodape resumo" },
    @{ Old = "per"+$fd+"odo";                  New = "periodo" },
    @{ Old = "    "+$fd+"    ";                New = "    |    " }
)

foreach ($pair in $pairs) {
    $c = $c.Replace($pair.Old, $pair.New)
}
[System.IO.File]::WriteAllText($f, $c, $utf8bom)
$remaining = [regex]::Matches($c, [char]65533).Count
Write-Host "Done. Remaining FFFD: $remaining"
