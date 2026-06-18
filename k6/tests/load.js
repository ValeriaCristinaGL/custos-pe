/**
 * Load Test — Transparência PE
 *
 * Objetivo: Simular carga normal de uso do portal público.
 * Perfil de tráfego realista com rampa de subida, patamar estável e rampa de descida.
 *
 * Cenários simulados:
 *   - Usuários navegando no dashboard (80% do tráfego)
 *   - Usuários realizando pesquisas (15% do tráfego)
 *   - Usuários exportando dados CSV (5% do tráfego)
 *
 * Duração total: ~10 minutos
 * VUs máximos: 50
 */

import http from "k6/http";
import { sleep } from "k6";
import { loadThresholds } from "../config/thresholds.js";
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

export const options = {
  scenarios: {
    // Usuários navegando no dashboard (rampa gradual)
    dashboard_usuarios: {
      executor: "ramping-vus",
      startVUs: 0,
      stages: [
        { duration: "2m", target: 20 }, // rampa de subida
        { duration: "5m", target: 40 }, // carga estável
        { duration: "2m", target: 40 }, // manter pico
        { duration: "1m", target: 0 }, // rampa de descida
      ],
      exec: "navegarDashboard",
      tags: { scenario: "dashboard" },
    },

    // Usuários realizando pesquisas
    pesquisa_usuarios: {
      executor: "ramping-vus",
      startVUs: 0,
      stages: [
        { duration: "2m", target: 8 },
        { duration: "5m", target: 8 },
        { duration: "2m", target: 8 },
        { duration: "1m", target: 0 },
      ],
      exec: "realizarPesquisa",
      tags: { scenario: "pesquisa" },
    },

    // Usuários exportando CSV (menor frequência)
    exportacao_usuarios: {
      executor: "ramping-vus",
      startVUs: 0,
      stages: [
        { duration: "2m", target: 2 },
        { duration: "5m", target: 2 },
        { duration: "2m", target: 2 },
        { duration: "1m", target: 0 },
      ],
      exec: "exportarDados",
      tags: { scenario: "exportacao" },
    },
  },

  thresholds: loadThresholds,
};

// ─── Cenário: Navegação no Dashboard ─────────────────────────────────────────

export function navegarDashboard() {
  const ano = randomItem(ANOS_DISPONIVEIS);
  const codigoOrgao = randomItem(CODIGOS_ORGAO);

  // Página inicial: resumo geral
  const resumoRes = http.get(`${BASE_URL}/api/v1/dashboard/resumo?ano=${ano}`, {
    headers: DEFAULT_HEADERS,
    tags: { endpoint: "dashboard_resumo" },
  });
  checkResponse(resumoRes, "Dashboard Resumo");
  dashboardResumoDuration.add(resumoRes.timings.duration);
  sleep(randomPause(1, 3));

  // Visualiza comparativo entre órgãos
  const comparativoRes = http.get(
    `${BASE_URL}/api/v1/dashboard/comparativo?ano=${ano}`,
    { headers: DEFAULT_HEADERS, tags: { endpoint: "dashboard_comparativo" } },
  );
  checkResponse(comparativoRes, "Dashboard Comparativo");
  dashboardComparativoDuration.add(comparativoRes.timings.duration);
  sleep(randomPause(2, 5));

  // Drill-down em um órgão específico
  const evolucaoRes = http.get(
    `${BASE_URL}/api/v1/dashboard/evolucao?codigoOrgao=${codigoOrgao}&ano=${ano}`,
    { headers: DEFAULT_HEADERS, tags: { endpoint: "dashboard_evolucao" } },
  );
  checkResponse(evolucaoRes, "Dashboard Evolução");
  dashboardEvolucaoDuration.add(evolucaoRes.timings.duration);
  sleep(randomPause(1, 3));
}

// ─── Cenário: Pesquisa Global ─────────────────────────────────────────────────

export function realizarPesquisa() {
  const termo = randomItem(TERMOS_PESQUISA);
  const ano = randomItem(ANOS_DISPONIVEIS);

  // Pesquisa por termo
  const pesquisaRes = http.get(
    `${BASE_URL}/api/v1/pesquisa/global?termo=${encodeURIComponent(termo)}`,
    { headers: DEFAULT_HEADERS, tags: { endpoint: "pesquisa_global" } },
  );
  checkResponse(pesquisaRes, "Pesquisa Global");
  pesquisaGlobalDuration.add(pesquisaRes.timings.duration);
  sleep(randomPause(2, 6));

  // Após pesquisar, navega no dashboard para ver o contexto
  const resumoRes = http.get(`${BASE_URL}/api/v1/dashboard/resumo?ano=${ano}`, {
    headers: DEFAULT_HEADERS,
    tags: { endpoint: "dashboard_resumo_pos_pesquisa" },
  });
  checkResponse(resumoRes, "Dashboard Resumo (pós pesquisa)");
  dashboardResumoDuration.add(resumoRes.timings.duration);
  sleep(randomPause(1, 3));
}

// ─── Cenário: Exportação de dados ─────────────────────────────────────────────

export function exportarDados() {
  const termo = randomItem(TERMOS_PESQUISA);
  const ano = randomItem(ANOS_DISPONIVEIS);

  // Exporta CSV com filtros
  const csvRes = http.get(
    `${BASE_URL}/api/v1/exportar/csv?termo=${encodeURIComponent(termo)}&ano=${ano}`,
    {
      headers: { ...DEFAULT_HEADERS, Accept: "text/csv" },
      tags: { endpoint: "exportar_csv" },
    },
  );
  checkCsvResponse(csvRes);
  exportarCsvDuration.add(csvRes.timings.duration);
  sleep(randomPause(5, 15)); // Usuários demoram mais entre exportações
}

// ─── Utilitário ───────────────────────────────────────────────────────────────

function randomPause(min, max) {
  return Math.random() * (max - min) + min;
}
