insert into role (id, name) values (1, 'Sales'), (2, 'Development');

insert into status (id, name) values (1, 'New'), (2, 'In progress'), (3, 'Won'), (4, 'Lost');

insert into employee (id, role_id, firstName, lastName, email, phoneNumber, password, createdAt)
values (1, 1, 'Milan', 'Vermeer', 'milan@example.com', '0612345678', 'dev-only', now()),
       (2, 1, 'Lotte', 'Bakker', 'lotte@example.com', '0612345679', 'dev-only', now()),
       (3, 2, 'Sanne', 'de Groot', 'sanne@example.com', '0612345680', 'dev-only', now());

insert into module (name, description, productionTimeInWeeks, createdAt)
values ('auth-gateway', 'OAuth2/OIDC gateway with session management and MFA', 4, now()),
       ('case-tracker', 'Case and status management with configurable stages', 3, now()),
       ('notification-hub', 'Email, SMS and push from one template manager', 2, now());

insert into client (id, companyName, firstName, lastName, email, phoneNumber, createdAt)
values (1, 'Gemeente Tilburg', 'Anouk', 'Jansen', 'anouk@tilburg.example', '0131234567', now()),
       (2, 'Sportbond NL', 'Daan', 'de Wit', 'daan@sportbond.example', '0307654321', now());

insert into deal (client_id, employee_id, status_id, title, requirement, estimatedDate, createdAt)
values (1, 1, 2, 'Citizen Portal 2027', 'Online portal where residents can book appointments and follow their applications.', '2026-11-15', now()),
       (2, 2, 1, 'Member portal', 'Portal for members with login and event registration.', '2026-12-20', now());

insert into deal_module (deal_id, module_id) values (1, 1), (1, 3);

insert into comment (employee_id, deal_id, message, createdAt)
values (3, 1, 'Login can reuse auth-gateway. Booking appointments needs a custom calendar module, about three weeks of work.', now()),
       (1, 1, 'Thanks. The customer is fine with a redirect for the login. I will ask about the calendar.', now());
