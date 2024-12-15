

# EclipseWorks Tasks

## Pré-requisitos
- Docker + Compose
- .Net Core v9.0
- C# v13

## Instalação

Após ter clonado o respositório, execute o terminal na pasta do projeto (onde se encontra o arquivo EclipseWorksTasks.csproj);

No terminal, execute o comando abaixo:

```markdown
docker compose -f docker-compose.yml -p eclipseworkstasks up -d
```

Após o comando concluir a preparação e execução do container, acesse a API com Swagger acessando o link abaixo:

```markdown
http://localhost:8080/swagger
```

# Perguntas ao Product Owner

Bem, mesmo sendo um projeto de 'Desafio', eu questionaria os seguintes aspéctos do projeto:

- Funções de usuários: Ficou claro que há uma função de gerente, e estou acostumado a trabalhar com três níveis de função, sendo 'Colaborador', 'Gerente' e 'Administrador'. Seria a pretenção ter estes níveis?
- Como foi solicitado um projeto sem autenticação, simplifiquei os EndPoints passando direto a ID do usuário. Qual seria futuramente as principais formas de o cliente logar no sistema?
- A API se tornará pública? Caso isso ocorresse, eu sugeriria que as IDs não retornassem valores numéricos, mas sim hashes curtas para 'mascarar' as IDs tornando o processo mais seguro;
- Os prazos das tarefas poderiam no futuro ter a necessidade de ter uma hora de prazo?
- Os projetos podem necessitar no futuro de um status?
- Os relatórios de 30 dias, precisariam ser parametrizados com períodos diferentes?
- Seria apropriado desenvolver logs para todas as alterações no sistema?
- O 'cliente' poderia necessitar no futuro adicionar anexos aos projetos e/ou tarefas?
- As tarefas poderiam depender no futuro de trabalhar em equipe?

