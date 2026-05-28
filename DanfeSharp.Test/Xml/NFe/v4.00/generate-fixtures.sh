#!/usr/bin/env bash
# Gerador das fixtures de demonstração da PR #42 (#39).
# Produz 3 XMLs de NF-e modelo 55 v4.00 cobrindo cenários distintos:
#   - v4_DanfeMinimo.xml         (1 item, sem cobr, sem pag)
#   - v4_DanfeIntermediario.xml  (5 itens, 2 duplicatas, 2 detPag, mix infAdProd)
#   - v4_DanfeCompleto.xml       (20 itens, 6 duplicatas, 3 detPag, todos infAdProd)
#
# Re-execute esse script se precisar regenerar:
#   bash DanfeSharp.Test/Xml/NFe/v4.00/generate-fixtures.sh
set -euo pipefail

cd "$(dirname "$0")"

# ----- Helpers -----
gen_item() {
    local nItem=$1
    local descricao=$2
    local vProd=$3
    local with_infAdProd=$4   # "yes" | "no"
    local infText="${5:-}"

    local vBC=$vProd
    local vICMS=$(awk -v v="$vProd" 'BEGIN{ printf "%.2f", v*0.12 }')
    local vPIS=$(awk -v v="$vProd" 'BEGIN{ printf "%.2f", v*0.0165 }')
    local vCOFINS=$(awk -v v="$vProd" 'BEGIN{ printf "%.2f", v*0.076 }')

    cat <<XMLITEM
            <det nItem="$nItem">
                <prod>
                    <cProd>PROD-$nItem</cProd>
                    <cEAN>SEM GTIN</cEAN>
                    <xProd>$descricao</xProd>
                    <NCM>49111090</NCM>
                    <CFOP>5102</CFOP>
                    <uCom>UN</uCom>
                    <qCom>1.0000</qCom>
                    <vUnCom>$vProd</vUnCom>
                    <vProd>$vProd</vProd>
                    <cEANTrib>SEM GTIN</cEANTrib>
                    <uTrib>UN</uTrib>
                    <qTrib>1.0000</qTrib>
                    <vUnTrib>$vProd</vUnTrib>
                    <indTot>1</indTot>
                </prod>
                <imposto>
                    <ICMS>
                        <ICMS00>
                            <orig>0</orig>
                            <CST>00</CST>
                            <modBC>3</modBC>
                            <vBC>$vBC</vBC>
                            <pICMS>12.00</pICMS>
                            <vICMS>$vICMS</vICMS>
                        </ICMS00>
                    </ICMS>
                    <PIS>
                        <PISAliq>
                            <CST>01</CST>
                            <vBC>$vBC</vBC>
                            <pPIS>1.65</pPIS>
                            <vPIS>$vPIS</vPIS>
                        </PISAliq>
                    </PIS>
                    <COFINS>
                        <COFINSAliq>
                            <CST>01</CST>
                            <vBC>$vBC</vBC>
                            <pCOFINS>7.60</pCOFINS>
                            <vCOFINS>$vCOFINS</vCOFINS>
                        </COFINSAliq>
                    </COFINS>
                </imposto>
XMLITEM
    if [ "$with_infAdProd" = "yes" ]; then
        echo "                <infAdProd>$infText</infAdProd>"
    fi
    echo "            </det>"
}

gen_dup() {
    local nDup=$1
    local dVenc=$2
    local vDup=$3
    cat <<XMLDUP
                <dup>
                    <nDup>$nDup</nDup>
                    <dVenc>$dVenc</dVenc>
                    <vDup>$vDup</vDup>
                </dup>
XMLDUP
}

gen_detpag() {
    local tPag=$1
    local vPag=$2
    local xPag=${3:-}
    if [ -n "$xPag" ]; then
        cat <<XMLP
                <detPag>
                    <tPag>$tPag</tPag>
                    <xPag>$xPag</xPag>
                    <vPag>$vPag</vPag>
                </detPag>
XMLP
    else
        cat <<XMLP
                <detPag>
                    <tPag>$tPag</tPag>
                    <vPag>$vPag</vPag>
                </detPag>
XMLP
    fi
}

# Header comum (emit / dest / transp / protocolo). Argumentos: chavePad (38 dígitos), nNF, vTot
# A chave de acesso da NF-e tem 44 dígitos NUMÉRICOS (sem letras) — concatenamos
# o prefixo padrão "351105" + 38 dígitos do chavePad.
gen_header() {
    local chavePad=$1
    local nNF=$2
    local vTot=$3
    local vICMS=$(awk -v v="$vTot" 'BEGIN{ printf "%.2f", v*0.12 }')
    local vPIS=$(awk -v v="$vTot" 'BEGIN{ printf "%.2f", v*0.0165 }')
    local vCOFINS=$(awk -v v="$vTot" 'BEGIN{ printf "%.2f", v*0.076 }')

    cat <<XMLHDR
<?xml version="1.0" encoding="UTF-8"?>
<nfeProc versao="4.00" xmlns="http://www.portalfiscal.inf.br/nfe">
    <NFe xmlns="http://www.portalfiscal.inf.br/nfe">
        <infNFe Id="NFe351105${chavePad}" versao="4.00">
            <ide>
                <cUF>35</cUF>
                <cNF>$nNF</cNF>
                <natOp>VENDA</natOp>
                <mod>55</mod>
                <serie>1</serie>
                <nNF>$nNF</nNF>
                <dhEmi>2026-05-28T14:00:00-03:00</dhEmi>
                <dhSaiEnt>2026-05-28T14:00:00-03:00</dhSaiEnt>
                <tpNF>1</tpNF>
                <idDest>1</idDest>
                <cMunFG>3550308</cMunFG>
                <tpImp>1</tpImp>
                <tpEmis>1</tpEmis>
                <cDV>1</cDV>
                <tpAmb>1</tpAmb>
                <finNFe>1</finNFe>
                <indFinal>1</indFinal>
                <indPres>1</indPres>
                <procEmi>0</procEmi>
                <verProc>1.0</verProc>
            </ide>
            <emit>
                <CNPJ>22257735000138</CNPJ>
                <xNome>Demo Emitente LTDA</xNome>
                <xFant>Demo Co</xFant>
                <enderEmit>
                    <xLgr>Av. Demo</xLgr>
                    <nro>100</nro>
                    <xBairro>Centro</xBairro>
                    <cMun>3550308</cMun>
                    <xMun>São Paulo</xMun>
                    <UF>SP</UF>
                    <CEP>01000000</CEP>
                    <cPais>1058</cPais>
                    <xPais>BRASIL</xPais>
                    <fone>1133334444</fone>
                </enderEmit>
                <IE>123456789012</IE>
                <CRT>3</CRT>
            </emit>
            <dest>
                <CNPJ>11222333000181</CNPJ>
                <xNome>Demo Destinatário S/A</xNome>
                <enderDest>
                    <xLgr>Rua Cliente</xLgr>
                    <nro>200</nro>
                    <xBairro>Bairro Cliente</xBairro>
                    <cMun>3550308</cMun>
                    <xMun>São Paulo</xMun>
                    <UF>SP</UF>
                    <CEP>02000000</CEP>
                    <cPais>1058</cPais>
                    <xPais>BRASIL</xPais>
                </enderDest>
                <indIEDest>9</indIEDest>
            </dest>
XMLHDR
}

# Footer comum (ICMSTot, transp, infAdic, protocolo). Argumentos: vTot, [cobr_xml], [pag_xml]
gen_footer() {
    local vTot=$1
    local cobr_block=${2:-}
    local pag_block=${3:-}
    local vICMS=$(awk -v v="$vTot" 'BEGIN{ printf "%.2f", v*0.12 }')
    local vPIS=$(awk -v v="$vTot" 'BEGIN{ printf "%.2f", v*0.0165 }')
    local vCOFINS=$(awk -v v="$vTot" 'BEGIN{ printf "%.2f", v*0.076 }')

    cat <<XMLFOOT
            <total>
                <ICMSTot>
                    <vBC>$vTot</vBC>
                    <vICMS>$vICMS</vICMS>
                    <vICMSDeson>0.00</vICMSDeson>
                    <vFCP>0.00</vFCP>
                    <vBCST>0.00</vBCST>
                    <vST>0.00</vST>
                    <vFCPST>0.00</vFCPST>
                    <vFCPSTRet>0.00</vFCPSTRet>
                    <vProd>$vTot</vProd>
                    <vFrete>0.00</vFrete>
                    <vSeg>0.00</vSeg>
                    <vDesc>0.00</vDesc>
                    <vII>0.00</vII>
                    <vIPI>0.00</vIPI>
                    <vIPIDevol>0.00</vIPIDevol>
                    <vPIS>$vPIS</vPIS>
                    <vCOFINS>$vCOFINS</vCOFINS>
                    <vOutro>0.00</vOutro>
                    <vNF>$vTot</vNF>
                    <vTotTrib>0.00</vTotTrib>
                </ICMSTot>
            </total>
            <transp>
                <modFrete>9</modFrete>
            </transp>
XMLFOOT
    if [ -n "$cobr_block" ]; then
        echo "$cobr_block"
    fi
    if [ -n "$pag_block" ]; then
        echo "$pag_block"
    fi
    cat <<'XMLFOOT2'
            <infAdic>
                <infCpl>Fixture sintética gerada por generate-fixtures.sh para demo da PR #42.</infCpl>
            </infAdic>
        </infNFe>
    </NFe>
    <protNFe versao="4.00">
        <infProt>
            <tpAmb>1</tpAmb>
            <verAplic>SP</verAplic>
            <chNFe>35110501000000000000000000000000000000000000</chNFe>
            <dhRecbto>2026-05-28T14:01:00-03:00</dhRecbto>
            <nProt>135260000000000</nProt>
            <digVal>aaa=</digVal>
            <cStat>100</cStat>
            <xMotivo>Autorizado o uso da NF-e</xMotivo>
        </infProt>
    </protNFe>
</nfeProc>
XMLFOOT2
}

# ===========================================================================
# 1. v4_DanfeMinimo.xml — 1 item, sem cobr, sem pag, sem infAdProd
# ===========================================================================
{
    gen_header "10000000000000000000000000000000000001" "100000001" "50.00"
    gen_item 1 "Caneta esferográfica azul" "50.00" "no"
    gen_footer "50.00" "" ""
} > v4_DanfeMinimo.xml
echo "✓ v4_DanfeMinimo.xml gerado"

# ===========================================================================
# 2. v4_DanfeIntermediario.xml — 5 itens (3 com infAdProd, 2 sem),
#                                 2 duplicatas, 2 detPag
# ===========================================================================
TOTAL_INTER="1000.00"
{
    gen_header "20000000000000000000000000000000000002" "200000001" "$TOTAL_INTER"
    gen_item 1 "Notebook Demo Modelo X" "300.00" "yes" "S/N: NB-001 · Garantia: 12 meses"
    gen_item 2 "Mouse sem fio bluetooth"  "50.00" "no"
    gen_item 3 "Teclado mecânico ABNT2"  "200.00" "yes" "Switch: Cherry MX Brown · Layout: ABNT2"
    gen_item 4 "Cabo USB-C 1m"             "30.00" "no"
    gen_item 5 "Monitor 24 polegadas"    "420.00" "yes" "Resolução: 1920x1080 · 75Hz · Modelo: MN-24FHD"
    cobr_inter=$(cat <<XMLCOBR
            <cobr>
                <fat>
                    <nFat>200000001</nFat>
                    <vOrig>$TOTAL_INTER</vOrig>
                    <vDesc>0.00</vDesc>
                    <vLiq>$TOTAL_INTER</vLiq>
                </fat>
$(gen_dup "001" "2026-06-28" "500.00")$(gen_dup "002" "2026-07-28" "500.00")
            </cobr>
XMLCOBR
)
    pag_inter=$(cat <<XMLPAG
            <pag>
$(gen_detpag "03" "600.00")$(gen_detpag "17" "400.00")
            </pag>
XMLPAG
)
    gen_footer "$TOTAL_INTER" "$cobr_inter" "$pag_inter"
} > v4_DanfeIntermediario.xml
echo "✓ v4_DanfeIntermediario.xml gerado"

# ===========================================================================
# 3. v4_DanfeCompleto.xml — 20 itens (todos com infAdProd),
#                           6 duplicatas, 3 detPag
# ===========================================================================
TOTAL_COMP="2000.00"
PER_ITEM="100.00"
ITEM_DESCRICOES=(
    "Item demo A — material de escritório"
    "Item demo B — papelaria fina"
    "Item demo C — suprimento gráfico"
    "Item demo D — material para eventos"
    "Item demo E — kit de apresentação"
    "Item demo F — material institucional"
    "Item demo G — embalagem premium"
    "Item demo H — material de divulgação"
    "Item demo I — kit corporativo"
    "Item demo J — material expositor"
    "Item demo K — brinde personalizado"
    "Item demo L — material para feira"
    "Item demo M — kit promocional"
    "Item demo N — embalagem padrão"
    "Item demo O — papel timbrado"
    "Item demo P — material de campanha"
    "Item demo Q — bobina térmica"
    "Item demo R — bloco de notas"
    "Item demo S — agenda corporativa"
    "Item demo T — calendário 2026"
)
{
    gen_header "30000000000000000000000000000000000003" "300000001" "$TOTAL_COMP"
    for i in $(seq 1 20); do
        idx=$((i-1))
        gen_item "$i" "${ITEM_DESCRICOES[$idx]}" "$PER_ITEM" "yes" "Lote LT-$i-2026 · Validade 12 meses · Origem: Brasil"
    done
    DUP_MES=("2026-06-30" "2026-07-30" "2026-08-30" "2026-09-30" "2026-10-30" "2026-11-30")
    cobr_comp_dups=""
    for j in $(seq 0 5); do
        n=$(printf "%03d" $((j+1)))
        cobr_comp_dups+=$(gen_dup "$n" "${DUP_MES[$j]}" "333.34")
    done
    cobr_comp=$(cat <<XMLCOBR
            <cobr>
                <fat>
                    <nFat>300000001</nFat>
                    <vOrig>$TOTAL_COMP</vOrig>
                    <vDesc>0.00</vDesc>
                    <vLiq>2000.04</vLiq>
                </fat>
${cobr_comp_dups}
            </cobr>
XMLCOBR
)
    pag_comp=$(cat <<XMLPAG
            <pag>
$(gen_detpag "17" "800.00")$(gen_detpag "03" "700.00")$(gen_detpag "99" "500.00" "PROMOCIONAL BLACK FRIDAY")
            </pag>
XMLPAG
)
    gen_footer "$TOTAL_COMP" "$cobr_comp" "$pag_comp"
} > v4_DanfeCompleto.xml
echo "✓ v4_DanfeCompleto.xml gerado"

echo ""
echo "=== Arquivos gerados ==="
ls -la v4_Danfe*.xml
