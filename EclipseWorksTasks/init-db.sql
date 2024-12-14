CREATE TABLE IF NOT EXISTS usuarios
(
    id     int auto_increment              primary key,
    nome   varchar(100)                    not null,
    funcao enum ('colaborador', 'gerente') not null
);

CREATE TABLE IF NOT EXISTS projetos
(
    id        int auto_increment primary key,
    nome      varchar(100) not null,
    usuarioId int          not null,
    constraint projetos_ibfk_1 foreign key (usuarioId) references usuarios (id)
);

CREATE TABLE IF NOT EXISTS tarefas
(
    id             int auto_increment primary key,
    projetoId      int                                         not null,
    usuarioCriouId int                                         not null,
    titulo         varchar(100)                                not null,
    prioridade     enum ('baixa', 'média', 'alta')             not null,
    descricao      varchar(512)                                not null,
    vencimento     date                                        not null,
    status         enum ('pendente', 'andamento', 'concluída') not null,
    dataConclusao  date                                        null,
    constraint tarefas_ibfk_1
        foreign key (projetoId) references projetos (id)
            on update cascade on delete cascade,
    constraint tarefas_ibfk_2
        foreign key (usuarioCriouId) references usuarios (id)
);

CREATE TABLE IF NOT EXISTS historicosTarefas
(
    id        int auto_increment
        primary key,
    tarefaId  int                              not null,
    usuarioId int                              not null,
    dataHora  datetime                         not null,
    tipo      enum ('alteração', 'comentário') not null,
    conteudo  mediumtext                       not null,
    constraint historicosTarefas_tarefas_id_fk
        foreign key (tarefaId) references tarefas (id)
            on update cascade on delete cascade,
    constraint historicosTarefas_usuarios_id_fk
        foreign key (usuarioId) references usuarios (id)
            on update cascade
);


if not EXISTS(SELECT 0 from usuarios) THEN    
    INSERT INTO usuarios (nome, funcao) VALUES ('Karin Jantsch', 1);
    INSERT INTO usuarios (nome, funcao) VALUES ('Raphael Zimermann', 2); 
end if;

if not EXISTS(SELECT 0 from projetos) THEN
    INSERT INTO projetos (nome, usuarioId) VALUES ('Kazo Pizza Online', 1);
    INSERT INTO projetos (nome, usuarioId) VALUES ('Server Online services', 1);
    INSERT INTO projetos (nome, usuarioId) VALUES ('Kota Website', 2);
end if;

if not EXISTS(SELECT 0 from tarefas) THEN

    INSERT INTO tarefas (projetoId, usuarioCriouId, titulo, descricao, vencimento, status, dataConclusao, prioridade)    
    SELECT p.id, p.usuarioId, 'Reunião de alinhamento', 'Entrar em contato com os responsáveis e alinhar as coisas', CURRENT_DATE, 'concluída', CURRENT_DATE, 'alta' 
    from projetos p;
    
    INSERT INTO tarefas (projetoId, usuarioCriouId, titulo, descricao, vencimento, status, prioridade)    
    SELECT p.id, p.usuarioId, 'Ajustar as coisas', 'ajustar o que foi alinhado', DATE_ADD(CURRENT_DATE, INTERVAL 15 DAY), 'pendente', 'média' 
    from projetos p;
        
    INSERT INTO historicosTarefas (tarefaId, usuarioId, dataHora, tipo, conteudo) 
    SELECT t.id, p.usuarioId, current_timestamp, 'comentário', 'Este é um comentário.'  
    FROM tarefas t 
    JOIN projetos p ON t.projetoId = p.id;
        
end if;