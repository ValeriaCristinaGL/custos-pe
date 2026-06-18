/**
 * Smoke Test — Transparência PE
 *
 * Objetivo: Verificar que todos os endpoints respondem corretamente
 * com carga mínima (1 VU). Deve ser executado antes de qualquer outro
 * teste para confirmar que o ambiente está saudável.
 *
 * Duração: ~1 minuto
 * VUs: 1
 */

import http from "k6/http";
import { sleep } from "k6";
import { smokeThresholds } from "../config/thresholds.js";
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
  vus: 1,
  duration: "1m",
  thresholds: smokeThresholds,
  tags: { test_type: "smoke" },
};

export default function () {
  const ano = randomItem(ANOS_DISPONIVEIS);
  const codigoOrgao = randomItem(CODIGOS_ORGAO);
  const termo = randomItem(TERMOS_PESQUISA);

  // ── 1. Dashboard Resumo ──────────────────────────────────────────────────
  {
    const res = http.get(`${BASE_URL}/api/v1/dashboard/resumo?ano=${ano}`, {
      headers: DEFAULT_HEADERS,
      tags: { endpoint: "dashboard_resumo" },
    });
    checkResponse(res, "Dashboard Resumo");
    dashboardResumoDuration.add(res.timings.duration);
    sleep(0.5);
  }

  // ── 2. Dashboard Resumo sem filtro de ano ────────────────────────────────
  {
    const res = http.get(`${BASE_URL}/api/v1/dashboard/resumo`, {
      headers: DEFAULT_HEADERS,
      tags: { endpoint: "dashboard_resumo_sem_ano" },
    });
    checkResponse(res, "Dashboard Resumo (sem ano)");
    dashboardResumoDuration.add(res.timings.duration);
    sleep(0.5);
  }

  // ── 3. Dashboard Comparativo ─────────────────────────────────────────────
  {
    const res = http.get(
      `${BASE_URL}/api/v1/dashboard/comparativo?ano=${ano}`,
      { headers: DEFAULT_HEADERS, tags: { endpoint: "dashboard_comparativo" } },
    );
    checkResponse(res, "Dashboard Comparativo");
    dashboardComparativoDuration.add(res.timings.duration);
    sleep(0.5);
  }

  // ── 4. Dashboard Evolução (Drill-down) ────────────────────────────────────
  {
    const res = http.get(
      `${BASE_URL}/api/v1/dashboard/evolucao?codigoOrgao=${codigoOrgao}&ano=${ano}`,
      { headers: DEFAULT_HEADERS, tags: { endpoint: "dashboard_evolucao" } },
    );
    checkResponse(res, "Dashboard Evolução");
    dashboardEvolucaoDuration.add(res.timings.duration);
    sleep(0.5);
  }

  // ── 5. Pesquisa Global ────────────────────────────────────────────────────
  {
    const res = http.get(
      `${BASE_URL}/api/v1/pesquisa/global?termo=${encodeURIComponent(termo)}`,
      { headers: DEFAULT_HEADERS, tags: { endpoint: "pesquisa_global" } },
    );
    checkResponse(res, "Pesquisa Global");
    pesquisaGlobalDuration.add(res.timings.duration);
    sleep(0.5);
  }

  // ── 6. Exportar CSV ───────────────────────────────────────────────────────
  {
    const res = http.get(
      `${BASE_URL}/api/v1/exportar/csv?termo=${encodeURIComponent(termo)}&ano=${ano}`,
      {
        headers: { ...DEFAULT_HEADERS, Accept: "text/csv" },
        tags: { endpoint: "exportar_csv" },
      },
    );
    checkCsvResponse(res);
    exportarCsvDuration.add(res.timings.duration);
    sleep(1);
  }
}
