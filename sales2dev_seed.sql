insert into role (id, name) values (1, 'Sales'), (2, 'Development');

insert into employee (id, role_id, firstName, lastName, email, phoneNumber, password, createdAt)
values (1, 1, 'Milan', 'Vermeer', 'milan@example.com', '0612345678', 'dev-only', now()),
       (2, 1, 'Lotte', 'Bakker', 'lotte@example.com', '0612345679', 'dev-only', now()),
       (3, 2, 'Sanne', 'de Groot', 'sanne@example.com', '0612345680', 'dev-only', now());

insert into module (name, description, priceInCents, productionTimeInWeeks, createdAt)
values ('auth-gateway', 'OAuth2/OIDC gateway with session management and MFA', 1250000, 4, now()),
       ('case-tracker', 'Case and status management with configurable stages', 980000, 3, now()),
       ('notification-hub', 'Email, SMS and push from one template manager', 640000, 2, now());

insert into client (id, companyName, firstName, lastName, email, phoneNumber, createdAt)
values (1, 'Gemeente Tilburg', 'Anouk', 'Jansen', 'anouk@tilburg.example', '0131234567', now()),
       (2, 'Sportbond NL', 'Daan', 'de Wit', 'daan@sportbond.example', '0307654321', now());

insert into quotation (client_id, employee_id, title, description, estimatedDate, createdAt)
values (1, 1, 'Citizen Portal 2027', 'Online portal where residents can book appointments and follow their applications.', '2026-11-15', now()),
       (2, 2, 'Member portal', 'Portal for members with login and event registration.', '2026-12-20', now());
