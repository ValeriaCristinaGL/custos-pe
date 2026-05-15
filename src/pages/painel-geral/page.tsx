import { TrendingUp } from 'lucide-react'
import Card from '../../components/Card'

export default function PainelGeral() {
  return (
    <div>
      <div className="flex gap-8">
        <Card
          titulo="Despesa Total"
          icone={<TrendingUp className="h-4 w-h-4" />}
          quantidade="R$"
          comparacao="+ 7,5% vs ano anterior"
        />
        <Card
          titulo="Receita Total"
          icone={<TrendingUp className="h-4 w-h-4" />}
          quantidade="R$"
          comparacao="+ 7,5% vs ano anterior"
        />
        <Card
          titulo="Investimentos"
          icone={<TrendingUp className="h-4 w-h-4" />}
          quantidade="R$"
          comparacao="- 7,5% vs ano anterior"
        />
      </div>
      <div className="grid grid-cols-7 gap-8 mt-8">
        <Card
          titulo="Evolução mensal de despesas"
          descricao="Comparativo do ano atual (em milhões R$)"
          className="col-span-4"
        ></Card>
        <Card
          titulo="Distribuição por categtorias"
          descricao="Composição das despesas no ano atual"
          className="col-span-3"
        ></Card>
        <Card
          titulo="Maiores Orgaos por Despesa"
          descricao="Top 10 órgão com maior volume de despesas no ano atual"
          className="col-span-7"
        ></Card>
      </div>
    </div>
  )
}
