/**
 * Soak Test (Endurance Test) — Transparência PE
 *
 * Objetivo: Detectar problemas que surgem apenas com uso prolongado:
 *   - Vazamento de memória (memory leaks)
 *   - Degradação gradual de performance
 *   - Esgotamento de conexões com o banco de dados
 *   - Acúmulo de logs ou arquivos temporários
 *
 * Estratégia: Carga moderada mantida por longo período (padrão: 1 hora).
 * Ajuste a variável de ambiente SOAK_DURATION para alterar a duração.
 *
 * Duração padrão: 1 hora
 * VUs: 20 (carga moderada e sustentável)
 *
 * Uso:
 *   k6 run k6/tests/soak.js
 *   k6 run -e SOAK_DURATION=30m k6/tests/soak.js   # Versão mais curta para CI
 */

import http from "k6/http";
import { sleep } from "k6";
import { soakThresholds } from "../config/thresholds.js";
import {
  BASE_URL,
  DEFAULT_HEADERS,
  ANOS_DISPONIVEIS,
  CODIGOS_ORGAO,
  TERMOS_PESQUISA,
  randomItem,
  checkResponse,
  checkCsvResponse,
  dashboardResumoDuration,
  dashboardComparativoDuration,
  dashboardEvolucaoDuration,
  pesquisaGlobalDuration,
  exportarCsvDuration,
} from "../utils/helpers.js";

const SOAK_DURATION = __ENV.SOAK_DURATION || "1h";

export const options = {
  stages: [
    { duration: "5m", target: 20 }, // Rampa de subida gradual
    { duration: SOAK_DURATION, target: 20 }, // Carga sustentada
    { duration: "5m", target: 0 }, // Rampa de descida
  ],
  thresholds: soakThresholds,
  tags: { test_type: "soak" },
};

export default function () {
  const ano = randomItem(ANOS_DISPONIVEIS);
  const codigoOrgao = randomItem(CODIGOS_ORGAO);
  const termo = randomItem(TERMOS_PESQUISA);

  // Ciclo completo de navegação — simula sessão real de usuário
  const roll = Math.random();

  if (roll < 0.3) {
    // Fluxo A: Dashboard completo (resumo → comparativo → evolução)
    const r1 = http.get(`${BASE_URL}/api/v1/dashboard/resumo?ano=${ano}`, {
      headers: DEFAULT_HEADERS,
      tags: { endpoint: "dashboard_resumo" },
    });
    checkResponse(r1, "Dashboard Resumo");
    dashboardResumoDuration.add(r1.timings.duration);
    sleep(randomPause(2, 4));

    const r2 = http.get(`${BASE_URL}/api/v1/dashboard/comparativo?ano=${ano}`, {
      headers: DEFAULT_HEADERS,
      tags: { endpoint: "dashboard_comparativo" },
    });
    checkResponse(r2, "Dashboard Comparativo");
    dashboardComparativoDuration.add(r2.timings.duration);
    sleep(randomPause(3, 6));

    const r3 = http.get(
      `${BASE_URL}/api/v1/dashboard/evolucao?codigoOrgao=${codigoOrgao}&ano=${ano}`,
      { headers: DEFAULT_HEADERS, tags: { endpoint: "dashboard_evolucao" } },
    );
    checkResponse(r3, "Dashboard Evolução");
    dashboardEvolucaoDuration.add(r3.timings.duration);
    sleep(randomPause(2, 5));
  } else if (roll < 0.7) {
    // Fluxo B: Pesquisa e navegação
    const r1 = http.get(
      `${BASE_URL}/api/v1/pesquisa/global?termo=${encodeURIComponent(termo)}`,
      { headers: DEFAULT_HEADERS, tags: { endpoint: "pesquisa_global" } },
    );
    checkResponse(r1, "Pesquisa Global");
    pesquisaGlobalDuration.add(r1.timings.duration);
    sleep(randomPause(3, 7));

    const r2 = http.get(`${BASE_URL}/api/v1/dashboard/resumo`, {
      headers: DEFAULT_HEADERS,
      tags: { endpoint: "dashboard_resumo" },
    });
    checkResponse(r2, "Dashboard Resumo");
    dashboardResumoDuration.add(r2.timings.duration);
    sleep(randomPause(2, 4));
  } else {
    // Fluxo C: Exportação de dados
    const r1 = http.get(`${BASE_URL}/api/v1/exportar/csv?ano=${ano}`, {
      headers: { ...DEFAULT_HEADERS, Accept: "text/csv" },
      tags: { endpoint: "exportar_csv" },
    });
    checkCsvResponse(r1);
    exportarCsvDuration.add(r1.timings.duration);
    sleep(randomPause(8, 15));
  }
}

function randomPause(min, max) {
  return Math.random() * (max - min) + min;
}
