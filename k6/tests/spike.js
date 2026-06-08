/**
 * Spike Test — Transparência PE
 *
 * Objetivo: Simular um pico repentino e extremo de tráfego, como ocorre quando:
 *   - Uma notícia divulga o portal para um grande público
 *   - Um relatório ou publicação oficial gera acesso massivo
 *   - Horário de pico coincide com publicação de dados novos
 *
 * O teste valida se o sistema consegue:
 *   1. Absorver o pico sem travar completamente
 *   2. Se recuperar após o pico voltar à normalidade
 *
 * Duração total: ~12 minutos
 * VUs máximos: 500 (pico)
 */

import http from "k6/http";
import { sleep } from "k6";
import { spikeThresholds } from "../config/thresholds.js";
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
    { duration: "1m", target: 10 }, // Tráfego normal antes do pico
    { duration: "30s", target: 500 }, // ⚡ Pico repentino
    { duration: "2m", target: 500 }, // Sustentar o pico
    { duration: "30s", target: 10 }, // Queda rápida
    { duration: "3m", target: 10 }, // Período de recuperação
    { duration: "1m", target: 100 }, // Segundo pico menor
    { duration: "30s", target: 10 }, // Volta ao normal
    { duration: "1m", target: 0 }, // Encerramento
  ],
  thresholds: spikeThresholds,
  tags: { test_type: "spike" },
};

export default function () {
  const ano = randomItem(ANOS_DISPONIVEIS);
  const codigoOrgao = randomItem(CODIGOS_ORGAO);
  const termo = randomItem(TERMOS_PESQUISA);

  // Durante um pico, a maioria dos usuários acessa a página principal
  const roll = Math.random();

  if (roll < 0.5) {
    // 50% — Resumo (página inicial do dashboard — mais acessada)
    const res = http.get(`${BASE_URL}/api/v1/dashboard/resumo?ano=${ano}`, {
      headers: DEFAULT_HEADERS,
      tags: { endpoint: "dashboard_resumo" },
    });
    checkResponse(res, "Dashboard Resumo");
    dashboardResumoDuration.add(res.timings.duration);
  } else if (roll < 0.75) {
    // 25% — Comparativo entre órgãos
    const res = http.get(
      `${BASE_URL}/api/v1/dashboard/comparativo?ano=${ano}`,
      { headers: DEFAULT_HEADERS, tags: { endpoint: "dashboard_comparativo" } },
    );
    checkResponse(res, "Dashboard Comparativo");
    dashboardComparativoDuration.add(res.timings.duration);
  } else if (roll < 0.9) {
    // 15% — Pesquisa por termos populares
    const res = http.get(
      `${BASE_URL}/api/v1/pesquisa/global?termo=${encodeURIComponent(termo)}`,
      { headers: DEFAULT_HEADERS, tags: { endpoint: "pesquisa_global" } },
    );
    checkResponse(res, "Pesquisa Global");
    pesquisaGlobalDuration.add(res.timings.duration);
  } else {
    // 10% — Drill-down em órgão
    const res = http.get(
      `${BASE_URL}/api/v1/dashboard/evolucao?codigoOrgao=${codigoOrgao}&ano=${ano}`,
      { headers: DEFAULT_HEADERS, tags: { endpoint: "dashboard_evolucao" } },
    );
    checkResponse(res, "Dashboard Evolução");
  }

  // Pausa mínima — durante pico usuários fazem requisições rapidamente
  sleep(0.3);
}
