# GoodHamburger

Sistema de pedidos de uma hamburgueria, desenvolvido como desafio técnico. Permite montar pedidos com produtos de diferentes categorias, aplicando descontos automáticos conforme a combinação escolhida.

---

## Sumário

- [Tecnologias](#tecnologias)
- [Decisões de Arquitetura](#decisões-de-arquitetura)
- [Regras de Negócio](#regras-de-negócio)
- [Pré-requisitos](#pré-requisitos)
- [Configuração](#configuração)
- [Como Executar](#como-executar)
- [Endpoints da API](#endpoints-da-api)
- [Testes](#testes)

---

## Tecnologias

| Camada | Tecnologia |
|--------|-----------|
| Backend | ASP.NET Core 10 (Minimal API) |
| Frontend | Blazor WebAssembly 10 + MudBlazor 9 |
| ORM | Entity Framework Core 10 |
| Banco de dados | SQL Server |
| Documentação | Scalar (OpenAPI) |
| Testes | xUnit + Moq | 

---

## Decisões de Arquitetura

### Clean Architecture em camadas

O projeto está dividido em quatro camadas com dependências unidirecionais:

```
Web (Blazor) ──► Api ──► Core ◄── Data
```

- **Core** — entidades de domínio, regras de negócio, interfaces e exceções. Não depende de nenhuma camada externa.
- **Data** — implementações de repositório, DbContext (EF Core), Unit of Work e migrations. Depende somente do Core.
- **Api** — endpoints Minimal API, DTOs de request/response, injeção de dependências e tratamento global de exceções. Depende do Core e Data.
- **Web** — aplicação Blazor WebAssembly que consome a API via HTTP. Depende somente dos contratos da Api.

### Minimal API

Os endpoints são organizados em classes estáticas com método `Handle`, mantendo a responsabilidade de cada operação isolada e testável sem necessidade de instanciar um controller.

```
Api/Endpoints/
  OrderEndpoints/
    GetOrders.cs   GetOrder.cs   PostOrder.cs   UpdateOrder.cs
    DeleteOrder.cs CancelOrder.cs ConfirmOrder.cs UpdateOrderStatus.cs
  OrderItemEndpoints/ ...
  ProductEndpoints/ ...
```

### Repository + Unit of Work

O acesso a dados é abstraído pelo padrão Repository, permitindo substituição da implementação sem afetar a camada de negócio. O `UnitOfWork` garante atomicidade em operações que envolvem múltiplas escritas (ex.: adicionar item e recalcular totais do pedido).

### Exceções de domínio tipadas

Erros de regra de negócio são expressos via exceções específicas, capturadas pelo middleware global e mapeadas para respostas HTTP padronizadas:

| Exceção | Situação |
|---------|----------|
| `DuplicateItemException` | Produto da mesma categoria já está no pedido |
| `InvalidItemQuantityException` | Quantidade diferente de 1 |
| `EntityNotFoundException` | Entidade não encontrada por ID |
| `BusinessRuleViolationException` | Demais violações de regra |

### Progressão de status

O status do pedido segue uma máquina de estados linear com transições apenas para frente:

```
Pending → Confirmed → Preparing → Ready → Completed
                                         ↗
                   (qualquer estado não-terminal) → Cancelled
```

Pedidos `Completed` ou `Cancelled` são terminais — nenhuma alteração é permitida. A exclusão física só é permitida para pedidos `Cancelled`.

### Desconto calculado no servidor

O cálculo de desconto reside no `DiscountCalculatorService` (Core), garantindo que a lógica não seja bypassável pelo cliente. O total é recalculado sempre que itens são adicionados, removidos ou atualizados.

### Blazor — carrinho no lado do cliente

O `CartService` mantém o estado do carrinho em memória durante a sessão. Ao inicializar, busca pedidos com status `Pending` e os reidrata localmente. Adições são sincronizadas com a API em tempo real.

A regra de uma categoria por pedido é aplicada tanto no servidor (lança `DuplicateItemException`) quanto no cliente (o `CartService` bloqueia antes de fazer a requisição), proporcionando feedback instantâneo ao usuário.

---

## Regras de Negócio

### Descontos automáticos

| Combinação de categorias | Desconto |
|--------------------------|----------|
| Sanduíche + Acompanhamento + Bebida | 20% |
| Sanduíche + Bebida | 15% |
| Sanduíche + Acompanhamento | 10% |
| Qualquer outra combinação | 0% |

O desconto é calculado sobre o subtotal do pedido.

### Restrições de pedido

- Cada pedido pode conter no máximo **um produto por categoria**.
- A quantidade de cada item é sempre **exatamente 1**.
- Somente produtos **ativos** podem ser adicionados.
- O preço unitário é capturado no momento da adição e não muda com alterações futuras no produto.

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server acessível em `localhost,1433`
    - Usuário: `sa`
    - Senha: configurada em `appsettings.Development.json`

---

## Configuração

Edite `GoodHamburger.Api/appsettings.Development.json` com a string de conexão do seu ambiente:

```json
{
  "ConnectionStrings": {
    "GoodHamburgerConnection": "Server=localhost,1433;Database=GoodHamburger;User ID=sa;Password=SUA_SENHA;Trusted_Connection=False;TrustServerCertificate=True;"
  }
}
```

---

## Como Executar

### 1. Banco de dados

Execute para a instalação dos pacotes necessários:
```bash
dotnet restore
```


Aplique as migrations para criar o schema e popular os dados iniciais (categorias e produtos):

```bash
dotnet ef database update --project GoodHamburger.Data --startup-project GoodHamburger.Api
```

### 2. API

```bash
cd GoodHamburger.Api
dotnet run
```

A API sobe em `http://localhost:5198`.  
Documentação interativa disponível em `http://localhost:5198/api-docs`.

### 3. Aplicação Web

Em outro terminal:

```bash
cd GoodHamburger.Web
dotnet run
```

A aplicação Blazor sobe em `http://localhost:5012`.

---

## Endpoints da API

### Pedidos `/api/orders`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/orders` | Listar todos os pedidos |
| `GET` | `/api/orders/{id}` | Buscar pedido por ID |
| `POST` | `/api/orders` | Criar pedido |
| `PUT` | `/api/orders/{id}` | Atualizar itens do pedido |
| `DELETE` | `/api/orders/{id}` | Excluir pedido (apenas `Cancelled`) |
| `PATCH` | `/api/orders/{id}/confirm` | Confirmar pedido `Pending` |
| `PATCH` | `/api/orders/{id}/cancel` | Cancelar pedido |
| `PATCH` | `/api/orders/{id}/status` | Avançar status na progressão |

### Itens de pedido `/api/order-items`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/order-items` | Listar todos os itens |
| `GET` | `/api/order-items/{id}` | Buscar item por ID |
| `POST` | `/api/order-items` | Adicionar item a um pedido |
| `PUT` | `/api/order-items/{id}` | Atualizar item |
| `DELETE` | `/api/order-items/{id}` | Remover item |

### Produtos `/api/products`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/products` | Listar produtos ativos |
| `GET` | `/api/products/{id}` | Buscar produto por ID |

### Categorias `/api/product-categories`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/product-categories` | Listar categorias |
| `GET` | `/api/product-categories/{id}` | Buscar categoria por ID |

---

## Testes

```bash
dotnet test
```

| Projeto | Cobertura |
|---------|-----------|
| `GoodHamburger.Core.Tests` | `DiscountCalculatorService`, `OrderService`, `OrderItemService`, `ProductService`, `ProductCategoryService` |

---

Feito com ☕ por Ronald.
