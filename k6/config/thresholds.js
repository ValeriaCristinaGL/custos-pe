// ─── Thresholds compartilhados por cenário ────────────────────────────────────

/**
 * Thresholds conservadores para smoke test.
 * Apenas valida que o sistema está em pé.
 */
export const smokeThresholds = {
  http_req_failed: [{ threshold: "rate<0.01", abortOnFail: true }],
  http_req_duration: ["p(95)<3000"],
  error_rate: [{ threshold: "rate<0.01", abortOnFail: true }],
};

/**
 * Thresholds para load test (carga normal esperada).
 * SLA: 95% das requisições abaixo de 1,5s; menos de 1% de erros.
 */
export const loadThresholds = {
  http_req_failed: [{ threshold: "rate<0.01", abortOnFail: false }],
  http_req_duration: ["p(50)<500", "p(90)<1000", "p(95)<1500", "p(99)<2000"],
  error_rate: ["rate<0.01"],
  dashboard_resumo_duration: ["p(95)<1500"],
  dashboard_comparativo_duration: ["p(95)<1500"],
  dashboard_evolucao_duration: ["p(95)<1500"],
  pesquisa_global_duration: ["p(95)<2000"],
  exportar_csv_duration: ["p(95)<5000"],
};

/**
 * Thresholds para stress test (carga acima do normal).
 * Mais permissivos — detectar degradação, não reprovar.
 */
export const stressThresholds = {
  http_req_failed: ["rate<0.05"],
  http_req_duration: ["p(95)<3000", "p(99)<5000"],
  error_rate: ["rate<0.05"],
};

/**
 * Thresholds para soak test (carga sustentada por longo período).
 * Detecta vazamentos de memória e degradação gradual.
 */
export const soakThresholds = {
  http_req_failed: ["rate<0.01"],
  http_req_duration: ["p(95)<2000"],
  error_rate: ["rate<0.01"],
};

/**
 * Thresholds para spike test (pico repentino de carga).
 * Valida recuperação do sistema após o pico.
 */
export const spikeThresholds = {
  http_req_failed: ["rate<0.10"],
  http_req_duration: ["p(95)<5000"],
  error_rate: ["rate<0.10"],
};
