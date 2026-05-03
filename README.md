# PeopleHub

Aplicação simples para cadastro e gerenciamento de pessoas, desenvolvida como etapa prática de processo seletivo. O foco do projeto é demonstrar organização de código, separação de responsabilidades, validações de negócio e uso da stack solicitada: ASP.NET MVC, Entity Framework, SQL Server e Knockout.js.

<img width="1897" height="949" alt="image" src="https://github.com/user-attachments/assets/9c809952-a79e-4069-bbd9-a66146f2cf05" />

## Funcionalidades

- Cadastro, edição, listagem e exclusão de pessoas.
- Busca por nome, e-mail ou CPF.
- Paginação no backend.
- Soft delete através do campo `Archived`.
- Validação de e-mail obrigatório e único.
- Validação de CPF obrigatório e único.
- Validação de idade mínima de 18 anos.
- Interface com Knockout.js consumindo endpoints JSON.
- SQL Server via Docker Compose.
- Testes automatizados para controller, service e repository.

## Stack

- ASP.NET Core MVC
- Entity Framework Core
- Microsoft SQL Server
- Knockout.js
- jQuery
- Bootstrap/Tailwind no front-end
- xUnit, Moq e FluentAssertions nos testes
- Docker Compose para banco local

## Arquitetura

A solução foi organizada em camadas inspiradas em Clean Architecture:

```text
PeopleHub.Domain
  People/
    Person.cs

PeopleHub.Application
  People/
    DTOs/
    Repositories/
    Results/
    Services/

PeopleHub.Infrastructure
  Data/
    AppDbContext.cs
    DbInitializer.cs
  Migrations/
  Repositories/

PeopleHub.Web
  Controllers/
  Views/
  wwwroot/

PeopleHub.Tests
```

### Responsabilidades

`PeopleHub.Domain` contém a entidade `Person` e representa o núcleo de negócio. Essa camada não depende de Entity Framework, MVC ou detalhes externos.

`PeopleHub.Application` contém contratos, DTOs, serviços de aplicação e resultados tipados. É onde ficam as regras do caso de uso, como idade mínima, unicidade de CPF/e-mail e soft delete.

`PeopleHub.Infrastructure` contém detalhes técnicos: `AppDbContext`, migrations, seed e implementação concreta do repositório com Entity Framework.

`PeopleHub.Web` contém apenas a entrada da aplicação: controllers, views MVC, assets estáticos e configuração HTTP. O controller traduz requisições e respostas HTTP, mas não concentra regra de negócio.

## Como Executar

Suba o SQL Server:

```bash
docker compose up -d
```

Configure as variáveis no arquivo `.env` com base em `.env.example`. O arquivo `.env.example` deve ser versionado como referência de configuração; o `.env` real fica fora do Git para evitar exposição de credenciais locais.

Rode a aplicação:

```bash
dotnet run --project PeopleHub.Web
```

Ao iniciar, a aplicação executa automaticamente as migrations pendentes e realiza o seed inicial de dados, desde que o SQL Server esteja disponível.

A tela principal fica em:

```text
/People
```

Em ambiente de desenvolvimento, a documentação Swagger também fica disponível.

## Testes

Para executar a suíte:

```bash
dotnet test
```

Atualmente os testes cobrem:

- Regras de negócio no `PersonService`.
- Respostas principais do `PeopleController`.
- Filtro de registros arquivados e soft delete no `PersonRepository`.

## Decisões Técnicas

O projeto usa DTOs para entrada de dados, evitando acoplamento direto entre contrato externo e entidade persistida. O serviço retorna `PersonOperationResult`, um resultado tipado com status explícito para sucesso, validação, conflito e não encontrado. Isso evita decisões baseadas em texto de mensagem e deixa o fluxo mais previsível.

As restrições de unicidade de CPF e e-mail foram aplicadas também no banco via índices únicos. A validação em memória melhora a experiência do usuário, mas a constraint no banco protege a integridade dos dados.

O soft delete foi escolhido para preservar histórico e evitar remoção física imediata. A listagem padrão ignora registros arquivados.

As migrations ficam na camada de infraestrutura porque são detalhes de persistência. A aplicação Web apenas registra o `AppDbContext`, executa migrations e inicia o seed.

Os arquivos iniciais do template MVC, como `HomeController`, `Views/Home` e tela de erro padrão, foram removidos porque não faziam parte do fluxo do PeopleHub. A rota padrão aponta diretamente para `PeopleController.Index`.

## O que melhoraria com mais tempo

- Adicionar autenticação/autorização caso o sistema deixasse de ser apenas uma prova prática.
- Tratar melhor erros de API, retornando códigos/mensagens padronizadas em inglês no backend e traduzindo as mensagens no front-end.
- Adicionar rate limit como medida de segurança para reduzir abuso dos endpoints.
- Criar testes de integração com SQL Server em container.
- Adicionar tratamento global de exceções com respostas padronizadas.
- Melhorar observabilidade com logs estruturados.
- Automatizar um pipeline de CI com build, testes e análise estática.
