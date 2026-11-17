create table client
(
    id          int auto_increment
        primary key,
    companyName varchar(255) not null,
    firstName   varchar(255) not null,
    lastName    varchar(255) not null,
    email       varchar(255) not null,
    phoneNumber varchar(255) not null,
    createdAt   datetime     not null,
    updatedAT   datetime     null
);

create table module
(
    id                    int auto_increment
        primary key,
    name                  varchar(255) not null,
    description           varchar(255) null,
    priceInCents          int          not null,
    productionTimeInWeeks int          not null,
    createdAt             datetime     not null,
    updatedAt             datetime     null
);

create table role
(
    id   int auto_increment
        primary key,
    name varchar(255) not null
);

create table employee
(
    id          int auto_increment
        primary key,
    role_id     int          not null,
    firstName   varchar(255) not null,
    lastName    varchar(255) not null,
    email       varchar(255) not null,
    phoneNumber varchar(255) not null,
    password    varchar(255) not null,
    createdAt   datetime     not null,
    updatedAt   datetime     null,
    constraint role_fk
        foreign key (role_id) references role (id)
);

create table status
(
    id   int auto_increment
        primary key,
    name varchar(255) not null
);

create table deal
(
    id            int auto_increment
        primary key,
    client_id     int          not null,
    employee_id   int          not null,
    status_id     int          not null,
    waiting_role_id int        null,
    title         varchar(255) not null,
    requirement   varchar(255) not null,
    estimatedDate datetime     not null,
    createdAt     datetime     not null,
    updatedAt     datetime     null,
    constraint deal_client_id_fk
        foreign key (client_id) references client (id),
    constraint deal_employee_id_fk
        foreign key (employee_id) references employee (id),
    constraint deal_status_id_fk
        foreign key (status_id) references status (id),
    constraint deal_waiting_role_id_fk
        foreign key (waiting_role_id) references role (id)
);

create table comment
(
    id          int auto_increment
        primary key,
    employee_id int      not null,
    deal_id     int      not null,
    message     text     not null,
    createdAt   datetime not null,
    updatedAt   datetime null,
    constraint comment_employee_id_fk
        foreign key (employee_id) references employee (id),
    constraint comment_deal_id_fk
        foreign key (deal_id) references deal (id)
);

create table deal_module
(
    module_id int not null,
    deal_id   int not null,
    primary key (deal_id, module_id),
    constraint deal_module_module_id_fk
        foreign key (module_id) references module (id),
    constraint deal_module_deal_id_fk
        foreign key (deal_id) references deal (id)
);




create table deal_client
(
    deal_id   int not null,
    client_id int not null,
    primary key (deal_id, client_id),
    constraint deal_client_deal_id_fk
        foreign key (deal_id) references deal (id)
            on delete cascade,
    constraint deal_client_client_id_fk
        foreign key (client_id) references client (id)
);
