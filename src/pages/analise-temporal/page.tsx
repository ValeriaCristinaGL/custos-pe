import Card from '../../components/Card'

export default function AnaliseTemporal() {
  return (
    <div className="flex flex-col gap-8">
      <div className="flex gap-8">
        <Card titulo="Crescimento anual médio"></Card>
        <Card titulo="Pico sazonal"></Card>
        <Card titulo="Tendência 2026"></Card>
      </div>
      <div className="flex gap-8">
        <Card
          titulo="Despesas Trimestrais"
          descricao="Evolução trimestral dos últimos 2 anos (em milhoes R$)"
        ></Card>
        <Card
          titulo="Crescimento anual"
          descricao="Taxa de crescimento da despesa total por ano"
        ></Card>
      </div>
      <Card
        titulo="Índice de sazonalidade"
        descricao="Padrão de gastos ao longo do ao (índice 100 = média mensal)"
      ></Card>
      <Card
        titulo="Comparativo mensal por ano"
        descricao="Despesas mensais dos últimos 2 anos (em milhões R$)"
      ></Card>
    </div>
  )
}
