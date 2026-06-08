import { check } from "k6";
import { Rate, Trend, Counter } from "k6/metrics";

// ─── Métricas customizadas ────────────────────────────────────────────────────

export const errorRate = new Rate("error_rate");
export const dashboardResumoDuration = new Trend(
  "dashboard_resumo_duration",
  true,
);
export const dashboardComparativoDuration = new Trend(
  "dashboard_comparativo_duration",
  true,
);
export const dashboardEvolucaoDuration = new Trend(
  "dashboard_evolucao_duration",
  true,
);
export const pesquisaGlobalDuration = new Trend(
  "pesquisa_global_duration",
  true,
);
export const exportarCsvDuration = new Trend("exportar_csv_duration", true);
export const totalRequests = new Counter("total_requests");

// ─── URL base ────────────────────────────────────────────────────────────────

export const BASE_URL = __ENV.BASE_URL || "http://localhost:5034";

// ─── Headers padrão ──────────────────────────────────────────────────────────

export const DEFAULT_HEADERS = {
  "Content-Type": "application/json",
  Accept: "application/json",
};

// ─── Dados de exemplo ─────────────────────────────────────────────────────────

export const ANOS_DISPONIVEIS = [2021, 2022, 2023, 2024, 2025];

export const CODIGOS_ORGAO = [
  "050101",
  "070101",
  "080101",
  "090101",
  "110101",
  "120101",
  "130101",
  "150101",
  "160101",
  "170101",
];

export const TERMOS_PESQUISA = [
  "Empresa",
  "Construção",
  "Serviços",
  "Consultoria",
  "Manutenção",
  "Fornecimento",
  "Engenharia",
  "Tecnologia",
];

// ─── Helpers ─────────────────────────────────────────────────────────────────

/**
 * Retorna um elemento aleatório de um array.
 */
export function randomItem(arr) {
  return arr[Math.floor(Math.random() * arr.length)];
}

/**
 * Verifica resposta HTTP e registra métricas.
 */
export function checkResponse(res, name, expectedStatus = 200) {
  totalRequests.add(1);

  const success = check(res, {
    [`${name} - status ${expectedStatus}`]: (r) => r.status === expectedStatus,
    [`${name} - tempo < 2s`]: (r) => r.timings.duration < 2000,
    [`${name} - body não vazio`]: (r) => r.body && r.body.length > 0,
  });

  errorRate.add(!success);
  return success;
}

/**
 * Verifica resposta CSV.
 */
export function checkCsvResponse(res) {
  totalRequests.add(1);

  const success = check(res, {
    "exportar CSV - status 200": (r) => r.status === 200,
    "exportar CSV - content-type text/csv": (r) =>
      r.headers["Content-Type"] &&
      r.headers["Content-Type"].includes("text/csv"),
    "exportar CSV - tempo < 5s": (r) => r.timings.duration < 5000,
    "exportar CSV - body não vazio": (r) => r.body && r.body.length > 0,
  });

  errorRate.add(!success);
  return success;
}
