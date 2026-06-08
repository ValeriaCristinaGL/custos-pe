/**
 * Stress Test — Transparência PE
 *
 * Objetivo: Identificar o ponto de ruptura (breaking point) do sistema.
 * Aumenta a carga progressivamente até o sistema começar a falhar.
 * Permite descobrir o limite máximo de VUs que a API suporta.
 *
 * ⚠️  ATENÇÃO: Este teste é destrutivo por natureza.
 *     Execute apenas em ambiente de staging/homologação,
 *     nunca em produção.
 *
 * Duração total: ~18 minutos
 * VUs máximos: 200
 */

import http from "k6/http";
import { sleep } from "k6";
import { stressThresholds } from "../config/thresholds.js";
import {
  BASE_URL,
  DEFAULT_HEADERS,
  ANOS_DISPONIVEIS,
  CODIGOS_ORGAO,
  TERMOS_PESQUISA,
  randomItem,
  checkResponse,
  dashboardResumoDuration,
  dashboardComparativoDuration,
  pesquisaGlobalDuration,
} from "../utils/helpers.js";

export const options = {
  stages: [
    { duration: "2m", target: 20 }, // Aquecimento
    { duration: "2m", target: 50 }, // Carga normal
    { duration: "2m", target: 80 }, // Acima do normal
    { duration: "2m", target: 100 }, // Alto estresse
    { duration: "2m", target: 150 }, // Estresse extremo
    { duration: "2m", target: 200 }, // Ponto de ruptura
    { duration: "2m", target: 200 }, // Manter pico
    { duration: "2m", target: 0 }, // Recuperação
  ],
  thresholds: stressThresholds,
  tags: { test_type: "stress" },
};

export default function () {
  const ano = randomItem(ANOS_DISPONIVEIS);
  const codigoOrgao = randomItem(CODIGOS_ORGAO);
  const termo = randomItem(TERMOS_PESQUISA);

  // Distribui as requisições entre os endpoints mais críticos
  const roll = Math.random();

  if (roll < 0.4) {
    // 40% — Dashboard Resumo (endpoint mais acessado)
    const res = http.get(`${BASE_URL}/api/v1/dashboard/resumo?ano=${ano}`, {
      headers: DEFAULT_HEADERS,
      tags: { endpoint: "dashboard_resumo" },
    });
    checkResponse(res, "Dashboard Resumo");
    dashboardResumoDuration.add(res.timings.duration);
  } else if (roll < 0.7) {
    // 30% — Dashboard Comparativo
    const res = http.get(
      `${BASE_URL}/api/v1/dashboard/comparativo?ano=${ano}`,
      { headers: DEFAULT_HEADERS, tags: { endpoint: "dashboard_comparativo" } },
    );
    checkResponse(res, "Dashboard Comparativo");
    dashboardComparativoDuration.add(res.timings.duration);
  } else if (roll < 0.9) {
    // 20% — Dashboard Evolução
    const res = http.get(
      `${BASE_URL}/api/v1/dashboard/evolucao?codigoOrgao=${codigoOrgao}&ano=${ano}`,
      { headers: DEFAULT_HEADERS, tags: { endpoint: "dashboard_evolucao" } },
    );
    checkResponse(res, "Dashboard Evolução");
  } else {
    // 10% — Pesquisa Global
    const res = http.get(
      `${BASE_URL}/api/v1/pesquisa/global?termo=${encodeURIComponent(termo)}`,
      { headers: DEFAULT_HEADERS, tags: { endpoint: "pesquisa_global" } },
    );
    checkResponse(res, "Pesquisa Global");
    pesquisaGlobalDuration.add(res.timings.duration);
  }

  sleep(0.5); // Pausa mínima — simula tráfego intenso
}
