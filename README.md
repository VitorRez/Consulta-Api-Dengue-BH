# Desafio Técnico — Dengue em Belo Horizonte

Aplicação full-stack que consulta dados de alertas de dengue da API [AlertaDengue](https://info.dengue.mat.br/) para a cidade de Belo Horizonte, persiste em SQL Server e exibe os dados em uma interface web.

---

## Sumário

- [Visão Geral](#-visão-geral)
- [Arquitetura](#-arquitetura)
- [Tecnologias](#-tecnologias)
- [Pré-requisitos](#-pré-requisitos)
- [Variáveis de Ambiente](#-variáveis-de-ambiente)
- [Como Rodar](#-como-rodar)
- [Endpoints da API](#-endpoints-da-api)
- [Testes](#-testes)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Decisões Técnicas](#-decisões-técnicas)

---

## Visão Geral

A aplicação resolve três problemas:

1. **Consulta e persistência**: importa os dados de dengue dos últimos 6 meses para Belo Horizonte (geocode `3106200`) a partir da API AlertaDengue e persiste em SQL Server.
2. **Consulta por semana**: expõe um endpoint HTTP que retorna os dados de uma semana epidemiológica específica.
3. **Visualização**: aplicação web que exibe as últimas 3 semanas em cards, tabela e gráfico de barras, além de destacar a semana de maior e menor incidência de todo o histórico.

---

## Arquitetura

O backend segue **Clean Architecture** em 4 camadas, com dependências apontando sempre para o centro (Domain):

```
┌──────────────────────────────────────────────┐
│                   API                        │  Controllers, Program.cs
│  ┌────────────────────────────────────────┐  │
│  │           Application                  │  │  Services, DTOs, Interfaces
│  │  ┌──────────────────────────────────┐  │  │
│  │  │            Domain                │  │  │  Entidades (DengueAlert)
│  │  └──────────────────────────────────┘  │  │
│  │           ↑                            │  │
│  │           │                            │  │
│  │  ┌────────┴─────────────────────────┐  │  │
│  │  │         Infrastructure           │  │  │  DbContext, Repositórios,
│  │  │                                  │  │  │  Cliente HTTP, Migrations
│  │  └──────────────────────────────────┘  │  │
│  └────────────────────────────────────────┘  │
└──────────────────────────────────────────────┘
```

- **Domain**: entidade `DengueAlert` — C# puro, sem dependência de frameworks.
- **Application**: regras de orquestração (`DengueService`), DTOs e interfaces.
- **Infrastructure**: detalhes técnicos — EF Core (SQL Server), cliente HTTP da API AlertaDengue, repositório.
- **API**: camada de apresentação — controllers, configuração de DI, CORS, Swagger.

### Fluxo de dados

```
[Frontend React] ──HTTP──▶ [DengueController]
                                  │
                                  ▼
                           [DengueService]
                            │           │
                            ▼           ▼
                   [DengueRepository] [AlertaDengueClient]
                            │           │
                            ▼           ▼
                        [SQL Server] [API AlertaDengue]
```

---

## Tecnologias

### Backend
- **.NET 10** com C#
- **ASP.NET Core** (Web API)
- **Entity Framework Core 10** com SQL Server
- **xUnit** + **Moq** para testes
- **DotNetEnv** para carregar variáveis de ambiente
- **Swashbuckle** (Swagger)

### Frontend
- **React 19** (com Vite)
- **JavaScript** (ES2023+)
- **Tailwind CSS v4**
- **Recharts** para gráficos
- **Axios** para requisições HTTP

### Infra
- **Docker** + **Docker Compose** para o SQL Server

---

## Pré-requisitos

| Ferramenta | Versão mínima | Como verificar |
|---|---|---|
| .NET SDK | 10.0 | `dotnet --version` |
| Node.js | 20.19 ou 22.12+ | `node --version` |
| npm | 10 | `npm --version` |
| Docker | 24 | `docker --version` |
| Docker Compose | v2 | `docker compose version` |
| dotnet-ef (CLI) | 10 | `dotnet ef --version` |

### Instalando o `dotnet-ef` (caso não tenha)

```bash
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"  # adicione ao ~/.bashrc para persistir
```

---

## Variáveis de Ambiente

O projeto usa dois arquivos `.env`, ambos fora do versionamento (cobertos pelo `.gitignore`):

### `.env` (raiz do projeto)

Lido pelo **Docker Compose** e pelo **backend .NET** (via `DotNetEnv`).

| Variável | Descrição |
|---|---|
| `MSSQL_SA_PASSWORD` | Senha do usuário `sa` do SQL Server |
| `CONNECTION_STRING` | Connection string completa usada pelo backend |

### `frontend/.env`

Lido pelo **Vite** em tempo de build.

| Variável | Descrição |
|---|---|
| `VITE_API_URL` | URL base do backend (`http://localhost:5096`) |

Cada arquivo tem um `.example` versionado no Git, que serve de template.

---

## Como Rodar

### 1. Configurar variáveis de ambiente

O projeto centraliza as credenciais do banco num arquivo `.env` na raiz. Ele é lido por:

- **Docker Compose**, para criar o container do SQL Server com a senha correta.
- **Backend .NET**, via `DotNetEnv`, para montar a connection string.

Copie o template e ajuste se necessário:

```bash
cp .env.example .env
```

O conteúdo esperado:

```
MSSQL_SA_PASSWORD=SuaSenhaForte123
CONNECTION_STRING=Server=localhost,1433;Database=DengueDb;User Id=sa;Password=SuaSenhaForte123;TrustServerCertificate=True;
```

> A senha em `MSSQL_SA_PASSWORD` **precisa ser igual** à `Password=` do `CONNECTION_STRING`.

> O arquivo `.env` está no `.gitignore` e nunca deve ser commitado.

### 2. Subir o SQL Server

```bash
docker compose up -d
docker compose logs -f sqlserver
```

Aguarde a mensagem `SQL Server is now ready for client connections`. Aperte `Ctrl+C` para sair dos logs.

Confirme que está rodando:

```bash
docker compose ps
```

### 3. Aplicar as migrations

Da raiz do projeto:

```bash
dotnet ef database update \
  -p backend/src/Dengue.Infrastructure/Dengue.Infrastructure.csproj \
  -s backend/src/Dengue.Api/Dengue.Api.csproj
```

Isso cria o banco `DengueDb` e a tabela `DengueAlerts`.

### 4. Rodar o backend

```bash
dotnet run --project backend/src/Dengue.Api/Dengue.Api.csproj
```

A API sobe em `http://localhost:5096`. No primeiro startup, o `DengueSyncHostedService` detecta que o banco está vazio e importa os últimos 6 meses de dados da API AlertaDengue automaticamente.

Log esperado:

```
[ENV] Carregado: /caminho/para/.env
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5096
info: Dengue.Api.DengueSyncHostedService[0]
      Banco vazio. Iniciando sync de startup...
info: Dengue.Api.DengueSyncHostedService[0]
      Sync de startup concluído: 26 registros.
```

Acesse o Swagger: **http://localhost:5096/swagger**

### 5. Rodar o frontend

Em outro terminal:

```bash
cd frontend
npm install
```

Crie o `frontend/.env`:

```bash
cp .env.example .env
```

Confirme o conteúdo:

```
VITE_API_URL=http://localhost:5096
```

Suba:

```bash
npm run dev
```

Acesse **http://localhost:5173**.

---

## Endpoints da API

### `GET /api/dengue?ew={ew}&ey={ey}`

Retorna os dados de uma semana epidemiológica específica.

| Parâmetro | Tipo | Descrição |
|---|---|---|
| `ew` | int | Semana epidemiológica (1-53) |
| `ey` | int | Ano epidemiológico |

**Exemplo:**

```bash
curl "http://localhost:5096/api/dengue?ew=36&ey=2026"
```

**Resposta 200:**

```json
{
  "semana_epidemiologica": "2026-36",
  "data_inicio_semana": "2026-09-06T00:00:00Z",
  "casos_est": 722,
  "casos_notificados": 37,
  "nivel_alerta": 3
}
```

**Resposta 404:** quando não há dados para a semana solicitada.

```json
{
  "message": "Sem dados para a semana 2026-37."
}
```

---

### `GET /api/dengue/extremos`

Retorna a semana com **maior nível de alerta** e a com **menor nível de alerta** entre todos os dados armazenados.

**Exemplo:**

```bash
curl "http://localhost:5096/api/dengue/extremos"
```

**Resposta 200:**

```json
{
  "maior_nivel": {
    "semana_epidemiologica": "2026-12",
    "data_inicio_semana": "2026-03-22T00:00:00Z",
    "casos_est": 1307,
    "casos_notificados": 1307,
    "nivel_alerta": 4
  },
  "menor_nivel": {
    "semana_epidemiologica": "2026-34",
    "data_inicio_semana": "2026-08-23T00:00:00Z",
    "casos_est": 503,
    "casos_notificados": 236,
    "nivel_alerta": 1
  }
}
```

---

### `POST /api/dengue/sync`

Força uma sincronização manual dos últimos 6 meses a partir da API AlertaDengue. Útil para atualizar os dados sem esperar o agendamento semanal.

**Exemplo:**

```bash
curl -X POST "http://localhost:5096/api/dengue/sync"
```

**Resposta 200:**

```json
{
  "message": "Sincronização concluída.",
  "records_processed": 26
}
```

---

## Testes

Para rodar os testes do backend:

```bash
dotnet test
```

Ou, especificamente o projeto:

```bash
dotnet test backend/tests/Dengue.Tests/Dengue.Tests.csproj
```

### Cobertura atual

| Teste | O que verifica |
|---|---|
| `SyncLastSixMonthsAsync_ShouldReturnZero_WhenApiReturnsEmpty` | Retorna 0 quando a API externa devolve lista vazia |
| `SyncLastSixMonthsAsync_ShouldCallUpsert_WhenApiReturnsData` | Chama o repositório com a lista correta de entidades |
| `SyncLastSixMonthsAsync_ShouldMapSeToEwAndEy_Correctly` | O parsing de `SE=202636` para `Ew=36, Ey=2026` |
| `GetByWeekAsync_ShouldReturnNull_WhenRepositoryReturnsNull` | Retorna `null` quando não há dados para a semana |
| `GetByWeekAsync_ShouldReturnMappedDto_WhenRepositoryReturnsEntity` | Mapeia corretamente entidade para DTO |
| `GetExtremosAsync_ShouldReturnNull_WhenBothRepositoriesReturnNull` | Retorna `null` quando o banco está vazio |
| `GetExtremosAsync_ShouldReturnBothExtremes_WhenDataExists` | Retorna o maior e o menor nível corretamente |

---

## Estrutura do Projeto

```
Teste Técnico/
├── .env                        # variáveis de ambiente (não versionado)
├── .env.example                # template versionado
├── .gitignore
├── README.md
├── docker-compose.yml          # SQL Server
├── backend/
│   ├── Dengue.slnx
│   ├── src/
│   │   ├── Dengue.Domain/           # Entidades
│   │   ├── Dengue.Application/      # Services, DTOs, Interfaces
│   │   ├── Dengue.Infrastructure/   # DbContext, Repositórios, Cliente HTTP
│   │   └── Dengue.Api/
│   │       ├── Program.cs           # carrega .env via DotNetEnv
│   │       ├── DengueSyncHostedService.cs
│   │       ├── appsettings.json
│   │       ├── appsettings.Example.json
│   │       └── Controllers/
│   └── tests/
│       └── Dengue.Tests/            # Testes unitários
└── frontend/
    ├── .env                    # não versionado
    ├── .env.example
    ├── package.json
    └── src/
        ├── api/                # Comunicação com o backend
        ├── components/         # CardSemana, CardDestaque, Tabela, Gráfico
        ├── styles/             # Classes Tailwind reutilizáveis
        └── utils/              # Cálculo de semana ISO, formatação de data
```

---