# 🚀 Como Rodar o Projeto Biblioteca Online

## ⚠️ IMPORTANTE: Pré-requisitos

**Você PRECISA instalar o .NET 8.0 SDK primeiro!**

### 1. Instalar .NET 8.0 SDK
- Acesse: https://dotnet.microsoft.com/download/dotnet/8.0
- Baixe o **.NET 8.0 SDK** (não o Runtime)
- Execute o instalador
- **Reinicie o terminal/PowerShell** após a instalação

### 2. Verificar Instalação
```bash
dotnet --version
```
Deve mostrar algo como: `8.0.xxx`

## 🏃‍♂️ Passos para Rodar

### Passo 1: Navegar para a pasta do projeto
```bash
cd BibliotecaOnline
```

### Passo 2: Restaurar dependências
```bash
dotnet restore
```

### Passo 3: Executar o projeto
```bash
dotnet run
```

### Passo 4: Acessar a aplicação
- **API**: https://localhost:7001
- **Swagger UI**: https://localhost:7001/swagger

## 🗄️ Configuração do Banco de Dados

### Opção 1: SQL Server LocalDB (Recomendado para desenvolvimento)
1. Instale o SQL Server LocalDB (vem com Visual Studio)
2. Ou instale o SQL Server Express

### Opção 2: SQL Server Express
1. Baixe SQL Server Express
2. Configure a connection string em `appsettings.json`

### Opção 3: Docker (Avançado)
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourPassword123" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

## 🔧 Solução de Problemas

### Erro: "No .NET SDKs were found"
- ✅ Instale o .NET 8.0 SDK
- ✅ Reinicie o terminal
- ✅ Execute `dotnet --version`

### Erro: "Connection string not found"
- ✅ Verifique se `appsettings.json` existe
- ✅ Confirme a connection string

### Erro: "SQL Server connection failed"
- ✅ Verifique se SQL Server está rodando
- ✅ Confirme a connection string
- ✅ Teste a conexão

### Erro: "Build failed"
- ✅ Execute `dotnet clean`
- ✅ Execute `dotnet restore`
- ✅ Execute `dotnet build`

## 📱 Testando a API

### Via Swagger UI
1. Acesse: https://localhost:7001/swagger
2. Teste os endpoints disponíveis

### Via Postman/Insomnia
- Base URL: `https://localhost:7001`
- Endpoints:
  - `GET /api/livro` - Lista livros
  - `POST /api/livro` - Cria livro
  - `GET /api/livro/{id}` - Obtém livro específico

### Via cURL
```bash
# Listar livros
curl -X GET "https://localhost:7001/api/livro"

# Criar livro
curl -X POST "https://localhost:7001/api/livro" \
  -H "Content-Type: application/json" \
  -d '{"titulo":"Dom Casmurro","anoPublicacao":1899,"autorId":1}'
```

## 🎯 Próximos Passos

1. ✅ Projeto configurado
2. ✅ Estrutura organizada
3. ✅ Dependências definidas
4. 🔄 Instalar .NET SDK
5. 🔄 Configurar banco de dados
6. 🔄 Executar projeto
7. 🔄 Testar API

## 📞 Suporte

Se encontrar problemas:
1. Verifique se o .NET SDK está instalado
2. Confirme se SQL Server está rodando
3. Verifique os logs de erro
4. Consulte a documentação oficial do .NET
