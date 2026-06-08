# 13 — Motor de Conversão

## Conversão MVP

```txt
DOCX -> PDF
```

## Ferramenta

LibreOffice headless dentro do container Docker do backend.

## Fluxo técnico

1. worker baixa DOCX do Supabase Storage para diretório temporário;
2. LibreOffice converte para PDF;
3. worker valida se PDF foi criado;
4. worker envia PDF para Supabase Storage;
5. worker limpa arquivos temporários.

## Diretórios temporários

Usar diretório temporário do container:

```txt
/tmp/convertly/{conversionJobId}
```

Arquivos temporários devem ser removidos após sucesso ou falha.

## Interface

```csharp
public interface IFileConverter
{
    bool CanConvert(string sourceFormat, string targetFormat);
    Task<ConversionResult> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken);
}
```

## Implementação

```txt
DocxToPdfConverter
```

## Limitações

A conversão pode não preservar 100%:

- fontes;
- espaçamento;
- imagens;
- quebras de página.

Isso é aceitável no MVP.
