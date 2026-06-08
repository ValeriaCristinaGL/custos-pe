#!/usr/bin/env bash
# ============================================================
#  run-tests.sh — Executa testes automatizados + gera cobertura
#  Uso: bash run-tests.sh
# ============================================================
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
RESULTS_DIR="$SCRIPT_DIR/TestResults"
REPORT_DIR="$RESULTS_DIR/CoverageReport"
SOLUTION_FILE="$SCRIPT_DIR/TransparenciaPE.sln"

echo ""
echo "============================================================"
echo "  TransparênciaPE — Testes Automatizados e Cobertura"
echo "============================================================"
echo ""

# ── 1. Verificar .NET ────────────────────────────────────────
if ! command -v dotnet &>/dev/null; then
  echo "[ERRO] 'dotnet' não encontrado. Instale o .NET SDK:"
  echo "       https://dotnet.microsoft.com/download"
  exit 1
fi
echo "[OK] .NET SDK: $(dotnet --version)"

# ── 2. Verificar / instalar reportgenerator ──────────────────
if ! command -v reportgenerator &>/dev/null; then
  echo ""
  echo "[INFO] 'reportgenerator' não encontrado. Instalando..."
  dotnet tool install --global dotnet-reportgenerator-globaltool
  # Garante que ferramentas globais do dotnet estejam no PATH
  export PATH="$PATH:$HOME/.dotnet/tools"
fi
echo "[OK] reportgenerator: $(reportgenerator --version 2>/dev/null | head -1)"

# ── 3. Limpar resultados anteriores ─────────────────────────
echo ""
echo "[INFO] Limpando resultados anteriores..."
rm -rf "$RESULTS_DIR"

# ── 4. Executar testes com coleta de cobertura ───────────────
echo ""
echo "[INFO] Executando testes unitários e de integração..."
echo "------------------------------------------------------------"
dotnet test "$SOLUTION_FILE" \
  --collect:"XPlat Code Coverage" \
  --settings "$SCRIPT_DIR/coverlet.runsettings" \
  --results-directory "$RESULTS_DIR"
echo "------------------------------------------------------------"

# ── 5. Gerar relatório HTML ──────────────────────────────────
echo ""
echo "[INFO] Gerando relatório de cobertura..."
reportgenerator \
  -reports:"$RESULTS_DIR/*/coverage.opencover.xml" \
  -targetdir:"$REPORT_DIR" \
  -reporttypes:"Html;TextSummary" \
  -assemblyfilters:"-*Tests*;-*Migrations*"

# ── 6. Exibir resumo no terminal ─────────────────────────────
echo ""
echo "============================================================"
echo "  RESUMO DE COBERTURA"
echo "============================================================"
cat "$REPORT_DIR/Summary.txt"

# ── 7. Abrir relatório no navegador ─────────────────────────
echo ""
echo "[INFO] Relatório HTML disponível em:"
echo "       $REPORT_DIR/index.html"
echo ""

# Tenta abrir automaticamente (Linux com xdg-open, macOS com open)
if command -v xdg-open &>/dev/null; then
  xdg-open "$REPORT_DIR/index.html" &>/dev/null &
elif command -v open &>/dev/null; then
  open "$REPORT_DIR/index.html"
else
  echo "[INFO] Abra o arquivo acima manualmente no navegador."
fi

echo "[CONCLUÍDO]"
echo ""
