@echo off
REM ============================================================
REM  run-tests.bat — Executa testes automatizados + gera cobertura
REM  Uso: clique duplo ou execute no terminal: run-tests.bat
REM ============================================================
setlocal EnableDelayedExpansion

set SCRIPT_DIR=%~dp0
set RESULTS_DIR=%SCRIPT_DIR%TestResults
set REPORT_DIR=%RESULTS_DIR%\CoverageReport
set SOLUTION_FILE=%SCRIPT_DIR%TransparenciaPE.sln

echo.
echo ============================================================
echo   TransparenciaPE -- Testes Automatizados e Cobertura
echo ============================================================
echo.

REM ── 1. Verificar .NET ──────────────────────────────────────
where dotnet >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERRO] 'dotnet' nao encontrado. Instale o .NET SDK:
    echo        https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
for /f "tokens=*" %%v in ('dotnet --version') do echo [OK] .NET SDK: %%v

REM ── 2. Verificar / instalar reportgenerator ────────────────
where reportgenerator >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [INFO] 'reportgenerator' nao encontrado. Instalando...
    dotnet tool install --global dotnet-reportgenerator-globaltool
    if %ERRORLEVEL% NEQ 0 (
        echo [ERRO] Falha ao instalar reportgenerator.
        pause
        exit /b 1
    )
)
echo [OK] reportgenerator instalado

REM ── 3. Limpar resultados anteriores ────────────────────────
echo.
echo [INFO] Limpando resultados anteriores...
if exist "%RESULTS_DIR%" rmdir /s /q "%RESULTS_DIR%"

REM ── 4. Executar testes com coleta de cobertura ─────────────
echo.
echo [INFO] Executando testes unitarios e de integracao...
echo ------------------------------------------------------------
dotnet test "%SOLUTION_FILE%" ^
  --collect:"XPlat Code Coverage" ^
  --settings "%SCRIPT_DIR%coverlet.runsettings" ^
  --results-directory "%RESULTS_DIR%"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERRO] Um ou mais testes falharam. Verifique a saida acima.
    pause
    exit /b 1
)
echo ------------------------------------------------------------

REM ── 5. Gerar relatório HTML ─────────────────────────────────
echo.
echo [INFO] Gerando relatorio de cobertura...
reportgenerator ^
  -reports:"%RESULTS_DIR%\*\coverage.opencover.xml" ^
  -targetdir:"%REPORT_DIR%" ^
  -reporttypes:"Html;TextSummary" ^
  -assemblyfilters:"-*Tests*;-*Migrations*"

REM ── 6. Exibir resumo no terminal ───────────────────────────
echo.
echo ============================================================
echo   RESUMO DE COBERTURA
echo ============================================================
type "%REPORT_DIR%\Summary.txt"

REM ── 7. Abrir relatório no navegador ────────────────────────
echo.
echo [INFO] Abrindo relatorio no navegador...
start "" "%REPORT_DIR%\index.html"

echo.
echo [CONCLUIDO]
echo.
pause
