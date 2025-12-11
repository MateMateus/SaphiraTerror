# 🩸 Saphira Terror

> **Versão 3.0** — Catálogo e Gerenciamento de Filmes de Terror.

O **SaphiraTerror** é uma aplicação web robusta desenvolvida em **.NET 9**, focada na exibição e gestão de um catálogo de filmes de terror. O projeto foi construído seguindo os princípios de **Clean Architecture**, **SOLID** e **DDD (Domain-Driven Design)** para garantir escalabilidade e manutenibilidade.

-----

## 🏛 Arquitetura do Projeto

A solução foi desenhada para separar responsabilidades e facilitar a evolução independente de cada módulo.

A solução é composta por 5 camadas principais:

| Projeto | Responsabilidade |
| :--- | :--- |
| **1. Domain** | Núcleo do sistema. Contém as entidades (`Filme`, `Genero`, `Usuario`) e regras de negócio puras. Sem dependências externas. |
| **2. Application** | Casos de uso, DTOs, interfaces de Repositórios/Serviços e orquestração de regras de negócio. |
| **3. Infrastructure** | Implementação de acesso a dados (EF Core), Migrations, Seeds e serviços externos (Identity). |
| **4. Api** | Endpoints RESTful, documentação Swagger e controle de autenticação/autorização (JWT/Identity). |
| **5. Web** | Interface do usuário (MVC), Dashboard Administrativo, gráficos e consumo da API via `Fetch`. |

-----

## 🚀 Tecnologias Utilizadas

  * **Backend:** .NET 9 (C\#)
  * **ORM:** Entity Framework Core (SQL Server)
  * **Auth:** ASP.NET Core Identity
  * **Frontend:** ASP.NET MVC, Razor Views, Bootstrap 5.3 (Dark Theme)
  * **Scripts:** JavaScript (Fetch API), Chart.js (Dashboard)
  * **Ferramentas:** Swagger/OpenAPI

-----

## ⚙️ Configuração e Execução

Siga os passos abaixo para rodar o projeto em sua máquina local.

### Pré-requisitos

  * [.NET SDK 9.0](https://dotnet.microsoft.com/download)
  * SQL Server (LocalDB ou instância dedicada)

### 1\. Clonar e Restaurar

```bash
git clone https://github.com/seu-usuario/SaphiraTerror.git
cd SaphiraTerror
dotnet restore
```

### 2\. Configurar Banco de Dados

O projeto utiliza **Migrations** e um **Seeder** automático. Certifique-se de que a Connection String no `appsettings.Development.json` (na API e na Web) aponta para o seu servidor SQL.

Aplique as migrations e popule o banco:

```bash
# Na raiz da solução
dotnet tool install --global dotnet-ef --version 9.* # Se não tiver instalado
dotnet ef database update -p SaphiraTerror.Infrastructure -s SaphiraTerror.Api
```

### 3\. Rodar a Aplicação

Você precisará rodar a **API** e o **Web** simultaneamente (ou configurar a solução para múltiplos startups no Visual Studio).

**Terminal 1 (Backend - API):**

```bash
dotnet run --project SaphiraTerror.Api
# A API rodará em: http://localhost:5203 (Swagger disponível)
```

**Terminal 2 (Frontend - Web):**

```bash
dotnet run --project SaphiraTerror.Web
# O Site rodará em: https://localhost:5001 ou http://localhost:5000
```

-----

## 👥 Perfis de Acesso (Login)

O `DatabaseSeeder` cria automaticamente usuários padrão para testes:

| Perfil | Email | Senha Padrão | Permissões |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@saphira.local` | `Admin@123` | Acesso total (Dashboard, CRUDs, Exclusão). |
| **Gerente** | `gerente@saphira.local`| `Gerente@123`| Gestão de conteúdo (Sem permissão de exclusão). |
| **Usuário** | `usuario@saphira.local`| `Usuario@123`| Visualização do catálogo e filtros. |

-----

## 🗺 Roadmap de Desenvolvimento

O projeto foi construído em 8 fases incrementais:

  - [x] **Fase 0:** Scaffolding da solução e camadas.
  - [x] **Fase 1:** Domínio e Infraestrutura (EF Core, Migrations, Seeds).
  - [x] **Fase 2:** Application (Interfaces, Services e DTOs).
  - [x] **Fase 3:** API (Endpoints REST, Swagger, CORS).
  - [x] **Fase 4:** Web - Catálogo (Consumo da API, Filtros e Paginação).
  - [x] **Fase 5:** Autenticação e Autorização (Identity, Cookies, Login).
  - [x] **Fase 6:** Dashboard Administrativo (Gráficos Chart.js).
  - [x] **Fase 7:** CRUDs completos (Filmes, Gêneros, Classificações, Usuários).
  - [x] **Fase 8:** Upload de Imagens e Refinamentos de UX.

-----

## 📂 Estrutura de Pastas

```bash
SaphiraTerror/
├── SaphiraTerror.Api/            # Entry point do Backend
├── SaphiraTerror.Application/    # Regras de Negócio e Interfaces
├── SaphiraTerror.Domain/         # Entidades Principais
├── SaphiraTerror.Infrastructure/ # Banco de Dados e Implementações
├── SaphiraTerror.Web/            # Frontend MVC
└── SaphiraTerror.sln             # Arquivo da Solução
```

-----

**Saphira Terror** © 2025 - Desenvolvido para fins educacionais.

-----

### Gostaria que eu criasse também um arquivo `CONTRIBUTING.md` com as regras para abrir Pull Requests no repositório, ou prefere ajuda para implementar o código da "Fase 0"?
