import Card from '../../components/Card'

export default function Comparacao() {
  return (
    <div>
      <div className="grid grid-cols-7 gap-8">
        <Card
          titulo="Evolucao Comparativa"
          descricao="Despesas por orgao ao longo dos anos (em milhoes R$)"
          className="col-span-4"
        ></Card>
        <Card
          titulo="Perfil de Gastos"
          descricao="Analise multidimensional dos orgaos selecionados"
          className="col-span-3"
        ></Card>
      </div>
      <div className="flex gap-8 mt-8">
        <Card titulo="SEE" descricao="Sec. Educação" quantidade="R$"></Card>
        <Card titulo="SES" descricao="Sec. Saúde" quantidade="R$"></Card>
        <Card
          titulo="SDS"
          descricao="Sec. Defesa Social"
          quantidade="R$"
        ></Card>
      </div>
      <Card
        titulo="Detalhamento por Ógão"
        descricao="Execução orçamentária detalhada por secretaria"
        className="mt-8"
      ></Card>
    </div>
  )
}
