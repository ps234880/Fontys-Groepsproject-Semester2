create table client
(
    id          int auto_increment
        primary key,
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

create table quotation
(
    id            int auto_increment
        primary key,
    client_id     int          not null,
    employee_id   int          not null,
    title         varchar(255) not null,
    requirement   varchar(255) not null,
    estimatedDate datetime     not null,
    createdAt     datetime     not null,
    updatedAt     datetime     null,
    constraint Quotation_client_id_fk
        foreign key (client_id) references client (id),
    constraint quotation_employee_id_fk
        foreign key (employee_id) references employee (id)
);

create table comment
(
    id           int auto_increment
        primary key,
    employee_id  int      not null,
    quotation_id int      not null,
    saleOrder_id int      not null,
    message      text     not null,
    createdAt    datetime not null,
    updatedAt    datetime null,
    constraint Comment_employee_id_fk
        foreign key (employee_id) references employee (id),
    constraint Comment_quotation_id_fk
        foreign key (quotation_id) references quotation (id)
);

create table quotation_module
(
    module_id    int not null,
    quotation_id int not null,
    primary key (quotation_id, module_id),
    constraint quotation_module_module_id_fk
        foreign key (module_id) references module (id),
    constraint quotation_module_quotation_id_fk
        foreign key (quotation_id) references quotation (id)
);

create table status
(
    id   int auto_increment
        primary key,
    name varchar(255) not null
);

create table sale_order
(
    id                 int auto_increment
        primary key,
    definiteModule_id  int      not null,
    satus_id           int      not null,
    agreedPriceInCents int      not null,
    deadline           datetime not null,
    createdAt          datetime not null,
    updatedAt          datetime null,
    constraint sale_order_status_id_fk
        foreign key (satus_id) references status (id)
);

create table module_sale_order
(
    module_id     int not null,
    sale_order_id int not null,
    primary key (module_id, sale_order_id),
    constraint module_sale_order_module_id_fk
        foreign key (module_id) references module (id),
    constraint module_sale_order_sale_order_id_fk
        foreign key (sale_order_id) references sale_order (id)
);


