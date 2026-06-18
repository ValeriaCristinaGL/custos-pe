#!/usr/bin/env bash
# =============================================================================
# run-k6.sh — Script de execução dos testes de carga k6
# Transparência PE
# =============================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BASE_URL="${BASE_URL:-http://localhost:5034}"
RESULTS_DIR="${SCRIPT_DIR}/results"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")

# Cores para saída no terminal
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# -----------------------------------------------------------------------------
# Funções auxiliares
# -----------------------------------------------------------------------------

log_info()    { echo -e "${BLUE}[INFO]${NC} $*"; }
log_success() { echo -e "${GREEN}[OK]${NC}   $*"; }
log_warn()    { echo -e "${YELLOW}[WARN]${NC} $*"; }
log_error()   { echo -e "${RED}[ERRO]${NC} $*"; }

check_k6() {
  if ! command -v k6 &>/dev/null; then
    log_error "k6 não encontrado. Instale em: https://k6.io/docs/getting-started/installation/"
    echo ""
    echo "  Linux (apt):   sudo gpg -k && sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69 && echo 'deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main' | sudo tee /etc/apt/sources.list.d/k6.list && sudo apt-get update && sudo apt-get install k6"
    echo "  macOS (brew):  brew install k6"
    echo "  Docker:        docker run --rm -i grafana/k6 run - <script.js"
    exit 1
  fi
  log_success "k6 $(k6 version | head -1) encontrado"
}

check_api() {
  log_info "Verificando conectividade com a API em ${BASE_URL}..."
  if curl -sf --max-time 5 "${BASE_URL}/api/v1/dashboard/resumo" > /dev/null 2>&1; then
    log_success "API respondendo em ${BASE_URL}"
  else
    log_warn "Não foi possível conectar em ${BASE_URL}."
    log_warn "Certifique-se de que a API está rodando antes de executar os testes."

    if [[ "${CI:-false}" == "true" || "${K6_NON_INTERACTIVE:-false}" == "true" ]]; then
      log_error "Execução não interativa: abortando porque a API não respondeu."
      exit 1
    fi

    read -rp "Continuar mesmo assim? [s/N]: " choice
    [[ "${choice,,}" == "s" ]] || exit 1
  fi
}

confirm_destructive_test() {
  local name="$1"

  if [[ "${K6_CONFIRM_DESTRUCTIVE:-false}" == "true" ]]; then
    return 0
  fi

  if [[ "${CI:-false}" == "true" || "${K6_NON_INTERACTIVE:-false}" == "true" ]]; then
    log_error "${name} exige K6_CONFIRM_DESTRUCTIVE=true em execução não interativa."
    exit 1
  fi

  read -rp "Confirmar execução em ${BASE_URL}? [s/N]: " choice
  [[ "${choice,,}" == "s" ]] || exit 0
}

mkdir_results() {
  mkdir -p "${RESULTS_DIR}"
}

run_test() {
  local name="$1"
  local file="$2"
  shift 2
  local extra_args=("$@")

  log_info "Executando: ${name}"
  local output_file="${RESULTS_DIR}/${name}_${TIMESTAMP}.json"

  if k6 run \
      --out "json=${output_file}" \
      -e "BASE_URL=${BASE_URL}" \
      "${extra_args[@]}" \
      "${SCRIPT_DIR}/tests/${file}"; then
    log_success "${name} concluído. Resultado: ${output_file}"
  else
    log_error "${name} falhou."
    return 1
  fi
}

# -----------------------------------------------------------------------------
# Uso
# -----------------------------------------------------------------------------

usage() {
  cat <<EOF
Uso: $(basename "$0") [COMANDO] [OPÇÕES]

Comandos:
  smoke       Smoke test — valida que a API está respondendo (1 VU, 1 min)
  load        Load test  — carga normal de produção (~50 VUs, 10 min)
  stress      Stress test — encontra o ponto de ruptura (~200 VUs, 18 min)
  soak        Soak test  — carga sustentada para detectar memory leaks (20 VUs, 1h)
  spike       Spike test — pico repentino de tráfego (~500 VUs, 12 min)
  all         Executa smoke + load em sequência (recomendado para CI/CD)

Variáveis de ambiente:
  BASE_URL       URL base da API (padrão: http://localhost:5034)
  SOAK_DURATION  Duração do soak test   (padrão: 1h, ex.: 30m para CI)
  K6_NON_INTERACTIVE       Aborta prompts em automação (true/false)
  K6_CONFIRM_DESTRUCTIVE   Libera stress/spike em automação (true/false)

Exemplos:
  $(basename "$0") smoke
  BASE_URL=https://staging.meusite.com $(basename "$0") load
  SOAK_DURATION=30m $(basename "$0") soak
  K6_NON_INTERACTIVE=true K6_CONFIRM_DESTRUCTIVE=true $(basename "$0") stress
  $(basename "$0") all

EOF
}

# -----------------------------------------------------------------------------
# Main
# -----------------------------------------------------------------------------

check_k6
mkdir_results

COMMAND="${1:-}"

case "$COMMAND" in
  smoke)
    check_api
    run_test "smoke" "smoke.js"
    ;;
  load)
    check_api
    run_test "load" "load.js"
    ;;
  stress)
    check_api
    log_warn "O stress test pode causar degradação do serviço."
    log_warn "Execute APENAS em ambiente de staging/homologação."
    confirm_destructive_test "stress"
    run_test "stress" "stress.js"
    ;;
  soak)
    check_api
    log_info "Duração do soak: ${SOAK_DURATION:-1h}"
    run_test "soak" "soak.js"
    ;;
  spike)
    check_api
    log_warn "O spike test envia até 500 VUs simultâneos."
    log_warn "Execute APENAS em ambiente de staging/homologação."
    confirm_destructive_test "spike"
    run_test "spike" "spike.js"
    ;;
  all)
    check_api
    log_info "Executando smoke + load em sequência..."
    run_test "smoke" "smoke.js"
    run_test "load"  "load.js"
    log_success "Todos os testes concluídos."
    ;;
  ""|help|--help|-h)
    usage
    ;;
  *)
    log_error "Comando desconhecido: ${COMMAND}"
    usage
    exit 1
    ;;
esac
