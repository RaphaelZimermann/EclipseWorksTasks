

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

## Testes

Para executar os testes unitários, é necessário executar a aplicação pelo
Docker, por conta da conexão com a base de dados que é gerada automaticamente.
Veja a classe 'TesteGeral' na pasta 'Tests'

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

Poderia ir bem longe nos questionamentos, mas acredito que isso seja o suficiente em um primeiro momento.

# O que eu melhoraria?

Por se tratar de um projeto apenas para o desafio, tentei simplificar várias coisas ao mesmo tempo que evitei utilizar 
ferramentas adicionais. Porém, em um projeto real de larga escala, eu levaria em 
consideração os seguintes fatores:

## Tecnologias
Utilizaria o Entity Framework com ferramentas de geração automática de código dos 
modelos e contexto, para agilizar o desenvolvimento e prevenir erros.

Muitos detalhes da regra de negócios levaria diretamente para a base de dados via TRIGGERs,
como por exemplo o limite de 20 tarefas por projeto e os campos que não
podem ser alterados após a inclusão. Também criaria funções intermediárias para
a execução da regra de negócios (não diretamente nos Controllers), para que
aja a oportunidade de programadores menos experientes participarem do projeto 
apenas montando e testando os controllers.

## Infraestrutura

Com certeza subiria a base de dados para a nuvem, como o Azure por exemplo. 
Porém, dependendo do tamanho do projeto e o orçamento, procuraria uma opção
mais em conta para o cliente e tão segura e estável como o Azure (conheço
várias);

Foi utilizado o Docker no projeto, mas eu pessoalmente prefiro opções como o
Azure Apps ou até mesmo configurar um servidor Linux especialmente para o
projeto. Tudo vai depender do tamanho do projeto, necessidade de escala,
processo de atualização e principalmente a necessidade do uso de dados, em
casos que o ideal seja gravar dados diretamente no servidor dedicado.

## Padrões

Eu sou realmente MUITO chato com padrões. Com mais tempo e em um projeto
real, eu trataria vários padrões referente a base de dados e código. Primeiro,
sobre código, pode-se ver que eu prefiro utilizar respostas padrões para
todas as chamadas de APIs. Isso facilita o entendimento dos programadores,
propicia mellhor refatoramento e reduz o tempo de desenvolvimento. Em um 
projeto que envolvesse o FrontEnd, eu definiria vários padrões para tratamento
de erros e para execução assíncrona de processos que tornasse a experiência 
do usuário mais amigável.

Uma observação, é que como pode-se ver, utilizei o padrão camelCase no retorno 
das APIs, mas obviamente outros padrões poderiam ter sido usados no começo
do projeto. Outro detalhe também é que inicialmente, eu comecei a desenvolver
o projeto usando termos em inglês, pois é o padrão que vejo mesmo no Brasil.
Porém, como isso não foi especificado, decidi dar continuidade em português.

## Visão do Projeto

Sendo bem realista e sincero, uma das minhas especialidades é gestão de 
processos e tarefas. Por conta disso, se o projeto se limitasse apenas
nas funções solicitadas, seria mais honesto indicar ao cliente soluções
já disponíveis no meracado. Porém, se o cliente chegou ao ponto de solicitar
o desenvolvimento de uma solução, significa que há algo que ele precisa
e não encontrou disponível. Em casos como esse, é muito importante entender
bem o que realmente o cliente precisa que faria diferença na vida dele, e 
explorar (mais especificamente, 'vender') soluções que otimizem de verdade
o dia dos colaboradores da empresa. Muitas vezes os clientes não sabem
que é possível desenvolver certas coisas, e cabe a nós visualizar isso
e entregar opções ao cliente.