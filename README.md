
# API de Finanças

[![Build e testes SonarCloud](https://github.com/dmrogger/Api-Financas/actions/workflows/sonar.yml/badge.svg)](https://github.com/dmrogger/Api-Financas/actions/workflows/sonar.yml)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-22-DD0031?logo=angular)

## Sobre o projeto

Este projeto nasceu de uma dor pessoal. Eu usava alguns serviços de gestão financeira que ofereciam apenas funções básicas e, para ter acesso a recursos mais completos, era preciso aderir a um plano mensal.

Diante disso, resolvi desenvolver minha própria API REST, integrada a um front-end, e aproveitar a oportunidade para aplicar e estudar novas ferramentas, como o SonarCloud, que roda na pipeline e avalia a qualidade de todo código que entra neste repositório.

Fique a vontade para bisbilhotar, em caso de dúvidas ou críticas é só chamar 

## Funcionalidades

- **Usuários**: cadastro, consulta, edição de nome/senha e exclusão da própria conta (com confirmação de senha)
- **Autenticação** via JWT (token válido por 2 horas), com senhas armazenadas em hash BCrypt
- **Movimentações**: registro de entradas e saídas, listagem com filtro por período e exclusão
- **Categorias**: 8 categorias padrão do sistema + categorias personalizadas por usuário
- **Resumo mensal**: total de entradas, saídas e saldo do mês, agrupado por categoria
- **Health check** em `/health` (inclui a conexão com o banco)
- **Swagger** com botão *Authorize* para testar os endpoints protegidos
- Tratamento global de exceções com respostas no padrão *Problem Details*

## Tecnologias

| Camada | Tecnologias |
|---|---|
| Back-end | C# · ASP.NET Core 8 · Entity Framework Core 8 · Npgsql |
| Banco de dados | PostgreSQL 16 |
| Segurança | JWT Bearer · BCrypt.Net |
| Documentação | Swagger (Swashbuckle) com comentários XML |
| Testes | xUnit · Moq · Coverlet |
| Qualidade / CI | GitHub Actions · SonarCloud |
| Front-end | Angular 22 · Angular Material · Vitest |
| Infra | Docker · Docker Compose |

## Arquitetura

O projeto segue uma organização em camadas:

```
ApiFinancas/Src
├── Domain           # Entidades, enums e interfaces de repositório (regras de negócio)
├── Application      # Services, DTOs (requests/responses) e o padrão Result<T>
├── Infrastructure   # EF Core (DbContext, configurações, migrations, seeds), repositórios e segurança (JWT, BCrypt)
└── Presentation     # Controllers, middlewares e extensões
Tests/               # Testes unitários dos controllers e do middleware de exceções
frontend/            # Aplicação Angular que consome a API
```

Os services retornam um `Result<T>` em vez de lançar exceções para erros de negócio; os controllers convertem o tipo de erro no status HTTP adequado (400, 403, 404 ou 409).

## Como executar

### Opção 1: Docker (recomendado)

Pré-requisito: [Docker](https://www.docker.com/) com Docker Compose.

```bash
git clone https://github.com/dmrogger/Api-Financas.git
cd Api-Financas

# Opcional: defina sua própria senha do banco e chave JWT
cp .env.example .env

docker compose up --build
```

A API sobe com o PostgreSQL e aplica as migrations automaticamente.

- Swagger: http://localhost:8080/swagger
- Health check: http://localhost:8080/health

### Opção 2: Localmente com o .NET SDK

Pré-requisitos: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) e um PostgreSQL rodando.

```bash
cd ApiFinancas

# Configurações sensíveis via User Secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=apifinancas;Username=postgres;Password=postgres"
dotnet user-secrets set "Jwt:Key" "uma-chave-secreta-com-pelo-menos-32-bytes"

# Cria/atualiza o banco
dotnet tool install --global dotnet-ef   # se ainda não tiver
dotnet ef database update

dotnet run --launch-profile http
```

A API fica disponível em http://localhost:5159 e o Swagger em http://localhost:5159/swagger.

> A aplicação não inicia se `Jwt:Key` estiver ausente ou tiver menos de 32 bytes.

### Front-end (Angular)

Com a API rodando localmente na porta 5159:

```bash
cd frontend
npm install
npm start
```

Acesse http://localhost:4200. As chamadas para `/api` são redirecionadas para a API pelo `proxy.conf.json`.

## Configuração

| Chave | Variável de ambiente | Descrição | Padrão |
|---|---|---|---|
| `ConnectionStrings:DefaultConnection` | `ConnectionStrings__DefaultConnection` | Conexão com o PostgreSQL | — |
| `Jwt:Key` | `Jwt__Key` | Chave de assinatura do token (mín. 32 bytes) | — |
| `Jwt:Issuer` | `Jwt__Issuer` | Emissor do token | `ApiFinancas` |
| `Jwt:Audience` | `Jwt__Audience` | Público do token | `ApiFinancasUsuario` |
| `Aplicacao:FusoHorario` | `Aplicacao__FusoHorario` | Fuso usado para definir início e fim do mês nos resumos | `America/Sao_Paulo` |
| `Database:AplicarMigracoes` | `Database__AplicarMigracoes` | Aplica as migrations pendentes ao iniciar | `false` (`true` no Docker) |

## Endpoints

Todas as rotas usam o prefixo `/api/v1`. As marcadas com 🔒 exigem o header `Authorization: Bearer {token}`.

### Autenticação e usuários

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/usuarios` | Cria um usuário |
| `POST` | `/auth/login` | Autentica e retorna o token JWT |
| `GET` | `/usuarios/me` 🔒 | Dados do usuário autenticado |
| `PATCH` | `/usuarios/me` 🔒 | Altera nome e/ou senha (exige a senha atual) |
| `DELETE` | `/usuarios/me` 🔒 | Exclui a conta (exige confirmação da senha) |

### Movimentações

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/movimentacoes` 🔒 | Registra uma entrada ou saída |
| `GET` | `/movimentacoes?dataInicial=&dataFinal=` 🔒 | Lista as movimentações, com filtro opcional por período |
| `DELETE` | `/movimentacoes/{id}` 🔒 | Exclui uma movimentação |

### Categorias

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/categorias` 🔒 | Lista as categorias padrão e as do usuário |
| `POST` | `/categorias` 🔒 | Cria uma categoria personalizada |
| `PUT` | `/categorias/{id}` 🔒 | Renomeia uma categoria personalizada |
| `DELETE` | `/categorias/{id}` 🔒 | Exclui uma categoria personalizada (as movimentações dela ficam sem categoria) |

Categorias padrão: Alimentação, Moradia, Transporte, Saúde, Educação, Lazer, Salário e Outros. Elas não podem ser alteradas nem excluídas.

### Resumos

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/resumos/mensal?ano=2026&mes=11` 🔒 | Entradas, saídas e saldo do mês, por categoria |

## Exemplos de uso

**1. Criar usuário**

```http
POST /api/v1/usuarios
Content-Type: application/json

{
  "nome": "Maria Silva",
  "email": "maria@email.com",
  "senha": "Senha@123"
}
```

A senha precisa ter de 8 a 64 caracteres, com ao menos 1 letra maiúscula e 1 caractere especial.

**2. Fazer login**

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "maria@email.com",
  "senha": "Senha@123"
}
```

```json
{
  "success": true,
  "error": null,
  "data": {
    "idUsuario": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nome": "Maria Silva",
    "email": "maria@email.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

**3. Registrar uma saída**

```http
POST /api/v1/movimentacoes
Authorization: Bearer {token}
Content-Type: application/json

{
  "valor": 150.90,
  "tipo": 2,
  "descricao": "Supermercado",
  "categoriaId": "{id-da-categoria}",
  "dataOperacao": "2026-11-10T14:30:00Z"
}
```

`tipo`: `1` = Entrada, `2` = Saída. O valor é sempre positivo; `dataOperacao` e `categoriaId` são opcionais (sem data, usa a data atual).

**4. Consultar o resumo do mês**

```http
GET /api/v1/resumos/mensal?ano=2026&mes=11
Authorization: Bearer {token}
```

```json
{
  "success": true,
  "error": null,
  "data": {
    "ano": 2026,
    "mes": 11,
    "totalEntradas": 5000.00,
    "totalSaidas": 1850.40,
    "saldo": 3149.60,
    "categorias": [
      { "categoriaId": "…", "nome": "Moradia", "totalEntradas": 0, "totalSaidas": 1200.00 },
      { "categoriaId": "…", "nome": "Alimentação", "totalEntradas": 0, "totalSaidas": 650.40 },
      { "categoriaId": "…", "nome": "Salário", "totalEntradas": 5000.00, "totalSaidas": 0 }
    ]
  }
}
```

### Formato das respostas

As respostas seguem o envelope `Result<T>`:

```json
{ "success": false, "error": "Mensagem descrevendo o problema", "data": null }
```

## Testes

```bash
dotnet test
```

Para gerar o relatório de cobertura (o mesmo usado no CI):

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

A cada push ou pull request para `main`, `master` ou `develop`, o GitHub Actions compila o projeto, roda os testes e envia a cobertura para o SonarCloud.

## Próximos passos

- [ ] Edição de movimentações (`PUT /movimentacoes/{id}`)
- [ ] Contas bancárias (entidades `Conta` e `TipoConta` já iniciadas no domínio)
- [ ] Paginação na listagem de movimentações

## Autor

**Roger** · [GitHub @dmrogger](https://github.com/dmrogger)
