# GoodHamburger

Sistema de pedidos de uma hamburgueria, desenvolvido como desafio técnico. Permite montar pedidos com produtos de diferentes categorias, aplicando descontos automáticos conforme a combinação escolhida.

---

## Sumário

- [🛠 Tecnologias](#-tecnologias)
- [🏗 Decisões de Arquitetura](#-decisões-de-arquitetura)
- [📋 Regras de Negócio](#-regras-de-negócio)
- [🚀 Como Executar](#-como-executar)
  - [🐳 Docker (recomendado)](#-docker-recomendado)
  - [💻 Local](#-local)
- [📡 Endpoints da API](#-endpoints-da-api)
- [🧪 Testes](#-testes)
- [💡 Melhorias Não Implementadas](#-melhorias-não-implementadas)

---

## 🛠 Tecnologias

| Camada | Tecnologia |
|--------|-----------|
| Backend | ASP.NET Core 10 (Minimal API) |
| Frontend | Blazor WebAssembly 10 + MudBlazor 9 |
| ORM | Entity Framework Core 10 |
| Banco de dados | SQL Server 2022 |
| Validação | FluentValidation 12 |
| Documentação | Scalar (OpenAPI) |
| Testes | xUnit + Moq |
| Infraestrutura | Docker + Docker Compose |

---

## 🏗 Decisões de Arquitetura

### Clean Architecture em camadas

```
Web (Blazor) ──► Api ──► Core ◄── Data
```

- **Core** — entidades, regras de negócio, interfaces e exceções. Sem dependências externas.
- **Data** — repositórios, DbContext, Unit of Work e migrations.
- **Api** — endpoints Minimal API, DTOs, validação de entrada e middleware de exceções.
- **Web** — Blazor WebAssembly que consome a API via HTTP.

### Minimal API

Endpoints organizados em classes estáticas com método `Handle`, mantendo cada operação isolada.

### Repository + Unit of Work

Acesso a dados abstraído pelo padrão Repository. O `UnitOfWork` garante atomicidade em operações com múltiplas escritas — por exemplo, adicionar um item e recalcular os totais do pedido numa única transação.

### Validação com FluentValidation

Requisições de entrada são validadas via `ValidationEndpointFilter<T>`, um `IEndpointFilter` genérico aplicado aos endpoints de escrita. Os validadores ficam em `Api/Validators/` e retornam `400 Bad Request` com detalhes dos erros antes de o handler ser executado.

### Tratamento centralizado de exceções

O `ExceptionHandlerMiddleware` intercepta exceções de domínio e as mapeia para respostas HTTP padronizadas — eliminando try/catch dos handlers:

| Exceção | HTTP |
|---------|------|
| `EntityNotFoundException` | 404 |
| `DuplicateItemException` | 400 |
| `BusinessRuleViolationException` | 400 |
| `InvalidItemQuantityException` | 400 |
| `JsonException` / `BadHttpRequestException` | 400 |

### Progressão de status

```
Pending → Confirmed → Preparing → Ready → Completed
                (qualquer estado não-terminal) → Cancelled
```

Pedidos `Completed` ou `Cancelled` são terminais. Exclusão física só é permitida para pedidos `Cancelled`.

### Desconto calculado no servidor

O `DiscountCalculatorService` (Core) garante que a lógica de desconto não seja bypassável pelo cliente. O total é recalculado sempre que itens são adicionados, removidos ou atualizados.

### Blazor — carrinho no lado do cliente

O `CartService` mantém o estado do carrinho em memória. Ao inicializar, busca pedidos com status `Pending` e os reidrata localmente. A regra de uma categoria por pedido é validada no servidor (`DuplicateItemException`) e também no cliente, antes de fazer a requisição.

---

## 📋 Regras de Negócio

### Descontos automáticos

| Combinação | Desconto |
|------------|----------|
| Sanduíche + Acompanhamento + Bebida | 20% |
| Sanduíche + Bebida | 15% |
| Sanduíche + Acompanhamento | 10% |
| Qualquer outra | 0% |

### Restrições de pedido

- Máximo de **um produto por categoria** por pedido.
- Quantidade de cada item sempre **exatamente 1**.
- Somente produtos **ativos** podem ser adicionados.
- Preço unitário capturado no momento da adição — imune a alterações futuras no produto.

---

## 🚀 Como Executar

### 🐳 Docker (recomendado)

**Pré-requisito:** [Docker Desktop](https://www.docker.com/products/docker-desktop/) ou Docker Engine + Compose plugin.

```bash
docker compose up --build
```

O Compose sobe três serviços automaticamente:

| Serviço | Descrição | URL |
|---------|-----------|-----|
| `sqlserver` | SQL Server 2022 | `localhost:1433` |
| `api` | ASP.NET Core Minimal API | `http://localhost:5198` |
| `web` | Blazor WASM via nginx | `http://localhost` |

> As migrations são aplicadas automaticamente na inicialização da API.

**URLs de acesso:**

| O quê | URL |
|-------|-----|
| Frontend | `http://localhost` |
| Documentação da API (Scalar) | `http://localhost:5198/api/docs` |

Para encerrar:

```bash
docker compose down
```

Para limpar também os volumes (banco de dados):

```bash
docker compose down -v
```

---

### 💻 Local

**Pré-requisitos:**
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server em `localhost,1433`

**1. Configure a connection string** em `GoodHamburger.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "GoodHamburgerConnection": "Server=localhost,1433;Database=GoodHamburger;User ID=sa;Password=SUA_SENHA;Trusted_Connection=False;TrustServerCertificate=True;"
  }
}
```

**2. Restaure os pacotes e aplique as migrations:**

```bash
dotnet restore
dotnet ef database update --project GoodHamburger.Data --startup-project GoodHamburger.Api
```

**3. Suba os projetos em terminais separados:**

```bash
# Terminal 1 — API
cd GoodHamburger.Api && dotnet run
```

```bash
# Terminal 2 — Frontend
cd GoodHamburger.Web && dotnet run
```

| O quê | URL |
|-------|-----|
| Frontend | `http://localhost:5012` |
| Documentação da API (Scalar) | `http://localhost:5198/api/docs` |

---

## 📡 Endpoints da API

### Pedidos `/api/orders`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/orders?pageNumber=1&pageSize=10` | Listar pedidos (paginado) |
| `GET` | `/api/orders/{id}` | Buscar por ID |
| `POST` | `/api/orders` | Criar pedido |
| `PUT` | `/api/orders/{id}` | Atualizar itens |
| `DELETE` | `/api/orders/{id}` | Excluir (apenas `Cancelled`) |
| `PATCH` | `/api/orders/{id}/confirm` | Confirmar pedido `Pending` |
| `PATCH` | `/api/orders/{id}/cancel` | Cancelar pedido |
| `PATCH` | `/api/orders/{id}/status` | Avançar status |

### Itens de pedido `/api/order-items`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/orders/{orderId}/items` | Listar itens de um pedido |
| `POST` | `/api/order-items` | Adicionar item |
| `DELETE` | `/api/order-items/{id}` | Remover item |

### Produtos `/api/products`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/products?pageNumber=1&pageSize=10` | Listar produtos (paginado) |

### Categorias `/api/product-categories`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/product-categories` | Listar categorias |

---

## 🧪 Testes

```bash
dotnet test
```

Cobertura unitária em `GoodHamburger.Core.Tests`: `DiscountCalculatorService`, `OrderService`, `OrderItemService`, `ProductService`, `ProductCategoryService`.

---

## 💡 Melhorias Não Implementadas

### 🔐 Autenticação e Autorização

Sem mecanismo de identidade. Em produção seria necessário separar perfis (cliente × operador) com **ASP.NET Core Identity** + JWT ou solução externa (Keycloak, Auth0).

### 🧩 Testes de Integração

Os testes atuais são unitários com mocks. Testes de integração com banco real (**Testcontainers** + SQL Server efêmero) dariam mais confiança nas queries EF Core, migrations e constraints de banco.

### 📦 Gestão de Produtos e Categorias

Não há opções para cadastrar, editar ou desativar produtos e categorias — isso exige acesso ao banco.

### 🖼 Upload de Imagens

O campo `ImageUrl` armazena uma URL externa. Uma solução completa teria upload para bucket (S3, Azure Blob) com geração da URL após o upload.

### 📝 Logs Estruturados

O projeto usa o logging padrão do ASP.NET Core. Em produção, **Serilog** com saída JSON e sink para plataforma centralizada (Elastic, Seq, Datadog) facilitaria rastreio e correlação de requisições.

### 🔔 Notificações em Tempo Real

Mudanças de status exigem recarga manual da página. **SignalR** permitiria que o frontend recebesse atualizações sem polling.

### ⚡ Cache de Respostas

Endpoints de leitura estável (produtos, categorias) se beneficiariam de **IMemoryCache** ou Redis para reduzir carga no banco.

### ⚙️ Pipeline de CI/CD

Sem automação de build/testes/deploy. Um workflow no **GitHub Actions** rodando `dotnet build` e `dotnet test` a cada pull request evitaria regressões silenciosas.

### 🩺 Health Checks

Um endpoint `/health` com `Microsoft.Extensions.Diagnostics.HealthChecks` verificando conectividade com o banco permitiria que orquestradores (como o próprio Docker Compose) detectem instâncias degradadas e reiniciem automaticamente.

---

Feito com ☕ por Ronald.
