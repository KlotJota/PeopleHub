# PeopleHub

Aplicação simples para cadastro e gerenciamento de pessoas, desenvolvida como etapa prática de processo seletivo. O foco do projeto é demonstrar organização de código, separação de responsabilidades, validações de negócio e uso da stack solicitada: ASP.NET MVC, Entity Framework, SQL Server e Knockout.js.

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

## Como Executar

Suba o SQL Server:

```bash
docker compose up -d
```

Configure as variáveis no arquivo `.env` com base em `.env.example`.

Rode a aplicação:

```bash
dotnet run --project PeopleHub.Web
```

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

A aplicação foi mantida propositalmente simples, mas com responsabilidades separadas. O controller recebe as requisições HTTP e traduz o resultado da aplicação para status codes. O service concentra as regras de negócio, como idade mínima e unicidade. O repository encapsula as consultas e comandos com Entity Framework.

Os endpoints usam DTOs para entrada de dados, evitando acoplamento direto entre o contrato externo da API e a entidade persistida. O serviço retorna um `PersonOperationResult` tipado, com status explícito para sucesso, validação, conflito e não encontrado. Isso evita decisões baseadas em texto de mensagem e deixa o fluxo mais previsível.

As restrições de unicidade de CPF e e-mail foram aplicadas também no banco via índices únicos. A validação em memória melhora a experiência do usuário, mas a constraint no banco protege a integridade dos dados.

O soft delete foi escolhido para preservar histórico e evitar remoção física imediata. A listagem padrão ignora registros arquivados.

## O Que Melhoraria Com Mais Tempo

- Separar a solução em camadas/projetos dedicados, como `Domain`, `Application`, `Infrastructure` e `Web`.
- Adicionar validação real de CPF.
- Criar testes de integração com SQL Server em container.
- Adicionar tratamento global de exceções com respostas padronizadas.
- Melhorar observabilidade com logs estruturados.
- Adicionar autenticação/autorização caso o sistema deixasse de ser apenas uma prova prática.
- Automatizar migrations em um fluxo controlado para ambientes reais.
