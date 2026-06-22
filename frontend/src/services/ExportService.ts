import jsPDF from 'jspdf'
import autoTable from 'jspdf-autotable'
import ExcelJS from 'exceljs'
import { saveAs } from 'file-saver'

export interface ExportColumn {
  header: string
  key: string
  width?: number
  isCurrency?: boolean
  isPercentage?: boolean
}

export interface ExportTable {
  title: string
  columns: ExportColumn[]
  data: Record<string, any>[]
  imageBase64?: string
}

export async function exportToPdf(filename: string, tables: ExportTable[]) {
  const doc = new jsPDF()

  tables.forEach((table, index) => {
    if (index > 0) {
      doc.addPage()
    }

    doc.setFontSize(16)
    doc.text(table.title, 14, 15)

    const head = [table.columns.map((col) => col.header)]
    const body = table.data.map((row) =>
      table.columns.map((col) => {
        const val = row[col.key]
        if (col.isCurrency) {
          const num = typeof val === 'number' ? val : Number(val) || 0
          return num.toLocaleString('pt-BR', {
            style: 'currency',
            currency: 'BRL',
          })
        }
        if (col.isPercentage) {
          return `${val}%`
        }
        return val ?? '-'
      })
    )

    autoTable(doc, {
      startY: 25,
      head: head,
      body: body,
      theme: 'striped',
      headStyles: { fillColor: '#142F4B', textColor: '#FFFFFF' },
      styles: { fontSize: 10, cellPadding: 4 },
      alternateRowStyles: { fillColor: '#F8FAFC' },
    })

    if (table.imageBase64) {
      const finalY = (doc as any).lastAutoTable.finalY || 25
      const pageHeight = doc.internal.pageSize.getHeight()
      const imgWidth = 180
      const imgHeight = 80

      if (finalY + 10 + imgHeight > pageHeight - 15) {
        doc.addPage()
        doc.addImage(table.imageBase64, 'PNG', 14, 20, imgWidth, imgHeight)
      } else {
        doc.addImage(table.imageBase64, 'PNG', 14, finalY + 10, imgWidth, imgHeight)
      }
    }
  })

  doc.save(`${filename}.pdf`)
}

export async function exportToExcel(filename: string, tables: ExportTable[]) {
  const workbook = new ExcelJS.Workbook()
  workbook.creator = 'Portal Custos PE'

  tables.forEach((table) => {
    let sheetName = table.title.replace(/[\\/?*[\]]/g, '').slice(0, 31)

    let attempt = 1
    let originalName = sheetName
    while (workbook.getWorksheet(sheetName)) {
      sheetName = `${originalName.slice(0, 28)}_${attempt}`
      attempt++
    }

    const worksheet = workbook.addWorksheet(sheetName)

    worksheet.columns = table.columns.map((col) => ({
      header: col.header,
      key: col.key,
      width: col.width || 20,
      style: {
        numFmt: col.isCurrency ? '"R$" #,##0.00' : undefined,
      },
    }))

    worksheet.addRows(table.data)

    const headerRow = worksheet.getRow(1)
    headerRow.height = 24
    headerRow.eachCell((cell) => {
      cell.fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'FF142F4B' },
      }
      cell.font = {
        color: { argb: 'FFFFFFFF' },
        bold: true,
        size: 12,
      }
      cell.alignment = { vertical: 'middle', horizontal: 'center' }
    })

    worksheet.views = [{ state: 'frozen', ySplit: 1 }]

    if (table.imageBase64) {
      const imageId = workbook.addImage({
        base64: table.imageBase64,
        extension: 'png',
      })
      
      const finalRow = table.data.length + 3
      worksheet.addImage(imageId, {
        tl: { col: 0, row: finalRow },
        ext: { width: 800, height: 400 },
      })
    }
  })

  const buffer = await workbook.xlsx.writeBuffer()
  const blob = new Blob([buffer], {
    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  })
  saveAs(blob, `${filename}.xlsx`)
}
